using ICSharpCode.AvalonEdit.Document;
using XmlEditor.Models.DataType.AvalonEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XmlEditor.Models
{
    public class EditableFile : INotifyPropertyChanged
    {
        private string path;
        private string header;

        private string text;

        private bool isPreview;
        private int untitled;
        private bool isChangedText;
        private bool isRename;

        private MyAvalonEditor textEditor;
        public bool CanAcceptCgildren { get; set; }
       
        public MyAvalonEditor TextEditor
        {
            get { return textEditor; }
            set
            {
                textEditor = value;
                OnPropertyChanged("TextEditor");
            }
        }
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged("Path");
            }
        }

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }


     
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }

        public int Untitled
        {
            get { return untitled; }
            set
            {
                untitled = value;
                OnPropertyChanged("Untitled");
            }
        }

        public bool IsChangedText
        {
            get { return isChangedText; }
            set
            {
                isChangedText = value;
                OnPropertyChanged("IsChangedText");
            }
        }

        public bool IsPreview
        {
            get { return isPreview; }
            set
            {
                isPreview = value;
                OnPropertyChanged("IsPreview");
            }
        }

        public bool IsRename
        {
            get { return isRename; }
            set
            {
                isRename = value;
                OnPropertyChanged("IsRename");
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
