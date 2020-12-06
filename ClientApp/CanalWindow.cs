using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class CanalWindow : Form
    {
        delegate void SafeCallDelegate(string text);
        ConnectionController connectionController;
        MenuWindow frmMain = (MenuWindow)Application.OpenForms["MenuWindow"];
        public CanalWindow()
        {
            InitializeComponent();
        }

        public CanalWindow(ConnectionController connectionController, string canalName)
        {
          
            connectionController.sendText("switchto " + canalName);
            this.connectionController = connectionController;
            InitializeComponent();
            Thread receiver = new Thread(receive);
            receiver.Start();
            CanalName.Text = canalName;
        }
       
        private void print(string text)
        {
            if (messagesBox.InvokeRequired)
            {
                var d = new SafeCallDelegate(print);
                messagesBox.Invoke(d, new object[] { text });
            }
            else
            {
                messagesBox.AppendText(text);
            }
        }

        private void receive()
        {
            string text;
            connectionController.setTimeout(100);
            while (true)
            {
                try
                {
                    if (frmMain.canalOpened)
                    {
                        text = connectionController.readText();
                        print(text);
                    }
                }
                catch (Exception)
                {

                }
            }
        }


        

            private void LeaveButton_Click(object sender, EventArgs e)
        {
            frmMain.closeCanal();
            this.Close();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {

                string command = UserInput.Text;
                connectionController.sendText(UserInput.Text);
                string[] comms = command.Split();
             
             
                 print(connectionController.getUsername() + ": " + command + "\r\n");
            

                UserInput.Text = string.Empty;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
