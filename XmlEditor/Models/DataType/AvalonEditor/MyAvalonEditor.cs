using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using NHunspell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Xml;

namespace XmlEditor.Models.DataType.AvalonEditor
{
    public class MyAvalonEditor : ICSharpCode.AvalonEdit.TextEditor, INotifyPropertyChanged
    {
        // Vars.
        // private static bool canScroll = true;

       // BehaviorCollection Behavior = Interaction.GetBehaviors(new SpellCheckerBehavior());

        /// <summary>
        /// Default constructor to set up event handlers.
        /// </summary>

       

        public MyAvalonEditor()
        {
            // Default options.
       
            ShowLineNumbers = true;
            //SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("XML");

            var converter = new BrushConverter();
            Background = (Brush)converter.ConvertFromString("#282C34");

            Foreground = Brushes.White;
            WordWrap = true;

            SetValue(FoldingMargin.FoldingMarkerBackgroundBrushProperty, (Brush)converter.ConvertFromString("#3B4052"));
            SetValue(FoldingMargin.SelectedFoldingMarkerBackgroundBrushProperty, (Brush)converter.ConvertFromString("#3B4052"));
            SetValue(FoldingMargin.SelectedFoldingMarkerBrushProperty, (Brush)converter.ConvertFromString("#EAEAEA")); 

            TextChanged += MyTextEditor_TextChanged;
            foldingManager = FoldingManager.Install(TextArea);
            
            SpellChecker.Default.HunspellInstance = new Hunspell(@".\Dictionaries\ru_RU.aff", @".\Dictionaries\ru_RU.dic");
            TextArea.TextView.LineTransformers.Add(new SpellCheckerColorizer());

            SyntaxHighlighting = HighlightingLoader.Load(new XmlTextReader("Highlighting/XML.xshd"),HighlightingManager.Instance);

            Loaded += MyTextEditor_Loaded;
          
          
        }

        public void Paiting()
        {
            SyntaxHighlighting = HighlightingLoader.Load(new XmlTextReader("Highlighting/XML.xshd"), HighlightingManager.Instance);
        }

        private void MyTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Interaction.GetBehaviors(sender as MyAvalonEditor).Add(new SpellCheckerBehavior());
            //SearchPanel.Install(sender as MyAvalonEditor);
        }

         
            
            private void MyTextEditor_TextChanged(object sender, EventArgs e)
        {
            MyAvalonEditor target = (MyAvalonEditor)sender;
            foldingStrategy.UpdateFoldings(foldingManager, Document);
        }

        FoldingManager foldingManager;
        XmlFoldingStrategy foldingStrategy = new XmlFoldingStrategy();


        /*
        #region Text.
        /// <summary>
        /// Dependancy property for the editor text property binding.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
             DependencyProperty.Register("Text", typeof(string), typeof(MyTextEditor),
             new PropertyMetadata((obj, args) =>
             {
                 MyTextEditor target = (MyTextEditor)obj;
  
                 target.Text = (string)args.NewValue;

             }));

        /// <summary>
        /// Provide access to the Text.
        /// </summary>
        public new string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Return the current text length.
        /// </summary>
        public int Length
        {
            get { return base.Text.Length; }
        }

        /// <summary>
        /// Override of OnTextChanged event.
        /// </summary>
        protected override void OnTextChanged(EventArgs e)
        {
            RaisePropertyChanged("Length");
            base.OnTextChanged(e);
        }

        /// <summary>
        /// Event handler to update properties based upon the selection changed event.
        /// </summary>
        void TextArea_SelectionChanged(object sender, EventArgs e)
        {
            this.SelectionStart = SelectionStart;
            this.SelectionLength = SelectionLength;
        }

        /// <summary>
        /// Event that handles when the caret changes.
        /// </summary>
        void TextArea_CaretPositionChanged(object sender, EventArgs e)
        {
            try
            {
                canScroll = false;
                this.TextLocation = TextLocation;
            }
            finally
            {
                canScroll = true;
            }
        }
        #endregion // Text.
        */
        /*   #region Caret Offset.
           /// <summary>
           /// DependencyProperty for the TextEditorCaretOffset binding. 
           /// </summary>
           public static DependencyProperty CaretOffsetProperty =
               DependencyProperty.Register("CaretOffset", typeof(int), typeof(CodeEditor),
               new PropertyMetadata((obj, args) =>
               {
                   CodeEditor target = (CodeEditor)obj;
                   if (target.CaretOffset != (int)args.NewValue)
                       target.CaretOffset = (int)args.NewValue;
               }));

           /// <summary>
           /// Access to the SelectionStart property.
           /// </summary>
           public new int CaretOffset
           {
               get { return base.CaretOffset; }
               set { SetValue(CaretOffsetProperty, value); }
           }
           #endregion // Caret Offset.*/

        /* #region Selection.
         /// <summary>
         /// DependencyProperty for the TextLocation. Setting this value 
         /// will scroll the TextEditor to the desired TextLocation.
         /// </summary>
         public static readonly DependencyProperty TextLocationProperty =
              DependencyProperty.Register("TextLocation", typeof(TextLocation), typeof(MyTextEditor),
              new PropertyMetadata((obj, args) =>
              {
                  MyTextEditor target = (MyTextEditor)obj;
                  TextLocation loc = (TextLocation)args.NewValue;
                  if (canScroll)
                      target.ScrollTo(loc.Line, loc.Column);
              }));

         /// <summary>
         /// Get or set the TextLocation. Setting will scroll to that location.
         /// </summary>
         public TextLocation TextLocation
         {
             get { return base.Document.GetLocation(SelectionStart); }
             set { SetValue(TextLocationProperty, value); }
         }

         /// <summary>
         /// DependencyProperty for the TextEditor SelectionLength property. 
         /// </summary>
         public static readonly DependencyProperty SelectionLengthProperty =
              DependencyProperty.Register("SelectionLength", typeof(int), typeof(MyTextEditor),
              new PropertyMetadata((obj, args) =>
              {
                  MyTextEditor target = (MyTextEditor)obj;
                  if (target.SelectionLength != (int)args.NewValue)
                  {
                      target.SelectionLength = (int)args.NewValue;
                      target.Select(target.SelectionStart, (int)args.NewValue);
                  }
              }));

         /// <summary>
         /// Access to the SelectionLength property.
         /// </summary>
         public new int SelectionLength
         {
             get { return base.SelectionLength; }
             set { SetValue(SelectionLengthProperty, value); }
         }

         /// <summary>
         /// DependencyProperty for the TextEditor SelectionStart property. 
         /// </summary>
         public static readonly DependencyProperty SelectionStartProperty =
              DependencyProperty.Register("SelectionStart", typeof(int), typeof(MyTextEditor),
              new PropertyMetadata((obj, args) =>
              {
                  MyTextEditor target = (MyTextEditor)obj;
                  if (target.SelectionStart != (int)args.NewValue)
                  {
                      target.SelectionStart = (int)args.NewValue;
                      target.Select((int)args.NewValue, target.SelectionLength);
                  }
              }));

         /// <summary>
         /// Access to the SelectionStart property.
         /// </summary>
         public new int SelectionStart
         {
             get { return base.SelectionStart; }
             set { SetValue(SelectionStartProperty, value); }
         }
         #endregion // Selection.*/

        /* #region Properties.
         /// <summary>
         /// The currently loaded file name. This is bound to the ViewModel 
         /// consuming the editor control.
         /// </summary>
         public string FilePath
         {
             get { return (string)GetValue(FilePathProperty); }
             set { SetValue(FilePathProperty, value); }
         }

         // Using a DependencyProperty as the backing store for FilePath. 
         // This enables animation, styling, binding, etc...
         public static readonly DependencyProperty FilePathProperty =
              DependencyProperty.Register("FilePath", typeof(string), typeof(CodeEditor),
              new PropertyMetadata(String.Empty, OnFilePathChanged));
         #endregion // Properties.*/

        #region Raise Property Changed.
        /// <summary>
        /// Implement the INotifyPropertyChanged event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
          public void RaisePropertyChanged([CallerMemberName] string caller = null)
          {
              var handler = PropertyChanged;
              if (handler != null)
                  PropertyChanged(this, new PropertyChangedEventArgs(caller));
          }
          #endregion // Raise Property Changed.
    }
}
