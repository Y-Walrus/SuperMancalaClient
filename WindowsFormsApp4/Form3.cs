using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;


namespace WindowsFormsApp4
{
    public partial class Form3 : Form
    {
        private NetworkStream stream;
        private TcpClient client;

        public Form3(TcpClient client,NetworkStream stream)
        {
            InitializeComponent();
            this.client = client;
            this.stream = stream;
        }

        private void startGameBtn_Click(object sender, EventArgs e)
        {
            //string gameID = textBox1.Text;
            //Byte[] data = System.Text.Encoding.ASCII.GetBytes("start");

            //stream.Write(data, 0, data.Length);
            Form1 f = new Form1(client,stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //this.stream.Close();
            //this.client.Close();
            Application.Exit();
        }

        private void joinBtn_Click(object sender, EventArgs e)
        {
            //string gameID = textBox1.Text;
            //Byte[] data = System.Text.Encoding.ASCII.GetBytes("join "+gameID);

            //stream.Write(data, 0, data.Length);
            Form1 f = new Form1(client,stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.stream.Close();
            //this.client.Close();
            Application.Exit();
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Byte[] data = System.Text.Encoding.ASCII.GetBytes("logout");
            //stream.Write(data, 0, data.Length);

            Form2 f = new Form2(this.client, this.stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }
    }
}
