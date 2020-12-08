using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Utils;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace HRScripting.Controllers
{
    public class APIController : Controller
    {
        #region Private Method
        const string clientID = "HRMMOBILE";
        private HRSCache _cache;
        public APIController(IMemoryCache memoryCache)
        {
            _cache = new HRSCache(memoryCache);
        }
        private string BuildForm(HRSContext _context, string UrlAction = "/API/GetParamAPI",
            string ClientID = "",       bool IsClient = true, 
            string FunctionName = "",   bool IsFunctionName = true, 
            string Username = "",       bool IsUsername = true,
            string Company = "",        bool IsCompany = false)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append("<h1>" + _context.GetLanguageLable("GetParamAPIPageTitle") + "</h1><hr>");
            r1.Append("<form name=\"bosfrm\" method=\"POST\" action=\"" + UrlAction + "\">" +
                Environment.NewLine + "<div class=\"row inline-input\">" +
                        Environment.NewLine + "<div class=\"col-sm-4\">" +
                        "<div class=\"form-group row\">");
            r1.Append(Environment.NewLine + UIDef.UIButton("bntSubmit", _context.GetLanguageLable("Search"), true, " class=\"btn save\"") + "</div>");
            r1.Append(UIDef.UIHidden("IsPOST", "1"));
            r1.Append(UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()));
            r1.Append(UIDef.UIHidden("MenuID", _context.GetRequestVal("MenuID")));
            //r1.Append("<tr><td colspan=2><b>" + _context.GetLanguageLable("GetParamAPIPageTitle") + "</b></td></tr> ");
            if (IsClient) r1.Append("<div class=\"form-group row\">" +
                Environment.NewLine + "<label class=\"col-form-label active\">" + _context.GetLanguageLable("GetParamAPIClientID") + "</label>" +
                UIDef.UITextbox(_context.GetLanguageLable("GetParamAPIClientID"), "ClientID", ClientID, " size=20 maxlength=30") + "</div>");//<tr><td>  </td><td> </td></tr>
            if (IsCompany) r1.Append("<div class=\"form-group row\">" +
                Environment.NewLine + "<label class=\"col-form-label active\">" + _context.GetLanguageLable("GetParamAPICompany") + "</label>" +
                UIDef.UITextbox(_context.GetLanguageLable("GetParamAPICompany"), "Company", Company, " size=40 maxlength=100") + "</div> ");
            if (IsFunctionName) r1.Append("<div class=\"form-group row\">" +
                Environment.NewLine + "<label class=\"col-form-label active\">" + _context.GetLanguageLable("GetParamAPIFunctionName") + "</label>" +
                UIDef.UITextbox(_context.GetLanguageLable("GetParamAPIFunctionName"), "FunctionName", FunctionName, " size=40 maxlength=100") + "</div> ");
            if (IsUsername) r1.Append("<div class=\"form-group row\">" +
                Environment.NewLine + "<label class=\"col-form-label active\">" + _context.GetLanguageLable("GetParamAPIUsername") + "</label>" +
                UIDef.UITextbox(_context.GetLanguageLable("GetParamAPIUsername"), "Username", Username, " size=20 maxlength=30") + "</div> ");
            if (IsUsername) r1.Append("<div class=\"form-group row\">" +
                Environment.NewLine + "<label class=\"col-form-label active\">" + _context.GetLanguageLable("GetParamAPIPassword") + "</label>" +
                UIDef.UIPassword("Password", " size=20 maxlength=30") + "</div>" +
                "</div></div> ");
            //r1.Append("<tr><td></td><td> " + UIDef.UIButton("bntSubmit", _context.GetLanguageLable("Search"), true, " class=\"btn find\"") + " " + UIDef.UIButton("bntReset", _context.GetLanguageLable("Reset"), false, " class=\"btn refresh\"") + "</td></tr> ");
            r1.Append("</form>"); //</table>
            r = r1.ToString();
            r1 = null;
            return r;
        }
        #endregion

        #region Method
        public IActionResult Index(string functionName)
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context);
            string l = _context.GetSession("language");
            HTTP_CODE.WriteLogAction("functionName:/API/Index\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName"), _context);
            if (l == null || l == "") l = _context.LanguageDefault;
            //ViewData["language"] = UIDef.UILanguage(l, "/API/Index");
            bool MenuOn = (_context.GetRequestMenuOn() != "Off");
            string MenuID = _context.GetRequestVal("MenuID");

            StringBuilder r1 = new StringBuilder();
            //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn));
            r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn));
            r1.Append(UIDef.UIContentTagOpen (ref bosDAO, _context,MenuOn, "0", false));
            string ClientID = _context.GetFormValue("ClientID");
            string FunctionName = _context.GetFormValue("FunctionName");
            string Username = _context.GetFormValue("Username");
            string Password = _context.GetFormValue("Password");
            r1.Append(BuildForm(_context, "/API/GetParamAPI", ClientID, true, FunctionName, true, Username, true)); // BuildForm(_context, ClientID, FunctionName, Username));
            r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));

            ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
            ViewData["IsPageLogin"] = !MenuOn;
            ViewData["Secure"] = (_context.GetRequestMenuOn()=="Min"?" sidebar-collapse":"");
            ViewData["PageTitle"] = "" + _context.GetLanguageLable("GetParamAPIPageTitle");// + "Nhập thông tin ClientID; Nghiệp vụ cần tra cứu.";
            ViewData["IndexBody"] = r1.ToString();
            ViewData["iframe"] = _context.GetRequestVal("iframe");
            ViewData["txtClose"] = _context.GetLanguageLable("Close");
            r1 = null;
            HTTP_CODE.WriteLogAction("functionName:/API/Index\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName"), _context);
            return View();
        }

        [Route("API/MobileGetParam/{functionName}")]
        [HttpGet]
        [HttpPost]
        public IActionResult MobileGetParam(string functionName)
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context);
            bool MenuOn = (_context.GetRequestMenuOn() != "Off");
            // check login
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
            string Company = _context.GetRequestVal("Company");
            //if (!_context.CheckPermistion(int.Parse(MenuID)))
            //{
            //    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
            //}
            functionName = functionName.ToLower();
            StringBuilder r1 = new StringBuilder();
            //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, MenuID));
            r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, MenuID));
            r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, MenuID, false));
            r1.Append(BuildForm(_context, "/API/MobileGetParam/" + functionName, "", false, "", false, "", false, Company, true));

            if (Company != "")
            {
                r1.Append("<hr><h1>" + _context.GetLanguageLable("GetParamAPIPageTitle") + "</h1>");
                string config = _context.APIConfig.ReadConfig(_context);
                try
                {
                    dynamic d = JObject.Parse(config);
                    int i = 0; bool kt = false;
                    while (!kt && i < d.config.Count)
                    {
                        if (d.config[i].clientId.ToString() == clientID) kt = true; i++;
                    }
                    if (kt)
                    {
                        i = i - 1;
                        var d1 = d.config[i].functionListName;
                        i = 0; kt = false;
                        while (i < d1.Count && !kt)
                        {
                            if (d1[i].FunctionName.ToString().ToLower() == functionName) kt = true; i++;
                        }
                        if (kt)
                        {
                            i--; string jsonIn = ""; string StoreName = d1[i].StoreProceduceName.ToString();
                            d = d1[i].ParamIn;
                            for (int j = 0; j < d.Count; j++)
                            {
                                if (d[j].IsHidden.ToString() == "0") jsonIn = jsonIn + "\"" + d[j].ParamName + "\":\"" + d[j].ParamType + "(" + d[j].ParamLength + ")\",";
                            }
                            if (jsonIn != "")
                            {
                                jsonIn = Tools.RemoveLastChar(jsonIn);
                                jsonIn = "{" + jsonIn + "}";
                            }
                            r1.Append("<hr><b>" + _context.GetLanguageLable("GetParamAPIJsonRequest") + "</b>: " + jsonIn);
                            string jsonOut = "";
                            d = d1[i].ParamOut;
                            for (int j = 0; j < d.Count; j++)
                            {
                                jsonOut = jsonOut + "\"" + d[j].ParamName + "\":\"" + d[j].ParamType + "(" + d[j].ParamLength + ")\",";
                            }
                            if (jsonOut != "")
                            {
                                jsonOut = Tools.RemoveLastChar(jsonOut);
                                jsonOut = "{" + jsonOut + "}";
                            }
                            else
                            {
                                jsonOut = "null";
                            }

                            ToolDAO db = new ToolDAO(Company.ToUpper(),_context);
                            string parameterOutput = ""; int errorCode = 0; string errorString = ""; string json = "";
                            d = JObject.Parse("{\"parameterInput\":[" +
                                "{\"ParamName\":\"Company\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"40\", \"InputValue\":\"" + Company + "\"}," +
                                "{\"ParamName\":\"StoreName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + StoreName + "\"}," +
                                "{\"ParamName\":\"query\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"2000\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"RequestJson\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"ResponseJson\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"Param\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"Column\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"ParamExecInput\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"ParamInput\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"ColumnInput\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"ParamIn\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}," +
                                "{\"ParamName\":\"ParamOut\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}]}");
                            db.ExecuteStore("getParamAPI", "BOS.SP_CONFIG__ExecuteStore", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                            if (errorCode == 200)
                            {
                                d = JObject.Parse("{" + parameterOutput + "}");
                                d1 = JObject.Parse("{" + d.ParameterOutput.ResponseJson.ToString() + "}");
                                if (d1.Items != null)
                                {
                                    r1.Append("<hr><b>" + _context.GetLanguageLable("GetParamAPIJsonResponse") + "</b>: {\"ParameterOutput\":" + jsonOut + ",\"Count\":0,\"ItemType\":" + d1.Items[0].ToString().Replace("\"", "\\\"") + ",\"Items\":[" + d1.Items[0].ToString().Replace("\"", "\\\"") + "]}");
                                }
                                else
                                    r1.Append("<hr><b>" + _context.GetLanguageLable("GetParamAPIJsonResponse") + "</b>: {\"ParameterOutput\":" + jsonOut + ",\"Count\":0,\"ItemType\":null,\"Items\":[]}");
                            }
                            else
                                r1.Append("<hr><font color=\"red\">" + _context.GetLanguageLable("GetParamAPIErrorJsonResponse") + "</font>");
                        }
                        else
                        {
                            r1.Append("<hr><font color=\"red\">" + _context.GetLanguageLable("GetParamAPIErrorFunctionNotFound") + "</font>");
                        }
                    }
                    else
                    {
                        r1.Append("<hr><font color=\"red\">" + _context.GetLanguageLable("GetParamAPIErrorClientIDNotFound") + "</font>");
                    }
                }
                catch (Exception Ex)
                {
                    r1.Append("<hr><font color=\"red\">" + _context.GetLanguageLable("GetParamAPIErrorException") + "</font>" + Ex.ToString());
                }
            }            

            r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));

            //ViewData["Title"] = "Tra cứu Parameter API";
            ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
            ViewData["IsPageLogin"] = !MenuOn;
            ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
            ViewData["PageTitle"] = "" + _context.GetLanguageLable("GetParamAPIPageTitle");// + "Nhập thông tin ClientID; Nghiệp vụ cần tra cứu.";
            ViewData["IndexBody"] = r1.ToString();
            ViewData["iframe"] = _context.GetRequestVal("iframe");
            ViewData["txtClose"] = _context.GetLanguageLable("Close");
            r1 = null;
            return View();
        }

        [Route("API/Mobile/{functionName}")]
        [HttpPost]
        public IActionResult Mobile(string functionName)
        {
            #region Variable
            int ResponseCode; string ResponseMessage; string Data = "null";
            string lang = Request.Headers["Language"]; if (lang == null) lang = ""; if (lang == "") lang = "vi";
            HRSContext _context = new HRSContext(HttpContext, _cache, lang);
            dynamic d;
            string clientContentType; string clientAccept;
            #endregion

            #region check Request API
            // Get config
            HTTP_CODE.WriteLogServiceIn("1. Get and Check file jsonConfig", _context);
            try
            {
                d = _context.APIConfig.ReadConfigToJson(_context);
            }
            catch (Exception e)
            {
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;//.HTTP_ERROR_SERVER;
                string msg = e.ToString().Replace("'", "~").Replace(System.Environment.NewLine, "%3Cbr%3E").Replace("\\", "\\\\");
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Lỗi không tồn tại file config: " + msg + "\nHoặc lỗi không đọc được định dạng Json file config: " + msg + "\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }
            // check HTTPS
            bool IsHttpsReq = (d.Https.ToString() == "1");    
            if (IsHttpsReq)
            {
                HTTP_CODE.WriteLogServiceIn("2. Check HTTPS");
                if (!Request.IsHttps)
                {
                    ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"2. Lỗi connect HTTPS\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage);
                    return StatusCode(ResponseCode);
                }
            }
            // Check Accept; Content-Type
            HTTP_CODE.WriteLogServiceIn("3. Get and Check thông tin Accept/Content-Type", _context);
            clientContentType = Request.Headers["Content-Type"];
            clientAccept = Request.Headers["Accept"];
            if (clientAccept.ToLower() != "application/json")
            {
                HTTP_CODE.WriteLogServiceIn("3.1. Accept fail", _context);
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"3.1. Accept fail\", \"Data\": " + Data + "}";                
                HTTP_CODE.WriteLogServiceIn(ResponseMessage);
                return StatusCode(ResponseCode);
            }
            if (clientContentType.ToLower() != "application/json")
            {
                HTTP_CODE.WriteLogServiceIn("3.2. Content-Type fail", _context);
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"3.2. Content-Type fail\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage);
                return StatusCode(ResponseCode);
            }

            // check clientid
            bool kt = false; int i = 0; //int j = 0; string pwd;
            HTTP_CODE.WriteLogServiceIn("4. Check Client ID", _context);
            while ((i < d.config.Count) && !kt)
            {
                if (d.config[i].clientId.ToString().ToUpper() == clientID.ToUpper())
                {
                    kt = true;
                }
                i++;
            }
            if (!kt)
            {
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"4. Không tìm thấy CLIENTID\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }
            i = i - 1; // giảm 1 về vị trí tìm thấy

            // Check nghiệp vụ
            string deviceID = ""; string companyCode = ""; string userID = ""; string token = "";
            HTTP_CODE.WriteLogServiceIn("5. Get and Check nghiệp vụ yêu cầu", _context);
            if (functionName == null)
            {
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"5. Function name is null!\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }
            functionName = functionName.ToLower();
            switch (functionName)
            {
                case "login":
                    break;
                default: // check Authen /// DeviceID^CompanyCode^UserID^Token
                    HTTP_CODE.WriteLogServiceIn("6. Get and Check thông tin authen", _context);
                    string authHeader = Request.Headers["Authorization"];
                    if (authHeader != null)
                    {
                        try
                        {
                            string txtAuthorization = HTTP_CODE.Base64Decode(Tools.RemoveFisrtChar(authHeader, "Bearer ".Length).Trim());
                            string[] arrAuthorization = txtAuthorization.Split(new string[] { "^" }, StringSplitOptions.None);
                            deviceID = arrAuthorization[0];
                            companyCode = arrAuthorization[1];
                            userID = arrAuthorization[2];
                            token = arrAuthorization[3];
                        }
                        catch (Exception e)
                        {
                            ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                            string msg = e.ToString().Replace("'", "~").Replace(System.Environment.NewLine, "%3Cbr%3E").Replace("\\", "\\\\");
                            ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"6. Authorization wrong config: " + msg + "\", \"Data\": " + Data + "}";
                            HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                            return StatusCode(ResponseCode);
                        }
                    }
                    else
                    {
                        ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                        ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"6. Authorization is null!\", \"Data\": " + Data + "}";
                        HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                        return StatusCode(ResponseCode);
                    }
                    break;
            }
            kt = false; int j = 0; dynamic d1 = d.config[i].functionListName; string _functionName = "";
            while ((j < d1.Count) && !kt)
            {
                _functionName = d1[j].FunctionName;
                HTTP_CODE.WriteLogServiceIn("5. " + j + ": Check nghiệp vụ " + _functionName + "||" + functionName, _context);
                if (functionName == _functionName.ToLower()) kt = true;
                j++;
            }
            if (!kt)
            {
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"5. Chức năng không hợp lệ\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }
            j = j - 1; d1 = d.config[i].functionListName[j];// d1 - function

            // get request
            HTTP_CODE.WriteLogServiceIn("7. Request data", _context);
            string content = "";
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                content = reader.ReadToEnd();
            }
            if (content == null || content == "") content = "";
            
            dynamic varJsonRequest = null;
            HTTP_CODE.WriteLogServiceIn("7. Get and Check Json Request", _context);
            if (content != "")
            {
                try
                {
                    varJsonRequest = JObject.Parse(content);
                }
                catch
                {
                    ResponseCode = HTTP_CODE.HTTP_BAD_REQUEST;
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"7. Request no json: " + content + "\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                    return StatusCode(ResponseCode);
                }
            }

            string StoreName = d1.StoreProceduceName.ToString();
            bool IsInputList = (d1.IsParamInputList.ToString() == "1");
            string StoreParam = ""; dynamic dParam = null;
            try
            {
                if (varJsonRequest != null)
                {
                    for (j = 0; j < d1.ParamIn.Count; j++) // Param In
                    {
                        if (functionName != "login" && d1.ParamIn[j].ParamName.ToString().ToLower() == "companycode")
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), companyCode);
                        else if (functionName != "login" && d1.ParamIn[j].ParamName.ToString().ToLower() == "deviceid")
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), deviceID);
                        else if (functionName != "login" && d1.ParamIn[j].ParamName.ToString().ToLower() == "userid")
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), userID);
                        else if (functionName != "login" && d1.ParamIn[j].ParamName.ToString().ToLower() == "token")
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), token);
                        else
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), varJsonRequest[d1.ParamIn[j].ParamName.ToString()].ToString());
                    }
                    for (j = 0; j < d1.ParamOut.Count; j++) // Param In
                    {
                        StoreParam = StoreParam + "," + d1.ParamOut[j].ParamJson.ToString();
                    }
                }
                else
                {
                    for (j = 0; j < d1.ParamIn.Count; j++) // Param In
                    {
                        if (functionName != "login" && d1.ParamIn[j].ParamName.ToString().ToLower() == "companycode")
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), companyCode);
                        else if (functionName != "login" && d1.ParamIn[j].ParamName.ToString().ToLower() == "deviceid")
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), deviceID);
                        else if (functionName != "login" && d1.ParamIn[j].ParamName.ToString().ToLower() == "userid")
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), userID);
                        else if (functionName != "login" && d1.ParamIn[j].ParamName.ToString().ToLower() == "token")
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), token);
                        else
                            StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString();
                    }
                    for (j = 0; j < d1.ParamOut.Count; j++) // Param In
                    {
                        StoreParam = StoreParam + "," + d1.ParamOut[j].ParamJson.ToString();
                    }
                }
            }
            catch
            {
                ResponseCode = HTTP_CODE.HTTP_BAD_REQUEST;
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"7. Request no json: " + content + "\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }
            if (StoreParam != "")
            {
                StoreParam = Tools.RemoveFisrtChar(StoreParam);
                dParam = JObject.Parse("{\"parameterInput\":[" + StoreParam + "]}");
            }            

            Data = "{\"deviceID\": \"" + deviceID + "\", \"companyCode\": \"" + companyCode + "\", \"userID\": \"" + userID + "\", \"token\": \"" + token +
                "\", \"clientID\": \"" + clientID + "\", \"functionName\": \"" + functionName + "\", \"clientContentType\": \"" + clientContentType +
                "\", \"clientAccept\": \"" + clientAccept + "\", \"jsonRequest\": \"" + content + "\", \"StoreParam\": \"" + StoreParam + "\"}";

            #endregion

            #region Runing API
            ToolDAO db; string parameterOutput = ""; string json = ""; int errorCode = HTTP_CODE.HTTP_ACCEPT; string errorString = "";
            try
            {
                HTTP_CODE.WriteLogServiceIn("8. Function [" + functionName + "] ==> Starting execute DATA", _context);                
                switch (functionName)
                {
                    case "logout":
                    case "loginwithtoken":
                        HTTP_CODE.WriteLogServiceIn("8.1. Function [" + functionName + "] ==> Khởi tạo connect string DB", _context);
                        db = new ToolDAO(_context);
                        break;
                    case "login":
                        if (varJsonRequest == null)
                        {
                            ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                            ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"8. Function [" + functionName + "] ==> Request Is Null\", \"Data\": " + Data + "}";
                            HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                            return StatusCode(ResponseCode);
                        }
                        HTTP_CODE.WriteLogServiceIn("8.1. Function [" + functionName + "] ==> Khởi tạo connect string DB", _context);
                        db = new ToolDAO(_context);
                        break;                 
                    default:
                        bool Chk = false; json = "";
                        Chk = CheckToken(_context, functionName, companyCode, userID, deviceID, token, out json);
                        if (!Chk)
                        {
                            d = JObject.Parse(json);
                            return Json(d[functionName]);// Bỏ functionname
                        }
                        Chk = false; json = "";
                        Chk = CheckPermission(_context, userID, functionName, out json);
                        if (!Chk)
                        {
                            d = JObject.Parse(json);
                            return Json(d[functionName]);// Bỏ functionname
                        }
                        HTTP_CODE.WriteLogServiceIn("8.1. Function [" + functionName + "] ==> Khởi tạo connect string DB/ Check quyền", _context);
                        db = new ToolDAO(companyCode, _context);
                        break;
                }
                if (functionName == "login")
                {
                    string Provider; string ServerID;string Username;string UserPassword; string DeviceID;
                    try { Provider = varJsonRequest.Provider.ToString(); } catch { Provider = "1"; }
                    try { ServerID = varJsonRequest.ServerID.ToString(); } catch { ServerID = "0"; }
                    try { Username = varJsonRequest.UserName.ToString(); } catch { Username = ""; }
                    try { UserPassword = varJsonRequest.Password.ToString(); } catch { UserPassword = ""; }
                    try { DeviceID = varJsonRequest.DeviceID.ToString(); } catch { DeviceID = ""; }
                    Authen authen = new Authen(_context, db);
                    if (Provider == "3") // Email User
                    {
                        bool ktt = authen.LoginWithEmail(ServerID, Username, UserPassword, DeviceID, ref json, ref errorCode, ref errorString);
                        if (!ktt) json = "{\"login\":{\"ResponseStatus\":\"-600\", \"Message\":\"UserOrPasswordIsWrong\"}}";
                    }
                    else if (Provider == "2") // AD User
                    {
                        bool ktt = authen.LoginWithADUser(ServerID, Username, UserPassword, DeviceID, ref json, ref errorCode, ref errorString);
                        if (!ktt) json = "{\"login\":{\"ResponseStatus\":\"-600\", \"Message\":\"UserOrPasswordIsWrong\"}}";
                    }
                    else // Histaff User
                    {
                        authen.LoginWithHistaffUser(Username, UserPassword, DeviceID, ref json, ref errorCode, ref errorString);
                    }
                }
                else
                {
                    db.ExecuteStoreAPI(functionName, StoreName, dParam, ref parameterOutput, ref json, ref errorCode, ref errorString);
                }                    
                if (errorCode != HTTP_CODE.HTTP_ACCEPT)
                {
                    HTTP_CODE.WriteLogServiceIn("8.3. Function [" + functionName + "] ==> Lỗi: " + errorString + "; json: " + json, _context);
                    return StatusCode(errorCode);
                }
                d = JObject.Parse(json);
                long status = 1; bool IsResponseStatus = true; string Msg = "";
                try { status = long.Parse(d[functionName].ResponseStatus.ToString()); Msg = _context.GetLanguageLable(d[functionName].Message.ToString()); } catch { Msg = ""; status = 1; IsResponseStatus = false; }//(Exception ex)
                if (functionName == "login" && status > 0)
                {
                    deviceID = varJsonRequest["DeviceID"].ToString();
                    //companyCode = d[functionName].ParameterOutput.CompanyCode.ToString();
                    //userID = d[functionName].ParameterOutput.UserID.ToString();
                    //token = d[functionName].ParameterOutput.Token.ToString();
                    //d[functionName].ParameterOutput["Token"] = HTTP_CODE.Base64Encode(deviceID + "^" + companyCode + "^" + userID + "^" + token);//DeviceID^CompanyCode^UserID^Token
                    ////companyCode = d[functionName].CompanyCode.ToString();
                    ////userID = d[functionName].UserID.ToString();
                    ////token = d[functionName].Token.ToString();
                    ////d[functionName]["Token"] = HTTP_CODE.Base64Encode(deviceID + "^" + companyCode + "^" + userID + "^" + token);//DeviceID^CompanyCode^UserID^Token
                    companyCode = d[functionName].ParameterOutput.CompanyCode.ToString();
                    userID = d[functionName].ParameterOutput.UserID.ToString();
                    token = d[functionName].ParameterOutput.Token.ToString();
                    d[functionName].ParameterOutput["Token"] = HTTP_CODE.Base64Encode(deviceID + "^" + companyCode + "^" + userID + "^" + token);//DeviceID^CompanyCode^UserID^Token
                }
                
                if (!IsResponseStatus)
                    return Json(d[functionName]);// Không có ResponseStatus
                else if (status > 0)
                    return Json(d[functionName]);// ResponseStatus > 0
                else                             // ResponseStatus < 1
                    return Json(JObject.Parse("{\"ResponseStatus\": " + status.ToString() + ", \"Message\": \"" + Msg + "\"}"));
            }
            catch (Exception e)
            {
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                string msg = e.ToString().Replace("'", "~").Replace(System.Environment.NewLine, "%3Cbr%3E").Replace("\\", "\\\\");
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"8. Function [" + functionName + "] ==> " + msg + "\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }
            #endregion
        }
        private bool CheckToken(HRSContext _context, string functionName, string companyCode, string userID, string deviceID, string token, out string json)
        {
            HTTP_CODE.WriteLogServiceIn("8.2. Function [CheckToken] ==> Khởi tạo connect string DB", _context);
            ToolDAO db = new ToolDAO(_context); string parameterOutput = ""; json = ""; int errorCode = HTTP_CODE.HTTP_ACCEPT; string errorString = "";
            dynamic d = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\":\"CompanyCode\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + companyCode + "\"}," +
                "{\"ParamName\":\"UserID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + userID + "\"}," +
                "{\"ParamName\":\"DeviceID\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"100\", \"InputValue\":\"" + deviceID + "\"}," +
                "{\"ParamName\":\"Token\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"" + token + "\"}," +
                "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
            db.ExecuteStoreAPI(functionName, "SP_API__Users_LoginWithToken", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            HTTP_CODE.WriteLogServiceIn("8.3. Function [CheckToken] ==> Data: " + json, _context);
            try
            {
                dynamic d1 = JObject.Parse(json); //Msg = d.ParameterOutput.Message.ToString();
                if ((long)d1[functionName].ResponseStatus > 0)
                    return true;
                else
                {
                    HTTP_CODE.WriteLogServiceIn("8.3. Function [CheckToken] ==> errorString: " + errorString + "; Message: " + d1[functionName].Message.ToString(), _context);
                    return false;
                }
            }
            catch (Exception e)
            {
                //Msg = "Có lỗi!";
                HTTP_CODE.WriteLogServiceIn("8.3. Function [CheckToken] ==> Error: " + e.ToString().Replace("'", "~").Replace(System.Environment.NewLine, "%3Cbr%3E").Replace("\\", "\\\\"), _context);
                return false;
            }
        }
        private bool CheckPermission(HRSContext _context, string userID, string functionName, out string json)
        {
            HTTP_CODE.WriteLogServiceIn("8.2. Function [CheckToken] ==> Khởi tạo connect string DB", _context);
            ToolDAO db = new ToolDAO(_context); string parameterOutput = ""; json = ""; int errorCode = HTTP_CODE.HTTP_ACCEPT; string errorString = "";
            dynamic d = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\":\"UserID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + userID + "\"}," +
                "{\"ParamName\":\"Url\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"/API/Mobile/" + functionName + "\"}," +
                "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
            db.ExecuteStoreAPI(functionName, "SP_API__Users_CheckPermission", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            HTTP_CODE.WriteLogServiceIn("8.4. Function [CheckPermission] ==> Data: " + json, _context);
            try
            {
                dynamic d1 = JObject.Parse(json); //Msg = d.ParameterOutput.Message.ToString();
                if ((long)d1[functionName].ResponseStatus > 0)
                    return true;
                else
                {
                    HTTP_CODE.WriteLogServiceIn("8.4. Function [CheckPermission] ==> errorString: " + errorString + "; Message: " + d1[functionName].Message.ToString(), _context);
                    return false;
                }
            }
            catch (Exception e)
            {
                //Msg = "Có lỗi!";
                HTTP_CODE.WriteLogServiceIn("8.4. Function [CheckPermission] ==> Error: " + e.ToString().Replace("'", "~").Replace(System.Environment.NewLine, "%3Cbr%3E").Replace("\\", "\\\\"), _context);
                return false;
            }
        }

        [Route("API/GetParamAPI/{functionName}")]
        [HttpGet]
        [HttpPost]
        public IActionResult GetParamAPI(string functionName)
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context);
            bool MenuOn = (_context.GetRequestMenuOn() != "Off");
            // check login
            //string l = _context.CheckLogin(ref bosDAO);
            //if (l != "")
            //{
            //    return Redirect(_context.ReturnUrlLogin(l));
            //}
            //else
            // {
            HTTP_CODE.WriteLogAction("functionName:/API/GetParamAPI\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName"), _context);
                string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                //if (!_context.CheckPermistion("IsGrant", int.Parse(MenuID), 0, ""))
                //{
                //    return Redirect("/Home");
                //}
                StringBuilder r1 = new StringBuilder();
                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, MenuID, false));
                string ClientID = _context.GetFormValue("ClientID");
                string FunctionName = _context.GetFormValue("FunctionName");
                string Username = _context.GetFormValue("Username");
                string Password = _context.GetFormValue("Password");
                r1.Append(BuildForm(_context, "/API/GetParamAPI", ClientID, true, FunctionName, true, Username, true));

                string IsPOST = _context.GetFormValue("IsPOST"); if (IsPOST == "") IsPOST = "0";
                if (IsPOST == "1" && ClientID != "" && FunctionName != "" && Username != "" && Password != "")
                {
                    r1.Append("<hr><h1>" + _context.GetLanguageLable("GetParamAPIPageTitle") + "</h1>");
                    dynamic enc = _context.enc;
                    string config = _context.APIConfig.ReadConfig(_context); string company; string username; string password; string spName = ""; int IsInputList;
                    try
                    {
                        dynamic d = JObject.Parse(config);
                        int i = 0; bool kt = false;
                        while (!kt && i < d.config.Count)
                        {
                            if (d.config[i].clientId.ToString() == ClientID) kt = true; i++;
                        }
                        if (kt)
                        {
                            i = i - 1;
                            company = d.config[i].companyCode.ToString(); username = d.config[i].username.ToString(); password = d.config[i].password.ToString();
                            try
                            {
                                password = enc.Decrypt(password);
                            }
                            catch { }
                            if (username == Username && password == Password)
                            {
                                var d1 = d.config[i].functionListName;
                                i = 0; kt = false;
                                while (i < d1.Count && !kt)
                                {
                                    if (d1[i].FunctionName.ToString() == FunctionName) kt = true; i++;
                                }
                                if (kt)
                                {
                                    i--; spName = d1[i].StoreProceduceName.ToString();
                                    IsInputList = d1[i].IsParamInputList;
                                    ToolDAO db = new ToolDAO(company, _context);
                                    string parameterOutput = ""; int errorCode = 0; string errorString = ""; string json = "";
                                    d = JObject.Parse("{\"parameterInput\":[" +
                                    "{\"ParamName\":\"Company\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"40\", \"InputValue\":\"" + company + "\"}," +
                                    "{\"ParamName\":\"StoreName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + spName + "\"}," +
                                    "{\"ParamName\":\"RequestJson\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"8000\", \"InputValue\":\"\"}," +
                                    "{\"ParamName\":\"ResponseJson\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"8000\", \"InputValue\":\"\"}," +
                                    "{\"ParamName\":\"ParamInputJson\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"8000\", \"InputValue\":\"\"}," +
                                    "{\"ParamName\":\"ColumnJson\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"2000\", \"InputValue\":\"\"}]}");
                                    db.ExecuteStore("getParamAPI", "SP_CONFIG__ExecuteStore", d, ref parameterOutput, ref json, ref errorCode, ref errorString);

                                    try
                                    {
                                        d = JObject.Parse("{" + parameterOutput + "}");
                                        if (IsInputList == 1)
                                        {
                                            r1.Append("<hr><b>" + _context.GetLanguageLable("GetParamAPIJsonRequest") + "</b>: {\"RequestList\":[" + d.ParameterOutput["@RequestJson"].ToString() + "]}"); //"@ResponseJson"
                                            r1.Append("<hr><b>" + _context.GetLanguageLable("GetParamAPIJsonResponse") + "</b>: {\"ResponseList\":[" + "{\"Error\":\"200\", \"Message\": \"Truy vấn thành công\", \"ParameterOutput\": {\"@Rowcount\":\"1\"}, \"Data\": {\"" + FunctionName + "\": {\"Count\": 1," + d.ParameterOutput["@ResponseJson"].ToString() + "}}]}");
                                        }
                                        else
                                        {
                                            r1.Append("<hr><b>" + _context.GetLanguageLable("GetParamAPIJsonRequest") + "</b>: " + d.ParameterOutput["@RequestJson"].ToString()); //"@ResponseJson"
                                            r1.Append("<hr><b>" + _context.GetLanguageLable("GetParamAPIJsonResponse") + "</b>: " + "{\"Error\":\"200\", \"Message\": \"Truy vấn thành công\", \"ParameterOutput\": {\"@Rowcount\":\"1\"}, \"Data\": {\"" + FunctionName + "\": {\"Count\": 1," + d.ParameterOutput["@ResponseJson"].ToString() + "}}");
                                        }
                                    }
                                    catch (Exception Ex)
                                    {
                                        r1.Append("<hr><b>parameterOutput</b>: " + parameterOutput);
                                        r1.Append("<hr><b>json</b>: " + json);
                                        r1.Append("<hr><b>Exception</b>: " + Ex.ToString());
                                    }
                                }
                                else
                                {
                                    r1.Append("<hr><font color=\"red\">" + _context.GetLanguageLable("GetParamAPIErrorFunctionNotFound") + "</font>");
                                }
                            }
                            else
                            {
                                r1.Append("<hr><font color=\"red\">" + _context.GetLanguageLable("GetParamAPIErrorAccountNotFound") + "</font>");
                            }
                        }
                        else
                        {
                            r1.Append("<hr><font color=\"red\">" + _context.GetLanguageLable("GetParamAPIErrorClientIDNotFound") + "</font>");
                        }
                    }
                    catch (Exception Ex)
                    {
                        r1.Append("<hr><font color=\"red\">" + _context.GetLanguageLable("GetParamAPIErrorException") + "</font>" + Ex.ToString());
                    }

                }
                r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));

                //ViewData["Title"] = "Tra cứu Parameter API";
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
                ViewData["IsPageLogin"] = !MenuOn;
                ViewData["Secure"] = (_context.GetRequestMenuOn()=="Min"?" sidebar-collapse":"");
                ViewData["PageTitle"] = "" + _context.GetLanguageLable("GetParamAPIPageTitle");// + "Nhập thông tin ClientID; Nghiệp vụ cần tra cứu.";
                ViewData["IndexBody"] = r1.ToString();
                ViewData["iframe"] = _context.GetRequestVal("iframe");
                ViewData["txtClose"] = _context.GetLanguageLable("Close");
                r1 = null;
                return View();
           // }
        }

        [Route("API/Server/{functionName}")]
        [HttpPost]
        public IActionResult Server(string functionName)
        {
            #region Variable
            int ResponseCode; string ResponseMessage; string Data = "null"; 
            string lang = Request.Headers["Language"]; if (lang == null) lang = ""; if (lang == "") lang = "vi";
            HRSContext _context = new HRSContext(HttpContext, _cache, lang);
            dynamic enc = _context.enc;
            dynamic d;dynamic d1;
            string SPName = ""; int IsInputList = 0; string authHeader; string txtAuthorization;
            string[] arrAuthorization; string companycode; string clientID; string username; string password;
            string clientIP; string clientContentType; string clientAccept;
            #endregion

            #region check Request API
            // Get config
            HTTP_CODE.WriteLogServiceIn("1. Get and Check file jsonConfig", _context);
            try
            {
                d = _context.APIConfig.ReadConfigToJson(_context);
            }
            catch (Exception e)
            {
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;//.HTTP_ERROR_SERVER;
                string msg = e.ToString().Replace("'", "~").Replace(System.Environment.NewLine, "%3Cbr%3E").Replace("\\", "\\\\");
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Lỗi không tồn tại file config: " + msg + "\nHoặc lỗi không đọc được định dạng Json file config: " + msg + "\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }
            // check HTTPS
            bool IsHttpsReq = (d.Https.ToString() == "1");
            if (IsHttpsReq)
            {
                HTTP_CODE.WriteLogServiceIn("2. Check HTTPS");
                if (!Request.IsHttps)
                {
                    ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"2. Lỗi connect HTTPS\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage);
                    return StatusCode(ResponseCode);
                }
            }

            HTTP_CODE.WriteLogServiceIn("6. Get and Check thông tin authen", _context);
            authHeader = Request.Headers["Authorization"];
            if (authHeader != null)
            {
                try
                {
                    txtAuthorization = HTTP_CODE.Base64Decode(Tools.RemoveFisrtChar(authHeader, "Bearer ".Length).Trim());
                    arrAuthorization = txtAuthorization.Split(new string[] { "^" }, StringSplitOptions.None);
                    //companycode = arrAuthorization[0];
                    clientID = arrAuthorization[0];
                    username = arrAuthorization[1];
                    password = arrAuthorization[2];
                }
                catch (Exception e)
                {
                    ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                    string msg = e.ToString().Replace("'", "~").Replace(System.Environment.NewLine, "%3Cbr%3E").Replace("\\", "\\\\");
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Authorization wrong config: " + msg + "\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                    return StatusCode(ResponseCode);
                }
            }
            else
            {
                ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Authorization is null!\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }

            // get request
            HTTP_CODE.WriteLogServiceIn("7. Request data", _context);
            string content = "";
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                content = reader.ReadToEnd();
            }
            if (content == null || content == "") content = "";

            clientIP = _context.RemoteIpAddress();
            clientContentType = Request.Headers["Content-Type"];
            clientAccept = Request.Headers["Accept"];
            dynamic varJsonRequest = null;
            HTTP_CODE.WriteLogServiceIn("8. Get and Check Json Request", _context);
            if (content != "")
            {
                try
                {
                    varJsonRequest = JObject.Parse(content);
                }
                catch
                {
                    ResponseCode = HTTP_CODE.HTTP_BAD_REQUEST;
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Request no json: " + content + "\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                    return StatusCode(ResponseCode);
                }
            }
            #endregion

            #region check valid
            // check authen
            bool kt = false; int i = 0; int j = 0; string pwd;
            Data = "{\"clientID\": \"" + clientID + "\", \"username\": \"" + username + "\", \"password\": \"" + password + 
                "\", \"clientIP\": \"" + clientIP + "\", \"functionName\": \"" + functionName + "\", \"clientContentType\": \"" + clientContentType + 
                "\", \"clientAccept\": \"" + clientAccept + "\", \"jsonRequest\": \"" + content + "\"}";
            try
            {
                // check clientid
                HTTP_CODE.WriteLogServiceIn("9. Check Client ID", _context);
                while ((i < d.config.Count) && !kt)
                {
                    if (d.config[i].clientId == clientID)
                    {
                        kt = true;
                    }
                    i++;
                }
                if (!kt)
                {
                    ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Không tìm thấy CLIENTID\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                    return StatusCode(ResponseCode);
                }
                i = i - 1; // giảm 1 về vị trí tìm thấy

                companycode = d.config[i].companyCode.ToString();
                // check IP
                HTTP_CODE.WriteLogServiceIn("10. Check IP client", _context);
                kt = false; j = 0; pwd = ""; d1 = d.config[i].clientListIP;
                while ((j < d1.Count) && !kt)
                {
                    pwd = d1[j].IPAddress;
                    HTTP_CODE.WriteLogServiceIn("10. " + j + ": IP client " + pwd + "||" + clientIP, _context);
                    if (clientIP == pwd || pwd == "*") kt = true;
                    j++;
                }
                if (!kt)
                {
                    ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"IP không hợp lệ\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage,_context);
                    return StatusCode(ResponseCode);
                }

                // check function
                HTTP_CODE.WriteLogServiceIn("11. Check nghiệp vụ yêu cầu", _context);
                kt = false; j = 0; pwd = ""; d1 = d.config[i].functionListName;
                while ((j < d1.Count) && !kt)
                {
                    pwd = d1[j].FunctionName;
                    HTTP_CODE.WriteLogServiceIn("11. " + j + ": Check nghiệp vụ " + pwd + "||" + functionName, _context);
                    if (functionName == pwd) { kt = true; SPName = d1[j].StoreProceduceName; IsInputList = d1[j].IsParamInputList; }
                    j++;
                }
                if (!kt)
                {
                    ResponseCode = HTTP_CODE.HTTP_NOT_FOUND;
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Chức năng không hợp lệ\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                    return StatusCode(ResponseCode);
                }

                // check Authen
                HTTP_CODE.WriteLogServiceIn("12. Check thông tin Authen", _context);
                pwd = "";
                try
                {
                    pwd = enc.Decrypt(d.config[i].password.ToString());
                }
                catch {
                    pwd = d.config[i].password.ToString();
                }
                if ((d.config[i].companyCode != companycode) || (d.config[i].username != username) || (pwd != password))
                {
                    ResponseCode = HTTP_CODE.HTTP_NOT_AUTHEN;
                    ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Lỗi thông tin xác thực\", \"Data\": " + Data + "}";
                    HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                    return StatusCode(ResponseCode);
                }
            }
            catch (Exception e)
            {
                ResponseCode = HTTP_CODE.HTTP_ERROR_SERVER;
                string msg = e.ToString().Replace("'", "~").Replace(System.Environment.NewLine, "%3Cbr%3E").Replace("\\", "\\\\");
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"Lỗi trong quá trình xử lý: " + msg + "\", \"Data\": " + Data + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
                return StatusCode(ResponseCode);
            }
            #endregion

            #region Runing API
            HTTP_CODE.WriteLogServiceIn("13. Starting execute DATA", _context);
            HTTP_CODE.WriteLogServiceIn("13.1. Khởi tạo connect string DB", _context);
            //HRSContext _context = new HRSContext(HttpContext);
            ToolDAO db = new ToolDAO(companycode, _context);
            HTTP_CODE.WriteLogServiceIn("13.2. Xử lý dữ liệu API", _context);
            string parameterOutput = ""; string json = ""; int errorCode = 200; string errorString = "";
            if (varJsonRequest == null) IsInputList = 0;
            if (IsInputList == 1)
            {
                HTTP_CODE.WriteLogServiceIn("13.2.1. Execute list Input", _context);
                string iParameterOutput = ""; int iErrorCode = HTTP_CODE.HTTP_ACCEPT; string iErrorString = ""; string jsonResponse = "";
                HTTP_CODE.WriteLogServiceIn("13.2.1.0. Count: " + d.RequestList.Count, _context);
                for (i = 0; i < varJsonRequest.RequestList.Count; i++)
                {
                    ResponseCode = 0; ResponseMessage = "";
                    string sJson = SetParam(_context, varJsonRequest.RequestList[i], d1, ref ResponseCode, ref ResponseMessage);
                    dynamic d2 = null; if(sJson != "") d2 = JObject.Parse(sJson);
                    db.ExecuteStoreAPI(functionName, SPName, d2, ref iParameterOutput, ref jsonResponse, ref iErrorCode, ref iErrorString);
                    if (jsonResponse == "") jsonResponse = "null";
                    if (iParameterOutput == "") iParameterOutput = "\"ParameterOutput\": null";
                    json = json + ",{\"Error\":\"" + iErrorCode + "\", \"Message\": \"" + iErrorString + "\", " + iParameterOutput + ", \"Data\": " + jsonResponse + "}";
                }
                ResponseCode = HTTP_CODE.HTTP_ACCEPT;
                ResponseMessage = "{\"ResponseList\":[" + Tools.RemoveFisrtChar(json) + "]}"; 
            }
            else
            {
                HTTP_CODE.WriteLogServiceIn("13.2.2. Execute single Input", _context);
                ResponseCode = 0; ResponseMessage = "";
                string sJson = SetParam(_context, varJsonRequest, d1, ref ResponseCode, ref ResponseMessage);
                dynamic d2 = null; if (sJson != "") d2 = JObject.Parse(sJson);
                db.ExecuteStoreAPI(functionName, SPName, d2, ref parameterOutput, ref json, ref errorCode, ref errorString);
                if (json == "") json = "null";
                if (parameterOutput == "") parameterOutput = "\"ParameterOutput\": null";
                ResponseMessage = "{\"Error\":\"" + errorCode + "\", \"Message\": \"" + errorString + "\", " + parameterOutput + ", \"Data\": " + json + "}";
            }
            d = JObject.Parse(ResponseMessage);
            return Json (d);
            #endregion
        }
        private string SetParam(HRSContext _context, dynamic varJsonRequest, dynamic d1, ref int ResponseCode, ref string ResponseMessage)
        {
            string StoreParam = "";
            try
            {
                if (varJsonRequest != null)
                {
                    for (int j = 0; j < d1.ParamIn.Count; j++) // Param In
                    {
                        StoreParam = StoreParam + "," + 
                            d1.ParamIn[j].ParamJson.ToString().Replace("REQUEST" + d1.ParamIn[j].ParamName.ToString(), 
                                varJsonRequest[d1.ParamIn[j].ParamName.ToString()].ToString());
                    }
                    for (int j = 0; j < d1.ParamOut.Count; j++) // Param In
                    {
                        StoreParam = StoreParam + "," + d1.ParamOut[j].ParamJson.ToString();
                    }
                }
                else
                {
                    for (int j = 0; j < d1.ParamIn.Count; j++) // Param In
                    {
                        StoreParam = StoreParam + "," + d1.ParamIn[j].ParamJson.ToString();
                    }
                    for (int j = 0; j < d1.ParamOut.Count; j++) // Param In
                    {
                        StoreParam = StoreParam + "," + d1.ParamOut[j].ParamJson.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                StoreParam = "";
                ResponseCode = HTTP_CODE.HTTP_BAD_REQUEST;
                ResponseMessage = "{\"Error\":\"" + ResponseCode + "\", \"Message\": \"7. Request no json: " + ex.ToString() + "}";
                HTTP_CODE.WriteLogServiceIn(ResponseMessage, _context);
            }
            return StoreParam;
        }
        #endregion
    }
}