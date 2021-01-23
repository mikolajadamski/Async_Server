using ClientApplication.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        ConnectionController connectionController;

        private bool connection = true;

        public MainWindow()
        {
            connectionController = new ConnectionController();
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
            if (connection)
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
            else
                showNotification("Brak połączenia z serwerem");
        }
        private void Sign_Up_Click(object sender, RoutedEventArgs e)
        {
            if (connection)
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
            else
                showNotification("Brak połączenia z serwerem");
        }

        private void showNotification(string text)
        {
            Thread thread = new Thread(() => notify(text));
            thread.Start();
        }
        private void notify(string text)
        {
            int hashCode = 0;
            this.Dispatcher.Invoke(() =>
            {
                Notification notification = new Notification();
                notification.setText = text;
                hashCode = notification.GetHashCode();
                notification.closeButton_Click = closeNotificationButton_Click;
                notification.setLoginNotification();

                notificationBar.Children.Add(notification);


            });
            System.Threading.Thread.Sleep(5000);

            this.Dispatcher.Invoke(() =>
            {
                var enumerator = notificationBar.Children.GetEnumerator();
                Notification notif = null;
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.GetHashCode() == hashCode)
                    {
                        notif = (Notification)enumerator.Current;
                        break;
                    }
                }
                if (notif != null)
                    notificationBar.Children.Remove(notif);
            });
        }

        private void closeNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            notificationBar.Children.RemoveAt(0);
        }
    }
}
