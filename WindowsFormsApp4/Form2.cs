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
        //class Form2 inherits from Form
        //Form2 is responsible of the login screen

        private TcpClient client; //The client defined in the main program
        private NetworkStream stream; //The stream defined in the main program

        public Form2(TcpClient client,NetworkStream stream)
        {
            //The contractor of the login screen
            //arg: client, stream

            InitializeComponent();
            this.client = client;
            this.stream = stream;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //Closes the entire program by clicking on the X
            //arg: e

            //this.client.Close();
            //this.stream.Close();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Logins to the server when the "Login" button is pressed and moves 
            //to the next screen
            //arg: sender, e

            string name = textBox1.Text;
            //Byte[] data = System.Text.Encoding.ASCII.GetBytes("login "+name);
            //stream.Write(data, 0, data.Length);

            Form3 f = new Form3(this.client,this.stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }
    }
}
