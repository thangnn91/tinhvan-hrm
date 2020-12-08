using System;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace Utils
{
    public class HRSContext
    {
        // HTTPContext
        public HttpContext httpContext;
        public bool IsHttps;
        public HRSCache _cache;
        // Folder - tools
        public ToolFolder APIConfig; // Folder-Files API setting
        public ToolFolder AppConfig; // Folder-Files App setting
        public dynamic _appConfig; // Appsetting.json

        public Algorithm enc; // mã hóa, giải mã

        // System
        public string SysKey;
        public string SysCode;
        public int TimeCache;
        public int TimeCacheReport;

        public string PageDefault;
        public string LanguageDefault;
        public string PageSizeReport;
        public string PageSizeBaseTab;
        public string LeftFormEdit;

        public string WriteLog;

        public int Application; // 1. Web app; 2. iPortal
        public int MaxUserPortal; // Số lượng tối đa UserPortal được đăng ký
        public int MaxUserHrm; // Số lượng tối đa UserHrm được đăng ký
        public int MaxUserLoginCurrent; // Số lượng tối đa UserLogin 1 thời điểm

        // Const
        private const string ListQueryStringParam = ",GetUrl,GetQueryString,UrlBack,jsColAdd,SearchIndex,Message,";
        private const string QueryStringParamMenuOn = ",On,Off,Min,";
        private const string RemoveListParam = ",Message,";
        private const int MaxWhileCount = 500;

        private void SetAlgorithmObj(bool IsSys = true)
        {
            if (IsSys)
            {
                enc = new Algorithm("GYC9OFeeGtnbaN");
                SysKey = enc.Decrypt(_appConfig.System.SysKey.ToString());
                enc = null; enc = new Algorithm(SysKey);
            }
            else
            {
                string PrivateKey = _appConfig.ConnectionStrings.PrivateKey.ToString();
                if (PrivateKey == "")
                    enc = new Algorithm();
                else
                    enc = new Algorithm(PrivateKey);
            }
        }
        public string GetLanguage()
        {
            string l = GetSession("language"); // Kiểm tra Session language
            if (l == "") // Set Session
            {
                l = LanguageDefault;
                SetSession("language", l);
            }
            return l;
        }

        public HRSContext(HttpContext _httpContext, HRSCache cache, string LanguageAPI = "")
        {
            _cache = cache;
            httpContext = _httpContext;
            APIConfig = new ToolFolder("config.json");
            AppConfig = new ToolFolder("appsettings.json");
            IsHttps = httpContext.Request.IsHttps;
            _appConfig = JObject.Parse(AppConfig.ReadConfig(this));

            // No Enc
            try { PageDefault = _appConfig.System.PageDefault.ToString(); } catch { PageDefault = "/Home"; }
            if (LanguageAPI == "")
                LanguageDefault = _appConfig.System.LanguageDefault.ToString();
            else
                LanguageDefault = LanguageAPI;
            PageSizeReport = _appConfig.System.PageSizeReport.ToString();
            PageSizeBaseTab = _appConfig.System.PageSizeBaseTab.ToString();
            LeftFormEdit = _appConfig.System.LeftFormEdit.ToString();
            WriteLog = _appConfig.System.WriteLog.ToString();
            // Enc
            SetAlgorithmObj();
            SysCode = enc.Decrypt(_appConfig.System.SysCode.ToString());
            try { MaxUserPortal = int.Parse(enc.Decrypt(_appConfig.System.MaxUserPortal.ToString())); } catch { MaxUserPortal = 2; }//(Exception ex)
            try { MaxUserHrm = int.Parse(enc.Decrypt(_appConfig.System.MaxUserHrm.ToString())); } catch { MaxUserHrm = 2; }
            try { MaxUserLoginCurrent = int.Parse(enc.Decrypt(_appConfig.System.MaxUserLoginCurrent.ToString())); } catch { MaxUserLoginCurrent = 2; }
            try { TimeCache = int.Parse(_appConfig.ConnectionStrings.TimeCacheStore.ToString()); } catch { TimeCache = 0; }
            try { TimeCacheReport = int.Parse(_appConfig.System.TimeCacheReport.ToString()); } catch { TimeCacheReport = 0; }
            Application = 1; try { Application = int.Parse(_appConfig.System.Application.ToString()); } catch { Application = 1; }

            SetAlgorithmObj(false);

            ToolDAO bos = new ToolDAO(this);
            dynamic d1 = null; string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = "";
            string l;//= GetSession("json-language");
            //if (l == "")
            bool IsCached = _cache.Get("json-language_" + GetLanguage(), out l);
            if (!IsCached)
            {
                l = GetSession("language"); // Kiểm tra Session language
                if (l == "") // Set Session
                {
                    l = LanguageDefault; SetSession("language", l);
                }
                d1 = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\":\"Language\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + l + "\"}," +
                "{\"ParamName\":\"Json\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}]}");
                bos.ExecuteStore("LanguageText", "SP_CMS__LanguageText_ListAll", d1, ref parameterOutput, ref json, ref errorCode, ref errorString);
                d1 = JObject.Parse("{" + parameterOutput + "}");
                //l = d1.ParameterOutput.Json.ToString();
                //SetSession("json-language", l);
                _cache.Set("json-language_" + GetLanguage(), d1.ParameterOutput.Json.ToString());
            }
            //l = GetSession("cate-language");
            //if (l == "")
            IsCached = _cache.Get("cate-language", out d1);
            if (!IsCached)
            {
                //ToolDAO bos = new ToolDAO(this);
                //dynamic d1 = null; string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = "";
                d1 = null; // JObject.Parse("{\"parameterInput\":[" +
                //"{\"ParamName\":\"Cursor\", \"ParamType\":\"999\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}]}");
                bos.ExecuteStore("CateLanguage", "SP_CMS__Language_ListAll", d1, ref parameterOutput, ref json, ref errorCode, ref errorString);
                //SetSession("cate-language", json);
                _cache.Set("cate-language", JObject.Parse(json));
            }
        }

        #region HttpContext-Cookie
        public void SetCookie(string Ikey, string Ival, int ExpireTime = 720, bool IsMinutes = true)
        {
            CookieOptions option = new CookieOptions();
            if (IsMinutes)
                option.Expires = DateTime.Now.AddMinutes(ExpireTime);
            else
                option.Expires = DateTime.Now.AddSeconds(ExpireTime);
            //option.Secure = true;
            //option.Domain = GetHost();
            option.HttpOnly = true;
            option.SameSite = SameSiteMode.Strict;
            httpContext.Response.Cookies.Append(Ikey, Ival, option);
        }
        public string GetCookie(string Ikey)
        {
            return httpContext.Request.Cookies[Ikey];
        }
        public void DeleteCookie(string Ikey)
        {
            httpContext.Response.Cookies.Delete(Ikey);
            SetCookie(Ikey, Ikey, -720);
        }
        public void DeleteAllCookie(string NotInStr = ",versionays,buildays,")
        {
            foreach (var IKey in httpContext.Request.Cookies.Keys)
            {
                int i = NotInStr.IndexOf("," + IKey.ToString() + ",");// if (i == null) i = -1;
                if (i < 0) DeleteCookie(IKey.ToString());
            }
        }
        public string GetAllKeyCookie(string c = ",")
        {
            string b = "";
            foreach (var IKey in httpContext.Request.Cookies.Keys)
            {
                b = b + ";" + IKey.ToString() + "||" + GetCookie(IKey.ToString());
            }
            return b;
        }
        #endregion

        #region HttpContext-Session
        public void SetSession(string KEY, string VALUE)
        {
            //try {
            httpContext.Session.SetString(KEY, VALUE); //} catch { }
            //string a = httpContext.Session.ToString();
        }
        public string GetSession(string KEY)
        {
            string r = "";
            //try {
            r = httpContext.Session.GetString(KEY); //} catch { r = ""; }
            //string a = httpContext.Session.ToString();
            if (r == null || r == "")
            {
                switch (KEY)
                {
                    case "UserID":
                        r = "-1";
                        break;
                    case "StaffID":
                        r = "0";
                        break;
                    case "CompanyID":
                        r = "0";
                        break;
                    default:
                        r = "";
                        break;
                }
            }
            return r;
        }
        public void ClearSession()
        {
            httpContext.Session.Clear();
        }
        public string ReplaceSessionValue(string s)
        {
            int i; int j; int l = "SESSION".Length;
            i = s.IndexOf("SESSION");
            j = s.Length;
            if (i == -1 || j < l)
                return s;
            else
            {
                string s1 = s.Substring(l, j - l); s = GetSession(s1);
                return s;
            }
        }
        public string ReplaceListSessionValue(string s)
        {
            return s.Replace("SESSIONToken", GetSession("Token"))
                 .Replace("SESSIONUserID", GetSession("UserID"))
                 .Replace("SESSIONPeriodID", GetSession("PeriodID"))
                 .Replace("SESSIONUserName", GetSession("UserName"))
                 .Replace("SESSIONStaffID", GetSession("StaffID"))
                 .Replace("SESSIONCompanyID", GetSession("CompanyID"))
                 .Replace("SESSIONCompanyCode", GetSession("CompanyCode"))
                 .Replace("SESSIONPeriodID", GetSession("PeriodID"))
                 .Replace("SESSIONPeriodStartDate", GetSession("PeriodStartDate"))
                 .Replace("SESSIONPeriodEndDate", GetSession("PeriodEndDate"));
        }
        #endregion

        #region HttpContext-Url-Language
        public string GetHost()
        {
            string host = httpContext.Request.Host.ToString();
            return host;
        }
        public string RemoteIpAddress()
        {
            string host = httpContext.Connection.RemoteIpAddress.ToString();
            return host;
        }
        private string GetQueryStringPage()
        {
            string query = httpContext.Request.QueryString.ToString().Replace("?", "");
            string[] a = query.Split(new string[] { "&" }, StringSplitOptions.None);
            query = "";
            for (int i = 0; i < a.Length; i++)
            {
                string[] b = a[i].Split(new string[] { "=" }, StringSplitOptions.None);
                string Param = b[0].Trim();
                string Val = ""; if (b.Length > 1) Val = b[1].Trim();
                if (Param != "")
                {
                    if (RemoveListParam.IndexOf("," + Param + ",") < 0)
                    {
                        query = query + "&" + Param + "=" + Val;
                    }
                }
            }
            if (query != "") query = Tools.RemoveFisrtChar(query);
            return query;
        }
        public string GetQueryStrPage(string iParam)
        {
            string query = httpContext.Request.QueryString.ToString().Replace("?", "");
            string[] a = query.Split(new string[] { "&" }, StringSplitOptions.None);
            query = "";
            for (int i = 0; i < a.Length; i++)
            {
                string[] b = a[i].Split(new string[] { "=" }, StringSplitOptions.None);
                string Param = b[0].Trim();
                string Val = ""; if (b.Length > 1) Val = b[1].Trim();
                if (Param != "")
                {
                    if (("," + iParam + ",").IndexOf("," + Param + ",") < 0)
                    {
                        query = query + "&" + Param + "=" + Val;
                    }
                }
            }

            if (query != "") query = Tools.RemoveFisrtChar(query);
            return query;
        }
        private string GetUrlPage()
        {
            return httpContext.Request.Path.ToString().Replace("?", "");
        }
        public string GetUrl()
        {
            string Url = GetUrlPage();
            return Compress.Zip(Url);
        }
        public string GetQueryString()
        {
            string query = GetQueryStringPage();
            return Compress.Zip(query);//Tools.UrlEncode
        }
        public string GetUrlBack()
        {
            string path = GetUrlPage();
            string query = GetQueryStringPage();
            if (query != "") path = path + "?" + query;//Tools.UrlEncode
            return Compress.Zip(path);
        }
        public string ReturnMsg(string msg, string classBox = "OK", string IsWindowClose = "", string IsParentReload = "")
        {
            if (classBox == "Error")
            {
                msg = "alert-error^1^" + GetLanguageLable(msg);
            }
            else if (classBox == "Warning")
            {
                msg = "alert-warning^1^" + GetLanguageLable(msg);
            }
            else
            {
                msg = "alert-success^1^" + GetLanguageLable(msg);
            }
            if (IsWindowClose == "1") msg = IsWindowClose + "^" + msg;
            if (IsParentReload == "1") msg = IsParentReload + "^" + msg;
            return Compress.Zip(msg);
        }
        public string ReturnUrlLogin(string l, bool IsGetBack = true, bool IsChangePwd = false, string MsgError = "alert-error")
        {
            if (IsChangePwd)
                return "/Home/ChangePwd?Message=" + Compress.Zip(l) + "&Username=" + Tools.UrlEncode(GetRequestVal("Username"));
            else if (IsGetBack)
                return "/Home/Login?Message=" + Compress.Zip(l) + "&MsgError=" + MsgError + "&Username=" + Tools.UrlEncode(GetRequestVal("Username")) + "&UrlBack=" + GetUrlBack();
            else
                return "/Home/Login?Message=" + Compress.Zip(l) + "&MsgError=" + MsgError + "&Username=" + Tools.UrlEncode(GetRequestVal("Username")) + "&UrlBack=";
        }
        public string ReplaceStringLangValue(string s)
        {
            int i; int j;
            i = s.IndexOf("{");
            j = s.IndexOf("}");
            if (i == -1 || j == -1 || i == j)
                return s;
            else
            {
                int iWhile = 0; // chặn lỗi Out Of Memory
                while (!(i == -1 || j == -1 || i == j) && iWhile < MaxWhileCount)
                {
                    iWhile++;
                    string s1 = s.Substring(i + 1, j - i - 1);
                    s = s.Replace("{" + s1 + "}", GetLanguageText(s1));
                    i = s.IndexOf("{");
                    j = s.IndexOf("}");
                }
                return s;
            }
        }
        public string ParamLanguage(string ParamName, string ParamValue)
        {
            if (",Json,Company,StoreName,query,RequestJson,ResponseJson,Param,Column,ParamExecInput,ParamInput,ColumnInput,ParamIn,ParamOut,".IndexOf("," + ParamName + ",") < 0)
            {
                return GetLanguageLable(ParamValue);
            }
            return ParamValue;
        }
        public string GetLanguageLable(string KEY)
        {
            string r = ReplaceStringLangValue(KEY);
            r = GetLanguageText(r);
            /*dynamic d; string r = "";
            try
            {
                string l = GetSession("json-language");
                d = JObject.Parse(l);
                try { r = d[KEY].ToString(); } catch /*(Exception ex)* / { r = KEY; /*HTTP_CODE.WriteLogAction("functionName:GetLanguageLable\nKEY:" + KEY + "\nError:" + ex.ToString()); * /}
            }
            catch /*(Exception ex)* / { r = KEY; /*HTTP_CODE.WriteLogAction("functionName:GetLanguageLable\nKEY:" + KEY + "\nError:" + ex.ToString());* /}
            //catch (Exception ex) { r = ex.ToString(); }
            */
            return r;
        }
        private string GetLanguageText(string KEY)
        {
            //string sKey = KEY; // ReplaceStringLangValue(KEY);
            dynamic d; string r = "";
            try
            {
                string l;//= GetSession("json-language");
                _cache.Get("json-language_" + GetLanguage(), out l);
                d = JObject.Parse(l);
                //HTTP_CODE.WriteLogAction("json-language:" + d.ToString(), this);
                r = d[KEY].ToString();
                //try { r = d[KEY].ToString(); } catch /*(Exception ex)*/ { r = KEY; /*HTTP_CODE.WriteLogAction("functionName:GetLanguageLable\nKEY:" + KEY + "\nError:" + ex.ToString()); */}
            }
            catch /*(Exception ex)*/ { r = KEY; /*HTTP_CODE.WriteLogAction("functionName:GetLanguageLable\nKEY:" + KEY + "\nError:" + ex.ToString());*/}
            //catch (Exception ex) { r = ex.ToString(); }
            return r;
        }
        #endregion

        #region HttpContext-Request
        public string GetRequestTabIndex()
        {
            string s = GetRequestOneValue("TabIndex");
            if (s == "") s = "DBConfig";
            return s;
        }
        public string GetRequestRptID()
        {
            return GetRequestOneValue("RptID");
        }
        public string GetRequestOneValue(string Param)
        {
            string s = GetRequestVal(Param);
            int i = s.IndexOf(",");
            if (!(i < 0)) s = s.Substring(0, i);
            return s;
        }
        public string GetRequestMenuOn()
        {
            string s = GetRequestOneValue("MenuOn");
            if (QueryStringParamMenuOn.IndexOf("," + s + ",") < 0) s = "";
            return s;
        }
        public string GetRequestVal(string Param)
        {
            string s = GetFormValue(Param);
            if (s == "") s = GetQueryString(Param);
            return s;
        }
        // Lấy request value từ Form trình duyệt
        public string GetFormValue(string Param)
        {
            string r = "";
            try
            {
                r = httpContext.Request.Form[Param];
                if (ListQueryStringParam.IndexOf("," + Param + ",") > -1)
                {
                    //r = Tools.UrlDecode(r);
                    r = Compress.UnZip(r);
                }
            }
            catch { }
            if (r == null) r = "";
            r = ReplaceStr(r);
            return r;
        }
        // Lấy request value từ Header PageRequest
        public string GetHeaderValue(string Param)
        {
            string r = "";
            try
            {
                r = httpContext.Request.Headers[Param];
                r = ReplaceStr(r);
            }
            catch { }
            if (r == null) r = "";
            return r;
        }
        // Lấy request value từ Url trình duyệt
        public string GetQueryString(string Param)
        {
            string r = "";
            try
            {
                r = httpContext.Request.Query[Param];
                if (ListQueryStringParam.IndexOf("," + Param + ",") > -1)
                {
                    //r = Tools.UrlDecode(r);
                    r = Compress.UnZip(r);
                }
            }
            catch { }
            if (r == null) r = "";
            r = ReplaceStr(r);
            return r;
        }
        // Thay thế ký tự đặc biệt trong Reuqest value -> trống
        public bool IsSysAdmin()
        {
            return (GetSession("IsSysAdmin") == "1");
        }
        public string ReplaceStr(string s)
        {
            if (IsSysAdmin())
            {
                s = s.Replace("\"", "")
                    .Replace("%22", "")
                    .Replace("%27", "`")
                    .Replace("'", "`");
            }
            else
            {
                s = s.Replace("??", "?")
                    .Replace("'", "`")
                    .Replace("%27", "`")
                    .Replace("\"", "")
                    .Replace("%22", "")
                    //.Replace("\\\\", "")
                    //.Replace("\\", "")
                    .Replace("(", "[")
                    .Replace("{", "")
                    .Replace(")", "]")
                    .Replace("<", "«")
                    .Replace(">", "»")
                    .Replace("}", "");
                //.Replace("»", ">")
                //.Replace("«", "<");
            }
            return s;
        }
        //Thay thế giá trị Request cho đường dẫn
        private void GetRequestParam(string s, ref int i, ref int j)
        {
            string s0;
            j = s.IndexOf("=REQUEST");
            if (j > 1)
            {
                s0 = s.Substring(0, j);
                i = s0.LastIndexOf("&");
            }
            else
            {
                i = j;
            }
        }
        public string ReplaceRequestValue(string s, string f = "document.FilterForm")
        {
            //s = s.Replace(" = ", "=")
            //  .Replace(" = ", "=");
            int i = -1; int j = -1; //string s0 = s;
            //i = s.IndexOf("&");
            //j = s.IndexOf("=REQUEST");
            GetRequestParam(s, ref i, ref j);
            if (i == -1 || j == -1 || i == j)
                return s;
            else
            {
                int iWhile = 0; // chặn lỗi Out Of Memory
                while (!(i == -1 || j == -1 || i == j) && iWhile < MaxWhileCount)
                {
                    iWhile++;
                    string s1 = s.Substring(i + 1, j - i - 1);
                    string s2 = GetRequestVal(s1);
                    if (s2 == "" && f != "") s2 = "\" + " + f + "." + s1 + ".value + \"";
                    s = s.Replace("REQUEST" + s1, s2);
                    GetRequestParam(s, ref i, ref j);
                    //s = s.Substring(j + 8);
                    //i = s.IndexOf("&");
                    //j = s.IndexOf("=REQUEST");
                }
                return s;
            }
        }
        #endregion

        #region HttpContext-Login-Roles
        public string RoleDebug(string Error)
        {
            if (CheckRoles(807)) // 807 - Debug
            {
                return Error.Replace("'", "~").Replace(Environment.NewLine, "<br>").Replace("\\", "\\\\");
            }
            else
            {
                return GetLanguageLable("SystemError");
            }
        }
        public bool CheckDebug()
        {
            if (CheckRoles(807) && GetRequestVal("Debug") == "On") // 807 - Debug
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void CheckCountErrorLogin()
        {
            dynamic d = AppConfig.ReadConfigToJson(this);
            string s = GetSession("CountErrorLogin"); if (s == "") s = "0";
            int cnt = int.Parse(s);
            cnt = cnt + 1;
            if (cnt < int.Parse(d.LoginError.CountLoginError.ToString()))
            {
                SetSession("CountErrorLogin", cnt.ToString());
                HTTP_CODE.WriteLogAction("CountErrorLogin: " + cnt.ToString(), this);
            }
            else
            {
                DateTime n = DateTime.Now; DateTime chk;
                s = GetSession("BlockTime"); if (s == "") { s = string.Format("{0:M/d/yyyy HH:mm:ss}", n.AddSeconds(double.Parse(d.LoginError.NoSecondBlock.ToString()))); SetSession("BlockTime", s); }
                chk = DateTime.Parse(s);// Tools.SwapDate(s));
                int result = DateTime.Compare(n, chk);

                //TimeSpan spanForMinutes = n - chk;
                if (result > 0)
                {
                    SetSession("IsBlock", "0");
                    SetSession("BlockTime", "");
                    SetSession("CountErrorLogin", "0");
                    HTTP_CODE.WriteLogAction("Hết hạn khóa", this);
                }
                else
                {
                    HTTP_CODE.WriteLogAction("Khóa đến: " + s, this);
                    SetSession("IsBlock", "1");
                }
            }
        }
        public string DisableByRole(string SysU, string Id = "")
        {
            //string Creator = GetSession("UserID");
            //if (Creator == "") Creator = "0";.Split(new string[] { ";" }, StringSplitOptions.None);
            //if (long.Parse(Creator) != SysU && !CheckRoles(804))
            if (!CheckEditDeleteRoles(SysU, Id))
                return "disabled";
            else
                return "";
        }
        public bool CheckEditDeleteRoles(string SysU, string Id = "") //804 - Overwrite;805 - Attachments
        {
            string UserID = GetSession("UserID"); string SysU1 = "";
            try { SysU1 = enc.Decrypt(SysU); } catch { SysU1 = SysU; }
            bool kt = (SysU1.ToLower() == UserID.ToLower());
            if (!kt)
            {
                string[] a = SysU1.Split(new string[] { "||" }, StringSplitOptions.None);
                if (a.Length > 2)
                {
                    if (Id == "") Id = GetRequestVal(a[0]);
                    kt = (Id == a[1] && a[2].ToLower() == UserID.ToLower());
                }
            }
            return (CheckRoles(804) || kt);
        }
        public bool CheckApproved(long FucntionID = 0, long BaseTabID = 0, string BaseTabCode = "")
        {
            bool kt = CheckPermistion("IsApproved", FucntionID, BaseTabID, BaseTabCode);
            if (!kt) kt = CheckPermistion("IsPublish", FucntionID, BaseTabID, BaseTabCode);
            if (!kt) kt = CheckPermistion("IsDelete", FucntionID, BaseTabID, BaseTabCode);
            return kt;
        }
        public bool CheckRoles(long RoleID = 804) //804 - Overwrite;805 - Attachments
        {
            return CheckPermistion("IsRole", RoleID);
        }
        public void SetPermistion(ref bool IsInsert, ref bool IsUpdate, ref bool IsDelete, string FormEditType = "Normal", string IsSys = "0",
            string ActionType = "", bool IsPublish = false, bool IsApply = false)
        {
            if (IsSys == "1" && !IsSysAdmin())
            {
                IsInsert = false; IsUpdate = false; IsDelete = false;
            }
            else
            {
                switch (FormEditType)
                {
                    case "ViewOnly":
                        IsInsert = false; IsUpdate = false; IsDelete = false;
                        if (ActionType != "-1" && (IsPublish || IsApply)) IsDelete = true;
                        break;
                    case "InsertOnly":
                        IsUpdate = false; IsDelete = false;
                        if (ActionType != "-1" && (IsPublish || IsApply)) IsDelete = true;
                        break;
                    case "UpdateOnly":
                        IsInsert = false; IsDelete = false;
                        if (ActionType != "-1" && (IsPublish || IsApply)) IsDelete = true;
                        break;
                    case "InsertDeny":
                        IsInsert = false;
                        break;
                    case "UpdateDeny":
                        IsUpdate = false;
                        break;
                    case "InsertUpdateDeleteDe":
                        IsUpdate = false;
                        IsInsert = false;
                        IsDelete = false;
                        if (ActionType != "-1" && (IsPublish || IsApply)) IsDelete = true;
                        break;
                }
            }
        }
        public bool CheckPermistion(string Pers, long FucntionID = 0, long BaseTabID = 0, string BaseTabCode = "")
        {
            string per = GetSession("PermissionList");
            if (per == "") return false;
            dynamic d = JObject.Parse(per);
            bool kt = false; bool ktMatch = false; int i = 0;
            if (FucntionID <= 0 && BaseTabID <= 0 && BaseTabCode == "") return kt;
            while (!ktMatch && i < d.Authen.Items.Count)
            {

                long MenuItemID = Tools.GetDataJson(d.Authen.Items[i], "MenuID", "bigint");
                long TabID = Tools.GetDataJson(d.Authen.Items[i], "TabID", "bigint");
                string TabCode = Tools.GetDataJson(d.Authen.Items[i], "TabCode");
                int IsGrant = Tools.GetDataJson(d.Authen.Items[i], "IsGrant", "int");
                int IsInsert = Tools.GetDataJson(d.Authen.Items[i], "IsInsert", "int");
                int IsUpdate = Tools.GetDataJson(d.Authen.Items[i], "IsUpdate", "int");
                int IsDelete = Tools.GetDataJson(d.Authen.Items[i], "IsDelete", "int");
                int IsReview = Tools.GetDataJson(d.Authen.Items[i], "IsReview", "int");
                int IsPublish = Tools.GetDataJson(d.Authen.Items[i], "IsPublish", "int");
                int IsApproved = Tools.GetDataJson(d.Authen.Items[i], "IsApproved", "int");
                int IsRole = Tools.GetDataJson(d.Authen.Items[i], "IsRole", "int");
                if ((FucntionID == MenuItemID && FucntionID > 0) || (BaseTabID == TabID && BaseTabID > 0) || (BaseTabCode == TabCode && TabCode != ""))
                {
                    ktMatch = true;
                    switch (Pers)
                    {
                        case "IsGrant": //IsGrant
                            if ((IsGrant == 1) || (IsInsert == 1) || (IsUpdate == 1) || (IsDelete == 1) || (IsDelete == 1) || (IsReview == 1) || (IsApproved == 1)) kt = true;
                            break;
                        case "IsInsert": //IsInsert
                            if (IsInsert == 1 || IsUpdate == 1 || IsDelete == 1) kt = true;
                            break;
                        case "IsUpdate": //IsUpdate
                            if (IsUpdate == 1 || IsDelete == 1) kt = true;
                            break;
                        case "IsDelete": //IsDelete
                            if (IsDelete == 1) kt = true;
                            break;
                        case "IsReview": //IsReview
                            if (IsReview == 1 || IsPublish == 1) kt = true;
                            break;
                        case "IsPublish": //IsPublish
                            if (IsPublish == 1) kt = true;
                            break;
                        case "IsApproved": //IsApproved
                            if (IsApproved == 1) kt = true;
                            break;
                        case "IsRole": //IsApproved
                            if (IsRole == 1) kt = true;
                            break;
                    }
                }
                i++;
            }
            HTTP_CODE.WriteLogAction("functionName:CheckPermistion\nPers:" + Pers + "\nFucntionID:" + FucntionID + "\nBaseTabID:" + BaseTabID + "\nBaseTabCode:" + BaseTabCode + "\nkt:" + kt, this);
            return kt;
        }
        public bool CheckPermistion(long FucntionID = 0)
        {
            string UrlPath = GetUrlPage();
            string per = GetSession("PermissionList");
            if (per == "") return false;
            dynamic d = JObject.Parse(per);
            bool kt = false; bool ktMatch = false; int i = 0;
            if (FucntionID <= 0 || UrlPath == "") return kt;
            while (!ktMatch && i < d.Authen.Items.Count)
            {

                long MenuItemID = Tools.GetDataJson(d.Authen.Items[i], "MenuID", "bigint");
                if (FucntionID == MenuItemID && FucntionID > 0) ktMatch = true;
                i++;
            }
            if (ktMatch)
            {
                i = i - 1;
                kt = (UrlPath == Tools.GetDataJson(d.Authen.Items[i], "Url"));
            }

            return kt;
        }
        public string CheckLogin(ref ToolDAO toolDAO)
        {
            string Token = ""; string UserID = ""; string s = "";
            Token = GetSession("Token");
            UserID = GetSession("UserID");
            dynamic d; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            bool IsCached = false;
            try { IsCached = _cache.Get("CheckLogin_" + UserID + "_" + Token, out s); } catch { IsCached = false; }
            //if (IsCached)
            //{
            //    HTTP_CODE.WriteLogAction("CheckLogin_" + UserID + "_" + Token + ": " + s, this);
            //}
            //else
            if (!IsCached)
            {
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"UserID\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + UserID + "\"}," +
                    "{\"ParamName\":\"Token\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"" + Token + "\"}," +
                    "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");

                if ((Token != "") && (UserID != ""))
                {
                    toolDAO.ExecuteStore("CheckLogin", "SP_CMS__Users_LoginWithToken", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                    parameterOutput = "\"ParameterOutput\": {\"Message\":\"Token sai\",\"ResponseStatus\":\"1\"}";
                    d = JObject.Parse("{" + parameterOutput + "}");
                    if ((long)d.ParameterOutput.ResponseStatus > 0)
                    {
                        s = "";
                        _cache.Set("CheckLogin_" + UserID + "_" + Token, s, TimeCache);
                    }
                    else
                        s = GetLanguageLable(d.ParameterOutput.Message.ToString());
                }
                else
                    s = GetLanguageLable("NotLogin");

                //HTTP_CODE.WriteLogAction("functionName:CheckLogin\nUserID:" + UserID + "\nToken:" + Token + "\ns:" + s, this);
            }
            return s;
        }
        public string InputDataSetParam(string s, string t = "", string tSplit = "*")
        {
            string s1 = "";
            if (s != "")
            {
                // thay the Session
                //s = ReplaceListSessionValue(s);
                string[] b = s.Split(new string[] { "||" }, StringSplitOptions.None);
                string[] c = t.Split(new string[] { tSplit }, StringSplitOptions.None);
                for (int i = 0; i < b.Length; i++)
                {
                    string[] b1 = b[i].Split(new string[] { "|" }, StringSplitOptions.None);
                    string val = GetRequestVal(b1[0]);
                    if (i < c.Length)// && val == "")
                    {
                        string[] aType = c[i].Split(new string[] { ":" }, StringSplitOptions.None);
                        if (aType.Length > 1)
                        {
                            val = Tools.ParseValue(this, aType[0], val, aType[1]);
                            if (val == "") val = aType[1];
                        }
                    }

                    if (val == "") val = Tools.ParseValue(this, b1[4], true);
                    if (val == "0")
                    {
                        if (b1[0] == "Page") val = "1"; else if (b1[0] == "PageSize") val = GetSession("PageSizeReport");
                    }
                    //if (val == " ") val = "";
                    if (b1[0] == "Creator") val = GetSession("UserID");
                    val = val.Trim();
                    s1 = s1 + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":\"" + val + "\"}";
                }
            }
            if (s1 != "")
            {
                s = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(s1) + "]}";
            }
            return s;
        }
        public string InputDataSetParamStr(string s)
        {
            string s1 = "";
            if (s != "")
            {
                string[] b = s.Split(new string[] { "||" }, StringSplitOptions.None);
                for (int i = 0; i < b.Length; i++)
                {
                    string[] b1 = b[i].Split(new string[] { "|" }, StringSplitOptions.None);
                    s1 = s1 + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":\"" + b1[4] + "\"}";
                }
            }
            if (s1 != "")
            {
                s = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(s1) + "]}";
            }
            return s;
        }
        #endregion
    }
}