using ClientApplication.Buttons;
using ClientApplication.Views;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private List<usersPage> listOfSmallPages = new List<usersPage>();
        private addPage AddPage = new addPage();
        private choseCanalPage ChoseCanalPage = new choseCanalPage();
        Thread receiver;
        private string currentCanal;
        Regex msgFinder;
        MatchCollection matchCollection;


        public ClientWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;
            InitializeComponent();
            receiver = new Thread(receive);
            receiver.Start();
            currentCanal = string.Empty;
            msgFinder = new Regex("MSG ([\\w\\W\t\r\n]+?) ENDMSG\r\n");

            displayAvailableCanals();
            createNecessaryPages();
            smallFrame.Content = ChoseCanalPage;
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
                updateUsersList(activeUsers);
            }
            else if (text.Substring(0, 4) == "RESP")
            {
                processResponse(text);
            }
            else if(text.Substring(0,3) == "MSG")
            {
                
                matchCollection = msgFinder.Matches(text);
                foreach (Match match in matchCollection)
                {
                    print(match.Groups[1].Value);

                }
            }
            else if (text.Substring(0, 3) == "ADD")
            {
                showNotification(text);
            }
            else if (text.Substring(0, 6) == "CANALS")
            {
                string[] canals = text.Substring(7).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                updateCanalsList(canals);

            }
            else
            {
                //do nothing, not part of communication protocol
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

                case "CREATE":
                    processCreate(response[2]);
                    break;

                case "DEL":
                    processDelete(response[2]);
                    break;

                case "JOIN":
                    processJoin(response[2], response[3]);
                    break;

                case "ADU":
                    processAddUser(response[2], response[3]);
                    break;

                case "RMV":
                    processRemove(response[2], response[3]);
                    break;
                case "LEAVE":
                    processLeave(response[2]);
                    break;
            }
        }

        private void processLeave(string response)
        {
            if(response == "OK")
            {
                MessageBox.Show("Opuszczono kanał");
            }
            else if(response == "OK_DELETED")
            {
                MessageBox.Show("Opuszczono kanał\n Brak członków - kanał usunięty.");
            }
            else if(response == "NOT_MEMBER")
            {
                MessageBox.Show("Nie jesteś członkiem tego kanału");
            }
            else
            {
                MessageBox.Show("Błąd");
            }
        }

        private void processAddUser(string response, string name)
        {
            if (response == "OK")
            {
                showNotification("Dołączono " + name + " do kanału");
            }
            else if (response == "U_OK")
            {
                showNotification("Dołączono do kanału " + name);
            }
            else if (response == "ALREADY_MEMBER")
            {
                showNotification("Użytkownik " + name + " już jest członkiem tego kanału");
            }
            else if (response == "INVALID")
            {
                showNotification("Kanał " + name + " nie istnieje");
            }
            else if (response == "INVALID_MEMBER")
            {
                showNotification("Użytkownik " + name + " nie istnieje");
            }
        }

        private void processJoin(string response, string name)
        {
            if (response == "OK")
            {
                showNotification("Dołączono do kanału " + name);
            }
            else if (response == "ALREADY_MEMBER")
            {
                showNotification("Jesteś już członkiem kanału " + name);
            }
            else if (response == "INVALID")
            {
                showNotification("Kanał " + name + " nie istnieje");
            }
        }

        private void processDelete(string response)
        {
            if (response == "OK")
            {
                this.Dispatcher.Invoke(() =>
                {
                    displayAvailableCanals();
                });
            }
            else if(response == "AUTH_ERR")
            {
                showNotification("Brak uprawnień!");
            }
            else if(response == "INVALID")
            {
                showNotification("Taki kanał nie istnieje!");
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
                showNotification("Nie jesteś członkiem tego kanału");
                currentCanal = string.Empty;
            }
            else if (response == "INVALID_ERROR")
            {
                //kanał nie istnieje
                currentCanal = string.Empty;
            }
        }

        private void processCreate(string response)
        {
            if(response == "OK")
            {
                //do nothing, canal has been created and can be seen
            }
            else if(response == "ERR")
            {
                showNotification("Nazwa kanału zajęta");
            }
        }

        private void processRemove(string response,string name)
        {
            if (response == "OK")
            {
                showNotification("Usunięto " + name + " z kanału");
            }
            else if (response == "U_OK")
            {
                this.Dispatcher.Invoke(() =>
                {
                    leaveCanal();
                });
                showNotification("Usunięto cię z kanału " + name);
                
            }
            else if (response == "NO_MEMBER")
            {
                showNotification("Użytkownik " + name + " nie jest członkiem tego kanału");
            }
            else if (response == "NO_PERM")
            {
                showNotification("Brak uprawnień");
            }
            else if (response == "INVALID_CANAL")
            {
                showNotification("Kanał " + name + " nie istnieje");
            }
            else if (response == "INVALID_MEMBER")
            {
                showNotification("Użytkownik " + name + " nie istnieje");
            }
        }

        private void print(string text)
        {
            this.Dispatcher.Invoke(() =>
            {
                var page = listOfPages.First(p => p.Name == currentCanal + "Page");
            
                
                    if (text != "Opuszczono kanal")
                    {
                        Message message = new Message();

                        string[] messageData = text.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        message.setMessage(messageData);

                        if (messageData[0] != connectionController.Username)
                            message.setHorizontalAlignment = HorizontalAlignment.Left;
                        else
                        {
                            message.setOwnMessage();
                        }

                        page.setMessage(message);
                    }
                
            });
        }

        private void updateUsersList(string[] users)
        {
            this.Dispatcher.Invoke(() =>
            {
                usersPage UsersPage = (usersPage)listOfSmallPages.First(p => p.Name == currentCanal + "UsersPage");
                UsersPage.UsersPanel.Children.Clear();
                for(int i=0; i< users.Length; i = i + 2)
                {
                    if (users[i] != connectionController.Username)
                    {
                        UserButton userButton = new UserButton();

                        if(users[i+1] == "0")
                        userButton.UserButtonLabel = users[i];
                        else userButton.UserButtonLabel = users[i] + " @admin";

                        MenuItem removeBar = new MenuItem();
                        removeBar.Click += deleteUserButton_Click;
                        removeBar.Tag = users[i];
                        removeBar.Header = "Usuń osobę";
                        ContextMenu contextMenu = new ContextMenu();
                        contextMenu.Items.Add(removeBar);

                        userButton.setContextMenu = contextMenu;

                        UsersPage.UsersPanel.Children.Add(userButton);
                    }
                }

             
            });
        }

        private void updateCanalsList(string[] canals)
        {
            this.Dispatcher.Invoke(() =>
            {
                ChoseCanalPage.CanalsPanel.Children.Clear();
                foreach (string canalName in canals)
                {
                    createCanalButton(canalName, ChoseCanalPage.getCanalsPanel);

                    createCanalPage(canalName);

                    createCanalUsersPage(canalName);
                }
            });
        }

        public ConnectionController GetConnectionController
        {
            get => connectionController;
            set => connectionController = value;
        }

        private void ExitButton_Click(object sender, ExecutedRoutedEventArgs e)
        {
            if(currentCanal != string.Empty)
            {
                connectionController.leaveCanal();
            }
            receiver.Abort();
            connectionController.disconnectClient();
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
            string type;
            if (AddPage.isPrivate.IsChecked == true)
            {
                type = "private";
            }
            else
            {
                type = "public";
            }

            string canalName = AddPage.getCenterPanelTextBox;
            if (canalName.Length != 0)
            {
                connectionController.createCanal(canalName + " " + type);
                AddPage.getCenterPanelTextBox = null;
                displayAvailableCanals();
            }
            else
            {
                showNotification("Error");
            }

            pagesBorder.Visibility = Visibility.Hidden;

        }

        private void deleteCanal_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((MenuItem)sender).Tag.ToString();
            connectionController.deleteCanal(buttonName.Remove(buttonName.Length - 6, 6));
        }

        private void displayAvailableCanals()
        {
            ChoseCanalPage.getCanalsPanel.Children.Clear();

            connectionController.getCanals();
        }

        private void createCanalUsersPage(string canalName)
        {
            usersPage UsersPage = new usersPage();

            UsersPage.setAddUser_Click = AddNewUserButton_Click;
            UsersPage.Name = canalName + "UsersPage";

            listOfSmallPages.Add(UsersPage);
        }

        public void createNecessaryPages()
        {
            AddPage.setBottomButton_Click = createCanal_Click;
            AddPage.setLeftTopButton_Click = backAddPage_Click;
            AddPage.Name = "AddPage";

            ChoseCanalPage.setCreateNewCanalButton_Click = createNewCanal_Click;
            ChoseCanalPage.setResetCanalListButton_Click = resetCanalListButton_Click;
        }

        private void resetCanalListButton_Click(object sender, RoutedEventArgs e)
        {
            displayAvailableCanals();
        }

        private void createCanalPage(string name)
        {
            canalPage CanalPage = new canalPage();

            CanalPage.setLeftTopButton_Click = leaveCanalButton_Click;
            CanalPage.setRightTopButton_Click = infoCanalButton_Click;
            CanalPage.SendButton_Click = sendMessageButton_Click;
            CanalPage.Key_Click = returnClick;
            CanalPage.setCenterTopNamePanel = name;
            CanalPage.getSendButtonTag = name;
            CanalPage.Name = name + "Page";

            string mess = CanalPage.getMessageText;

            listOfPages.Add(CanalPage);
        }

        private void createCanalButton(string name, StackPanel canalsPanel)
        {
    
            CanalButton canalButton = new CanalButton();
            canalButton.getCanalNameButtonLabel = name;
            canalButton.setCanalButton_Click = switchToCanal_Click;
            canalButton.setCanalButtonMargin = new Thickness(0, 2, 0, 2);
            canalButton.getCanalButtonName = name + "Button";

            MenuItem joinBar = new MenuItem();
            joinBar.Click += joinCanalClick;
            joinBar.Tag = canalButton.getCanalButtonName;
            joinBar.Header = "Join Canal";

            MenuItem leaveBar = new MenuItem();
            leaveBar.Click += leaveCommunityClick;
            leaveBar.Tag = canalButton.getCanalButtonName;
            leaveBar.Header = "Leave Canal";

            MenuItem deleteBar = new MenuItem();
            deleteBar.Click += deleteCanal_Click;
            deleteBar.Tag = canalButton.getCanalButtonName;
            deleteBar.Header = "Delete Canal";

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(joinBar);
            contextMenu.Items.Add(leaveBar);
            contextMenu.Items.Add(deleteBar);

            canalButton.setContextMenu = contextMenu;
            canalsPanel.Children.Add(canalButton);
        }

        private void leaveCommunityClick(object sender, RoutedEventArgs e)
        {
            string name = ((MenuItem)sender).Tag.ToString();
            string canalName = name.Remove(name.Length - 6);
            connectionController.leaveCommunity(canalName);
        }

        private void joinCanalClick(object sender, RoutedEventArgs e)
        {
            string name = ((MenuItem)sender).Tag.ToString();
            string canalName = name.Remove(name.Length - 6);
            connectionController.joinCanal(canalName);
        }

        private void switchToCanal_Click(object sender, RoutedEventArgs e)
        {
            currentCanal = ((Button)sender).Name.Remove(((Button)sender).Name.Length - 6);

            connectionController.switchToCanal(currentCanal);

            
        }

        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = listOfSmallPages.First(p => p.Name == currentCanal + "UsersPage").getUserName;

            listOfSmallPages.First(p => p.Name == currentCanal + "UsersPage").removeAddUserPanel();

            connectionController.addUserToCanal(currentCanal, userName);
        }

        private void deleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = ((MenuItem)sender).Tag.ToString();

            connectionController.removeUserFromCanal(currentCanal, userName);
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
            framePages.Content = null;
        }

        private void leaveCanalButton_Click(object sender, RoutedEventArgs e)
        {
            leaveCanal();  
        }

        //to do
        private void infoCanal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var page = listOfPages.First(p => p.Name == currentCanal + "Page");
            string message = page.getMessageText;
            page.flushMessageText();
            connectionController.sendText(message);
        }

        private void returnClick(object sender, KeyEventArgs e)
        {
            var page = listOfPages.First(p => p.Name == currentCanal + "Page");
            if (Keyboard.IsKeyDown(Key.RightShift) && Keyboard.IsKeyDown(Key.Enter))
            {
                page.messageBox.Text += Environment.NewLine;
                page.messageBox.SelectionStart = page.messageBox.Text.Length;
            }
            else if (e.Key == Key.Enter)
            {
                string message = page.getMessageText;
                page.flushMessageText();
                connectionController.sendText(message);
            }
        }

        //to do
        private void infoCanalButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void leaveCanal()
        {
            connectionController.leaveCanal();

            var page = listOfPages.First(p => p.Name == currentCanal + "Page");
            page.clearMessages();

            pagesBorder.Visibility = Visibility.Hidden;
            framePages.Content = null;
            smallFrame.Content = ChoseCanalPage;

            currentCanal = string.Empty;

            pagesBorder.Visibility = Visibility.Hidden;
            framePages.Content = null;
            smallFrame.Content = ChoseCanalPage;
            displayAvailableCanals();
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

                notificationBar.Children.Add(notification);
                
                
            });
            System.Threading.Thread.Sleep(5000);

            this.Dispatcher.Invoke(() =>
            {
                var enumerator = notificationBar.Children.GetEnumerator();
                Notification notif = null;
                while (enumerator.MoveNext())
                {
                        if(enumerator.Current.GetHashCode() == hashCode)
                        {
                            notif = (Notification)enumerator.Current;
                            break;
                        }
                } 
                if(notif!=null)
                    notificationBar.Children.Remove(notif);
            });
        }

        private void closeNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            notificationBar.Children.RemoveAt(0);
        }
    }
}
