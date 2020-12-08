using System;
using System.IO;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Utils
{
    public class WordProcessing
    {
        private HRSContext context;
        private dynamic Data;
        private string DataSrcName;
        private string[] DataCol;
        private int ColNo;
        private TableRow[] DataRows;
        private bool IsStart;

        private const string CharSplitDataSource = ".";
        private const string BeginVariableTag = "«Var:"; // DataVar
        private const string BeginLanguageTag = "«Lang:"; // DataLang
        private const string BeginTableStartTag = "«TableStart:"; // DataList i:= ,1,2,3,4,5,6,7,8...
        private const string BeginColumnTag = "«";
        private const string BeginTableEndTag = "«TableEnd:";
        private const string EndTag = "»";
        private const int MaxCols = 100;
        private const int MaxWhileCount = 10000;

        public WordProcessing(string SourcePath, string DestinationPath, HRSContext _context, string Json = "")
        {
            if (Json != "")
                Data = JObject.Parse(Json);
            else
                Data = null;
            if (Data == null) return;
            context = _context;
            DataSrcName = ""; DataCol = null; ColNo = 0; DataRows = null; IsStart = false; 
            ReplaceContent(SourcePath, DestinationPath);
        }
        public WordProcessing(string SourcePath, string DestinationPath, string Json = "")
        {
            if (Json != "")
                Data = JObject.Parse(Json);
            else
                Data = null;
            if (Data == null) return;
            context = null;
            DataSrcName = ""; DataCol = null; ColNo = 0; DataRows = null; IsStart = false;
            ReplaceContent(SourcePath, DestinationPath);
        }
        ~WordProcessing()
        {
            context = null;
            Data = null;
        }
        private void ReplaceContent(string SourcePath, string DestinationPath)
        {
            byte[] docAsArray;
            if (context == null)
            {
                ToolFolder toolFolder = new ToolFolder("appsettings.json");
                docAsArray = toolFolder.FileReadAllBytes(SourcePath);// File.ReadAllBytes(SourcePath);
            }
            else
                docAsArray = context.AppConfig.FileReadAllBytes(SourcePath);// File.ReadAllBytes(SourcePath);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(docAsArray, 0, docAsArray.Length);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;
                    for (int ia = 0; ia < body.ChildElements.Count; ia++)
                    {
                        if (body.ChildElements[ia].GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                        {
                            var para = body.ChildElements[ia];
                           Console.WriteLine("para: " + para.ToString());
                            foreach (var run in para.Elements<Run>())
                            {
                               Console.WriteLine("run: " + run.ToString());
                                foreach (var text in run.Elements<Text>())
                                {
                                    string s = text.Text;
                                    s = ReplaceVarable(s, Data["DataVar"].Items[0], Data["DataVar"].ItemType);
                                    s = ReplaceLanguage(s);
                                   Console.WriteLine("Text: " + text.Text + "; s: " + s);
                                    text.Text = s;
                                }
                            }
                        }
                        else if (body.ChildElements[ia].GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Table")
                        {
                           //Console.WriteLine("Table: " + body.ChildElements[ia].ToString());
                            ProcessTable((Table)body.ChildElements[ia]);
                        }
                    }
                    doc.Close();
                }
                File.WriteAllBytes(DestinationPath, stream.ToArray());
                stream.Close();
                docAsArray = null;
            }
        }
        private void ProcessTable(Table table)
        {
           //Console.WriteLine("Table: " + table.ToString());
            var rows = table.Elements<TableRow>();
            foreach (var row in rows)
            {
               //Console.WriteLine("Row: " + row.ToString());
                var cells = row.Elements<TableCell>();
                foreach (var cell in cells)
                {
                   //Console.WriteLine("Cell: " + cell.ToString());
                    for (int l = 0; l < cell.ChildElements.Count; l++)
                    {
                        if (cell.ChildElements[l].GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Table")
                        {
                            ProcessTable((Table)cell.ChildElements[l]);
                        }
                        else if (cell.ChildElements[l].GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                        {
                            ProcessParaGraph((Paragraph)cell.ChildElements[l]);
                        }
                    }
                }
            }
            if (DataRows != null)
            {
                for (int i = 0; i < DataRows.Length; i++)
                    if (DataRows[i] != null)
                    {
                        table.Append(DataRows[i].CloneNode(true));
                        DataRows[i] = null;
                    }               
            }
        }
        private void ProcessParaGraph(Paragraph para)
        {
           Console.WriteLine("para: " + para.ToString());
            foreach (var run in para.Elements<Run>())
            {
               Console.WriteLine("run: " + run.ToString());
                foreach (var text in run.Elements<Text>())
                {
                   Console.WriteLine("Text: " + text.Text);
                    string s1 = text.Text;
                    s1 = ReplaceVarable(s1, Data["DataVar"].Items[0], Data["DataVar"].ItemType);
                    s1 = ReplaceLanguage(s1);
                    Console.WriteLine("Text: " + text.Text + "; s: " + s1);
                    if (!IsStart) ReplaceDataSourceStart(ref s1);
                    if (IsStart) s1 = SearchDataColumn(s1);
                    if (IsStart) ReplaceDataSourceEnd(ref s1);
                    text.Text = s1;
                }
            }
        }
        // Hàm dùng chung
        private string ReplaceVarable(string s, dynamic d, dynamic d1) // Xử lý biến
        {
            int i; int j;
            i = s.IndexOf(BeginVariableTag);
            j = s.IndexOf(EndTag);
            if (i == -1 || j == -1 || i == j)
            {
                return s;
            }
            else
            {
                int iWhile = 0; // chặn lỗi Out Of Memory
                while (!(i == -1 || j == -1 || i == j) && iWhile < MaxWhileCount)
                {
                    string s1; iWhile++;
                    s1 = s.Substring(i + BeginVariableTag.Length, j - i - BeginVariableTag.Length);
                    string ValS1 = Tools.GetDataFormatJson(d, d1, s1);
                    s = s.Replace(BeginVariableTag + s1 + EndTag, ValS1);
                    i = s.IndexOf(BeginVariableTag);
                    j = s.IndexOf(EndTag);
                }
                return s;
            }
        }
        private string ReplaceLanguage(string s) // Xử lý ngôn ngữ
        {
            if (context == null) return s;
            int i; int j;
            i = s.IndexOf(BeginLanguageTag);
            j = s.IndexOf(EndTag);
            if (i == -1 || j == -1 || i == j)
            {
                return s;
            }
            else
            {
                int iWhile = 0; // chặn lỗi Out Of Memory
                while (!(i == -1 || j == -1 || i == j) && iWhile < MaxWhileCount)
                {
                    string s1; iWhile++;
                    s1 = s.Substring(i + BeginLanguageTag.Length, j - i - BeginLanguageTag.Length);
                    s = s.Replace(BeginLanguageTag + s1 + EndTag, context.GetLanguageLable(s1));
                    i = s.IndexOf(BeginLanguageTag);
                    j = s.IndexOf(EndTag);
                }
                return s;
            }
        }
        private void ReplaceDataSourceStart(ref string s) // Check Tag table Start
        {
            int i; int j;
            i = s.IndexOf(BeginTableStartTag);
            j = s.IndexOf(EndTag);

            if (!(i == -1 || j == -1 || i == j))
            {
                string s1;
                s1 = s.Substring(i + BeginTableStartTag.Length, j - i - BeginTableStartTag.Length);
                s = s.Replace(BeginTableStartTag + s1 + EndTag, "");

                DataSrcName = s1; DataCol = null; DataCol = new string[MaxCols]; ColNo = 0; DataRows = null; IsStart = true;
                //DataSrcName = ""; DataCol = null; ColNo = 0; DataRows = null; IsStart = false; 
            }
        }
        private string SearchDataColumn(string s) // Xử lý từng cột trong DataSource
        {
            int i; 
            i = s.IndexOf(BeginTableEndTag);
            if (i > -1)
                DataCol[ColNo] = "";
            else
                DataCol[ColNo] = s;
            ColNo++;
            return ReplaceDataColumn(s);
        }
        private string ReplaceDataColumn(string s, int k = 0) // Xử lý dữ liệu Cell
        {
            int i; int j;
            i = s.IndexOf(BeginTableStartTag); // table start
            if (i >= 0) return s;
            i = s.IndexOf(BeginTableEndTag); // table end
            if (i >= 0) return s;
            i = s.IndexOf(BeginVariableTag); // var
            if (i >= 0) return s;
            i = s.IndexOf(BeginLanguageTag); // lang
            if (i >= 0) return s;
            i = s.IndexOf(BeginColumnTag);
            j = s.IndexOf(EndTag);
            if (i == -1 || j == -1 || i == j)
            {
                return s;
            }
            else
            {
                int iWhile = 0; // chặn lỗi Out Of Memory
                while (!(i == -1 || j == -1 || i == j) && iWhile < MaxWhileCount)
                {
                    string s1; iWhile++;
                    s1 = s.Substring(i + BeginColumnTag.Length, j - i - BeginColumnTag.Length);
                    string ValS1 = Tools.GetDataJson(Data[DataSrcName].Items[k], s1);
                    s = s.Replace(BeginColumnTag + s1 + EndTag, ValS1);
                    i = s.IndexOf(BeginColumnTag);
                    j = s.IndexOf(EndTag);
                }
                return s;
            }
        }
        private void ReplaceDataSourceEnd(ref string s) // Check Tag End
        {
            int i; int j;
            i = s.IndexOf(BeginTableEndTag);
            j = s.IndexOf(EndTag);

            if (!(i == -1 || j == -1 || i == j))
            {
                string s1;
                s1 = s.Substring(i + BeginTableEndTag.Length, j - i - BeginTableEndTag.Length);
                s = s.Replace(BeginTableEndTag + s1 + EndTag, "");
                DataRows = new TableRow[Data[DataSrcName].Items.Count + 1];
                // Console.WriteLine(DataSrcName + "_Count: " + (Data[DataSrcName].Items.Count + 1).ToString());
                for (int k = 1; k < Data[DataSrcName].Items.Count; k++)
                {
                   //Console.WriteLine(DataSrcName + "_Rows: " + k.ToString());
                    DataRows[k-1] = new TableRow();
                    for (int l = 1; l < ColNo; l++)
                    {
                        string Str = ReplaceDataColumn(DataCol[l], k);
                      // Console.WriteLine(DataSrcName + "_Cell: " + l.ToString() + " = " + Str);
                        DataRows[k-1].Append(new TableCell(new Paragraph(new Run(new Text(Str)))));
                    }
                }
                DataSrcName = ""; DataCol = null; ColNo = 0; IsStart = false;
                //DataSrcName = ""; DataCol = null; ColNo = 0; DataRows = null; IsStart = false;
            }
        }
    }
}
