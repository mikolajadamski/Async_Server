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

        public string setLeftTopButton_Click(RoutedEventHandler LeftTopButton_Click)
        {
            try
            {
                leftTopButton.Click += LeftTopButton_Click;
            }
            catch
            {
                return "error";
            }
            return "OK";
        }

        public string setBottomButton_Click(RoutedEventHandler bottomButton_Click)
        {
            try
            {
                bottomButton.Click += bottomButton_Click;
            }
            catch
            {
                return "error";
            }
            return "OK";
        }

        public string getCenterPanelTextBox()
        {
            return centerPanelTextBox.Text;
        }
    }
}
