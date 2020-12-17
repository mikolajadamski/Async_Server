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
        ConnectionController connectionController = new ConnectionController();

        public ClientWindow()
        {
            connectionController.initializeConnection();

           

            InitializeComponent();

            displayAvailableCanals();

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
            string canalName = sender.ToString();

            displayCanal(canalName);
        }

        private void createNewCanal_Click(object sender, RoutedEventArgs e)
        {
            createNewCanal();
        }

        private void deleteCanal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void displayAvailableCanals()
        {
            string canalName = "";
            canalName = connectionController.getCanals();

            createCanalButton("Bład");
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
            menuItem.Header = "Delete Canal";

            contextMenu.Items.Add(menuItem);

            button.Content = stackPanel;
            button.ContextMenu = contextMenu;

            button.MouseDoubleClick += switchToCanal_Click;


            CanalsPanel.Children.Add(button);
        }

        private void displayCanal(string name)
        {
            StackPanel topInfoPanel = new StackPanel();

            canalName.Text = name;

        }
      

        private void createNewCanal()
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
