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
    /// Interaction logic for addPage.xaml
    /// </summary>
    public partial class addPage : Page
    {
        public addPage()
        {
            InitializeComponent();
        }

        public RoutedEventHandler setLeftTopButton_Click
        {
            set { leftTopButton.Click += value; }
        }

        public RoutedEventHandler setBottomButton_Click
        {
            set{ bottomButton.Click += value; }
        }

        public string getCenterPanelTextBox
        {
            get { return centerPanelTextBox.Text; }
        }

    }
}
