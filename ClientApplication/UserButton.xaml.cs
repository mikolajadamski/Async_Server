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
    /// Interaction logic for CanalButton.xaml
    /// </summary>
    public partial class UserButton : UserControl
    {
        public UserButton()
        {
            InitializeComponent();
        }
        public object UserButtonLabel
        {
            set { userButtonLabel.Content = value; }
            get { return userButtonLabel.Content; }
        }
        public Brush setBorderColor
        {
            set { userBorder.BorderBrush = value; }
        }
    }
}