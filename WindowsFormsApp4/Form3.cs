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
        //class Form3 inherits from Form
        //Form3 is responsible of the choose between join or start screen

        private NetworkStream stream; //The client defined in the main program
        private TcpClient client; //The stream defined in the main program

        public Form3(TcpClient client,NetworkStream stream)
        {
            //The contractor of the choosing screen
            //arg: client, stream

            InitializeComponent();
            this.client = client;
            this.stream = stream;
        }

        private void startGameBtn_Click(object sender, EventArgs e)
        {
            //Asks to start a game from the server and moves to the game screen when the 
            //"Start Game" button is pressed 

            string gameID = textBox1.Text;
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("start");
            stream.Write(data, 0, data.Length);

            Form1 f = new Form1(client,stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }

        private void joinGameBtn_Click(object sender, EventArgs e)
        {
            //Displays text box for entering the game ID and the "Join!" button for it

            //Hides the choosing buttons
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
            //Closes the entire program by clicking on the X
            //arg: e

            this.stream.Close();
            this.client.Close();
            Application.Exit();
        }

        private void joinBtn_Click(object sender, EventArgs e)
        {
            //Joinig a game when the "Join!" button is pressed

            string gameID = textBox1.Text;
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("join "+gameID);
            stream.Write(data, 0, data.Length);

            Form1 f = new Form1(client,stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Closes the entire program by clicking on this option in the menu

            this.stream.Close();
            this.client.Close();
            Application.Exit();
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Returns to the previous screen by clicking on this option in the menu
            //arg: sender, e

            Byte[] data = System.Text.Encoding.ASCII.GetBytes("logout");
            stream.Write(data, 0, data.Length);

            Form2 f = new Form2(this.client, this.stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1(client, stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }
    }
}
