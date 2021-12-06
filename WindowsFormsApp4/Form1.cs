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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            



        }

        private void UpdateBoard(int [] updateArr,bool isMyTurn,Label [] boardLabels, int choice)
        {
            Label[] boardLables = { this.label1,this.label2,this.label3,this.label4,
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

        private void returnToJoinstartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.Location = this.Location;
            f.Show();
            this.Close();
        }
    }
}
