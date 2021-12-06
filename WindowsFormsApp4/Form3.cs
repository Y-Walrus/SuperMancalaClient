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
        private Form mainScreen;
        private NetworkStream stream;
        private TcpClient client;

        public Form3(Form mainScreen,TcpClient client,NetworkStream stream)
        {
            InitializeComponent();
            this.mainScreen = mainScreen;
            this.client = client;
            this.stream = stream;
        }

        private void startGameBtn_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1(mainScreen,client,stream);
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.stream.Close();
            this.client.Close();
            this.mainScreen.Close();
        }

        private void joinBtn_Click(object sender, EventArgs e)
        {
            string gameID = textBox1.Text;
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("join "+gameID);

            stream.Write(data, 0, data.Length);
            Form1 f = new Form1(mainScreen,client,stream);
            f.Location = this.Location;
            f.Show();
            this.Close();
        }
    }
}
