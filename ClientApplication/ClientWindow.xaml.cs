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

        public ClientWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;

            InitializeComponent();

            displayAvailableCanals();

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

            displayCanal();
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
            }
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

        private void displayCanal()
        {
            StackPanel topInfoPanel = new StackPanel();

        }
      

        private void infoCanal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendMessageToCanal_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
