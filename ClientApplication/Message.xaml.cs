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
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : UserControl
    {
        public Message()
        {
            InitializeComponent();
        }
        public void setMessage(string[] message)
        {
            userMessageName.Text = message[0];
            userMessageTime.Text = message[1];
            messageBlockText.Text = message[2];
        }
        public HorizontalAlignment setHorizontalAlignment
        {
            set { messageControl.HorizontalAlignment = value; }
        }

        public void setOwnMessage()
        {
            messageControl.HorizontalAlignment = HorizontalAlignment.Right;
            messageBorder.BorderThickness = new Thickness(0, 0, 3, 3);
            messageBorder.Background = Brushes.LightSkyBlue;
            GradientStop leftGradient = new GradientStop(Brushes.LightBlue.Color, 0.0);
            GradientStop rightGradient = new GradientStop(Brushes.DarkBlue.Color, 1);
            userMessageName.Visibility = Visibility.Hidden;
            addtionalMessageInfo.HorizontalAlignment = HorizontalAlignment.Right;
        }

        public string setmess
        {
            set { messageBlockText.Text = value; }
        }
    }
}