using FastColoredTextBoxNS;
using ICSharpCode.AvalonEdit.Document;
using XmlEditor.Models;
using XmlEditor.Models.DataType;
using XmlEditor.Models.DataType.AvalonEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XmlEditor.ViewModels
{
    class TableViewModel : INotifyPropertyChanged
    {
      
        public ObservableCollection<TableRow> Table { get; set; }

        public TableModel model = new TableModel();
        public TableViewModel()
        {
            Table = new ObservableCollection<TableRow>();
        }

        private string name = "";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

      

        private TableRow selectedRow;
        public TableRow SelectedRow
        {
            get { return selectedRow; }
            set
            {
                selectedRow = value;
                OnPropertyChanged("SelectedRow");
            }
        }

        private int selectedRowIndex;
        public int SelectedRowIndex
        {
            get { return selectedRowIndex; }
            set
            {
                selectedRowIndex = value;
                OnPropertyChanged("SelectedRowIndex");
            }
        }



        private RelayCommand hiddenRow;
        public RelayCommand HiddenRow
        {
            get
            {
                return hiddenRow ?? (hiddenRow = new RelayCommand(obj =>
                {
                    if ((bool)obj == true)
                    {
                       model.showHidenRow(Visibility.Collapsed,SelectedRowIndex, Table);
                    }
                    else model.showHidenRow(Visibility.Visible, SelectedRowIndex, Table);
                }, (obj) => Table.Count > 0));
            }
        }

        private RelayCommand findRow;
        public RelayCommand FindRow
        {
            get
            {
                return findRow ?? (findRow = new RelayCommand(obj =>
                {
                    try
                    {
                        model.findTag(SelectedRow);
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка в " + SelectedRowIndex + "строке");
                    }

                }, (obj) => Table.Count > 0));
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
