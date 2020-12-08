using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Utils {
    public class BaseTab
    {
        #region Properties
        public string TabIndex;
        public string MenuID;
        public string MenuOn;
        public dynamic BTab;
        public dynamic BTabGrp;
        private HRSContext context;
        private ToolDAO DataDAO;
        private ToolDAO ConfigDAO;
        #endregion

        #region Private Method
        private void SetParam()
        {
            dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            bool IsCached = context._cache.Get("DBConfig_" + TabIndex + "_" + context.GetSession("language"), out d);
            //json = context.GetSession("DBConfig_" + TabIndex);
            //if (json == "")
            if (!IsCached)
            {
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"DBConfigID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"-1\"}," +
                    "{\"ParamName\":\"DBConfigCode\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"40\", \"InputValue\":\"" + TabIndex + "\"}," +
                    "{\"ParamName\":\"Keyword\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"Page\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                    "{\"ParamName\":\"PageSize\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + context.GetSession("PageSizeBaseTab") + "\"}," +
                    "{\"ParamName\":\"Rowcount\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}" +
                    "]}");
                /*
                 ," +
                    "{\"ParamName\":\"cursor\", \"ParamType\":\"999\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"\"}
                 */
                ConfigDAO.ExecuteStore("DBConfig", "SP_CMS__DBConfig_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString, false);
                //json = context.ReplaceListSessionValue(json);
                BTab = JObject.Parse(json);
                //context.SetSession("DBConfig_" + TabIndex, json);
                context._cache.Set("DBConfig_" + TabIndex + "_" + context.GetSession("language"), BTab);
                MenuID = Tools.GetDataJson(BTab.DBConfig.Items[0], "FunctionID");
            }
            else
            {
                //BTab = JObject.Parse(json);
                BTab = d;
                MenuID = Tools.GetDataJson(BTab.DBConfig.Items[0], "FunctionID");
            }

            //json = context.GetSession("DBConfigGrp_" + TabIndex);
            //if (json == "")
            IsCached = false;
            IsCached = context._cache.Get("DBConfigGrp_" + TabIndex + "_" + context.GetSession("language"), out d);
            if (!IsCached)
            {
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"DBConfigGrpID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                    "{\"ParamName\":\"DBConfigID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + Tools.GetDataJson(BTab.DBConfig.Items[0], "DBConfigID") + "\"}," +
                    "{\"ParamName\":\"Page\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                    "{\"ParamName\":\"PageSize\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + context.GetSession("PageSizeBaseTab") + "\"}," +
                    "{\"ParamName\":\"Rowcount\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}" +
                    "]}");
                ConfigDAO.ExecuteStore("DBConfigGrp", "SP_CMS__DBConfigGrp_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                //json = context.ReplaceListSessionValue(json);
                BTabGrp = JObject.Parse(json);
                context._cache.Set("DBConfigGrp_" + TabIndex + "_" + context.GetSession("language"), BTabGrp);
                //context.SetSession("DBConfigGrp_" + TabIndex, json);
            }
            else
            {
                //BTabGrp = JObject.Parse(json);
                BTabGrp = d;
            }

            //json = "";//context.GetSession("BTabHelper_" + TabIndex);
            //if (json == "")
            IsCached = false;
            IsCached = context._cache.Get("BTabHelper_" + TabIndex + "_" + context.GetSession("language"), out d);
            if (!IsCached)
            {
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"DBConfigHelperID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"-1\"}," +
                    "{\"ParamName\":\"DBConfigID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + Tools.GetDataJson(BTab.DBConfig.Items[0], "DBConfigID") + "\"}," +// Tools.GetDataJson(BTab.DBConfig.Items[0], "DBConfigID")
                "{\"ParamName\":\"Page\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                    "{\"ParamName\":\"PageSize\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + context.GetSession("PageSizeBaseTab") + "\"}," +
                    "{\"ParamName\":\"Rowcount\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}" +
                    "]}");
                ConfigDAO.ExecuteStore("Helper", "SP_CMS__DBConfigHelper_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                //context.SetSession("BTabHelper_" + TabIndex, json);
                context._cache.Set("BTabHelper_" + TabIndex + "_" + context.GetSession("language"), JObject.Parse(json));
            }
        }
        private string UIPageTable(HRSContext context, int CurrPage, int PageSize, int RowsCount, string FrmName = "document.FilterForm")
        {
            int PageCount = RowsCount / (PageSize <= 0 ? int.Parse(context.GetSession("PageSizeBaseTab")) : PageSize);
            if (PageCount * PageSize < RowsCount) PageCount = PageCount + 1;
            if (PageCount <= 0) return "";
            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append(Environment.NewLine + "<div class=\"row table-footer\">" +
                Environment.NewLine + "<div class=\"cantrai\">");
            if (CurrPage > 1) r1.Append(Environment.NewLine + "<a href=\"javascript: Goto('First', " + CurrPage.ToString() + ");\" class=\"icon-table-footer\"><i class=\"fa fa-angle-double-left\" aria-hidden=\"true\"></i></a>" +
                Environment.NewLine + "<a href=\"javascript: Goto('Back', " + CurrPage.ToString() + ");\" class=\"icon-table-footer\"><i class=\"fa fa-angle-left\" aria-hidden=\"true\"></i></a>");
            int iStart = (CurrPage - 5 > 1 ? CurrPage - 5 : 1);
            int iEnd = (CurrPage + 5 > PageCount ? PageCount : CurrPage + 5);
            for (int i = iStart; i <= iEnd; i++)
            {
                r1.Append(Environment.NewLine + "<a href=\"javascript: Goto(" + i.ToString() + ", " + CurrPage.ToString() + ");\" " + (i == CurrPage ? "class=\"active\"" : "") + ">" + i.ToString() + "</a>");
            }
            if (CurrPage < PageCount) r1.Append(Environment.NewLine + "<a href=\"javascript: Goto('Next', " + CurrPage.ToString() + ");\" class=\"icon-table-footer\"><i class=\"fa fa-angle-right\" aria-hidden=\"true\"></i></a>" +
                Environment.NewLine + "<a href=\"javascript: Goto('Last', " + CurrPage.ToString() + ");\" class=\"icon-table-footer\"><i class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></i></a>");
            r1.Append(Environment.NewLine + "</div>" +
                Environment.NewLine + "<div class=\"canphai\">" +
                Environment.NewLine + "<div class=\"ghichu\">" + context.GetLanguageLable("Page") + ": <span>" + CurrPage.ToString() + "</span>/<span>" + PageCount.ToString() + "</span>" + context.GetLanguageLable("TotalRecord") + ": <span>" + RowsCount.ToString() + "</span></div></div></div>");

            r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                    Environment.NewLine + "function Goto(page, currPage){" +
                    Environment.NewLine + "var f = " + FrmName + "; " +
                    Environment.NewLine + "if (page == 'First' && currPage != 1){" +
                    Environment.NewLine + "f.Page.value = 1; f.PageSize.value = " + PageSize.ToString() + ";f.submit();" +
                    Environment.NewLine + "} else if (page == 'Back' && currPage > 1) {" +
                    Environment.NewLine + "f.Page.value = (parseInt(f.Page.value) - 1); f.PageSize.value = " + PageSize.ToString() + ";f.submit();" +
                    Environment.NewLine + "} else if (page == 'Next' && currPage < " + PageCount.ToString() + ") {" +
                    Environment.NewLine + "f.Page.value = (parseInt(f.Page.value) + 1); f.PageSize.value = " + PageSize.ToString() + ";f.submit();" +
                    Environment.NewLine + "} else if (page == 'Last' && currPage != " + PageCount.ToString() + ") {" +
                    Environment.NewLine + "f.Page.value = " + PageCount.ToString() + "; f.PageSize.value = " + PageSize.ToString() + ";f.submit();" +
                    Environment.NewLine + "} else if (page != currPage) {" +
                    Environment.NewLine + "f.Page.value = page; f.PageSize.value = " + PageSize.ToString() + ";f.submit();" +
                    Environment.NewLine + "} else JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("YouAreNotChoiceData") + "', '', '0');" + //alert('" + context.GetLanguageLable("YouAreNotChoiceData") + "')
                    Environment.NewLine + "}");
            r1.Append(Environment.NewLine + "</script>"); // row / box
            
            r = r1.ToString(); r1 = null;
            return r;
        }
        #endregion

        #region Contruction
        public BaseTab (HRSContext _context, ToolDAO _ConfigDAO, ToolDAO _DataDAO)
        {
            context = _context; ConfigDAO = _ConfigDAO; DataDAO = _DataDAO;
            TabIndex = _context.GetRequestTabIndex();
            MenuOn = _context.GetRequestMenuOn();
            MenuID = _context.GetRequestOneValue("MenuID");
            SetParam();
        }
        #endregion

        #region UI SearchForm || ListForm || ListUpdateForm || EditForm || StreeForm
        public string UIHeaderFormList()
        {
            string RequestImport =  Tools.GetDataJson(BTab.DBConfig.Items[0], "RequestImport");
            string urlFormExcel =   Tools.GetDataJson(BTab.DBConfig.Items[0], "urlFormExcel");
            string urlDbXlsx =      Tools.GetDataJson(BTab.DBConfig.Items[0], "urlDbXlsx");
            //if (urlFormExcel == "") urlFormExcel = "/Excel/FormExcel?TabIndex=" + TabIndex + "";
            if (urlDbXlsx == "") urlDbXlsx = "/Excel/DbXlsx?TabIndex=" + TabIndex + "" + "&" + context.GetQueryStrPage("TabIndex");

            urlDbXlsx = context.ReplaceRequestValue(urlDbXlsx);
            urlFormExcel = context.ReplaceRequestValue(urlFormExcel);

            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append(Environment.NewLine + "<div class=\"titlebox titlebox-fix\">" +
                Environment.NewLine + "<h4 class=\"titlebox-title\">" + context.GetLanguageLable(TabIndex) +
                Environment.NewLine + "<a id=\"aObj\" href=\"javascript:showhelp('aObj', '" + TabIndex + "', 1)\" class=\"question\">" +
                Environment.NewLine + "<i class=\"fa fa-question-circle-o\" aria-hidden=\"true\"></i>" +
                Environment.NewLine + "</a>");
            r1.Append(Environment.NewLine + "<div class=\"canphai\">");
            if (RequestImport == "2") r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Import") , "ImportRaw();", "tvc-nhap-excel vang2") +
                UIDef.UIHrefButton(context.GetLanguageLable("Form"), "ImportForm();", "tvc-bieu-mau xanh"));
            if (RequestImport == "2" || RequestImport == "1") r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Export"), "ExportXLS();", "tvc-xuat-excel xanhla"));
            r1.Append(Environment.NewLine + "</div>");
            r1.Append(Environment.NewLine + "</h4>");
            r1.Append(Environment.NewLine + "</div>");

            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            if (RequestImport == "2")
            {
                if (urlFormExcel == "")
                {
                    r1.Append(Environment.NewLine + "function ImportForm() {" +
                    Environment.NewLine + "window.location.href='/Excel/FormExcel?TabIndex=" + TabIndex + "';//_attw = window.open('/Excel/FormExcel?TabIndex=" + TabIndex + "', " +
                    Environment.NewLine + "//'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=50, left=50, width=700, height=200');" +
                    Environment.NewLine + "}");
                }
                else
                {
                    r1.Append(Environment.NewLine + "function ImportForm() {" +
                    Environment.NewLine + "    var f = document.FilterForm; " +
                    Environment.NewLine + "    f.method = \"POST\";" +
                    Environment.NewLine + "    f.action = \"" + urlFormExcel + "\";" +
                    Environment.NewLine + "    f.submit();" +
                    Environment.NewLine + "}");
                }
            }
            
            if (RequestImport == "2") r1.Append(Environment.NewLine + "function ImportRaw() {" +
                    Environment.NewLine + "_attw = window.open('/Excel/ImportExcel?TabIndex=" + TabIndex + "', " +
                    Environment.NewLine + "'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=50, left=50, width=1000, height=500');" +
                    Environment.NewLine + "}");
            if (RequestImport == "2" || RequestImport == "1") r1.Append(Environment.NewLine + "function ExportXLS() {" +
                    Environment.NewLine + "window.location.href='" + urlDbXlsx + "';//_attw = window.open('/Excel/DbXlsx?TabIndex=" + TabIndex + "&" + context.GetQueryStrPage("TabIndex") + "', " +
                    Environment.NewLine + "//'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=50, left=50, width=700, height=500');" +
                    Environment.NewLine + "}");
            r1.Append(Environment.NewLine + "function showhelp(obj, C, stage) {" +
                    Environment.NewLine + "var helpregion = document.getElementById('helpdisp'); " +
                    Environment.NewLine + "if (helpregion) return;" +
                    Environment.NewLine + "var a = JsPopupInfo(obj, 'helpdisp', '" + context.GetLanguageLable("Help") + "'); " +
                    Environment.NewLine + "ajPage('/Basetab/BasetabText?id=' + C + '&stage=' + stage, showhelpret);" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function showhelpret(http) {" +
                    Environment.NewLine + "    var helpregion = document.getElementById('idContenthelpdisp');" +
                    Environment.NewLine + "    helpregion.style.display = 'block';" +
                    Environment.NewLine + "    helpregion.innerHTML = http.responseText;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "</script>");
            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIFilterForm()
        {
            StringBuilder b = new StringBuilder(); StringBuilder bx = new StringBuilder(); string r = "";
            b.Append(Environment.NewLine + "<form onsubmit=\"return false\" name=\"FilterForm\" method=\"GET\">" +
                UIDef.UIHidden("MenuOn", MenuOn) +
                UIDef.UIHidden("iframe", context.GetRequestVal("iframe")) +
                UIDef.UIHidden("MenuID", MenuID) +
                UIDef.UIHidden("TabIndex", TabIndex) +
                UIDef.UIHidden("RptID", TabIndex) +
                UIDef.UIHidden("FormTypeAction", ""));
            
            string[] a1; string[] a2; string[] a3; string[] a4;
            string QueryStringFilter =      Tools.GetDataJson(BTab.DBConfig.Items[0], "QueryStringFilter");
            string QueryStringTextLable =   Tools.GetDataJson(BTab.DBConfig.Items[0], "QueryStringTextLable");
            string QueryStringType =        Tools.GetDataJson(BTab.DBConfig.Items[0], "QueryStringType");
            string QueryStringInline =      Tools.GetDataJson(BTab.DBConfig.Items[0], "QueryStringInline");
            string FormType =               Tools.GetDataJson(BTab.DBConfig.Items[0], "FormType"); // List; Tree; Edit

            a1 = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            a2 = QueryStringTextLable.Split(new string[] { "^" }, StringSplitOptions.None);
            a3 = QueryStringType.Split(new string[] { "^" }, StringSplitOptions.None);
            a4 = QueryStringInline.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataVal = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataTxt = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataParent = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            StringBuilder jsdbcols; jsdbcols = new StringBuilder();
            jsdbcols.Append("var jsdbcols = new Array();" +
                    Environment.NewLine + "var DataVal = new Array();" +
                    Environment.NewLine + "var DataTxt = new Array();" +
                    Environment.NewLine + "var DataParent = new Array();");
            string json = "";
            if (FormType.ToLower() == "program")
            {
                string[] lblButton = Tools.GetDataJson(BTab.DBConfig.Items[0], "ActionName").Split(new string[] { "$" }, StringSplitOptions.None);
                string[] StoreName = Tools.GetDataJson(BTab.DBConfig.Items[0], "spName").Split(new string[] { "$" }, StringSplitOptions.None);
                UIFormElements.UIFillterForm(ref bx, ref jsdbcols, ref json, context, DataDAO, a1, a2, a3, a4, lblButton, StoreName);
            }
            else
                UIFormElements.UIFillterForm(ref bx, ref jsdbcols, ref json, context, DataDAO, a1, a2, a3, a4, ";");

            b.Append(bx.ToString()); bx = null;
            
            b.Append(Environment.NewLine + "</form>" +
                Environment.NewLine + "<script language=\"javascript\">" + jsdbcols.ToString() +
                Tools.GenResetFunc("nextfocus") +
                Tools.GenResetFunc("cnextfocus") +
                Environment.NewLine + "function btnSearch (a) {" +
                Environment.NewLine + "    var f = document.FilterForm; " +
                Environment.NewLine + "    f.method = \"GET\";" +
                Environment.NewLine + "    f.action = \"\";" +
                Environment.NewLine + "    f.FormTypeAction.value=a;" +
                Environment.NewLine + "    f.submit();" +
                Environment.NewLine + "}" +
                Tools.GenResetFunc() +
                Environment.NewLine + "</script>");
            r = b.ToString(); b = null;
            return r;
        }
        private void UIActionButton(string ParamAction, string ColumnID, string DeleteColumnName, ref int ColLenght, ref string ActionHtml, ref string ActionButtonHtml, bool IsDelete = true)
        {
            //string ParamListInputInline =     Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumn");
            string StoreGetPrintForm =          Tools.GetDataJson(BTab.DBConfig.Items[0], "StoreGetPrintForm");
            string StoreGetPrintFormOption =    Tools.GetDataJson(BTab.DBConfig.Items[0], "StoreGetPrintFormOption");
            bool IsApproved = context.CheckApproved(0, 0, TabIndex);
            if (ParamAction != "" && ColumnID != "" && (IsDelete == true || IsApproved == true)) // Quyen Delete
            {
                ColLenght = -1;
                string[] w = ParamAction.Split(new string[] { "^" }, StringSplitOptions.None);
                ActionHtml = UIDef.UICheckbox("chkAll", "", "1", "0", " onclick=\"checkAll(this, document.ListForm, '" + ColumnID + "');\"");
                
                if (StoreGetPrintFormOption != "") ActionButtonHtml = ActionButtonHtml +
                    UIDef.UIHrefButton(context.GetLanguageLable("PrintOption"), "showPrint('bntAction999', 'ListForm', '" + ColumnID + "');", "fa fa-print xanh", "", "bntAction999");
                //UIDef.UIButton("bntAction999", context.GetLanguageLable("FormOption"), "showPrint('bntAction999', 'ListForm', '" + ColumnID + "', " class=\"btn print\");") + "&nbsp;&nbsp;";
                if (StoreGetPrintForm != "") ActionButtonHtml = ActionButtonHtml +
                    UIDef.UIHrefButton(context.GetLanguageLable("Print"), "execPrint();", "fa fa-print xanh", "", "bntPrint999");
                        //UIDef.UIButton("bntPrint999", context.GetLanguageLable("Print"), "execPrint(document.getElementById('aPrint'), 'ListForm', '" + ColumnID + "');", " class=\"btn print\"");
                        //"<a id=\"aPrint\" class=\"cus-tooltip print\" data-title=\"" +
                        //        context.GetLanguageLable("Print") + "\" href=\"javascript:execPrint(document.getElementById('aPrint'), 'ListForm', '" + ColumnID + "');\" data-tooltip=\"" +
                        //       context.GetLanguageLable("Print") + "\"><img src=\"/images/Print2.gif\"></a>&nbsp;&nbsp;";
                for (int i = 0; i < w.Length; i++)
                {
                    string[] w1 = w[i].Split(new string[] { ";" }, StringSplitOptions.None);
                    switch (w1[0])
                    {
                        case "-1":
                            if (IsDelete) ActionButtonHtml = ActionButtonHtml +
                                 UIDef.UIHrefButton(context.GetLanguageLable(w1[1]), "document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');", "fa fa-trash-o do", "", "bntAction" + i);
                                    //UIDef.UIButton("bntAction" + i, context.GetLanguageLable(w1[1]), "document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');", " class=\"btn delete\"");
                                    //"<a id=\"bntAction" + i + "\" class=\"cus-tooltip\" data-title=\"" +
                                    //context.GetLanguageLable(w1[1]) + "\" href=\"javascript:document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');\" data-tooltip=\"" +
                                    //context.GetLanguageLable(w1[1]) + "\"><img src=\"/images/xoa.png\"></a>&nbsp;&nbsp;";
                            break;
                        case "1":
                            if (IsApproved) ActionButtonHtml = ActionButtonHtml +
                                UIDef.UIHrefButton(context.GetLanguageLable(w1[1]), "document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');", "tvc-unlocked xanhla", "", "bntAction" + i);
                                //    UIDef.UIButton("bntAction" + i, context.GetLanguageLable(w1[1]), "document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');", " class=\"btn unlock\"");
                                //"<a id=\"bntAction" + i + "\" class=\"cus-tooltip\" data-title=\"" +
                                //context.GetLanguageLable(w1[1]) + "\" href=\"javascript:document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');\" data-tooltip=\"" +
                                //context.GetLanguageLable(w1[1]) + "\"><img src=\"/images/duyet.png\"></a>&nbsp;&nbsp;";
                            break;
                        case "2":
                        case "0":
                            if (IsApproved) ActionButtonHtml = ActionButtonHtml +
                                UIDef.UIHrefButton(context.GetLanguageLable(w1[1]), "document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');", "fa fa-lock vang2", "", "bntAction" + i);
                                //UIDef.UIButton("bntAction" + i, context.GetLanguageLable(w1[1]), "document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');", " class=\"btn lock\"");
                                //"<a id=\"bntAction" + i + "\" class=\"cus-tooltip\" data-title=\"" +
                                //context.GetLanguageLable(w1[1]) + "\" href=\"javascript:document.DeleteForm.Type.value='" + w1[0] + "';onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');\" data-tooltip=\"" +
                                //context.GetLanguageLable(w1[1]) + "\"><img src=\"/images/an.png\"></a>&nbsp;&nbsp;";
                            break;
                        default:
                            if (IsApproved) ActionButtonHtml = ActionButtonHtml +
                                UIDef.UIHrefButton(context.GetLanguageLable(w1[1]), "document.DeleteForm.Type.value='" + w1[0] + "'; " + "onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');", "", "", "bntAction" + i);
                                //UIDef.UIButton("bntAction" + i, context.GetLanguageLable(w1[1]), "document.DeleteForm.Type.value='" + w1[0] + "'; " + "onDelete('bntAction" + i + "', 'ListForm', 'DeleteForm', '" + ColumnID + "', '" + (DeleteColumnName != "" ? "true" : "false") + "', '" + context.GetLanguageLable("Confirm" + w1[1]) + "');", " class=\"btn1 btnyellow\"");
                            break;
                    }
                }
            }
        }
        private void ExecPivotProc(string SPPivot, string QueryStringFilter, string ParamPivot, string ParamDefault, string QueryStringType,
            ref string ColumnName, ref string ColumnTextLable, ref string ColumnType, bool IsStoreCache = true)
        {
            string json = ""; int errorCode = 0; string errorString = ""; string parameterOutput = ""; dynamic d;
            string[] ColName = Tools.GetDataJson(BTab.DBConfig.Items[0], "ColumnNamePivot").Split(new string[] { "^" }, StringSplitOptions.None);
            string[] ColType = Tools.GetDataJson(BTab.DBConfig.Items[0], "ColumnTypePivot").Split(new string[] { "^" }, StringSplitOptions.None);
            Tools.ExecParam("PivotColumn", QueryStringFilter, ParamPivot, ParamDefault, QueryStringType, context,
                SPPivot, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, false, "", "", IsStoreCache);

            string ColNamePivot = ""; string ColLablePivot = ""; string ColTypePivot = "";
            if (errorCode == 200)
            {
                d = JObject.Parse(json);
                for (int i = 0; i < d.PivotColumn.Items.Count; i++)
                {
                    ColLablePivot = ColLablePivot + "^" +   Tools.GetDataJson(d.PivotColumn.Items[i], ColName[1]);
                    ColNamePivot = ColNamePivot + "^" +     Tools.GetDataJson(d.PivotColumn.Items[i], ColName[0]);
                    ColTypePivot = ColTypePivot + "^" + ColType[ColType.Length - 1];
                }
            }
            ColumnName = ColumnName.Replace("[COLUMNNAMEPIVOT]", ColNamePivot);
            ColumnTextLable = ColumnTextLable.Replace("[COLUMNLABLEPIVOT]", ColLablePivot);
            ColumnType = ColumnType.Replace("[COLUMNTYPEPIVOT]", ColTypePivot);
        }
        private string ExecuteData(string QueryStringFilter, string[] ParamListArray, string[] ParamDefaultArray, string QueryStringType, string[] StoreName, ref string parameterOutput,
            ref string ColumnName, ref string ColumnTextLable, ref string ColumnType,
            ref string json, ref int errorCode, ref string errorString, ref dynamic d, string FormType = "List", bool IsStoreCache = true)
        {
            string SPName = StoreName[0]; string ParamList = ParamListArray[0]; string ParamDefault = ParamDefaultArray[0];
            string Message = "";
            string FormTypeAction = context.GetRequestVal("FormTypeAction");
            string SPPivot =    Tools.GetDataJson(BTab.DBConfig.Items[0], "SPPivot");
            string ParamPivot = Tools.GetDataJson(BTab.DBConfig.Items[0], "ParamPivot");
            switch (FormType.ToLower())
            {
                case "pivot":
                    ExecPivotProc(SPPivot, QueryStringFilter, ParamPivot, ParamDefault, QueryStringType,
                        ref ColumnName, ref ColumnTextLable, ref ColumnType);
                    parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                    Tools.ExecParam("ListForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, context,
                        SPName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, false, "", "", IsStoreCache);
                    d = JObject.Parse(json);
                    break;
                case "edit":
                case "tree":
                case "directory":
                case "list":
                    Tools.ExecParam("ListForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, context, 
                        SPName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, false, "", "", IsStoreCache);
                    d = JObject.Parse(json);
                    break;
                case "program":
                    int cnt = StoreName.Length;
                    int iCnt = 0; try { iCnt = int.Parse(FormTypeAction); } catch {}
                    Message = "";
                    if (iCnt > 0 && cnt > 1)
                    {
                        SPName = StoreName[iCnt]; ParamList = ParamListArray[iCnt]; ParamDefault = ParamDefaultArray[iCnt];
                        string urlService = "BasetabExec";
                        int indexOf = SPName.ToLower().IndexOf("sp_mail");
                        if (indexOf > -1) urlService = "Mail";
                        ToolWeb toolWeb = new ToolWeb((context.IsHttps ? "https://" : "http://") + context.GetHost() + "/Service/" + urlService);
                        string Authorization = context.GetSession("CompanyCode") + 
                            "^" + context.GetSession("UserID") + 
                            "^" + MenuID + 
                            "^" + context.GetSession("Token");
                        json = Tools.ReadParam(context, QueryStringFilter, ParamList, ParamDefault, QueryStringType); 
                        if (json != "") json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                        string pzData = "{\"StoreName\":\"" + SPName + "\",\"StoreParam\":" + (json != "" ? json : "\"\"") + "}";
                        
                        toolWeb.WebRequestService(TabIndex, Authorization, pzData);
                        //Tools.ExecParam("ListForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, context,
                        //    SPName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, false, "", "", IsStoreCache);
                        //d = JObject.Parse(json);
                        //if (errorCode == 500)
                        //{
                        //    Message = "ERROR^" + errorString;
                        //}
                        //else
                        //{
                        //    dynamic d1 = JObject.Parse("{" + parameterOutput + "}");
                        //    //if ((long)d.ParameterOutput.ResponseStatus < 1)// thông báo lỗi và kết thúc
                        //    //{
                        //    Message = "ERROR^" + context.GetLanguageLable(d1.ParameterOutput.Message.ToString());
                        //    //}
                        //    //else
                        //    if ((long)d1.ParameterOutput.ResponseStatus > 0)
                        //    {
                        //        Message = "OK^" + context.GetLanguageLable(d1.ParameterOutput.Message.ToString());
                        //    //    if (SPPivot != "")
                        //    //    {
                        //    //        ExecPivotProc(SPPivot, QueryStringFilter, ParamPivot, ParamDefault, QueryStringType,
                        //    //            ref ColumnName, ref ColumnTextLable, ref ColumnType);
                        //    //    }
                        //    //    parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                        //    //    //SPName = StoreName[1];
                        //    //    SPName = StoreName[0]; ParamList = ParamListArray[0]; ParamDefault = ParamDefaultArray[0];
                        //    //    Tools.ExecParam("ListForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, context, SPName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        //    //    d = JObject.Parse(json);
                        //    }
                        //}
                    }
                    //else
                    //{
                        if (SPPivot != "")
                        {
                            ExecPivotProc(SPPivot, QueryStringFilter, ParamPivot, ParamDefault, QueryStringType,
                                ref ColumnName, ref ColumnTextLable, ref ColumnType, IsStoreCache);
                        }
                        parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                        SPName = StoreName[0]; ParamList = ParamListArray[0]; ParamDefault = ParamDefaultArray[0];
                        Tools.ExecParam("ListForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, context, SPName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, false, "", "", IsStoreCache);
                        d = JObject.Parse(json);
                    //}
                    break;
            }
            return Message;
        }
        public string UIListForm(bool IsInsert = true, bool IsUpdate = true, bool IsDelete = true)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            string Creator = context.GetSession("UserID");
            string ActionHtml = ""; string ActionButtonHtml = "";
            string ParamAction =                Tools.GetDataJson(BTab.DBConfig.Items[0], "ParamAction");// - Default "-1;Delete" "1;Active^0;Unactive^-1;Delete"; //
            string StoreGetPrintForm =          Tools.GetDataJson(BTab.DBConfig.Items[0], "StoreGetPrintForm");
            string StoreGetPrintFormOption =    Tools.GetDataJson(BTab.DBConfig.Items[0], "StoreGetPrintFormOption");
            string IndexTab =                   Tools.GetDataJson(BTab.DBConfig.Items[0], "DBConfigCode");
            string FormType =                   Tools.GetDataJson(BTab.DBConfig.Items[0], "FormType"); // List; Tree; Edit
            string ColumnID =                   Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumn");
            string QueryStringFilter =          Tools.GetDataJson(BTab.DBConfig.Items[0], "QueryStringFilter");
            string QueryStringType =            Tools.GetDataJson(BTab.DBConfig.Items[0], "QueryStringType");

            string ColumnFix =                  Tools.GetDataJson(BTab.DBConfig.Items[0], "ColumnFix");// Left^Right (2^1 => Tính bên Left cột thứ 2; Tính bên right cột thứ 1)
            string ColumnName =                 Tools.GetDataJson(BTab.DBConfig.Items[0], "ColumnName");
            string ColumnTextLable =            Tools.GetDataJson(BTab.DBConfig.Items[0], "ColumnTextLable");
            string ColumnType =                 Tools.GetDataJson(BTab.DBConfig.Items[0], "ColumnType");
            string UrlDbtab =                   Tools.GetDataJson(BTab.DBConfig.Items[0], "urlList"); UrlDbtab = context.ReplaceRequestValue(UrlDbtab);
            string UrlEdittab =                 Tools.GetDataJson(BTab.DBConfig.Items[0], "urlEdit"); UrlEdittab = context.ReplaceRequestValue(UrlEdittab);
            string DeleteColumnName =           Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumnName");

            string SPName =                     Tools.GetDataJson(BTab.DBConfig.Items[0], "SchemaName");
            string[] ParamListArray =           Tools.GetDataJson(BTab.DBConfig.Items[0], "paramList").Split(new string[] { "$" }, StringSplitOptions.None);
            string[] ParamDefaultArray =        Tools.GetDataJson(BTab.DBConfig.Items[0], "paramDefault").Split(new string[] { "$" }, StringSplitOptions.None); 
            string[] StoreName =                Tools.GetDataJson(BTab.DBConfig.Items[0], "spName").Split(new string[] { "$" }, StringSplitOptions.None);
            SPName = SPName.Trim();
            for (int iSP = 0; iSP < StoreName.Length; iSP++) if (SPName != "" && SPName != null) StoreName[iSP] = SPName + "." + StoreName[iSP];
            SPName = StoreName[0];
            //if (FormType == "Program")
            string JsonUrl = ""; int ColLenght = 0;
            UIActionButton(ParamAction, ColumnID, DeleteColumnName, ref ColLenght, ref ActionHtml, ref ActionButtonHtml, IsDelete);

            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d = null; string Message = ""; bool IsStoreCache = false;
            Message = ExecuteData(QueryStringFilter, ParamListArray, ParamDefaultArray, QueryStringType, StoreName, ref parameterOutput,
                ref ColumnName, ref ColumnTextLable, ref ColumnType,
                ref json, ref errorCode, ref errorString, ref d, FormType, IsStoreCache);
            string iframe  = context.GetRequestVal("iframe");

            r1.Append("<form target=\"saveTranFrm" + iframe + "\" onsubmit=\"return false\" name=\"DeleteForm\" method=post>");
            r1.Append(UIDef.UIHidden("Type", "-1") + 
                UIDef.UIHidden("TabIndex", TabIndex)+
                UIDef.UIHidden("MenuOn", MenuOn) +
                UIDef.UIHidden("iframe", iframe) +
                UIDef.UIHidden("GetUrl", context.GetUrl()) +
                UIDef.UIHidden("GetQueryString", context.GetQueryString()));
            r1.Append(UIDef.UIHidden(ColumnID, ""));
            r1.Append(UIDef.UIHidden("SysUID", ""));
            r1.Append("</form>");
            r1.Append("<form target=\"saveTranFrm" + iframe + "\" onsubmit=\"return false\" name=\"ListForm\" method=post>" +
                UIDef.UIHidden("MenuOn", MenuOn) +
                UIDef.UIHidden("iframe", iframe) +
                UIDef.UIHidden("MenuID", MenuID) +
                UIDef.UIHidden("GetUrl", context.GetUrl()) +
                UIDef.UIHidden("GetQueryString", context.GetQueryString()) +
                UIDef.UIHidden("TabIndex", TabIndex));

            // bang danh sach
            ColumnTextLable = ColumnTextLable.Replace("[Action]", ActionHtml);
            string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] b; b = ColumnTextLable.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] c; c = ColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataVal; DataVal = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataTxt; DataTxt = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataParent; DataParent = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            r1.Append(UIDef.UIHidden("SysUID", context.enc.Encrypt(a[1] + "||" + "||" + context.GetSession("UserID"))));
            //int iRow = 0;
            
            switch (FormType.ToLower())
            {
                case "directory": // Staff Directoty; Org Directory
                    
                    string oldParent = "";
                    for (int i = 0; i < d.ListForm.Items.Count; i++) 
                    {
                        string ClassName = Tools.GetDataJson(d.ListForm.Items[i], "ClassName");
                        string parent = Tools.GetDataJson(d.ListForm.Items[i], "ParentID");
                        if (parent != oldParent)
                        {
                            if (oldParent != "")
                            {
                                r1.Append(Environment.NewLine + "<!-- /" + oldParent + i + " -->" +
                                    Environment.NewLine + "</div>");
                            }
                            r1.Append(Environment.NewLine + "<div class=\"row tree-ns " + ClassName + "\">" + //(iRow == 0? "": (iRow == 1 ? " this" : " border"))
                                Environment.NewLine + "<!-- " + oldParent + i + " -->");
                            //iRow = iRow + 1;
                        }
                        oldParent = parent;
                        r1.Append(Environment.NewLine + "<div class=\"col-sm-3\">" +
                            Environment.NewLine + "<!-- col-sm-3" + oldParent + i + " -->" +
                            Environment.NewLine + "<div class=\"card\">" +
                            Environment.NewLine + "<!-- card" + oldParent + i + " -->");
                        for (int j = 0; j < b.Length; j++)
                        {
                            if (c[j].ToLower() == "attachimage")
                            {
                                r1.Append(Environment.NewLine + "<div class=\"card-img-top\">" +
                                    Environment.NewLine + "<img src=\"/Media/RenderFile?ImageId=" + Tools.GetDataJson(d.ListForm.Items[i], a[j]) + "&NoCrop=\">" +
                                    Environment.NewLine + "</div>");
                            }
                        }
                        r1.Append(Environment.NewLine + "<div class=\"card-body\">" +
                            Environment.NewLine + "<!-- card-body" + oldParent + i + " -->");
                        for (int j = 0; j < b.Length; j++)
                        {
                            if (c[j].ToLower() == "title")
                            {
                                r1.Append(Environment.NewLine + "<h5 class=\"card-title\">" + Tools.GetDataJson(d.ListForm.Items[i], a[j]) + "</h5>");
                            }
                            else
                            {
                                string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                                string val = "";
                                try { val = Tools.GetDataJson(d.ListForm.Items[i], a[j]); } catch { val = ""; }
                                if (val == null) val = "";
                                r1.Append(Environment.NewLine + "<p class=\"card-text\">" +
                                    Environment.NewLine + "<span>" + context.GetLanguageLable(b[i]) + ":</span>" +
                                    Environment.NewLine + "<span>");
                                switch (a4[0])
                                {
                                    case "HREF":
                                        if (a4.Length > 3)
                                        {
                                            r1.Append("<a href=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ",'" + a4[3] + "');\">" + val + "</a>");
                                        }
                                        else
                                        {
                                            r1.Append("<a href=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\">" + val + "</a>");
                                        }
                                        break;
                                    case "Date":
                                        DateTime val1;
                                        try
                                        {
                                            val1 = DateTime.Parse(val);
                                            r1.Append(Tools.HRMDate(val1));
                                        }
                                        catch
                                        {
                                            //val1 = DateTime.Now;
                                            r1.Append("");// + Tools.HRMDate(val1));
                                        }
                                        break;
                                    case "Datetime":
                                        DateTime val2;
                                        try
                                        {
                                            val2 = DateTime.Parse(val);
                                            r1.Append(Tools.HRMDateTime(val2));
                                        }
                                        catch
                                        {
                                            //val2 = DateTime.Now;
                                            r1.Append("");// + Tools.HRMDateTime(val2));
                                        }
                                        break;
                                    case "Time":
                                        DateTime val3;
                                        try
                                        {
                                            val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                            r1.Append(Tools.HRMTime(val3));
                                        }
                                        catch
                                        {
                                            val3 = DateTime.Now;
                                            r1.Append("");
                                        }
                                        break;
                                    case "Numeric":
                                        if (val == null) val = "0";
                                        int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                                        r1.Append(Tools.FormatNumber(val, iRound));
                                        break;
                                    case "":
                                        r1.Append(val);
                                        break;
                                }
                                r1.Append(Environment.NewLine + "</span>" +
                                    Environment.NewLine + "</p>");
                            }
                        }
                        r1.Append(Environment.NewLine + "<!-- /card-body" + oldParent + i + " -->" +
                            Environment.NewLine + "</div>" +
                            (ClassName.ToLower() != "this" && Tools.GetDataJson(d.ListForm.Items[i], "Cnt", "int") > 0 ? " <a class=\"collapse-switch\"></a>" : "") + 
                            Environment.NewLine + "<!-- /card" + oldParent + i + " -->" + 
                            Environment.NewLine + "</div>" +
                            Environment.NewLine + "<!-- /col-sm-3" + oldParent + i + " -->" +
                            Environment.NewLine + "</div>");
                    }
                    break;
                case "tree":
                    // nut lenh
                    if (IsInsert || ActionButtonHtml != "")
                    {
                        r1.Append(Environment.NewLine + "<div class=\"titlebox titlebox-fix\">" +
                            Environment.NewLine + "<div class=\"nut\">");
                        if (IsInsert)
                        {
                            r1.Append(Environment.NewLine + "<div class=\"cantrai\">");
                            r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("AddNewChild"), "onAddnew(ParentID);"));
                            r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("AddNewWithThisParent"), "onAddnew(ParentID);"));
                            r1.Append(Environment.NewLine + "</div>");
                        }
                        if (ActionButtonHtml != "")
                        {
                            r1.Append(Environment.NewLine + "<div class=\"canphai\">" + ActionButtonHtml + 
                                Environment.NewLine + "</div>");
                        }
                        r1.Append(Environment.NewLine + "</div>" +
                            Environment.NewLine + "</div>");
                    }

                    r1.Append(Environment.NewLine + "<table id=\"dbtablist\" width=100% class=\"table treetinhvan table-hover use-icon\">" +
                        Environment.NewLine + "<thead class=\"thead-light\">" +
                        Environment.NewLine + "<tr>");
                    for (int i = 0; i < b.Length; i++)
                    {
                        string[] a4 = c[i].Split(new string[] { ";" }, StringSplitOptions.None);
                        if (a4[0] != "-") r1.Append(Environment.NewLine + "<th>" + context.GetLanguageLable(b[i]) + "</th>");
                    }
                    r1.Append("</tr></thead>");
                    r1.Append("<tbody>");
                    for (int i = 0; i < d.ListForm.Items.Count; i++)
                    {
                        r1.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "\" data-tt-id=\"" + Tools.GetDataJson(d.ListForm.Items[i], "OrderBy") + "\" " + (Tools.GetDataJson(d.ListForm.Items[i], "ParentID", "bigint") > 0 ? "data-tt-parent-id=\"" + Tools.GetDataJson(d.ListForm.Items[i], "ParentOrderBy") + "\"" : ""));
                        string HrefTd = "";
                        for (int j = 0; j < b.Length; j++)
                        {
                            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            if (a4[0].ToUpper() == "HREF" && HrefTd == "")
                            {
                                if (a4.Length > 3)
                                {
                                    HrefTd = " style=\"cursor: pointer;\" onclick=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ",'" + a4[3] + "');\" ";
                                }
                                else
                                {
                                    HrefTd = " style=\"cursor: pointer;\" onclick=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\" ";
                                }
                            }
                        }
                        r1.Append(">");
                        string SysU = "";
                        if (FormType != "Pivot") Creator = Tools.GetDataJson(d.ListForm.Items[i], "CreatorID");
                        SysU = a[1] + i + "||" + Tools.GetDataJson(d.ListForm.Items[i], a[1]) + "||" + Creator;
                        string disable = context.DisableByRole(Creator, Tools.GetDataJson(d.ListForm.Items[i], a[1]));
                        r1.Append(UIDef.UIHidden("SysUID" + i, context.enc.Encrypt(SysU)));
                        int chkUrl = 0; bool IsSpaned = false;
                        for (int j = 0; j < b.Length; j++)
                        {
                            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            string val = "";
                            try { val = Tools.GetDataJson(d.ListForm.Items[i], a[j]); } catch { val = ""; }
                            if (val == null) val = "";
                            switch (a4[0])
                            {
                                case "HREF":
                                    if (chkUrl == 0)
                                    {
                                        chkUrl = 1;
                                        string UrlName = ""; if ((j + 1) < b.Length) { UrlName = Tools.GetDataJson(d.ListForm.Items[i], a[j + 1]); }
                                        JsonUrl = JsonUrl + ", {\"UrlName\":\"" + UrlName + "\", \"UrlLable\":\"" + val + "\", \"Url\":\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\"}";
                                    }
                                    if (a4.Length > 3)
                                    {
                                        r1.Append("<td " + HrefTd + "><a href=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ",'" + a4[3] + "');\">" + val + "</a>");
                                    }
                                    else
                                    {
                                        r1.Append("<td " + HrefTd + "><a href=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\">" + val + "</a>");
                                    }
                                    break;
                                case "Date":
                                    DateTime val1;
                                    try
                                    {
                                        val1 = DateTime.Parse(val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMDate(val1));
                                    }
                                    catch
                                    {
                                        //val1 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">");// + Tools.HRMDate(val1));
                                    }
                                    break;
                                case "Datetime":
                                    DateTime val2;
                                    try
                                    {
                                        val2 = DateTime.Parse(val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMDateTime(val2));
                                    }
                                    catch
                                    {
                                        //val2 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">");// + Tools.HRMDateTime(val2));
                                    }
                                    break;
                                case "Time":
                                    DateTime val3;
                                    try
                                    {
                                        val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMTime(val3));
                                    }
                                    catch
                                    {
                                        val3 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMTime(val3));
                                    }
                                    break;
                                case "Numeric":
                                    if (val == null) val = "0";
                                    int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                                    r1.Append("<td class=\"text-right\" " + HrefTd + ">" + Tools.FormatNumber(val, iRound));
                                    break;
                                case "Checkbox":
                                    string strChk = "1";
                                    if (a4.Length > 4) if (a4[4] != "") { if (val != "") strChk = val; val = a4[4]; }
                                    r1.Append("<td>" + UIDef.UICheckbox(a[j] + i, "", val, strChk, disable + " onclick=\"if(this.checked==true)markline(" + i + ", true, false); else markline(" + i + ", false, false);\""));
                                    break;
                                case "":
                                    if (IsSpaned)
                                        r1.Append("<td " + HrefTd + ">" + val);
                                    else
                                        r1.Append("<td><span class='" + (Tools.GetDataJson(d.ListForm.Items[i], "Cnt", "int") > 0 ? (Tools.GetDataJson(d.ListForm.Items[i], "Cnt", "int") > 0 ? "folder" : "file uses") : "file") + "'>" + val + "</span>");
                                    IsSpaned = true;
                                    break;
                            }
                        }
                    }
                    r1.Append("</tbody>");
                    r1.Append("</table>"); 
                    r1.Append(UIDef.UIHidden("ItemsCnt", d.ListForm.Items.Count.ToString()));

                    // JS ------------------------------
                    r1.Append(Environment.NewLine + "<script>");
                    r1.Append(Environment.NewLine + "$(\"#dbtablist\").treetable({ expandable: true" +
                    Environment.NewLine + "});");
                    ////r1.Append(Environment.NewLine + "// ContextMenu");
                    ////r1.Append(Environment.NewLine + "$(function(){ $('#dbtablist').contextMenu({ 	selector: 'tr',  	callback: function(key, options) { 		var m = \"clicked: \" + key + \" on \" + $(this).text(); 		window.console && console.log(m) || alert(m);  	}, 	items: { 		\"edit\": {name: \"Edit\", icon: \"edit\"}, 		\"cut\": {name: \"Cut\", icon: \"cut\"}, 		\"copy\": {name: \"Copy\", icon: \"copy\"}, 		\"paste\": {name: \"Paste\", icon: \"paste\"}, 		\"delete\": {name: \"Delete\", icon: \"delete\"}, 		\"sep1\": \"-------- - \", 		\"quit\": {name: \"Quit\", icon: function($element, key, item){ return 'context-menu-icon context-menu-icon-quit'; }} 	} });});");
                    r1.Append(Environment.NewLine + "// Highlight selected row");
                    r1.Append(Environment.NewLine + "$(\"#dbtablist tbody\").on(\"mousedown\", \"tr\", function() { $(\".selected\").not(this).removeClass(\"selected\"); $(this).toggleClass(\"selected\"); });");
                    r1.Append(Environment.NewLine + "// Drag & Drop Example Code");
                    r1.Append(Environment.NewLine + "$(\"#dbtablist .sFile, #dbtablist .folder\").draggable({ helper: \"clone\", opacity: .75, refreshPositions: true, revert: \"invalid\", revertDuration: 300, scroll: true });");
                    r1.Append(Environment.NewLine + "$(\"#dbtablist .folder\").each(function() { $(this).parents(\"#dbtablist tr\").droppable({ accept: \".sFile, .folder\", drop: function(e, ui) { var droppedEl = ui.draggable.parents(\"tr\");  $(\"#dbtablist\").treetable(\"move\", droppedEl.data(\"ttId\"), $(this).data(\"ttId\"), '" + IndexTab + "'); }, hoverClass: \"accept\", over: function(e, ui) { var droppedEl = ui.draggable.parents(\"tr\"); if(this != droppedEl[0] && !$(this).is(\".expanded\")) { $(\"#dbtablist\").treetable(\"expandNode\", $(this).data(\"ttId\")); } } }); });");
                    r1.Append("</script>");
                    //-----------------------------------

                    break;
                case "program":
                case "pivot":
                    // nut lenh
                    if (IsInsert || ActionButtonHtml != "")
                    {
                        r1.Append(Environment.NewLine + "<div class=\"titlebox titlebox-fix\">" +
                            Environment.NewLine + "<div class=\"nut\">");
                        if (IsInsert)
                        {
                            r1.Append(Environment.NewLine + "<div class=\"cantrai\">");
                            r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("AddNew"), "onAddnew();"));
                            r1.Append(Environment.NewLine + "</div>");
                        }
                        if (ActionButtonHtml != "")
                        {
                            r1.Append(Environment.NewLine + "<div class=\"canphai\">" + ActionButtonHtml +
                                Environment.NewLine + "</div>");
                        }
                        r1.Append(Environment.NewLine + "</div>" +
                            Environment.NewLine + "</div>");
                    }
                    string[] aCol = null; bool IsFixCol = false;
                    if (ColumnFix != "")
                    {
                        aCol = ColumnFix.Split(new string[] { "^" }, StringSplitOptions.None);
                        IsFixCol = (aCol.Length > 1);
                    }
                    if (IsFixCol)
                        r1.Append(Environment.NewLine + "<table width=100% class=\"stripe\" id=\"dbtablist\">" +
                            //Environment.NewLine + "<col width=\"40px\">" +
                            //Environment.NewLine + "<col width=\"200px\">" +
                            Environment.NewLine + "<thead>" +
                            Environment.NewLine + "<tr>");
                    else
                        r1.Append(Environment.NewLine + "<table width=100% class=\"table table-hover table-border flex\" id=\"dbtablist\">" +
                            //Environment.NewLine + "<col width=\"40px\">" +
                            //Environment.NewLine + "<col width=\"200px\">" +
                            Environment.NewLine + "<thead>" +
                            Environment.NewLine + "<tr>");
                    for (int i = 0; i < b.Length; i++)
                    {
                        string[] a4 = c[i].Split(new string[] { ";" }, StringSplitOptions.None);
                        if (a4[0] != "-") r1.Append(Environment.NewLine + "<th>" + context.GetLanguageLable(b[i]) + "</th>");
                    }
                    r1.Append(Environment.NewLine + "</tr>" +
                        Environment.NewLine + "</thead>" +
                        Environment.NewLine + "<tbody>");
                    for (int i = 0; i < d.ListForm.Items.Count; i++)
                    {
                        r1.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "\"");
                        string HrefTd = "";
                        for (int j = 0; j < b.Length; j++)
                        {
                            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            if (a4[0].ToUpper() == "HREF" && HrefTd == "")
                            {
                                if (a4.Length > 3)
                                {
                                    HrefTd = " style=\"cursor: pointer;\" onclick=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ",'" + a4[3] + "');\" ";
                                }
                                else
                                {
                                    HrefTd = " style=\"cursor: pointer;\" onclick=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\" ";
                                }
                            }
                        }
                        r1.Append(">");
                        string SysU = "";
                        if (FormType != "Pivot") Creator = Tools.GetDataJson(d.ListForm.Items[i], "CreatorID");
                        SysU = a[1] + i + "||" + Tools.GetDataJson(d.ListForm.Items[i], a[1]) + "||" + Creator;
                        string disable = context.DisableByRole(Creator, Tools.GetDataJson(d.ListForm.Items[i], a[1]));
                        r1.Append(UIDef.UIHidden("SysUID" + i, context.enc.Encrypt(SysU)));
                        int chkUrl = 0;
                        for (int j = 0; j < b.Length; j++)
                        {
                            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            string val = "";
                            try { val = Tools.GetDataJson(d.ListForm.Items[i], a[j]); } catch { val = ""; }
                            if (val == null) val = "";
                            switch (a4[0])
                            {
                                case "HREF":
                                    if (chkUrl == 0)
                                    {
                                        chkUrl = 1;
                                        string UrlName = ""; if ((j + 1) < b.Length) { UrlName = Tools.GetDataJson(d.ListForm.Items[i], a[j + 1]); }
                                        JsonUrl = JsonUrl + ", {\"UrlName\":\"" + UrlName + "\", \"UrlLable\":\"" + val + "\", \"Url\":\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\"}";
                                    }
                                    if (a4.Length > 3)
                                    {
                                        r1.Append("<td " + HrefTd + "><a href=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ",'" + a4[3] + "');\">" + val + "</a>");
                                    }
                                    else
                                    {
                                        r1.Append("<td " + HrefTd + "><a href=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\">" + val + "</a>");
                                    }
                                    break;
                                case "Date":
                                    DateTime val1;
                                    try
                                    {
                                        val1 = DateTime.Parse(val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMDate(val1));
                                    }
                                    catch
                                    {
                                        //val1 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">");// + Tools.HRMDate(val1));
                                    }
                                    break;
                                case "Datetime":
                                    DateTime val2;
                                    try
                                    {
                                        val2 = DateTime.Parse(val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMDateTime(val2));
                                    }
                                    catch
                                    {
                                        //val2 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">");// + Tools.HRMDateTime(val2));
                                    }
                                    break;
                                case "Time":
                                    DateTime val3;
                                    try
                                    {
                                        val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMTime(val3));
                                    }
                                    catch
                                    {
                                        val3 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMTime(val3));
                                    }
                                    break;
                                case "Numeric":
                                    if (val == null) val = "0";
                                    int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                                    r1.Append("<td class=\"text-right\" " + HrefTd + ">" + Tools.FormatNumber(val, iRound));
                                    break;
                                case "Checkbox":
                                    string strChk = "1";
                                    if (a4.Length > 4) if (a4[4] != "") { if (val != "") strChk = val; val = a4[4]; }
                                    r1.Append("<td>" + UIDef.UICheckbox(a[j] + i, "", val, strChk, disable + " onclick=\"if(this.checked==true)markline(" + i + ", true, false); else markline(" + i + ", false, false);\""));
                                    break;
                                case "":
                                    r1.Append("<td " + HrefTd + ">" + val);
                                    break;
                            }
                        }
                    }
                    r1.Append(Environment.NewLine + "</tbody>" +
                        Environment.NewLine + "</table>");
                    if (IsFixCol)
                    {
                        r1.Append(Environment.NewLine + "<script>" +
                            Environment.NewLine + "setTimeout(function(){var table = $('#dbtablist').dataTable(" +
                            "{scrollY: '', scrollX: true, scrollCollapse: true, " +
                            "paging: false, ordering: false, info: false, " +
                            Environment.NewLine + "fixedColumns: {leftColumns: " + aCol[0] + ", rightColumns: " + aCol[1] + "}, " +
                            Environment.NewLine + "\"initComplete\": function(settings, json) {" +
                            Environment.NewLine + "$(this).find(\"tr\").hover(function() {" +
                            Environment.NewLine + "var indextr = $(this).index() + 2;" +
                            Environment.NewLine + "var closestTable = $(this).closest('.dataTables_wrapper');" +
                            Environment.NewLine + "closestTable.find(\".DTFC_LeftWrapper tr\").removeClass('hover').eq(indextr).addClass('hover');" +
                            Environment.NewLine + "closestTable.find(\".DTFC_RightWrapper tr\").removeClass('hover').eq(indextr).addClass('hover');}).click(function() {" +
                            Environment.NewLine + "var indextr = $(this).index() + 2;" +
                            Environment.NewLine + "var closestTable = $(this).closest('.dataTables_wrapper');" +
                            Environment.NewLine + "closestTable.find(\".DTFC_LeftWrapper tr\").removeClass('selected').eq(indextr).addClass('selected');" +
                            Environment.NewLine + "closestTable.find(\".DTFC_RightWrapper tr\").removeClass('selected').eq(indextr).addClass('selected');})}});" +
                            Environment.NewLine + "},200);</script>");
                    }
                    r1.Append(UIDef.UIHidden("ItemsCnt", d.ListForm.Items.Count.ToString()));
                    break;
                case "list":
                    // nut lenh
                    if (IsInsert || ActionButtonHtml != "")
                    {
                        r1.Append(Environment.NewLine + "<div class=\"titlebox titlebox-fix\">" +
                            Environment.NewLine + "<div class=\"nut\">");
                        if (IsInsert)
                        {
                            r1.Append(Environment.NewLine + "<div class=\"cantrai\">");
                            r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("AddNew"), "onAddnew();"));
                            r1.Append(Environment.NewLine + "</div>");
                        }
                        if (ActionButtonHtml != "")
                        {
                            r1.Append(Environment.NewLine + "<div class=\"canphai\">" + ActionButtonHtml +
                                Environment.NewLine + "</div>");
                        }
                        r1.Append(Environment.NewLine + "</div>" +
                            Environment.NewLine + "</div>");
                    }
                    aCol = null; IsFixCol = false;
                    if (ColumnFix != "")
                    {
                        aCol = ColumnFix.Split(new string[] { "^" }, StringSplitOptions.None);
                        IsFixCol = (aCol.Length > 1);
                    }
                    if (IsFixCol)
                        r1.Append(Environment.NewLine + "<table width=100% class=\"stripe\" id=\"dbtablist\">" +
                            //Environment.NewLine + "<col width=\"40px\">" +
                            //Environment.NewLine + "<col width=\"200px\">" +
                            Environment.NewLine + "<thead>" +
                            Environment.NewLine + "<tr>"); 
                    else
                        r1.Append(Environment.NewLine + "<table width=100% class=\"table table-hover table-border flex\" id=\"dbtablist\">" +
                            //Environment.NewLine + "<col width=\"40px\">" +
                            //Environment.NewLine + "<col width=\"200px\">" +
                            Environment.NewLine + "<thead>" +
                            Environment.NewLine + "<tr>");
                    for (int i = 0; i < b.Length; i++)
                    {
                        string[] a4 = c[i].Split(new string[] { ";" }, StringSplitOptions.None);
                        if (a4[0] != "-") r1.Append(Environment.NewLine + "<th>" + context.GetLanguageLable(b[i]) + "</th>");
                    }
                    r1.Append(Environment.NewLine + "</tr>" +
                        Environment.NewLine + "</thead>" +
                        Environment.NewLine + "<tbody>");
                    for (int i = 0; i < d.ListForm.Items.Count; i++)
                    {
                        r1.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "\"");
                        string HrefTd = "";
                        for (int j = 0; j < b.Length; j++)
                        {
                            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            if (a4[0].ToUpper() == "HREF" && HrefTd == "")
                            {
                                if (a4.Length > 3)
                                {
                                    HrefTd = " style=\"cursor: pointer;\" onclick=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ",'" + a4[3] + "');\" ";
                                }
                                else
                                {
                                    HrefTd = " style=\"cursor: pointer;\" onclick=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\" ";
                                }
                            }
                        }
                        r1.Append(">");
                        string SysU = "";
                        if (FormType != "Pivot") Creator = Tools.GetDataJson(d.ListForm.Items[i], "CreatorID");
                        SysU = a[1] + i + "||" + Tools.GetDataJson(d.ListForm.Items[i], a[1]) + "||" + Creator;
                        string disable = context.DisableByRole(Creator, Tools.GetDataJson(d.ListForm.Items[i], a[1]));
                        r1.Append(UIDef.UIHidden("SysUID" + i, context.enc.Encrypt(SysU)));
                        int chkUrl = 0;
                        for (int j = 0; j < b.Length; j++)
                        {
                            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            string val = "";
                            try { val = Tools.GetDataJson(d.ListForm.Items[i], a[j]); } catch { val = ""; }
                            if (val == null) val = "";
                            switch (a4[0])
                            {
                                case "HREF":
                                    if (chkUrl == 0)
                                    {
                                        chkUrl = 1;
                                        string UrlName = ""; if ((j + 1) < b.Length) { UrlName = Tools.GetDataJson(d.ListForm.Items[i], a[j+1]); }
                                        JsonUrl = JsonUrl + ", {\"UrlName\":\"" + UrlName + "\", \"UrlLable\":\"" + val + "\", \"Url\":\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\"}"; 
                                    }                                    
                                    if (a4.Length > 3)
                                    {
                                        r1.Append("<td " + HrefTd + "><a href=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ",'" + a4[3] + "');\">" + val + "</a>");
                                    }
                                    else
                                    {
                                        r1.Append("<td " + HrefTd + "><a href=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\">" + val + "</a>");
                                    }                                    
                                    break;
                                case "Date":
                                    DateTime val1;
                                    try
                                    {
                                        val1 = DateTime.Parse(val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMDate(val1));
                                    }
                                    catch
                                    {
                                        //val1 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">");// + Tools.HRMDate(val1));
                                    }
                                    break;
                                case "Datetime":
                                    DateTime val2;
                                    try
                                    {
                                        val2 = DateTime.Parse(val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMDateTime(val2));
                                    }
                                    catch
                                    {
                                        //val2 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">");// + Tools.HRMDateTime(val2));
                                    }
                                    break;
                                case "Time":
                                    DateTime val3;
                                    try
                                    {
                                        val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMTime(val3));
                                    }
                                    catch
                                    {
                                        val3 = DateTime.Now;
                                        r1.Append("<td class=\"text-center\" " + HrefTd + ">" + Tools.HRMTime(val3));
                                    }
                                    break;
                                case "Numeric":
                                    if (val == null) val = "0";
                                    int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                                    r1.Append("<td class=\"text-right\" " + HrefTd + ">" + Tools.FormatNumber(val, iRound));
                                    break;
                                case "Checkbox":
                                    string strChk = "1";
                                    if (a4.Length > 4) if (a4[4] != "") { if (val != "") strChk = val; val = a4[4]; }
                                    r1.Append("<td>" + UIDef.UICheckbox(a[j] + i, "", val, strChk, disable + " onclick=\"if(this.checked==true)markline(" + i + ", true, false); else markline(" + i + ", false, false);\""));
                                    break;
                                case "":
                                    r1.Append("<td " + HrefTd + ">" + val);
                                    break;
                            }
                        }
                    }
                    r1.Append(Environment.NewLine + "</tbody>" +
                        Environment.NewLine + "</table>");
                    if (IsFixCol)
                    {
                        r1.Append(Environment.NewLine + "<script>" +
                            Environment.NewLine + "setTimeout(function(){var table = $('#dbtablist').dataTable(" +
                            "{scrollY: '', scrollX: true, scrollCollapse: true, " +
                            "paging: false, ordering: false, info: false, " +
                            Environment.NewLine + "fixedColumns: {leftColumns: " + aCol[0] + ", rightColumns: " + aCol[1] + "}, " +
                            Environment.NewLine + "\"initComplete\": function(settings, json) {" +
                            Environment.NewLine + "$(this).find(\"tr\").hover(function() {" +
                            Environment.NewLine + "var indextr = $(this).index() + 2;" +
                            Environment.NewLine + "var closestTable = $(this).closest('.dataTables_wrapper');" +
                            Environment.NewLine + "closestTable.find(\".DTFC_LeftWrapper tr\").removeClass('hover').eq(indextr).addClass('hover');" +
                            Environment.NewLine + "closestTable.find(\".DTFC_RightWrapper tr\").removeClass('hover').eq(indextr).addClass('hover');}).click(function() {" +
                            Environment.NewLine + "var indextr = $(this).index() + 2;" +
                            Environment.NewLine + "var closestTable = $(this).closest('.dataTables_wrapper');" +
                            Environment.NewLine + "closestTable.find(\".DTFC_LeftWrapper tr\").removeClass('selected').eq(indextr).addClass('selected');" +
                            Environment.NewLine + "closestTable.find(\".DTFC_RightWrapper tr\").removeClass('selected').eq(indextr).addClass('selected');})}});" +
                            Environment.NewLine + "},200);</script>");
                    }                        
                    r1.Append(UIDef.UIHidden("ItemsCnt", d.ListForm.Items.Count.ToString()));
                    break;
                case "edit":
                    // nut lenh
                    if (IsInsert || ActionButtonHtml != "" || IsUpdate)
                    {
                        r1.Append(Environment.NewLine + "<div class=\"titlebox titlebox-fix\">" +
                            Environment.NewLine + "<div class=\"nut\">");
                        if (IsInsert || IsUpdate)
                        {
                            r1.Append(Environment.NewLine + "<div class=\"cantrai\">");
                            if (IsInsert) r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("AddNew"), (IsInsert ? "insertTabRow('dbtablist');" : "")));
                            r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Save"), (IsInsert || IsUpdate ? "onUpdateRow(document.ListForm);" : ""), "fa fa-floppy-o xanhla"));
                            r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Refesh"), "onReload();", "tvc-lammoi vang2"));
                            r1.Append(Environment.NewLine + "</div>");
                        }
                        if (ActionButtonHtml != "")
                        {
                            r1.Append(Environment.NewLine + "<div class=\"canphai\">" + ActionButtonHtml +
                                Environment.NewLine + "</div>");
                        }
                        r1.Append(Environment.NewLine + "</div>" +
                            Environment.NewLine + "</div>");
                    }
                    
                    r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                        Environment.NewLine + "var ColumnName = new Array();" +
                        Environment.NewLine + "var ColumnLable = new Array();" +
                        Environment.NewLine + "var ColumnType = new Array();" +
                        Environment.NewLine + "var ColumnRequire = new Array();" +
                        Environment.NewLine + "var ColumnData = new Array();" +
                        Environment.NewLine + "var DataParent = new Array();" +
                        Environment.NewLine + "var DataTxt = new Array();" +
                        Environment.NewLine + "var DataVal = new Array();</script>");
                    r1.Append("<table width=\"100%\" class=\"table table-hover table-border flex\" id=\"dbtablist\">");
                    r1.Append("<thead class=\"thead-light\"><tr>");
                    int totalCol = 0;
                    for (int i = 0; i < b.Length; i++)
                    {
                        string mandantoryLabel = "";
                        DataVal[i] = ""; DataTxt[i] = ""; DataParent[i] = "";
                        string[] a4 = c[i].Split(new string[] { ";" }, StringSplitOptions.None);

                        if (i < ColLenght + b.Length)
                        {
                            r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                                Environment.NewLine + "ColumnRequire[" + i + "] = '_';" +
                                Environment.NewLine + "ColumnName[" + i + "] = '" + a[i] + "';" +
                                Environment.NewLine + "ColumnType[" + i + "] = '" + c[i] + "';" +
                                Environment.NewLine + "ColumnLable[" + i + "] = '" + context.GetLanguageLable(b[i]) + "';</script>");
                            if (a4.Length > 2)
                            {
                                if (Tools.CIntNull(a4[2]) == 1)
                                {
                                    r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                                        Environment.NewLine + "ColumnRequire[" + i + "] = '" + a[i] + "';</script>");
                                    mandantoryLabel = " <font color='red'>*</font>";//(*)
                                }
                            }
                        }

                        if (a4[0] != "-" && a4[0] != "Hidden") { r1.Append("<th>" + context.GetLanguageLable(b[i]) + mandantoryLabel + "</th>"); totalCol++; }
                        string s = ""; string val = ""; string v = ""; string ReqJson = "";
                        if (a4.Length > 1) v = a4[1];
                        if(Tools.Left(v, 7) == "REQUEST") v = context.GetRequestVal(v.Substring(7));
                        if (val == "" && v != "") val = v;
                        switch (a4[0])
                        {
                            case "AutoNumber":
                                string inputCheck = c[i];
                                s = UIDef.SelectStrOption(c[i], context.GetLanguageLable("AutoNumber"), ref inputCheck, true);
                                break;
                            case "Radio":
                                s = UIDef.SelectStrOption(a4[3], a4[4], ref val, true);
                                break;
                            case "Actb":
                                ReqJson = context.InputDataSetParam(a4[6]);
                                UIDef.OptionStringVal(ref DataDAO, a4[5], ReqJson, a4[7], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);                                
                                s = UIDef.SelectStrOption(DataVal[i], DataTxt[i], ref val, true);
                                break;
                            case "ActbText":
                                s = UIDef.SelectStrOption(a4[5], a4[6], ref val, true);
                                break;
                            case "Selectbox":
                                ReqJson = context.InputDataSetParam(a4[4]);
                                UIDef.OptionStringVal(ref DataDAO, a4[3], ReqJson, a4[5], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                                s = UIDef.SelectStrOption(DataVal[i], DataTxt[i], ref val, true);
                                break;
                            case "SelectboxText":
                                s = UIDef.SelectStrOption(a4[3], a4[4], ref val, true);
                                break;
                        }
                        r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                            Environment.NewLine + "ColumnData[" + i + "] = '" + s + "'" +
                            Environment.NewLine + "DataParent[" + i + "] = '" + DataParent[i] + "'" +
                            Environment.NewLine + "DataTxt[" + i + "] = '" + DataTxt[i] + "'" +
                            Environment.NewLine + "DataVal[" + i + "] = '" + DataVal[i] + "'" +
                            Environment.NewLine + "</script>");
                    }
                    r1.Append("</tr></thead>");
                    r1.Append("<tbody>"); //totalCol
                                          //if (d.ListForm.Items.Count < 0)
                                          //{
                    r1.Append("<tr><td colspan=" + totalCol +
                        " style=\"height:0px !important\"></td></tr>");
                    //}
                    for (int i = 0; i < d.ListForm.Items.Count; i++)
                    {
                        r1.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "\">");
                        string HrefTd = "";
                        for (int j = 0; j < b.Length; j++)
                        {
                            string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            if (a4[0].ToUpper() == "HREF" && HrefTd == "")
                            {
                                if (a4.Length > 3)
                                {
                                    HrefTd = " style=\"cursor: pointer;\" onclick=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ",'" + a4[3] + "');\" ";
                                }
                                else
                                {
                                    HrefTd = " style=\"cursor: pointer;\" onclick=\"" + a4[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a4[2])]) + ");\" ";
                                }
                            }
                        }
                        r1.Append(UIDef.UIHidden("Changed" + i, "0"));
                        string SysU = a[1] + i + "||" + Tools.GetDataJson(d.ListForm.Items[i], a[1]) + "||" + Tools.GetDataJson(d.ListForm.Items[i], "CreatorID");
                        string disable = context.DisableByRole(Tools.GetDataJson(d.ListForm.Items[i], "CreatorID"), Tools.GetDataJson(d.ListForm.Items[i], a[1]));
                        r1.Append(UIDef.UIHidden("SysUID" + i, context.enc.Encrypt(SysU)));
                        for (int j = 0; j < b.Length; j++)
                        {
                            string[] a31 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            string val = "";
                            
                            try { val = Tools.GetDataJson(d.ListForm.Items[i], a[j]); } catch { val = ""; } if (val == null) val = "";
                            string v = "";
                            if(a31.Length>1) v = a31[1];
                            //if (v.Length > 7) if (v.Substring(0, 7) == "REQUEST") v = context.GetRequestVal(v.Substring(7));
                            if (Tools.Left(v, 7) == "REQUEST") v = context.GetRequestVal(v.Substring(7));
                            if (val == "" && v != "") val = v;
                            if (a31[0] == "-")
                                r1.Append(UIDef.UIHidden(a[j] + i, val));
                            else
                            {
                                string ReqJson = ""; //string funcOnchange = "";
                                switch (a31[0])
                                {
                                    case "HREF":
                                        r1.Append("<td " + HrefTd + ">");
                                        string UrlName = ""; if ((j + 1) < b.Length) { UrlName = Tools.GetDataJson(d.ListForm.Items[i], a[j+1]); }
                                        JsonUrl = JsonUrl + ", {\"UrlName\":\"" + UrlName + "\", \"UrlLable\":\"" + val + "\", \"Url\":\"" + a31[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a31[2])]) + ");\"}";
                                        r1.Append(UIDef.UIHidden(a[j] + i, val));
                                        r1.Append("<a href=\"" + a31[1] + "(" + Tools.GetDataJson(d.ListForm.Items[i], a[j - int.Parse(a31[2])]) + ");\">" + val + "</a>");
                                        break;
                                    //a31[2]: Option check Require
                                    case "Textarea":
                                        r1.Append("<td>");
                                        r1.Append(UIDef.UITextarea(context.GetLanguageLable(b[j]), a[j] + i, val, " " + disable + " autocomplete=\"off\" cols=\"" + a31[3] + "\" rows=\"" + a31[4] + "\" onchange=\"markline(" + i + ", true, true);\"", "", ""));
                                        break;
                                    case "Password":
                                        r1.Append("<td>");
                                        //UIPassword(string InputName, string Param, string NextFocus = "")
                                        r1.Append(UIDef.UIPassword(a[j] + i, disable + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", " onchange=\"markline(" + i + ", true, true);\" "));
                                        break;
                                    case "Textbox":
                                        r1.Append("<td>");
                                        if (a31.Length > 5)
                                            r1.Append(UIDef.UITextbox(context.GetLanguageLable(b[j]), a[j] + i, val, " " + disable + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" onchange=\"markline(" + i + ", true, true);\" ", ""/*UIDef.FocusAfter(a[j])*/, a31[5]));
                                        else
                                            r1.Append(UIDef.UITextbox(context.GetLanguageLable(b[j]), a[j] + i, val, " " + disable + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" onchange=\"markline(" + i + ", true, true);\"", ""/*UIDef.FocusAfter(a[j])*/));
                                        break;
                                    case "Numeric":
                                        r1.Append("<td>");
                                        if (a31.Length > 5)
                                            r1.Append(UIDef.UINumeric(context.GetLanguageLable(b[j]), a[j] + i, val, " " + disable + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", ""/*UIDef.FocusAfter(a[j])*/, "markline(" + i + ", true, true);" + a31[5]));
                                        else
                                            r1.Append(UIDef.UINumeric(context.GetLanguageLable(b[j]), a[j] + i, val, " " + disable + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", ""/*UIDef.FocusAfter(a[j])*/, "markline(" + i + ", true, true);"));
                                        break;
                                    case "Date":
                                        r1.Append("<td>");
                                        val = Tools.HRMDate(val);
                                        r1.Append(UIDef.UIDate(context.GetLanguageLable(b[j]), a[j] + i, val, " autocomplete=\"off\" " + disable, UIDef.CNextFocusAction(a[j]) + " markline(" + i + ", true, true);", "ListForm"));
                                        break;
                                    case "Datetime":
                                        r1.Append("<td>");
                                        val = Tools.HRMDateTime(val);
                                        r1.Append(UIDef.UIDateTime(context.GetLanguageLable(b[j]), a[j] + i, val, " autocomplete=\"off\" " + disable, UIDef.CNextFocusAction(a[j]) + " markline(" + i + ", true, true);", "ListForm"));
                                        break;
                                    case "Time":
                                        r1.Append("<td>");
                                        r1.Append(UIDef.UITime(context.GetLanguageLable(b[j]), a[j] + i, val, " autocomplete=\"off\" " + disable, UIDef.CNextFocusAction(a[j]) + " markline(" + i + ", true, true);", "ListForm"));
                                        break;
                                    case "Checkbox":
                                        r1.Append("<td>");
                                        string strChk = "1";
                                        if (a31.Length > 4) if (a31[4] != "") { if (val != "") strChk = val; val = a31[4]; }
                                        //if (a31.Length > 4 && val == "") val = a31[4];
                                        r1.Append(UIDef.UICheckbox(a[j] + i, "", val, strChk, " " + disable + ""/*UIDef.FocusAfter(a[j])*/ + " onclick=\"markline(" + i + ", true, true);\""));//if(this.checked==true)markline(" + i + ", true, true); else markline(" + i + ", false, false);
                                        break;
                                    //case "MutilCheckbox":
                                    //    ReqJson = context.InputDataSetParam(a31[4]);
                                    //    r1.Append(UIDef.UIDD(a[j], context, ref DataDAO, a31[3], ReqJson, a31[5], val, ""/*UIDef.FocusAfter(a[j])*/ + " markline(" + i + ", true, true);", "", "", int.Parse(a31[6]), int.Parse(a31[7])));
                                    //    break;
                                    case "Radio":
                                        r1.Append("<td>");
                                        //r1.Append(UIDef.UIRadio(a[j], a31[3], a31[4], val, ""/*UIDef.FocusAfter(a[j])*/ + " markline(" + i + ", true, true);"));
                                        r1.Append(UIDef.UISelectStr(a[j] + i, a31[3], a31[4], ref val, true, " " + disable, ""/*UIDef.CNextFocus(a[j])*/ + " markline(" + i + ", true, true);", "document.ListForm"));
                                        break;
                                    case "Actb":
                                        r1.Append("<td>");
                                        ReqJson = context.InputDataSetParam(a31[6]);
                                        r1.Append(UIDef.UISelectStr(a[j] + i, DataVal[j], DataTxt[j], ref val, true, " " + disable, ""/*UIDef.CNextFocus(a[j])*/ + " markline(" + i + ", true, true);", "document.ListForm"));
                                        //r1.Append(UIDef.UISelect(a[j], ref DataDAO, a31[5], ReqJson, a31[7], val, true, "", ""/*UIDef.CNextFocus(a[j])*/ + " markline(" + i + ", true, true);"));
                                        //r1.Append(UIDef.UIActb(a[j], ref DataDAO, a31[5], ReqJson, a31[7], val, " size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", ""/*UIDef.FocusAfter(a[j])*/ + " markline(" + i + ", true, true);", "ListForm"));
                                        break;
                                    case "ActbText":
                                        r1.Append("<td>");
                                        r1.Append(UIDef.UISelectStr(a[j] + i, a31[5], a31[6], ref val, true, " " + disable, ""/*UIDef.CNextFocus(a[j])*/ + " markline(" + i + ", true, true);", "document.ListForm"));
                                        //r1.Append(UIDef.UIActbStr(a[j], a31[5], a31[6], val, " size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", ""/*UIDef.FocusAfter(a[j])*/ + " markline(" + i + ", true, true);", "ListForm"));
                                        break;
                                    case "Selectbox":
                                        r1.Append("<td>");
                                        ReqJson = context.InputDataSetParam(a31[4]);
                                        //r1.Append(UIDef.UISelect(a[j], ref DataDAO, a31[3], ReqJson, a31[5], val, true, "", ""/*UIDef.CNextFocus(a[j])*/ + " markline(" + i + ", true, true);"));
                                        r1.Append(UIDef.UISelectStr(a[j] + i, DataVal[j], DataTxt[j], ref val, true, " " + disable, ""/*UIDef.CNextFocus(a[j])*/ + " markline(" + i + ", true, true);", "document.ListForm"));
                                        break;
                                    case "SelectboxText":
                                        r1.Append("<td>");
                                        r1.Append(UIDef.UISelectStr(a[j] + i, a31[3], a31[4], ref val, true, " " + disable, ""/*UIDef.CNextFocus(a[j])*/ + " markline(" + i + ", true, true);", "document.ListForm"));
                                        break;
                                    case "AutoNumber":
                                        r1.Append("<td>");
                                        r1.Append(UIDef.UISelectStr(a[j] + i, val, val, ref val, true, " " + disable, ""/*UIDef.CNextFocus(a[j])*/ + " markline(" + i + ", true, true);", "document.ListForm"));
                                        break;
                                    default:
                                        r1.Append("<td " + HrefTd + ">");
                                        r1.Append(val);
                                        break;
                                }
                            }                            
                        }
                    }
                    r1.Append("</tbody>");
                    r1.Append("</table>");
                    r1.Append(UIDef.UIHidden("ItemsCnt", d.ListForm.Items.Count.ToString()));
                    break;
            }
            if (JsonUrl != "") { JsonUrl = Tools.RemoveFisrtChar(JsonUrl); context.SetSession("BTabUrlList_" + IndexTab, "{\"UrlList\":[" + JsonUrl + "]}"); }

            r1.Append("</form>");

            int CurrPage = int.Parse((context.GetRequestVal("Page") != ""? context.GetRequestVal("Page"):"1"));
            int PageSize = int.Parse((context.GetRequestVal("PageSize") != "" ? context.GetRequestVal("PageSize") : context.GetSession("PageSizeBaseTab")));
            int RowsCount = 1;
            
            try { dynamic ad = JObject.Parse("{" + parameterOutput + "}"); RowsCount = int.Parse(ad.ParameterOutput.Rowcount.ToString()); } catch { RowsCount = 1; } 

            if (FormType.ToLower() != "directory" && FormType.ToLower() != "tree") r1.Append(UIPageTable(context, CurrPage, PageSize, RowsCount, "document.FilterForm"));
            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            string[] q;
            if (Message != "")
            {
                q = Message.Split(new string[] { "^" }, StringSplitOptions.None);
                if (q[0] == "OK")
                {
                    r1.Append(Environment.NewLine + "JsAlert('alert-success', '" + context.GetLanguageLable("Alert-Success") + "', '" + q[1] + "', '', '0');");
                }
                else if (q[0] == "ERROR")
                {
                    r1.Append(Environment.NewLine + "JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + q[1] + "', '', '0');");
                }
            }

            r1.Append(Environment.NewLine + "var mySubmit1=true;" +
                    Environment.NewLine + "function Saving(a){" +
                    Environment.NewLine + "if(a){" +
                    Environment.NewLine + "document.getElementById('saving" + iframe + "').style.display='block';" +
                    Environment.NewLine + "if(document.getElementById('bntAddnew'))document.getElementById('bntAddnew').disabled=true;" +
                    Environment.NewLine + "if(document.getElementById('bntUpdate'))document.getElementById('bntUpdate').disabled=true;" +
                    Environment.NewLine + "mySubmit1=false;" +
                    Environment.NewLine + "}else{" +
                    Environment.NewLine + "document.getElementById('saving" + iframe + "').style.display='none';" +
                    Environment.NewLine + "if(document.getElementById('bntAddnew'))document.getElementById('bntAddnew').disabled=false;" +
                    Environment.NewLine + "if(document.getElementById('bntUpdate'))document.getElementById('bntUpdate').disabled=false;" +
                    Environment.NewLine + "mySubmit1=true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "}" +
                    //Environment.NewLine + "var ColumnName = '" + ColumnName + "';" +
                    //Environment.NewLine + "var ColumnType = '" + ColumnType + "';" +
                    Environment.NewLine + "var linebackgrounds = new Array(" + d.ListForm.Items.Count.ToString() + ");" +
                    Environment.NewLine + "var iCheck=false;");

            r1.Append(Environment.NewLine + "function markline(kline, IsMark, IsEditLst){" +
                    Environment.NewLine + "var frm = document.ListForm;" +
                    Environment.NewLine + "var a = document.getElementById(\"trrowid\" + kline);" +
                    Environment.NewLine + "if(IsEditLst==true){" +
                    Environment.NewLine + "   var d = frm.elements['Changed' + kline];" +
                    Environment.NewLine + "   if(d){" +
                    Environment.NewLine + "       d.value=1;iCheck=true;" +
                    Environment.NewLine + "   }" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "if(IsMark) {" +
                    Environment.NewLine + "/*linebackgrounds[kline]=a.style.background;a.style.background=\"#99aaff\";*/a.className = 'selected';" +
                    Environment.NewLine + "} else " +
                    Environment.NewLine + "/*a.style.background=linebackgrounds[kline];*/a.className = '';	" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "ActionKeypress = " + (FormType=="Edit"?"1":"0") + ";");

            q = QueryStringType.Split(new string[] { "^" }, StringSplitOptions.None);
            string qStr = "";
            for (int i = 0; i < q.Length; i++)
            {
                qStr = qStr + "&" + q[i];
            }
            r1.Append(Environment.NewLine + "function onConfirmDelete (fLst, fDelete, IsActionForm){" +
                    Environment.NewLine + "var s = '-1';" +
                    Environment.NewLine + "var frmLst = document.forms[fLst];" +
                    Environment.NewLine + "var frmDelete = document.forms[fDelete];" +
                    Environment.NewLine + "var s0 = '-1';" +
                    Environment.NewLine + "var qs = '';" +
                    Environment.NewLine + "var frmFillter = document.FilterForm;" +
                    Environment.NewLine + "var arr = '" + QueryStringFilter + "'.split('^');" +
                    Environment.NewLine + "for(var i=0; i<arr.length; i++ ){" +
                    Environment.NewLine + "if(frmFillter.elements[arr[i]])" +
                    Environment.NewLine + "if(frmFillter.elements[arr[i]].value != '')" +
                    Environment.NewLine + "qs = qs + '&' + arr[i] + '=' + frmFillter.elements[arr[i]].value;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "var cnt = parseInt(frmLst.ItemsCnt.value);" +
                    Environment.NewLine + "if (cnt>0){" +
                    Environment.NewLine + "for(var i=0; i<cnt; i++ ){" +
                    Environment.NewLine + "var d = frmLst.elements['" + ColumnID + "' + i];" +
                    Environment.NewLine + "var d1 = frmLst.elements['SysUID' + i];" +
                    Environment.NewLine + "if(d){" +
                    Environment.NewLine + "     if(!(d.value == \"\" || d.disabled==true || d.checked == false)) {" +
                    Environment.NewLine + "         s=s+','+d.value;" +
                    Environment.NewLine + "         s0 = s0 +','+d1.value;" +
                    Environment.NewLine + "     }" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "frmDelete.elements['" + ColumnID + "'].value=s;" +
                    Environment.NewLine + "frmDelete.elements['SysUID'].value=s0;" +
                    Environment.NewLine + "frmDelete.action='/Basetab/Delete';" +
                    Environment.NewLine + "if(frmDelete.Type.value == '-1' || !IsActionForm){" +
                    Environment.NewLine + "Saving(true);frmDelete.submit();" +
                    Environment.NewLine + "} else {" +
                    Environment.NewLine + "if (document.getElementById('viewActiondiv'))viewActionquit();" +
                    Environment.NewLine + "var /*b= document.getElementById(obj);" +
                    Environment.NewLine + "if (!b) */b = document.getElementById('chkAll');" +
                    Environment.NewLine + "var a = createDivTop0('300px', '600px', 'viewActiondiv');" + //GetQueryString GetUrl
                    Environment.NewLine + "document.body.appendChild(a);" +
                    Environment.NewLine + "ajPage('/Basetab/DbAction', viewAction_ret, 'TabIndex=" + TabIndex + "&" + ColumnID + "=' + s + '&SysUID=' + encodeURI(s0) + '&Type=' + frmDelete.Type.value + '&GetUrl=' + frmDelete.GetUrl.value + '&GetQueryString=' + frmDelete.GetQueryString.value + qs);}" +
                    //Environment.NewLine + "}" +
                    //Environment.NewLine + "}" +
                    Environment.NewLine + "}");
            r1.Append(Environment.NewLine + "function onDelete (obj, frmLst, frmDelete, InputName, IsActionForm, txt){" +
                    Environment.NewLine + "if(ChkCheck(frmLst, InputName)){" +
                    Environment.NewLine + "var ActionLable = '';" +
                    Environment.NewLine + "if(txt) ActionLable = txt;" +
                    Environment.NewLine + "else if(document.forms[frmDelete].Type.value=='-1') ActionLable = '" + context.GetLanguageLable("ConfirmDelete") + "';" +
                    Environment.NewLine + "else if(document.forms[frmDelete].Type.value=='0') ActionLable = '" + context.GetLanguageLable("ConfirmUnactive") + "';" +
                    Environment.NewLine + "else if(document.forms[frmDelete].Type.value=='1') ActionLable = '" + context.GetLanguageLable("ConfirmActive") + "';" +
                    Environment.NewLine + "JsConfirm('" + context.GetLanguageLable("Confirm") + "', ActionLable, '" + context.GetLanguageLable("OK") + "', '" + context.GetLanguageLable("Cancle") + "', 'onConfirmDelete (\\'' + frmLst + '\\', \\'' + frmDelete + '\\', ' + IsActionForm +')');" +
                    Environment.NewLine + "}}");

            if (StoreGetPrintForm != "") r1.Append(Environment.NewLine + "function execPrint(obj, frmLst, InputName) {" +
                    Environment.NewLine + "    if(!ChkCheck(frmLst, InputName)) return;" +
                    Environment.NewLine + "    var frm = document.forms[frmLst];" +
                    Environment.NewLine + "    var cnt = parseInt(frm.ItemsCnt.value);" +
                    Environment.NewLine + "    var s = '';" +
                    Environment.NewLine + "    for(var i=0; i<cnt; i++ ){" +
                    Environment.NewLine + "        var d = frm.elements['" + ColumnID + "' + i];" +
                    Environment.NewLine + "        if(d){" +
                    Environment.NewLine + "            if(!(d.value == \"\" || d.disabled==true || d.checked == false)) {" +
                    Environment.NewLine + "                s=s+','+d.value;" +
                    Environment.NewLine + "            }" +
                    Environment.NewLine + "        }" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    if (s != '') s = s.substring(1, s.length);" +
                    Environment.NewLine + "    window.location.href = \"/Basetab/Print?TabIndex=" + TabIndex + "&ColumnID=" + ColumnID + "&" + ColumnID + "=\" + s;" +
                    Environment.NewLine + "}");

            if (StoreGetPrintFormOption != "") r1.Append(Environment.NewLine + "function showPrint(obj, frmLst, InputName) {" +
                    Environment.NewLine + "    if(!ChkCheck(frmLst, InputName)) return;" +
                    Environment.NewLine + "    var frm = document.forms[frmLst];" +
                    Environment.NewLine + "    var cnt = parseInt(frm.ItemsCnt.value);" +
                    Environment.NewLine + "    var s = '';" +
                    Environment.NewLine + "    for(var i=0; i<cnt; i++ ){" +
                    Environment.NewLine + "        var d = frm.elements['" + ColumnID + "' + i];" +
                    Environment.NewLine + "        if(d){" +
                    Environment.NewLine + "            if(!(d.value == \"\" || d.disabled==true || d.checked == false)) {" +
                    Environment.NewLine + "                s=s+','+d.value;" +
                    Environment.NewLine + "            }" +
                    Environment.NewLine + "        }" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    if (s != '') s = s.substring(1, s.length);" +
                    Environment.NewLine + "    var helpregion = document.getElementById('PrintForm'); " +
                    Environment.NewLine + "    if (helpregion) return; " +
                    Environment.NewLine + "    var a = JsPopupInfo(obj, 'PrintForm', '" + context.GetLanguageLable("Print") + "'); " +
                    Environment.NewLine + "    ajPage('/Basetab/PrintForm?TabIndex=" + TabIndex +
                        "&ColumnID=" + ColumnID +
                        "&" + ColumnID + "=' + s, PrintFormReturn);" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function PrintFormReturn(http) {" +
                    Environment.NewLine + "    var a = document.getElementById('idContentPrintForm');" +
                    Environment.NewLine + "    var rethtml = http.responseText;" +
                    Environment.NewLine + "    var jssepr = rethtml.indexOf('<!--JS-->');" +
                    Environment.NewLine + "    if (jssepr > 0) {" +
                    Environment.NewLine + "        a.innerHTML = rethtml.substring(0, jssepr);" +
                    Environment.NewLine + "        // Append javascript code!" +
                    Environment.NewLine + "        var newScript = document.createElement('script');" +
                    Environment.NewLine + "        newScript.text = rethtml.substring(jssepr + 9, rethtml.length);" +
                    Environment.NewLine + "        a.appendChild(newScript);" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    else {" +
                    Environment.NewLine + "        a.innerHTML = rethtml;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "}");

            r1.Append(Environment.NewLine + "function viewAction_ret(http){" +
                    Environment.NewLine + "document.getElementById('viewActiondiv').innerHTML=\"<input type='button' class='btn' value='Close' onClick='viewActionquit()'>\"; ExecJs(document.getElementById('viewActiondiv'), http.responseText);}");

            r1.Append(Environment.NewLine + "function viewActionquit() {" +
                    Environment.NewLine + "document.body.removeChild(document.getElementById('viewActiondiv'));}");

            r1.Append(Environment.NewLine + "function onAddnew(){" +
                    Environment.NewLine + "location.href=\"" + UrlEdittab + "-1" +
                    "&GetQueryString=\" + document.ListForm.GetQueryString.value + \"" +
                    "&GetUrl=\" + document.ListForm.GetUrl.value + \"" +
                    "&MenuOn=" + context.GetRequestMenuOn() + "\"" +
                    Environment.NewLine + "}");

            r1.Append(Environment.NewLine + "function onEdit(Id){" +
                    Environment.NewLine + "location.href=\"" + UrlEdittab + "\" + Id + \"" +
                    "&GetQueryString=\" + document.ListForm.GetQueryString.value + \"" +
                    "&GetUrl=\" + document.ListForm.GetUrl.value + \"" +
                    "&MenuOn=" + context.GetRequestMenuOn() + "\";" +
                    Environment.NewLine + "}");

            r1.Append(Environment.NewLine + "function onEditOther(Id, Url) {" +
                    Environment.NewLine + "_attw = window.open(Url + Id, " +
                    Environment.NewLine + "'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=100, left=200, width=650, height=400');" +
                    Environment.NewLine + "}");

            r1.Append(Environment.NewLine + "function onReload(){" +
                    Environment.NewLine + "location.href=\"" + Compress.UnZip(context.GetUrl()) + "?" + Compress.UnZip(context.GetQueryString()) + "\";" +
                    Environment.NewLine + "}");

            r1.Append(Environment.NewLine + "function onUpdateRow(frm){" +
                    Environment.NewLine + "if(!iCheck){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("YouAreNotEnterData") + "', '', '0');" +//alert('" + context.GetLanguageLable("YouAreNotEnterData") + "');
                    Environment.NewLine + "return;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "var cnt = parseInt(frm.ItemsCnt.value);" +
                    Environment.NewLine + "var msg = '';" +
                    Environment.NewLine + "var inputFocus;" +
                    Environment.NewLine + "for (var i = 0; i < cnt; i++){" +
                    Environment.NewLine + "for (var j = 0; j < ColumnRequire.length; j++){" +
                    Environment.NewLine + "var inputName = ColumnRequire[j] + i;" +
                    Environment.NewLine + "var input = frm.elements[inputName];" +
                    Environment.NewLine + "if (input){" +
                    Environment.NewLine + "var IsReq = true;" +
                    Environment.NewLine + "if (input.type == \"select-one\" || input.type == 'select-multiple'){" +
                    Environment.NewLine + "/*for (var l = 0; l < input.options.length; l++){" +
                    Environment.NewLine + "if(input.options[l].selected == true) IsReq = false;" +
                    Environment.NewLine + "}*/if (input.selectedIndex >= 0 && input.options[input.selectedIndex].value != '' && input.options[input.selectedIndex].value != '-1' && input.options[input.selectedIndex].text != '')IsReq = false;" +
                    Environment.NewLine + "} else if (input.type == \"checkbox\"){" +
                    Environment.NewLine + "if(input.checked == true) " +
                    Environment.NewLine + "IsReq = false;} else {" +
                    Environment.NewLine + "if(input.value != '') " +
                    Environment.NewLine + "IsReq = false;}" +
                    Environment.NewLine + "if (IsReq) {" +
                    Environment.NewLine + "msg=msg+'" + context.GetLanguageLable("UpdateRow") + " ' + (i+1) + ' " + context.GetLanguageLable("Column") + " [' + ColumnLable[j] + '] " + context.GetLanguageLable("IsNull") + "!<br>';" +
                    Environment.NewLine + "if (!inputFocus) inputFocus = input;}}}}" +
                    Environment.NewLine + "if (msg != ''){" +
                    Environment.NewLine + "if (!inputFocus) input.focus(); " +
                    Environment.NewLine + "else inputFocus.focus();" +
                    Environment.NewLine + "JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', msg, '', '0');" +//alert (msg);
                    Environment.NewLine + "return;} " +
                    Environment.NewLine + "Saving(true); frm.action = \"/Basetab/SaveDBTable\"; " +
                    Environment.NewLine + "frm.submit();" +
                    Environment.NewLine + "}");

            r1.Append(Environment.NewLine + "function ChkCheck(f, InputName){" +
                    Environment.NewLine + "var a=0;var frm = document.forms[f]" +
                    Environment.NewLine + "var cnt = parseInt(frm.ItemsCnt.value);" +
                    Environment.NewLine + "if (cnt>0){" +
                    Environment.NewLine + "for(var i=0; i<cnt; i++ ){" +
                    Environment.NewLine + "var d = frm.elements[InputName + i];" +
                    Environment.NewLine + "if(d){" +
                    Environment.NewLine + "if(!(d.value == \"\" || d.disabled==true || d.checked == false)) a=a+1;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "if(!(a>0)){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("YouAreNotChoiceData") + "', '', '0'); " +//alert('" + context.GetLanguageLable("YouAreNotChoiceData") + "');
                    Environment.NewLine + "return false;" +
                    Environment.NewLine + "}else " +
                    Environment.NewLine + "return true;" +
                    Environment.NewLine + "}");

            r1.Append(Environment.NewLine + "function checkAll(chk, frm, InputName){" +
                    Environment.NewLine + "var cnt = parseInt(frm.ItemsCnt.value);" +
                    Environment.NewLine + "if (cnt>0){" +
                    Environment.NewLine + "for(var i=0; i<cnt; i++ ){" +
                    Environment.NewLine + "var d = frm.elements[InputName + i];" +
                    Environment.NewLine + "if(d){" +
                    Environment.NewLine + "if(!(d.value == \"\" || d.disabled==true)){d.checked = chk.checked; markline(i, chk.checked);}" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "}}");

            r1.Append(Environment.NewLine + "</script>");

            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIListFormLeft()
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            if (context.LeftFormEdit == "1")
            {
                dynamic BTabUrlList = null;
                string UrlEdittab = Tools.GetDataJson(BTab.DBConfig.Items[0], "urlEdit");
                string IndexTab =   Tools.GetDataJson(BTab.DBConfig.Items[0], "DBConfigCode");
                string UrlList = context.GetSession("BTabUrlList_" + IndexTab);

                UrlEdittab = context.ReplaceRequestValue(UrlEdittab);              
                if (UrlList != null && UrlList != "") BTabUrlList = JObject.Parse(UrlList);
                if (BTabUrlList != null)
                {
                    r1.Append(Environment.NewLine + "<div class=\"col-left\">" +
                        Environment.NewLine + "<ul class=\"tabvertical\">");
                    for (int i = 0; i < BTabUrlList.UrlList.Count; i++)
                    {
                        r1.Append(Environment.NewLine + "<li class=\"tablinks\" onclick=\"" + BTabUrlList.UrlList[i].Url + ";\">" + BTabUrlList.UrlList[i].UrlLable + "</li>"); //active
                    }
                    r1.Append(Environment.NewLine + "</ul></div>");
                    r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                        Environment.NewLine + "function onEdit(Id){" +
                        Environment.NewLine + "location.href=\"" + UrlEdittab + "\" + Id + \"&GetUrl=" + context.GetUrl() + "&GetQueryString=" + context.GetQueryString() + "\";}" +
                        Environment.NewLine + "</script>");
                }
            }
            r = r1.ToString();
            r1 = null;
            return r;
        }
        public string UIEditFormHeader(int Total, string SysU, string SysV, string LeftMenu = "", bool IsEdit = false, bool IsInsert = true, bool IsUpdate = true, bool IsDelete = true)
        {
            string StoreGetPrintFormOption = Tools.GetDataJson(BTab.DBConfig.Items[0], "StoreGetPrintFormOption");
            string StoreGetPrintForm =       Tools.GetDataJson(BTab.DBConfig.Items[0], "StoreGetPrintForm");
            string ColumnID =                Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumn"); 
            //string StoreGetData =          Tools.GetDataJson(BTab.DBConfig.Items[0], "StoreGetPrintFormOption");
            //context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType);
            //int Total = 1;
            //for (int i = 0; i < BTabGrp.DBConfigGrp.Items.Count; i++)
            //{
            //    string[] a1; a1 = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "EditColumn").Split(new string[] { "^" }, StringSplitOptions.None);
            //    Total = Total + a1.Length;
            //}
            StringBuilder r1 = new StringBuilder(); string r = "";
            string IdCol = context.GetRequestVal(context.GetRequestVal("IdCol")); if (IdCol == "") IdCol = "-1";
            r1.Append(Environment.NewLine + "<div class=\"titlebox titlebox-fix\">" + // div titlebox titlebox-fix open
                Environment.NewLine + "<h4 class=\"titlebox-title\">" + context.GetLanguageLable(TabIndex) +
                Environment.NewLine + "<a id=\"aObj\" href=\"javascript:showhelp('aObj', '" + TabIndex + "', 2)\" class=\"question\">" +
                Environment.NewLine + "<i class=\"fa fa-question-circle-o\" aria-hidden=\"true\"></i>" +
                Environment.NewLine + "</a>" +
                Environment.NewLine + "<div class=\"canphai\">");
            if (IsEdit)
            {
                //r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("AccountID") + ": " + SysU, "void(0);", "fa fa-vcard-o"));
                //r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Version") + ": " + SysV, "void(0);", "fa fa-code-fork"));
                //r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("History"), "view_T_Hist('" + TabIndex + "'," + IdCol + ",-1);", "fa fa-history"));
                r1.Append(UIDef.UIHrefButton(SysU, "void(0);", ""));
                r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Version") + " " + SysV, "void(0);", ""));
                r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("History"), "view_T_Hist('" + TabIndex + "'," + IdCol + ",-1);", ""));
            }
            r1.Append(Environment.NewLine + "</div></h4></div>");// div titlebox titlebox-fix close

            r1.Append(Environment.NewLine + "<!--row-->" + 
                Environment.NewLine + "<div class=\"row\">"); // div row open

            // Check xem form có Left menu ko?
            if (LeftMenu != "") // Hàm left menu sẽ lấy 1 số cột khai báo; + list menu child
            {
                r1.Append(Environment.NewLine + "<div class=\"use-menu col-sm\">" + // div use-menu col-sm open
                    Environment.NewLine + "<!-- Rotating card -->" +
                    Environment.NewLine + "<div class=\"card-wrapper\">" +
                    Environment.NewLine + "<div id=\"card-1\" class=\"card card-rotating text-center\">" +
                    Environment.NewLine + "<!-- Front Side -->" +
                    LeftMenu +
                    Environment.NewLine + "<!-- Front Side End -->" +
                    Environment.NewLine + "</div></div></div>" + // div use-menu col-sm close
                    Environment.NewLine + "<!-- col-sm-10 use-content -->" +
                    Environment.NewLine + "<div class=\"col-sm-10 use-content\">"); // div col-sm-10 use-content open
            }
            else
            {
                r1.Append(Environment.NewLine + "<!-- col-sm-10 use-content -->" + 
                    Environment.NewLine + "<div class=\"col-sm\">"); // div col-sm-10 use-content open
            }
            // Nút lệnh (Ghi; Copy; Xóa; Back)
            r1.Append(Environment.NewLine + "<div class=\"titlebox titlebox-fix\">" + // open titlebox titlebox-fix
                Environment.NewLine + "<div class=\"nut\">" +
                Environment.NewLine + "<div class=\"cantrai\">");
            // Copy
            if (IsEdit && IsInsert)
            {
                r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("SaveAs"), "mySubmit(document.bosfrm, 1);", "fa fa-clone xanhla", Total.ToString(), "_copysavebtn"));
                Total = Total + 1;
            }
            // Save
            if (IsEdit && IsUpdate || !IsEdit && IsInsert)
            {
                r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Save"), "mySubmit(document.bosfrm, 0);", "fa fa-floppy-o xanhla", Total.ToString(), "_editsavebtn"));
                Total = Total + 1;
            }
            // Delete
            if (IsEdit && IsDelete)
            {
                r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Delete"), "myDelete('document.DeleteForm');", "fa fa-trash-o do", Total.ToString(), "_deletebtn"));
                Total = Total + 1;
            }
            // In with option
            if (StoreGetPrintFormOption != "" && IsEdit) {
                r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("PrintOption"), "showPrint('bntAction999');", "fa fa-print xanh", Total.ToString(), "bntAction999"));
                Total = Total + 1;
            }
            // In with default
            if (StoreGetPrintForm != "" && IsEdit) {
                r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Print"), "execPrint();", "fa fa-print xanh", Total.ToString()));
                Total = Total + 1;
            }
            // Back
            r1.Append(UIDef.UIHrefButton(context.GetLanguageLable("Back"), "myBack();", "tvc-undo xanhla", Total.ToString()) +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "</div>" + // close titlebox titlebox-fix
                Environment.NewLine + UIDef.UIHidden("CopyData", "0"));

            r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                    Environment.NewLine + "function showhelpret(http) {" +
                    Environment.NewLine + "    var helpregion = document.getElementById('idContenthelpdisp');" +
                    Environment.NewLine + "    helpregion.style.display = 'block';" +
                    Environment.NewLine + "    helpregion.innerHTML = http.responseText;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function showhelp(obj, C, stage) {" +
                    Environment.NewLine + "var helpregion = document.getElementById('helpdisp'); " +
                    Environment.NewLine + "if (helpregion) return;" +
                    Environment.NewLine + "var a = JsPopupInfo(obj, 'helpdisp', '" + context.GetLanguageLable("Help") + "'); " +
                    Environment.NewLine + "ajPage('/Basetab/BasetabText?id=' + C + '&stage=' + stage, showhelpret);" +
                    //Environment.NewLine + "}" +
                    Environment.NewLine + "}");
            if (StoreGetPrintForm != "") r1.Append(Environment.NewLine + "function execPrint() {" +
                    Environment.NewLine + "    window.location.href = \"/Basetab/Print?TabIndex=" + TabIndex + "&ColumnID=" + ColumnID + "&" + ColumnID + "=" + context.GetRequestVal(context.GetRequestVal("IdCol")) + "\";" +
                    Environment.NewLine + "}");
            if (StoreGetPrintFormOption != "") r1.Append(Environment.NewLine + "function showPrint(obj) {" +
                    Environment.NewLine + "var helpregion = document.getElementById('PrintForm'); " +
                    Environment.NewLine + "if (helpregion) return; " +
                    Environment.NewLine + "var a = JsPopupInfo(obj, 'PrintForm', '" + context.GetLanguageLable("Print") + "'); " +
                    Environment.NewLine + "ajPage('/Basetab/PrintForm?TabIndex=" + TabIndex +
                        "&ColumnID=" + ColumnID + 
                        "&" + ColumnID + "=" + context.GetRequestVal(context.GetRequestVal("IdCol")) + "', PrintFormReturn);" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function PrintFormReturn(http) {" +
                    Environment.NewLine + "    var a = document.getElementById('idContentPrintForm');" +
                    Environment.NewLine + "    var rethtml = http.responseText;" +
                    Environment.NewLine + "    var jssepr = rethtml.indexOf('<!--JS-->');" +
                    Environment.NewLine + "    if (jssepr > 0) {" +
                    Environment.NewLine + "        a.innerHTML = rethtml.substring(0, jssepr);" +
                    Environment.NewLine + "        // Append javascript code!" +
                    Environment.NewLine + "        var newScript = document.createElement('script');" +
                    Environment.NewLine + "        newScript.text = rethtml.substring(jssepr + 9, rethtml.length);" +
                    Environment.NewLine + "        a.appendChild(newScript);" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    else {" +
                    Environment.NewLine + "        a.innerHTML = rethtml;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "}");
                    r1.Append(Environment.NewLine + "</script>");
            
            //r1.Append("</td></tr></tbody></table>");
            // script javascript
            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            r1.Append(Environment.NewLine + "function view_T_Hist(TabIndex,ID,SysV){" +
                    Environment.NewLine + "if (document.getElementById('viewthistdiv')) return;" +
                    Environment.NewLine + "var a = JsPopupInfo('viewhistbtn', 'viewthistdiv', '" + context.GetLanguageLable("History") + "');" +
                    Environment.NewLine + "ajPage('/Basetab/ViewHist?TabIndex=' + TabIndex +'&ID=' + ID +'&SysV=' + SysV, view_T_Hist_ret);}");
            r1.Append(Environment.NewLine + "function view_T_Hist_ret(http){" +
                    Environment.NewLine + "document.getElementById('idContentviewthistdiv').innerHTML= http.responseText;}");
            r1.Append(Environment.NewLine + "</script>");
            r = r1.ToString();
            r1 = null;
            return r;
        }
        private string UIBuildTabInForm(dynamic BTabGrp, ref int indexTab, ref string [] listTabName, ref int[] colsmNo)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            // Lay danh sach tab
            string tabNamePrev = "";
            // Tính Class col-sm-NUMBER
            //int CntCol = 1;
            for (int i = 0; i < BTabGrp.DBConfigGrp.Items.Count; i++)
            {
                ////// Tính Class col-sm-NUMBER với từng BOX
                ////int CntA4 = 1;
                ////string[] a4; a4 = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "EditColumnInline").Split(new string[] { "^" }, StringSplitOptions.None);
                ////for (int j = 0; j < a4.Length; j++)
                ////{
                ////    if (a4[j] == "1") CntA4 = CntA4 + 1;
                ////}
                ////// Tính Class col-sm-NUMBER với từng BOX lớn nhất
                ////if (CntA4 > CntCol && CntA4 < 13) CntCol = CntA4;
                string s = Tools.StrNull(Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "ColBreakBefore"));
                if (s == "") s = "0";
                if (s != tabNamePrev)
                {
                    colsmNo[indexTab] = 12;
                    listTabName[indexTab] = s;
                    indexTab = indexTab + 1;
                    tabNamePrev = s;
                }
            }
            for (int i = 0; i < indexTab; i++)
            {
                int CntCol = 1;
                for (int j = 0; j < BTabGrp.DBConfigGrp.Items.Count; j++)
                {
                    string s = Tools.StrNull(Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[j], "ColBreakBefore"));
                    if (s == "") s = "0";
                    if (s == listTabName[i])
                    {
                        // Tính Class col-sm-NUMBER với từng BOX
                        int CntA4 = 1;
                        string[] a4; a4 = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[j], "EditColumnInline").Split(new string[] { "^" }, StringSplitOptions.None);
                        for (int l = 0; l < a4.Length; l++)
                        {
                            if (a4[l] == "1") CntA4 = CntA4 + 1;
                        }
                        // Tính Class col-sm-NUMBER với từng BOX lớn nhất
                        if (CntA4 > CntCol && CntA4 < 13) CntCol = CntA4;
                    }
                }
                colsmNo[i] = (int)colsmNo[i] / CntCol;
                if (colsmNo[i] < 3) colsmNo[i] = 3;
            }

            // Build Tab
            r1.Append(Environment.NewLine + "<!-- classic-tabs -->" + 
                Environment.NewLine + "<div class=\"classic-tabs\">");
            if (indexTab > 1) // Có tab
            {
                r1.Append(Environment.NewLine + "<ul class=\"nav\" id=\"myClassicTab\" role=\"tablist\">");
                for (int i = 0; i < indexTab; i++)
                {
                    r1.Append(Environment.NewLine + "<li class=\"nav-item\">" +
                        Environment.NewLine + "<a href=\"#myClassicTab" + i.ToString() + "\" class=\"nav-link waves-light waves-effect waves-light " + (i == 0 ? "active" : "") + "\" data-toggle=\"tab\" role=\"tab\" " + (i == 0? "aria-selected=\"true\"":"") +" > " +
                        listTabName[i] + "</a>" +
                        Environment.NewLine + "</li>");
                }
                r1.Append(Environment.NewLine + "</ul>");
            }
            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIFormLeft()
        {
            string MenuLeftOnForm = Tools.GetDataJson(BTab.DBConfig.Items[0], "MenuLeftOnForm");
            StringBuilder r1 = new StringBuilder(); string r = "";
            if (MenuLeftOnForm != "")
            {
                try
                {
                    string[] a1; string[] a2;
                    string ChildName = Tools.GetDataJson(BTab.DBConfig.Items[0], "ChildName");
                    string ChildURL = Tools.GetDataJson(BTab.DBConfig.Items[0], "ChildURL");
                    a1 = ChildName.Split(new string[] { "^" }, StringSplitOptions.None);
                    a2 = ChildURL.Split(new string[] { "^" }, StringSplitOptions.None);
                    r1.Append(Environment.NewLine + "<!-- Front Side -->" +
                        Environment.NewLine + "<div class=\"face front\">" +
                        Environment.NewLine + "<!-- Avatar -->" +
                        Environment.NewLine + "<div class=\"card-up\"></div>" +
                        Environment.NewLine + "<div class=\"avatar mx-auto white\">" +
                        Environment.NewLine + "<img src=\"https://mdbootstrap.com/img/Photos/Avatars/img(10).jpg\" class=\"rounded-circle img-fluid\" alt=\"First sample avatar image\">" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "<!-- Content -->" +
                        Environment.NewLine + "<div class=\"card-body\">" +
                        Environment.NewLine + "<h4 class=\"mt-1 mb-3\">Nguyen Thanh Chien</h4>" +
                        Environment.NewLine + "<p class=\"font-weight-bold dark-grey-text\">Tinh Van</p>" +
                        Environment.NewLine + "<ul class=\"tabvertical\">");
                    string url0 = ""; string urli = "";
                    for (var i = 0; i < a2.Length; i++)
                    {
                        string[] a0; string[] a01; string[] a02;
                        a0 = a2[i].Split(new string[] { "||" }, StringSplitOptions.None);
                        a01 = a0[1].Split(new string[] { ";" }, StringSplitOptions.None);
                        a02 = a0[2].Split(new string[] { ";" }, StringSplitOptions.None);
                        urli = a0[0] + "?" + a01[0] + "=" + a02[0] + "&iframe=" + i;
                        for (var j = 1; j < a01.Length; j++)
                        {
                            string _a02 = a02[j];
                            if (_a02.Length > 7)
                            {
                                //if (_a02.Substring(0, 7) == "REQUEST") _a02 = context.GetRequestVal(_a02.Substring(7));
                                if (Tools.Left(_a02, 7) == "REQUEST") _a02 = context.GetRequestVal(Tools.Right(_a02, 7, false));
                            }
                            urli = urli + "&" + a01[j] + "=" + _a02;
                        }
                        if (i == 0 && TabIndex == "" || TabIndex == a1[i])
                        {
                            url0 = urli;
                            r1.Append(Environment.NewLine + "<li class=\"active\" onclick=\"window.location.href='" + urli + "';\">" +
                                Environment.NewLine + "<a href=\"" + urli + "\">" + context.GetLanguageLable(a1[i]) + "</a></li>");
                        }
                        else
                            r1.Append(Environment.NewLine + "<li onclick=\"window.location.href='" + urli + "';\">" +
                                Environment.NewLine + "<a href=\"" + urli + "\">" + context.GetLanguageLable(a1[i]) + "</a></li>");
                    }
                    
                    r1.Append(Environment.NewLine + "</ul>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "<!-- /Front Side -->");
                }
                catch (Exception e)
                {
                    HTTP_CODE.WriteLogAction("UIFormLeft - Error: " + e.ToString());
                }
            }           

            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIEditForm(ref int tabIndexNo, ref string SysU, ref string SysV, ref bool IsEdit, ref bool IsInsert, ref bool IsUpdate, ref bool IsDelete)
        {
            //string FormEditType = Tools.GetDataJson(BTab.DBConfig.Items[0], "FormEditType");
            //context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType);
            StringBuilder r1; r1 = new StringBuilder(); string r = ""; string SysUID = "-1";
            StringBuilder jsdbcols; jsdbcols = new StringBuilder();
            string[] DataVal = new string [500]; 
            string[] DataTxt = new string[500]; 
            string[] DataParent = new string[500]; 
            StringBuilder jsdbcolsReadOnly; jsdbcolsReadOnly = new StringBuilder();
            StringBuilder jsdbcolsRequire; jsdbcolsRequire = new StringBuilder();
            StringBuilder jsdbcolsDataReq; jsdbcolsDataReq = new StringBuilder();
            StringBuilder jsdbcolsName; jsdbcolsName = new StringBuilder();
            dynamic EditData = null; string ParamList = "";
            string TabIndex = Tools.GetDataJson(BTab.DBConfig.Items[0], "DBConfigCode");
            string Editor = context.GetSession("UserID");
            string iframe = context.GetRequestVal("iframe");

            if (IsEdit)
            {
                string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                string QueryStringFilter =  Tools.GetDataJson(BTab.DBConfig.Items[0], "QueryStringFilter");
                string QueryStringType =    Tools.GetDataJson(BTab.DBConfig.Items[0], "QueryStringType");
                string SPName =             Tools.GetDataJson(BTab.DBConfig.Items[0], "SchemaName");
                if (SPName != "" && SPName != null) SPName = SPName + "." + Tools.GetDataJson(BTab.DBConfig.Items[0], "spName"); else SPName = Tools.GetDataJson(BTab.DBConfig.Items[0], "spName");
                ParamList =                 Tools.GetDataJson(BTab.DBConfig.Items[0], "paramList");
                string ParamDefault =       Tools.GetDataJson(BTab.DBConfig.Items[0], "paramDefault");; bool IsStoreCache = false;
                Tools.ExecParam("EditForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, context, SPName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, false, "", "", IsStoreCache);
                SysU = "N/A";
                SysV = "0";
                if (errorCode == 200)
                {
                    EditData = JObject.Parse(json);
                    try
                    {
                        //Editor = Editor + " - " + Tools.GetDataJson(EditData.EditForm.Items[0], "CreatorName");
                        SysU = Tools.GetDataJson(EditData.EditForm.Items[0], "CreatorID") + " - " + Tools.GetDataJson(EditData.EditForm.Items[0], "CreatorName");
                        SysUID = Tools.GetDataJson(EditData.EditForm.Items[0], "CreatorID");
                        SysV = Tools.GetDataJson(EditData.EditForm.Items[0], "CreatorVersion");
                    } catch { }
                }
            }

            // check role overwrite //804 - Overwrite;805 - Attachments
            if (!context.CheckEditDeleteRoles(SysUID) && IsEdit)
            {
                IsUpdate = false; IsDelete = false;
            }
            r1.Append(UIDef.UIHidden("SysUID", context.enc.Encrypt(SysUID.ToString())));
            int iGrp = 0; dynamic[] dTab = new dynamic[50]; //int iTab = 0; 
            int jCnt = 0;
            // Get list tab (Count > 0)
            int indexTab = 0; string[] listTabName = new string[10]; int[] colsmNo = new int[10];
            r1.Append(UIBuildTabInForm(BTabGrp, ref indexTab, ref listTabName, ref colsmNo));

            r1.Append(Environment.NewLine + "<!-- Open tab-content -->" +
                Environment.NewLine + "<div class=\"tab-content\" id=\"myClassicTabContent\">"); // Open tab-content
            for (int iTab = 0; iTab < indexTab; iTab++)
            {
                r1.Append(Environment.NewLine + "<div class=\"tab-pane fade  " + (iTab == 0 ? "active show" : "") + "\" role=\"tabpanel\" id=\"myClassicTab" + iTab.ToString() + "\">" +
                    Environment.NewLine + "<!--Accordion wrapper-->" +
                    Environment.NewLine + "<div class=\"accordion md-accordion\" id=\"accordionEx\" role=\"tablist\">"); // Open tab-pane fade // accordion md-accordion

                bool IsOpenGrp = false; bool IsShow = true; 
                for (int i = 0; i < BTabGrp.DBConfigGrp.Items.Count; i++)
                {
                    string ColBreakBefore = Tools.StrNull(Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "ColBreakBefore"));
                    if (ColBreakBefore == "") ColBreakBefore = "0";
                    if (ColBreakBefore == listTabName[iTab])
                    {
                        string DBConfigGrpName = Tools.StrNull(Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "DBConfigGrpName"));
                        string EditColumn = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "EditColumn");
                        string EditColumnLable = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "EditColumnLable");
                        string EditColumnType = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "EditColumnType");
                        string EditColumnInline = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "EditColumnInline");
                        string ReadOnlyMasks = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "ReadOnlyMasks");
                        string Cacheable = Tools.GetDataJson(BTabGrp.DBConfigGrp.Items[i], "Cacheable");
                        
                        // Start Step
                        if (IsOpenGrp) r1.Append(Environment.NewLine + "<!-- /row inline-input -->" +
                            Environment.NewLine + "</div>" +
                            Environment.NewLine + "<!-- /Card body -->" +
                            Environment.NewLine + "</div>" +
                            Environment.NewLine + "<!-- /collapseOne" + (i-1).ToString() + " -->" +
                            Environment.NewLine + "</div>" + // Close card body
                            Environment.NewLine + "<!-- /Accordion card -->" +
                            Environment.NewLine + "</div>"); // Close card
                        IsOpenGrp = true;
                        r1.Append(Environment.NewLine + "<!-- Accordion card -->" +
                            Environment.NewLine + "<div class=\"card\">"); // Open card
                        
                        // Card header
                        r1.Append(Environment.NewLine + "<!-- Card header -->" +
                            Environment.NewLine + "<div class=\"card-header\" role=\"tab\" id=\"headingOne" + i.ToString() + "\">" +
                            (IsShow ?
                                Environment.NewLine + "<a data-toggle=\"collapse\" data-parent=\"#accordionEx\" href=\"#collapseOne" + i.ToString() + "\" " + "aria-expanded=\"true\"" + " aria-controls=\"collapseOne" + i.ToString() + "\">" :
                                Environment.NewLine + "<a class=\"collapsed\" data-toggle=\"collapse\" data-parent=\"#accordionEx\" href=\"#collapseOne" + i.ToString() + "\" " + "aria-expanded=\"false\"" + " aria-controls=\"collapseOne" + i.ToString() + "\">"
                            ) +
                            "<h5 class=\"mb-0 titlediv\">" + DBConfigGrpName + " <i class=\"fa fa-angle-down rotate-icon\"></i></h5></a>" +
                            Environment.NewLine + "</div>" +
                            Environment.NewLine + "<!-- /Card header -->");

                        // Card body
                        r1.Append(Environment.NewLine + "<!-- collapseOne" + i.ToString() + " -->" + // Open card body
                            Environment.NewLine + "<div id=\"collapseOne" + i.ToString() + "\" class=\"collapse " + (IsShow ? "show" : "") + "\" role=\"tabpanel\" aria-labelledby=\"headingOne" + i.ToString() + "\" data-parent=\"#accordionEx\">" +
                            Environment.NewLine + "<!-- Card body -->" +
                            Environment.NewLine + "<div class=\"card-body\">" +
                            Environment.NewLine + "<!-- row inline-input -->" +
                            Environment.NewLine + "<div class=\"row inline-input\">");

                        IsShow = false;

                        string[] a1; a1 = EditColumn.Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] a2; a2 = EditColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] a3; a3 = EditColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] a4; a4 = EditColumnInline.Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] a5; a5 = ReadOnlyMasks.Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] a6; a6 = Cacheable.Split(new string[] { "^" }, StringSplitOptions.None);

                        string ClassReadonly = " disabled class='ironly' ";
                        string mandantoryLabel = " <font color='red'>*</font>";//(*)
                        r1.Append(Environment.NewLine + "<!-- col-sm-" + colsmNo[iTab].ToString() + " -->" +
                            Environment.NewLine + "<div class=\"col-sm-" + colsmNo[iTab].ToString() + "\">");// Open col-sm-4
                        for (int j = 0; j < a2.Length; j++)
                        {
                            jsdbcols.Append(Environment.NewLine + "jsdbcols[" + (jCnt + j) + "] = '" + a1[j] + "';");
                            jsdbcolsName.Append(Environment.NewLine + "jsdbcolsName[" + (jCnt + j) + "] = '" + context.GetLanguageLable(a2[j]) + "';");
                            DataVal[jCnt + j] = ""; DataTxt[jCnt + j] = ""; DataParent[jCnt + j] = "";
                            jsdbcolsReadOnly.Append("jsdbcolsReadOnly[" + (jCnt + j) + "] = " + (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? "'" + a1[j] + "'" : "''") + ";");

                            string[] a31 = a3[j].Split(new string[] { ";" }, StringSplitOptions.None);
                            string v = "";// context.GetQueryString(a1[j]); - get cache value
                            try
                            {
                                v = Tools.GetDataJson(EditData.EditForm.Items[0], a1[j]);
                            }
                            catch { v = ""; }
                            if (v == "") v = a31[1];
                            v = Tools.ParseValue(context, v, false);
                            string val = v;
                            if (a6[j] == "1" && !IsEdit) if (val == "") val = context.GetSession("txtSession_" + context.GetSession("UserID") + "_" + a1[j]);

                            // Set default ko check data                    
                            jsdbcolsDataReq.Append(Environment.NewLine + "jsdbcolsDataReq[" + (jCnt + j) + "] = '';");
                            if (a31[0] == "Hidden")
                            {
                                val = context.ReplaceSessionValue(v);
                                r1.Append(UIDef.UIHidden(a1[j], val));
                                jsdbcolsRequire.Append(Environment.NewLine + "jsdbcolsRequire[" + (jCnt + j) + "] = '';");
                            }
                            else
                            {
                                // TabIndex  // khóa nhâp liệu
                                string tabIndexForm = " tabindex=\"" + tabIndexNo + "\" ";
                                ClassReadonly = (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? " readonly class='form-control' " : " class='form-control' ") + tabIndexForm;
                                tabIndexNo++;

                                bool ColSpan = false;
                                if (j < a4.Length) ColSpan = (Tools.CIntNull(a4[j]) == 1);
                                if (ColSpan)
                                {
                                    r1.Append(Environment.NewLine + "<!-- /col-sm-" + colsmNo[iTab].ToString() + " -->" +
                                        Environment.NewLine + "</div>" + // Close col-sm-" + colsmNo.ToString() + "
                                        Environment.NewLine + "<!-- col-sm-" + colsmNo[iTab].ToString() + " -->" +
                                        Environment.NewLine + "<div class=\"col-sm-" + colsmNo[iTab].ToString() + "\">");// Open col-sm-" + colsmNo.ToString() + "
                                }
                                if (a31[0] != "Checkbox")
                                    r1.Append(Environment.NewLine + "<!-- form-group row -->" + 
                                        Environment.NewLine + "<div class=\"form-group row\">" + // Open form-group row
                                        Environment.NewLine + "<label class=\"col-form-label active\">" + context.GetLanguageLable(a2[j]) + (Tools.CIntNull(a31[2]) == 1 ? mandantoryLabel : "") + ": </label>");

                                string ReqJson = ""; //string onclick = "";
                                switch (a31[0])
                                {
                                    //a31[2]: Option check Require
                                    case "AutoNumber":
                                        string inputchk = context.GetLanguageLable("AutoNumber");
                                        //r1.Append(UIDef.SelectStrOption(a3[j], context.GetLanguageLable("AutoNumber"), ref inputchk, true));
                                        r1.Append(UIDef.UISelectStr(a1[j], context.GetLanguageLable("AutoNumber"), inputchk, ref inputchk, true, ClassReadonly, UIDef.CNextFocus(a1[j])));
                                        break;
                                    case "Textarea":
                                        bool IsEditor = false;
                                        if (a31.Length > 6)
                                            IsEditor = (a31[6] == "1");
                                        r1.Append(UIDef.UITextarea(context.GetLanguageLable(a2[j]), a1[j], val, ClassReadonly + " maxLength = \"" + a31[3] + "\" onkeypress =\"if(this.maxLength <= this.value.length)JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Column") + " [" + context.GetLanguageLable(a2[j]) + "] " + context.GetLanguageLable("MaxLength") + ": ' + this.maxLength, '', '0');\"", "", "", IsEditor));//" cols=\"" + a31[3] + "\" rows=\"" + a31[4] + "\" "
                                        break;
                                    case "Textbox":
                                        r1.Append(UIFormElements.UITextbox(context.GetLanguageLable(a2[j]), a1, j, val, a31, " onkeypress =\"if(this.maxLength <= this.value.length)" +
                                            "JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Column") + " [" + context.GetLanguageLable(a2[j]) + "] " + context.GetLanguageLable("MaxLength") + ": ' + this.maxLength, '', '0'); else cnextfocus(event, this.form, '" + a1[j] + "');\"" +//alert('" + context.GetLanguageLable("Column") + " [" +context.GetLanguageLable(a2[j]) + "] " + context.GetLanguageLable("MaxLength") + ": ' + this.maxLength);
                                            " autocomplete=\"off\" " + ClassReadonly));
                                        if (Tools.Right(a1[j], 6).ToUpper() == "MOBILE" || Tools.Right(a1[j], 5).ToUpper() == "PHONE" ||
                                            Tools.Right(a1[j], 3).ToUpper() == "TEL" || Tools.Right(a1[j], 3).ToUpper() == "FAX")
                                        {
                                            jsdbcolsDataReq.Append(Environment.NewLine + "jsdbcolsDataReq[" + (jCnt + j) + "] = '" + a1[j] + "|phoneValid';");
                                        }
                                        else if (Tools.Right(a1[j], 5).ToUpper() == "EMAIL")
                                        {
                                            jsdbcolsDataReq.Append(Environment.NewLine + "jsdbcolsDataReq[" + (jCnt + j) + "] = '" + a1[j] + "|emailValid';");
                                        }
                                        break;
                                    case "Password":
                                        //UIPassword(string InputName, string Param, string NextFocus = "")
                                        r1.Append(UIDef.UIPassword(a1[j], ClassReadonly + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", UIDef.FocusAfter(a1[j])));
                                        break;
                                    case "Algorithm":
                                        //UIPassword(string InputName, string Param, string NextFocus = "")
                                        try { val = context.enc.Decrypt(val); } catch { }
                                        r1.Append(UIDef.UIPassword(a1[j], ClassReadonly + " value=\"" + val + "\" autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", UIDef.FocusAfter(a1[j])));
                                        break;
                                    case "Numeric":
                                        r1.Append(UIFormElements.UINumeric(context, context.GetLanguageLable(a2[j]), a1, a2, j, val, a31, " autocomplete=\"off\" " + ClassReadonly));
                                        jsdbcolsDataReq.Append(Environment.NewLine + "jsdbcolsDataReq[" + (jCnt + j) + "] = '" + a1[j] + "|numericValid';");
                                        break;
                                    case "Date":
                                        val = Tools.HRMDate(val);
                                        if (a31.Length > 3)
                                            r1.Append(UIDef.UIDate(context.GetLanguageLable(a2[j]), a1[j], val, " autocomplete=\"off\" " + ClassReadonly, a31[3], UIDef.NextFocus(a1[j], "bosfrm"), "bosfrm"));
                                        else
                                            r1.Append(UIDef.UIDate(context.GetLanguageLable(a2[j]), a1[j], val, " autocomplete=\"off\" " + ClassReadonly, UIDef.NextFocus(a1[j], "bosfrm"), "bosfrm"));
                                        jsdbcolsDataReq.Append(Environment.NewLine + "jsdbcolsDataReq[" + (jCnt + j) + "] = '" + a1[j] + "|vietDateValid|" + context.GetQueryString("Min" + a1[j]) + "|" + context.GetQueryString("Max" + a1[j]) + "';");
                                        break;
                                    case "Datetime":
                                        val = Tools.HRMDateTime(val);
                                        if (a31.Length > 3)
                                            r1.Append(UIDef.UIDateTime(context.GetLanguageLable(a2[j]), a1[j], val, " autocomplete=\"off\" " + ClassReadonly, a31[3], UIDef.NextFocus(a1[j], "bosfrm"), "bosfrm"));
                                        else
                                            r1.Append(UIDef.UIDateTime(context.GetLanguageLable(a2[j]), a1[j], val, " autocomplete=\"off\" " + ClassReadonly, UIDef.NextFocus(a1[j], "bosfrm"), "bosfrm"));
                                        jsdbcolsDataReq.Append(Environment.NewLine + "jsdbcolsDataReq[" + (jCnt + j) + "] = '" + a1[j] + "|vietDateTimeValid|" + context.GetQueryString("Min" + a1[j]) + "|" + context.GetQueryString("Max" + a1[j]) + "';");
                                        break;
                                    case "Time":
                                        if (a31.Length > 3)
                                            r1.Append(UIDef.UITime(context.GetLanguageLable(a2[j]), a1[j], val, " autocomplete=\"off\" " + ClassReadonly, a31[3], UIDef.NextFocus(a1[j], "bosfrm"), "bosfrm"));
                                        else
                                            r1.Append(UIDef.UITime(context.GetLanguageLable(a2[j]), a1[j], val, " autocomplete=\"off\" " + ClassReadonly, UIDef.NextFocus(a1[j], "bosfrm"), "bosfrm"));
                                        //r1.Append(UIDef.UITime(a1[j], val, " autocomplete=\"off\" " + ClassReadonly, UIDef.CNextFocusAction(a1[j]), "bosfrm"));
                                        jsdbcolsDataReq.Append(Environment.NewLine + "jsdbcolsDataReq[" + (jCnt + j) + "] = '" + a1[j] + "|timeValid|" + context.GetQueryString("Min" + a1[j]) + "|" + context.GetQueryString("Max" + a1[j]) + "';");
                                        break;
                                    case "Checkbox":
                                        ClassReadonly = (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? " readonly class='form-check-input' " : " class='form-check-input' ") + tabIndexForm;
                                        r1.Append(UIDef.UICheckbox(a1[j], context.GetLanguageLable(a2[j]) + (Tools.CIntNull(a31[2]) == 1 ? mandantoryLabel : ""), a31[4], val, ClassReadonly + UIDef.FocusAfter(a1[j])));
                                        break;
                                    case "Radio":
                                        r1.Append(UIFormElements.UIRadio(context, a1, j, val, a31, ClassReadonly));
                                        //r1.Append(UIDef.UIRadio(a1[j], a31[3], a31[4], val, ClassReadonly + UIDef.FocusAfter(a1[j])));
                                        break;
                                    case "Actb":
                                        ReqJson = context.InputDataSetParam(a31[6]);
                                        r1.Append(UIDef.UIActbId(a1[j], ref DataDAO, a31[5], ReqJson, a31[7], val, ClassReadonly + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a1[j]), "bosfrm"));
                                        break;
                                    case "ActbText":
                                        r1.Append(UIDef.UIActbStrId(a1[j], a31[5], a31[6], val, ClassReadonly + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a1[j]), "bosfrm"));
                                        break;
                                    case "Treebox":
                                        ReqJson = context.InputDataSetParam(a31[4]);
                                        r1.Append(UIFormElements.UITreeview(a1[j], ref DataDAO, a31[3], ReqJson, a31[5], val, a31, (jCnt + j)));
                                        ////r1.Append(UIDef.UITreeview(context.GetLanguageLable(a2[j]), DataDAO, context, a1[j], a31[3], ReqJson, a31[5], a31[6], val, int.Parse(a31[7]), int.Parse(a31[8]), false, false, true, a31[9], a31[10]));
                                        break;
                                    case "MutilCheckbox":
                                        ////ReqJson = context.InputDataSetParam(a31[4]);
                                        ////onclick = ""; if (a31.Length > 8) onclick = " onclick=\"" + a31[6] + ",'" + a1[j] + "'," + (jCnt + j) + ");\"";
                                        ////r1.Append(UIDef.UIMultipleSelect(a1[j], context, ref DataDAO, a31[3], ReqJson, a31[5], ref val, UIDef.FocusAfter(a1[j]), "", ref DataVal[jCnt + j], ref DataParent[jCnt + j]));
                                        //////r1.Append(UIDef.UIDD(a1[j], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a1[j]), "", ClassReadonly, int.Parse(a31[6]), int.Parse(a31[7])));
                                        ////break;
                                    case "DivMutilCheckbox":
                                        ////ReqJson = context.InputDataSetParam(a31[4]);
                                        //////r1.Append(UIDef.UIMultipleSelect(a1[j], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a1[j]), ""));
                                        ////onclick = ""; if (a31.Length > 8) onclick = " onclick=\"" + a31[8] + ",'" + a1[j] + "'," + (jCnt + j) + ");\"";
                                        ////r1.Append(UIDef.UIDD(a1[j], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a1[j]), onclick,
                                        ////    ClassReadonly, int.Parse(a31[6]), int.Parse(a31[7]), ref DataVal[jCnt + j], ref DataParent[jCnt + j]));
                                        ////break;
                                    case "Selectbox":
                                        ReqJson = context.InputDataSetParam(a31[4]);
                                        UIDef.OptionStringVal(ref DataDAO, a31[3], ReqJson, a31[5], ref DataVal[jCnt + j], ref DataTxt[jCnt + j], ref DataParent[jCnt + j]);
                                        r1.Append(UIFormElements.UISelectStr(a1[j], DataVal[jCnt + j], DataTxt[jCnt + j], a31, ref val, true, ClassReadonly, (jCnt + j)));
                                        ////string size = "";
                                        ////if (a31.Length > 7) size = a31[7];
                                        ////if (a31.Length > 6)
                                        ////    if (a31[6].Length > 0)
                                        ////        r1.Append(UIDef.UISelectStr(a1[j], DataVal[jCnt + j], DataTxt[jCnt + j], ref val, true, ClassReadonly + (size != "" ? " style=\"width:" + size + "px;\"" : ""),
                                        ////        a31[6].Replace("`", "'") + "," + (jCnt + j + 1) + ");" + UIDef.NextFocus(a1[j])));
                                        ////    else
                                        ////        r1.Append(UIDef.UISelectStr(a1[j], DataVal[jCnt + j], DataTxt[jCnt + j], ref val, true, ClassReadonly + (size != "" ? " style=\"width:" + size + "px;\"" : ""), UIDef.NextFocus(a1[j])));
                                        ////else
                                        ////    r1.Append(UIDef.UISelectStr(a1[j], DataVal[jCnt + j], DataTxt[jCnt + j], ref val, true, ClassReadonly + (size != "" ? " style=\"width:" + size + "px;\"" : ""), UIDef.NextFocus(a1[j])));
                                        break;
                                    case "SelectboxText":
                                        r1.Append(UIFormElements.UISelectStr(a1[j], a31[3], a31[4], a31, ref val, true, ClassReadonly, (jCnt + j), 6));
                                        ////string Onchange = "";
                                        ////if (a31.Length > 5) Onchange = a31[5];
                                        ////r1.Append(UIDef.UISelectStr(a1[j], a31[3], a31[4], ref val, true, ClassReadonly, Onchange + ";" + UIDef.CNextFocus(a1[j])));
                                        break;
                                    case "AttachImage":
                                        r1.Append(Environment.NewLine + "<span class=\"input-group-addon\" data-toggle=\"modal\" data-target=\"#modalVM\">" +
                                            Environment.NewLine + "<a target=\"ModelPopup" + iframe + "\"" +
                                            "href=\"" + (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? "#" : "/Media/UploadImage?TabIndex=" + TabIndex + "&InputName=" + a1[j]) + "\" " +
                                            "class=\"avatar mx-auto white avatar32\" >" +
                                            //"data-toggle=\"modal\" " +
                                            //"data-target=\"#modalVM\">" +
                                            Environment.NewLine + "<img id=\"Img" + a1[j] + "\" name=\"Img" + a1[j] + "\" " +
                                            "src=\"/Media/RenderFile?ImageId=" + val + "&NoCrop=\" " +
                                            "class=\"rounded-circle img-fluid\">" +
                                            Environment.NewLine + "</a></span>");

                                        ////ClassReadonly = (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? " disabled class='btn inport' " : " class='btn inport' ") + tabIndexForm;
                                        ////r1.Append(UIDef.UIHidden(a1[j], val));
                                        ////if (IsInsert || IsUpdate || IsDelete) r1.Append(UIDef.UIButton("Bnt" + a1[j], context.GetLanguageLable("Upload"), "attachment_" + a1[j] + "();", ClassReadonly) + " <br>");
                                        ////r1.Append("<img id=\"Img" + a1[j] + "\" name=\"Img" + a1[j] + "\" src=\"/Media/RenderFile?ImageId=" + val + "&NoCrop=\" width=" + a31[3] + ">");// height=" + a31[4] + "
                                        if (IsInsert || IsUpdate || IsDelete) r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                                           ////Environment.NewLine + "function attachment_" + a1[j] + "() {" +
                                           ////Environment.NewLine + "_attw = window.open('/Media/UploadImage?TabIndex=" + TabIndex + "&InputName=" + a1[j] + "', " +
                                           ////Environment.NewLine + "'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=100, left=200, width=650, height=400');" +
                                           ////Environment.NewLine + "} " +
                                           Environment.NewLine + "function addattachedfile_" + a1[j] + "(docid) {" +
                                           Environment.NewLine + "var attdoc = document.getElementById('Img" + a1[j] + "');" +
                                           Environment.NewLine + "attdoc.src = \"/Media/RenderFile?ImageId=\" + docid + \"&NoCrop=\";" +
                                           Environment.NewLine + "var a = document.bosfrm.elements['" + a1[j] + "'];a.value = docid;" +
                                           Environment.NewLine + "}</script>");
                                        break;
                                    case "AttachFiles":
                                        ClassReadonly = (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? " disabled class='btn inport' " : " class='btn inport' ") + tabIndexForm;
                                        r1.Append(UIDef.UIHidden(a1[j], val));
                                        if (IsInsert || IsUpdate || IsDelete) r1.Append(UIDef.UIButton("Bnt" + a1[j], context.GetLanguageLable("Upload"), "attachment_" + a1[j] + "();", ClassReadonly + " class=\"btn inport\"") + "<br>");
                                        r1.Append("<ul id=\"docsUL_" + a1[j] + "\">");
                                        if (val != "")
                                        {
                                            string[] aImg = val.Split(new string[] { "^" }, StringSplitOptions.None);
                                            if (aImg.Length > 1)
                                            {
                                                for (int ji = 0; ji < aImg.Length; ji++)
                                                {
                                                    if (aImg[ji] != "")
                                                    {
                                                        string[] aImg1 = aImg[ji].Split(new string[] { ";" }, StringSplitOptions.None);
                                                        if (aImg1.Length > 1) r1.Append(Environment.NewLine + "<li id='" + aImg1[0] + ";" + aImg1[1] + "'><a href=\"/Media/RenderFile?ImageId=" + aImg1[0] + "\">" +
                                                               aImg1[1] + "</a>&nbsp;&nbsp;&nbsp;<a href=\"javascript:RemoveFile_" + a1[j] + "('" + aImg1[0] + ";" + aImg1[1] + "');\">" + context.GetLanguageLable("Delete") + "</a></li>");
                                                    }
                                                }
                                            }
                                        }
                                        r1.Append("</ul>");
                                        if (IsInsert || IsUpdate || IsDelete) r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                                           Environment.NewLine + "function attachment_" + a1[j] + "() {" +
                                           Environment.NewLine + "_attw = window.open('/Media/UploadFile?TabIndex=" + TabIndex + "&InputName=" + a1[j] + "', " +
                                           Environment.NewLine + "'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=100, left=200, width=650, height=400');" +
                                           Environment.NewLine + "} " +
                                           Environment.NewLine + "function RemoveFile_" + a1[j] + "(id) {" +
                                           Environment.NewLine + "var a = document.bosfrm.elements['" + a1[j] + "'];" +
                                           Environment.NewLine + "var attdoc = document.getElementById(id); " +
                                           Environment.NewLine + "if(attdoc){attdoc.innerHTML=''; " +
                                           Environment.NewLine + "if(a.value.indexOf('^') > 0) " +
                                           Environment.NewLine + "a.value = a.value.replace('^' + id); " +
                                           Environment.NewLine + "else " +
                                           Environment.NewLine + "a.value = '';" +
                                           Environment.NewLine + "}} " +
                                           Environment.NewLine + "function addattachedfile_" + a1[j] + "(docid, filename) {" +
                                           Environment.NewLine + "var attdoc = document.getElementById('docsUL_" + a1[j] + "');" +
                                           Environment.NewLine + "attdoc.innerHTML = attdoc.innerHTML + '<li id=\"' + docid + ';' + filename + '\"><a href=\"/Media/RenderFile?ImageId=' + docid + '\">' + filename + '</a>&nbsp;&nbsp;&nbsp;<a href=\"javascript:RemoveFile_" + a1[j] + "(\\'' + docid + ';' + filename + '\\');\">" + context.GetLanguageLable("Delete") + "</a></li>';" +
                                           Environment.NewLine + "var a = document.bosfrm.elements['" + a1[j] + "'];if(a.value != '') a.value = a.value + '^' + docid + ';' + filename; else a.value = docid + ';' + filename; " +
                                           Environment.NewLine + "}</script>");
                                        break;
                                    case "SearchBox":
                                        ClassReadonly = (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? " JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Disabled") + "', '', '0'); " : " Search_" + a1[j] + "(); ") + "\"" + tabIndexForm;
                                        r1.Append(UIFormElements.UISearchBox(context.GetLanguageLable(a2[j]), a1, j, val, a3, a31, ClassReadonly, ref jsdbcols, context, EditData, true, "bosfrm", IsInsert, IsUpdate, IsDelete));
                                        break;
                                    case "SearchBoxM":
                                        //r1.Append(UIFormElements.UISearchBoxCount(a1, j, val, a3, a31, ClassReadonly, ref jsdbcols, context, EditData, true, "bosfrm", IsInsert, IsUpdate, IsDelete));
                                        ClassReadonly = (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? " JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Disabled") + "', '', '0'); " : " Search_" + a1[j] + "(); ") + "\"" + tabIndexForm;
                                        r1.Append(UIFormElements.UISearchBoxMulti(context.GetLanguageLable(a2[j]), a1, j, val, a3, a31, ClassReadonly, ref jsdbcols, context, EditData, true, "bosfrm", IsInsert, IsUpdate, IsDelete));
                                        break;
                                    case "SearchBoxC":
                                        ClassReadonly = (Tools.CIntNull(a5[j]) == 1 || Tools.CIntNull(a5[j]) == 2 && IsEdit || Tools.CIntNull(a5[j]) == 3 && !IsEdit ? " JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Disabled") + "', '', '0'); " : " Search_" + a1[j] + "(); ") + "\"" + tabIndexForm;
                                        r1.Append(UIFormElements.UISearchBoxCount(context.GetLanguageLable(a2[j]), a1, j, val, a3, a31, ClassReadonly, ref jsdbcols, context, EditData, true, "bosfrm", IsInsert, IsUpdate, IsDelete));
                                        //r1.Append(UIFormElements.UISearchBoxMulti(a1, j, val, a3, a31, ClassReadonly, ref jsdbcols, context, EditData, true, "bosfrm", IsInsert, IsUpdate, IsDelete));
                                        break;
                                    case "BoxTable":
                                        r1.Append(UIFormElements.UIBoxTable(a1, j, a31, ClassReadonly, DataDAO, context, EditData, "bosfrm"));
                                        //r1.Append(UIFormElements.UITableBox(a1, j, val, a3, a31, ClassReadonly, ref jsdbcols, context, EditData, true, "bosfrm", IsInsert, IsUpdate, IsDelete));
                                        //r1.Append(UIFormElements.UISearchBoxMulti(a1, j, val, a3, a31, ClassReadonly, ref jsdbcols, context, EditData, true, "bosfrm", IsInsert, IsUpdate, IsDelete));
                                        break;
                                }

                                if (a31[0] != "Checkbox") r1.Append(Environment.NewLine + "</div>" +
                                    Environment.NewLine + "<!-- /form-group row -->");// Close form-group row
                                if (Tools.CIntNull(a31[2]) == 1)
                                {
                                    jsdbcolsRequire.Append(Environment.NewLine + "jsdbcolsRequire[" + (jCnt + j) + "] = '" + a1[j] + "';");
                                }
                                else
                                    jsdbcolsRequire.Append(Environment.NewLine + "jsdbcolsRequire[" + (jCnt + j) + "] = '';");
                            }

                            jsdbcols.Append(Environment.NewLine + "DataParent[" + (jCnt + j) + "] = '" + DataParent[jCnt + j] + "';");
                            jsdbcols.Append(Environment.NewLine + "DataTxt[" + (jCnt + j) + "] = '" + DataTxt[jCnt + j] + "';");
                            jsdbcols.Append(Environment.NewLine + "DataVal[" + (jCnt + j) + "] = '" + DataVal[jCnt + j] + "';");
                        }
                        r1.Append(Environment.NewLine + "<!-- /col-sm-" + colsmNo[iTab].ToString() + " -->" +
                            Environment.NewLine + "</div>"); // Close col-sm-" + colsmNo.ToString() + "
                        //r1.Append("</div>");
                        jCnt = jCnt + a2.Length;
                        iGrp = iGrp + 1;
                    }                    
                }

                r1.Append(Environment.NewLine + "<!-- /row inline-input -->" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /Card body -->" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /collapseOne -->" +
                    Environment.NewLine + "</div>" + // Close card body
                    Environment.NewLine + "<!-- /Accordion card -->" +
                    Environment.NewLine + "</div>"); // Close card

                r1.Append(Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!--/Accordion wrapper-->" +
                    Environment.NewLine + "</div>"); // Close tab-pane fade // accordion md-accordion
            }
            r1.Append(Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /Open tab-content -->" +
                    Environment.NewLine + "</div>" + // Close div col-sm-10 use-content
                    Environment.NewLine + "<!-- /classic-tabs -->" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /col-sm-10 use-content -->" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /row -->"); // Close div row

            // script javascript
            r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                    Environment.NewLine + "$('select.select2.form-control:not([multiple])').select2();" +
                    Environment.NewLine + "$('select.select2.form-control[multiple]').select2({" +
                    Environment.NewLine + "closeOnSelect: false" +
                    Environment.NewLine + "});" +
                    Environment.NewLine + "ActionKeypress = 2;" +
                    Environment.NewLine + "var jsdbcols = new Array();" +
                    Environment.NewLine + "var jsdbcolsName = new Array();" +
                    Environment.NewLine + "var DataParent = new Array();" +
                    Environment.NewLine + "var DataTxt = new Array();" +
                    Environment.NewLine + "var DataVal = new Array();" +
                    Environment.NewLine + "var jsdbcolsReadOnly = new Array();" +
                    Environment.NewLine + "var jsdbcolsRequire = new Array();" +
                    Environment.NewLine + "var jsdbcolsDataReq = new Array();" +
                    Environment.NewLine + "function parentcallback(method, url, params) {" +
                    Environment.NewLine + "var parent=window.opener;if (parent==null) parent=dialogArguments;" +
                    Environment.NewLine + "if (method=='[JS]') {eval('parent.' + url + '(' + params + ');');}" +
                    Environment.NewLine + "else {parent.location.replace(url + '?' + params);}" +
                    Environment.NewLine + "window.close();}");
            r1.Append(Environment.NewLine + jsdbcols.ToString());
            r1.Append(Environment.NewLine + jsdbcolsName.ToString());
            r1.Append(Environment.NewLine + jsdbcolsRequire.ToString());
            r1.Append(Environment.NewLine + jsdbcolsDataReq.ToString());
            r1.Append(Environment.NewLine + jsdbcolsReadOnly.ToString());
            r1.Append(Environment.NewLine + "" +
                    Environment.NewLine + "var _attw; // this is windowID of attachment window popup");
            r1.Append(Tools.GenResetFunc("focusfirst") +
                    Environment.NewLine + "focusfirst(document.bosfrm);" +
                    Tools.GenResetFunc("nextfocus") +
                    Tools.GenResetFunc("cnextfocus"));
            string UserAgent = context.GetHeaderValue("User-Agent"); bool IsFirefox = (UserAgent.IndexOf("Firefox/")>-1);
            r1.Append(Environment.NewLine + "var isfirefox = " + (IsFirefox?"true":"false") + ";" +
                    Environment.NewLine + "var mySubmit1=true;" +
                    Environment.NewLine + "function Saving(a){" +
                    Environment.NewLine + "if(a){" +
                    Environment.NewLine + "document.getElementById('saving" + iframe + "').style.display='block';" +
                    Environment.NewLine + "if(document.getElementById('_editsavebtn'))document.getElementById('_editsavebtn').disabled=true;" +
                    Environment.NewLine + "mySubmit1=false;" +
                    Environment.NewLine + "}else{" +
                    Environment.NewLine + "document.getElementById('saving" + iframe + "').style.display='none';" +
                    Environment.NewLine + "if(document.getElementById('_editsavebtn'))document.getElementById('_editsavebtn').disabled=false;" +
                    Environment.NewLine + "mySubmit1=true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "var copytonew=false;");
            r1.Append(Environment.NewLine + "function myBack() {location.href='" + Tools.UrlDecode(context.GetRequestVal("GetUrl")) + "?" +
                Tools.UrlDecode(context.GetRequestVal("GetQueryString")) + "';if (_attw!=null) _attw.close();}");

            if(IsDelete)r1.Append(Environment.NewLine + "function myDelete(f) {" +
                Environment.NewLine + "if (!mySubmit1){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Disabled") + "', '', '0');" +
                    Environment.NewLine + "//alert('" + context.GetLanguageLable("Disabled") + "!');" +
                    Environment.NewLine + "return false;}" +
                    Environment.NewLine + "JsConfirm('" + context.GetLanguageLable("Delete") + "', '" + context.GetLanguageLable("ConfirmDelete") + " ?', '" + context.GetLanguageLable("OK") + "', '" + context.GetLanguageLable("Cancle") + "', f + \".action='/Basetab/Delete';\" + f + \".submit()\");}");

            if (IsInsert || IsUpdate) r1.Append(Environment.NewLine + "function newfrmvar(f, nev, val, hidden){" +
                    Environment.NewLine + "var elm = document.createElement(\"input\");" +
                    Environment.NewLine + "if (hidden) elm.type='hidden';" +
                    Environment.NewLine + "elm.name = nev;elm.value = val;" +
                    Environment.NewLine + "f.appendChild(elm);}");

            if (IsInsert || IsUpdate) r1.Append(Environment.NewLine + "function myCopy(f) {" +
                    Environment.NewLine + "newfrmvar(f, \"editcopy\", \"" + context.GetLanguageLable("Copying") + "... \", false);" +
                    Environment.NewLine + "newfrmvar(f, \"isCopy\", \"on\", true);" +
                    Environment.NewLine + "/*f.submit();*/}");

            if (IsInsert && IsUpdate) r1.Append(Environment.NewLine + "function changeSaveOption(f) {" +
                    Environment.NewLine + "var saveastitle='" + context.GetLanguageLable("SaveAs") + "';" +
                    Environment.NewLine + "var editsavebtn=document.getElementById('_editsavebtn');" +
                    Environment.NewLine + "if (editsavebtn.value==saveastitle) {" +
                    Environment.NewLine + "editsavebtn.value='" + context.GetLanguageLable("Save") + "';" +
                    "f." + context.GetRequestVal("IdCol") + ".value='" + context.GetRequestVal(context.GetRequestVal("IdCol")) + "';" +
                    Environment.NewLine + "copytonew = false; f.CopyID.value=0;" +
                    Environment.NewLine + "}else {" +
                    Environment.NewLine + "editsavebtn.value=saveastitle;f." + context.GetRequestVal("IdCol") + ".value='-1';" +
                    Environment.NewLine + "copytonew = true; f.CopyID.value=" + context.GetRequestVal(context.GetRequestVal("IdCol")) + ";}}");
            
            if (IsInsert && !IsUpdate) r1.Append(Environment.NewLine + "function changeSaveOption(f) {" +
                    Environment.NewLine + "var saveastitle='" + context.GetLanguageLable("SaveAs") + "';" +
                    Environment.NewLine + "var editsavebtn=document.getElementById('_editsavebtn');" +
                    Environment.NewLine + "if(!editsavebtn){document.getElementById('_idDivMySubmit').innerHTML=\"" + UIDef.UIButton("_editsavebtn", context.GetLanguageLable("Save"), "mySubmit(document.bosfrm)", " class=\"btn save\"").Replace("\"", "\\\"") + "\";editsavebtn=document.getElementById('_editsavebtn');}" +
                    "" +
                    Environment.NewLine + "editsavebtn.value=saveastitle;f." + context.GetRequestVal("IdCol") + ".value='-1';" +
                    Environment.NewLine + "copytonew = true;}");
            
            if (IsInsert || IsUpdate)
            {
                r1.Append(Environment.NewLine + "function myCheckRequire(f, a){" +
                    Environment.NewLine + "	var kt = false;" +
                    Environment.NewLine + "	if (a != ''){" +
                    Environment.NewLine + "		var input = f.elements[a];" +
                    Environment.NewLine + "		if (input){" +
                    Environment.NewLine + "			kt = true;" +
                    Environment.NewLine + "			if (input.type == \"select-one\" || input.type == 'select-multiple'){" +
                    Environment.NewLine + "				if(input.selectedIndex > -1 && input.value != '' && input.value != '-1') kt = false;" +
                    Environment.NewLine + "			} else if (input.length){" +
                    Environment.NewLine + "				for(var i = 0; i < input.length; i++){" +
                    Environment.NewLine + "					if (input[i].type == \"checkbox\" || input[i].type == \"radio\"){" +
                    Environment.NewLine + "						if(input[i].checked == true) kt = false;" +
                    Environment.NewLine + "					} else if (input[i].type == \"select-one\" || input.type == 'select-multiple'){" +
                    Environment.NewLine + "						if(input[i].selectedIndex > -1 && input[i].value != '' && input[i].value != '-1') kt = false;" +
                    Environment.NewLine + "					} else {" +
                    Environment.NewLine + "						if(input[i].value != '') kt = false;" +
                    Environment.NewLine + "					}" +
                    Environment.NewLine + "				}" +
                    Environment.NewLine + "			}" +
                    Environment.NewLine + "			else {" +
                    Environment.NewLine + "				if (input.type == \"checkbox\" || input.type == \"radio\"){" +
                    Environment.NewLine + "					if(input.checked == true) kt = false;" +
                    Environment.NewLine + "				} else if (input.value){" +
                    Environment.NewLine + "					if(input.value != \"\" && input.value != '-1') kt = false;" +
                    Environment.NewLine + "				}" +
                    Environment.NewLine + "			}" +
                    Environment.NewLine + "		}" +
                    Environment.NewLine + "	}" +
                    Environment.NewLine + "	return kt;" +
                    Environment.NewLine + "}");
                r1.Append(Environment.NewLine + "function DateParse(a){" +
                    Environment.NewLine + "	var d; var b = a.split('/');if(b.length < 3) d=Date(); else d = new Date(b[1] + '/' + b[0] + '/' + b[2]);" +
                    Environment.NewLine + "	return d;" +
                    Environment.NewLine + "}");
                r1.Append(Environment.NewLine + "function myCheckDataValid(f, a, n){" +
                    Environment.NewLine + "	var kt = true;" +
                    Environment.NewLine + "	if (a != ''){" +
                    Environment.NewLine + "		var b = a.split('|');" +
                    Environment.NewLine + "		if (b[1] == 'phoneValid') kt = phoneValid(f.elements[b[0]]);" +
                    Environment.NewLine + "		if (b[1] == 'emailValid') kt = emailValid(f.elements[b[0]]);" +
                    Environment.NewLine + "		if (b[1] == 'numericValid') kt = numericValid(f.elements[b[0]]);" +
                    Environment.NewLine + "		if (b[1] == 'timeValid') kt = timeValid(f.elements[b[0]]);" +                    
                    Environment.NewLine + "		if (b[1] == 'vietDateValid') {" +
                    Environment.NewLine + " 		kt = vietDateValid(f.elements[b[0]]);" +
                    Environment.NewLine + " 		if (kt) {" +
                    Environment.NewLine + "				var d = DateParse(f.elements[b[0]].value);" +
                    Environment.NewLine + " 			var sd1 = '';" +
                    Environment.NewLine + " 			if (b[2]!='' && f.elements[b[0]].value != ''){" +
                    Environment.NewLine + " 				sd1 = b[2];" +
                    Environment.NewLine + " 				var d1 = DateParse(sd1);" +
                    Environment.NewLine + " 				if (d < d1){" +
                    Environment.NewLine + " 					kt = false;" +
                    Environment.NewLine + " 					JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Column") + " [' + n + '] > ' + sd1, '', '0');try {document.bosfrm.elements[b[0]].focus()}catch(ex){}" +
                    Environment.NewLine + " 				}" +
                    Environment.NewLine + " 			}" +
                    Environment.NewLine + " 			if (b[3]!='' && f.elements[b[0]].value != ''){" +
                    Environment.NewLine + " 				sd1 = b[3];" +
                    Environment.NewLine + " 				var d1 = DateParse(sd1);" +
                    Environment.NewLine + " 				if (d > d1){" +
                    Environment.NewLine + " 					kt = false;" +
                    Environment.NewLine + " 					JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Column") + " [' + n + '] < ' + sd1, '', '0');try {document.bosfrm.elements[b[0]].focus()}catch(ex){}" +
                    Environment.NewLine + " 				}" +
                    Environment.NewLine + " 			}" +
                    Environment.NewLine + " 		}" +
                    Environment.NewLine + "		}" +
                    Environment.NewLine + "		if (b[1] == 'vietDateTimeValid') {" +
                    Environment.NewLine + " 		kt = vietDateTimeValid(f.elements[b[0]]);" +
                    Environment.NewLine + " 		if (kt) {" +
                    Environment.NewLine + "				var d = DateParse(f.elements[b[0]].value);" +
                    Environment.NewLine + " 			var sd1 = '';" +
                    Environment.NewLine + " 			if (b[2]!='' && f.elements[b[0]].value != ''){" +
                    Environment.NewLine + " 				sd1 = b[2];" +
                    Environment.NewLine + " 				var d1 = DateParse(sd1);" +
                    Environment.NewLine + " 				if (d < d1){" +
                    Environment.NewLine + " 					kt = false;" +
                    Environment.NewLine + " 					JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Column") + " [' + n + '] > ' + sd1, '', '0');try {document.bosfrm.elements[b[0]].focus()}catch(ex){}" +
                    Environment.NewLine + " 				}" +
                    Environment.NewLine + " 			}" +
                    Environment.NewLine + " 			if (b[3]!='' && f.elements[b[0]].value != ''){" +
                    Environment.NewLine + " 				sd1 = b[3];" +
                    Environment.NewLine + " 				var d1 = DateParse(sd1);" +
                    Environment.NewLine + " 				if (d > d1){" +
                    Environment.NewLine + " 					kt = false;" +
                    Environment.NewLine + " 					JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Column") + " [' + n + '] < ' + sd1, '', '0');try {document.bosfrm.elements[b[0]].focus()}catch(ex){}" +
                    Environment.NewLine + " 				}" +
                    Environment.NewLine + " 			}" +
                    Environment.NewLine + " 		}" +
                    Environment.NewLine + "		}" +
                    Environment.NewLine + "	}" +
                    Environment.NewLine + "	return kt;" +
                    Environment.NewLine + "}");
                r1.Append(Environment.NewLine + "function mySubmit(f, isNew){" +
                    Environment.NewLine + "if (isNew == 1) {" +
                    Environment.NewLine + "copytonew = true;" +
                    Environment.NewLine + "f.CopyID.value=" + context.GetRequestVal(context.GetRequestVal("IdCol")) + ";" +
                    Environment.NewLine + "f." + context.GetRequestVal("IdCol") + ".value='-1';" +
                    Environment.NewLine + "myCopy(f);" +
                    Environment.NewLine + "} else {" +
                    Environment.NewLine + "copytonew = false;" +
                    Environment.NewLine + "f.CopyID.value=0;" +
                    Environment.NewLine + "f." + context.GetRequestVal("IdCol") + ".value='" + context.GetRequestVal(context.GetRequestVal("IdCol")) + "';" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "if (!mySubmit1){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Disabled") + "', '', '0');" +
                    Environment.NewLine + "//alert('" + context.GetLanguageLable("Disabled") + "!');" +
                    Environment.NewLine + "return false;}" +
                    Environment.NewLine + "var i=0;" +
                    Environment.NewLine + "while (i<jsdbcolsReadOnly.length){" +
                    Environment.NewLine + "if(jsdbcolsReadOnly[i] != ''){" +
                    Environment.NewLine + "var a = document.bosfrm.elements[jsdbcolsReadOnly[i]];" +
                    Environment.NewLine + "if(a.disabled==true)a.disabled=false; " +
                    Environment.NewLine + "/*alert(a.value);*/}" +
                    Environment.NewLine + "i++;}" +
                    Environment.NewLine + "i=0;var msg='';" +
                    Environment.NewLine + "var IsRequire = false;" +
                    Environment.NewLine + "while (i<jsdbcolsRequire.length && !IsRequire){" +
                    Environment.NewLine + "if(myCheckRequire(f, jsdbcolsRequire[i])){" +
                    Environment.NewLine + "     msg=msg+'\\n" + context.GetLanguageLable("Column") + " [' + jsdbcolsName[i] + '] " + context.GetLanguageLable("IsNull") + "!';" +
                    Environment.NewLine + "     try{f.elements[jsdbcolsRequire[i]].focus();}catch(ex){try{f.elements[jsdbcolsRequire[i]][0].focus();}catch(ex){}}" +
                    //Environment.NewLine + "     //if (isfirefox) {" +
                    //Environment.NewLine + "     //     fe[jsdbcols[i]].style.borderColor='red';" +
                    //Environment.NewLine + "     //}" +
                    Environment.NewLine + "     IsRequire = true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + " if (!myCheckDataValid(f,jsdbcolsDataReq[i],jsdbcolsName[i])) return;" +
                    Environment.NewLine + "i++;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "if (IsRequire){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', msg, '', '0'); " +
                    Environment.NewLine + "return;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "Saving(true);" +
                    Environment.NewLine + "f.submit();" +
                    Environment.NewLine + "}");
            }

            r1.Append(Environment.NewLine + "</script>");
            r = r1.ToString(); r1 = null;
            return r;
        }
        public string UIEditChild(bool IsTabMenu = true)
        {
            string ChildName = Tools.GetDataJson(BTab.DBConfig.Items[0], "ChildName");
            string ChildURL = Tools.GetDataJson(BTab.DBConfig.Items[0], "ChildURL");
            return UIDef.UITabForm(context, ChildName, ChildURL);
        }
        public string UIActionForm ()
        {
            StringBuilder b = new StringBuilder(); string r = "";
            StringBuilder jsdbcols; jsdbcols = new StringBuilder();
            string ColumnID = Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumn");
            string DeleteColumnName = Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumnName");
            string DeleteColumnLable = Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumnLable");
            string DeleteColumnType = Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumnType");
            string DeleteColumnInline = Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumnInline");

            string[] a1 = DeleteColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] a2 = DeleteColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] a3 = DeleteColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] a4 = DeleteColumnInline.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataVal = DeleteColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataTxt = DeleteColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] DataParent = DeleteColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
            string iframe = context.GetRequestVal("iframe");

            jsdbcols.Append("var jsdbcols = new Array();" +
                    Environment.NewLine + "var DataVal = new Array();" +
                    Environment.NewLine + "var DataTxt = new Array();" +
                    Environment.NewLine + "var DataParent = new Array();");

            b.Append(Environment.NewLine + "<form target=\"saveTranFrm" + iframe + "\" onsubmit=\"return false\" name=\"ActionForm\" method=\"POST\">" +
                UIDef.UIHidden("SysUID", context.GetRequestVal("SysUID")) + // ẩn dữ liệu chọn //GetQueryString GetUrl
                UIDef.UIHidden("GetUrl", Compress.Zip(context.GetRequestVal("GetUrl"))) +
                UIDef.UIHidden("GetQueryString", Compress.Zip(context.GetRequestVal("GetQueryString"))) +
                UIDef.UIHidden("MenuOn", MenuOn) +
                UIDef.UIHidden("iframe", iframe) +
                UIDef.UIHidden("MenuID", MenuID) +
                UIDef.UIHidden("TabIndex", TabIndex) +
                Environment.NewLine + "<div id=\"pnlUpload\" class=\"row inline-input\">" +
                        Environment.NewLine + "<div class=\"col-sm-12\"><div class=\"form-group row\">" +
                Environment.NewLine + UIDef.UIButton("bntAction", context.GetLanguageLable("Action"), "btnAction(this.form)", " class=\"btn save\" ") +
                UIDef.UIButton("bntSearchReset", context.GetLanguageLable("Reset"), false, " class=\"btn refresh\"") +
                "</div>");

            for (var i = 0; i < a1.Length; i++)
            {
                DataVal[i] = ""; DataTxt[i] = ""; DataParent[i] = "";
                string[] a31 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None);
                // Get Form/QueryString value OR default Input
                string v = context.GetRequestVal(a1[i]);
                if (v == "") v = a31[1];
                //if (v.Length > 7) if (v.Substring(0, 7) == "REQUEST") v = context.GetRequestVal(v.Substring(7));
                v = Tools.ParseValue(context, v, false);
                string val = v;
                if (a31[0] == "Hidden") // Type is Hidden
                {
                    val = context.ReplaceSessionValue(v); // Replace Session Param
                    b.Append(UIDef.UIHidden(a1[i], val));
                }
                else
                {
                    b.Append(Environment.NewLine + "<div class=\"form-group row\"><label class=\"col-form-label active\">" + context.GetLanguageLable(a2[i]) + ": </label>");
                    string ReqJson = "";
                    switch (a31[0])
                    {
                        case "Textbox":
                            b.Append(UIFormElements.UITextbox(context.GetLanguageLable(a2[i]), a1, i, val, a31, context.GetLanguageLable(a2[i])));
                            break;
                        case "Numeric":
                            b.Append(UIFormElements.UINumeric(context, context.GetLanguageLable(a2[i]), a1, a2, i, val, a31));
                            break;
                        case "Date":
                            b.Append(UIFormElements.UIDate(context.GetLanguageLable(a2[i]), a1, i, val, "ActionForm"));
                            break;
                        case "Datetime":
                            b.Append(UIFormElements.UIDateTime(context.GetLanguageLable(a2[i]), a1, i, val, "ActionForm"));
                            break;
                        case "Checkbox":
                            b.Append(UIDef.UICheckbox(a1[i], a31[3], a31[4], val, UIDef.FocusAfter(a1[i])));
                            break;
                        //case "MutilCheckbox":
                        //    ReqJson = context.InputDataSetParam(a31[4]);
                        //    b.Append(UIDef.UIDD(a1[i], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a1[i]), "", "", int.Parse(a31[6]), int.Parse(a31[7])));
                        //    break;
                        case "Radio":
                            //b.Append(UIDef.UIRadio(a1[i], context.ReplaceStringLangValue(a31[3]), a31[4], val, UIDef.FocusAfter(a1[i])));
                            b.Append(UIFormElements.UIRadio(context, a1, i, val, a31));
                            break;
                        case "Actb":
                            ReqJson = context.InputDataSetParam(a31[6]);
                            b.Append(UIDef.UIActbId(a1[i], ref DataDAO, a31[5], ReqJson, a31[7], val, " size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a1[i]), "ActionForm"));
                            break;
                        case "ActbText":
                            b.Append(UIDef.UIActbStrId(a1[i], context.ReplaceStringLangValue(a31[5]), a31[6], val, " size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a1[i]), "ActionForm"));
                            break;
                        case "Selectbox":
                            ReqJson = context.InputDataSetParam(a31[4]);
                            UIDef.OptionStringVal(ref DataDAO, a31[3], ReqJson, a31[5], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                            b.Append(UIFormElements.UISelectStr(a1[i], DataVal[i], DataTxt[i], a31, ref val, true, "", i));
                            ////if (a31.Length > 7)
                            ////    b.Append(UIDef.UISelectStr(a1[i], DataVal[i], DataTxt[i], ref val, true, "", (a31[6] != ""? a31[6] + "," + (i + 1) + ");" : "") + UIDef.NextFocus(a1[i]), (a31[7]=="1")));
                            ////else if (a31.Length > 6)
                            ////    b.Append(UIDef.UISelectStr(a1[i], DataVal[i], DataTxt[i], ref val, true, "", (a31[6] != "" ? a31[6] + "," + (i + 1) + ");" : "") + UIDef.NextFocus(a1[i]), "document.ActionForm"));
                            ////else
                            ////    b.Append(UIDef.UISelectStr(a1[i], DataVal[i], DataTxt[i], ref val, true, "", UIDef.NextFocus(a1[i]), "document.ActionForm"));
                            //b.Append(UIDef.UISelect(a1[i], ref DataDAO, a31[3], ReqJson, a31[5], val, true, "", UIDef.CNextFocus(a1[i])));
                            break;
                        case "SelectboxText":
                            b.Append(UIFormElements.UISelectStr(a1[i], a31[3], context.ReplaceStringLangValue(a31[4]), a31, ref val, true, "", i, 6));
                            ////if (a31.Length > 6)
                            ////    b.Append(UIDef.UISelectStr(a1[i], a31[3], context.ReplaceStringLangValue(a31[4]), ref val, true, "", UIDef.NextFocus(a1[i]), (a31[6] == "1")));
                            ////else if (a31.Length > 5)
                            ////    b.Append(UIDef.UISelectStr(a1[i], a31[3], context.ReplaceStringLangValue(a31[4]), ref val, true, "", (a31[5] != "" ? a31[5] + "," + (i + 1) + ");" : "") + UIDef.NextFocus(a1[i]), "document.ActionForm"));
                            ////else
                            ////    b.Append(UIDef.UISelectStr(a1[i], context.ReplaceStringLangValue(a31[3]), a31[4], ref val, true, "", UIDef.NextFocus(a1[i]), "document.ActionForm"));
                            break;
                        case "SearchBox":
                            b.Append(UIFormElements.UISearchBox(context.GetLanguageLable(a2[i]), a1, i, "", a3, a31, " readOnly class='ironly' ", ref jsdbcols, context, null, false, "ActionForm"));
                            break;
                    }
                    b.Append(Environment.NewLine + "</div>");
                }
                jsdbcols.Append("jsdbcols[" + i + "] = '" + a1[i] + "';" +
                    Environment.NewLine + "DataVal[" + i + "] = '" + DataVal[i] + "';" +
                    Environment.NewLine + "DataTxt[" + i + "] = '" + DataTxt[i] + "';" +
                    Environment.NewLine + "DataParent[" + i + "] = '" + DataParent[i] + "';");
            }
            //b.Append(Environment.NewLine + "<tr><td><td>&nbsp;" + UIDef.UIButton("bntAction", context.GetLanguageLable("Action"), "btnAction(this.form)") +
            //    UIDef.UIButton("bntSearchReset", context.GetLanguageLable("Reset"), false, " class=\"btn refresh\"") +
            //    "<td>");
            b.Append(Environment.NewLine + "</div>");
            b.Append(Environment.NewLine + "</div>");
            b.Append(Environment.NewLine + "</form>");
            b.Append(Environment.NewLine + "<!--JS-->" +
                     Environment.NewLine + jsdbcols.ToString());
            b.Append(Environment.NewLine + "function btnAction (frm){" +
                     Environment.NewLine + "frm.action='/Basetab/Delete';" +
                     Environment.NewLine + "frm.submit();" +
                     Environment.NewLine + "}");
            //b.Append(Environment.NewLine + "<!--/JS-->");
            r = b.ToString(); b = null;
            return r;
        }
        public string UIPrintForm()
        {
            StringBuilder b = new StringBuilder(); string r = "";
            StringBuilder jsdbcols; jsdbcols = new StringBuilder();
            string StoreGetPrintFormOption = Tools.GetDataJson(BTab.DBConfig.Items[0], "StoreGetPrintFormOption");
            string ColumnID = Tools.GetDataJson(BTab.DBConfig.Items[0], "DeleteColumn");

            b.Append(Environment.NewLine + "<form onsubmit=\"return false\" name=\"PrintForm\" method=\"POST\">" +
                UIDef.UIHidden("ColumnID", ColumnID) +
                UIDef.UIHidden(ColumnID, context.GetRequestVal(ColumnID)) +
                UIDef.UIHidden("TabIndex", TabIndex) +
                Environment.NewLine + "<div id=\"pnlUpload\" class=\"row inline-input\">" +
                        Environment.NewLine + "<div class=\"col-sm-12\"><div class=\"form-group row\">");
            b.Append(Environment.NewLine + UIDef.UIButton("bntAction", context.GetLanguageLable("Print"), "btnPrint(this.form)", " class=\"btn print\"") + "</div>");
            b.Append(Environment.NewLine + "<div class=\"form-group row\"><label class=\"col-form-label active\">" + context.GetLanguageLable("FormOption") + "</label>");
            // Start -> StoreGetPrintForm
            string[] a31 = StoreGetPrintFormOption.Split(new string[] { ";" }, StringSplitOptions.None);
            string val = "";
            if (a31[0] == "Hidden")
            {
                val = a31[1];
                val = context.ReplaceSessionValue(val);
                b.Append(UIDef.UIHidden("FormID", val));
            }
            else
            {
                switch (a31[0])
                {
                    case "Selectbox":
                        string ReqJson = context.InputDataSetParam(a31[4]); string DataVal = ""; string DataTxt = ""; string DataParent = "";
                        UIDef.OptionStringVal(ref DataDAO, a31[3], ReqJson, a31[5], ref DataVal, ref DataTxt, ref DataParent);
                        if (a31.Length > 6)
                            b.Append(UIDef.UISelectStr("FormID", DataVal, DataTxt, ref a31[1], true, "", a31[6], "document.PrintForm"));
                        else
                            b.Append(UIDef.UISelectStr("FormID", DataVal, DataTxt, ref a31[1], true, "", "", "document.PrintForm"));
                        break;
                    case "SelectboxText":
                        string Onchange = "";
                        if (a31.Length > 5) Onchange = a31[5];
                        b.Append(UIDef.UISelectStr("FormID", a31[3], a31[4], ref val, true, "", Onchange));
                        break;
                }
                
            }
            // End -> StoreGetPrintForm
            b.Append(Environment.NewLine + "</div></div></div>");
            b.Append(Environment.NewLine + "</form>");
            b.Append(Environment.NewLine + "<!--JS-->" +
                     Environment.NewLine + jsdbcols.ToString());
            b.Append(Environment.NewLine + "function btnPrint (frm){" +
                     Environment.NewLine + "frm.action='/Basetab/PrintOption';" +
                     Environment.NewLine + "frm.submit();" +
                     Environment.NewLine + "/*$('.close').click();*/" +
                     Environment.NewLine + "}");
            //b.Append(Environment.NewLine + "<!--/JS-->");
            r = b.ToString(); b = null;
            return r;
        }
        #endregion
    }
}
/*
        public string UIListFormLeft()
        {
            string UrlEdittab = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "urlEdit"); UrlEdittab = context.ReplaceRequestValue(UrlEdittab);
            string IndexTab = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "DBConfigCode");
            StringBuilder r1 = new StringBuilder(); string r = ""; dynamic BTabUrlList = null;
            string UrlList = context.GetSession("BTabUrlList_" + IndexTab); if(UrlList != null && UrlList != "") BTabUrlList = JObject.Parse(UrlList);
            if (BTabUrlList == null)
            {
                r1.Append(Environment.NewLine + "<table width=\"100%\"><tbody><tr style=\"vertical-align:top\">" +
                    Environment.NewLine + "<!-- Left panel ------------------------><td id=\"printhide3\">" +
                    Environment.NewLine + "<td width=\"99%\">" +
                    Environment.NewLine + " <!-- Right panel ---------------------------------------->");
            }
            else
            {
                r1.Append(Environment.NewLine + "<table width=\"100%\"><tbody><tr style=\"vertical-align:top\">" +
                    Environment.NewLine + "<!-- Left panel ------------------------><td width=\"19%\" id=\"printhide3\">" +
                    Environment.NewLine + "<table class=\"editleftpaneltab\" cellpadding=\"2\">" +
                    Environment.NewLine + "<tbody>");
                for (int i = 0; i < BTabUrlList.UrlList.Count; i++)
                {
                    r1.Append(Environment.NewLine + "<tr style=\"vertical-align:top\">" +
                        Environment.NewLine + "<td><a href=\"javascript:" + BTabUrlList.UrlList[i].Url + "\">" + BTabUrlList.UrlList[i].UrlLable + "</a></td><td>" + BTabUrlList.UrlList[i].UrlName + "</td></tr>");
                }                
                r1.Append(Environment.NewLine + "<script language=\"javascript\">function onEdit(Id){location.href=\"" + UrlEdittab + "\" + Id;}" +
                    Environment.NewLine + "function onEditOther(Id, Url) {location.href= Url + \"=\" + Id;}</script>" +
                    Environment.NewLine + "</tbody></table></td>");
                r1.Append(Environment.NewLine + "<td width=\"81%\">" +
                    Environment.NewLine + " <!-- Right panel ---------------------------------------->");
            }
            r = r1.ToString();
            r1 = null;
            return r;
        }
        public string UIListFormLeftClose()
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append(Environment.NewLine + "</tr> </tbody></table>");
            r = r1.ToString();
            r1 = null;
            return r;
        }
        */
/*
        public string ParseValue (string InputType = "", string val = "")
        {
            switch (InputType)
            {
                case "Datetime":
                    DateTime dNow1 = DateTime.Now; int iDay1 = 0;
                    if (val == "@" || val == "")
                    {
                        val = string.Format("{0:M/d/yyyy HH:mm}", dNow1);
                    }
                    else if (val.Length > 2 && val.Substring(0, 1) == "@")
                    {
                        if (val.Substring(0, 2) == "@+")
                        {
                            iDay1 = int.Parse(Tools.Right(val, 2, false));
                            val = string.Format("{0:M/d/yyyy HH:mm}", dNow1.AddDays(iDay1));
                        }
                        else if (val.Substring(0, 2) == "@-")
                        {
                            iDay1 = 0 - int.Parse(Tools.Right(val, 2, false));
                            val = string.Format("{0:M/d/yyyy HH:mm}", dNow1.AddDays(iDay1));
                        }                        
                    }
                    else val = Tools.SwapDate(val);
                    val = val + ":00";
                    break;
                case "Date":
                    DateTime dNow = DateTime.Now; int iDay = 0;
                    if (val == "@" || val == "")
                    {
                        val = string.Format("{0:M/d/yyyy}", dNow);
                    }
                    else if (val.Length > 2 && val.Substring(0, 1) == "@")
                    {
                        if (val.Substring(0, 2) == "@+")
                        {
                            iDay = int.Parse(Tools.Right(val, 2, false));
                            val = string.Format("{0:M/d/yyyy}", dNow.AddDays(iDay));
                        }
                        else if (val.Substring(0, 2) == "@-")
                        {
                            iDay = 0 - int.Parse(Tools.Right(val, 2, false));
                            val = string.Format("{0:M/d/yyyy}", dNow.AddDays(iDay));
                        }                        
                    }
                    else val = Tools.SwapDate(val);
                    break;
            }
            return val;
        }

        private void ExecParam(string FunctionName, string QueryStringFilter, string ParamList, string ParamDefault, string QueryStringType,
            ref string SPName, ref ToolDAO DataDAO, ref string parameterOutput, ref string json, ref int errorCode, ref string errorString)
        {
            string[] a = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] b = ParamList.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] c = ParamDefault.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] ac = QueryStringType.Split(new string[] { "^" }, StringSplitOptions.None);

            dynamic d = null; 
            if (ParamList != "")
            {
                for (int i = 0; i < b.Length; i++)
                {
                    string[] b1 = b[i].Split(new string[] { ";" }, StringSplitOptions.None);
                    if (i < a.Length)
                    {
                        string val = ""; // Khoi tao
                        val = context.GetRequestVal(a[i]); // lay tu request
                        if (i < c.Length && val == "") val = c[i]; // lay default
                        if (val == "") val = b1[4]; // lay default
                        val = Tools.ParseValue(ac[i].Substring(0, ac[i].IndexOf(";")), val);
                        json = json + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":\"" + val + "\"}";
                    }
                    else
                        json = json + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":\"" + b1[4] + "\"}";
                }
            }
            if (json != "")
            {
                json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                d = JObject.Parse(json);
            }
            DataDAO.ExecuteStore(FunctionName, SPName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
        }*/
