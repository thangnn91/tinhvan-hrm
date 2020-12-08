using System;
using System.IO;
using System.Net;
using System.Net.Security;

namespace Utils
{
    public class ToolWeb {
        #region Properties
        private string URL;
        private string[] FunctionName; // = { "createArea", "editArea", "createCustomer", "editCustomer", "createPackage", "editPackage" }
        private string[] ActionPOST; // = { "POST", "POST", "POST", "POST", "POST", "POST" }
        private string[] FolderName; // = { @"/area/insert", @"/area/edit", @"/area/insert", @"/area/edit", @"/package/insert", @"/package/edit" }
        private string[] ContentType; //  = { "application/json", "application/json", "application/json", "application/json", "application/json", "application/json" }
        private string[] Accept; // = { "application/json", "application/json", "application/json", "application/json", "application/json", "application/json" }
        #endregion

        #region Method Contruction
        public ToolWeb(string sURL, string FunctionNameStr, string ActionPOSTStr, string FolderNameStr, string ContentTypeStr, string AcceptStr)
        {
            URL = sURL;
            FunctionName = FunctionNameStr.Split(new string[] { "||" }, StringSplitOptions.None); 
            ActionPOST = ActionPOSTStr.Split(new string[] { "||" }, StringSplitOptions.None);
            FolderName = FolderNameStr.Split(new string[] { "||" }, StringSplitOptions.None);
            ContentType = ContentTypeStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Accept = AcceptStr.Split(new string[] { "||" }, StringSplitOptions.None);
        }
        public ToolWeb(string sURL)
        {
            URL = sURL;
            FunctionName = null; ActionPOST = null; FolderName = null;
            ContentType = new string [1]; Accept = new string[1];
            ContentType[0] = "application/json"; Accept[0] = "application/json";
        }
        #endregion

        #region Private Method
        private int GetIDByArray(string Val, string[] a)
        {
            int i; bool kt;
            kt = false;
            i = 0;
            while ((i < a.Length) && !kt)
            {
                if (Val == a[i]) kt = true;
                i++;
            }
            //if (i>=a.Length) i=0;
            return i - 1;
        }

        private void InitiateSSLTrust()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                   new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch
            {

            }
        }
        #endregion

        #region Method
        public string WebRequestService(string func, string Authorization, string pzData, bool IsNoResponse = true)
        {
            string logMessage = ""; string result = "";
            string pzUrl = URL + "/" + func; string pzAuthorization = HTTP_CODE.Base64Encode(Authorization);
            logMessage = Environment.NewLine + "pzUrl: " + pzUrl + Environment.NewLine;
            logMessage = logMessage + "pzData: " + pzData + Environment.NewLine;
            logMessage = logMessage + "ContentType: " + ContentType[0] + Environment.NewLine;
            logMessage = logMessage + "Accept: " + Accept[0] + Environment.NewLine;
            logMessage = logMessage + "ActionPOST: POST" + Environment.NewLine;
            logMessage = logMessage + "pzAuthorization: " + pzAuthorization + Environment.NewLine;
            
            try
            {
                HttpWebRequest httpWebRequest; 
                
                httpWebRequest = (HttpWebRequest)WebRequest.Create(pzUrl);
                httpWebRequest.ContentType = ContentType[0];
                httpWebRequest.Accept = Accept[0];
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + pzAuthorization);
                httpWebRequest.Proxy = new WebProxy();//no proxy
                if (!string.IsNullOrEmpty(pzData))
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = pzData;
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                InitiateSSLTrust();//bypass SSL
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (!IsNoResponse)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }

                logMessage = logMessage + "result: " + result + Environment.NewLine;
            }
            catch (Exception ex)
            {
                logMessage = logMessage + Environment.NewLine + ex.ToString();
            }
            HTTP_CODE.WriteLogServiceIn(logMessage);
            return result;
        }
        public string WebHTTPRequest(string func, string Authorization, string pzData)
        {
            int i = GetIDByArray(func, FunctionName);
            string logMessage;
            string pzUrl;
            string pzAuthorization;
            string result = string.Empty;
            HttpWebRequest httpWebRequest;

            pzUrl = URL + FolderName[i];
            pzAuthorization = HTTP_CODE.Base64Encode(Authorization);
            try
            {
                if (ActionPOST[i] == "POST")
                {
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(pzUrl);
                    httpWebRequest.ContentType = ContentType[i];
                    httpWebRequest.Accept = Accept[i];
                    httpWebRequest.Method = ActionPOST[i];
                    httpWebRequest.Headers.Add("Authorization", "Basic " + pzAuthorization);
                    httpWebRequest.Proxy = new WebProxy();//no proxy

                    if (!string.IsNullOrEmpty(pzData))
                    {
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string json = pzData;
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                    }
                }
                else
                {
                    pzUrl = pzUrl + "?" + pzData;
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(pzUrl);
                    httpWebRequest.ContentType = ContentType[i];
                    httpWebRequest.Accept = Accept[i];
                    httpWebRequest.Method = ActionPOST[i];
                    httpWebRequest.Headers.Add("Authorization", "Basic " + pzAuthorization);
                    httpWebRequest.Proxy = new WebProxy();//no proxy
                }
                InitiateSSLTrust();//bypass SSL
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                logMessage = Environment.NewLine + "pzUrl: " + pzUrl + Environment.NewLine;
                logMessage = logMessage + "pzData: " + pzData + Environment.NewLine;
                logMessage = logMessage + "ContentType: " + ContentType[i] + Environment.NewLine;
                logMessage = logMessage + "Accept: " + Accept[i] + Environment.NewLine;
                logMessage = logMessage + "ActionPOST: " + ActionPOST[i] + Environment.NewLine;
                //logMessage = logMessage + "pzAuthorization: " + pzAuthorization + Environment.NewLine;
                logMessage = logMessage + "result: " + result + Environment.NewLine;
                logMessage = logMessage + "--------------------------------------------";
            }
            catch (Exception ex)
            {
                result = "pzUrl: " + pzUrl + "<br> Method: " + ActionPOST[i] + "<br> ContentType: " + ContentType[i] + "<br> Accept: " + Accept[i] + "<br> pzData: " + pzData + "<br> ex: " + ex.ToString();
                logMessage = Environment.NewLine + "pzData: " + pzData + Environment.NewLine;
                logMessage = logMessage + "pzUrl: " + pzUrl + Environment.NewLine;
                logMessage = logMessage + "ContentType: " + ContentType[i] + Environment.NewLine;
                logMessage = logMessage + "Accept: " + Accept[i] + Environment.NewLine;
                logMessage = logMessage + "ActionPOST: " + ActionPOST[i] + Environment.NewLine;
                //logMessage = logMessage + "pzAuthorization: " + pzAuthorization + Environment.NewLine;
                logMessage = logMessage + "result: " + result + Environment.NewLine;
                logMessage = logMessage + "--------------------------------------------";
            }
            HTTP_CODE.WriteLogServiceOut(logMessage);
            return result;
        }
        #endregion
    }
}    