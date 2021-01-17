using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XmlEditor.Views.Resources
{
    partial class ListBox: ResourceDictionary
    {
        public ListBox()
        {
            InitializeComponent();
        }

       

        private void ReNameMod_TextChanged(object sender, TextChangedEventArgs e)
        {
            //(sender as TextBox).SelectAll();
        }

        private void ReNameMod_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void ReNameMod_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TextBox).Focus();
            (sender as TextBox).SelectAll();
        }
    }
}
