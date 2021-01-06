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
    /// Interaction logic for CanalButtonContexMenu.xaml
    /// </summary>
    public partial class CanalButtonContexMenu : UserControl
    {
        public CanalButtonContexMenu()
        {
            InitializeComponent();
        }

        public string getCanalButtonContextMenuName 
        {
            set { canalButtonContextMenu.Name = value; }
            get { return canalButtonContextMenu.Name; }
        }
    }
}
