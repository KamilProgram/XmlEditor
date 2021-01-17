using CefSharp.Wpf;
using XmlEditor.Models.DataType;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace XmlEditor.ViewModels
{
    public class BrowserViewModel : INotifyPropertyChanged
    {

        private string WarningsList;

        private string address;
  
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }


        private string warnings;
        public string Warnings
        {
            get { return warnings; }
            set
            {
                WarningsList = value;
                warnings = "Не обнаружено файлов: " + (value.Split('\n').Length - 1);

                OnPropertyChanged("Warnings");
            }
        }

        

        private bool warningVisible;
        public bool WarningVisible
        {
            get { return warningVisible; }
            set
            {
                warningVisible = value;
                OnPropertyChanged("WarningVisible");
            }
        }


 

        private RelayCommand test;
        public RelayCommand Test
        {
            get
            {
                return test ?? (test = new RelayCommand(obj =>
                {
                    MessageBox.Show("Work");
                }));
            }
        }

        private RelayCommand showWarning;
        public RelayCommand ShowWarning
        {
            get
            {
                return showWarning ?? (showWarning = new RelayCommand(obj =>
                {
                   MessageBox.Show(WarningsList);
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
