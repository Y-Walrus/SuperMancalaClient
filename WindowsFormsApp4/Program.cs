//Program: Mancala GUI
//Authors: Yegor Stolyarsky, Ron Fefer & Ben Solomovitch
//The program enables a GUI for the game Mancala with backend communication  
//for requests to login, logout, start a new game, join an existing game and more

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace WindowsFormsApp4
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Int32 port = 21257;
            string host = "127.0.0.1";

            TcpClient client = new TcpClient(host, port);
            NetworkStream stream = client.GetStream();

            
            //Opens a login screen
            Form2 f2 = new Form2(client, stream);
            Application.Run(f2);
        }
    }
}
