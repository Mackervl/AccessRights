using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.DirectoryServices;
using System.Management;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace WindowsFormsApplication9
{
    public partial class Form4 : Form
    {
        RegistryKey RK;
        public Form4()
        {
            InitializeComponent();
            TreeInit(Registry.CurrentUser);
            TreeInit(Registry.LocalMachine);
            TreeInit(Registry.ClassesRoot);
            TreeInit(Registry.Users);
            TreeInit(Registry.CurrentConfig);

            var usersSearcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_UserAccount");
            var users = usersSearcher.Get();
            WqlObjectQuery query = new WqlObjectQuery("Select * from Win32_Group where LocalAccount = 'true'");
            ManagementObjectSearcher find = new ManagementObjectSearcher(query);
            listBox1.Items.Add(" Список пользователей");
            listBox1.Items.Add("____________");

            foreach (var child in users)
            {

                listBox1.Items.Add(child["Name"]);
            }

            listBox1.Items.Add("");
            listBox1.Items.Add("Список групп");
            listBox1.Items.Add("____________");
            foreach (ManagementObject mo in find.Get())
            {

                listBox1.Items.Add(mo["Name"]);
            }
        }
        private void TreeInit(RegistryKey RK)
        {
            TreeNode tn = new TreeNode(RK.Name);
            tn.Tag = RK;
            tn.Nodes.Add("Fake");
            treeView1.Nodes.Add(tn);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            bool flag = false;
            string name1 = Convert.ToString(listBox1.SelectedItem);
            if (textBox1.Text == "")
            {
                MessageBox.Show(" Субъект не выбран. Будет показан список объектов доступных для каждого субъекта.");
            }
            else
            {
                if (textBox1.Text != name1)
                {
                    string[] s;
                    s = new string[listBox1.Items.Count - 1];
                    for (int i = 0; i < listBox1.Items.Count - 1; i++)
                    {

                        if (listBox1.Items.ToString() != "")
                            s[i] = listBox1.Items[i].ToString();
                        if (textBox1.Text == s[i]) flag = true;
                    }
                    if (flag != true || textBox1.Text != "Список групп" || textBox1.Text != "Список пользователей" || textBox1.Text != "____________")
                    {
                        MessageBox.Show(" Такого субъекта не существует.");
                    }
                }
            }
           

            listBox2.DataSource = null;
            RegistryRights WriteAndRead = RegistryRights.ReadKey | RegistryRights.SetValue | RegistryRights.CreateSubKey; //== writekey and readkey
            List<RegistryKey> RKMas = new List<RegistryKey>();
            List<string> list = new List<string>();


            foreach (string name in RK.GetSubKeyNames())
            {
                try
                {
                   
                    RKMas.Add(RK.OpenSubKey(name));
                 
                }
                catch { }
                
            }
            foreach (RegistryKey rk in RKMas)
            {

                RegistrySecurity rs = rk.GetAccessControl();
                foreach (RegistryAccessRule ar in rs.GetAccessRules(true, true, typeof(NTAccount)))
                {

                    string buf = ar.IdentityReference.ToString();
                    if (radioButton1.Checked == true)
                    {
                        if (ar.RegistryRights == RegistryRights.ReadKey | ar.RegistryRights == RegistryRights.FullControl | ar.RegistryRights == WriteAndRead && buf.Contains(textBox1.Text))
                            //listBox2.Items.Add(rk + " доступен на чтение для " + ar.IdentityReference);
                            list.Add(rk + " доступен для чтения  " + ar.IdentityReference);
                        listBox2.DataSource = list.Distinct().ToList();
                    }
                    if (radioButton2.Checked == true)
                    {
                        if (ar.RegistryRights == RegistryRights.WriteKey | ar.RegistryRights == RegistryRights.FullControl | ar.RegistryRights == WriteAndRead && buf.Contains(textBox1.Text))
                            //listBox2.Items.Add(rk +  " доступен на запись для "  + ar.IdentityReference);
                            list.Add(rk + " доступен на запись для " + ar.IdentityReference);
                        listBox2.DataSource = list.Distinct().ToList();
                    }
                    if (radioButton3.Checked == true)
                    {

                        if (ar.RegistryRights == WriteAndRead | ar.RegistryRights == RegistryRights.FullControl && buf.Contains(textBox1.Text))

                            //listBox2.Items.Add(rk + " доступен на чтение и запись для " + ar.IdentityReference);
                            list.Add(rk + " доступен на чтение и запись для  " + ar.IdentityReference);
                        listBox2.DataSource = list.Distinct().ToList();


                    }
                }
            }
        }

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = e.Node;
            tn.Nodes.Clear();
            tn.Nodes.Add("Fake");
        }

       

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            listBox2.Items.Clear();
            string name = Convert.ToString(listBox1.SelectedItem);
            textBox1.Text = name;
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode tn = e.Node;
            tn.Nodes.Clear();

            RegistryKey RK = (RegistryKey)tn.Tag;

            foreach (string skeyName in RK.GetSubKeyNames())
            {
                TreeNode NewNode = new TreeNode(skeyName);

                try
                {
                    NewNode.Tag = RK.OpenSubKey(skeyName);
                    if (RK.OpenSubKey(skeyName).SubKeyCount != 0)
                        NewNode.Nodes.Add("Fake");
                }
                catch
                { }
                tn.Nodes.Add(NewNode);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FAQ F = new FAQ();
            F.ShowDialog();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            RK = (RegistryKey)e.Node.Tag;
            textBox2.Text = Convert.ToString(e.Node.Tag);
            button1.Enabled = true;
        }
    }
}
