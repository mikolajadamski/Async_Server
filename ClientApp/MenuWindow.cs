using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class MenuWindow : Form
    {
        delegate void SafeCallDelegate(string text);
        ConnectionController connectionController;
        bool canalOpened;
        public MenuWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;
            canalOpened = false;
            InitializeComponent();
            Thread receiver = new Thread(receive);
            receiver.Start();
        }
        private void receive()
        {
            string text;
            connectionController.setTimeout(100);
            while(true)
            {
                try
                {
                    text = connectionController.readText();
                    printInLogger(text);
                }
                catch(Exception)
                {

                }
            }
        }
        private void printInLogger(string text)
        {
            if(loggerBox.InvokeRequired)
            {
                var d = new SafeCallDelegate(printInLogger);
                loggerBox.Invoke(d, new object[] { text });
            }
            else
            {
                loggerBox.AppendText(text);
            }
        }
        private void MenuWindow_Load(object sender, EventArgs e)
        {

        }
        private void commandBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                
                string command = commandBox.Text;
                connectionController.sendText(commandBox.Text);
                string[] comms = command.Split();
                if (comms[0] == "//leave")
                    canalOpened = false;
                if (canalOpened)
                    printInLogger(connectionController.getUsername() +": "+ command + "\r\n");
                if (comms[0] == "switchto")
                    canalOpened = true;

                commandBox.Text = string.Empty;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
