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
    /// Interaction logic for choseCanalPage.xaml
    /// </summary>
    public partial class choseCanalPage : Page
    {
        public choseCanalPage()
        {
            InitializeComponent();
        }

        public string setCreateNewCanalButton_Click(RoutedEventHandler createNewCanalButton_Click)
        {
            try
            {
                CreateNewCanalButton.Click += createNewCanalButton_Click;
            }
            catch
            {
                return "error";
            }
            return "OK";
        }

        public StackPanel getCanalsPanel()
        {
            return CanalsPanel;
        }

        private void CanalsPanel_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
