using System.Windows;
namespace XmlEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += ViewModel.OnWindowsClosing;
        }
    }
}
