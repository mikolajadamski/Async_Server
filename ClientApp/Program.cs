using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConnectionController clientController = new ConnectionController();
            if (clientController.initializeConnection())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LoginWindow(clientController));
                if(clientController.IsLogged)
                {
                    Application.Run(new MenuWindow(clientController));
                    clientController.disconnectClient();
                }
            }
        }
    }
}
