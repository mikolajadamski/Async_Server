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

        public ClientWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;

            InitializeComponent();

            displayAvailableCanals();

        }

        private void createPages()
        {
            throw new NotImplementedException();
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

        private void switchToCanal_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;

            string canalName = connectionController.switchToCanal(buttonName.Remove(buttonName.Length - 6, 6));

            MessageBox.Show(canalName);

            displayCanal(canalName.Remove(buttonName.Length-6));
        }

        private void createNewCanal_Click(object sender, RoutedEventArgs e)
        {
            string newCanal = connectionController.createCanal("NowyKanal");

            displayAvailableCanals();

            MessageBox.Show(newCanal);
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
            CanalsPanel.Children.Clear();
            
            string canalsName = connectionController.getCanals();

            char[] separators = new char[] { '\r', '\n' };

            string[] canals = canalsName.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string canalName in canals)
            {
                createCanalButton(canalName);

                createPage(canalName);
            }
        }

        private void createPage(string name)
        {
            Page page = new Page();

            StackPanel topStackPanel = new StackPanel();

            StackPanel leftButtonPanel = new StackPanel();

            StackPanel rightButtonPanel = new StackPanel();

            #region topStackPanel
            topStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            topStackPanel.Orientation = Orientation.Horizontal;
            topStackPanel.Name = name;
            topStackPanel.Margin = new Thickness(0, 10, 0, 10);
            topStackPanel.Width = 600;



            leftButtonPanel.HorizontalAlignment = HorizontalAlignment.Left;
            leftButtonPanel.Margin = new Thickness(0, 0, 200, 0);

            rightButtonPanel.HorizontalAlignment = HorizontalAlignment.Right;
            rightButtonPanel.Margin = new Thickness(200, 0, 0, 0);
            #endregion

            #region backButtonElements
            Button backButton = new Button();
            StackPanel backButtonPanel = new StackPanel();
            PackIconMaterial backButtonIcon = new PackIconMaterial();
            Label backButtonLabel = new Label();
            #endregion

            #region infoButtonElements
            Button infoButton = new Button();
            StackPanel infoButtonPanel = new StackPanel();
            PackIconMaterial infoButtonIcon = new PackIconMaterial();
            Label infoButtonLabel = new Label();
            #endregion


            #region backButtonProperties

            backButton.Click += backCanalButton_Click;
            backButton.Width = 100;
            backButton.Height = 40;
            backButton.Name = "backCanalButton";
            backButton.HorizontalAlignment = HorizontalAlignment.Right;

            backButtonPanel.Orientation = Orientation.Horizontal;

            backButtonIcon.Kind = PackIconMaterialKind.LessThan;
            backButtonIcon.VerticalAlignment = VerticalAlignment.Center;
            backButtonIcon.HorizontalAlignment = HorizontalAlignment.Center;
            backButtonIcon.FontSize = 40;
            backButtonIcon.Foreground = Brushes.Black;

            backButtonLabel.Content = name;
            backButtonLabel.FontSize = 22;

            backButtonPanel.Children.Add(backButtonIcon);
            backButtonPanel.Children.Add(backButtonLabel);

            backButton.Content = backButtonPanel;
            #endregion

            #region infoButtonProperties
            infoButton.Click += infoCanalButton_Click;
            infoButton.Width = 100;
            infoButton.Height = 40;
            infoButton.Name = "infoCanalButton";
            infoButton.HorizontalAlignment = HorizontalAlignment.Right;

            infoButtonPanel.Orientation = Orientation.Horizontal;

            infoButtonIcon.Kind = PackIconMaterialKind.InformationOutline;
            infoButtonIcon.VerticalAlignment = VerticalAlignment.Center;
            infoButtonIcon.HorizontalAlignment = HorizontalAlignment.Center;
            infoButtonIcon.FontSize = 40;
            infoButtonIcon.Foreground = Brushes.Black;

            infoButtonPanel.Children.Add(infoButtonIcon);

            infoButton.Content = infoButtonPanel;
            #endregion


            leftButtonPanel.Children.Add(backButton);
            rightButtonPanel.Children.Add(infoButton);

            topStackPanel.Children.Add(leftButtonPanel);
            topStackPanel.Children.Add(rightButtonPanel);

            page.Content = topStackPanel;
            page.Name = name + "Page";

            listOfPages.Add(page);
        }

        private void createCanalButton(string name)
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

            CanalsPanel.Children.Add(button);
        }

        private void displayCanal(string canalName)
        {
            pagesBorder.Visibility = Visibility.Visible;
            framePages.Content = listOfPages.First(p => p.Name == canalName+"Page");
        }

        private void backCanalButton_Click(object sender, RoutedEventArgs e)
        {
            string res = connectionController.leaveCanal();
            MessageBox.Show(res);

            pagesBorder.Visibility = Visibility.Hidden;
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
    }
}
