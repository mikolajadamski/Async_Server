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
using System.Windows.Shapes;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private ConnectionController connectionController;

        private List<canalPage> listOfPages = new List<canalPage>();

        private List<Page> listOfSmallPages = new List<Page>();

        private addPage AddPage = new addPage();

        private choseCanalPage ChoseCanalPage = new choseCanalPage();

        public ClientWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;

            InitializeComponent();

            displayAvailableCanals();

            createNecessaryPages();

            smallFrame.Content = ChoseCanalPage;

        }

        public ConnectionController GetConnectionController
        {
            get => connectionController;
            set => connectionController = value;
        }

        private void ExitButton_Click(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void ClientWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        //to do
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void createNewCanal_Click(object sender, RoutedEventArgs e)
        {
            pagesBorder.Visibility = Visibility.Visible;
            framePages.Content = AddPage;
        }

        private void createCanal_Click(object sender, RoutedEventArgs e)
        {
            string canalName = AddPage.getCenterPanelTextBox;
            if (canalName.Length != 0)
            {
                string newCanal = connectionController.createCanal(canalName);

                displayAvailableCanals();

                //MessageBox.Show(newCanal);
            }
            else
            {
                MessageBox.Show("Error");
            }

            pagesBorder.Visibility = Visibility.Hidden;
           
        }

        private void deleteCanal_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((MenuItem)sender).Tag.ToString();

            string canalName = connectionController.deleteCanal(buttonName.Remove(buttonName.Length - 6, 6));

            displayAvailableCanals();

            MessageBox.Show(canalName);
        }

        //to do scroll
        private void displayAvailableCanals()
        {
            ChoseCanalPage.getCanalsPanel.Children.Clear();
            
            string canalsName = connectionController.getCanals();

            char[] separators = new char[] { '\r', '\n' };

            string[] canals = canalsName.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string canalName in canals)
            {
                createCanalButton(canalName, ChoseCanalPage.getCanalsPanel);

                createCanalPage(canalName);

                createCanalUsersPage(canalName);
            }
        }

        private void createCanalUsersPage(string canalName)
        {
            usersPage UsersPage = new usersPage();

            UsersPage.addNewUserButton.Click += AddNewUserButton_Click;
            UsersPage.Name = canalName + "UsersPage";

            listOfSmallPages.Add(UsersPage);
        }

        public void createNecessaryPages()
        {
            AddPage.setBottomButton_Click = createCanal_Click;
            AddPage.setLeftTopButton_Click = backAddPage_Click;
            AddPage.Name = "AddPage";

            ChoseCanalPage.setCreateNewCanalButton_Click = createNewCanal_Click;
        }

        private void createCanalPage(string name)
        {
            canalPage CanalPage = new canalPage();

            CanalPage.setLeftTopButton_Click = leaveCanalButton_Click;
            CanalPage.setRightTopButton_Click = infoCanalButton_Click;
            CanalPage.setSendButton_Click = sendMessageButton_Click;
            CanalPage.setCenterTopNamePanel = name;
            CanalPage.getSendButtonTag = name;
            CanalPage.Name = name+"Page";

            string mess= CanalPage.getMessageText;

            listOfPages.Add(CanalPage);
        }

        //to do ContextMenu
        private void createCanalButton(string name, StackPanel canalsPanel)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            CanalButton canalButton = new CanalButton();
            canalButton.getCanalNameButtonLabel = name;
            canalButton.setCanalButton_Click = switchToCanal_Click;
            canalButton.setCanalButtonMargin = new Thickness(0, 2, 0, 2);

            canalButton.getCanalButtonName = name + "Button";

            menuItem.Click += deleteCanal_Click;
            menuItem.Tag = canalButton.getCanalButtonName;
            menuItem.Header = "Delete Canal";

            contextMenu.Items.Add(menuItem);

            canalButton.setContextMenu = contextMenu;

            canalsPanel.Children.Add(canalButton);
        }

        private void switchToCanal_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;

            string canalName = connectionController.switchToCanal(buttonName.Remove(buttonName.Length - 6, 6));

            //MessageBox.Show(canalName);

            displayCanal(canalName.Remove(buttonName.Length - 6));
        }

        //to do
        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dodaj nowego uzytkownika do kanalu!");
        }

        private void displayCanal(string canalName)
        {
            pagesBorder.Visibility = Visibility.Visible;
            framePages.Content = listOfPages.First(p => p.Name == canalName+"Page");
            smallFrame.Content = listOfSmallPages.First(p => p.Name == canalName + "UsersPage");
        }

        private void backAddPage_Click(object sender, RoutedEventArgs e)
        {
            pagesBorder.Visibility = Visibility.Hidden;
        }

        private void leaveCanalButton_Click(object sender, RoutedEventArgs e)
        {
            string res = connectionController.leaveCanal();
            //MessageBox.Show(res);

            pagesBorder.Visibility = Visibility.Hidden;
            smallFrame.Content = ChoseCanalPage;
        }
        
        //to do
        private void infoCanal_Click(object sender, RoutedEventArgs e)
        {

        }

        //to do
        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string canalName = ((Button)sender).Tag.ToString();

            string message = listOfPages.First(p => p.Name == canalName + "Page").getMessageText;
            connectionController.sendText(message);
        }

        //to do
        private void infoCanalButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
