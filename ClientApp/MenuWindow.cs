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
        public bool canalOpened;
        Thread receiver;
        public MenuWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;
            canalOpened = false;
            InitializeComponent();
            receiver = new Thread(receive);
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
                    if (!canalOpened)
                    {
                        text = connectionController.readText();
                        printInLogger(text);
                    }
                }
                catch(Exception)
                {

                }
            }
        }

        public void closeCanal()
        {
            canalOpened = false;
            connectionController.sendText("//leave");
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
              
    
                if (comms[0] == "switchto")
                {

                    try
                    {
                        canalOpened = true;
                        var frm = new CanalWindow(connectionController, comms[1]);
                        frm.Location = this.Location;
                        frm.StartPosition = FormStartPosition.Manual;
                        frm.FormClosing += delegate { this.Show(); };
                        frm.Show();
                        this.Hide();
                    }
                    catch(IndexOutOfRangeException)
                    { }

                }

                commandBox.Text = string.Empty;
                e.Handled = true;
                e.SuppressKeyPress = true;
              
            }
        }

        private void MenuWindow_Shown(object sender, EventArgs e)
        {
        }

        private void MenuWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            receiver.Abort();
            connectionController.disconnectClient();
        }
    }
}
