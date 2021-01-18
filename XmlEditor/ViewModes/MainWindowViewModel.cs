using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Data;
using System.Globalization;

using GongSolutions.Wpf.DragDrop;
using System.Drawing;

using System.Diagnostics;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
using XmlEditor.Models.DataType.ExploerTree;
using XmlEditor.Models.DataType;
using XmlEditor.Models;
using ICSharpCode.AvalonEdit.Editing;
using XmlEditor.Models.DataType.AvalonEditor;

namespace XmlEditor.ViewModels
{

    public class MainWindowViewModel : INotifyPropertyChanged, IDropTarget
    {
        public ObservableCollection<FileSystemObjectInfo> WorkFolder { get; set; }
        public ObservableCollection<EditableFile> Items { get; set; }

 

        MainWindowModel model = new MainWindowModel();
        
       // EditableFile PageItem = null;
        
        public MainWindowViewModel()
        {
            WorkFolder = new ObservableCollection<FileSystemObjectInfo>();
            Items = new ObservableCollection<EditableFile>();

            bool FolderFromProperties = model.LoadFolderFormProperties(WorkFolder);

            TreeVisible = FolderFromProperties;
            WorkspaceVisible = FolderFromProperties;

            model.LoadedFilesFromProperies(Items);

            OnCurrentPageTab = true;
            OnAutoTransform = false;
            ErrorVisible = false;

            ToolTipVisible = Properties.Settings.Default.TipsVisible;
            RenderVisible = Properties.Settings.Default.RenderVisible;
            ListVisible = true;
       
            TagVisible = true;
            ToolsVisible = true;

            CurrentPage = model.browserPage;
            MyXslt = model.currentXslt.Name;

            
            //Line = model.LineNum;
        }
        /// <summary>
        /// Перетаскиванием
        /// </summary>
        #region dragDrop
       
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {

            /*EditableFile sourceItem = dropInfo.Data as EditableFile;
            EditableFile targetItem = dropInfo.TargetItem as EditableFile;

            if(sourceItem != null && targetItem != null && targetItem.CanAcceptCgildren)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
            */
           
         
               try
              {

              DragDropEffects Effect = new DragDropEffects();
              // Effect = DragDropEffects.Copy;

              var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
              Effect = dragFileList.Any(item =>
                  {
                      var extension = Path.GetExtension(item as string);

                      dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

                      return extension != null && (extension.Equals(".xml") || Directory.Exists(item));
                  }) ? DragDropEffects.Copy : DragDropEffects.None;

              dropInfo.Effects = Effect;

              
              }
              catch
              {
                  dropInfo.Effects = DragDropEffects.None;
              }
        }
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            /*EditableFile sourceItem = dropInfo.Data as EditableFile;
            EditableFile targetItem = dropInfo.TargetItem as EditableFile;
            Items.Add(sourceItem);
            */
           
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                if (dragFileList.Count() == 1 && Directory.Exists(dragFileList.ElementAt(0)))
                {
                    model.newFolder(WorkFolder,dragFileList.ElementAt(0));
                }
                else
                {
                    for (int i = 0; i < dragFileList.Count(); i++)
                    {
                        if (Path.GetExtension(dragFileList.ElementAt(i)).Equals(".xml"))
                        {

                            model.LoadFile(dragFileList.ElementAt(i),Items);
                        }
                    }
                    SelectedItem = Items[Items.Count - 1];
                   CurrentPage =  model.Transform(SelectedItem);
                }
                var extension = Path.GetExtension(item);
                return extension != null && (extension.Equals(".xml") || Directory.Exists(item));

            }) ? DragDropEffects.Copy : DragDropEffects.None;
        }
        #endregion

        /// <summary>
        /// Работу с отдельными файлами
        /// </summary>
        #region file

        private RelayCommand openFileCommand;
        public RelayCommand OpenFileCommand
        {
            get
            {
                return openFileCommand ?? (openFileCommand = new RelayCommand(obj =>
                {

                    EditableFile newItem = model.OpenFile(Items);
                    if (newItem!= null)
                    {
                        SelectedItem = newItem;
                    }
                  
                    //OpenFile();
                }));
            }
        }

        private RelayCommand saveAsCommand;
        public RelayCommand SaveAsCommand
        {
            get
            {
                return saveAsCommand ?? (saveAsCommand = new RelayCommand(obj =>
                {
                    model.SaveAs(SelectedItem);
                }, (obj) => Items.Count > 0));
            }
        }



        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(obj =>
                {
                   model.Save(SelectedItem);

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand copyPathCommand;
        public RelayCommand CopyPathCommand
        {
            get
            {
                return copyPathCommand ?? (copyPathCommand = new RelayCommand(obj =>
                {
                    if (SelectedItem.Path != null)
                    {
                        Clipboard.SetText(SelectedItem.Path);
                    }

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand copyNameCommand;
        public RelayCommand CopyNameCommand
        {
            get
            {
                return copyNameCommand ?? (copyNameCommand = new RelayCommand(obj =>
                {
                    if (SelectedItem.Header != null)
                    {
                        Clipboard.SetText(SelectedItem.Header);
                    }

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand newFileCommand;
        public RelayCommand NewFileCommand
        {
            get
            {
                return newFileCommand ?? (newFileCommand = new RelayCommand(obj =>
                {
                    Items.Insert(Items.Count, model.CreateNewItem());
                }));
            }
        }

        private RelayCommand renameCommand;
        public RelayCommand RenameCommand
        {
            get
            {
                return renameCommand ?? (renameCommand = new RelayCommand(obj =>
                {
                  
                    SelectedItem.IsRename = true;
                    model.OldFileName = SelectedItem.Header;
                    SelectedItem.Header = Path.GetFileNameWithoutExtension(SelectedItem.Header);
                }));
            }
        }

        private RelayCommand renameStop;
        public RelayCommand RenameStop
        {
            get
            {
                return renameStop ?? (renameStop = new RelayCommand(obj =>
                {
                    EditableFile SelectedRename = (obj as EditableFile);
                    SelectedRename.IsRename = false;
                    SelectedRename.Path = model.rename(SelectedRename.Path, SelectedRename.Header);
                
                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand renameCancel;
        public RelayCommand RenameCancel
        {

            get
            {
                return renameCancel ?? (renameCancel = new RelayCommand(obj =>
                {
                    EditableFile SelectedRename = (obj as EditableFile);
                    SelectedRename.IsRename = false;
                    SelectedRename.Header = model.OldFileName;

                }, (obj) => Items.Count > 0));
            }
        }
        #endregion

        /// <summary>
        /// Операции с элементами коллекции Items
        /// </summary>
        #region items


        private string header;
        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }

        private EditableFile selectedItem;
        public EditableFile SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (selectedItem != null)
                {
                    selectedItem.TextEditor.TextArea.Caret.PositionChanged += TextArea_CaretPositionChanged;
                    selectedItem.TextEditor.TextChanged += TextEditor_TextChanged;

                    Header = selectedItem.Header;
                }
                else Header = "";

                OnPropertyChanged("SelectedItem");
            }
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            SelectedItem.IsChangedText = true;
        }

        private void TextArea_CaretPositionChanged(object sender, EventArgs e)
        {
            Caret caret = sender as Caret;
            Line = caret.Line;
            Col = caret.Column;
        }

        private int selectedItemIndex;
        public int SelectedItemIndex
        {
            get { return selectedItemIndex; }
            set
            {
                selectedItemIndex = value;
                OnPropertyChanged("SelectedItemIndex");
            }
        }

        private RelayCommand closeAllItemsCommand;
        public RelayCommand CloseAllItemsCommand
        {
            get
            {
                return closeAllItemsCommand ?? (closeAllItemsCommand = new RelayCommand(obj =>
                {
                    model.ClosingAllItems(Items);
                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand closeItemCommand;
        public RelayCommand CloseItemCommand
        {
            get
            {
                return closeItemCommand ?? (closeItemCommand = new RelayCommand(obj =>
                {
                     model.ClosingItem(Items, obj as EditableFile);
                }, (obj) => Items.Count > 0));
            }
        }

        #endregion

        /// <summary>
        /// Работа с проводником
        /// </summary>
        #region folder


        private RelayCommand newFolderCommand;
        public RelayCommand NewFolderCommand
        {
            get
            {
                return newFolderCommand ?? (newFolderCommand = new RelayCommand(obj =>
                {

                    model.newFolder(WorkFolder);

                }));
            }
        }

        private RelayCommand openFolderCommand;
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return openFolderCommand ?? (openFolderCommand = new RelayCommand(obj =>
                {
                    if (WorkFolder.Count != 0)
                    {
                       Process.Start(model.OpenedFolder);
                    }
                }));
            }
        }

        private RelayCommand updateFolderCommand;
        public RelayCommand UpdateFolderCommand
        {
            get
            {
                return updateFolderCommand ?? (updateFolderCommand = new RelayCommand(obj =>
                {
                    if (WorkFolder.Count != 0)
                    {
                        model.updateFolder(WorkFolder);
                    }
                }));
            }
        }


        private RelayCommand loadXmlPre;
        public RelayCommand LoadXmlPre
        {
            get
            {
                return loadXmlPre ?? (loadXmlPre = new RelayCommand(obj =>
                {
                    var TreeItem = obj as FileSystemObjectInfo;
                    if (TreeItem.IsFile == true)
                    {
                        SelectedItemIndex = model.loadXmlPre(obj as FileSystemObjectInfo, Items);
                    }
                   
                }));
            }
        }

        private RelayCommand loadXml;
        public RelayCommand LoadXml
        {
            get
            {
                return loadXml ?? (loadXml = new RelayCommand(obj =>
                {
                    Items[Items.Count - 1].IsPreview = false;
                }));
            }
        }

        #endregion

        /// <summary>
        /// Правка
        /// </summary>
        #region correction
        private RelayCommand undoCommand;
        public RelayCommand UndoCommand
        {
            get
            {
                return undoCommand ?? (undoCommand = new RelayCommand(obj =>
                {
                   SelectedItem.TextEditor.Undo();

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand redoCommand;
        public RelayCommand RedoCommand
        {
            get
            {
                return redoCommand ?? (redoCommand = new RelayCommand(obj =>
                {
                    SelectedItem.TextEditor.Redo();

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand cutCommand;
        public RelayCommand CutCommand
        {
            get
            {
                return cutCommand ?? (cutCommand = new RelayCommand(obj =>
                {
                   SelectedItem.TextEditor.Cut();

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand copyCommand;
        public RelayCommand CopyCommand
        {
            get
            {
                return copyCommand ?? (copyCommand = new RelayCommand(obj =>
                {           
                  SelectedItem.TextEditor.Copy();

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand pasteCommand;
        public RelayCommand PasteCommand
        {
            get
            {
                return pasteCommand ?? (pasteCommand = new RelayCommand(obj =>
                {
                    if (Clipboard.ContainsText())
                    {
                       SelectedItem.TextEditor.Paste();
                    }
                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand findCommand;
        public RelayCommand FindCommand
        {
            get
            {
                return findCommand ?? (findCommand = new RelayCommand(obj =>
                {
                    //model.FindDialog(SelectedItem.TextEditor);
                    //FindReplaceDialog.ShowForReplace(SelectedItem.TextEditor);
                    FindReplaceDialog.ShowForFind(SelectedItem.TextEditor);

                }, (obj) => Items.Count > 0));
            }
        }
      

       private RelayCommand changeCommand;
        public RelayCommand ChangeCommand
        {
            get
            {
                return changeCommand ?? (changeCommand = new RelayCommand(obj =>
                {
                    //model.ReplaceDialog(SelectedItem.TextEditor);
                    // FindReplaceDialog.ShowForReplace(SelectedItem.TextEditor);
                    FindReplaceDialog.ShowForReplace(SelectedItem.TextEditor);

                }, (obj) => Items.Count > 0));
            }
        }

        /*
        private RelayCommand hotKeysCommand;
        public RelayCommand HotKeysCommand
        {
            get
            {
                return hotKeysCommand ?? (hotKeysCommand = new RelayCommand(obj =>
                {
                    

                }));
            }
        }*/

        #endregion

        /// <summary>
        /// Сервис
        /// </summary>
        #region service

        private bool onCurrentPageTab;
        public bool OnCurrentPageTab
        {
            get { return onCurrentPageTab; }
            set
            {
                onCurrentPageTab = value;
                OnPropertyChanged("OnCurrentPageTab");
            }
        }

        private bool onAutoTransform;
        public bool OnAutoTransform
        {
            get { return onAutoTransform; }
            set
            {
                onAutoTransform = value;
                OnPropertyChanged("OnAutoTransform");
            }
        }

        private RelayCommand transformCommand;
        public RelayCommand TransformCommand
        {
            get
            {
                return transformCommand ?? (transformCommand = new RelayCommand(obj =>
                {
                    CurrentPage = model.Transform(SelectedItem);
                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand transformTabCommand;
        public RelayCommand TransformTabCommand
        {
            get
            {
                return transformTabCommand ?? (transformTabCommand = new RelayCommand(obj =>
                {
                   if (onAutoTransform == true)
                    {
                        CurrentPage = model.Transform(SelectedItem);
                    }                   
                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand updateFileCommand;
        public RelayCommand UpdateFileCommand
        {
            get
            {
                return updateFileCommand ?? (updateFileCommand = new RelayCommand(obj =>
                {
                    model.UpdateFile(SelectedItem);
                    CurrentPage = model.Transform(SelectedItem);

                }, (obj) => Items.Count > 0));
            }
        }


        private RelayCommand currentPageTab;
        public RelayCommand CurrentPageTab
        {
            get
            {
                return currentPageTab ?? (currentPageTab = new RelayCommand(obj =>
                {

                    if (OnCurrentPageTab == true)
                    {
                        if (SelectedItem != model.RenderItem)
                        {
                            SelectedItem = model.RenderItem;
                        }
                    }
 
                    //   MessageBox.Show(model.RenderItem.Header);

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand changePageCommand;
        public RelayCommand ChangePageCommand
        {
            get
            {
                return changePageCommand ?? (changePageCommand = new RelayCommand(obj =>
                {
                    CurrentPage = model.ChangePage();
                }));
            }
        }

        private RelayCommand changeXsltCommand;
        public RelayCommand ChangeXsltCommand
        {
            get
            {
                return changeXsltCommand ?? (changeXsltCommand = new RelayCommand(obj =>
                {
                    MyXslt = model.ChangeXslt();
                }));
            }
        }

        private Page currentPage;
        public Page CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (currentPage == value)
                    return;

                currentPage = value;
               // Error = currentPage.Error;
               // Warnings = currentPage.Warnings;
                //currentPage.MouseLeftButtonDown += CurrentPage_MouseLeftButtonDown;
                OnPropertyChanged("CurrentPage");
            }
        }
        private int clickCount = 0;
        private void CurrentPage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            clickCount++;

            if(clickCount == 2)
            {
                clickCount = 0;
                   if (OnCurrentPageTab == true)
                      {
                         /* if (SelectedItem != model.RenderItem)
                          {
                              SelectedItem = model.RenderItem;
                          }*/
                      }
                SelectedItemIndex = 0;
            }
        }

        private string myXslt;
        public string MyXslt
        {
            get { return myXslt; }
            set
            {
                if (myXslt == value)
                    return; 

                myXslt = value;
                OnPropertyChanged("MyXslt");
            }
        }

        #endregion

        

        /// <summary>
        /// Редактирование текста
        /// </summary>
        #region text

        private int line;
        public int Line
        {
            get { return line; }
            set
            {
                line = value;
                OnPropertyChanged("Line");
            }
        }

        private int col;
        public int Col
        {
            get { return col; }
            set
            {
                col = value;
                OnPropertyChanged("Col");
            }
        }


        private int warningsCount = 0;
        public int WarningsCount
        {
            get { return warningsCount; }
            set
            {
                warningsCount = value;
                OnPropertyChanged("WarningsCount");
            }
        }


        private int errorCount = 0;
        public int ErrorCount
        {
            get { return errorCount; }
            set
            {
                errorCount = value;
                OnPropertyChanged("ErrorCount");
            }
        }

        private string warnings;
        public string Warnings
        {
            get { return warnings; }
            set
            {
                warnings = value;
                if (!warnings.Equals(""))
                {
                    WarningsCount = (value.Split('\n').Length - 1);
                }
                else WarningsCount = 0;

              
                OnPropertyChanged("Warnings");
            }
        }
        private string error;
        public string Error
        {
            get { return Error; }
            set
            {
                error = value;
                if (!error.Equals(""))
                {
                    ErrorCount = 1;
                } else  ErrorCount = 0;

                OnPropertyChanged("Error");
            }
        }
        private RelayCommand setPosition;
        public RelayCommand SetPosition
        {
            get
            {
                return setPosition ?? (setPosition = new RelayCommand(obj =>
                {
                    model.NewPlace(SelectedItem, Line, Col);

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand setText;
        public RelayCommand SetText
        {
            get
            {
                return setText ?? (setText = new RelayCommand(obj =>
                {
                    model.SetText(SelectedItem, obj as string);
                }, (obj) => Items.Count > 0));
            }
        }
        #endregion

        /// <summary>
        /// Вид
        /// </summary>
        #region visible

        private bool toolTipVisible;
        public bool ToolTipVisible
        {
            get { return toolTipVisible; }
            set
            {             
                Properties.Settings.Default.TipsVisible = value;
                toolTipVisible = value;
                OnPropertyChanged("ToolTipVisible");
            }
        }

        private bool renderVisible;
        public bool RenderVisible
        {
            get { return renderVisible; }
            set
            {
                Properties.Settings.Default.RenderVisible = value;
                renderVisible = value;
                OnPropertyChanged("RenderVisible");
            }
        }

        private bool workspaceVisible;
        public bool WorkspaceVisible
        {
            get { return workspaceVisible; }
            set
            {
                workspaceVisible = value;
                OnPropertyChanged("WorkspaceVisible");
            }
        }

        private bool listVisible;
        public bool ListVisible
        {
            get { return listVisible; }
            set
            {
                listVisible = value;
                OnPropertyChanged("ListVisible");
            }
        }

        private bool treeVisible;
        public bool TreeVisible
        {
            get { return treeVisible; }
            set
            {
                treeVisible = value;
                OnPropertyChanged("TreeVisible");
            }
        }

        private bool tagVisible;
        public bool TagVisible
        {
            get { return tagVisible; }
            set
            {
                tagVisible = value;
                OnPropertyChanged("TagVisible");
            }
        }

        private bool errorVisible;
        public bool ErrorVisible
        {
            get { return errorVisible; }
            set
            {
                errorVisible = value;
                OnPropertyChanged("ErrorVisible");
            }
        }

        private bool toolsVisible;
        public bool ToolsVisible
        {
            get { return toolsVisible; }
            set
            {
                //Properties.Settings.Default.TipsVisible = value;
                toolsVisible = value;
                OnPropertyChanged("ToolsVisible");
            }
        }

        /// <summary>
        /// Команды отвечающие за смену цвета элементов текстового поля
        /// </summary>
        #region colors

        private RelayCommand repaintStyleCommand;
        public RelayCommand RepaintStyleCommand
        {
            get
            {
                return repaintStyleCommand ?? (repaintStyleCommand = new RelayCommand(obj =>
                {
                    model.repaintStyle(Items, obj as string);

                }, (obj) => Items.Count > 0));
            }
        }

        private RelayCommand defaultStyleCommand;
        public RelayCommand DefaultStyleCommand
        {
            get
            {
                return defaultStyleCommand ?? (defaultStyleCommand = new RelayCommand(obj =>
                {
                    model.defaultStyle(Items);
                }, (obj) => Items.Count > 0));
            }
        }
        #endregion

        #endregion

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

        private RelayCommand close;
        public RelayCommand Close
        {
            get
            {
                return close ?? (close = new RelayCommand(obj =>
                {
                    (obj as Window).Close();
                    //MessageBox.Show("Work");
                }));
            }
        }

        public void OnWindowsClosing(object sender,CancelEventArgs e)
        {
            model.SaveProperties(Items, WorkFolder);
           if (model.ClosingAllItems(Items) == false)
             {
                e.Cancel = true;
             } //else FindReplaceDialog.FindReplaceDialogClose();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
