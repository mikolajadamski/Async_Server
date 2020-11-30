using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class MenuWindow : Form
    {
        ConnectionController connectionController;
        public MenuWindow(ConnectionController connectionController)
        {
            this.connectionController = connectionController;
            InitializeComponent();
        }

        private void MenuWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
