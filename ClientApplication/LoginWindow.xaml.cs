using ClientApplication.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConnectionController connectionController = new ConnectionController();

        

        public MainWindow()
        {

            connectionController.initializeConnection();

            InitializeComponent();

        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ExitButton_Click(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Sign_In_Click(object sender, RoutedEventArgs e)
        {
            string response = connectionController.login(UsernameBox.Text, PasswordBox.Password);
            if (response != "OK")
            {
                showNotification(response);
            }
            else
            {
                connectionController.setUsername(UsernameBox.Text);
                connectionController.IsLogged = true;

                ClientWindow clientWindow = new ClientWindow(connectionController);

                clientWindow.Show();

                Close();
               
            }
        }
        private void Sign_Up_Click(object sender, RoutedEventArgs e)
        {
            string response = connectionController.register(UsernameBox.Text, PasswordBox.Password);
            if (response != "OK")
            {
                serverResponse.Foreground = Brushes.Red;
                showNotification(response);
            }
            else
            {
                serverResponse.Foreground = Brushes.White;
                showNotification("Pomyślnie utworzono użykownika.");
            }
        }

        private void showNotification(string text)
        {
            Notification notification = new Notification();

            notification.setText = text;

            notification.closeButton_Click = closeNotificationButton_Click;
            notification.setLoginNotification();

            notificationBar.Children.Add(notification);
        }

        private void closeNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            notificationBar.Children.RemoveAt(0);
        }
    }
}
