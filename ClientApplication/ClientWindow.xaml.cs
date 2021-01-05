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

        private List<Page> listOfPages = new List<Page>();

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

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {

        }

       

        private void createNewCanal_Click(object sender, RoutedEventArgs e)
        {
            displayCanal("Add");
        }

        private void createCanal_Click(object sender, RoutedEventArgs e)
        {
            string canalName = AddPage.getCenterPanelTextBox();
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

        private void displayAvailableCanals()
        {
            ChoseCanalPage.getCanalsPanel().Children.Clear();
            
            string canalsName = connectionController.getCanals();

            char[] separators = new char[] { '\r', '\n' };

            string[] canals = canalsName.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string canalName in canals)
            {
                createCanalButton(canalName, ChoseCanalPage.getCanalsPanel());

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
            AddPage.setBottomButton_Click(createCanal_Click);
            AddPage.setLeftTopButton_Click(backAddPage_Click);
            AddPage.Name = "AddPage";

            listOfPages.Add(AddPage);

            ChoseCanalPage.setCreateNewCanalButton_Click(createNewCanal_Click);
        }

        private void createCanalPage(string name)
        {
            canalPage CanalPage = new canalPage();

            CanalPage.setLeftTopButton_Click(leaveCanalButton_Click);
            CanalPage.setRightTopButton_Click(infoCanalButton_Click);
            CanalPage.setRightBottomButton_Click(SendMessageToCanal_Click);
            CanalPage.setCenterTopNamePanel(name);
            CanalPage.Name = name+"Page";

            listOfPages.Add(CanalPage);
        }

        //to do
        private void createCanalButton(string name, StackPanel canalsPanel)
        {
            Button button = new Button();
            StackPanel stackPanel = new StackPanel();
            PackIconMaterial packIconMaterial = new PackIconMaterial();
            Label label = new Label();
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            stackPanel.Orientation = Orientation.Horizontal;

            button.Height = 40;
            button.Width = 100;
            button.Name = name + "Button";
            button.VerticalAlignment = VerticalAlignment.Center;
            button.HorizontalAlignment = HorizontalAlignment.Left;

            packIconMaterial.Kind = PackIconMaterialKind.Phone;
            packIconMaterial.VerticalAlignment = VerticalAlignment.Center;
            packIconMaterial.HorizontalAlignment = HorizontalAlignment.Center;
            packIconMaterial.Foreground = Brushes.Black;

            label.Content = name;

            stackPanel.Children.Add(packIconMaterial);
            stackPanel.Children.Add(label);

            menuItem.Click += deleteCanal_Click;
            menuItem.Tag = button.Name;
            menuItem.Header = "Delete Canal";

            contextMenu.Items.Add(menuItem);

            button.Content = stackPanel;
            button.ContextMenu = contextMenu;

            button.Click += switchToCanal_Click;

            canalsPanel.Children.Add(button);
        }

        private void switchToCanal_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;

            string canalName = connectionController.switchToCanal(buttonName.Remove(buttonName.Length - 6, 6));

            //MessageBox.Show(canalName);

            displayCanal(canalName.Remove(buttonName.Length - 6));
        }

        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dodaj nowego uzytkownika do kanalu!");
        }

        private void displayCanal(string canalName)
        {
            pagesBorder.Visibility = Visibility.Visible;
            framePages.Content = listOfPages.First(p => p.Name == canalName+"Page");
            if(canalName != "Add")
                smallFrame.Content = listOfSmallPages.First(p => p.Name == canalName + "UsersPage");
        }

        private void backAddPage_Click(object sender, RoutedEventArgs e)
        {
            string res = connectionController.leaveCanal();
            MessageBox.Show(res);

            pagesBorder.Visibility = Visibility.Hidden;
        }

        private void leaveCanalButton_Click(object sender, RoutedEventArgs e)
        {
            string res = connectionController.leaveCanal();
            //MessageBox.Show(res);

            pagesBorder.Visibility = Visibility.Hidden;
            smallFrame.Content = ChoseCanalPage;
        }

        private void infoCanalButton_Click(object sender, RoutedEventArgs e)
        {
        }


        private void infoCanal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendMessageToCanal_Click(object sender, RoutedEventArgs e)
        {

        }



        #region canalPageProperties

        private void LeftTopButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
