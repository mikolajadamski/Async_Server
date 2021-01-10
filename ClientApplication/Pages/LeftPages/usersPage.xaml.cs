using MahApps.Metro.IconPacks;
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
    /// Interaction logic for usersPage.xaml
    /// </summary>
    public partial class usersPage : Page
    {
        private AddUser addUser = new AddUser();

        public usersPage()
        {
            InitializeComponent();
        }

        public StackPanel UsersPanel
        {
            get { return usersPanel; }
        }

        private void addNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter converter = new BrushConverter();

            Border border = new Border();

            border.Margin = new Thickness(10, 0, 10, 0);
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = (Brush)converter.ConvertFromString("#f083da");

            addUserPanel.Children.Add(border);
            addUserPanel.Children.Add(addUser);

        }

        public RoutedEventHandler setAddUser_Click
        {
            set { addUser.setAddButton_Click = value; }
        }

        public string getUserName
        {
            get { return addUser.getUserName; }
        }
    }
}
