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
    /// Interaction logic for canalPage.xaml
    /// </summary>
    public partial class canalPage : Page
    {
        public canalPage()
        {
            InitializeComponent();
        }

        public Visibility setLeftTopButtonVisibility
        {
            set { leftTopButtonPanel.Visibility = value; }
        }

        public RoutedEventHandler setLeftTopButton_Click
        {
            set { leftTopButton.Click += value; }
           
        }

        public string setCenterTopNamePanel
        {
            set { centerTopNameLabel.Content = value; }
        }

        public Visibility setRightTopButtonVisibility
        {
            set { rightTopButtonPanel.Visibility = value; }
        }

        public RoutedEventHandler setRightTopButton_Click
        {
            set { rightTopButton.Click += value; }
        }

        public StackPanel setCenterStackPanel
        {
            set { centerPanel.Children.Add(value); }
        }

        public string insertBottomBlockText
        {
            set { insertBottomBlock.Text = value; }
        }

        public RoutedEventHandler setRightBottomButton_Click
        {
           set{ rightBottomButton.Click += value; }
        }
    }
}
