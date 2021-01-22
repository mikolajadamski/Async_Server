using ClientApplication.Buttons;
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
        private Border border = new Border();
        private bool isButtonShow = false;

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
            if (!isButtonShow)
            {
                BrushConverter converter = new BrushConverter();

                border.Margin = new Thickness(10, 5, 10, 5);
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = (Brush)converter.ConvertFromString("#f083da");

                canalsScrollViewer.Height = 248;

                addUserPanel.Children.Add(border);
                addUserPanel.Children.Add(addUser);

                createCanalLabael.Content = "Zamknij";
                icon.Kind = PackIconMaterialKind.Close;

                isButtonShow = true;
            }
            else 
            {
                addUserPanel.Children.Remove(border);
                addUserPanel.Children.Remove(addUser);
                canalsScrollViewer.Height = 295;
                createCanalLabael.Content = "Dodaj osobę";
                icon.Kind = PackIconMaterialKind.Plus;
                isButtonShow = false;
            }

            

        }

        public void removeAddUserPanel()
        {
            addUserPanel.Children.Remove(border);
            addUserPanel.Children.Remove(addUser);
            canalsScrollViewer.Height = 295;
            createCanalLabael.Content = "Dodaj osobę";
            icon.Kind = PackIconMaterialKind.Plus;
            isButtonShow = false;
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
