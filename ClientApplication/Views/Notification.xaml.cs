using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ClientApplication.Views
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        Thread thread;
        public Notification()
        {
            InitializeComponent();
        }

        public string setText
        {
            set { ntfyTextBlock.Text = value; }
        }
        
        public RoutedEventHandler closeButton_Click
        {
            set { closeNotificationButton.Click += value; }
        }

        public void setLoginNotification()
        {
            mainGrid.MinHeight = 40;
            mainGrid.MaxWidth = 200;
            notificationStackPanel.MinHeight = 32;
            notificationStackPanel.MaxWidth = 172;
            ntfyTextBlock.MinHeight = 15;
            ntfyTextBlock.MaxWidth = 135;
            ntfyTextBlock.FontSize = 15;
        }
        public Thread Thread
        {
            set => Thread = value;
        }
        public void stopThread()
        {
            thread.Abort();
        }
    }

}
