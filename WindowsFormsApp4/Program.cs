using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;


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
            Form2 f2 = new Form2(stream);
            Application.Run(f2);

            Int32 port = 13000;
            string host = "127.0.0.1";
            TcpClient client = new TcpClient(host, port);
            NetworkStream stream = client.GetStream();
        }
    }
}
