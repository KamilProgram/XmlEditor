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

namespace XmlEditor.Views
{
    /// <summary>
    /// Логика взаимодействия для BrowserPage.xaml
    /// </summary>
    public partial class BrowserPage : Page
    {
        public BrowserPage()
        {
            InitializeComponent();
           // ChromiumWebBrowser.BrowserSettings.WebSecurity = CefSharp.CefState.Disabled;
            ChromiumWebBrowser.AddHandler(UIElement.MouseWheelEvent, new MouseWheelEventHandler(Page_MouseWheel), true);
        }



        bool isCtrlKeyPress = false;
        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
            {
                isCtrlKeyPress = false;
            }
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
            {
                isCtrlKeyPress = true;
            }
        }

        private void Page_MouseWheel(object sender, MouseWheelEventArgs e)
        {


            if (isCtrlKeyPress == true)
            {

                if (e.Delta > 0)// && ChromiumWebBrowser.ZoomLevel <= 100
                {
                    /// MessageBox.Show("work1");
                    ChromiumWebBrowser.ZoomInCommand.Execute(null);
                }
                else if (e.Delta < 0)//&& ChromiumWebBrowser.ZoomLevel >= -100
                {
                    //MessageBox.Show("work2");
                    ChromiumWebBrowser.ZoomOutCommand.Execute(null);
                }
            }
        }
    }
}
