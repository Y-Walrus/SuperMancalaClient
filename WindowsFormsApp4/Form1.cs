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
        
        private TcpClient client;
        private NetworkStream stream;

        public Form1(TcpClient client,NetworkStream stream)
        {
            InitializeComponent();

            
            this.checkBox1.ForeColor = Color.Black;
            this.checkBox2.ForeColor = Color.Black;
            
            

            this.client = client;
            this.stream = stream;


            //// String to store the response ASCII representation.
            //Byte[] data = new Byte[256];

            //// Read the first batch of the TcpServer response bytes.
            //Int32 bytes = stream.Read(data, 0, data.Length);
            //string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            //while (responseData != "LOSS" && responseData != "WIN")
            //{
            //    responseData.Replace("[", "");
            //    responseData.Replace("]", "");
            //    string[] update = responseData.Split(' ');
            //    string[] updateBoardLabels = new string[update.Length - 1];
            //    for (int i = 0; i < updateBoardLabels.Length; i++) 
            //    {
            //        updateBoardLabels[i] = update[i][0]+"";
            //    }
            //    UpdateBoard(updateBoardLabels, update[update.Length - 1]);

            //    data = new Byte[256];

            //    // Read the first batch of the TcpServer response bytes.
            //    bytes = stream.Read(data, 0, data.Length);
            //    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //}
            //if(responseData=="WIN")
            //    label14.Text = "YOU WON!";
            //else
            //    label14.Text = "YOU LOST :(";
            
        }

        private void UpdateBoard(string [] updateArr,string isMyTurn, int choice=0)
        {
            Label[] boardLabels = { this.label1,this.label2,this.label3,this.label4,
                this.label5,this.label6,this.label7,this.label8,this.label9,this.label10,
                this.label11,this.label12,this.label13,this.label14};

            if (isMyTurn=="true")
            {
                this.checkBox1.Checked = true;
                this.checkBox2.Checked = false;
            }
            else
            {
                this.checkBox1.Checked = false;
                this.checkBox2.Checked = true;
            }

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
            //this.stream.Close();
            //this.client.Close();
            Application.Exit();
        }

        private void returnToJoinstartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3(client,stream);
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

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Byte[] data = System.Text.Encoding.ASCII.GetBytes("restart");
            //stream.Write(data, 0, data.Length);
        }
    }
}
