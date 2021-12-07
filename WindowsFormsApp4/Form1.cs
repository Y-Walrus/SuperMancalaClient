﻿using System;
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
        //class Form1 inherits from Form
        //Form1 is responsible of the game screen

        private TcpClient client; //The client defined in the main program
        private NetworkStream stream; //The stream defined in the main program
        private Label[] boardLabels; //Array of the labels that make up the game board

        public Form1(TcpClient client,NetworkStream stream)
        {
            //The contractor of the game screen
            //arg: client, stream

            InitializeComponent();

            Label[] boardLabels = { this.hole1,this.hole2,this.hole3,this.hole4,
                this.hole5,this.hole6,this.hole7,this.hole8,this.hole9,this.hole10,
                this.hole11,this.hole12,this.hole13,this.labelTitle};

            this.boardLabels = boardLabels;
            this.client = client;
            this.stream = stream;

            //Game();
        }

        private void Game()
        {
            //Performs backend communication to update the board

            Byte[] data = new Byte[256]; //the response ASCII representation

            // Read the first batch of the TcpServer response bytes
            Int32 bytes = stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            //A loop that lasts until the end of the game and updates the board
            while (responseData != "LOSS" && responseData != "WIN")
            {
                responseData.Replace("[", "");
                responseData.Replace("]", "");
                string[] update = responseData.Split(' ');
                string[] updateBoardLabels = new string[update.Length - 1];

                for (int i = 0; i < updateBoardLabels.Length; i++)
                {
                    updateBoardLabels[i] = update[i][0] + "";
                }

                UpdateBoard(updateBoardLabels, update[update.Length - 1]);

                data = new Byte[256]; //the response ASCII representation

                // Read the first batch of the TcpServer response bytes
                bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            }

            //If the user wins changes the screen title to "YOU WON!", else "YOU LOST:("
            if (responseData == "WIN")
                labelTitle.Text = "YOU WON!";
            else
                labelTitle.Text = "YOU LOST :(";
        }

        private void UpdateBoard(string [] updateArr,string isMyTurn, int choice=0)
        {
            //Updating the board
            //arg: updateArr, isMyTurn, choice
            //updateArr contains the new values of the holes in the board
            //isMyTurn true if it's the user's turn, else false
            //choice stores the hole number from which the turn was made

            //Marks who is playing
            if (isMyTurn=="true")
            {
                this.checkBoxMe.Checked = true;
                this.checkBoxOpponent.Checked = false;
            }
            else
            {
                this.checkBoxMe.Checked = false;
                this.checkBoxOpponent.Checked = true;
            }

            //Returns all holes to their original color
            for (int i=0;i<boardLabels.Length;i++)
            {
                this.boardLabels[i].BackColor = Color.Tan;
            }

            //Marks the hole number from which the turn was made
            boardLabels[choice].BackColor = Color.Tomato;

            for (int i = 0; i < boardLabels.Length; i++)
            {
                boardLabels[i].Text = updateArr + "";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //Closes the entire program by clicking on the X
            //arg: e

            //this.stream.Close();
            //this.client.Close();
            Application.Exit();
        }

        private void returnToJoinstartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Returns to the previous screen by clicking on this option in the menu
            //arg: sender, e

            Form3 f = new Form3(client,stream);
            f.Location = this.Location;
            f.Show();
            this.Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Closes the entire program by clicking on this option in the menu

            //this.stream.Close();
            //this.client.Close();
            Application.Exit();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Requests a new game and updates the screen accordingly.

            //Returns the board to its original position
            for (int i=0;i<this.boardLabels.Length;i++)
            {
                if (i == 0 || i == 7)
                    this.boardLabels[i].Text = "0";
                else this.boardLabels[i].Text = "4";
            }

            this.checkBoxMe.Checked = false;
            this.checkBoxOpponent.Checked = false;
            this.labelTitle.Text = "GAME STARTED";

            //Byte[] data = System.Text.Encoding.ASCII.GetBytes("restart");
            //stream.Write(data, 0, data.Length);

            //Game();
        }
    }
}
