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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void startGameBtn_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Location = this.Location;
            f.Show();
            this.Close();
        }

        private void joinGameBtn_Click(object sender, EventArgs e)
        {
            startGameBtn.Enabled = false;
            joinGameBtn.Enabled = false;
            startGameBtn.Hide();
            joinGameBtn.Hide();

            joinBtn.Enabled = true;
            joinBtn.Visible = true;
            textBox1.Enabled = true;
            textBox1.Visible = true;
        }

        private void joinBtn_Click(object sender, EventArgs e)
        {
            string gameID = textBox1.Text;
            Form1 f = new Form1();
            f.Location = this.Location;
            f.Show();
            this.Close();
        }
    }
}
