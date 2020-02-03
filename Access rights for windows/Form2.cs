using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication9
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 F3= new Form3();
            F3.Owner = this;
            F3.ShowDialog();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Form4 F4 = new Form4();
            F4.Owner = this;
            F4.ShowDialog();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
