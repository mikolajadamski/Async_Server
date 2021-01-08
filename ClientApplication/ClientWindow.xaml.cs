using MahApps.Metro.IconPacks;
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
        Thread receiver;
        private string currentCanal;


        public ClientWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;
            InitializeComponent();
            displayAvailableCanals();
            createNecessaryPages();
            smallFrame.Content = ChoseCanalPage;
            receiver = new Thread(receive);
            receiver.Start();
            currentCanal = string.Empty;
        }

        private void receive()
        {
            string text;
            connectionController.setTimeout(100);
            while (true)
            {
                try
                {

                    text = connectionController.readText();
                    processText(text);
                    text = string.Empty;
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
            else if (text.Substring(0, 4) == "RESP")
            {
                processResponse(text);
            }
            else
            {
                print(text);
            }
        }

        private void processResponse(string text)
        {
            string[] response = text.Split();
            switch (response[1])
            {
                case "SWITCHTO":
                    processSwitchTo(response[2]);
                    break;
            }
        }

        private void processSwitchTo(string response)
        {
            if (response == "OK")
            {
                displayCanal(currentCanal);
            }
            else if (response == "AUTH_ERROR")
            {
                //brak dostępu
                currentCanal = string.Empty;
            }
            else if (response == "INVALID_ERROR")
            {
                //kanał nie istnieje
                currentCanal = string.Empty;
            }
        }

        private void print(string text)
        {
            this.Dispatcher.Invoke(() =>
            {
                var page = listOfPages.First(p => p.Name == currentCanal + "Page");
                page.setMessagesBoxText(text);
            });
        }

        private void updateUsersList(string text)
        {
            throw new NotImplementedException();
        }

        /*
        private void printInLogger(string text)
        {
            if (loggerBox.InvokeRequired)
            {
                var d = new SafeCallDelegate(printInLogger);
                loggerBox.Invoke(d, new object[] { text });
            }
            else
            {
                loggerBox.AppendText(text);
            }
        }
        */

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
            CanalPage.Name = name + "Page";

            string mess = CanalPage.getMessageText;

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
            currentCanal = ((Button)sender).Name.Remove(((Button)sender).Name.Length - 6);
            connectionController.switchToCanal(currentCanal);
        }

        //to do
        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dodaj nowego uzytkownika do kanalu!");
        }

        private void displayCanal(string canalName)
        {
            this.Dispatcher.Invoke(() =>
            {
                pagesBorder.Visibility = Visibility.Visible;
                framePages.Content = listOfPages.First(p => p.Name == canalName + "Page");
                smallFrame.Content = listOfSmallPages.First(p => p.Name == canalName + "UsersPage");
            });

        }

        private void backAddPage_Click(object sender, RoutedEventArgs e)
        {
            pagesBorder.Visibility = Visibility.Hidden;
        }

        private void leaveCanalButton_Click(object sender, RoutedEventArgs e)
        {
            connectionController.leaveCanal();
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

            var page = listOfPages.First(p => p.Name == currentCanal + "Page");
            string message = page.getMessageText;
            page.flushMessageText();
            page.setMessagesBoxText(message + "\r\n");
            connectionController.sendText(message);

        }

        //to do
        private void infoCanalButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
