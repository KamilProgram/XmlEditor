using ICSharpCode.AvalonEdit.Document;
using XmlEditor.Models.DataType.AvalonEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XmlEditor.Models
{
    class TableModel
    {
        private MyAvalonEditor TextEditor;
       // TableRow SelectedRow;
       // public ObservableCollection<TableRow> Table { get; set; }
        //int SelectedRowIndex;

        public string Name;

        public void showHidenRow(Visibility show, int SelectedRowIndex, ObservableCollection<TableRow> Table)
        {

            int ind = Table[SelectedRowIndex].Ind;
            int rowInd = Table[SelectedRowIndex + 1].Ind;

            for (int row = SelectedRowIndex + 1; row < Table.Count; row++)
            {
                rowInd = Table[row].Ind;
                if (rowInd > ind)
                {
                    if (Table[row].Collaps == true)
                    {
                        Table[row].Collaps = false;
                    }

                    Table[row].ShowRow = show;

                }
                if (rowInd == ind)
                {
                    break;
                }
            }
        }


        public XDocument RefreshTable(XDocument xDox)
        {
            // StreamReader stream = new StreamReader(xDox);
            // string html = stream.ReadToEnd();

            // string st = Path.GetDirectoryName(path);
            // st = Regex.Replace(st, @"\\", "/");
            string xml = xDox.ToString();
            xml = Regex.Replace(xml, @"\^2", "&#x00b2;");
            xml = Regex.Replace(xml, @"\^3", "&#x00b3;");
            xDox = XDocument.Parse(xml);
            // stream.Close();
            // stream.Dispose();

            //CheckLink(html);
            return xDox;
            //File.WriteAllText(path, html);
        }

        public void setData(XDocument xDox, MyAvalonEditor TextEditor, ObservableCollection<TableRow> Table)
        {
           // ObservableCollection<TableRow> Table = new ObservableCollection<TableRow>();

            xDox = RefreshTable(xDox);
            this.TextEditor = TextEditor;

            Table.Clear();

            if (xDox.XPathSelectElement("dmodule/idstatus/dmaddres/dmtitle") != null)
            {
                Name = xDox.XPathSelectElement("dmodule/idstatus/dmaddres/dmtitle").Value;
            }

            int prInd = 0;
            int rowIndex = 0;

            try
            {
                foreach (XElement elm in xDox.Descendants("csn"))
                {
                    string item = "";
                    int ind = 1;

                    TableRow newRow = new TableRow();
                    newRow.Collaps = null;
                    newRow.ShowRow = Visibility.Visible;
                    if (elm.Attribute("ind") != null)
                    {

                        ind = int.Parse(elm.Attribute("ind").Value);


                        if (ind > prInd && rowIndex != 0)
                        {
                            Table[rowIndex - 1].Collaps = false;
                        }


                        prInd = ind;

                        newRow.Ind = ind;

                    }

                    if (elm.Attribute("item") != null)
                    {
                        item += elm.Attribute("item").Value;
                        newRow.Item = item;
                    }

                    if (elm.Element("name") != null)
                    {
                        newRow.Name = elm.Element("name").Value;
                    }

                    Table.Insert(Table.Count, newRow);
                    rowIndex++;
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(e.ToString());
                MessageBox.Show("Невозможно отобразить " + (rowIndex + 1) + " строку таблицы");
            }

           // return Table;
        }

        public string selectedRowRegex(TableRow SelectedRow)
        {
            string res = "";
            res = "<csn ind=\"" + SelectedRow.Ind + "\" item=\"" + SelectedRow.Item + "\">";
            return res;
        }

        public void findTag(TableRow SelectedRow)
        {
            //string opentg = selectedRowRegex(SelectedRow);

            string opentg = "<csn ind=\"" + SelectedRow.Ind + "\" item=\"" + SelectedRow.Item + "\">";
            string closetg = opentg + "([\\s\\S]*?)</csn>";

            TextEditor.Focus();

            MatchCollection tag = Regex.Matches(TextEditor.Text, closetg);
            foreach (Match tg in tag)
            {
                TextEditor.SelectionStart = tg.Index;
                TextEditor.SelectionLength = tg.Length;
                TextLocation loc = TextEditor.TextArea.Document.GetLocation(TextEditor.SelectionStart);
                TextEditor.ScrollTo(loc.Line, loc.Column);
                // TextEditor.DoSelectionVisible();
              break;
            }
        }
    }
}
