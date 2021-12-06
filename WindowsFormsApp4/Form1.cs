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
    public partial class Form1 : Form
    {
        private Form mainScreen;
        private TcpClient client;
        private NetworkStream stream;

        public Form1(Form mainScreen,TcpClient client,NetworkStream stream)
        {
            InitializeComponent();
            this.mainScreen = mainScreen;
            this.client = client;
            this.stream = stream;



            // String to store the response ASCII representation.
            Byte[] data = new Byte[256];

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            while (responseData!="LOSS"&&responseData!="WIN")
            {
                responseData.Replace("[", "");
                responseData.Replace("]", "");
                string[] update = responseData.Split(' ');
                UpdateBoard()
            }
        }

        private void UpdateBoard(string [] updateArr,string isMyTurn, int choice)
        {
            Label[] boardLabels = { this.label1,this.label2,this.label3,this.label4,
                this.label5,this.label6,this.label7,this.label8,this.label9,this.label10,
                this.label11,this.label12,this.label13,this.label14};

            for(int i=0;i<boardLabels.Length;i++)
            {
                boardLabels[i].BackColor = Color.Tan;
            }

            boardLabels[choice].BackColor = Color.Tomato;

            for (int i = 0; i < boardLabels.Length; i++)
            {
                boardLabels[i].Text = updateArr + "";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.client.Close();
            this.stream.Close();
            this.mainScreen.Close();
        }

        private void returnToJoinstartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3(this,client,stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainScreen.Close();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("restart");
            stream.Write(data, 0, data.Length);
        }
    }
}
