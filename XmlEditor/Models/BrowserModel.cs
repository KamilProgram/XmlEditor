using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace XmlEditor.Models
{
    class BrowserModel
    {
        string pathTmp = Directory.GetCurrentDirectory() + "\\tmpSPOXmlView.html";

       public string WarningsList = "";

       public bool Warning = false;
       // string Warnings = "";


        public void CheckLink(string S)
        {
            Warning = false;

            Regex regex = new Regex("(?<=file://)(.*?)(.svg|.blend|.png)(?=\")");
            MatchCollection msac = regex.Matches(S);
            string res = "";
            string link;

            int i = 0;

            foreach (Match mat in msac)
            {
                link = mat.ToString();
                if (!File.Exists(link))
                {
                    Warning = true;
                    res += link + "\n";
                    i++;
                }
            }
            WarningsList = res;
           // WarningsList = "Не обнаружено " + i + " файлов";
            //  MessageBox.Show(res);
        }
        
        public void RefreshHtml(string path)
        {
            StreamReader stream = new StreamReader(pathTmp);
            string html = stream.ReadToEnd();

            string st = Path.GetDirectoryName(path);
            st = Regex.Replace(st, @"\\", "/");

            html = Regex.Replace(html, @"data=\""", "data=\"" + "file://" + st + "/");
            html = Regex.Replace(html, @"src=\""", "src=\"" + "file://" + st + "/");
            html = Regex.Replace(html, @"href=\""", "href=\"" + "file://" + st + "/");
            stream.Close();
            stream.Dispose();

            CheckLink(html);

            File.WriteAllText(pathTmp, html);
        }
        public string setData(XDocument xDox, string xsltPath, string path)
        {
            StreamWriter writer = new StreamWriter(pathTmp);
            try
            {
                XsltSettings setXs = new XsltSettings();
                setXs.EnableDocumentFunction = true;

                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xsltPath, setXs, new XmlUrlResolver());

                XmlReaderSettings settngs = new XmlReaderSettings();
                settngs.DtdProcessing = DtdProcessing.Parse;

                XmlReader reader = XmlReader.Create(xDox.CreateReader(), settngs);

                xslt.Transform(reader, null, writer);
                writer.Close();
                writer.Dispose();

                RefreshHtml(path);

                string flLink = Regex.Replace(pathTmp, @"\\", "/");
                flLink = "file://" + flLink;

                reader.Close();
                reader.Dispose();

                return flLink;
            }
            catch (Exception ex)
            {
                writer.Close();
                writer.Dispose();

                MessageBox.Show(ex.ToString());

                return "about: blank";
            }
        }
    }
}
