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
    public partial class Form2 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        public Form2(TcpClient client,NetworkStream stream)
        {
            InitializeComponent();
            this.client = client;
            this.stream = stream;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //this.client.Close();
            //this.stream.Close();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            //Byte[] data = System.Text.Encoding.ASCII.GetBytes(login name);
            //stream.Write(data, 0, data.Length);

            Form3 f = new Form3(this.client,this.stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }
    }
}
