using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Utils;
using HRScripting.Services;
using System.IO;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace HRScripting.Controllers
{
    public class ServiceController : Controller
    {
        public IBackgroundTaskQueue Queue { get; }

        #region Method Contructor
        public ServiceController(IBackgroundTaskQueue queue)
        {

            Queue = queue;
        }
        #endregion

        #region Private
        private bool CheckLoginWithToken(ToolDAO bosDAO, string UserID, string Token, 
            out dynamic d, out int errorCode, out string errorString)
        {
            bool kt = false; errorCode = 0; errorString = "";
             d = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\": \"UserID\", \"ParamType\": \"0\", \"ParamInOut\": \"1\", \"ParamLength\": \"8\", \"InputValue\": \"" + UserID + "\"}," +
                "{\"ParamName\": \"Token\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"200\", \"InputValue\": \"" + Token + "\"}," +
                "{\"ParamName\": \"Message\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"600\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"ResponseStatus\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
            string parameterOutput = ""; string json = ""; 
            bosDAO.ExecuteStoreService("Authen", "SP_CMS__Users_LoginWithToken", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse("{" + parameterOutput + "}");
            kt = (long.Parse(d.ParameterOutput.ResponseStatus.ToString()) > 0);
            return kt;
        }
        private bool CheckPermission(ToolDAO bosDAO, string UserID, string MenuID, 
            out dynamic d, out string parameterOutput, out string json, out int errorCode, out string errorString)
        {
            bool kt = false; errorCode = 0; errorString = ""; json = ""; parameterOutput = "";
            d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\": \"UserID\", \"ParamType\": \"8\", \"ParamInOut\": \"1\", \"ParamLength\": \"4\", \"InputValue\": \"" + UserID + "\"}," +
                    "{\"ParamName\": \"MenuID\", \"ParamType\": \"8\", \"ParamInOut\": \"1\", \"ParamLength\": \"4\", \"InputValue\": \"" + MenuID + "\"}," +
                    "{\"ParamName\": \"IsGrant\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                    "{\"ParamName\": \"IsInsert\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                    "{\"ParamName\": \"IsUpdate\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                    "{\"ParamName\": \"IsDelete\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                    "{\"ParamName\": \"IsReview\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                    "{\"ParamName\": \"IsPublish\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                    "{\"ParamName\": \"IsApply\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                    "{\"ParamName\": \"Message\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"600\", \"InputValue\": \"\"}," +
                    "{\"ParamName\": \"ResponseStatus\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
            parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
            bosDAO.ExecuteStoreService("CheckPermission", "SP_CMS__Users_CheckPermission", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse("{" + parameterOutput + "}");
            kt = (long.Parse(d.ParameterOutput.ResponseStatus.ToString()) > 0);
            return kt;
        }
        private int CheckAuthen(ToolFolder toolFolder, string functionName, 
            out string Json, out string Token, out string UserID, out string MenuID, out string CompanyCode)
        {
            int kt = 1; Json = ""; Token = ""; UserID = "0"; MenuID = "0"; CompanyCode = "0";
            // Check Authorization
            string authHeader = Request.Headers["Authorization"];
            if (authHeader == null) authHeader = "";
            if (authHeader.Trim() == "")
            {
                toolFolder.WriteLogService(functionName + " - Authorization Is Null");
                return -601;
            }            
            try
            {
                string[] a = HTTP_CODE.Base64Decode(Tools.RemoveFisrtChar(authHeader, "Bearer ".Length).Trim()).Split(new string[] { "^" }, StringSplitOptions.None);
                CompanyCode = a[0];
                UserID = a[1];
                MenuID = a[2];
                Token = a[3];
            }
            catch (Exception Ex)
            {
                toolFolder.WriteLogService(functionName + " - Authorization (" + authHeader + ") Is Error: " + Ex.ToString());
                return -99;
            }

            // Check IP Client

            // Get Request
            using (var reader = new StreamReader(Request.Body))
            {
                Json = reader.ReadToEnd();
            }
            return kt;
        }
        private void TemplateProcess(ToolFolder toolFolder, ToolDAO bosDAO, string functionname, string ContentFile, 
            string DataType, string DataVar, string DataSource, ref string Subject, out string Content, out string[] FileList)
        {
            string[] a = ContentFile.Split(new string[] { "^" }, StringSplitOptions.None); string fileDestination = "";
            FileList = new string[a.Length];
            Content = ""; 
            string guid = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            for(int i = 0; i < a.Length; i++)
            {
                try
                {
                    string[] a1 = a[i].Split(new string[] { ";" }, StringSplitOptions.None);
                    dynamic d = Tools.GetMedia(a1[0], bosDAO);
                    if (d != null)
                    {
                        string fileExt = Tools.GetDataJson(d.GetMedia.Items[0], "FileExtention");
                        string fileOrigina = toolFolder.FolderRoot + Tools.GetDataJson(d.GetMedia.Items[0], "UrlOriginal").Replace("||", "\\");
                        string data = "{\"DataVar\": {\"Items\":[" + DataVar + "], \"ItemType\": " + DataType + "}" + (DataSource != "" ? ", " + DataSource : "") + "}";
                        fileDestination = toolFolder.FolderRoot + "\\RootFolder\\Mail\\" + guid;
                        toolFolder.PathCreateDirectory(fileDestination);
                        fileDestination = fileDestination + "\\" + functionname + i.ToString() + fileExt;

                        // Subject
                        PlaintextProcessing plaintext = new PlaintextProcessing(Subject, data);
                        Subject = plaintext.DestinationData;
                        plaintext = null;

                        // Content File
                        switch (fileExt.ToLower())
                        {
                            case ".txt":
                                string fileContent = toolFolder.FileReadAllText(fileOrigina);
                                plaintext = new PlaintextProcessing(fileContent, data);                                
                                toolFolder.FileWrite(fileDestination, plaintext.DestinationData);
                                FileList[i] = fileDestination;
                                Content = plaintext.DestinationData;
                                plaintext = null;
                                break;
                            case ".doc":
                            case ".docx":
                                WordProcessing w = new WordProcessing(fileOrigina, fileDestination, data);
                                FileList[i] = fileDestination;
                                w = null;
                                break;
                            case ".xls":
                            case ".xlsx":
                                ExcelPackage package = new ExcelPackage(new FileInfo(fileOrigina));
                                if (DataSource == "") DataSource = "\"Data\": null";
                                ReadExcelForm xlsx = new ReadExcelForm(package, DataVar, "{" + DataSource + "}", "{\"DataPivot\":\"NonPivot\"}");
                                package.SaveAs(new FileInfo(fileDestination), functionname + i.ToString());
                                FileList[i] = fileDestination;
                                package = null;
                                xlsx = null;
                                break;
                        }
                    }
                }
                catch (Exception Ex)
                {
                    toolFolder.WriteLogService("Error: " + Ex.ToString());
                }
            }
        }
        #endregion

        #region Method
        // Background Sendmail
        [Route("Service/Mail/{functionName}")]
        [HttpPost]
        public IActionResult Mail(string functionName)
        {
            ToolFolder toolFolder = new ToolFolder("appsettings.json");
            string Json = ""; string Token = ""; string UserID = "0"; string MenuID = "0"; string CompanyCode = "0";
            // Check Authorization
            int kt = CheckAuthen(toolFolder, functionName, out Json, out Token, out UserID, out MenuID, out CompanyCode);

            if (kt < 1) return StatusCode(404);

            Queue.QueueBackgroundWorkItem(async token =>
            {
                ToolDAO bosDAO = new ToolDAO(); Algorithm enc;
                 dynamic d1 = JObject.Parse(Json);
                toolFolder.WriteLogService(functionName + ": Start");
                toolFolder.WriteLogService(functionName + " - Request: " + d1.ToString());
                toolFolder.WriteLogService(functionName + " - Start Authen");
                string PrivateKey = toolFolder.ReadConfigToJson().ConnectionStrings.PrivateKey.ToString();
                if (PrivateKey == "")
                    enc = new Algorithm();
                else
                    enc = new Algorithm(PrivateKey);
                // Check token
                dynamic d;
                string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                if (CheckLoginWithToken(bosDAO, UserID, Token, out d, out errorCode, out errorString))
                {
                    toolFolder.WriteLogService(functionName + " - Authen OK: " + errorString);
                    // Check Permistion
                    toolFolder.WriteLogService(functionName + " - Start Permistion");
                    if (CheckPermission(bosDAO, UserID, MenuID,
                        out d, out parameterOutput, out json, out errorCode, out errorString))
                    {
                        toolFolder.WriteLogService(functionName + " - Permistion OK: " + errorString);
                        string StoreName = "SP_CMS__Sys_MailTempSetup_GetInfor";
                        dynamic StoreParam = JObject.Parse("{\"parameterInput\":[" +
                            "{\"ParamName\": \"Sys_MailTempSetupCode\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"60\", \"InputValue\": \"" + functionName + "\"}," +
                            "{\"ParamName\": \"EmailDomain\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"SMTPServer\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"Port\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}," +
                            "{\"ParamName\": \"IsSSL\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                            "{\"ParamName\": \"AccountUser\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"Pwd\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"AccountName\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"AccountEmail\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"Subject\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"1000\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"ContentFile\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"StoreGetInfor\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"StoreGetInforParam\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"2000\", \"InputValue\": \"\"}," +
                            //"{\"ParamName\": \"StoreGetListInfor\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                            //"{\"ParamName\": \"StoreGetListInforParam\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"2000\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"StoreGetEmailAddr\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"StoreGetEmailAddrParam\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"2000\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"Message\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"600\", \"InputValue\": \"\"}," +
                            "{\"ParamName\": \"ResponseStatus\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
                        
                        toolFolder.WriteLogService(functionName + " - Start Execute Store: " + StoreName + " with Param: " + StoreParam.ToString());
                        toolFolder.WriteLogService(functionName + " - Execute Store: " + StoreName + " with parameterOutput: " + parameterOutput + " with json: " + json);
                        ToolDAO dataDAO = new ToolDAO(CompanyCode);
                        dataDAO.ExecuteStoreService("GetInforMail", StoreName, StoreParam, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        if (errorCode == HTTP_CODE.HTTP_ACCEPT)
                        {
                            toolFolder.WriteLogService(functionName + " - Exec: " + StoreName + " OK - " + errorString);
                            d = JObject.Parse("{" + parameterOutput + "}");
                            if (long.Parse(d.ParameterOutput.ResponseStatus.ToString()) > 0)
                            {
                                string EmailDomain = d.ParameterOutput.EmailDomain.ToString();
                                string SMTPServer = d.ParameterOutput.SMTPServer.ToString();
                                int Port = int.Parse(d.ParameterOutput.Port.ToString());
                                bool IsSSL = (int.Parse(d.ParameterOutput.IsSSL.ToString()) == 1);
                                string AccountUser = d.ParameterOutput.AccountUser.ToString();
                                string Pwd = d.ParameterOutput.Pwd.ToString();
                                try { Pwd = enc.Decrypt(Pwd); } catch { }
                                string AccountName = d.ParameterOutput.AccountName.ToString();
                                string AccountEmail = d.ParameterOutput.AccountEmail.ToString();
                                string Subject = d.ParameterOutput.Subject.ToString();
                                string ContentFile = d.ParameterOutput.ContentFile.ToString();
                                string StoreGetInfor = d.ParameterOutput.StoreGetInfor.ToString();
                                string StoreGetInforParam = d.ParameterOutput.StoreGetInforParam.ToString();
                                //string StoreGetListInfor = d.ParameterOutput.StoreGetListInfor.ToString();
                                //string StoreGetListInforParam = d.ParameterOutput.StoreGetListInforParam.ToString();
                                string StoreGetEmailAddr = d.ParameterOutput.StoreGetEmailAddr.ToString();
                                string StoreGetEmailAddrParam = d.ParameterOutput.StoreGetEmailAddrParam.ToString();

                                // Thực thi Store StoreGetInfor ==> List Datavar. 
                                StoreName = d1.StoreName.ToString();
                                StoreParam = d1.StoreParam;
                                dataDAO.ExecuteStoreService("DatavarList", StoreName, StoreParam, ref parameterOutput, ref json, ref errorCode, ref errorString);
                                if (errorCode == HTTP_CODE.HTTP_ACCEPT)
                                {
                                    d = JObject.Parse(json);
                                    // Duyệt từng Datavar
                                    for (int i = 0; i < d.DatavarList.Items.Count; i++)
                                    {
                                        string[] EmailTo = null; string[] EmailCC = null; string[] EmailBcc = null; string[] FileList = null; string Content = "";
                                        dynamic dDataVar = d.DatavarList.Items[i];
                                        dynamic dDataType = d.DatavarList.ItemType;
                                        // Execute Store get EmailTo; EmailCC; Email Bcc
                                        json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(Tools.ReadParam(StoreGetEmailAddrParam, dDataVar)) + "]}";
                                        dynamic d45 = JObject.Parse(json);
                                        dataDAO.ExecuteStoreService("MailAddr", StoreGetEmailAddr, d45, ref parameterOutput, ref json, ref errorCode, ref errorString);
                                        if (errorCode == HTTP_CODE.HTTP_ACCEPT)
                                        {
                                            try
                                            {
                                                dynamic dDataSource = JObject.Parse("{" + parameterOutput + "}");
                                                EmailTo = dDataSource.ParameterOutput.EmailTo.ToString().Split(new string[] { ";" }, StringSplitOptions.None);
                                                EmailCC = dDataSource.ParameterOutput.EmailCC.ToString().Split(new string[] { ";" }, StringSplitOptions.None);
                                                EmailBcc = dDataSource.ParameterOutput.EmailBcc.ToString().Split(new string[] { ";" }, StringSplitOptions.None);

                                                string Data = "";
                                                // Create Param Exec DataSource
                                                string[] StoreList = StoreGetInfor.Split(new string[] { "$" }, StringSplitOptions.None);
                                                string[] StoreParamList = StoreGetInforParam.Split(new string[] { "$" }, StringSplitOptions.None);
                                                // Thực thi Store StoreGetListInfor ==> Data Source
                                                for (int j = 0; j < StoreList.Length; j++)
                                                {
                                                    json = Tools.RemoveFisrtChar(Tools.ReadParam(StoreParamList[j], dDataVar));
                                                    d45 = JObject.Parse("{\"parameterInput\":[" + json + "]}");
                                                    dataDAO.ExecuteStoreService("DataSource", StoreList[j], d45, ref parameterOutput, ref json, ref errorCode, ref errorString);
                                                    if (errorCode == HTTP_CODE.HTTP_ACCEPT)
                                                    {
                                                        dDataSource = JObject.Parse(json);
                                                        Data = Data + ",\"DATA" + (j > 0 ? j.ToString() : "") + "\": " + dDataSource.DataSource.ToString();
                                                    }
                                                }
                                                if (Data.Length > 0) Data = Tools.RemoveFisrtChar(Data);
                                                // Xử lý Template
                                                string SubjectUse = Subject;
                                                TemplateProcess(toolFolder, bosDAO, functionName, ContentFile, dDataType.ToString(), dDataVar.ToString(), Data, ref SubjectUse, out Content, out FileList);

                                                // Send Mail
                                                bool IsSendOk = false;
                                                //AccountUser = AccountUser + "@" + EmailDomain;
                                                //AccountEmail = "chiennt@tinhvan.com";
                                                SendmailProcessing sendmail = new SendmailProcessing(SMTPServer, Port, IsSSL, AccountName, AccountEmail, AccountUser + "@" + EmailDomain, Pwd,
                                                    EmailTo, EmailCC, EmailBcc, SubjectUse, Content, FileList);
                                                IsSendOk = sendmail.EmailSending();
                                                string guid = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                                                long HU_EmployeeID = Tools.GetDataJson(dDataVar, "HU_EmployeeID", "bigint");
                                                long AT_PeriodID = Tools.GetDataJson(dDataVar, "AT_PeriodID", "bigint");
                                                if (IsSendOk)
                                                {
                                                    dataDAO.ExecuteSQLService("ExecMail", "INSERT INTO Sys_MailSended (I, HU_EmployeeID, AT_PeriodID, EmailDomain, SMTPServer, Port, IsSSL, AccountName, EmailAddr, Subject) " +
                                                        "VALUES('" + guid + "', '" + HU_EmployeeID.ToString() + "', '" + AT_PeriodID.ToString() + "', N'" + EmailDomain + "', N'" + SMTPServer + "', '" + Port + "', '" + (IsSSL? "1":"0") + "', N'" + AccountName + "', N'" + AccountEmail + "', N'" + Subject + "')", ref json, ref errorCode, ref errorString);
                                                }
                                                else
                                                {
                                                    dataDAO.ExecuteSQLService("ExecMail", "INSERT INTO Sys_MailSending (I, HU_EmployeeID, AT_PeriodID, EmailDomain, SMTPServer, Port, IsSSL, AccountName, EmailAddr, Subject) " +
                                                        "VALUES('" + guid + "', '" + HU_EmployeeID.ToString() + "', '" + AT_PeriodID.ToString() + "', N'" + EmailDomain + "', N'" + SMTPServer + "', '" + Port + "', '" + (IsSSL ? "1" : "0") + "', N'" + AccountName + "', N'" + AccountEmail + "', N'" + Subject + "')", ref json, ref errorCode, ref errorString);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                toolFolder.WriteLogService(functionName + " - Get Email: " + ex.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            toolFolder.WriteLogService(functionName + " - Exec: " + StoreName + " Fail - " + errorString);
                        }
                    }
                    else
                    {
                        toolFolder.WriteLogService(functionName + " - Permistion Fail: " + errorString);
                    }
                }
                else
                {
                    toolFolder.WriteLogService(functionName + " - Authen Fail: " + errorString);
                }
                toolFolder.WriteLogService(functionName + ": End");
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            });
            return StatusCode(404);
        }
        // Background Calulator
        [Route("Service/BasetabExec/{functionName}")]
        [HttpPost]
        public IActionResult BasetabExec(string functionName)
        {
            ToolFolder toolFolder = new ToolFolder("appsettings.json");
            string Json = ""; string Token = ""; string UserID = "0"; string MenuID = "0"; string CompanyCode = "0";
            // Check Authorization
            int kt = CheckAuthen(toolFolder, functionName, out Json, out Token, out UserID, out MenuID, out CompanyCode);

            if (kt < 1) return StatusCode(404);

            Queue.QueueBackgroundWorkItem(async token =>
            {
                ToolDAO bosDAO = new ToolDAO();                
                dynamic d1 = JObject.Parse(Json);
                toolFolder.WriteLogService(functionName + ": Start");
                toolFolder.WriteLogService(functionName + " - Request: " + d1.ToString());
                toolFolder.WriteLogService(functionName + " - Start Authen");
                // Check token
                dynamic d;
                string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";                
                if (CheckLoginWithToken(bosDAO, UserID, Token, out d, out errorCode, out errorString))
                {
                    toolFolder.WriteLogService(functionName + " - Authen OK: " + errorString);
                    string StoreName = d1.StoreName.ToString();
                    dynamic StoreParam = d1.StoreParam;
                    // Check Permistion
                    toolFolder.WriteLogService(functionName + " - Start Permistion");                    
                    if (CheckPermission(bosDAO, UserID, MenuID,
                        out d, out parameterOutput, out json, out errorCode, out errorString))
                    {
                        toolFolder.WriteLogService(functionName + " - Permistion OK: " + errorString);
                        toolFolder.WriteLogService(functionName + " - Start Execute Store: " + StoreName + " with Param: " + StoreParam.ToString());
                        if (StoreParam.ToString() == "") StoreParam = null;
                        ToolDAO dataDAO = new ToolDAO(CompanyCode);
                        dataDAO.ExecuteStoreService("BasetabExec", StoreName, StoreParam, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        toolFolder.WriteLogService(functionName + " - Exec: " + StoreName + " - " + errorString);
                    }
                    else
                    {
                        toolFolder.WriteLogService(functionName + " - Permistion Fail: " + errorString);
                    }
                }
                else
                {
                    toolFolder.WriteLogService(functionName + " - Authen Fail: " + errorString);
                }
                toolFolder.WriteLogService(functionName + ": End");
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            });
            return StatusCode(404);
        }
        #endregion
        public IActionResult Index()
        {
            return StatusCode(404);
        }
    }
}