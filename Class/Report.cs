using System;
using System.Text;
using OfficeOpenXml;
using Newtonsoft.Json.Linq;

namespace Utils
{
    public class Report
    {
        #region Properties
        public string RptID;
        public string MenuID;
        public string MenuOn;
        public dynamic Rpt;
        private HRSContext context;
        private ToolDAO DataDAO;
        private ToolDAO ConfigDAO;
        private string DataOutput;
        #endregion

        #region Contruction
        /// <summary>
        /// Khởi tạo lớp báo cáo cho các màn hình báo cáo
        /// </summary>
        /// <param name="_context">Lớp HTTP_Context xử lý ngôn ngữ; HttpRequest</param>
        /// <param name="_ConfigDAO">Lớp xử lý DB (Config = Schema BOS)</param>
        /// <param name="_DataDAO">Lớp xử lý DB (dữ liệu = Schema Công ty khách hàng)</param>
        /// <param name="_ReportIndex">Mã báo cáo</param>
        /// <param name="_MenuOn">HttpRequest-MenuOn=Off thì ẩn menu;</param>
        /// <param name="_MenuID">HttpRequest-MenuID -> ID chức năng</param>
        /// <param name="_DataOutput">HttpRequest-DataOutput -> Kết quả báo cáo HTML-Web/Excel file </param>
        public Report(HRSContext _context, ToolDAO _ConfigDAO, ToolDAO _DataDAO, string _ReportIndex, string _MenuOn, string _MenuID, string _DataOutput)
        {
            context = _context; ConfigDAO = _ConfigDAO; DataDAO = _DataDAO;
            RptID = _ReportIndex; // _context.GetRequestRptID();
            MenuOn = _MenuOn; // _context.GetRequestMenuOn();
            MenuID = _MenuID; // _context.GetRequestOneValue("MenuID");
            DataOutput = _DataOutput;
            SetParam();
        }
        /// <summary>
        /// Khởi tạo lớp báo cáo cho màn hình Daskboard
        /// </summary>
        /// <param name="_context">Lớp HTTP_Context xử lý ngôn ngữ; HttpRequest</param>
        /// <param name="_ConfigDAO">Lớp xử lý DB (Config = Schema BOS)</param>
        /// <param name="_DataDAO">Lớp xử lý DB (dữ liệu = Schema Công ty khách hàng)</param>
        /// <param name="_ReportIndex">Mã báo cáo</param>
        /// <param name="_MenuOn">HttpRequest-MenuOn=Off thì ẩn menu</param>
        /// <param name="_MenuID">HttpRequest-MenuID -> ID chức năng</param>
        public Report(HRSContext _context, ToolDAO _ConfigDAO, ToolDAO _DataDAO, string _ReportIndex, string _MenuOn, string _MenuID)
        {
            context = _context; ConfigDAO = _ConfigDAO; DataDAO = _DataDAO;
            RptID = _ReportIndex; // _context.GetRequestRptID();
            MenuOn = _MenuOn; // _context.GetRequestMenuOn();
            MenuID = _MenuID; // _context.GetRequestOneValue("MenuID");
            DataOutput = "text/html";
            SetParamDashboard();
        }
        #endregion

        #region Private Method
        private void SetParam()
        {
            dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            //json = context.GetSession("RptConfig_" + RptID);
            //if (json == "")
            bool IsCached = context._cache.Get("RptConfig_" + RptID + "_" + context.GetSession("language"), out d);
            if (!IsCached)
            {
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"ReportConfigID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"-1\"}," +
                    "{\"ParamName\":\"ReportConfigCode\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"40\", \"InputValue\":\"" + RptID + "\"}," +
                    "{\"ParamName\":\"Keyword\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"Page\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                    "{\"ParamName\":\"PageSize\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + context.GetSession("PageSizeReport") + "\"}," +
                    "{\"ParamName\":\"Rowcount\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
                ConfigDAO.ExecuteStore("RptConfig", "SP_CMS__ReportConfig_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                //json = context.ReplaceListSessionValue(json);
                Rpt = JObject.Parse(json);
                //context.SetSession("RptConfig_" + RptID, json);
                context._cache.Set("RptConfig_" + RptID + "_" + context.GetSession("language"), Rpt);
                MenuID = Tools.GetDataJson(Rpt.RptConfig.Items[0], "FunctionID");
            }
            else
            {
                //Rpt = JObject.Parse(json);
                Rpt = d;
                MenuID = Tools.GetDataJson(Rpt.RptConfig.Items[0], "FunctionID");
            }
        }
        private void SetParamDashboard()
        {
            dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            //json = context.GetSession("DashboardConfig_" + RptID);
            bool IsCached = context._cache.Get("DashboardConfig_" + RptID + "_" + context.GetSession("language"), out d);
            //if (json == "")
            if (!IsCached)
            {
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"DashboardConfigID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"-1\"}," +
                    "{\"ParamName\":\"DashboardConfigCode\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"40\", \"InputValue\":\"" + RptID + "\"}," +
                    "{\"ParamName\":\"Keyword\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"Page\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                    "{\"ParamName\":\"PageSize\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + context.GetSession("PageSizeReport") + "\"}," +
                    "{\"ParamName\":\"Rowcount\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
                ConfigDAO.ExecuteStore("RptConfig", "SP_CMS__DashboardConfig_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                //json = context.ReplaceListSessionValue(json);
                Rpt = JObject.Parse(json);
                //context.SetSession("DashboardConfig_" + RptID, json);
                context._cache.Set("DashboardConfig_" + RptID + "_" + context.GetSession("language"), Rpt);
                MenuID = Tools.GetDataJson(Rpt.RptConfig.Items[0], "FunctionID");
            }
            else
            {
                Rpt = d;// JObject.Parse(json);
                MenuID = Tools.GetDataJson(Rpt.RptConfig.Items[0], "FunctionID");
            }
        }

        // Gen row daskboard
        //  ReportName5: Report Name; ReportList5: Report URL; n: Row no; Row count
        private string SetURL(string ReportName5, string ReportList5, int n=5, int m = 1)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            if (ReportName5 != "")
            {
                // ReportName5 - ReportList5
                r1.Append(Environment.NewLine + "<!--row dashboard " + n.ToString() + "-->" +
                    Environment.NewLine + "<div class=\"row dashboard\">");
                string[] a = ReportName5.Split(new string[] { "^" }, StringSplitOptions.None); // Tách box - Name
                string[] b = ReportList5.Split(new string[] { "^" }, StringSplitOptions.None); // Tách box - URL
                int wd = (int)(12/a.Length); // default class col-wd with 12 pie/ count box
                int wl = 12; // class col-wl (column lasted. Start full 12 pie)
                for (int i = 0; i < a.Length; i++)
                {
                    string a_i = a[i]; // Name + ';' + col class No (4 => col-4; 5 => col-5 [1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 12 - Pie])
                    int idx = a_i.IndexOf(";");
                    int w = 0;
                    if (idx > 0)
                    {
                        a_i = a[i].Substring(0, idx);
                        try { w = int.Parse(a[i].Substring(idx + 1)); } catch { w = 0; }
                    }
                    if (w == 0 || w > 12) w = wd;

                    string[] a1 = a_i.Split(new string[] { "||" }, StringSplitOptions.None); // Tách Report - Name
                    string[] b1 = b[i].Split(new string[] { "||" }, StringSplitOptions.None); // Tách Report - URL 
                    string c = b1[0];
                    if (i == (a.Length - 1))
                        w = wl;
                    else
                        wl = (wl - w > 0? wl - w : 0);

                    r1.Append(Environment.NewLine + "<!--col-" + w.ToString() + "-->" +
                        Environment.NewLine + "<div class=\"col-" + w.ToString() + "\">");
                    if (a1.Length == 1)
                    {
                        r1.Append(Environment.NewLine + "<font class=\"titlepage\">" + context.GetLanguageLable(a1[0]) + "</font>");
                    }
                    else
                    {
                        string outBuff = UIDef.SelectStrOption(b[i], a_i, ref c, false);
                        r1.Append("<select class=\"select2 form-control titlepage\" " +
                            "id=\"cb_" + n + "_" + i + "\" " +
                            "name=\"cb_" + n + "_" + i + "\" " +
                            "onChange=\"document.getElementById('subw" + n + "_" + i + "').src=this.value+'&height=" + ((int)(650 / m)).ToString() + "';\">" + outBuff + "</select>");
                    }
                    r1.Append(Environment.NewLine + "<!--embed-responsive embed-responsive-16by9-->" +
                        Environment.NewLine + "<div class=\"embed-responsive embed-responsive-16by9\">");
                    r1.Append(Environment.NewLine + "<iframe id='subw" + n + "_" + i + "' class=\"embed-responsive-item\" " +
                        "allowfullscreen scrolling=\"no\" " +
                        //"onload=\"resizeIframe(this)\" " +
                        "frameborder=\"0\" " +
                        "src=\"" + b1[0] +
                        "&Header=Off" + // Close Header
                        //"&height=" + ((int)(650 / m)).ToString() + 
                        "\"></iframe>");
                    r1.Append(Environment.NewLine + "</div>" +
                        Environment.NewLine + "<!--/embed-responsive embed-responsive-16by9-->");
                    r1.Append(Environment.NewLine + "</div>" +
                        Environment.NewLine + "<!--/col-" + w.ToString() + "-->");
                }
                r1.Append(Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!--/row dashboard" + n.ToString() + "-->");
            }
            r = r1.ToString(); r1 = null;
            return r;
        }
        #endregion        

        #region UI Dashboard
        public string UIDashboard()
        {
            StringBuilder r1 = new StringBuilder(); string r = ""; int Cnt = 1;
            string ReportName1 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportName1"); 
            string ReportList1 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportList1"); 
            string ReportName2 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportName2"); if (ReportName2 != "") Cnt++;
            string ReportList2 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportList2");
            string ReportName3 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportName3"); if (ReportName3 != "") Cnt++;
            string ReportList3 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportList3"); 
            string ReportName4 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportName4"); if (ReportName4 != "") Cnt++;
            string ReportList4 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportList4"); 
            string ReportName5 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportName5"); if (ReportName5 != "") Cnt++;
            string ReportList5 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportList5");

            r1.Append(Environment.NewLine + "<script>" +
                Environment.NewLine + "function resizeIframe(obj) {" +
                Environment.NewLine + "   obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';" +
                Environment.NewLine + "}" +
                Environment.NewLine + "</script>"); 
            // ReportName1 - ReportList1
            r1.Append(SetURL(ReportName1, ReportList1, 1, Cnt));
            // ReportName2 - ReportList2
            r1.Append(SetURL(ReportName2, ReportList2, 2, Cnt));
            // ReportName3 - ReportList3
            r1.Append(SetURL(ReportName3, ReportList3, 3, Cnt));
            // ReportName4 - ReportList4
            r1.Append(SetURL(ReportName4, ReportList4, 4, Cnt));
            // ReportName5 - ReportList5
            r1.Append(SetURL(ReportName5, ReportList5, 5, Cnt));            
            
            r = r1.ToString(); r1 = null;
            return r;
        }
        #endregion

        #region UI From/List
        public string UIFilterForm()
        {
            StringBuilder b = new StringBuilder(); string r = ""; bool IsHTML = (DataOutput == "text/html");
            b.Append(Environment.NewLine + "<form onsubmit=\"return false\" name=\"FilterForm\" method=\"POST\">" +
                UIDef.UIHidden("MenuOn", MenuOn) +
                UIDef.UIHidden("MenuID", MenuID) +
                UIDef.UIHidden("RptID", RptID));
            //b.Append(Environment.NewLine + "<table width=\"100%\" class=\"formsearch\">");
            string[] a1; string[] a2; string[] a3;
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter");
            string QueryStringLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringLable"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            if (QueryStringFilter == "") return "";
            a1 = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            a2 = QueryStringLable.Split(new string[] { "^" }, StringSplitOptions.None);
            a3 = QueryStringType.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] a4 = new string[a1.Length];
            string[] DataVal = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataTxt = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataParent = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            StringBuilder jsdbcols; jsdbcols = new StringBuilder(); //bool IsFilterForm = false;
            if (IsHTML) jsdbcols.Append(Environment.NewLine + "var jsdbcols = new Array();" +
                    Environment.NewLine + "var DataVal = new Array();" +
                    Environment.NewLine + "var DataTxt = new Array();" +
                    Environment.NewLine + "var DataParent = new Array();");
            string json = ""; StringBuilder bx = new StringBuilder();
            //UIFormElements.UIFillterFormRpt(ref bx, ref jsdbcols, ref IsFilterForm, ref json, context, DataDAO, a1, a2, a3, a4, ";");
            string[] lblButton = { "" }; string[] StoreName = { "" };
            UIFormElements.UIFillterForm(ref bx, ref jsdbcols, ref json, context, DataDAO, a1, a2, a3, a4, lblButton, StoreName);
           
            b.Append(Environment.NewLine + bx.ToString());
            b.Append(Environment.NewLine + "</form>");
            if (IsHTML) b.Append(Environment.NewLine + "<script language=\"javascript\">" + jsdbcols.ToString());
            if (IsHTML) b.Append(Tools.GenResetFunc("nextfocus") +
                 Tools.GenResetFunc("cnextfocus"));
            if (IsHTML) b.Append(Environment.NewLine + "function btnSearch (){document.FilterForm.submit();}");
            if (IsHTML) b.Append(Environment.NewLine + "</script>");
            r = b.ToString(); b = null;
            return r;
        }
        public ExcelPackage UIFillToExcelFormV01(bool IsPost = false)
        {
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType"); // 1||2||3||4;List||Pivot||Form||Other
            string UrlOrigina = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlOriginal"); 
            if (!IsPost) return null;
            if (ReportType != "3") return null;
            if (UrlOrigina == "") return null;
            UrlOrigina = context.AppConfig.FolderRoot + UrlOrigina.Replace("||", "\\");
            if (!context.AppConfig.PathIsFile(UrlOrigina)) return null;
            ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(UrlOrigina));

            // Param chung
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            string SchemaName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 

            // Param ADODB
            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";

            // Store Pivot
            string DataPivot = "";
            string[] SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPPivotName").Split(new string[] { "$" }, StringSplitOptions.None);
            string[] ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamPivotList").Split(new string[] { "$" }, StringSplitOptions.None);
            string spname = SchemaName + "." + SPListName[0];

            Tools.ExecParam("PivotData", QueryStringFilter, ParamList[0], "", QueryStringType, context, spname, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            if (errorCode == 200)
                DataPivot = Tools.RemoveLastChar(json);
            else
                HTTP_CODE.WriteLogAction("json: " + json + "; errorString: " + errorString, context);
            for (int l = 1; l < SPListName.Length; l++)
            {
                parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                spname = SchemaName + "." + SPListName[l];
                Tools.ExecParam("PivotData" + l, QueryStringFilter, ParamList[l], "", QueryStringType, context, spname, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
                if (errorCode == 200)
                    DataPivot = DataPivot + "," + Tools.RemoveFisrtChar(Tools.RemoveLastChar(json));
                else
                    HTTP_CODE.WriteLogAction("json: " + json + "; errorString: " + errorString, context);
            }
            if (DataPivot != "")
                DataPivot = DataPivot + "}";
            else
                DataPivot = "{\"DataPivot\":\"NonPivot\"}";

            // Get Language Data
            //string DataLang = context.GetSession("json-language");

            // Get Param datalist
            string[] a = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            string DataParam = "{" +
                "\"UserName\":\"" + context.GetSession("UserName") + "\", " +
                "\"CompanyCode\":\"" + context.GetSession("CompanyCode") + "\"," +
                "\"CompanyName\":\"" + context.GetSession("CompanyName") + "\"";
            //for (int l = 0; l < a.Length; l++)
            //{
            //    DataParam = DataParam + ",\"" + a[l] + "\":\"" + context.GetRequestVal(a[l]) + "\"";
            //}
            //DataParam = DataParam + "}"; //Lấy thêm từ ParamOutput

            // Get dataSourcelist
            string DataAll = "";
            SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName").ToString().Split(new string[] { "$" }, StringSplitOptions.None);
            ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList").Split(new string[] { "$" }, StringSplitOptions.None);
            spname = SchemaName + "." + SPListName[0];

            parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
            Tools.ExecParam("ListData", QueryStringFilter, ParamList[0], "", QueryStringType, context, spname, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);

            parameterOutput = Tools.RemoveFisrtChar(((dynamic)JObject.Parse("{" + parameterOutput + "}")).ParameterOutput.ToString());
            if (parameterOutput != "")//Lấy thêm từ ParamOutput
            {
                DataParam = DataParam + "," + parameterOutput;
            }
            else
                DataParam = DataParam + "}"; //Lấy thêm từ ParamOutput

            //if (errorCode == 200)
            DataAll = Tools.RemoveLastChar(json);
            //else
            //    HTTP_CODE.WriteLogAction("json: " + json + "; errorString: " + errorString, context);
            for (int l = 1; l < SPListName.Length; l++)
            {
                parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                spname = SchemaName + "." + SPListName[l];
                Tools.ExecParam("ListData" + l, QueryStringFilter, ParamList[l], "", QueryStringType, context, spname, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
                //if (errorCode == 200)
                    DataAll = DataAll + "," + Tools.RemoveFisrtChar(Tools.RemoveLastChar(json));
                //else
                 //   HTTP_CODE.WriteLogAction("json: " + json + "; errorString: " + errorString, context);
            }
            if (DataAll != "") DataAll = DataAll + "}";
            if (DataAll == "") { HTTP_CODE.WriteLogAction("DataAll: Is NULL", context); return null; }

            // Excute Excel
            ReadExcelForm ac = new ReadExcelForm(package, context, DataParam, DataAll, DataPivot);
            return package;
        }
        public string UIListForm(bool IsHTML = true)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            StringBuilder r11 = new StringBuilder(); StringBuilder r12 = new StringBuilder(); StringBuilder r13 = new StringBuilder();
            StringBuilder r14 = new StringBuilder(); StringBuilder r15 = new StringBuilder(); StringBuilder r16 = new StringBuilder();
            StringBuilder r17 = new StringBuilder(); StringBuilder r18 = new StringBuilder(); StringBuilder r19 = new StringBuilder();
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType"); // 1||2||3||4;List||Pivot||Form||Other
            // Store list
            dynamic dList = null; bool IsSum = false; bool IsHeaderOff = (context.GetRequestVal("Header") == "Off");
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList"); 
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName"); 
            string ColumnLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnLable"); 
            string ColumnType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnType"); 
            string ColumnGroup = ""; int ColumnSpan = 0; int ColumnSumStart = 0;
            string[] a = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnGroup").Split(new string[] { "||" }, StringSplitOptions.None);
            if (a.Length > 0) ColumnGroup = a[0];
            if (a.Length > 1) ColumnSpan = int.Parse(a[1]);
            if (a.Length > 2) ColumnSumStart = int.Parse(a[2]);

            string UrlLink1 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink1"); 
            string UrlLink2 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink2"); 
            string UrlLink3 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink3"); 
            string UrlLink4 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink4"); 
            string UrlLink5 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink5"); 

            if (ReportType != "1") return ""; // Bao cao list 

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            Tools.ExecParam("ListForm", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dList = JObject.Parse(json);
            r1.Append(Environment.NewLine + "<form onsubmit=\"return false\" name=\"ListForm\" method=post>" +
                UIDef.UIHidden("MenuOn", MenuOn) +
                UIDef.UIHidden("MenuID", MenuID) +
                UIDef.UIHidden("RptID", RptID));
            //r1.Append(Environment.NewLine + "<table width=\"100%\" cellspacing=\"1\" cellpadding=\"1\"><tbody><tr><td nowrap>");

            // bang danh sach
            a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] b; b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] c; c = ColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] d; d = ColumnGroup.Split(new string[] { "^" }, StringSplitOptions.None); ColumnGroup = "^" + ColumnGroup + "^";
            double[] SumCol1 = new double[b.Length]; string PrevCol1 = ""; string PrevColName1=""; string PrevColLable1 = ""; if (d.Length > 0) if (d[0] != "") { PrevColName1 = a[int.Parse(d[0])]; PrevColLable1 = b[int.Parse(d[0])]; }
            double[] SumCol2 = new double[b.Length]; string PrevCol2 = ""; string PrevColName2 = ""; string PrevColLable2 = ""; if (d.Length > 1) if (d[1] != "") { PrevColName2 = a[int.Parse(d[1])]; PrevColLable2 = b[int.Parse(d[1])]; }
            double[] SumCol3 = new double[b.Length]; string PrevCol3 = ""; string PrevColName3 = ""; string PrevColLable3 = ""; if (d.Length > 2) if (d[2] != "") { PrevColName3 = a[int.Parse(d[2])]; PrevColLable3 = a[int.Parse(d[2])]; }
            double[] SumCol4 = new double[b.Length]; string PrevCol4 = ""; string PrevColName4 = ""; string PrevColLable4 = ""; if (d.Length > 3) if (d[3] != "") { PrevColName4 = a[int.Parse(d[3])]; PrevColLable4 = a[int.Parse(d[3])]; }
            double[] SumCol5 = new double[b.Length]; string PrevCol5 = ""; string PrevColName5 = ""; string PrevColLable5 = ""; if (d.Length > 4) if (d[4] != "") { PrevColName5 = a[int.Parse(d[4])]; PrevColLable5 = a[int.Parse(d[4])]; }
            double[] SumCol6 = new double[b.Length]; string PrevCol6 = ""; string PrevColName6 = ""; string PrevColLable6 = ""; if (d.Length > 5) if (d[5] != "") { PrevColName6 = a[int.Parse(d[5])]; PrevColLable6 = a[int.Parse(d[5])]; }
            double[] SumCol7 = new double[b.Length]; string PrevCol7 = ""; string PrevColName7 = ""; string PrevColLable7 = ""; if (d.Length > 6) if (d[6] != "") { PrevColName7 = a[int.Parse(d[6])]; PrevColLable7 = a[int.Parse(d[6])]; }
            double[] SumCol8 = new double[b.Length]; string PrevCol8 = ""; string PrevColName8 = ""; string PrevColLable8 = ""; if (d.Length > 7) if (d[7] != "") { PrevColName8 = a[int.Parse(d[7])]; PrevColLable8 = a[int.Parse(d[7])]; }
            double[] SumCol9 = new double[b.Length]; string PrevCol9 = ""; string PrevColName9 = ""; string PrevColLable9 = ""; if (d.Length > 8) if (d[8] != "") { PrevColName9 = a[int.Parse(d[8])]; PrevColLable9 = a[int.Parse(d[8])]; }
            double[] SumAllCol = new double[b.Length];

            r1.Append(Environment.NewLine + "<table class=\"table no-scroll table-hover use-icon border-none\" id=\"dbtablist\">");

            if (!IsHeaderOff) r1.Append(Environment.NewLine + "<thead class=\"thead-light\"><tr>");
            for (int i = 0; i < a.Length; i++)
            {
                string[] a4 = c[i].Split(new string[] { ";" }, StringSplitOptions.None);
                if (!IsHeaderOff) if (!(a4[0] == "-" || ColumnGroup.IndexOf("^" + i + "^") > -1)) r1.Append(Environment.NewLine + "<th nowrap>" + context.GetLanguageLable(b[i]) + "</th>");
                // set default sum/prev
                SumAllCol[i] = 0; SumCol1[i] = 0; SumCol2[i] = 0; SumCol3[i] = 0; SumCol4[i] = 0; SumCol5[i] = 0; SumCol6[i] = 0; SumCol7[i] = 0; SumCol8[i] = 0; SumCol9[i] = 0;
            }
            if (!IsHeaderOff) r1.Append(Environment.NewLine + "</tr></thead>");
            r1.Append(Environment.NewLine + "<tbody>");
            
            // duyệt dữ liệu theo từng dòng
            for (int i = 0; i < dList.ListForm.Items.Count; i++)
            {
                // check group col
                Tools.SetGroupData(PrevColName9, ref PrevCol9, PrevColLable9, ref SumCol9, ref r19,
                    ref dList, i, context,
                    ref r18, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName8, ref PrevCol8, PrevColLable8, ref SumCol8, ref r18,
                    ref dList, i, context,
                    ref r17, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName7, ref PrevCol7, PrevColLable7, ref SumCol7, ref r17,
                    ref dList, i, context,
                    ref r16, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName6, ref PrevCol6, PrevColLable6, ref SumCol6, ref r16,
                    ref dList, i, context,
                    ref r15, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName5, ref PrevCol5, PrevColLable5, ref SumCol5, ref r15,
                    ref dList, i, context,
                    ref r14, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName4, ref PrevCol4, PrevColLable4, ref SumCol4, ref r14,
                    ref dList, i, context,
                    ref r13, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName3, ref PrevCol3, PrevColLable3, ref SumCol3, ref r13,
                    ref dList, i, context,
                    ref r12, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName2, ref PrevCol2, PrevColLable2, ref SumCol2, ref r12,
                    ref dList, i, context,
                    ref r11, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName1, ref PrevCol1, PrevColLable1, ref SumCol1, ref r11,
                    ref dList, i, context,
                    ref r1, ColumnSpan, ColumnSumStart,
                    b, c);

                r19.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "\">");
                // Duyệt dữ liệu theo từng cột
                int ILink = 0;
                for (int j = 0; j < b.Length; j++)
                {
                    string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                    string val = "";
                    try { val = Tools.GetDataJson(dList.ListForm.Items[i], a[j]); } catch { val = ""; }
                    if (val == null) val = "";
                    if (ColumnGroup.IndexOf("^" + j + "^") < 0)
                        switch (a4[0])
                        {
                            case "HREF":
                                if (!IsHTML)
                                    r19.Append(Environment.NewLine + "<td>" + val);
                                else
                                {
                                    ILink = ILink + 1;
                                    switch (ILink)
                                    {
                                        case 1:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink1 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                            break;
                                        case 2:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink2 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                            break;
                                        case 3:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink3 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                            break;
                                        case 4:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink4 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                            break;
                                        case 5:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink5 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                            break;
                                        default:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + a4[1] + "(" + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\">" + val + "</a>");
                                            break;
                                    }
                                }                                
                                break;
                            case "Date":
                                DateTime val1;
                                try
                                {
                                    val1 = DateTime.Parse(val);
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMDate(val1));
                                }
                                catch
                                {
                                    //val1 = DateTime.Now;
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">");// + Tools.HRMDate(val1));
                                }
                                break;
                            case "Datetime":
                                DateTime val2;
                                try
                                {
                                    val2 = DateTime.Parse(val);
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMDateTime(val2));
                                }
                                catch
                                {
                                    //val2 = DateTime.Now;
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">");// + Tools.HRMDateTime(val2));
                                }
                                break;
                            case "Time":
                                DateTime val3;
                                try
                                {
                                    val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMTime(val3));
                                }
                                catch
                                {
                                    val3 = DateTime.Now;
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMTime(val3));
                                }
                                break;
                            case "Numeric":
                                if (val == null) val = "0";
                                if (a4.Length > 2)
                                    if (a4[2] == "1")
                                    {
                                        IsSum = true;
                                        if (PrevColName3 != "") SumCol3[j] = SumCol3[j] + double.Parse(val);
                                        if (PrevColName2 != "") SumCol2[j] = SumCol2[j] + double.Parse(val);
                                        SumAllCol[j] = SumAllCol[j] + double.Parse(val);
                                        if (PrevColName1 != "") SumCol1[j] = SumCol1[j] + double.Parse(val);
                                    }
                                int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                                r19.Append(Environment.NewLine + "<td class=\"text-right\">" + Tools.FormatNumber(val, iRound));
                                break;
                            case "Checkbox":
                                r19.Append(Environment.NewLine + "<td>" + UIDef.UICheckbox(a[j], "", val, "1", " onclick=\"if(this.checked==true)markline(" + i + ", true, false); else markline(" + i + ", false, false);\""));
                                break;
                            case "":
                                r19.Append(Environment.NewLine + "<td>" + val);
                                break;
                        }
                }
                //if (i % 2 == 1)
                //    r1.Append(Environment.NewLine + "<tr class=\"basetabol\" id=\"trrowid" + i + "\">");
                //else
                
            }
            // check group col
            Tools.SetGroupData(PrevColName9, PrevCol9, PrevColLable9, ref SumCol9, ref r19,
                context,
                ref r18, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName8, PrevCol8, PrevColLable8, ref SumCol8, ref r18,
                context,
                ref r17, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName7, PrevCol7, PrevColLable7, ref SumCol7, ref r17,
                context,
                ref r16, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName6, PrevCol6, PrevColLable6, ref SumCol6, ref r16,
                context,
                ref r15, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName5, PrevCol5, PrevColLable5, ref SumCol5, ref r15,
                context,
                ref r14, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName4, PrevCol4, PrevColLable4, ref SumCol4, ref r14,
                context,
                ref r13, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName3, PrevCol3, PrevColLable3, ref SumCol3, ref r13,
                context,
                ref r12, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName2, PrevCol2, PrevColLable2, ref SumCol2, ref r12,
                context,
                ref r11, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName1, PrevCol1, PrevColLable1, ref SumCol1, ref r11,
                context,
                ref r1, ColumnSpan, ColumnSumStart,
                b, c);
            
            if (IsSum)
            {
                r1.Append(Environment.NewLine + "<tr>");
                r1.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + "><b><u>" + context.GetLanguageLable("TOTAL") + "</u></b>");
                for (int j = ColumnSumStart; j < b.Length; j++)
                {
                    string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                    switch (a4[0])
                    {
                        case "Numeric":
                            int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                            if (a4.Length > 2) if (a4[2] == "1") r1.Append(Environment.NewLine + "<td class=\"text-right\"><b><u>" + Tools.FormatNumber(SumAllCol[j].ToString(), iRound) + "</u></b>");
                            break;
                        default:
                            r1.Append(Environment.NewLine + "<td>");
                            break;
                    }
                }
            }
            
            r1.Append(Environment.NewLine + "</tbody>" +
                Environment.NewLine + "</table>" +
                Environment.NewLine + "<!--/table-->" +
                Environment.NewLine + "</form>");            
            r = r1.ToString(); r1 = null;
            return r;
        }
        #endregion

        #region UI Pivot
        public string UIPivotForm(bool IsHTML = true)
        {
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType"); // 1||2||3||4;List||Pivot||Form||Other
            if (ReportType != "2") return ""; // Bao cao Pivot 
            StringBuilder r1 = new StringBuilder(); string r = "";
            StringBuilder r11 = new StringBuilder(); StringBuilder r12 = new StringBuilder(); StringBuilder r13 = new StringBuilder();
            StringBuilder r14 = new StringBuilder(); StringBuilder r15 = new StringBuilder(); StringBuilder r16 = new StringBuilder();
            StringBuilder r17 = new StringBuilder(); StringBuilder r18 = new StringBuilder(); StringBuilder r19 = new StringBuilder();
            // tham số ADODB
            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            // tham số lọc 
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            // Store Pivot
            dynamic dPivot = null; bool IsSum = false;
            string SPPivotName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPPivotName != "" && SPPivotName != null)
                SPPivotName = SPPivotName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPPivotName"); 
            else
                SPPivotName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPPivotName"); 
            string ParamPivotList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamPivotList"); 
            string PivotColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "PivotColumnName"); 
            string[] aPivotColName = PivotColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            Tools.ExecParam("PivotForm", QueryStringFilter, ParamPivotList, "", QueryStringType, context, SPPivotName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dPivot = JObject.Parse(json);
            /*
            PivotColumnName = dPivot.ListForm.Items[0][aPivotColName[1]].ToString();
            PivotColumnType = "Numeric;0;1";
            for (int i = 1; i < dPivot.PivotForm.Items.Count; i++)
            {
                PivotColumnName = PivotColumnName + "^" + dPivot.ListForm.Items[i ][aPivotColName[1]].ToString();
                PivotColumnType = "^" + "Numeric;0;1";
            }
            */
            // Store list
            dynamic dList = null;            
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList"); 
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName"); 
            string ColumnLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnLable"); 
            string ColumnType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnType"); 
            string ColumnGroup = ""; int ColumnSpan = 0; int ColumnSumStart = 0;
            string[] a = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnGroup").Split(new string[] { "||" }, StringSplitOptions.None);
            if (a.Length > 0) ColumnGroup = a[0];
            if (a.Length > 1) ColumnSpan = int.Parse(a[1]);
            if (a.Length > 2) ColumnSumStart = int.Parse(a[2]);
            a = ColumnGroup.Split(new string[] { "^" }, StringSplitOptions.None);

            string UrlLink1 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink1"); 
            string UrlLink2 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink2");
            string UrlLink3 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink3"); 
            string UrlLink4 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink4"); 
            string UrlLink5 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink5"); 

            parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
            Tools.ExecParam("ListForm", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dList = JObject.Parse(json);
            r1.Append(Environment.NewLine + "<form onsubmit=\"return false\" name=\"ListForm\" method=post>" +
                UIDef.UIHidden("MenuOn", MenuOn) +
                UIDef.UIHidden("MenuID", MenuID) +
                UIDef.UIHidden("RptID", RptID));
            //r1.Append(Environment.NewLine + "<table width=\"100%\" cellspacing=\"1\" cellpadding=\"1\"><tbody><tr><td nowrap>");

            // bang danh sach
            a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string PivotColumnGroup = a[a.Length - 3]; // Length-1: Sum/Count; Length-2: Pivot; Length-3 ColumnCompare
            string[] b; b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] c; c = ColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] d; d = ColumnGroup.Split(new string[] { "^" }, StringSplitOptions.None); ColumnGroup = "^" + ColumnGroup + "^";
            double[] SumCol1 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol1 = ""; string PrevColName1 = ""; string PrevColLable1 = ""; if (d.Length > 0) if (d[0] != "") { PrevColName1 = a[int.Parse(d[0])]; PrevColLable1 = b[int.Parse(d[0])]; }
            double[] SumCol2 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol2 = ""; string PrevColName2 = ""; string PrevColLable2 = ""; if (d.Length > 1) if (d[1] != "") { PrevColName2 = a[int.Parse(d[1])]; PrevColLable2 = b[int.Parse(d[1])]; }
            double[] SumCol3 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol3 = ""; string PrevColName3 = ""; string PrevColLable3 = ""; if (d.Length > 2) if (d[2] != "") { PrevColName3 = a[int.Parse(d[2])]; PrevColLable3 = a[int.Parse(d[2])]; }
            double[] SumCol4 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol4 = ""; string PrevColName4 = ""; string PrevColLable4 = ""; if (d.Length > 3) if (d[3] != "") { PrevColName4 = a[int.Parse(d[3])]; PrevColLable4 = a[int.Parse(d[3])]; }
            double[] SumCol5 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol5 = ""; string PrevColName5 = ""; string PrevColLable5 = ""; if (d.Length > 4) if (d[4] != "") { PrevColName5 = a[int.Parse(d[4])]; PrevColLable5 = a[int.Parse(d[4])]; }
            double[] SumCol6 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol6 = ""; string PrevColName6 = ""; string PrevColLable6 = ""; if (d.Length > 5) if (d[5] != "") { PrevColName6 = a[int.Parse(d[5])]; PrevColLable6 = a[int.Parse(d[5])]; }
            double[] SumCol7 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol7 = ""; string PrevColName7 = ""; string PrevColLable7 = ""; if (d.Length > 6) if (d[6] != "") { PrevColName7 = a[int.Parse(d[6])]; PrevColLable7 = a[int.Parse(d[6])]; }
            double[] SumCol8 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol8 = ""; string PrevColName8 = ""; string PrevColLable8 = ""; if (d.Length > 7) if (d[7] != "") { PrevColName8 = a[int.Parse(d[7])]; PrevColLable8 = a[int.Parse(d[7])]; }
            double[] SumCol9 = new double[b.Length - 1 + dPivot.PivotForm.Items.Count]; string PrevCol9 = ""; string PrevColName9 = ""; string PrevColLable9 = ""; if (d.Length > 8) if (d[8] != "") { PrevColName9 = a[int.Parse(d[8])]; PrevColLable9 = a[int.Parse(d[8])]; }
            double[] SumAllCol = new double[b.Length - 1 + dPivot.PivotForm.Items.Count];

            string[,] data = new string[dList.ListForm.Items.Count, (b.Length - 1 + dPivot.PivotForm.Items.Count)];
            // duyệt dữ liệu theo từng dòng
            int i = 0; int j = 0; int l = -1; int k = 0; 
            string Prev = ""; string Curr = "";
            json = "";
            while (i < dList.ListForm.Items.Count)
            {
                Curr = Tools.GetDataJson(dList.ListForm.Items[i], PivotColumnGroup);
                if (Prev != Curr)
                {
                    l++; //json = json + "},{";
                    //json = json + "\"" + a[0] + "\":\"" + Tools.GetDataJson(dList.ListForm.Items[i], a[0]) + "\"";
                    data[l, 0] = Tools.GetDataJson(dList.ListForm.Items[i], a[0]);
                    for (j = 1; j < b.Length - 2; j++)
                    {
                        //json = json + ",\"" + a[j] + "\":\"" + Tools.GetDataJson(dList.ListForm.Items[i], a[j]) + "\"";
                        data[l, j] = Tools.GetDataJson(dList.ListForm.Items[i], a[j]);
                    }
                }                
                Prev = Curr;                
                for (k = 0; k < dPivot.PivotForm.Items.Count; k++)
                {
                    if (Tools.GetDataJson(dPivot.PivotForm.Items[k], aPivotColName[0]) == Tools.GetDataJson(dList.ListForm.Items[i], aPivotColName[0]))
                    {
                        //json = json + ",\"" + Tools.GetDataJson(dPivot.PivotForm.Items[k], aPivotColName[1]) + "\":\"" + Tools.GetDataJson(dList.ListForm.Items[i], a[j]) + "\"";
                        data[l, j + k] = Tools.GetDataJson(dList.ListForm.Items[i], a[b.Length - 1]);
                    }
                    else
                    {
                        //json = json + ",\"" + Tools.GetDataJson(dPivot.PivotForm.Items[k], aPivotColName[1]) + "\":\"0\"";
                        if (data[l, j + k] == null) data[l, j + k] = "0";
                    }                        
                }
                i++;
            }
            json = ""; ColumnLable = ""; ColumnType = ""; i = 0;
            //json = json + "\"" + a[0] + "\":\"" + Tools.GetDataJson(dList.ListForm.Items[i], a[0]) + "\"";
            json = json + "},{\"" + a[0] + "\":\"" + data[i, 0] + "\"";
            ColumnLable = b[0];
            ColumnType = c[0];
            for (j = 1; j < b.Length - 2; j++)
            {
                json = json + ",\"" + a[j] + "\":\"" + data[i, j] + "\"";
                ColumnLable = ColumnLable + "^" + b[j];
                ColumnType = ColumnType + "^" + c[j];
            }
            double SumByCol = 0;
            for (k = 0; k < dPivot.PivotForm.Items.Count; k++)
            {
                json = json + ",\"" + Tools.GetDataJson(dPivot.PivotForm.Items[k], aPivotColName[1]) + "\":\"" + data[i, j + k] + "\"";
                ColumnLable = ColumnLable + "^" + Tools.GetDataJson(dPivot.PivotForm.Items[k], aPivotColName[1]);
                ColumnType = ColumnType + "^Numeric;0;1";
                SumByCol = SumByCol + Double.Parse(data[i, j + k]);
            }
            // Total
            json = json + ",\"Total\":\"" + SumByCol.ToString() + "\"";
            ColumnLable = ColumnLable + "^Total";
            ColumnType = ColumnType + "^Numeric;0;1";
            for (i = 1; i <= l; i++)
            {
                SumByCol = 0;
                json = json + "},{\"" + a[0] + "\":\"" + data[i, 0] + "\"";
                for (j = 1; j < b.Length - 2; j++)
                {
                    json = json + ",\"" + a[j] + "\":\"" + data[i, j] + "\"";
                }
                for (k=0; k < dPivot.PivotForm.Items.Count; k++)
                {
                    json = json + ",\"" + Tools.GetDataJson(dPivot.PivotForm.Items[k], aPivotColName[1]) + "\":\"" + data[i, j + k] + "\"";
                    SumByCol = SumByCol + Double.Parse(data[i, j + k]);
                }
                // Total
                json = json + ",\"Total\":\"" + SumByCol.ToString() + "\"";
            }
            // View data
            json = Tools.RemoveFisrtChar(json, 2);
            json = "{\"Data\":[" + json + "}]}";
            dList = JObject.Parse(json);
            b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            a = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            c = ColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
            r1.Append(Environment.NewLine + "</table>" +
                Environment.NewLine + "<table class=\"table table-hover table-border flex\" id=\"dbtablist\">");
            r1.Append(Environment.NewLine + "<thead class=\"thead-light\"><tr>");
            for (i = 0; i < b.Length; i++)
            {
                string[] a4 = c[i].Split(new string[] { ";" }, StringSplitOptions.None);
                if (!(a4[0] == "-" || ColumnGroup.IndexOf("^" + i + "^") > -1)) r1.Append(Environment.NewLine + "<th nowrap>" + context.GetLanguageLable(b[i]) + "</th>");
                // set default sum/prev
                SumAllCol[i] = 0; SumCol1[i] = 0; SumCol2[i] = 0; SumCol3[i] = 0; //SumCol4[i] = 0; SumCol5[i] = 0; SumCol6[i] = 0; SumCol7[i] = 0; SumCol8[i] = 0; SumCol9[i] = 0;
            }

            r1.Append(Environment.NewLine + "</tr></thead>");
            r1.Append(Environment.NewLine + "<tbody>");

            // duyệt dữ liệu theo từng dòng
            for (i = 0; i < dList.Data.Count; i++)
            {
                // check group col
                Tools.SetGroupData(PrevColName9, ref PrevCol9, PrevColLable9, ref SumCol9, ref r19,
                    ref dList, i, context,
                    ref r18, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName8, ref PrevCol8, PrevColLable8, ref SumCol8, ref r18,
                    ref dList, i, context,
                    ref r17, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName7, ref PrevCol7, PrevColLable7, ref SumCol7, ref r17,
                    ref dList, i, context,
                    ref r16, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName6, ref PrevCol6, PrevColLable6, ref SumCol6, ref r16,
                    ref dList, i, context,
                    ref r15, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName5, ref PrevCol5, PrevColLable5, ref SumCol5, ref r15,
                    ref dList, i, context,
                    ref r14, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName4, ref PrevCol4, PrevColLable4, ref SumCol4, ref r14,
                    ref dList, i, context,
                    ref r13, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName3, ref PrevCol3, PrevColLable3, ref SumCol3, ref r13,
                    ref dList, i, context,
                    ref r12, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName2, ref PrevCol2, PrevColLable2, ref SumCol2, ref r12,
                    ref dList, i, context,
                    ref r11, ColumnSpan, ColumnSumStart,
                    b, c);
                Tools.SetGroupData(PrevColName1, ref PrevCol1, PrevColLable1, ref SumCol1, ref r11,
                    ref dList, i, context,
                    ref r1, ColumnSpan, ColumnSumStart,
                    b, c);

                r19.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "\">");
                // Duyệt dữ liệu theo từng cột
                int ILink = 0;
                for (j = 0; j < b.Length; j++)
                {
                    string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                    string val = "";
                    try { val = dList.Data[i][a[j]].ToString(); } catch { val = ""; }
                    if (val == null) val = "";
                    if (ColumnGroup.IndexOf("^" + j + "^") < 0)
                        switch (a4[0])
                        {
                            case "HREF":
                                if (!IsHTML)
                                    r19.Append(Environment.NewLine + "<td>" + val);
                                else
                                {
                                    ILink = ILink + 1;
                                    switch (ILink)
                                    {
                                        case 1:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink1 + dList.Data[i][a[j - int.Parse(a4[2])]] + "\">" + val + "</a>");
                                            break;
                                        case 2:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink2 + dList.Data[i][a[j - int.Parse(a4[2])]] + "\">" + val + "</a>");
                                            break;
                                        case 3:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink3 + dList.Data[i][a[j - int.Parse(a4[2])]] + "\">" + val + "</a>");
                                            break;
                                        case 4:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink4 + dList.Data[i][a[j - int.Parse(a4[2])]] + "\">" + val + "</a>");
                                            break;
                                        case 5:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + UrlLink5 + dList.Data[i][a[j - int.Parse(a4[2])]] + "\">" + val + "</a>");
                                            break;
                                        default:
                                            r19.Append(Environment.NewLine + "<td><a href=\"" + a4[1] + "(" + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\">" + val + "</a>");
                                            break;
                                    }
                                }                                
                                break;
                            case "Date":
                                DateTime val1;
                                try
                                {
                                    val1 = DateTime.Parse(val);
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMDate(val1));
                                }
                                catch
                                {
                                    //val1 = DateTime.Now;
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">");// + Tools.HRMDate(val1));
                                }
                                break;
                            case "Datetime":
                                DateTime val2;
                                try
                                {
                                    val2 = DateTime.Parse(val);
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMDateTime(val2));
                                }
                                catch
                                {
                                    //val2 = DateTime.Now;
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">");// + Tools.HRMDateTime(val2));
                                }
                                break;
                            case "Time":
                                DateTime val3;
                                try
                                {
                                    val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMTime(val3));
                                }
                                catch
                                {
                                    val3 = DateTime.Now;
                                    r19.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMTime(val3));
                                }
                                break;
                            case "Numeric":
                                if (val == null) val = "0";
                                if (a4.Length > 2)
                                    if (a4[2] == "1")
                                    {
                                        IsSum = true;
                                        if (PrevColName3 != "") SumCol3[j] = SumCol3[j] + double.Parse(val);
                                        if (PrevColName2 != "") SumCol2[j] = SumCol2[j] + double.Parse(val);
                                        SumAllCol[j] = SumAllCol[j] + double.Parse(val);
                                        if (PrevColName1 != "")SumCol1[j] = SumCol1[j] + double.Parse(val);
                                    }
                                int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                                r19.Append(Environment.NewLine + "<td class=\"text-right\">" + Tools.FormatNumber(val, iRound));
                                break;
                            case "Checkbox":
                                r19.Append(Environment.NewLine + "<td>" + UIDef.UICheckbox(a[j], "", val, "1", " onclick=\"if(this.checked==true)markline(" + i + ", true, false); else markline(" + i + ", false, false);\""));
                                break;
                            case "":
                                r19.Append(Environment.NewLine + "<td>" + val);
                                break;
                        }
                }
            }

            // check group col
            Tools.SetGroupData(PrevColName9, PrevCol9, PrevColLable9, ref SumCol9, ref r19,
                context,
                ref r18, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName8, PrevCol8, PrevColLable8, ref SumCol8, ref r18,
                context,
                ref r17, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName7, PrevCol7, PrevColLable7, ref SumCol7, ref r17,
                context,
                ref r16, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName6, PrevCol6, PrevColLable6, ref SumCol6, ref r16,
                context,
                ref r15, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName5, PrevCol5, PrevColLable5, ref SumCol5, ref r15,
                context,
                ref r14, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName4, PrevCol4, PrevColLable4, ref SumCol4, ref r14,
                context,
                ref r13, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName3, PrevCol3, PrevColLable3, ref SumCol3, ref r13,
                context,
                ref r12, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName2, PrevCol2, PrevColLable2, ref SumCol2, ref r12,
                context,
                ref r11, ColumnSpan, ColumnSumStart,
                b, c);
            Tools.SetGroupData(PrevColName1, PrevCol1, PrevColLable1, ref SumCol1, ref r11,
                context,
                ref r1, ColumnSpan, ColumnSumStart,
                b, c);

            if (IsSum)
            {
                r1.Append(Environment.NewLine + "<tr>");
                r1.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + "><b><u>" + context.GetLanguageLable("TOTAL") + "</u></b>");
                for (j = ColumnSumStart; j < b.Length; j++)
                {
                    string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                    switch (a4[0])
                    {
                        case "Numeric":
                            int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                            if (a4.Length > 2) if (a4[2] == "1") if (a4[2] == "1") r1.Append(Environment.NewLine + "<td align=right><b><u>" + Tools.FormatNumber(SumAllCol[j].ToString(), iRound) + "</u></b>");
                            break;
                        default:
                            r1.Append(Environment.NewLine + "<td>");
                            break;
                    }
                }
            }
            
            r1.Append(Environment.NewLine + "</tbody>");
            r1.Append(Environment.NewLine + "</table></div><!--/td></tr-->");

            r1.Append(Environment.NewLine + "<!--/table-->");
            r1.Append(Environment.NewLine + "</form>");
            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIPivotForm1()
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType");// List; Tree; Edit
            string Xlsx = Tools.GetDataJson(Rpt.RptConfig.Items[0], "Xlsx"); 
            // Store list
            dynamic dList = null;
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList"); 
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName"); 
            string ColumnLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnLable");
            string ColumnType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnType");
            // Store Pivot
            dynamic dPivot = null;
            string SPPivotName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPPivotName != "" && SPPivotName != null)
                SPPivotName = SPPivotName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPPivotName"); 
            else
                SPPivotName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPPivotName"); 
            string ParamPivotList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamPivotList"); 
            string PivotColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "PivotColumnName"); 

            int ILink = 0;
            string UrlLink1 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink1"); 
            string UrlLink2 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink2"); 
            string UrlLink3 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink3"); 
            string UrlLink4 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink4"); 
            string UrlLink5 = Tools.GetDataJson(Rpt.RptConfig.Items[0], "UrlLink5"); 

            if (ReportType != "2") return ""; // Bao cao pivot 

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            // store list
            Tools.ExecParam("ListForm", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dList = JObject.Parse(json);

            // store Pivot
            Tools.ExecParam("PivotForm", QueryStringFilter, ParamPivotList, "", QueryStringType, context, SPPivotName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dPivot = JObject.Parse(json);

            r1.Append(Environment.NewLine + "<form onsubmit=\"return false\" name=\"ListForm\" method=post>" +
                UIDef.UIHidden("MenuOn", MenuOn) +
                UIDef.UIHidden("MenuID", MenuID) +
                UIDef.UIHidden("RptID", RptID));
            //r1.Append(Environment.NewLine + "<table width=\"100%\" cellspacing=\"1\" cellpadding=\"1\"><tbody><tr><td nowrap>");

            // bang danh sach
            string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] b; b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] c; c = ColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
            r1.Append(Environment.NewLine + "<tr><td>" +
                Environment.NewLine + "<table class=\"table table-hover table-border flex\" id=\"dbtablist\">");
            r1.Append(Environment.NewLine + "<thead class=\"thead-light\"><tr>");
            for (int i = 0; i < b.Length; i++)
            {
                string[] a4 = c[i].Split(new string[] { ";" }, StringSplitOptions.None);
                if (a4[0] != "-") r1.Append(Environment.NewLine + "<th nowrap>" + context.GetLanguageLable(b[i]) + "</th>");
            }
            r1.Append(Environment.NewLine + "</tr></thead>");
            r1.Append(Environment.NewLine + "<tbody>");
            for (int i = 0; i < dList.ListForm.Items.Count; i++)
            {
                if (i % 2 == 1)
                    r1.Append(Environment.NewLine + "<tr class=\"basetabol\" id=\"trrowid" + i + "\">");
                else
                    r1.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "\">");
                for (int j = 0; j < b.Length; j++)
                {
                    string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                    string val = "";
                    try { val = Tools.GetDataJson(dList.ListForm.Items[i], a[j]); } catch { val = ""; }
                    if (val == null) val = "";
                    switch (a4[0])
                    {
                        case "HREF":
                            ILink = ILink + 1;
                            switch (ILink)
                            {
                                case 1:
                                    r1.Append(Environment.NewLine + "<td><a href=\"" + UrlLink1 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                    break;
                                case 2:
                                    r1.Append(Environment.NewLine + "<td><a href=\"" + UrlLink2 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                    break;
                                case 3:
                                    r1.Append(Environment.NewLine + "<td><a href=\"" + UrlLink3 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                    break;
                                case 4:
                                    r1.Append(Environment.NewLine + "<td><a href=\"" + UrlLink4 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                    break;
                                case 5:
                                    r1.Append(Environment.NewLine + "<td><a href=\"" + UrlLink5 + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + "\">" + val + "</a>");
                                    break;
                                default:
                                    r1.Append(Environment.NewLine + "<td><a href=\"" + a4[1] + "(" + Tools.GetDataJson(dList.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\">" + val + "</a>");
                                    break;
                            }
                            break;
                        case "Date":
                            DateTime val1;
                            try
                            {
                                val1 = DateTime.Parse(val);
                                r1.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMDate(val1));
                            }
                            catch
                            {
                                //val1 = DateTime.Now;
                                r1.Append(Environment.NewLine + "<td class=\"text-center\">");// + Tools.HRMDate(val1));
                            }
                            break;
                        case "Datetime":
                            DateTime val2;
                            try
                            {
                                val2 = DateTime.Parse(val);
                                r1.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMDateTime(val2));
                            }
                            catch
                            {
                                //val2 = DateTime.Now;
                                r1.Append(Environment.NewLine + "<td class=\"text-center\">");// + Tools.HRMDateTime(val2));
                            }
                            break;
                        case "Time":
                            DateTime val3;
                            try
                            {
                                val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                r1.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMTime(val3));
                            }
                            catch
                            {
                                val3 = DateTime.Now;
                                r1.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMTime(val3));
                            }
                            break;
                        case "Numeric":
                            if (val == null) val = "0";
                            int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                            r1.Append(Environment.NewLine + "<td class=\"text-right\">" + Tools.FormatNumber(val, iRound));
                            break;
                        case "Checkbox":
                            r1.Append(Environment.NewLine + "<td class=\"text-right\">" + UIDef.UICheckbox(a[j], "", val, "1", " onclick=\"if(this.checked==true)markline(" + i + ", true, false); else markline(" + i + ", false, false);\""));
                            break;
                        case "":
                            r1.Append(Environment.NewLine + "<td>" + val);
                            break;
                    }
                }
            }
            r1.Append(Environment.NewLine + "</tbody>");
            r1.Append(Environment.NewLine + "</table></td></tr>");
            r1.Append(Environment.NewLine + "</table>");
            r1.Append(Environment.NewLine + "</form>");            
            r = r1.ToString(); r1 = null;
            return r;
        }
        #endregion

        #region UI Chart
        public string UIChartPie (int width = 900, int height = 500)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            string ReportCode = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportConfigCode"); 
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType"); // 1||2||3||4;List||Pivot||Form||Other
            if (ReportType != "41" && ReportType != "42" && ReportType != "43") return ""; // Bao cao list doughnut
            string bar = ""; if (ReportType == "42") bar = ",is3D: true"; else if (ReportType == "43") bar = ",pieHole: 0.4";
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList"); 
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName"); 
            string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            Tools.ExecParam("ChartPie", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dynamic d = JObject.Parse(json);

            //r1.Append(Environment.NewLine + "<canvas style=\"width:" + width + ";height:" + height + "\" id=\"ReportCodeChartPie\" width=\"" + width + "\" height=\"" + height + "\"></canvas>");
            //r1.Append(Environment.NewLine + "<div id=\"piechart\" ><!--style=\"width: " + width + "px; height: " + height + "px;\"--></div>");
            r1.Append(Environment.NewLine + "<div id=\"piechart\" ><!--style=\"width: " + width + "px; height: " + height + "px;\"--></div>");
            r1.Append(Environment.NewLine + "<!--img id=\"save-pdf\" src=\"/images/Print2.gif\"/-->");
            r1.Append(Environment.NewLine + "<scr" + "ipt type=\"text/java" + "script\">");
            r1.Append(Environment.NewLine + "google.charts.load('current', {'packages':['corechart']});");
            r1.Append(Environment.NewLine + "google.charts.setOnLoadCallback(drawBasic);");
            r1.Append(Environment.NewLine + "function drawBasic() {");
            
            r1.Append(Environment.NewLine + "var data = google.visualization.arrayToDataTable([");
            r1.Append("[");
            r1.Append("'" + context.GetLanguageLable(a[0]) + "'");
            r1.Append(",'" + context.GetLanguageLable(a[1]) + "'");
            //for (var j = 1; j < a.Length; j++)
            //{
            //    r1.Append(",'" + a[j] + "'");
            //}
            r1.Append("]");            
            for (int i = 0; i < d.ChartPie.Items.Count; i++)
            {
                r1.Append(",[");
                r1.Append("'" + Tools.GetDataJson(d.ChartPie.Items[i], a[0]) + "',");
                r1.Append(Tools.GetDataJson(d.ChartPie.Items[i], a[1]));
                //for (var j = 1; j < a.Length; j++)
                //{
                //    r1.Append(",'" + Tools.GetDataJson(d.ChartPie.Items[i], a[j]) + "'");
                //}
                r1.Append("]");
            }
           
            r1.Append(Environment.NewLine + "]);");
            //r1.Append(Environment.NewLine + "var data = google.visualization.arrayToDataTable([['Tên', 'Số lượng'],['Hợp đồng không xác định thời hạn (30)', 30],['Hợp đồng thử việc (15)', 15]]);");
            r1.Append(Environment.NewLine + "var options = {title: ''" + bar + "," +//" + context.GetLanguageLable(ReportCode) + "
            //Environment.NewLine + "    legend:'none'," +
            Environment.NewLine + "    width: '100%'," +
            Environment.NewLine + "    height: '100%'," +
            Environment.NewLine + "    pieSliceText: 'percentage'," +
            //Environment.NewLine + "    colors: ['#0598d8', '#f97263']," +
            //Environment.NewLine + "    chartArea: {" +
            //Environment.NewLine + "        left: \"3%\"," +
            //Environment.NewLine + "        top: \"20px\"," +
            ////Environment.NewLine + "        height: \"100%\"," +
            //Environment.NewLine + "        width: \"97%\"" +
            //Environment.NewLine + "    }" +
            Environment.NewLine + "};");//,pieStartAngle: 100
            r1.Append(Environment.NewLine + "var chart = new google.visualization.PieChart(document.getElementById('piechart'));");
            r1.Append(Environment.NewLine + "chart.draw(data, options);");
            r1.Append(Environment.NewLine + "/*var btnPdf = document.getElementById('save-pdf');" +
                "btnPdf.addEventListener('click', function () {");
            r1.Append(Environment.NewLine + "var doc = new jsPDF();");
            r1.Append(Environment.NewLine + "doc.addImage(chart.getImageURI(), 0, 0);");
            r1.Append(Environment.NewLine + "doc.save('chart.pdf');");
            r1.Append(Environment.NewLine + "}, false);*/");            
            r1.Append(Environment.NewLine + "}window.onresize = function(event) { google.charts.setOnLoadCallback(drawBasic); }");
            /*r1.Append(Environment.NewLine + "var ChartColors = ['#8B0016', '#8E1E20', '#945305', '#976D00', '#9C9900', '#367517', '#006241', '#00676B', '#103667', '#211551', '#38044B', '#64004B', '#ECECEC', '#363636', '#B2001F', '#B6292B', '#BD6B09', '#C18C00', '#C7C300', '#489620', '#007F54', '#008489', '#184785', '#2D1E69', '#490761', '#780062', '#D7D7D7', '#C50023', '#C82E31', '#D0770B', '#D59B00', '#DCD800', '#50A625', '#008C5E', '#009298', '#1B4F93', '#322275', '#52096C', '#8F006D', '#C2C2C2', '#DF0029', '#E33539', '#EC870E', '#F1AF00', '#F9F400', '#5BBD2B', '#00A06B', '#00A6AD', '#205AA7', '#3A2885', '#5D0C7B', '#A2007C', '#B7B7B7', '#E54646', '#EB7153', '#F09C42', '#F3C246', '#FCF54C', '#83C75D', '#00AE72', '#00B2BF', '#426EB4', '#511F90', '#79378B', '#AF4A92', '#A0A0A0', '#EE7C6B', '#F19373', '#F5B16D', '#F9CC76', '#FEF889', '#AFD788', '#67BF7F', '#6EC3C9', '#7388C1', '#635BA2', '#8C63A4', '#C57CAC', '#898989', '#F5A89A', '#F6B297', '#FACE9C', '#FCE0A6', '#FFFAB3', '#C8E2B1', '#98D0B9', '#99D1D3', '#94AAD6', '#8273B0', '#AA87B8', '#D2A6C7', '#707070', '#FCDAD5', '#FCD9C4', '#FDE2CA', '#FEEBD0', '#FFFBD1', '#E6F1D8', '#C9E4D6', '#CAE5E8', '#BFCAE6', '#A095C4', '#C9B5D4', '#E8D3E3','#555555'];");
            r1.Append(Environment.NewLine + "var ChartValues = new Array();");
            r1.Append(Environment.NewLine + "var ChartLables = new Array();");
            for (var j = 1; j < a.Length; j++)
            {
                r1.Append(Environment.NewLine + "ChartValues[" + (j - 1) + "] = new Array();");
                for (int i = 0; i < d.ChartPie.Items.Count; i++)
                {
                    r1.Append(Environment.NewLine + "ChartLables[" + i + "] = \"" + d.ChartPie.Items[i ][a[0]].ToString() + "\";");
                    r1.Append(Environment.NewLine + "ChartValues[" + (j - 1) + "][" + i + "] = " + d.ChartPie.Items[i ][a[j]].ToString() + ";");
                }                
            }
            
            r1.Append(Environment.NewLine + "$(document).ready(function(){" +
            Environment.NewLine + "	var chartDiv = $(\"#ReportCodeChartPie\");" +
            Environment.NewLine + "	var myChart = new Chart(chartDiv, {" +
            Environment.NewLine + "		type: '" + bar + "'," +
            Environment.NewLine + "		data: {" +
            Environment.NewLine + "			labels: ChartLables," +
            Environment.NewLine + "			datasets: [");
            r1.Append(Environment.NewLine + "			{" +
                Environment.NewLine + "			data: ChartValues[0]," +
                Environment.NewLine + "			backgroundColor: ChartColors" +
                Environment.NewLine + "			}");
            for (var j = 1; j < a.Length - 1; j++)
            {
                r1.Append(Environment.NewLine + "			,{" +
                Environment.NewLine + "			data: ChartValues[" + j + "]," +
                Environment.NewLine + "			backgroundColor: ChartColors" +
                Environment.NewLine + "			}");
            }                
            r1.Append(Environment.NewLine + "]" +
            Environment.NewLine + "		}," +
            Environment.NewLine + "		options: {" +
            Environment.NewLine + "			title: {" +
            Environment.NewLine + "				display: true," +
            Environment.NewLine + "				text: 'Pie Chart'" +
            Environment.NewLine + "			}," +
            Environment.NewLine + "			responsive: true," +
            Environment.NewLine + "			maintainAspectRatio: false," +
            Environment.NewLine + "		}" +
            Environment.NewLine + "	});" +
            Environment.NewLine + "});");*/
            r1.Append(Environment.NewLine + "</scr" + "ipt>");

            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIChartBar(int width = 900, int height = 300)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            string ReportCode = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportConfigCode"); 
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType");  // 1||2||3||4;List||Pivot||Form||Other
            if (ReportType != "51" && ReportType != "52") return ""; // Bao cao list 
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList"); 
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName"); 
            string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string ColumnLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnLable"); 
            string[] b; b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            string bar = "bar"; if (ReportType == "52") bar = "horizontal";// horizontalBar";

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            Tools.ExecParam("ChartBar", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dynamic d = JObject.Parse(json);

            r1.Append(Environment.NewLine + "<div id=\"chart_div\"><!--style=\"width: " + width + "px; height: " + height + "px;\"--></div>");
            r1.Append(Environment.NewLine + "<!--img id=\"save-pdf\" src=\"/images/Print2.gif\"/-->");
            r1.Append(Environment.NewLine + "<scr" + "ipt type=\"text/java" + "script\">");
            r1.Append(Environment.NewLine + "google.charts.load('current', {'packages':['bar']});");
            r1.Append(Environment.NewLine + "google.charts.setOnLoadCallback(drawBasic);");
            r1.Append(Environment.NewLine + "function drawBasic() {");
            r1.Append(Environment.NewLine + "var data = google.visualization.arrayToDataTable([");
            r1.Append("[");
            r1.Append("'" + context.GetLanguageLable(a[0]) + "'");
            //r1.Append(",'" + a[1] + "'");
            for (int j = 1; j < a.Length; j++)
            {
                r1.Append(",'" + context.GetLanguageLable(a[j]) + "'");
            }
            r1.Append("]");
            for (int i = 0; i < d.ChartBar.Items.Count; i++)
            {
                r1.Append(",[");
                r1.Append("'" + Tools.GetDataJson(d.ChartBar.Items[i], a[0]) + "'");
                //r1.Append(Tools.GetDataJson(d.ChartBar.Items[i], a[1]));
                for (int j = 1; j < a.Length; j++)
                {
                    r1.Append(",'" + Tools.GetDataJson(d.ChartBar.Items[i], a[j]) + "'");
                }
                r1.Append("]");
            }

            r1.Append(Environment.NewLine + "]);");
            r1.Append(Environment.NewLine + "var options = {" +
            Environment.NewLine + "    chart:{" +
            Environment.NewLine + "        title: ''" +//" + context.GetLanguageLable(ReportCode) + "
            Environment.NewLine + "    }," +
            Environment.NewLine + "    bars: '" + bar + "'," +
            //Environment.NewLine + "    series: {" +
            //Environment.NewLine + "        0: { axis: 'distance' }," +
            //Environment.NewLine + "        1: { axis: 'brightness' }" +
            //Environment.NewLine + "    }," +
            //Environment.NewLine + "    axes: {" +
            //Environment.NewLine + "        x: {" +
            //Environment.NewLine + "            distance: {label: 'parsecs'}," +
            //Environment.NewLine + "            brightness: {side: 'top', label: 'apparent magnitude'}" +
            //Environment.NewLine + "        }" +
            //Environment.NewLine + "    }" +
            Environment.NewLine + "};");//,pieStartAngle: 100
            r1.Append(Environment.NewLine + "var chart = new google.charts.Bar(document.getElementById('chart_div'));");
            r1.Append(Environment.NewLine + "chart.draw(data, google.charts.Bar.convertOptions(options));");
            r1.Append(Environment.NewLine + "/*var btnPdf = document.getElementById('save-pdf');" +
                "btnPdf.addEventListener('click', function () {");
            r1.Append(Environment.NewLine + "var doc = new jsPDF();");
            r1.Append(Environment.NewLine + "doc.addImage(chart.getImageURI(), 0, 0);");
            r1.Append(Environment.NewLine + "doc.save('chart.pdf');");
            r1.Append(Environment.NewLine + "}, false);*/");
            r1.Append(Environment.NewLine + "}window.onresize = function(event) { google.charts.setOnLoadCallback(drawBasic); }");
            r1.Append(Environment.NewLine + "</scr" + "ipt>");
            
            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIChartLine(int width = 900, int height = 700)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            string ReportCode = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportConfigCode"); 
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType");  // 1||2||3||4;List||Pivot||Form||Other
            if (ReportType != "53") return ""; // Bao cao list 
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList"); 
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName"); 
            string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string ColumnLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnLable"); 
            string[] b; b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            string ColumnType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnType");
            string[] c; c = ColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
            //string bar = "line";

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            Tools.ExecParam("ChartBar", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dynamic d = JObject.Parse(json);

            r1.Append(Environment.NewLine + "<div id=\"chart_div\" ><!--style=\"width: " + width + "px; height: " + height + "px;\"--></div>");
            r1.Append(Environment.NewLine + "<scr" + "ipt type=\"text/java" + "script\">");
            r1.Append(Environment.NewLine + "google.charts.load('current', {packages: ['corechart', 'line']});" +
                Environment.NewLine + "google.charts.setOnLoadCallback(drawBasic);" +
                Environment.NewLine + "function drawBasic() {" +
                Environment.NewLine + " var data = new google.visualization.DataTable();");
            // Add column
            for (int j = 0; j < a.Length; j++)
            {
                string t = "number";
                if (c.Length > j) if (c[j] == "") t = "string";
                r1.Append(Environment.NewLine + " data.addColumn('" + t + "', '" + b[j] + "');");
            }
            // Add Rows
            r1.Append(Environment.NewLine + " data.addRows(" +
                Environment.NewLine + "     [");
            for (int i = 0; i < d.ChartBar.Items.Count; i++)
            {
                r1.Append(Environment.NewLine + "         [");
                for (int j = 0; j < a.Length; j++)
                {
                    bool IsString = false;
                    if (c.Length > j) if (c[j] == "") IsString = true;
                    if (j == 0)
                        r1.Append(Environment.NewLine + (IsString? "'" + Tools.GetDataJson(d.ChartBar.Items[i], a[j]) + "'": Tools.GetDataJson(d.ChartBar.Items[i], a[j])));
                    else
                        r1.Append(Environment.NewLine + ", " + (IsString ? "'" + Tools.GetDataJson(d.ChartBar.Items[i], a[j]) + "'" : Tools.GetDataJson(d.ChartBar.Items[i], a[j])));
                }
                if (i == (d.ChartBar.Items.Count -1))
                    r1.Append(Environment.NewLine + "         ]");
                else
                    r1.Append(Environment.NewLine + "         ],");
            }

            // title hAxis; vAxis
            string hAxis = context.GetRequestVal("hAxis"); if (hAxis == "") hAxis = "y";
            string vAxis = context.GetRequestVal("vAxis"); if (vAxis == "") hAxis = "x";

            r1.Append(Environment.NewLine + "     ]);" +
                Environment.NewLine + " var options = {" +
                Environment.NewLine + "     hAxis: {title: '" + hAxis + "'}," +
                Environment.NewLine + "     vAxis: {title: '" + vAxis + "'}," +
                Environment.NewLine + "     backgroundColor: '#f1f8e9'" +
                Environment.NewLine + " };" +
                Environment.NewLine + " var chart = new google.visualization.LineChart(document.getElementById('chart_div'));" +
                Environment.NewLine + " chart.draw(data, options);" +
                Environment.NewLine + "}window.onresize = function(event) { google.charts.setOnLoadCallback(drawBasic); }");
            r1.Append(Environment.NewLine + "</scr" + "ipt>");

            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIChartOrganization(int width = 900, int height = 600)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            string ReportCode = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportConfigCode"); 
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType"); // 1||2||3||4;List||Pivot||Form||Other
            if (ReportType != "6") return ""; // Bao cao list 
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList"); 
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName"); 
            string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string ColumnLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnLable"); 
            string[] b; b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            Tools.ExecParam("ChartOrg", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dynamic d = JObject.Parse(json);

            r1.Append(Environment.NewLine + "<div id=\"chart_div\" ><!--style=\"width: " + width + "px; height: " + height + "px;\"--></div>");
            r1.Append(Environment.NewLine + "<!--img id=\"save-pdf\" src=\"/images/Print2.gif\"/-->");
            r1.Append(Environment.NewLine + "<scr" + "ipt type=\"text/java" + "script\">");
            r1.Append(Environment.NewLine + "google.charts.load('current', {'packages':['orgchart']});");
            r1.Append(Environment.NewLine + "google.charts.setOnLoadCallback(drawBasic);");
            r1.Append(Environment.NewLine + "function drawBasic() {");
            r1.Append(Environment.NewLine + "var data = new google.visualization.DataTable();");
            r1.Append(Environment.NewLine + "data.addColumn('string', 'Name');");
            r1.Append(Environment.NewLine + "data.addColumn('string', 'Manager');");
            r1.Append(Environment.NewLine + "data.addColumn('string', 'ToolTip');");
            if (d.ChartOrg.Items.Count > 0)
            {
                int i = 0;
                r1.Append(Environment.NewLine + "data.addRows([");

                r1.Append(Environment.NewLine + "[{v:'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[0]) + "', f:'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[1]) + "'},'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[2]) + "', '" + Tools.GetDataJson(d.ChartOrg.Items[i], a[3]) + "']");
                for (i = 1; i < d.ChartOrg.Items.Count; i++)
                {
                    r1.Append(Environment.NewLine + ",[{v:'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[0]) + "', f:'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[1]) + "'},'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[2]) + "', '" + Tools.GetDataJson(d.ChartOrg.Items[i], a[3]) + "']");
                }
                r1.Append("]);");
            }
            r1.Append(Environment.NewLine + "var container = document.getElementById('chart_div');");
            r1.Append(Environment.NewLine + "var chart = new google.visualization.OrgChart(container);");
            r1.Append(Environment.NewLine + "container.addEventListener('click', function (e) {");
            r1.Append(Environment.NewLine + "e.preventDefault();");
            r1.Append(Environment.NewLine + "if (e.target.tagName.toUpperCase() === 'A') {");
            r1.Append(Environment.NewLine + "console.log(e.target.href);// window.open(e.target.href, '_blank');// location.href = e.target.href;");
            r1.Append(Environment.NewLine + "} else {");
            r1.Append(Environment.NewLine + "var selection = chart.getSelection();");
            r1.Append(Environment.NewLine + "if (selection.length > 0) {");
            r1.Append(Environment.NewLine + "var row = selection[0].row;");
            r1.Append(Environment.NewLine + "var collapse = (chart.getCollapsedNodes().indexOf(row) == -1);");
            r1.Append(Environment.NewLine + "chart.collapse(row, collapse);");
            r1.Append(Environment.NewLine + "}");
            r1.Append(Environment.NewLine + "}");
            r1.Append(Environment.NewLine + "chart.setSelection([]);");
            r1.Append(Environment.NewLine + "return false;");
            r1.Append(Environment.NewLine + "}, false);");
            r1.Append(Environment.NewLine + "chart.draw(data, {allowHtml:true, allowCollapse:true});}window.onresize = function(event) { google.charts.setOnLoadCallback(drawBasic); }");
            r1.Append(Environment.NewLine + "</scr" + "ipt>");

            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIChartOrg(int width = 900, int height = 600)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            string ReportCode = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportConfigCode");
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType"); // 1||2||3||4;List||Pivot||Form||Other
            if (ReportType != "6") return ""; // Bao cao list 
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter"); 
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType"); 
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName"); 
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName"); 
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList"); 
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName"); 
            string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string ColumnLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnLable"); 
            string[] b; b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            Tools.ExecParam("ChartOrg", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dynamic d = JObject.Parse(json);

            r1.Append(Environment.NewLine + "<div id=\"chart_div\" ><!--style=\"width: " + width + "px; height: " + height + "px;\"--></div>");
            r1.Append(Environment.NewLine + "<!--img id=\"save-pdf\" src=\"/images/Print2.gif\"/-->");
            r1.Append(Environment.NewLine + "<scr" + "ipt type=\"text/java" + "script\">");
            r1.Append(Environment.NewLine + "google.charts.load('visualization', '1', {'packages':['orgchart']});");
            r1.Append(Environment.NewLine + "google.charts.setOnLoadCallback(drawBasic);");
            r1.Append(Environment.NewLine + "function drawBasic() {");
            r1.Append(Environment.NewLine + "var data = new google.visualization.DataTable();");
            r1.Append(Environment.NewLine + "data.addColumn('string', 'Name');");
            r1.Append(Environment.NewLine + "data.addColumn('string', 'Manager');");
            r1.Append(Environment.NewLine + "data.addColumn('string', 'ToolTip');");
            if (d.ChartOrg.Items.Count > 0)
            {
                int i = 0;
                r1.Append(Environment.NewLine + "data.addRows([");

                r1.Append(Environment.NewLine + "[{v:'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[0]) + "', f:'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[1]) + "'},'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[2]) + "', '" + Tools.GetDataJson(d.ChartOrg.Items[i], a[3]) + "']");
                for (i = 1; i < d.ChartOrg.Items.Count; i++)
                {
                    r1.Append(Environment.NewLine + ",[{v:'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[0]) + "', f:'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[1]) + "'},'" + Tools.GetDataJson(d.ChartOrg.Items[i], a[2]) + "', '" + Tools.GetDataJson(d.ChartOrg.Items[i], a[3]) + "']");
                }
                r1.Append("]);");
            }

            r1.Append(Environment.NewLine + "var chart = new google.visualization.OrgChart(document.getElementById('chart_div'));");
            r1.Append(Environment.NewLine + "var options = {allowHtml: true};");
            r1.Append(Environment.NewLine + "var runOnce = google.visualization.events.addListener(chart, 'ready', function() {");
            r1.Append(Environment.NewLine + "// set up + sign event handlers");
            r1.Append(Environment.NewLine + "var previous;");
            r1.Append(Environment.NewLine + "$('#chart_div').on('click', 'div.plus', function () {");
            r1.Append(Environment.NewLine + "var selection = chart.getSelection();");
            r1.Append(Environment.NewLine + "var row;");
            r1.Append(Environment.NewLine + "if (selection.length == 0) {");
            r1.Append(Environment.NewLine + "row = previous;");
            r1.Append(Environment.NewLine + "} else {");
            r1.Append(Environment.NewLine + "row = selection[0].row;");
            r1.Append(Environment.NewLine + "previous = row;");
            r1.Append(Environment.NewLine + "}"); // End if (selection.length == 0)
            r1.Append(Environment.NewLine + "var collapsed = chart.getCollapsedNodes();");
            r1.Append(Environment.NewLine + "var collapse = (collapsed.indexOf(row) == -1);");
            r1.Append(Environment.NewLine + "chart.collapse(row, collapse);");
            r1.Append(Environment.NewLine + "chart.setSelection();");
            r1.Append(Environment.NewLine + "// get a new list of collapsed nodes");
            r1.Append(Environment.NewLine + "collapsed = chart.getCollapsedNodes();");
            r1.Append(Environment.NewLine + "// change the expand/collapse sign");
            r1.Append(Environment.NewLine + "var plusSrc = '/images/down.png';");// 23x23 || 22x22
            r1.Append(Environment.NewLine + "var minusSrc = '/images/up.png';");
            r1.Append(Environment.NewLine + "var src = (collapse) ? plusSrc : minusSrc;");
            r1.Append(Environment.NewLine + "data.setFormattedValue(row, 0, data.getFormattedValue(row, 0).replace(/src=\".*\"/i, 'src=\"' + src + '\"'));");
            r1.Append(Environment.NewLine + "// set up event listener to recollapse nodes after redraw");
            r1.Append(Environment.NewLine + "var runOnce2 = google.visualization.events.addListener(chart, 'ready', function() {");
            r1.Append(Environment.NewLine + "google.visualization.events.removeListener(runOnce2);");
            r1.Append(Environment.NewLine + "for (var i = 0; i < collapsed.length; i++) {");
            r1.Append(Environment.NewLine + "chart.collapse(collapsed[i], true);");
            r1.Append(Environment.NewLine + "}");
            r1.Append(Environment.NewLine + "});");//End runOnce2
            r1.Append(Environment.NewLine + "// redraw the chart to account for the change in the sign");
            r1.Append(Environment.NewLine + "chart.draw(data, options);");
            r1.Append(Environment.NewLine + "});"); //End #chart_div
            r1.Append(Environment.NewLine + "// remove this event listener *before* collapsing nodes");
            r1.Append(Environment.NewLine + "// otherwise this runs multiple times");
            r1.Append(Environment.NewLine + "google.visualization.events.removeListener(runOnce);");
            r1.Append(Environment.NewLine + "// collapse all nodes");
            r1.Append(Environment.NewLine + "for (var i = 0; i < data.getNumberOfRows(); i++) { chart.collapse(i, true); }");
            r1.Append(Environment.NewLine + "});");
            r1.Append(Environment.NewLine + "chart.draw(data, options);");
            r1.Append(Environment.NewLine + "}window.onresize = function(event) { google.charts.setOnLoadCallback(drawBasic); }"); // Function drawStuff
            r1.Append(Environment.NewLine + "</scr" + "ipt>");

            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIChartCombo(int width = 900, int height = 700)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            string ReportCode = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportConfigCode");
            string ReportType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ReportType");  // 1||2||3||4;List||Pivot||Form||Other
            if (ReportType != "9") return ""; // Bao cao list 
            string QueryStringFilter = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringFilter");
            string QueryStringType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "QueryStringType");
            string SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SchemaName");
            if (SPListName != "" && SPListName != null)
                SPListName = SPListName + "." + Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName");
            else
                SPListName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "SPListName");
            string ParamList = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ParamList");
            string ColumnName = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnName");
            string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string ColumnLable = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnLable");
            string[] b; b = ColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            string ColumnType = Tools.GetDataJson(Rpt.RptConfig.Items[0], "ColumnType");
            string[] c; c = ColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
            //string bar = "line";

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            Tools.ExecParam("ChartBar", QueryStringFilter, ParamList, "", QueryStringType, context, SPListName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
            dynamic d = JObject.Parse(json);

            r1.Append(Environment.NewLine + "<div id=\"chart_div\" ><!--style=\"width: " + width + "px; height: " + height + "px;\"--></div>" +
                Environment.NewLine + "<scr" + "ipt type=\"text/java" + "script\">" + 
                Environment.NewLine + "google.charts.load('current', {'packages':['corechart']});" +
                Environment.NewLine + "google.charts.setOnLoadCallback(drawBasic);" +
                Environment.NewLine + "function drawBasic() {" +
                Environment.NewLine + " // Some raw data (not necessarily accurate)" +
                Environment.NewLine + " var data = google.visualization.arrayToDataTable(" +
                Environment.NewLine + "     [");
            for (var j = 0; j < a.Length; j++)
            {
                bool IsString = false;
                if (c.Length > j) if (c[j] == "") IsString = true;
                if (j == 0)
                    r1.Append(Environment.NewLine + "     [");
                else
                    r1.Append(Environment.NewLine + "     ,[");
                for (int i = 0; i < d.ChartBar.Items.Count; i++)
                {
                    if (i == 0)
                        r1.Append(Environment.NewLine + (IsString ? "'" + Tools.GetDataJson(d.ChartBar.Items[i], a[j]) + "'" : Tools.GetDataJson(d.ChartBar.Items[i], a[j])));
                    else
                        r1.Append(Environment.NewLine + ", " + (IsString ? "'" + Tools.GetDataJson(d.ChartBar.Items[i], a[j]) + "'" : Tools.GetDataJson(d.ChartBar.Items[i], a[j])));
                }
                r1.Append(Environment.NewLine + "     ]");
            }
            r1.Append(Environment.NewLine + "     ]);");

            // title hAxis; vAxis
            string hAxis = context.GetRequestVal("hAxis"); if (hAxis == "") hAxis = "y";
            string vAxis = context.GetRequestVal("vAxis"); if (vAxis == "") hAxis = "x";

            r1.Append(Environment.NewLine + " var options = {" +
                Environment.NewLine + "     title : 'Monthly Coffee Production by Country'," +
                Environment.NewLine + "     vAxis: {title: '" + hAxis + "'}," +
                Environment.NewLine + "     hAxis: {title: '" + vAxis + "'}," +
                Environment.NewLine + "     seriesType: 'bars'," +
                Environment.NewLine + "     series: {5: {type: 'line'}}" +
                Environment.NewLine + " };" +
                Environment.NewLine + " var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));" +
                Environment.NewLine + " chart.draw(data, options);" +
                Environment.NewLine + "}" +
                Environment.NewLine + "window.onresize = function(event) {" +
                Environment.NewLine + "google.charts.setOnLoadCallback(drawBasic);" +
                Environment.NewLine + "}");
            r1.Append(Environment.NewLine + "</scr" + "ipt>");

            r = r1.ToString(); r1 = null;
            return r;
        }
        #endregion
    }
}



/*if (PrevColName3 != "")
{
    if (PrevCol3 != "" && PrevCol3 != Tools.GetDataJson(dList.ListForm.Items[i], PrevColName3))
    {
        r12.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "_3_" + PrevCol3 + "\">");
        r12.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + ">" + context.GetLanguageLable(PrevColLable3) + ": <i>" + PrevCol3 + "</i>");
        for (int j = ColumnSumStart; j < b.Length; j++)
        {
            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
            switch (a4[0])
            {
                case "Numeric":
                    if (a4.Length > 2) if (a4[2] == "1") r12.Append(Environment.NewLine + "<td align=right><i>" + Tools.FormatNumber(SumCol3[j].ToString(), (a4[1] == "0")) + "</i>");
                    break;
                default:
                    r12.Append(Environment.NewLine + "<td>");
                    break;
            }
            SumCol3[j] = 0;
        }
        r12.Append(r13.ToString()); r13 = new StringBuilder();
    }
    PrevCol3 = Tools.GetDataJson(dList.ListForm.Items[i], PrevColName3);
}
else
{ r12.Append(r13.ToString()); r13 = new StringBuilder(); }
if (PrevColName2 != "")
{
    if (PrevCol2 != "" && PrevCol2 != Tools.GetDataJson(dList.ListForm.Items[i], PrevColName2))
    {
        r11.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "_2_" + PrevCol2 + "\">");
        r11.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + ">" + context.GetLanguageLable(PrevColLable2) + ": <b>" + PrevCol2 + "</b>");
        for (int j = ColumnSumStart; j < b.Length; j++)
        {
            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
            switch (a4[0])
            {
                case "Numeric":
                    if (a4.Length > 2) if (a4[2] == "1") r11.Append(Environment.NewLine + "<td align=right><b>" + Tools.FormatNumber(SumCol2[j].ToString(), (a4[1] == "0")) + "</b>");
                    break;
                default:
                    r11.Append(Environment.NewLine + "<td>");
                    break;
            }
            SumCol2[j] = 0;
        }
        r11.Append(r12.ToString()); r12 = new StringBuilder();
    }
    PrevCol2 = Tools.GetDataJson(dList.ListForm.Items[i], PrevColName2);
}
else
{ r11.Append(r12.ToString()); r12 = new StringBuilder(); }
if (PrevColName1 != "")
{
    if (PrevCol1 != "" && PrevCol1 != Tools.GetDataJson(dList.ListForm.Items[i], PrevColName1))
    {
        r1.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "_1_" + PrevCol1 + "\">");
        r1.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + ">" + context.GetLanguageLable(PrevColLable1) + ": <b><u>" + PrevCol1 + "</u></b>");
        for (int j = ColumnSumStart; j < b.Length; j++)
        {
            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
            switch (a4[0])
            {
                case "Numeric":
                    if (a4.Length > 2) if (a4[2] == "1") r1.Append(Environment.NewLine + "<td align=right><b><u>" + Tools.FormatNumber(SumCol1[j].ToString(), (a4[1] == "0")) + "</u></b>");
                    break;
                default:
                    r1.Append(Environment.NewLine + "<td>");
                    break;
            }
            SumCol1[j] = 0;
        }
        r1.Append(r11.ToString()); r11 = new StringBuilder();
    }
    PrevCol1 = Tools.GetDataJson(dList.ListForm.Items[i], PrevColName1);
}
else
{ r1.Append(r11.ToString()); r11 = new StringBuilder(); }*/

/*
            if (PrevColName3 != "")
            {
                if (PrevCol3 != "")
                {
                    r12.Append(Environment.NewLine + "<tr id=\"trrowid_3_" + PrevCol3 + "\">");
                    r12.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + ">" + context.GetLanguageLable(PrevColLable3) + ": <i>" + PrevCol3 + "</i>");
                    for (int j = ColumnSumStart; j < b.Length; j++)
                    {
                        string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                        switch (a4[0])
                        {
                            case "Numeric":
                                if (a4.Length > 2) if (a4[2] == "1") r12.Append(Environment.NewLine + "<td align=right><i>" + Tools.FormatNumber(SumCol3[j].ToString(), (a4[1] == "0")) + "</i>");
                                break;
                            default:
                                r12.Append(Environment.NewLine + "<td>");
                                break;
                        }
                        SumCol3[j] = 0;
                    }
                    r12.Append(r13.ToString()); r13 = new StringBuilder();
                }
            }
            else
            { r12.Append(r13.ToString()); r13 = new StringBuilder(); }

            if (PrevColName2 != "")
            {
                if (PrevCol2 != "")
                {
                    r11.Append(Environment.NewLine + "<tr id=\"trrowid_2_" + PrevCol2 + "\">");
                    r11.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + ">" + context.GetLanguageLable(PrevColLable2) + ": <b>" + PrevCol2 + "</b>");
                    for (int j = ColumnSumStart; j < b.Length; j++)
                    {
                        string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                        switch (a4[0])
                        {
                            case "Numeric":
                                if (a4.Length > 2) if (a4[2] == "1") r11.Append(Environment.NewLine + "<td align=right><b>" + Tools.FormatNumber(SumCol2[j].ToString(), (a4[1] == "0")) + "</b>");
                                break;
                            default:
                                r11.Append(Environment.NewLine + "<td>");
                                break;
                        }
                        SumCol2[j] = 0;
                    }
                    r11.Append(r12.ToString()); r12 = new StringBuilder();
                }
            }
            else
            { r11.Append(r12.ToString()); r12 = new StringBuilder(); }
            // check group col
            if (PrevColName1 != "")
            {
                if (PrevCol1 != "")
                {
                    r1.Append(Environment.NewLine + "<tr id=\"trrowid_1_" + PrevCol1 + "\">");
                    r1.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + ">" + context.GetLanguageLable(PrevColLable1) + ": <b><u>" + PrevCol1 + "</u></b>");
                    for (int j = ColumnSumStart; j < b.Length; j++)
                    {
                        string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                        switch (a4[0])
                        {
                            case "Numeric":
                                if (a4.Length > 2) if (a4[2] == "1") r1.Append(Environment.NewLine + "<td align=right><b><u>" + Tools.FormatNumber(SumCol1[j].ToString(), (a4[1] == "0")) + "</u></b>");
                                break;
                            default:
                                r1.Append(Environment.NewLine + "<td>");
                                break;
                        }
                        SumCol1[j] = 0;
                    }
                    r1.Append(r11.ToString()); r11 = new StringBuilder();
                }
            }
            else
            { r1.Append(r11.ToString()); r11 = new StringBuilder(); }
            */

/*
           for (var i = 0; i < a1.Length; i++)
           {
               DataVal[i] = ""; DataTxt[i] = ""; DataParent[i] = "";
               string[] a31 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None);
               string v = context.GetRequestVal(a1[i]); string onclick = "";
               // fix PageSize
               if (v == "" && a1[i] == "PageSize") v = context.GetSession("PageSizeReport");
               if (v == "") v = Tools.ParseValue(context, a31[1], true); //a31[1];
               //if (v == "") v = a31[1];
               //if (v.Length > 7) if (v.Substring(0, 7) == "REQUEST") v = context.GetRequestVal(v.Substring(7)); ///Basetab/Dbtab||TabIndex;Function2GroupID;MenuOn||Function2Group;REQUESTFunction2GroupID;Off^/Basetab/Dbtab||TabIndex;User2GroupID;MenuOn||User2Group;REQUESTUser2GroupID;Off
               v = Tools.ParseValue(context, v, false);
               string val = v;
               //b.Append(Environment.NewLine + "<br>a3[i]:" + a3[i]);
               if (a31[0] == "Hidden" || a1[i] == "Page" || a1[i] == "PageSize")
               {
                   //if (v.Length > 7) if (v.Substring(0, 7) == "SESSION") val = context.GetSession(v.Substring(7));
                   val = context.ReplaceSessionValue(v);
                   b.Append(UIDef.UIHidden(a1[i], val));
               }
               else
               {
                   b.Append(Environment.NewLine + "<tr class=\"formrow\"><td align=right><label class=\"name\">" + context.GetLanguageLable(a2[i]) + ": </label><td class=\"controls\">");
                   string ReqJson = ""; 
                   switch (a31[0])
                   {
                       //a31[2]: Option check Require
                       //case "Textarea":
                       //    b.Append(UIDef.UITextarea(a1[i], val, " cols=\"" + a31[3] + "\" rows=\"" + a31[4] + "\" ", "", ""));
                       //    break;
                       case "Textbox":
                           IsFilterForm = true;
                           ReqJson = UIFormElements.UITextbox(a1, i, val, a31);
                           if (IsHTML)
                               b.Append(ReqJson);
                           else
                               b.Append(Environment.NewLine + "<b>" + val + "</b>");
                           break;
                       case "Numeric":
                           IsFilterForm = true;
                           ReqJson = UIFormElements.UINumeric(a1, i, val, a31);
                           if (IsHTML)
                               b.Append(ReqJson);
                           else
                           {
                               int iRound = 0; try { iRound = int.Parse(a31[1]); } catch { iRound = 0; }
                               b.Append(Environment.NewLine + "<b>" + Tools.FormatNumber(val, iRound) + "</b>");
                           }
                           break;
                       case "Date":
                           IsFilterForm = true;
                           DateTime dNow = DateTime.Now; int iDay = 0;
                           if (val == "@")
                           {
                               val = string.Format("{0:dd/MM/yyyy}", dNow);
                           }
                           else if (Tools.Left(val, 1) == "@")
                           {
                               if (Tools.Left(val, 2) == "@+") iDay = int.Parse(Tools.Right(val, 2, false));
                               if (Tools.Left(val, 2) == "@-") iDay = 0 - int.Parse(Tools.Right(val, 2, false));
                               val = string.Format("{0:dd/MM/yyyy}", dNow.AddDays(iDay));
                           }
                           if (IsHTML)
                               b.Append(UIDef.UIDate(a1[i], val, "", UIDef.CNextFocusAction(a1[i]), "FilterForm"));
                           else
                               b.Append(Environment.NewLine + "<b>" + val + "</b>");

                           break;
                       case "Datetime":
                           IsFilterForm = true;
                           DateTime dNow1 = DateTime.Now; int iDay1 = 0;
                           if (val == "@")
                           {
                               val = string.Format("{0:dd/MM/yyyy HH:mm}", dNow1);
                           }
                           else if (Tools.Left(val, 1) == "@")
                           {
                               if (Tools.Left(val, 2) == "@+") iDay1 = int.Parse(Tools.Right(val, 2, false));
                               if (Tools.Left(val, 2) == "@-") iDay1 = 0 - int.Parse(Tools.Right(val, 2, false));
                               val = string.Format("{0:dd/MM/yyyy HH:mm}", dNow1.AddDays(iDay1));
                           }
                           if (IsHTML)
                               b.Append(UIDef.UIDateTime(a1[i], val, "", UIDef.CNextFocusAction(a1[i]), "FilterForm"));
                           else
                               b.Append(Environment.NewLine + "<b>" + val + "</b>");

                           break;
                       case "Time":
                           IsFilterForm = true;
                           if (IsHTML)
                               b.Append(UIDef.UITime(a1[i], val, "", UIDef.CNextFocusAction(a1[i]), "FilterForm"));
                           else
                               b.Append(Environment.NewLine + "<b>" + val + "</b>");
                           break;
                       case "Checkbox":
                           IsFilterForm = true;
                           b.Append(UIDef.UICheckbox(a1[i], a31[3], a31[4], val, UIDef.FocusAfter(a1[i])));
                           break;
                       case "MutilCheckbox":
                           IsFilterForm = true;
                           ReqJson = context.InputDataSetParam(a31[4]);
                           onclick = ""; if (a31.Length > 8) onclick = " onclick=\"" + a31[6] + ",'" + a1[i] + "'," + i + ");\"";
                           b.Append(UIDef.UIMultipleSelect(a1[i], context, ref DataDAO, a31[3], ReqJson, a31[5], ref val, UIDef.FocusAfter(a1[i]), "", ref DataVal[i], ref DataParent[i]));
                           //r1.Append(UIDef.UIDD(a1[i], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a1[i]), "", (Tools.CIntNull(a5[j]) == 1 ? ClassReadonly : ""), int.Parse(a31[6]), int.Parse(a31[7])));
                           break;
                       case "DivMutilCheckbox":
                           IsFilterForm = true;
                           ReqJson = context.InputDataSetParam(a31[4]);
                           //r1.Append(UIDef.UIMultipleSelect(a1[i], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a1[i]), ""));
                           onclick = ""; if (a31.Length > 8) onclick = " onclick=\"" + a31[8] + ",'" + a1[i] + "'," + i + ");\"";
                           b.Append(UIDef.UIDD(a1[i], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a1[i]), onclick, "", int.Parse(a31[6]), int.Parse(a31[7]), ref DataVal[i], ref DataParent[i], "document.FilterForm"));
                           break;
                       case "Radio":
                           IsFilterForm = true;
                           //b.Append(UIDef.UIRadio(a1[i], context.ReplaceStringLangValue(a31[3]), a31[4], val, UIDef.FocusAfter(a1[i])));
                           b.Append(UIFormElements.UIRadio(context, a1, i, val, a31));
                           break;
                       //case "Actb":
                       //    ReqJson = context.InputDataSetParam(a31[6]);
                       //    UIDef.OptionStringVal(ref DataDAO, a31[5], ReqJson, a31[7], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                       //    s = UIDef.SelectStrOption(DataVal[i], DataTxt[i], ref val, true);
                       //    r1.Append(UIDef.UISelectStr(a[j], DataVal[j], DataTxt[j], val, true, "", ""/ *UIDef.CNextFocus(a[j])* /
           +" markline(" + i + ", true, true);"));
                       //    ReqJson = context.InputDataSetParam(a31[6]);
                       //    b.Append(UIDef.UIActb(a1[i], ref DataDAO, a31[5], ReqJson, a31[7], val, " size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a1[i]), "FilterForm"));
                       //    break;
                       //case "ActbText":
                       //    b.Append(UIDef.UIActbStr(a1[i], a31[5], a31[6], val, " size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a1[i]), "FilterForm"));
                       //    break;
                       case "Selectbox":
                           IsFilterForm = true;
                           ReqJson = context.InputDataSetParam(a31[4]);
                           UIDef.OptionStringVal(ref DataDAO, a31[3], ReqJson, a31[5], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                           if (a31.Length > 6)
                               ReqJson = UIDef.UISelectStr(a1[i], DataVal[i], DataTxt[i], ref val, true, "", a31[6] + "," + (i + 1) + ");" + UIDef.NextFocus(a1[i]), "document.FilterForm");
                           else
                               ReqJson = UIDef.UISelectStr(a1[i], DataVal[i], DataTxt[i], ref val, true, "", UIDef.NextFocus(a1[i]), "document.FilterForm");
                           //b.Append(UIDef.UISelect(a1[i], ref DataDAO, a31[3], ReqJson, a31[5], val, true, "", UIDef.CNextFocus(a1[i])));
                           if (IsHTML)
                               b.Append(ReqJson);
                           else
                               b.Append(Environment.NewLine + "<b>" + val + "</b>");
                           break;
                       case "SelectboxText":
                           IsFilterForm = true;
                           ReqJson = UIDef.UISelectStr(a1[i], context.ReplaceStringLangValue(a31[3]), a31[4], ref val, true, "", UIDef.NextFocus(a1[i]), "document.FilterForm");
                           if (IsHTML)
                               b.Append(ReqJson);
                           else
                               b.Append(Environment.NewLine + "<b>" + val + "</b>");
                           break;
                   }
               }
               jsdbcols.Append(Environment.NewLine + "jsdbcols[" + i + "] = '" + a1[i] + "';" +
                   Environment.NewLine + "DataVal[" + i + "] = '" + DataVal[i] + "';" +
                   Environment.NewLine + "DataTxt[" + i + "] = '" + DataTxt[i] + "';" +
                   Environment.NewLine + "DataParent[" + i + "] = '" + DataParent[i] + "';");
           }
           if (IsHTML && IsFilterForm)
           {
               string sHtml = "HTML";
               b.Append(Environment.NewLine + "<tr class=\"formrow\"><td align=right><label class=\"name\">" + context.GetLanguageLable("DataOutput") + ": </label><td class=\"controls\">" + UIDef.UISelectStr("DataOutput", "text/html||application/vnd.ms-excel||application/vnd.openxmlformats-officedocument.spreadsheetml.sheet||application/msword||application/pdf", "HTML||Excel||Excel xlsx||Word||Pdf", ref sHtml, false, "", ""));
               b.Append(Environment.NewLine + "<tr class=\"formrow\"><td><label class=\"name\"> </label><td class=\"controls\">" + UIDef.UIButton("bntSearch", context.GetLanguageLable("Search"), "btnSearch()", " class=\"btn find\"") +
                   UIDef.UIButton("bntSearchReset", context.GetLanguageLable("Reset"), false, " class=\"btn refresh\""));
           }

           b.Append(Environment.NewLine + "</table>");
           */
// mg-top-10
////if (IsHTML && IsFilterForm)
////{
////    //b.Append(Environment.NewLine + "<tr class=\"action mg-top-10\"><td>");
////    b.Append(Environment.NewLine + "<div class=\"action\">");
////}

////if (IsHTML && IsFilterForm) b.Append(UIDef.UIButton("bntSearch", context.GetLanguageLable("Search"),
////    "var a=this.form.elements['Page'];if(a)a.value=1;btnSearch('');", " class=\"btn find\"") +
////    UIDef.UIButton("bntSearchReset", context.GetLanguageLable("Reset"), "btnReset(this.form);", " class=\"btn refresh\""));//<td><label class=\"name\"></label> </td>

////if (IsHTML && IsFilterForm)
////{
////    //b.Append(Environment.NewLine + "<tr class=\"action mg-top-10\"><td>");
////    b.Append(Environment.NewLine + "</div>");
////}