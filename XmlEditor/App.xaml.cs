using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace XmlEditor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
           // Init();
        }

        private void Init()
        {
            try
            {
                var path = Path.Combine(Application.StartupPath, "Highlighting");
                var files = Directory.GetFiles(path).Where(x =>
                {
                    var extension = Path.GetExtension(x);
                    return extension != null && extension.Contains("xshd");
                });
                foreach (var file in files)
                {
                    var definition = LoadXshdDefinition(file);
                    var hightlight = LoadHighlightingDefinition(file);
                    HighlightingManager.Instance.RegisterHighlighting(definition.Name, definition.Extensions.ToArray(), hightlight);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //TODO: поставить логирование
            }
        }

        private static XshdSyntaxDefinition LoadXshdDefinition(string fullName)
        {
            using (var reader = new XmlTextReader(fullName))
                return HighlightingLoader.LoadXshd(reader);
        }


        private static IHighlightingDefinition LoadHighlightingDefinition(string fullName)
        {
            using (var reader = new XmlTextReader(fullName))
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }
}
