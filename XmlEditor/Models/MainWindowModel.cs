using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;
using System.Drawing;
using XmlEditor.Models.DataType.ExploerTree;
using ICSharpCode.AvalonEdit;
using XmlEditor.Models.DataType.AvalonEditor;
using XmlEditor.ViewModels;
using System.Text.RegularExpressions;

namespace XmlEditor.Models
{
    public class Xslt
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

   /* public class RenderPage
    {
        public Page Page { get; set; }
        public string Error { get; set; }
        public string Warnings { get; set; }
    }*/

    class MainWindowModel 
    {


        public Page browserPage;
        private BrowserViewModel BrowserViewModel = new BrowserViewModel();

        public Page tablePage;
        private TableViewModel TableViewModel = new TableViewModel();

        public Xslt currentXslt;
        private Xslt serna = new Xslt()
        {
            Name = "serna",
            Path = Environment.CurrentDirectory + "\\serna\\module.xsl"
        };

        private Xslt tg_builder = new Xslt()
        {
            Name = "tg_builder",
            Path = Environment.CurrentDirectory + "\\tg_builder\\s1000d_2_2.xslt"
        };

        BrowserModel browserModel = new BrowserModel();
        TableModel tableModel = new TableModel();

        Page RenderPage = new Page();

        //RenderPage RenderPage = new RenderPage();

        private string Warning = "";
        private string Error = "";
        public MainWindowModel()
        {
            browserPage = new Views.BrowserPage()
            {
                DataContext = BrowserViewModel,
            };

            RenderPage = browserPage;

            tablePage = new Views.TablePage()
            {
                DataContext = TableViewModel,
            };
            TableViewModel.model = tableModel;

            currentXslt = serna;

        }

      

        EditableFile PageItem = null;
        int UntitledIndex = 1;
        public string OpenedFolder = "";

        /// <summary>
        /// Работу с отдельными файлами
        /// </summary>
        #region file

        private int getFileIndex(string path, ObservableCollection<EditableFile> Items)
        {
           for (int i = 0; i < Items.Count; i++)
            {
                if (path.Equals(Items[i].Path) && Items[i].IsPreview == false)
                {
                    return i;
                }
            }
            return -1;
        }

        public EditableFile LoadFile(string path, ObservableCollection<EditableFile> Items)
        {
             int index = getFileIndex(path, Items);
              EditableFile newItem = new EditableFile();

            if (index < 0)
            {
                newItem = CreateItem(path);
                Items.Insert(Items.Count, newItem);
                //SelectedItemIndex = Items.Count-1;
            }
            else     newItem = Items.ElementAt(index);

            return newItem;
        }

        public EditableFile CreateItem(string path)
        {
            EditableFile item = new EditableFile
            {
                Path = path,
                Header = Path.GetFileName(path),
                Text = File.ReadAllText(path),

                TextEditor = new MyAvalonEditor
                {
                    Text = File.ReadAllText(path),
                },
                IsChangedText = false,
            };
            return item;
        }

        public EditableFile CreateNewItem()
        {
            EditableFile item = new EditableFile
            {
                Header = "Без названия-" + UntitledIndex + ".xml",
                Path = "",

                TextEditor = new MyAvalonEditor
                {
                    Text = ""
                },
                Untitled = UntitledIndex,
                IsChangedText = false
            };
            UntitledIndex++;
            return item;
        }

       

        public EditableFile OpenFile(ObservableCollection<EditableFile> Items)
        {
            EditableFile newItem = null;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = ".xml | *.xml";

                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (string path in openFileDialog.FileNames)
                    {
                        try
                        {
                            newItem = LoadFile(path, Items);
                            Transform(Items[Items.Count - 1]);

                        }
                        catch
                        {
                            MessageBox.Show("Не удается открыть " + path);
                        }
                    }
                   // Transform();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return newItem;
        }

        public string OldFileName;

        public string rename(string path, string newName)
        {
            if (!path.Equals(""))
            {
                string newPath = Path.GetDirectoryName(path) + "\\" + newName + ".xml";

                try
                {
                    File.Move(path, newPath);
                    OldFileName = newName;
                    return newPath;

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return path;
                }
            }
            return "";
        }
        public bool? SaveAs(EditableFile SelectedItem)
        {
            bool? res;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            {
                saveFileDialog.Filter = ".xml | *.xml";
                saveFileDialog.FileName = SelectedItem.Header;
                res = saveFileDialog.ShowDialog();

                if (res == true)
                {
                    SelectedItem.Path = saveFileDialog.FileName;
                    SelectedItem.Header = Path.GetFileName(SelectedItem.Path);
                    SelectedItem.IsChangedText = false;
                    File.WriteAllText(SelectedItem.Path, SelectedItem.TextEditor.Text);
                    res = true;
                }
            }

            return res;
        }

        public bool? Save(EditableFile SelectedItem)
        {
            bool? res = null;
            string Message = "Сохранить изменения в файле" + Environment.NewLine + SelectedItem.Header;//+ Environment.NewLine + Path.GetFileName(SelectedTab.Path)
            MessageBoxResult result = MessageBox.Show(Message, "Сохранить", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    File.WriteAllText(SelectedItem.Path, SelectedItem.TextEditor.Text);
                    res = true;
                }
                catch
                {
                    res = SaveAs(SelectedItem);
                }

                SelectedItem.IsChangedText = false;
            }

            if (result == MessageBoxResult.No)
            {
                res = false;
            }

            return res;
        }
/*
        public void FindDialog(MyAvalonEditor TextEditor)
        {
            // FindReplaceDialog FindDialog = new FindReplaceDialog(TextEditor);
            //FindDialog.FindReplaceTabIndex(1);

            FindReplaceDialog.ShowForFind(TextEditor);
        }

        public void ReplaceDialog(MyAvalonEditor TextEditor)
        {
            // FindReplaceDialog ReplaceDialog = new FindReplaceDialog(TextEditor);
            //ReplaceDialog.FindReplaceTabIndex(0);
            //ReplaceDialog.Show();

            FindReplaceDialog.ShowForReplace(TextEditor);
        }
    */

        #endregion

        /// <summary>
        /// Операции с элементами коллекции Items
        /// </summary>
        #region items


        public bool ClosingAllItems(ObservableCollection<EditableFile> Items)
        {
            bool res = true;
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (ClosingItem(Items, Items[i]) == false)
                {
                    res = false;
                    break;
                }
            }
            return res;
        }

        public bool ClosingItem(ObservableCollection<EditableFile> Items, EditableFile Tab)
        {
            bool res = true;
            if (Tab.IsChangedText == true)
            {

                if (Save(Tab) != null)
                {
                    if (Tab.Untitled == UntitledIndex - 1)
                    {
                        UntitledIndex--;
                    }

                    Items.Remove(Tab);
                }
                else
                {
                    res = false;
                }
            }
            else
            {
                if (Tab.Untitled == UntitledIndex - 1)
                {
                    UntitledIndex--;
                }

                Items.Remove(Tab);
            }
            return res;
        }
        #endregion

        /// <summary>
        /// Работа с проводником
        /// </summary>
        #region folder

        private void LoadFolder(string file, ObservableCollection<FileSystemObjectInfo> WorkFolder)
        {
            WorkFolder.Clear();
            OpenedFolder = file;
            DirectoryInfo drives = new DirectoryInfo(file);
            var newDir = new FileSystemObjectInfo(drives);
            WorkFolder.Insert(WorkFolder.Count, newDir);
            // WorkspaceVisible = true;
            // TreeVisible = true;
        }


        public int LoadFilePreview(string file, ObservableCollection<EditableFile> Items )
        {
            int index = getFileIndex(file,Items);

            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].IsPreview == true)
                    {
                        Items.RemoveAt(i);
                        break;
                    }
                }
            }

            if (index < 0)
            {
                EditableFile item = new EditableFile();
                item = CreateItem(file);
                item.IsPreview = true;
                Items.Insert(Items.Count, item);

                return Items.Count - 1;
            }
            else return index;
        }

        public void newFolder(ObservableCollection<FileSystemObjectInfo> WorkFolder )
        {
            var openFolder = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (openFolder.ShowDialog().GetValueOrDefault())
            {
                WorkFolder.Clear();
                OpenedFolder = openFolder.SelectedPath;
                DirectoryInfo drives = new DirectoryInfo(openFolder.SelectedPath);
                var newDir = new FileSystemObjectInfo(drives);
                WorkFolder.Insert(WorkFolder.Count, newDir);

                //WorkspaceVisible = true;
                //TreeVisible = true;
            }
        }

        public void newFolder(ObservableCollection<FileSystemObjectInfo> WorkFolder, string path)
        {
            
                WorkFolder.Clear();
                OpenedFolder = path;
                DirectoryInfo drives = new DirectoryInfo(path);
                var newDir = new FileSystemObjectInfo(drives);
                WorkFolder.Insert(WorkFolder.Count, newDir);
        }

        public void updateFolder(ObservableCollection<FileSystemObjectInfo> WorkFolder)
        {
            WorkFolder.Clear();
            DirectoryInfo drives = new DirectoryInfo(OpenedFolder);
            var newDir = new FileSystemObjectInfo(drives);
            WorkFolder.Insert(WorkFolder.Count, newDir);
        }

        public int loadXmlPre(FileSystemObjectInfo TreeItem, ObservableCollection<EditableFile> Items)
        {
            //var TreeItem = obj as FileSystemObjectInfo;
           // EditableFile loadXml = new EditableFile();
           
            string file = TreeItem.FileSystemInfo.FullName;
            return LoadFilePreview(file, Items);
                //Transform();
        }
        private System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

        private string defaultAttributeColor = "#67CDFE";
        private string defaultAttributeValueColor = "#CE9178";
        private string defaultTagNameColor = "#569CCA";
        private string defaultCommentColor = "#6A9955";
        public void defaultStyle(ObservableCollection<EditableFile> Items)
        {
            StreamReader stream = new StreamReader("Highlighting/XML.xshd");
            string xshd = stream.ReadToEnd();
            stream.Close();
            stream.Dispose();

            xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"AttributeName\"))", defaultAttributeColor);

            xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"AttributeValue\"))", defaultAttributeValueColor);
            xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"CData\"))", defaultAttributeValueColor);
            xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"DocType\"))", defaultAttributeValueColor);

            xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"XmlTag\"))", defaultTagNameColor);
            xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"XmlDeclaration\"))", defaultTagNameColor);
            xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"BrokenEntity\"))", defaultTagNameColor);

            xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"Comment\"))", defaultCommentColor);

            File.WriteAllText("Highlighting/XML.xshd", xshd);

            for (int i = 0; i < Items.Count(); i++)
            {
                Items.ElementAt(i).TextEditor.Paiting();
            }
            //SelectedItem.TextEditor.SyntaxHighlighting = HighlightingLoader.Load(new XmlTextReader("Highlighting/XML.xshd"), HighlightingManager.Instance);
            //SelectedItem.TextEditor.TextArea.TextView.Redraw();
        }
        public void repaintStyle(ObservableCollection<EditableFile> Items, string param)
        {
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string color = ColorTranslator.ToHtml(colorDialog.Color);

                StreamReader stream = new StreamReader("Highlighting/XML.xshd");
                string xshd = stream.ReadToEnd();
                stream.Close();
                stream.Dispose();


                switch (param)
                {
                    case "Attribute":
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"AttributeName\"))", color);
                        break;

                    case "AttributeValue":
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"AttributeValue\"))", color);
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"CData\"))", color);
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"DocType\"))", color);
                        break;

                    case "TagName":
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"XmlTag\"))", color);
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"XmlDeclaration\"))", color);
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"BrokenEntity\"))", color);
                        break;

                   /* case "TagBracket":
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"Comment\"))", color);
                        break;*/

                    case "Comment":
                        xshd = Regex.Replace(xshd, "(?<=Color foreground=\")(.*?)(?=(\" name=\"Comment\"))", color);
                        break;
                }
                File.WriteAllText("Highlighting/XML.xshd", xshd);

                for(int i = 0; i < Items.Count(); i++)
                {
                    Items.ElementAt(i).TextEditor.Paiting();
                }

                //SelectedItem.TextEditor.Paiting();

                //SelectedItem.TextEditor.SyntaxHighlighting = HighlightingLoader.Load(new XmlTextReader("Highlighting/XML.xshd"), HighlightingManager.Instance);
                //SelectedItem.TextEditor.TextArea.TextView.Redraw();
            }
        }

        #endregion
        /// <summary>
        /// Редактирование текста
        /// </summary>
        #region text

        public void NewPlace(EditableFile SelectedItem, int Line, int Col)
        {
            SelectedItem.TextEditor.ScrollTo(Line, Col);
            SelectedItem.TextEditor.TextArea.Caret.Position = new TextViewPosition(Line, Col);
            SelectedItem.TextEditor.Focus();
        }

        public void SetText(EditableFile SelectedItem, string param)
        {

            MyAvalonEditor TextEditor = SelectedItem.TextEditor;

            string[] tagStr = (param).Split('_');
            if (tagStr.Length == 3)
            {
                if (tagStr[1].Equals("cursor"))
                {
                    TextEditor.SelectionLength = 0;
                    TextEditor.Document.Insert(TextEditor.CaretOffset, tagStr[0] + tagStr[2]);

                    TextEditor.SelectionStart = TextEditor.SelectionStart - tagStr[2].Length;
                    TextEditor.Focus();
                }
                if (tagStr[1].Equals("cursorSelect"))
                {

                    TextEditor.Document.Insert(TextEditor.SelectionStart, tagStr[0]);
                    TextEditor.Document.Insert(TextEditor.SelectionStart + TextEditor.SelectionLength, tagStr[2]);

                    if (TextEditor.SelectionLength == 0)
                    {
                        TextEditor.SelectionStart = TextEditor.SelectionStart - tagStr[2].Length;
                    }
                    TextEditor.Focus();
                }

            }

            /*if (tagStr.Length == 2)
            {
                if (tagStr[1].Equals("cursor"))
                {
                    TextEditor.Document.Insert(TextEditor.CaretOffset, tagStr[0]);
                    //TextEditor.SelectionLength = 0;
                    TextEditor.Focus();

                    /* SelectedItem.FastTextBox.SelectedText = tagStr[0];
                     SelectedItem.FastTextBox.Focus();
                }
                if (tagStr[0].Equals("cursor"))
                {
                    TextEditor.Document.Insert(TextEditor.CaretOffset, tagStr[0]);

                    TextEditor.CaretOffset -= tagStr[0].Length;
                    TextEditor.Focus();

                    /*SelectedItem.FastTextBox.SelectedText = tagStr[0];
                    SelectedItem.FastTextBox.SelectionStart = SelectedItem.FastTextBox.SelectionStart - tagStr[0].Length;
                    SelectedItem.FastTextBox.Focus();
                }
            }*/

            if (tagStr.Length == 1)
            {
                TextEditor.Document.Insert(TextEditor.SelectionStart, tagStr[0]);
                TextEditor.SelectionLength = 0;
                TextEditor.Focus();
            }
        }

        #endregion

        /// <summary>
        /// Сервис
        /// </summary>
        #region service

        public string ChangeXslt()
        {
            if (currentXslt == tg_builder)
            {
                currentXslt = serna;
            }
            else currentXslt = tg_builder;
            return currentXslt.Name;
        }

        public Page ChangePage()
        {
            if (RenderPage == tablePage)
            {
                RenderPage = browserPage;
            }
            else RenderPage = tablePage;

            return RenderPage;
        }

        public void UpdateFile(EditableFile SelectedItem)
        {
            try
            {
                SelectedItem.TextEditor.Text = File.ReadAllText(SelectedItem.Path);
                SelectedItem.IsChangedText = false;
                //Transform(SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        XDocument xDox = new XDocument();
        public EditableFile RenderItem = new EditableFile();
        public bool TestTipePage() {
            PageItem = RenderItem;
           // XDocument xDox = null;
            bool errorXml = false;

            if (RenderItem.TextEditor.Text.Length > 0)
            {
                try
                {
                    xDox = XDocument.Parse(RenderItem.TextEditor.Text);
                }
                catch (XmlException ex)
                {
                    errorXml = true;

                    NewPlace(RenderItem, ex.LineNumber, ex.LinePosition);
                    Error = ex.ToString();
                    MessageBox.Show(Error);
                    
                }

            }

            else errorXml = true;

            return errorXml;
        }

        public Page GetCurentPage()
        {
            Page Curent = browserPage;
          
         

         //   browserPage.DataContext = BrowserViewModel;

            foreach (XElement elm in xDox.Descendants("content"))
            {
                if (elm.Element("ipc") != null)
                {
                    Curent = tablePage;

                }
                if (elm.Element("descript") != null)
                {
                    Curent = browserPage;
                }
            }

            return Curent;

           

            //  TableViewModels.getData(xDox, SelectedItem.TextEditor);
            // BrowserViewModels.getData(xDox, MyXslt.Path, SelectedItem.Path);
        }

        private void BrowserSet()
        {
            BrowserViewModel.Address = browserModel.setData(xDox, currentXslt.Path, RenderItem.Path);

            if (browserModel.Warning == true)
            {
                BrowserViewModel.WarningVisible = true;
                BrowserViewModel.Warnings = browserModel.WarningsList;
                Warning = browserModel.WarningsList;
            }
            else BrowserViewModel.WarningVisible = false;
        }
        private void TableSet()
        {
            tableModel.setData(xDox, RenderItem.TextEditor, TableViewModel.Table);

            TableViewModel.Name = tableModel.Name;
        }
        public Page Transform(EditableFile Item)
        {
            RenderItem = Item;

        
            bool errorXml = TestTipePage();

            if (errorXml == false)
            {
                RenderPage = GetCurentPage();

                BrowserSet();
                TableSet();
            }
            else
            {
                TableViewModel.Table.Clear();
                TableViewModel.Name = "";

                BrowserViewModel.Address = "about:blank";
            }
            //RenderPage.Error = Error;
            //RenderPage.Warnings = Warning;

            return RenderPage;
        }
       /* public void TransformWithWarning(EditableFile Item, Page CurrentPage, string Error, string Warning)
        {
            this.RenderPage.Page = Transform(Item);
            CurrentPage = this.RenderPage.Page;

            Error = this.Error;
            Warning = this.Warning;
        }*/
        /*
       public Page Transform(EditableFile SelectedItem)
        {
            Page CurrentPage = browserPage;
            PageItem = SelectedItem;
            XDocument xDox = null;
            bool errorXml = false;

            if (SelectedItem.TextEditor.Text.Length > 0)
            {
                try
                {
                    xDox = XDocument.Parse(SelectedItem.TextEditor.Text);
                }
                catch (XmlException ex)
                {
                    errorXml = true;

                    NewPlace(SelectedItem, ex.LineNumber, ex.LinePosition);

                    MessageBox.Show(ex.ToString());
                }

            }
            else errorXml = true;

            if (errorXml == false)
            {
                foreach (XElement elm in xDox.Descendants("content"))
                {
                    if (elm.Element("ipc") != null)
                    {
                        CurrentPage = tablePage;

                    }
                    if (elm.Element("descript") != null)
                    {
                        CurrentPage = browserPage;
                    }
                }

                BrowserViewModel.Address = browserModel.getData(xDox, currentXslt.Path, SelectedItem.Path);

                if(browserModel.Warning == true)
                {
                    BrowserViewModel.WarningVisible = true;
                    BrowserViewModel.Warnings = browserModel.WarningsList;
                }
                else BrowserViewModel.WarningVisible = false;

                tableModel.getData(xDox, SelectedItem.TextEditor, TableViewModel.Table);

                TableViewModel.Name = tableModel.Name;
                // BrowserViewModels.getData(xDox, MyXslt.Path, SelectedItem.Path);
            }
            else
            {
               // TableViewModels.TableClear();
                BrowserViewModel.Address = "about:blank";
            }

            return CurrentPage;
        }

        */

            #endregion
            /// <summary>
            /// Сохраненные настройки
            /// </summary>
            #region properties
        public void LoadedFilesFromProperies(ObservableCollection<EditableFile> Items)
        {
           // Items = new ObservableCollection<MyItem>();

            if (!Properties.Settings.Default.Files.Equals(""))
            {
                string[] files = Properties.Settings.Default.Files.Split('|');
                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetExtension(files[i]).Equals(".xml"))
                    {
                        try
                        {
                            LoadFile(files[i], Items);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                            // MessageBox.Show(files[i] + "\nФайл не найден");
                        }
                    }
                }

                if (Items.Count > 0)
                {
                    Transform(Items[Items.Count-1]);
                    //WorkspaceVisible = true;
                };

            }

            if (Items.Count == 0)
            {
                Items.Insert(Items.Count, CreateNewItem());
            }
        }
        public bool LoadFolderFormProperties(ObservableCollection<FileSystemObjectInfo> WorkFolder)
        {
            string folder = Properties.Settings.Default.Folder;
            bool res = true;
            if (!folder.Equals(""))
            {
                string oldFolder = OpenedFolder;
                try
                {
                    OpenedFolder = folder;
                    updateFolder(WorkFolder);
                    // WorkspaceVisible = true;
                }
                catch (Exception)
                {

                    MessageBox.Show(OpenedFolder + "\nПапка не найдена");
                    OpenedFolder = oldFolder;
                    res = false;
                }

            }

            return res;
        }

        public void SaveProperties(ObservableCollection<EditableFile> Items, ObservableCollection<FileSystemObjectInfo> WorkFolder)
        {
            if (Items.Count > 0)
            {
                string files = "";
                foreach (var item in Items)
                {
                    if (item.Path != null)
                    {
                        files += '|' + item.Path;
                    }
                }
                Properties.Settings.Default.Files = files;
            }
            else Properties.Settings.Default.Files = "";

            if (WorkFolder.Count > 0)
            {
                Properties.Settings.Default.Folder = WorkFolder[0].FileSystemInfo.FullName;
            }
            else Properties.Settings.Default.Folder = "";


            Properties.Settings.Default.Save();

        }

        #endregion
    }
}
