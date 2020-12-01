using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class LoginWindow : Form
    {
        ConnectionController connectionController;
        public LoginWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string response = connectionController.login(usernameBox.Text, passwordBox.Text);
            if (response != "OK")
            {
                serverResponse.Text = response;
            }
            else
            {
                connectionController.setUsername(usernameBox.Text);
                connectionController.IsLogged = true;
                Close();
            }

        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            string response = connectionController.register(usernameBox.Text, passwordBox.Text);
            if(response != "OK")
            {
                serverResponse.ForeColor = System.Drawing.Color.Red;
                serverResponse.Text = response;
            }
            else
            {
                serverResponse.ForeColor = System.Drawing.Color.Black;
                serverResponse.Text = "Pomyślnie utworzono użykownika.";
            }
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                loginButton_Click(this, new EventArgs());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
