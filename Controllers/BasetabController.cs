using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Utils;
using Newtonsoft.Json.Linq;
//using HRS.Models;
//using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
//using System.IO.Compression;
//using System.IO;

namespace HRScripting.Controllers
{
    public class BasetabController : Controller
    {
        private HRSCache _cache;
        public BasetabController(IMemoryCache memoryCache)
        {
            _cache = new HRSCache(memoryCache);
        }
        public IActionResult Index()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                StringBuilder r1 = new StringBuilder(); string r = "";
                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn));
                r1.Append(UIDef.UIContentTagOpen (ref bosDAO, _context,MenuOn, "0", false));
                r1.Append(_context.GetLanguageLable("YouAreHomePage"));
                r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                //r1.Append(UIDef.UIFooter());
                r = r1.ToString();
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, _context.GetRequestVal("MenuID"));
                ViewData["IsPageLogin"] = !MenuOn;
                ViewData["Secure"] = (_context.GetRequestMenuOn()=="Min"?" sidebar-collapse":"");
                ViewData["IndexBody"] = r;
                ViewData["PageTitle"] = _context.GetLanguageLable("HomePageTitle");
                ViewData["iframe"] = _context.GetRequestVal("iframe");
                ViewData["txtClose"] = _context.GetLanguageLable("Close");
                r1 = null;
                return View();
            }
        }
        [HttpPost]
        public IActionResult SaveDBTable()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            StringBuilder r1 = new StringBuilder(); string r = "";
            string GetUrl = _context.GetRequestVal("GetUrl");
            string GetQueryString = _context.GetRequestVal("GetQueryString");
            GetUrl = Tools.UrlDecode(GetUrl);
            GetQueryString = Tools.UrlDecode(GetQueryString);
            GetQueryString = _context.ReplaceStr(GetQueryString);
            string url = GetUrl + "?" + GetQueryString;
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS

                string TabIndex = _context.GetRequestTabIndex();
                if (!(TabIndex.IndexOf(",") < 0)) TabIndex = TabIndex.Substring(0, TabIndex.IndexOf(","));
                HTTP_CODE.WriteLogAction("functionName:/Basetab/SaveDBTable\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    //return Redirect(_context.ReturnUrlLogin(l, false));
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + l);
                    r = l;
                    r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                    //r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"/\", \"0\");");
                    r1.Append(Environment.NewLine + "parent.location.href = '/Home/Login?Message=" + Compress.Zip(r) + "';");
                    r1.Append(Environment.NewLine + "</script>");
                }
                else
                {
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO); string[,] data;
                    string FormEditType = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormEditType");
                    string IsSysAdmin = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "IsSysAdmin");
                    // check quyen                
                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = false;
                    _context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType, IsSysAdmin);

                    if (!(IsInsert || IsUpdate))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex));
                        r = _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex);
                        r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                        r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"/\", \"0\");");
                        r1.Append(Environment.NewLine + "</script>");
                        //return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }
                    else
                    {
                        bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                        dynamic d = null;
                        string ParamUpdate = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "paramUpdate");
                        string[] Params = ParamUpdate.Split(new string[] { "^" }, StringSplitOptions.None);
                        int Cnt = int.Parse((_context.GetFormValue("ItemsCnt") == "" ? "0" : _context.GetFormValue("ItemsCnt")));

                        string[] jsdbcols = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnName").Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] ColumnType = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnType").Split(new string[] { "^" }, StringSplitOptions.None);

                        //string IdCol = Params[1].Substring(0, Params[1].IndexOf(";"));
                        //IdCol = _context.GetRequestVal(IdCol);
                        string UrlDbtab = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "urlList");
                        string Creator = _context.GetSession("UserID");
                        string SPUpdate = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "spUpdateName");

                        string json = "";

                        //string frmVal = _context.GetFormValue("Changed");
                        //string[] Changed = frmVal.Split(new string[] { "," }, StringSplitOptions.None);
                        //string[] IdCols = IdCol.Split(new string[] { "," }, StringSplitOptions.None);
                        //string[] SysUID = _context.GetRequestVal("SysUID").Split(new string[] { "," }, StringSplitOptions.None);
                        string frmVal = ""; string Changed = ""; string SysUID = "";
                        data = new string[Cnt, Params.Length];
                        for (int i = 0; i < Params.Length; i++)
                        {
                            string[] param = Params[i].Split(new string[] { ";" }, StringSplitOptions.None);
                            //string frmInput = param[0];
                            //frmVal = _context.GetFormValue(frmInput);
                            //string [] dataRequest = frmVal.Split(new string[] { "," }, StringSplitOptions.None);
                            for (int j = 0; j < Cnt; j++)
                            {
                                string val = "";
                                frmVal = _context.GetFormValue(param[0] + j);
                                if (param[0] == "Creator")
                                    val = Creator;
                                else
                                {
                                    val = frmVal;
                                    int j12 = Tools.GetArrayPos(param[0], jsdbcols);                                    
                                    if (j12 > -1)
                                    {
                                        string[] IColumnType = ColumnType[j12].Split(new string[] { ";" }, StringSplitOptions.None);
                                        switch (IColumnType[0].ToLower())
                                        {
                                            case "numeric": // Param Numeric
                                                if (val == "")
                                                    val = "0";
                                                else
                                                    val = Tools.RemNumSepr(val);
                                                break;
                                            case "date":
                                                if (val == "")
                                                    val = DateTime.Now.ToString("MM/dd/yyyy");
                                                else
                                                    val = Tools.SwapDate(val);
                                                break;
                                            case "datetime":
                                                if (val == "")
                                                    val = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                                                else
                                                    val = Tools.SwapDate(val + ":00");
                                                break;
                                            case "algorithm":
                                                if (val != "") val = _context.enc.Encrypt(val);
                                                break;
                                        }
                                    }
                                    if (val == "") val = param[4]; // default from param
                                    /* // Code cũ check theo param ko hết các kiểu input
                                    if (val == "") //val = null;
                                    {
                                        if (toolDAO.CheckParam(param[1])) val = "0";
                                        else if (toolDAO.CheckParam(param[1], "Date")) val = DateTime.Now.ToString("MM /dd/yyyy HH:mm:ss");
                                        else if (toolDAO.CheckParam(param[1], "Time")) val = DateTime.Now.ToString("HH:mm:ss");
                                    }
                                    else if (toolDAO.CheckParam(param[1], "Date")) val = Tools.SwapDate(val);
                                    else if (toolDAO.CheckParam(param[1]))
                                    {
                                        int iActb = val.IndexOf(Tools.C_ActbSepr);
                                        if (iActb > -1) val = val.Substring(0, iActb - 1);
                                        val = Tools.RemNumSepr(val);
                                    }
                                    */
                                }
                                data[j, i] = val;
                            }
                        }
                        //r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                        //r1.Append(Environment.NewLine + "alert ('");
                        int CntSuccess = 0; int CntError = 0; string msg = "";
                        for (int j = 0; j < Cnt; j++)
                        {
                            Changed = _context.GetFormValue("Changed" + j);
                            SysUID = _context.GetFormValue("SysUID" + j);

                            if (Changed == "1") // Có thay đổi
                            {
                                // Dong j
                                //r1.Append("\\n" + _context.GetLanguageLable("UpdateRow") + " " + (j + 1) + "....");
                                msg = msg + "<br>" + _context.GetLanguageLable("UpdateRow") + " " + (j + 1) + "....";
                                if (_context.CheckEditDeleteRoles(SysUID))
                                {
                                    json = "";
                                    for (int i = 0; i < Params.Length; i++)
                                    {
                                        string[] b1 = Params[i].Split(new string[] { ";" }, StringSplitOptions.None);
                                        string val = data[j, i];
                                        json = json + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":" + (val == null ? "null" : "\"" + val + "\"") + "}";
                                    }
                                    if (json != "")
                                    {
                                        json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                                        d = JObject.Parse(json);
                                    }
                                    string parameterOutput = ""; int errorCode = 0; string errorString = "";
                                    toolDAO.ExecuteStore("Update", SPUpdate, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                                    d = JObject.Parse("{" + parameterOutput + "}");
                                    //r1.Append(" ==> " + _context.GetLanguageLable(d.ParameterOutput.Message.ToString()) + ". " + _context.GetLanguageLable("End"));
                                    if (errorCode == 500)
                                    {
                                        CntError = CntError + 1;
                                        msg = msg + " ==> " + errorString;// + ". " + _context.GetLanguageLable("End");
                                    }
                                    else
                                    {
                                        if ((long)d.ParameterOutput.ResponseStatus > 0)
                                            CntSuccess = CntSuccess + 1;
                                        else
                                            CntError = CntError + 1;

                                        msg = msg + " ==> " + _context.GetLanguageLable(d.ParameterOutput.Message.ToString());// + ". " + _context.GetLanguageLable("End");
                                    }
                                }
                                else
                                {
                                    //r1.Append(" ==> " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + ". " + _context.GetLanguageLable("End"));
                                    msg = msg + " ==> " + _context.GetLanguageLable("YouAreNotIsGrantToFunction");// + ". " + _context.GetLanguageLable("End");
                                    CntError = CntError + 1;
                                }
                            }
                        }

                        //r1.Append("');");

                        if (CntSuccess == 0 && CntError > 0)
                        {
                            //msg = "alert-error^2^" + msg;
                            HTTP_CODE.WriteLogAction("functionName:Basetab/SaveTable\nException" + msg);
                            r = msg;
                            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                            r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"\", \"2\");");
                            r1.Append(Environment.NewLine + "</script>");
                        }
                        else if (CntSuccess > 0 && CntError > 0)
                        {
                            //msg = "alert-warning^2^" + msg;
                            HTTP_CODE.WriteLogAction("functionName:Basetab/SaveTable\nException" + msg);
                            r = msg;
                            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                            r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-warning\", \"" + _context.GetLanguageLable("Alert-Warning") + "\", \"" + r + "\", \"" + GetUrl + "?" + GetQueryString + "\", \"1\");");
                            r1.Append(Environment.NewLine + "</script>");
                        }
                        else
                        {
                            //msg = "alert-success^2^" + msg;
                            HTTP_CODE.WriteLogAction("functionName:Basetab/SaveTable\nException" + msg);
                            r = msg;
                            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                            r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-success\", \"" + _context.GetLanguageLable("Alert-Success") + "\", \"" + r + "\", \"" + GetUrl + "?" + GetQueryString + "\", \"1\");");
                            r1.Append(Environment.NewLine + "</script>");
                        }

                        //return Redirect(url + "&Message=" + Compress.Zip(msg));
                        //r1.Append(Tools.JavaRedirect(msg, url));
                        //r1.Append(Environment.NewLine + "</script>");

                        //r = r1.ToString();
                        //ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                        //ViewData["IndexBody"] = r;
                        //ViewData["PageTitle"] = "";
                        //r1 = null;
                        //return View();
                    }
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                r = _context.RoleDebug(ex.ToString());
                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"\", \"2\");");
                r1.Append(Environment.NewLine + "</script>");
                //return Redirect(url + "&Message=" + Compress.Zip(r));
            }
            r = r1.ToString(); r1 = null;
            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
        }
        [HttpPost]
        public IActionResult Delete()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            StringBuilder r1 = new StringBuilder(); string r = "";
            string GetUrl = Tools.UrlDecode(_context.GetRequestVal("GetUrl"));
            string GetQueryString = _context.GetRequestVal("GetQueryString");
            GetUrl = Tools.UrlDecode(GetUrl);
            GetQueryString = Tools.UrlDecode(GetQueryString);
            GetQueryString = _context.ReplaceStr(GetQueryString);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestTabIndex();
                HTTP_CODE.WriteLogAction("functionName:/Basetab/Delete\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + l);
                    r = l;
                    r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                    //r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"/\", \"0\");");
                    r1.Append(Environment.NewLine + "parent.location.href = '/Home/Login?Message=" + Compress.Zip(r) + "';");
                    r1.Append(Environment.NewLine + "</script>");
                    //return Redirect(_context.ReturnUrlLogin(l, false));
                }
                else
                {
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    // check quyen                
                    bool IsDelete = _context.CheckPermistion("IsDelete", 0, 0, TabIndex);
                    bool IsInsert = false;
                    bool IsUpdate = false;
                    string FormEditType = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormEditType");
                    string IsSysAdmin = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "IsSysAdmin");

                    string ActionType = _context.GetRequestVal("Type");
                    bool IsApproved = _context.CheckPermistion("IsApproved", 0, 0, TabIndex); ;
                    bool IsPublish = _context.CheckPermistion("IsPublish", 0, 0, TabIndex); ;

                    _context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType, IsSysAdmin, ActionType, IsPublish, IsApproved);
                    if (!IsDelete)
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex));
                        r = _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex);
                        r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                        r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"/\", \"0\");");
                        r1.Append(Environment.NewLine + "</script>");
                        //return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }
                    else
                    {
                        dynamic d = null;
                        string ColumnID = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "DeleteColumn");
                        string Creator = _context.GetSession("UserID");

                        string UrlDbtab = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "urlList");
                        string[] SysUID = _context.GetRequestVal("SysUID").Split(new string[] { "," }, StringSplitOptions.None);
                        string[] ColumnIDReq = _context.GetRequestVal(ColumnID).Split(new string[] { "," }, StringSplitOptions.None);
                        string sIDReq = "-1";
                        for (int i = 0; i < ColumnIDReq.Length; i++)
                        {
                            if (_context.CheckEditDeleteRoles(SysUID[i], ColumnIDReq[i])) sIDReq = sIDReq + "," + ColumnIDReq[i];
                        }

                        string[] jsdbcols = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "DeleteColumnName").Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] ColumnType = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "DeleteColumnType").Split(new string[] { "^" }, StringSplitOptions.None);
                        string SPDelete = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "spDeleteName");
                        string ParamDelete = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "paramDelete");
                        string json = "";
                        string[] b = ParamDelete.Split(new string[] { "^" }, StringSplitOptions.None);
                        for (int i = 0; i < b.Length; i++)
                        {
                            string[] b1 = b[i].Split(new string[] { ";" }, StringSplitOptions.None);
                            string val = "";
                            if (b1[0] == ColumnID)
                                val = sIDReq;
                            else
                            if (b1[0] == "Creator")
                                val = Creator;
                            else
                                val = _context.GetFormValue(b1[0]);

                            int j12 = Tools.GetArrayPos(b1[0], jsdbcols);
                            if (j12 > -1)
                            {
                                string[] IColumnType = ColumnType[j12].Split(new string[] { ";" }, StringSplitOptions.None);
                                switch (IColumnType[0].ToLower())
                                {
                                    case "numeric": // Param Numeric
                                        if (val == "")
                                            val = "0";
                                        else
                                            val = Tools.RemNumSepr(val);
                                        break;
                                    case "date":
                                        if (val == "")
                                            val = DateTime.Now.ToString("MM/dd/yyyy");
                                        else
                                            val = Tools.SwapDate(val);
                                        break;
                                    case "datetime":
                                        if (val == "")
                                            val = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                                        else
                                            val = Tools.SwapDate(val + ":00");
                                        break;
                                    case "algorithm":
                                        if (val != "") val = _context.enc.Encrypt(val);
                                        break;
                                }
                            }
                            if (val == "") val = b1[4]; // default from param
                            /*if (val == "") //val = null;
                            {
                                if (toolDAO.CheckParam(b1[1])) val = "0";
                                else if (toolDAO.CheckParam(b1[1], "Date")) val = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                                else if (toolDAO.CheckParam(b1[1], "Time")) val = DateTime.Now.ToString("HH:mm:ss");
                            }
                            else if (toolDAO.CheckParam(b1[1], "Date")) val = Tools.SwapDate(val);
                            else if (toolDAO.CheckParam(b1[1]))
                            {
                                int iActb = val.IndexOf(Tools.C_ActbSepr);
                                if (iActb > -1) val = val.Substring(0, iActb - 1);
                                val = Tools.RemNumSepr(val);
                            }
                            */
                            json = json + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":" + (val == null ? "null" : "\"" + val + "\"") + "}";
                        }
                        if (json != "")
                        {
                            json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                            d = JObject.Parse(json);
                        }
                        string parameterOutput = ""; int errorCode = 0; string errorString = "";
                        toolDAO.ExecuteStore("Delete", SPDelete, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        if (errorCode == 500)
                        {
                            HTTP_CODE.WriteLogAction("functionName:Basetab/Delete\nException" + errorString);
                            r = errorString;
                            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                            r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"\", \"2\");");
                            r1.Append(Environment.NewLine + "</script>");
                            //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(errorString, "Error"));
                        }
                        else
                        {
                            d = JObject.Parse("{" + parameterOutput + "}");
                            if ((long)d.ParameterOutput.ResponseStatus > 0)
                            {
                                HTTP_CODE.WriteLogAction("functionName:Basetab/Delete\nException" + d.ParameterOutput.Message.ToString());
                                r = _context.GetLanguageLable(d.ParameterOutput.Message.ToString());
                                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                                r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-success\", \"" + _context.GetLanguageLable("Alert-Success") + "\", \"" + r + "\", \"" + GetUrl + "?" + GetQueryString + "\", \"1\");");
                                r1.Append(Environment.NewLine + "</script>");
                                //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Success"));
                            }
                            else
                            {
                                HTTP_CODE.WriteLogAction("functionName:Basetab/SaveItem\nException" + d.ParameterOutput.Message.ToString());
                                r = _context.GetLanguageLable(d.ParameterOutput.Message.ToString());
                                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                                r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"\", \"2\");");
                                r1.Append(Environment.NewLine + "</script>");
                                //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Error"));
                            }
                        }
                        //bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                        //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                        //r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                        //r1.Append(Tools.JavaRedirect(_context.GetLanguageLable(d.ParameterOutput.Message.ToString()), GetUrl + "?" + GetQueryString));
                        //r1.Append(Environment.NewLine + "alert ('" + _context.GetLanguageLable(d.ParameterOutput.Message.ToString()) + "');");
                        //r1.Append(Environment.NewLine + "location.href='" + GetUrl + "?" + GetQueryString + "';");
                        //r1.Append(Environment.NewLine + "</script>");
                        //r = r1.ToString();
                        //ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                        //ViewData["IndexBody"] = r;
                        //ViewData["PageTitle"] = "";
                        //r1 = null;
                        //return View();
                    }
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                r = _context.RoleDebug(ex.ToString());
                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"\", \"2\");");
                r1.Append(Environment.NewLine + "</script>");
                //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(r, "Error"));
            }
            r = r1.ToString(); r1 = null;
            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
        }
        [HttpPost]
        public IActionResult SaveItem() //Basetab/SaveItem
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            string GetUrl = _context.GetRequestVal("GetUrl");
            GetUrl = Tools.UrlDecode(GetUrl);
            string GetQueryString = _context.GetRequestVal("GetQueryString");
            GetQueryString = Tools.UrlDecode(GetQueryString);
            StringBuilder r1 = new StringBuilder(); string r = "";
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestTabIndex();
                HTTP_CODE.WriteLogAction("functionName:/Basetab/SaveItem\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + l);
                    r = l;
                    r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                    //r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"/\", \"0\");");
                    r1.Append(Environment.NewLine + "parent.location.href = '/Home/Login?Message=" + Compress.Zip(r) + "';");
                    r1.Append(Environment.NewLine + "</script>");
                    //return Redirect(_context.ReturnUrlLogin(l, false));
                }
                else
                {
                    // check quyen
                    string IdCol = _context.GetRequestVal(_context.GetRequestVal("IdCol")); if (IdCol == "") IdCol = "-1";
                    bool IsEdit = (int.Parse(IdCol) > 0);
                    string SysUID = ""; string Editor = _context.GetSession("UserID");
                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = false;
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    string FormEditType = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormEditType");
                    string IsSysAdmin = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "IsSysAdmin");
                    _context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType, IsSysAdmin);
                    try { SysUID = _context.enc.Decrypt(_context.GetRequestVal("SysUID")); } catch { SysUID = "-1"; }
                    // check role overwrite //804 - Overwrite;805 - Attachments
                    if (!_context.CheckEditDeleteRoles(SysUID) && IsEdit)
                    {
                        IsUpdate = false;
                    }
                    HTTP_CODE.WriteLogAction("Basetab/SaveItem SaveItem: IdCol: " + IdCol + " IsEdit: " + IsEdit + " IsInsert: " + IsUpdate + " IsUpdate: " + IsInsert + " SysUID: " + SysUID + " " + _context.GetLanguageLable(TabIndex));
                    if (!(!IsEdit && IsInsert || IsEdit && IsUpdate))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex));
                        r = _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex);
                        r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                        //r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"/\", \"0\");");
                        r1.Append(Environment.NewLine + "parent.location.href = '/Home?Message=" + _context.ReturnMsg(r, "Error") + "';");
                        r1.Append(Environment.NewLine + "</script>");
                        //return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }
                    else
                    {
                        string EditColumn = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumn");
                        string EditColumnType = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumnType");
                        string EditCacheable = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "Cacheable");
                        for (int i = 1; i < a.BTabGrp.DBConfigGrp.Items.Count; i++)
                        {
                            EditColumn = EditColumn + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[i], "EditColumn");
                            EditColumnType = EditColumnType + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[i], "EditColumnType");
                            EditCacheable = EditCacheable + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[i], "Cacheable");
                        }

                        string[] jsdbcols = EditColumn.Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] ColumnType = EditColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
                        string[] Cacheable = EditCacheable.Split(new string[] { "^" }, StringSplitOptions.None);

                        dynamic d = null;
                        int BackAfterAddnew = int.Parse(Tools.GetDataJson(a.BTab.DBConfig.Items[0], "BackAfterAddnew"));//1; // 1. Addnext; 2. Edit; 3. Dbtab 
                        if (IsEdit) BackAfterAddnew = 3;
                        string UrlEdittab = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "urlEdit"); UrlEdittab = _context.ReplaceRequestValue(UrlEdittab, "");
                        //bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                        /*if (false)
                        {
                            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                            r1.Append(Environment.NewLine + "alert ('" + _context.GetLanguageLable("Debug") + "');");
                            if (BackAfterAddnew == 3)
                                r1.Append(Environment.NewLine + "alert ('" + UrlDbtab + "');location.href='" + UrlDbtab + "';");
                            else if (BackAfterAddnew == 1)
                                r1.Append(Environment.NewLine + "alert ('" + UrlEdittab + "-1');location.href='" + UrlEdittab + "-1&GetUrl=" + GetUrl + "&GetQueryString=" + GetQueryString + "';");
                            else
                                r1.Append(Environment.NewLine + "alert ('" + UrlEdittab + d.ParameterOutput.ResponseStatus.ToString() + "');location.href='" + UrlEdittab + d.ParameterOutput.ResponseStatus.ToString() + "&GetUrl=" + GetUrl + "&GetQueryString=" + GetQueryString + "';");
                            r1.Append(Environment.NewLine + "</script>");
                        }
                        else // GetArrayPos(string val, string[] a)
                        {*/
                        //string EditColumn = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumn");
                        //string EditColumnType = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumnType");
                        //for (int i = 1; i < a.BTabGrp.DBConfigGrp.Items.Count; i++)
                        //{
                        //    EditColumn = EditColumn + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumn");
                        //    EditColumnType = EditColumnType + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumnType");
                        //}
                        string Creator = _context.GetSession("UserID");
                        string SPUpdate = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "spUpdateName");
                        string ParamUpdate = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "paramUpdate");
                        string json = "";
                        string[] b = ParamUpdate.Split(new string[] { "^" }, StringSplitOptions.None);
                        //string[] c = EditColumn.Split(new string[] { "^" }, StringSplitOptions.None);
                        //string[] t = EditColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
                        for (int i = 0; i < b.Length; i++)
                        {
                            string[] b1 = b[i].Split(new string[] { ";" }, StringSplitOptions.None);
                            string val = "";
                            if (b1[0] == "Creator")
                                val = Creator;
                            else if (b1[0] == "MaxUserPortal")
                                val = _context.MaxUserPortal.ToString();
                            else if (b1[0] == "MaxUserHrm")
                                val = _context.MaxUserHrm.ToString();
                            else
                                val = _context.GetFormValue(b1[0]);

                            int j = Tools.GetArrayPos(b1[0], jsdbcols);
                            string[] IColumnType = null;
                            string ICacheable = "";
                            if (j > -1)
                            {
                                IColumnType = ColumnType[j].Split(new string[] { ";" }, StringSplitOptions.None);
                                ICacheable = Cacheable[j];
                            }
                            // cache du lieu
                            if (ICacheable == "1" && val != "" && !IsEdit) _context.SetSession("txtSession_" + Creator + "_" + jsdbcols[j], val);
                            // xac dinh kieu du lieu va tham so
                            if (IColumnType != null)
                            {
                                switch (IColumnType[0].ToLower())
                                {
                                    case "numeric":
                                        if (val == "")
                                            val = "0";
                                        else
                                            val = Tools.RemNumSepr(val);
                                        break;
                                    case "date":
                                        if (val == "")
                                            val = DateTime.Now.ToString("MM/dd/yyyy");
                                        else
                                            val = Tools.SwapDate(val);
                                        break;
                                    case "datetime":
                                        if (val == "")
                                            val = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                                        else
                                            val = Tools.SwapDate(val + ":00");
                                        break;
                                    case "algorithm":
                                        if (val != "") val = _context.enc.Encrypt(val);
                                        break;
                                }
                            }

                            /*
                            if (val == "") val = b1[4]; // default from param
                            if (val == "") //val = null;
                            {
                                if (toolDAO.CheckParam(b1[1])) val = "0";
                                else if (toolDAO.CheckParam(b1[1], "Date")) val = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                                else if (toolDAO.CheckParam(b1[1], "Time")) val = DateTime.Now.ToString("HH:mm:ss");
                            }
                            else if (toolDAO.CheckParam(b1[1], "Date")) val = Tools.SwapDate(val);
                            else if (toolDAO.CheckParam(b1[1]))
                            {
                                int iActb = val.IndexOf(Tools.C_ActbSepr);
                                if (iActb > -1) val = val.Substring(0, iActb - 1);
                                val = Tools.RemNumSepr(val);
                            }
                            */
                            if (val == "") val = b1[4]; // default from param
                            json = json + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":" + (val == null ? "null" : "\"" + val + "\"") + "}";
                        }
                        if (json != "")
                        {
                            json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                            d = JObject.Parse(json);
                        }
                        string parameterOutput = ""; int errorCode = 0; string errorString = "";
                        toolDAO.ExecuteStore("Update", SPUpdate, d, ref parameterOutput, ref json, ref errorCode, ref errorString);

                        if (errorCode == 500)
                        {
                            HTTP_CODE.WriteLogAction("functionName:Basetab/SaveItem\nException" + errorString);
                            r = errorString;
                            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                            r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"\", \"2\");");
                            r1.Append(Environment.NewLine + "</script>");
                            //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(errorString, "Error"));
                        }
                        else
                        {
                            d = JObject.Parse("{" + parameterOutput + "}");
                            if ((long)d.ParameterOutput.ResponseStatus > 0)
                            {
                                //_context.SetCookie("IsMessage", "1", 20, false);
                                if (BackAfterAddnew == 3)
                                {
                                    HTTP_CODE.WriteLogAction("functionName:Basetab/SaveItem\nException" + d.ParameterOutput.Message.ToString());
                                    r = _context.GetLanguageLable(d.ParameterOutput.Message.ToString());
                                    r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                                    r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-success\", \"" + _context.GetLanguageLable("Alert-Success") + "\", \"" + r + "\", \"" + GetUrl + "?" + GetQueryString + "\", \"0\");");
                                    //r1.Append(Environment.NewLine + "parent.location.href = '" + GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Success") +"';");
                                    r1.Append(Environment.NewLine + "</script>");
                                    //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Success"));
                                }
                                else if (BackAfterAddnew == 1)
                                {
                                    HTTP_CODE.WriteLogAction("functionName:Basetab/SaveItem\nException" + d.ParameterOutput.Message.ToString());
                                    r = _context.GetLanguageLable(d.ParameterOutput.Message.ToString());
                                    r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                                    r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-success\", \"" + _context.GetLanguageLable("Alert-Success") + "\", \"" + r + "\", \"" + UrlEdittab + "-1&MenuOn=" + _context.GetRequestOneValue("MenuOn") + "&GetUrl=" + Compress.Zip(GetUrl) + "&GetQueryString=" + Compress.Zip(GetQueryString) + "\", \"1\");");
                                    //r1.Append(Environment.NewLine + "parent.location.href = '" + UrlEdittab + "-1&GetUrl=" + Compress.Zip(GetUrl) + "&GetQueryString=" + Compress.Zip(GetQueryString) + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Success") + "';");
                                    r1.Append(Environment.NewLine + "</script>");
                                    //return Redirect(UrlEdittab + "-1&GetUrl=" + Compress.Zip(GetUrl) + "&GetQueryString=" + Compress.Zip(GetQueryString) + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Success"));
                                }
                                else
                                {
                                    HTTP_CODE.WriteLogAction("functionName:Basetab/SaveItem\nException" + d.ParameterOutput.Message.ToString());
                                    r = _context.GetLanguageLable(d.ParameterOutput.Message.ToString());
                                    r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                                    r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-success\", \"" + _context.GetLanguageLable("Alert-Success") + "\", \"" + r + "\", \"" + UrlEdittab + d.ParameterOutput.ResponseStatus.ToString() + "&MenuOn=" + _context.GetRequestOneValue("MenuOn") + "&GetUrl=" + Compress.Zip(GetUrl) + "&GetQueryString=" + Compress.Zip(GetQueryString) + "\", \"0\");");
                                    //r1.Append(Environment.NewLine + "parent.location.href = '" + UrlEdittab + d.ParameterOutput.ResponseStatus.ToString() + "&GetUrl=" + Compress.Zip(GetUrl) + "&GetQueryString=" + Compress.Zip(GetQueryString) + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Success") + "';");
                                    r1.Append(Environment.NewLine + "</script>");
                                    //return Redirect(UrlEdittab + d.ParameterOutput.ResponseStatus.ToString() + "&GetUrl=" + Compress.Zip(GetUrl) + "&GetQueryString=" + Compress.Zip(GetQueryString) + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Success"));
                                }
                            }
                            else
                            {
                                HTTP_CODE.WriteLogAction("functionName:Basetab/SaveItem\nException" + d.ParameterOutput.Message.ToString());
                                r = _context.GetLanguageLable(d.ParameterOutput.Message.ToString());
                                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                                r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"\", \"2\");");
                                r1.Append(Environment.NewLine + "</script>");
                                //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(d.ParameterOutput.Message.ToString(), "Error"));
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                r = _context.RoleDebug(ex.ToString());
                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                r1.Append(Environment.NewLine + "parent.JsAlert(\"alert-error\", \"" + _context.GetLanguageLable("Alert-Error") + "\", \"" + r + "\", \"\", \"2\");");
                r1.Append(Environment.NewLine + "</script>");
                //return Redirect(GetUrl + "?" + GetQueryString + "&Message=" + _context.ReturnMsg(r, "Error"));
            }
            r = r1.ToString(); r1 = null;
            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
        }
        public IActionResult BasetabText()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestVal("id");
                HTTP_CODE.WriteLogAction("functionName:/Basetab/BasetabText\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    /*string TabIndex = _context.GetRequestVal("id"); */
                    string stage = _context.GetRequestVal("stage");
                    StringBuilder r1 = new StringBuilder(); string r = "";
                    dynamic d = null; //string h = _context.GetSession("BTabHelper_" + TabIndex);
                    //r1.Append("<img src='/images/hb1rev.gif' class='imggo' onClick=\"showhelp('" + TabIndex + "', '" + stage + "')\">");
                    r1.Append("<b>" + _context.GetLanguageLable("Help") + " " + _context.GetLanguageLable(TabIndex) + "</b>:: <pre><font face='arial' size=2>");//OnViewCode
                    bool IsCached = _context._cache.Get("BTabHelper_" + TabIndex + "_" + _context.GetSession("language"), out d);
                    //if (h != "" && h != null)
                    if (IsCached)
                    {
                        //d = JObject.Parse(h);
                        if (d.Helper.Items.Count > 0)
                            r1.Append(Tools.GetDataJson(d.Helper.Items[0], "HelpText") + "<br>" + Tools.GetDataJson(d.Helper.Items[0], "DesignText"));
                        else
                            r1.Append(_context.GetLanguageLable("NoContent") + "<br>" + _context.GetLanguageLable("NoContent"));
                    }
                    r1.Append("</font><br>" + _context.GetLanguageLable("SearchTip") + " Search tip (if search box is present): <br>~~search keywords -> search left portion of the fields have search keywords </pre>");
                    r = r1.ToString();
                    r1 = null;
                    return Content(r);
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        public IActionResult ViewHist()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestVal("TabIndex");
                HTTP_CODE.WriteLogAction("functionName:/Basetab/ViewHist\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    /*string TabIndex = _context.GetRequestVal("id"); */
                    string[] Column = null; string ColumnName = ""; string ColumnType = "";
                    string ID = _context.GetRequestVal("ID"); if (ID == "") ID = "-1";
                    string SysV = _context.GetRequestVal("SysV"); if (SysV == "") SysV = "-1";
                    StringBuilder r1 = new StringBuilder(); string r = "";
                    string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                    dynamic d = JObject.Parse("{\"parameterInput\":[" +
                        "{\"ParamName\":\"ID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + ID + "\"}," +
                        "{\"ParamName\":\"SysV\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + SysV + "\"}," +
                        "{\"ParamName\":\"TabIndex\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + TabIndex + "\"}," +
                        "{\"ParamName\":\"Column\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}]}");
                    bosDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context);
                    bosDAO.ExecuteStore("ViewHist", "SP_CMS__ViewHist_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                    d = JObject.Parse("{" + parameterOutput + "}");
                    if (errorCode != 200) return Content(""); ;
                    Column = d.ParameterOutput.Column.ToString().Split(new string[] { "^" }, StringSplitOptions.None);
                    int cnt = 5; if (cnt > (Column.Length - 6)) cnt = (Column.Length - 6);
                    ColumnName = Column[2]; ColumnType = "";
                    for (int i = 3; i < cnt; i++)
                    {
                        ColumnName = ColumnName + "^" + Column[i];
                        ColumnType = ColumnType + "^";
                    }
                    ColumnName = ColumnName + "^SysV^CreatorName^CreatorDate^ChangeName^ChangeDate"; ColumnType = ColumnType + "^Numeric^^Datetime^^Datetime";  // Tai khoan tao; tai khoan thay doi
                    string[] a; a = ColumnName.Split(new string[] { "^" }, StringSplitOptions.None);
                    string[] c; c = ColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
                    r1.Append("<table class=\"table table-hover table-border flex\" id=\"dbtablist\">");
                    r1.Append("<thead class=\"thead-light\"><tr>");
                    for (int i = 0; i < a.Length; i++)
                    {
                        r1.Append("<th nowrap>" + _context.GetLanguageLable(a[i]) + "</th>");
                    }
                    r1.Append("</tr></thead>");
                    r1.Append("<tbody>");
                    d = JObject.Parse(json);
                    for (int i = 0; i < d.ViewHist.Items.Count; i++)
                    {
                        if (i % 2 == 1)
                            r1.Append("<tr class=\"basetabol\" id=\"trrowid" + i + "\">");
                        else
                            r1.Append("<tr id=\"trrowid" + i + "\">");
                        for (int j = 0; j < a.Length; j++)
                        {
                            string val = "";
                            if (a[j] == "I")
                            {
                                r1.Append("<td><a style=\"color:#000\" href=\"javascript:view_T_Hist('" + TabIndex + "'," + Tools.GetDataJson(d.ViewHist.Items[i], "I") + "," + Tools.GetDataJson(d.ViewHist.Items[i], "SysV") + ")\">" + Tools.GetDataJson(d.ViewHist.Items[i], "I") + "</a></td>");
                            }
                            else
                            {
                                switch (c[j])
                                {
                                    case "Datetime":
                                        try { val = (DateTime.Parse(Tools.GetDataJson(d.ViewHist.Items[i], a[j]))).ToString("dd/MM/yyyy HH:mm"); } catch { val = ""; }
                                        r1.Append("<td class=\"text-center\">" + val + "</td>");
                                        break;
                                    case "Numeric":
                                        try { val = Tools.FormatNumber(Tools.GetDataJson(d.ViewHist.Items[i], a[j])); } catch { val = ""; }
                                        r1.Append("<td class=\"text-right\">" + val + "</td>");
                                        break;
                                    default:
                                        try { val = Tools.GetDataJson(d.ViewHist.Items[i], a[j]); } catch { val = ""; }
                                        r1.Append("<td>" + val + "</td>");
                                        break;
                                }
                            }
                        }
                    }
                    r1.Append("</tbody>");
                    r1.Append("</table>");

                    r = r1.ToString();
                    r1 = null;
                    return Content(r);
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        public IActionResult DbAction()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestVal("TabIndex");
                HTTP_CODE.WriteLogAction("functionName:/Basetab/DbAction\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    if (!_context.CheckApproved(0, 0, TabIndex))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction - Action: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex), _context);
                        //return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                        return Content(_context.GetLanguageLable("YouAreNotIsGrantToFunction"));
                    }
                    StringBuilder r1 = new StringBuilder(); string r = "";
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context);
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    r1.Append(a.UIActionForm());

                    r = r1.ToString();
                    r1 = null;
                    return Content(r);
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        public IActionResult TabUrl()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                StringBuilder r1 = new StringBuilder(); string r = "";
                string ChildName = "";
                string ChildURL = "";
                string title = "";
                string TabIndex = _context.GetRequestVal("TabIndex"); if (TabIndex == "") TabIndex = "Nations";
                bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, MenuID, false));
                dynamic d; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\": \"DBTabConfigID\", \"ParamType\": \"0\", \"ParamInOut\": \"1\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                    "{\"ParamName\": \"Keyword\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"200\", \"InputValue\": \"" + TabIndex + "\"}," +
                    "{\"ParamName\": \"Page\", \"ParamType\": \"8\", \"ParamInOut\": \"1\", \"ParamLength\": \"4\", \"InputValue\": \"1\"}," +
                    "{\"ParamName\": \"PageSize\", \"ParamType\": \"8\", \"ParamInOut\": \"1\", \"ParamLength\": \"4\", \"InputValue\": \"1\"}," +
                    "{\"ParamName\": \"Rowcount\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
                bosDAO.ExecuteStore("DBTabConfig", "SP_CMS__DBTabConfig_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                d = JObject.Parse(json);
                ChildName = Tools.GetDataJson(d.DBTabConfig.Items[0], "ChildName");
                ChildURL = Tools.GetDataJson(d.DBTabConfig.Items[0], "ChildURL");
                /*
                try
                {
                    d = _context._appConfig.FrmTabList;
                    bool kt = false; int i = 0;
                    while (!kt && i < d.Count)
                    {
                        if (d[i].TabIndex == TabIndex) kt = true;
                        i++;
                    }
                    if (kt)
                    {
                        i--;
                        ChildName = d[i].ChildName;
                        ChildURL = d[i].ChildURL;
                    }
                    else
                    {
                        TabIndex = "Nations";
                        ChildName = "Nations^Citys^Districts^Wards";
                        ChildURL = "/Basetab/Dbtab||TabIndex;MenuOn||Nations;Off^/Basetab/Dbtab||TabIndex;MenuOn||Citys;Off^/Basetab/Dbtab||TabIndex;MenuOn||Districts;Off^/Basetab/Dbtab||TabIndex;MenuOn||Wards;Off";
                    }
                }
                catch
                {
                    TabIndex = "Nations";
                    ChildName = "Nations^Citys^Districts^Wards";
                    ChildURL = "/Basetab/Dbtab||TabIndex;MenuOn||Nations;Off^/Basetab/Dbtab||TabIndex;MenuOn||Citys;Off^/Basetab/Dbtab||TabIndex;MenuOn||Districts;Off^/Basetab/Dbtab||TabIndex;MenuOn||Wards;Off";
                }
                */
                title = _context.GetLanguageLable(TabIndex);
                r1.Append(UIDef.UITabForm(_context, ChildName, ChildURL, TabIndex));
                r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                r = r1.ToString();
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
                ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                ViewData["IndexBody"] = r;
                ViewData["PageTitle"] = title;
                ViewData["iframe"] = _context.GetRequestVal("iframe");
                ViewData["txtClose"] = _context.GetLanguageLable("Close");
                r1 = null;
                return View();
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        public IActionResult Dbtab()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestTabIndex();
                HTTP_CODE.WriteLogAction("functionName:/Basetab/Dbtab\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                // check login
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    // check quyen
                    bool IsFilter = (_context.GetRequestVal("IsFilter") != "Off");
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = _context.CheckPermistion("IsDelete", 0, 0, TabIndex);
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, TabIndex);
                    string FormEditType = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormEditType");
                    string IsSysAdmin = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "IsSysAdmin");
                    _context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType, IsSysAdmin);
                    if (!(IsGrant || IsInsert || IsUpdate || IsDelete))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex), _context);
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }
                    bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                    string title = _context.GetLanguageLable(TabIndex);

                    StringBuilder r1 = new StringBuilder(); string r = "";

                    r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, a.MenuID));
                    r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, a.MenuID, MenuOn));
                    r1.Append(a.UIHeaderFormList());
                    r1.Append(a.UIFilterForm());

                    r1.Append(a.UIListForm(IsInsert, IsUpdate, IsDelete));
                    r1.Append(UIDef.UIContentTagClose(_context, MenuOn, MenuOn));
                    r = r1.ToString();
                    ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, a.MenuID);
                    ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                    ViewData["IndexBody"] = r;
                    ViewData["PageTitle"] = title;
                    ViewData["iframe"] = _context.GetRequestVal("iframe");
                    ViewData["txtClose"] = _context.GetLanguageLable("Close");
                    r1 = null;
                    return View();
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }       
        public IActionResult DbEdit()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestTabIndex();
                HTTP_CODE.WriteLogAction("functionName:/Basetab/DbEdit\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                // check login
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    // check quyen
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    string IdCol = _context.GetRequestVal(_context.GetRequestVal("IdCol")); if (IdCol == "") IdCol = "-1";
                    bool IsEdit = (int.Parse(IdCol) > 0);
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, TabIndex);
                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    ///string abc = _context.GetRequestVal("IdCol");
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = _context.CheckPermistion("IsDelete", 0, 0, TabIndex);
                    string FormEditType = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormEditType");
                    string IsSysAdmin = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "IsSysAdmin");
                    _context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType, IsSysAdmin);
                    if (!(!IsEdit && IsInsert || IsUpdate || IsDelete || IsGrant))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex), _context);
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }
                    bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                    string title = //_context.GetLanguageLable("YouAre") +
                                   //_context.GetLanguageLable("Basetab") +
                           _context.GetLanguageLable(TabIndex);


                    StringBuilder r1 = new StringBuilder(); string r = "";
                    string UrlDbtab = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "urlList");
                    string UrlEdittab = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "urlEdit"); UrlEdittab = _context.ReplaceRequestValue(UrlEdittab);
                    string ColumnID = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "DeleteColumn");

                    //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, a.MenuID));
                    r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, a.MenuID));
                    r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, a.MenuID, MenuOn));
                    //if (_context.LeftFormEdit == "1") r1.Append(a.UIListFormLeft());
                    string SysU = ""; string SysV = ""; int TabFormIndex = 1;
                    string EditFrm = a.UIEditForm(ref TabFormIndex, ref SysU, ref SysV, ref IsEdit, ref IsInsert, ref IsUpdate, ref IsDelete);
                    string MenuLeftInForm = a.UIFormLeft();
                    string iframe = _context.GetRequestVal("iframe");

                    // form delete
                    r1.Append(//Environment.NewLine + "<scr" + "ipt src=\"/js/tinymce.min.js\"></scr" + "ipt>" +
                        Environment.NewLine + "<form target=\"saveTranFrm" + iframe + "\" onsubmit=\"return false\" name=\"DeleteForm\" method=post>");
                    string SysUID = ColumnID + "||" + IdCol + "||" + _context.GetSession("UserID");
                    r1.Append(UIDef.UIHidden("SysUID", _context.enc.Encrypt(SysUID)));
                    r1.Append(UIDef.UIHidden("Type", "-1")+
                        UIDef.UIHidden("TabIndex", TabIndex) +
                        UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()) +
                        UIDef.UIHidden("iframe", iframe) +
                        UIDef.UIHidden("GetUrl", Compress.Zip(_context.GetRequestVal("GetUrl"))) +
                        UIDef.UIHidden("GetQueryString", Compress.Zip(_context.GetRequestVal("GetQueryString"))));
                    r1.Append(UIDef.UIHidden(ColumnID, IdCol));
                    r1.Append("</form>");
                    // form edit
                    r1.Append(Environment.NewLine + "<form name=\"bosfrm\" target=\"saveTranFrm" + iframe + "\" onsubmit=\"return false\" action=\"/Basetab/SaveItem\" method=\"post\">");
                    r1.Append(UIDef.UIHidden("GetUrl", Compress.Zip(_context.GetRequestVal("GetUrl"))));
                    r1.Append(UIDef.UIHidden("GetQueryString", Compress.Zip(_context.GetRequestVal("GetQueryString"))));
                    r1.Append(UIDef.UIHidden(ColumnID, IdCol));
                    r1.Append(UIDef.UIHidden("TabIndex", TabIndex));
                    r1.Append(UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()));
                    r1.Append(UIDef.UIHidden("iframe", iframe));
                    r1.Append(UIDef.UIHidden("IdCol", _context.GetRequestVal("IdCol")) +
                        UIDef.UIHidden("CopyID", "0"));
                    //r1.Append(UIDef.UIHidden(_context.GetRequestVal("IdCol"), IdCol)); double với form edit
                    r1.Append(a.UIEditFormHeader(TabFormIndex, SysU, SysV, MenuLeftInForm, IsEdit, IsInsert, IsUpdate, IsDelete));

                    r1.Append(EditFrm);

                    //r1.Append(a.UIListFormLeftClose());
                    string UIEditChild = "";
                    if(MenuLeftInForm == "") UIEditChild = a.UIEditChild();
                    r1.Append(Environment.NewLine + "</" + "form>" + (IsEdit && UIEditChild != "" ? UIEditChild : ""));
                    r1.Append(UIDef.UIContentTagClose(_context, MenuOn, MenuOn));
                    r = r1.ToString();
                    ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, a.MenuID);
                    ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                    ViewData["IndexBody"] = r;
                    ViewData["PageTitle"] = title;
                    ViewData["iframe"] = iframe;
                    ViewData["txtClose"] = _context.GetLanguageLable("Close");
                    r1 = null;
                    return View();
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        public IActionResult PrintForm()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestTabIndex();
                HTTP_CODE.WriteLogAction("functionName:/Basetab/PrintForm\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                // check login
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    // check quyen
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    //string IdCol = _context.GetRequestVal(_context.GetRequestVal("IdCol"));
                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = _context.CheckPermistion("IsDelete", 0, 0, TabIndex);
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, TabIndex);
                    if (!(IsGrant || IsInsert || IsUpdate || IsDelete))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex), _context);
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }

                    StringBuilder r1 = new StringBuilder(); string r = "";
                    //r1.Append("<img src='/images/hb1rev.gif' class='imggo' onClick=\"showPrint(document.getElementById('aPrint'), '" + TabIndex + "')\">");
                    r1.Append(a.UIPrintForm());
                    r = r1.ToString();
                    r1 = null;
                    return Content(r);
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        public IActionResult Print()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestTabIndex();
                HTTP_CODE.WriteLogAction("functionName:/Basetab/PrintForm\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                // check login
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    // check quyen
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    string ColumnID = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "DeleteColumn");
                    string StoreGetPrintForm = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "StoreGetPrintForm");
                    string[] ColumnIDReq = _context.GetRequestVal(ColumnID).Split(new string[] { "," }, StringSplitOptions.None);

                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = _context.CheckPermistion("IsDelete", 0, 0, TabIndex);
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, TabIndex);
                    if (!(IsGrant || IsInsert || IsUpdate || IsDelete))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex), _context);
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }

                    string contentType = "application/zip"; string UrlFileDownload = "";
                    int cntFiles = ColumnIDReq.Length;
                    string[] fileFullnames = new string[cntFiles];
                    bool IsWord = false; if (cntFiles < 2) IsWord = true;
                    for (int k = 0; k < cntFiles; k++)
                    {
                        // Lấy biểu mẫu
                        string[] a99 = StoreGetPrintForm.Split(new string[] { ";" }, StringSplitOptions.None);
                        string[] a1 = a99[2].Split(new string[] { "," }, StringSplitOptions.None);
                        int errorCode = 0; string errorString = ""; string parameterOutput = ""; string json = "";
                        string ReqJson = _context.InputDataSetParamStr(a99[1] + ColumnIDReq[k]);
                        dynamic d = JObject.Parse(ReqJson);
                        toolDAO.ExecuteStore("MediaFileID", a99[0], d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        if (errorCode == 200) // Tra vấn được ID media
                        {
                            d = JObject.Parse(json);
                            string ImageId = Tools.GetDataJson(d.MediaFileID.Items[0], a1[0]);
                            errorCode = 0; errorString = "";
                            d = Tools.GetMedia(_context, ImageId, bosDAO, ref errorCode, ref errorString);
                            if (errorCode == 200) // Tra vấn được file media
                            {
                                if (Tools.GetDataJson(d.GetImage.Items[0], "FileExtention") != ".doc" && Tools.GetDataJson(d.GetImage.Items[0], "FileExtention") != ".docx")
                                {
                                    string r = _context.GetLanguageLable("{FileExtensionIsNotAllowed} (.docx;.doc)");
                                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nFileExtention" + Tools.GetDataJson(d.GetImage.Items[0], "FileExtention"));                                    
                                    return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                                }
                                string UrlFileName = _context.AppConfig.FolderRoot + Tools.GetDataJson(d.GetImage.Items[0], "UrlOriginal").Replace("||", "\\");
                                if (IsWord) contentType = Tools.GetDataJson(d.GetImage.Items[0], "MimeType");
                                UrlFileDownload = _context.AppConfig.FolderRoot + @"\wwwroot\docRoot\" + TabIndex + "_" + k + ".docx";
                                _context.AppConfig.FileDelete(UrlFileDownload);
                                // Execute Store Datalist
                                string StorePrintListName = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "StorePrintListName");
                                string StorePrintListParam = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "StorePrintListParam");
                                string DataAll = "";
                                string SchemaName = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "SchemaName"); if (SchemaName != "") SchemaName = SchemaName + ".";
                                string[] ListStoreName = StorePrintListName.Split(new string[] { "$" }, StringSplitOptions.None);
                                string[] ListStoreParam = StorePrintListParam.Split(new string[] { "$" }, StringSplitOptions.None);
                                string SPName = SchemaName + ListStoreName[0]; parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                                if (StorePrintListName != "")
                                {
                                    Tools.ExecParam("DataVar", "", ListStoreParam[0], "", "", _context, SPName, toolDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, true, ColumnID, ColumnIDReq[k]);
                                    if (errorCode == 200) DataAll = Tools.RemoveLastChar(json);
                                    for (int i = 1; i < ListStoreName.Length; i++)
                                    {
                                        SPName = SchemaName + ListStoreName[i]; parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                                        Tools.ExecParam("DATA" + i, "", ListStoreParam[i], "", "", _context, SPName, toolDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, true, ColumnID, ColumnIDReq[k]);
                                        if (errorCode == 200) DataAll = DataAll + "," + Tools.RemoveFisrtChar(Tools.RemoveLastChar(json));
                                    }
                                }

                                if (DataAll != "")
                                    DataAll = DataAll + "}";
                                else
                                    DataAll = "{ \"DataVar\": {   \"Count\": 0,   \"Items\": [     {       \"STT\": \"STT:1\",       \"VN_FULLNAME\": \"VN_FULLNAME:827252\",      " +
                                    " \"VN_FULLNAME1\": \"VN_FULLNAME1:TQ\",       \"YHEIGHT\": \"YHEIGHT:China\",       \"YHEIGHT1\": \"YHEIGHT1:1\",       " +
                                    "\"BIRTH_DATE\": \"BIRTH_DATE:827143\",       \"BIRTH_DATE2\": \"BIRTH_DATE2:NganDT\",       \"YWEIGHT\": \"YWEIGHT:2\",       " +
                                    "\"YWEIGHT1\": \"YWEIGHT1:827252\",		\"BIRTH_PLACE1\": \"BIRTH_PLACE1:2\",       \"BIRTH_PLACE\": \"BIRTH_PLACE:827252\",		" +
                                    "\"PER_ADDRESS1\": \"PER_ADDRESS1:2\",       \"PER_ADDRESS\": \"PER_ADDRESS:827252\"     }]}, \"DATA1\": {   \"Count\": 3,   \"Items\": [     {      " +
                                    " \"STT\": \"STT-Data1:1\",       \"START_DATE\": \"START_DATE-Data1:827252\",       \"END_DATE\": \"END_DATE-Data1:TQ\",     " +
                                    "  \"YSCHOOL_NAME\": \"YSCHOOL_NAME-Data1:China\",       \"YMAJOR\": \"YMAJOR-Data1:1\",       \"YTYPE\": \"YTYPE-Data1:827143\",       " +
                                    "\"YLEVELS\": \"YLEVELS-Data1:NganDT\",       \"CreatorVersion\": \"CreatorVersion-Data1:2\",       \"OT_NationIDList\": \"OT_NationIDList-Data1: 827252\"     }, " +
                                    "    {       \"STT\": \"2\",       \"START_DATE\": \"689\",       \"END_DATE\": \"VN\",       \"YSCHOOL_NAME\": \"VietName\",       \"YMAJOR\": \"1\",     " +
                                    "  \"YTYPE\": \"92\",       \"YLEVELS\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"689\"     },    " +
                                    " {       \"STT\": \"3\",       \"START_DATE\": \"139\",       \"END_DATE\": \"KR\",       \"YSCHOOL_NAME\": \"Korea\",     " +
                                    "  \"YMAJOR\": \"1\",       \"YTYPE\": \"92\",       \"YLEVELS\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",      " +
                                    " \"OT_NationIDList\": \"139\"     }   ] }, \"DATA3\": {   \"Count\": 3,   \"Items\": [     {       \"STT\": \"DATA3:1\",     " +
                                    "  \"FROM_DATE\": \"DATA3-FROM_DATE:827252\",       \"TO_DATE\": \"DATA3-TO_DATE:TQ\",       \"COMPANY\": \"China\",       \"POSITION\": \"1\",     " +
                                    "  \"GROSS_SAL\": \"DATA3:827143\",       \"TER_REASON\": \"NganDT\",       \"CreatorVersion\": \"2\",       \"OT_NationIDList\": \"827252\"     },   " +
                                    "  {       \"STT\": \"2\",       \"FROM_DATE\": \"689\",       \"TO_DATE\": \"VN\",       \"COMPANY\": \"VietName\",       \"POSITION\": \"1\",    " +
                                    "   \"GROSS_SAL\": \"92\",       \"TER_REASON\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"689\"     },     {      " +
                                    " \"STT\": \"3\",       \"FROM_DATE\": \"139\",       \"TO_DATE\": \"KR\",       \"COMPANY\": \"Korea\",       \"POSITION\": \"1\",     " +
                                    "  \"GROSS_SAL\": \"92\",       \"TER_REASON\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"139\"     }   ] }}";
                                WordProcessing w = new WordProcessing(UrlFileName, UrlFileDownload, _context, DataAll);
                                fileFullnames[k] = UrlFileDownload;
                                w = null;
                            }
                            else
                            {
                                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + errorString);
                                string r = _context.RoleDebug(errorString);
                                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                            }
                        }
                        else
                        {
                            HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + errorString);
                            string r = _context.RoleDebug(errorString);
                            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                        }
                    }
                    byte[] fileBytes = null;
                    if (IsWord)
                        fileBytes = _context.AppConfig.FileReadAllBytes(UrlFileDownload);
                    else
                        fileBytes = Compress.ZipAddStream(fileFullnames);//fileBytes = memoryStream.ToArray();
                    if (fileBytes != null)
                    {
                        return File(fileBytes, contentType);
                    }
                    else
                    {
                        HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException: NULL data");
                        return Content(_context.GetLanguageLable("DataIsNull"), "text/html; charset=utf-8", Encoding.UTF8);
                    }
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }//Basetab/Print
        [HttpPost]
        public IActionResult PrintOption()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestTabIndex();
                string ImageId = _context.GetFormValue("FormID");
                HTTP_CODE.WriteLogAction("functionName:/Basetab/PrintForm\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                // check login
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    // check quyen
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    //string IdCol = _context.GetRequestVal(_context.GetRequestVal("IdCol"));
                    string ColumnID = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "DeleteColumn");
                    string[] ColumnIDReq = _context.GetRequestVal(ColumnID).Split(new string[] { "," }, StringSplitOptions.None);

                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = _context.CheckPermistion("IsDelete", 0, 0, TabIndex);
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, TabIndex);
                    if (!(IsGrant || IsInsert || IsUpdate || IsDelete))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex), _context);
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }

                    int errorCode = 0; string errorString = "";
                    dynamic d = Tools.GetMedia(_context, ImageId, bosDAO, ref errorCode, ref errorString);
                    if (errorCode == 200 || errorCode == 204)
                    {
                        //d = JObject.Parse(json);
                        string UrlFileName = _context.AppConfig.FolderRoot + Tools.GetDataJson(d.GetImage.Items[0], "UrlOriginal").Replace("||", "\\");
                        string contentType = "application/zip"; string UrlFileDownload = "";
                        //byte[] fileBytes = null;
                        //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                        //System.IO.Compression.ZipArchive zip = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create);
                        int cntFiles = ColumnIDReq.Length;
                        string[] fileFullnames = new string[cntFiles];
                        bool IsWord = false; if (cntFiles < 2) IsWord = true;
                        if (IsWord) contentType = Tools.GetDataJson(d.GetImage.Items[0], "MimeType");
                        for (int k = 0; k < cntFiles; k++)
                        {
                            UrlFileDownload = _context.AppConfig.FolderRoot + @"\wwwroot\docRoot\" + TabIndex + "_" + k + ".docx";
                            _context.AppConfig.FileDelete(UrlFileDownload);

                            // Execute Store Datalist
                            string StorePrintListName = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "StorePrintListName");
                            string StorePrintListParam = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "StorePrintListParam");
                            string DataAll = "";
                            string SchemaName = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "SchemaName"); if (SchemaName != "") SchemaName = SchemaName + ".";
                            string[] ListStoreName = StorePrintListName.Split(new string[] { "$" }, StringSplitOptions.None);
                            string[] ListStoreParam = StorePrintListParam.Split(new string[] { "$" }, StringSplitOptions.None);
                            string SPName = SchemaName + ListStoreName[0]; string parameterOutput = ""; string json = ""; errorCode = 0; errorString = "";
                            if (StorePrintListName != "")
                            {
                                Tools.ExecParam("DataVar", "", ListStoreParam[0], "", "", _context, SPName, toolDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, true, ColumnID, ColumnIDReq[k]);
                                if (errorCode == 200) DataAll = Tools.RemoveLastChar(json);
                                for (int i = 1; i < ListStoreName.Length; i++)
                                {
                                    SPName = SchemaName + ListStoreName[i]; parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                                    Tools.ExecParam("DATA" + i, "", ListStoreParam[i], "", "", _context, SPName, toolDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, true, ColumnID, ColumnIDReq[k]);
                                    if (errorCode == 200) DataAll = DataAll + "," + Tools.RemoveFisrtChar(Tools.RemoveLastChar(json));
                                }
                            }

                            if (DataAll != "")
                                DataAll = DataAll + "}";
                            else
                                DataAll = "{ \"DataVar\": {   \"Count\": 0,   \"Items\": [     {       \"STT\": \"STT:1\",       \"VN_FULLNAME\": \"VN_FULLNAME:827252\",      " +
                                " \"VN_FULLNAME1\": \"VN_FULLNAME1:TQ\",       \"YHEIGHT\": \"YHEIGHT:China\",       \"YHEIGHT1\": \"YHEIGHT1:1\",       " +
                                "\"BIRTH_DATE\": \"BIRTH_DATE:827143\",       \"BIRTH_DATE2\": \"BIRTH_DATE2:NganDT\",       \"YWEIGHT\": \"YWEIGHT:2\",       " +
                                "\"YWEIGHT1\": \"YWEIGHT1:827252\",		\"BIRTH_PLACE1\": \"BIRTH_PLACE1:2\",       \"BIRTH_PLACE\": \"BIRTH_PLACE:827252\",		" +
                                "\"PER_ADDRESS1\": \"PER_ADDRESS1:2\",       \"PER_ADDRESS\": \"PER_ADDRESS:827252\"     }]}, \"DATA1\": {   \"Count\": 3,   \"Items\": [     {      " +
                                " \"STT\": \"STT-Data1:1\",       \"START_DATE\": \"START_DATE-Data1:827252\",       \"END_DATE\": \"END_DATE-Data1:TQ\",     " +
                                "  \"YSCHOOL_NAME\": \"YSCHOOL_NAME-Data1:China\",       \"YMAJOR\": \"YMAJOR-Data1:1\",       \"YTYPE\": \"YTYPE-Data1:827143\",       " +
                                "\"YLEVELS\": \"YLEVELS-Data1:NganDT\",       \"CreatorVersion\": \"CreatorVersion-Data1:2\",       \"OT_NationIDList\": \"OT_NationIDList-Data1: 827252\"     }, " +
                                "    {       \"STT\": \"2\",       \"START_DATE\": \"689\",       \"END_DATE\": \"VN\",       \"YSCHOOL_NAME\": \"VietName\",       \"YMAJOR\": \"1\",     " +
                                "  \"YTYPE\": \"92\",       \"YLEVELS\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"689\"     },    " +
                                " {       \"STT\": \"3\",       \"START_DATE\": \"139\",       \"END_DATE\": \"KR\",       \"YSCHOOL_NAME\": \"Korea\",     " +
                                "  \"YMAJOR\": \"1\",       \"YTYPE\": \"92\",       \"YLEVELS\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",      " +
                                " \"OT_NationIDList\": \"139\"     }   ] }, \"DATA3\": {   \"Count\": 3,   \"Items\": [     {       \"STT\": \"DATA3:1\",     " +
                                "  \"FROM_DATE\": \"DATA3-FROM_DATE:827252\",       \"TO_DATE\": \"DATA3-TO_DATE:TQ\",       \"COMPANY\": \"China\",       \"POSITION\": \"1\",     " +
                                "  \"GROSS_SAL\": \"DATA3:827143\",       \"TER_REASON\": \"NganDT\",       \"CreatorVersion\": \"2\",       \"OT_NationIDList\": \"827252\"     },   " +
                                "  {       \"STT\": \"2\",       \"FROM_DATE\": \"689\",       \"TO_DATE\": \"VN\",       \"COMPANY\": \"VietName\",       \"POSITION\": \"1\",    " +
                                "   \"GROSS_SAL\": \"92\",       \"TER_REASON\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"689\"     },     {      " +
                                " \"STT\": \"3\",       \"FROM_DATE\": \"139\",       \"TO_DATE\": \"KR\",       \"COMPANY\": \"Korea\",       \"POSITION\": \"1\",     " +
                                "  \"GROSS_SAL\": \"92\",       \"TER_REASON\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"139\"     }   ] }}";
                            WordProcessing w = new WordProcessing(UrlFileName, UrlFileDownload, _context, DataAll);
                            fileFullnames[k] = UrlFileDownload;
                            w = null;
                            //Compress.ZipAddStream(TabIndex + "_" + k + ".docx", UrlFileDownload, zip);
                            //ZipArchiveEntry zipItem = zip.CreateEntry(TabIndex + "_" + k + ".docx");
                            //Stream entryStream = zipItem.Open();
                            //FileStream fs = System.IO.File.OpenRead(UrlFileDownload);
                            //fs.CopyTo(entryStream);
                            //entryStream.Flush();
                            //FileInfo file = new FileInfo(UrlFileDownload);
                            //zip.CreateEntryFromFile(file.FullName, file.Name, CompressionLevel.Optimal);
                        }
                        //memoryStream.Flush();
                        byte[] fileBytes = null;
                        if (IsWord)
                            fileBytes = _context.AppConfig.FileReadAllBytes(UrlFileDownload);
                        else
                            fileBytes = Compress.ZipAddStream(fileFullnames);//fileBytes = memoryStream.ToArray();
                        if (fileBytes != null)
                        {
                            return File(fileBytes, contentType);
                        }
                        else
                        {
                            HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException: NULL data");
                            return Content("NULL DATA", "text/html; charset=utf-8", Encoding.UTF8);
                        }
                    }
                    else
                    {
                        HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + errorString);
                        string r = _context.RoleDebug(errorString);
                        return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                    }
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return Redirect("/Home");
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}