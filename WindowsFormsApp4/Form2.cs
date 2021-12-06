using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();

            string name=textBox1.Text;

            //if(login successful)
            //{
            //    f.Show();
            //    this.Hide();
            //}
            //else
            //{
            //    label3.Text = "Login Error";
            //}
            
        }
    }
}
