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

        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    this.Close();
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3(this);
            f.Location = this.Location;
            f.Show();
            this.Hide();

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
