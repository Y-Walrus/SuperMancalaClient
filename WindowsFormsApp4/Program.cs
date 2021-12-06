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



            //Int32 port = 43000;
            //string host = "127.0.0.0";


            TcpClient client = null;//new TcpClient(host, port);
            NetworkStream stream = null;//client.GetStream();

            Form2 f2 = new Form2(client,stream);
            Application.Run(f2);
        }
    }
}
