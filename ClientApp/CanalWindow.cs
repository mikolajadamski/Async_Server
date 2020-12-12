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
        Thread receiver;
        MenuWindow frmMain = (MenuWindow)Application.OpenForms["MenuWindow"];
        public CanalWindow()
        {
            InitializeComponent();
        }

        public CanalWindow(ConnectionController connectionController, string canalName)
        {
          
       
            this.connectionController = connectionController;
            InitializeComponent();
            receiver = new Thread(receive);
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
                        processText(text);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        private void processText(string text)
        {
            if (text.Substring(0, 6) == "UPDATE")
            {
                string[] activeUsers = text.Substring(7).Split();
                text = string.Join(" \r\n", activeUsers);
                updateUsersList(text);
            }
            else
            {
                print(text);
            }
        }

        private void updateUsersList(string text)
        {
            if (ActiveUsers.InvokeRequired)
            {
                var d = new SafeCallDelegate(updateUsersList);
                ActiveUsers.Invoke(d, new object[] { text });
            }
            else
            {
                ActiveUsers.Text = text;
            }
        }

        private void LeaveButton_Click(object sender, EventArgs e)
        {
            frmMain.closeCanal();
            receiver.Abort();
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
        private void CanalWindow_FormClosed(object sender, FormClosedEventArgs e)
        {         
            frmMain.closeCanal();
            receiver.Abort();
        }
    }
}
