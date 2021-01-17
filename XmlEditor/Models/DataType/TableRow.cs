using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XmlEditor.Models
{
    class TableRow : INotifyPropertyChanged
    {
        private Visibility showRow;
        private bool? collaps;

        private int ind;
        private string item;
        private string name;
      
        public Visibility ShowRow
        {
            get { return showRow; }
            set
            {
                showRow = value;
                OnPropertyChanged("ShowRow");
            }
        }

        public bool? Collaps
        {
            get { return collaps; }
            set
            {
                collaps = value;
                OnPropertyChanged("Collaps");
            }
        }

        public int Ind
        {
            get { return ind; }
            set
            {
                ind = value;
                OnPropertyChanged("Ind");
            }
        }

        public string Item
        {
            get { return item; }
            set
            {
                item = value;
                OnPropertyChanged("Item");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }

        }
    }
}
