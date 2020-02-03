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
    public partial class Form3 : Form
    {
        FileSystemRights READ = FileSystemRights.Read | FileSystemRights.Synchronize;
        FileSystemRights WRITE = FileSystemRights.Write | FileSystemRights.Synchronize;
        FileSystemRights RAW = FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.Synchronize;
        FileSystemRights MODIFY = FileSystemRights.Modify | FileSystemRights.Synchronize;
        public Form3()
        {
            InitializeComponent();

            var usersSearcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_UserAccount");
            var users = usersSearcher.Get();
            WqlObjectQuery query = new WqlObjectQuery("Select * from Win32_Group where LocalAccount = 'true'");
            ManagementObjectSearcher find = new ManagementObjectSearcher(query);
            listBox1.Items.Add("Список пользователей");
            listBox1.Items.Add("____________");

            foreach (var child in users)
            {
                
                listBox1.Items.Add(child["Name"]);
            }
            
            listBox1.Items.Add(" ");
            listBox1.Items.Add("Список групп");
            listBox1.Items.Add("____________");
            foreach (ManagementObject mo in find.Get())
            {
                
                listBox1.Items.Add(mo["Name"]);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            listBox2.DataSource=null;
            string name = Convert.ToString(listBox1.SelectedItem);
            textBox1.Text = name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = null;
            using (var dialog = new FolderBrowserDialog())
                if (dialog.ShowDialog() == DialogResult.OK)
                    path = dialog.SelectedPath;
            textBox2.Text = path;
            
        }

        private void button2_Click(object sender, EventArgs e)
        { bool flag = false;
            string name = Convert.ToString(listBox1.SelectedItem);
            if (textBox1.Text == "")
            {
                string Mesg = " Субъект не выбран. Будет показан список объектов доступных для каждого субъекта.";
                string Title = "Предупреждение";
                MessageBoxIcon MesIcon = MessageBoxIcon.Warning;
                MessageBoxButtons MesBut = MessageBoxButtons.OK;

                DialogResult DiRes = MessageBox.Show(Mesg, Title, MesBut, MesIcon);
                
            }
            else
            {
                if (textBox1.Text != name)
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
                        string Mesg = " Такого субъекта не существует.";
                        string Title = "Ошибка";
                        MessageBoxIcon MesIcon = MessageBoxIcon.Error;
                        MessageBoxButtons MesBut = MessageBoxButtons.OK;

                        DialogResult DiRes = MessageBox.Show(Mesg, Title, MesBut, MesIcon);

                    }
                }
            }
            string path = textBox2.Text;
            bool folderExists = Directory.Exists((path));
            if (!folderExists)
            {

                {
                    string Mesg = " Такого каталога не существует.";
                    string Title = "Ошибка";
                    MessageBoxIcon MesIcon = MessageBoxIcon.Error;
                    MessageBoxButtons MesBut = MessageBoxButtons.OK;

                    DialogResult DiRes = MessageBox.Show(Mesg, Title, MesBut, MesIcon);
                    return;
                }

            }

                List<string> list = new List<string>();
                //listBox2.Items.Clear();
                bool flag1 = false;
                bool flag2 = false;
                DirectoryInfo DI = new DirectoryInfo(textBox2.Text);
                FileInfo[] allfls = DI.GetFiles();


                foreach (FileInfo file in allfls)
            
                {
                try
                {
                    FileSecurity fs = file.GetAccessControl();


                    foreach (FileSystemAccessRule FSAR in fs.GetAccessRules(true, true, typeof(NTAccount)))
                    {
                        if (FSAR.FileSystemRights == WRITE)
                            flag2 = true;
                        if (FSAR.FileSystemRights == READ)
                            flag1 = true;
                        string buf = FSAR.IdentityReference.ToString();
                        if (radioButton1.Checked == true)
                        {

                            if (FSAR.FileSystemRights == READ | FSAR.FileSystemRights == FileSystemRights.FullControl | FSAR.FileSystemRights == MODIFY && buf.Contains(textBox1.Text))
                                list.Add(file.Name + "  доступен для чтения  " + FSAR.IdentityReference);
                            listBox2.DataSource = list.Distinct().ToList();
                            // listBox2.Items.Add(file.Name + "  " + FSAR.FileSystemRights + "  " + FSAR.IdentityReference);
                            
                        }
                        if (radioButton2.Checked == true)
                        {
                            if (FSAR.FileSystemRights == WRITE | FSAR.FileSystemRights == FileSystemRights.FullControl | FSAR.FileSystemRights == MODIFY && buf.Contains(textBox1.Text))
                                list.Add(file.Name + "  доступен на запись для  " + FSAR.IdentityReference);
                            listBox2.DataSource = list.Distinct().ToList();
                            //listBox2.Items.Add(file.Name + "  " + FSAR.FileSystemRights + "  " + FSAR.IdentityReference);
                        }
                        if (radioButton3.Checked == true)
                        {
                            if ((flag1 == true && flag2 == true) | FSAR.FileSystemRights == RAW | FSAR.FileSystemRights == MODIFY | FSAR.FileSystemRights == FileSystemRights.FullControl && buf.Contains(textBox1.Text))

                                list.Add(file.Name + " доступен на запись и чтение для  " + FSAR.IdentityReference);
                            listBox2.DataSource = list.Distinct().ToList();


                        }
                       
                    }
                }
                catch { }
                }
            }
        

        private void button3_Click(object sender, EventArgs e)
        {
            bool flag = false;
            string name = Convert.ToString(listBox1.SelectedItem);
            if (textBox1.Text == "")
            {
                MessageBox.Show(" Субъект не выбран. Будет показан список объектов доступных для каждого субъекта.");
            }
            else
            {
                if (textBox1.Text != name)
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
            string path = textBox2.Text;
            bool folderExists = Directory.Exists((path));
            if (!folderExists)
            {

                {
                    string Mesg = " Такого каталога не существует.";
                    string Title = "Ошибка";
                    MessageBoxIcon MesIcon = MessageBoxIcon.Error;
                    MessageBoxButtons MesBut = MessageBoxButtons.OK;

                    DialogResult DiRes = MessageBox.Show(Mesg, Title, MesBut, MesIcon);
                    return;
                }

            }
            List<string> list1 = new List<string>();
                //listBox3.Items.Clear();
                DirectoryInfo DI = new DirectoryInfo(textBox2.Text);
                DirectoryInfo[] alldir = DI.GetDirectories();



            foreach (DirectoryInfo folder in alldir)
            {
                try {
                    DirectorySecurity ds = folder.GetAccessControl();
                    bool flag1 = false;
                    bool flag2 = false;

                    foreach (FileSystemAccessRule FSAR in ds.GetAccessRules(true, true, typeof(NTAccount)))
                    {

                        if (FSAR.FileSystemRights == WRITE)
                            flag2 = true;
                        if (FSAR.FileSystemRights == READ)
                            flag1 = true;

                        string buf = FSAR.IdentityReference.ToString();
                        if (radioButton1.Checked == true)
                        {
                            if (FSAR.FileSystemRights == READ | FSAR.FileSystemRights == FileSystemRights.FullControl | FSAR.FileSystemRights == MODIFY && buf.Contains(textBox1.Text))
                                //listBox3.Items.Add(folder.Name + "  " + FSAR.FileSystemRights + "  " + FSAR.IdentityReference);
                                list1.Add(folder.Name + "  доступен для чтения  " + FSAR.IdentityReference);
                            listBox2.DataSource = list1.Distinct().ToList();
                        }
                        if (radioButton2.Checked == true)
                        {
                            if (FSAR.FileSystemRights == WRITE | FSAR.FileSystemRights == FileSystemRights.FullControl | FSAR.FileSystemRights == MODIFY && buf.Contains(textBox1.Text))
                                //  listBox3.Items.Add(folder.Name + "  " + FSAR.FileSystemRights + "  " + FSAR.IdentityReference);
                                list1.Add(folder.Name + "  доступен на запись для  " + FSAR.IdentityReference);
                            listBox2.DataSource = list1.Distinct().ToList();
                        }
                        if (radioButton3.Checked == true)
                        {
                            if ((flag1 == true && flag2 == true) | FSAR.FileSystemRights == RAW | FSAR.FileSystemRights == MODIFY | FSAR.FileSystemRights == FileSystemRights.FullControl && buf.Contains(textBox1.Text))
                                list1.Add(folder.Name + " доступен на запись и чтение для  " + FSAR.IdentityReference);
                            listBox2.DataSource = list1.Distinct().ToList();
                            //listBox3.Items.Add(folder.Name + "  " + FSAR.FileSystemRights + "  " + FSAR.IdentityReference);

                        }
                    }
                }
                catch { }
                }
            }
        
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
            {
                button2.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FAQ F = new FAQ();
            F.ShowDialog();
        }
    }
}
