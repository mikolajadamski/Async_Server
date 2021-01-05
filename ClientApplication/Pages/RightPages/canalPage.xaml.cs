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

        public string setLeftTopButtonVisibility(Visibility visibility)
        {
            try
            {
                leftTopButtonPanel.Visibility = visibility;
            }
            catch(Exception e)
            {
                return "Error";
            }
            return "OK";
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

        public string setCenterTopNamePanel(string content)
        {
            try
            {
                centerTopNameLabel.Content = content;
            }
            catch(Exception e)
            {
                return "Error";
            }

            return "OK";
        }

        public string setRightTopButtonVisibility(Visibility visibility)
        {
            try
            {
                rightTopButtonPanel.Visibility = visibility;
            }
            catch (Exception e)
            {
                return "Error";
            }
            return "OK";
        }

        public string setRightTopButton_Click(RoutedEventHandler RightTopButton_Click)
        {
            try
            {
                rightTopButton.Click += RightTopButton_Click;
            }
            catch
            {
                return "Error";
            }
            return "OK";
        }


        public string setCenterStackPanel(StackPanel stackPanel)
        {
            try
            {
                centerPanel.Children.Add(stackPanel);
            }
            catch (Exception e) 
            {
                return "Error";
            }

            return "OK";
        }


        public string insertBottomBlockText(string text)
        {
            try
            {
                insertBottomBlock.Text = text;
            }
            catch(Exception e)
            {
                return "Error";
            }

            return "OK";
        }

        public string setRightBottomButton_Click(RoutedEventHandler RightBottomButton_Click)
        {
            try
            {
                rightBottomButton.Click += RightBottomButton_Click;
            }
            catch (Exception e)
            {
                return "Error";
            }

            return "OK";
        }
    }
}
