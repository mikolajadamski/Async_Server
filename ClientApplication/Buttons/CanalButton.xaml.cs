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

namespace ClientApplication.Buttons
{
    /// <summary>
    /// Interaction logic for CanalButton.xaml
    /// </summary>
    public partial class CanalButton : UserControl
    {
        public CanalButton()
        {
            InitializeComponent();
        }

        public object getCanalNameButtonLabel
        {
            set { canalNameButtonLabel.Content = value; }
            get { return canalNameButtonLabel.Content; }
        }

        public RoutedEventHandler setCanalButton_Click
        {
            set { canalButton.Click += value; }
        }

        public string getCanalButtonName
        {
            get { return canalButton.Name; }
            set { canalButton.Name = value; }
        }

        public Thickness setCanalButtonMargin
        {
            set { canalButtonGrid.Margin = value; }
        }

        public ContextMenu setContextMenu
        {
            set { canalButton.ContextMenu = value; }
        }
    }
}
