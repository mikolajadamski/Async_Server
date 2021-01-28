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

namespace ClientApplication.Pages.RightPages
{
    /// <summary>
    /// Interaction logic for settingsPage.xaml
    /// </summary>
    public partial class settingsPage : Page
    {
        private bool isPasswordVisible = false;

        public settingsPage()
        {
            InitializeComponent();
        }

        public RoutedEventHandler setLeftTopButton_Click
        {
            set { leftTopButton.Click += value; }

        }

        public RoutedEventHandler changeButton_Click
        {
            set { changeButton.Click += value; }

        }

        private void changePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                executablePanel.Visibility = Visibility.Hidden;
                changePasswordLabel.Content = "Zmień hasło";
                changePasswordIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.FormTextboxPassword;
                isPasswordVisible = false;
            }
            else
            {
                executablePanel.Visibility = Visibility.Visible;
                changePasswordLabel.Content = "Zamknij";
                changePasswordIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Close;
                isPasswordVisible = true;
            }
        }

        public bool IsPasswordVisible
        {
            set { isPasswordVisible = value; }
        }

        public string UserName
        {
            set { nameLabel.Content = value; }
        }

        public string OldPassword
        {
            get { return oldPassword.Text; }
            set { oldPassword.Text = value; }
        }

        public string NewPassword
        {
            get { return newPassword.Text; }
            set { newPassword.Text = value; }
        }
    }
}
