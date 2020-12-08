using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Utils
{
    public class PlaintextProcessing
    {
        private HRSContext context;
        private dynamic Data;
        private string DataSrcName;

        private const string CharSplitDataSource = ".";
        private const string BeginVariableTag = "«Var:"; // DataVar
        private const string BeginLanguageTag = "«Lang:"; // DataLang
        private const string BeginTableStartTag = "«TableStart:"; // DataList i:= ,1,2,3,4,5,6,7,8...
        private const string BeginColumnTag = "«";
        private const string BeginTableEndTag = "«TableEnd:";
        private const string EndTag = "»";
        private const int MaxCols = 100;
        private const int MaxWhileCount = 10000;

        public string DestinationData;
        private string SourceData;

        public PlaintextProcessing(string _SourceData, HRSContext _context, string Json = "")
        {
            SourceData = _SourceData;
            if (Json != "")
                Data = JObject.Parse(Json);
            else
                Data = null;
            if (Data == null) return;
            if (Data["DataVar"] == null) return;
            context = _context;

            DestinationData = ReplaceVarable(_SourceData, Data["DataVar"].Items[0], Data["DataVar"].ItemType);
            DestinationData = ReplaceLanguage(DestinationData);
            DestinationData = FindDataSource(DestinationData);
        }
        public PlaintextProcessing(string _SourceData, string Json = "")
        {
            SourceData = _SourceData;
            if (Json != "")
                Data = JObject.Parse(Json);
            else
                Data = null;
            if (Data == null) return;
            if (Data["DataVar"] == null) return;
            context = null;

            DestinationData = ReplaceVarable(_SourceData, Data["DataVar"].Items[0], Data["DataVar"].ItemType);
            DestinationData = FindDataSource(DestinationData);
        }

        // Hàm dùng chung
        private void GetIndexOf(string s, string _BeginTag, string _EndTag, out int i, out int j)
        {
            i = s.IndexOf(_BeginTag);
            if (i > -1)
                j = s.IndexOf(_EndTag, i);
            else
                j = -1;
        }
        private string ReplaceVarable(string s, dynamic d, dynamic d1) // Xử lý biến
        {
            int i; int j;
            GetIndexOf(s, BeginVariableTag, EndTag, out i, out j);
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
                    GetIndexOf(s, BeginVariableTag, EndTag, out i, out j);
                }
                return s;
            }
        }
        private string ReplaceLanguage(string s) // Xử lý ngôn ngữ
        {
            int i; int j;
            GetIndexOf(s, BeginLanguageTag, EndTag, out i, out j);
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
                    GetIndexOf(s, BeginLanguageTag, EndTag, out i, out j);
                }
                return s;
            }
        }
        private string FindDataSource(string s)
        {
            int iWhile = 0; // chặn lỗi Out Of Memory

            DataSrcName = SeachDataSourceStart(s);
            while (DataSrcName != "" && iWhile < MaxWhileCount)
            {
                string DSStart =    BeginTableStartTag +    DataSrcName + EndTag;
                string DSEnd =      BeginTableEndTag +      DataSrcName + EndTag;
                int i; int j; string s1 = ""; string data = "";
                GetIndexOf(s, DSStart, DSEnd, out i, out j);
                if (!(i == -1 || j == -1 || i == j))
                {
                    s1 = s.Substring(i + DSStart.Length, j - i - DSStart.Length);
                    string s01 = s.Substring(0, i);
                    string s02 = s.Substring(j + DSEnd.Length);
                    string s03 = DSStart + "[" + DataSrcName + "]" + DSEnd;
                    s = s01 + s03 + s02;
                    // xử lý S1
                    for (int k = 0; k < Data[DataSrcName].Items.Count; k++)
                    {
                        data = data + ReplaceDataSource(s1, Data[DataSrcName].Items[k], Data[DataSrcName].ItemType);
                        //data = "<tr><td>" + data;
                    }
                    // thay thế data
                    s = s.Replace(s03, data);
                }
                DataSrcName = SeachDataSourceStart(s);
                iWhile++;
            }

            return s;
        }
        private string ReplaceDataSource(string s1, dynamic d, dynamic d1)
        {
            int i; int j;
            GetIndexOf(s1, BeginColumnTag, EndTag, out i, out j);
            if (!(i == -1 || j == -1 || i == j))
            {
                int iWhile = 0; // chặn lỗi Out Of Memory
                while (!(i == -1 || j == -1 || i == j) && iWhile < MaxWhileCount)
                {
                    string s11; iWhile++;
                    s11 = s1.Substring(i + BeginColumnTag.Length, j - i - BeginColumnTag.Length);
                    string ValS1 = Tools.GetDataFormatJson(d, d1, s11);
                    s1 = s1.Replace(BeginColumnTag + s11 + EndTag, ValS1);
                    GetIndexOf(s1, BeginColumnTag, EndTag, out i, out j);
                }
                return s1;
            }
            return "";
        }
        private string SeachDataSourceStart(string s)
        {
            int i; int j; string s1 = "";
            GetIndexOf(s, BeginTableStartTag, EndTag, out i, out j);
            if (!(i == -1 || j == -1 || i == j))
            {                
                s1 = s.Substring(i + BeginTableStartTag.Length, j - i - BeginTableStartTag.Length);
                s = s.Replace(BeginTableStartTag + s1 + EndTag, "");
            }
            return s1;
        }
    }
}
