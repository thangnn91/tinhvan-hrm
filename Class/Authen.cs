using System;
using Newtonsoft.Json.Linq;
using System.DirectoryServices.AccountManagement;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Utils
{
    public class Authen
    {
        #region Properties
        private HRSContext context;
        private ToolDAO bosDAO;
        #endregion

        #region Private Method
        private void GetHistaffUserByEmail(string Username, string DeviceID, ref string json, ref int errorCode, ref string errorString)// API Mobile
        {
            string parameterOutput = ""; 
            dynamic d1 = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\": \"UserName\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"60\", \"InputValue\": \"" + Username + "\"}," +
                "{\"ParamName\": \"DeviceID\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"100\", \"InputValue\": \"" + DeviceID + "\"}," +
                "{\"ParamName\": \"CompanyID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"CompanyCode\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"60\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"UserID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"FullName\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Email\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Mobile\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"40\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Avatar\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Token\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"UserType\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"ImageID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"StaffID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"Message\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"600\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"ResponseStatus\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
            bosDAO.ExecuteStoreAPI("login", "SP_CMS__Users_GetInfoByEmail", d1, ref parameterOutput, ref json, ref errorCode, ref errorString);
        }
        private void GetHistaffUserByEmail(string Username, out dynamic d)// Web app
        {
            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d1 = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\": \"UserName\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"60\", \"InputValue\": \"" + Username + "\"}," +
                "{\"ParamName\": \"DeviceID\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"100\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"CompanyID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"CompanyCode\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"60\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"UserID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"FullName\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Email\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Mobile\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"40\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Avatar\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Token\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"UserType\", \"ParamType\": \"16\", \"ParamInOut\": \"3\", \"ParamLength\": \"2\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"ImageID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"StaffID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"Message\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"600\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"ResponseStatus\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
            bosDAO.ExecuteStore("Authen", "SP_CMS__Users_GetInfoByEmail", d1, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse("{" + parameterOutput + "}");
        }
        private bool GetMailServerInfo(string CompanyCode, string MailServerID,
            out string domain, out string smtp, out int port, out bool IsSSL)// Mail server
        {
            try
            {
                ToolDAO dataDAO = new ToolDAO(CompanyCode, context);
                string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                dynamic d1 = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\": \"Sys_MailServerID\", \"ParamType\": \"0\", \"ParamInOut\": \"1\", \"ParamLength\": \"8\", \"InputValue\": \"" + MailServerID + "\"}," +
                    "{\"ParamName\": \"Keyword\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                    "{\"ParamName\": \"Page\", \"ParamType\": \"8\", \"ParamInOut\": \"1\", \"ParamLength\": \"4\", \"InputValue\": \"1\"}," +
                    "{\"ParamName\": \"PageSize\", \"ParamType\": \"8\", \"ParamInOut\": \"1\", \"ParamLength\": \"4\", \"InputValue\": \"100\"}," +
                    "{\"ParamName\": \"Rowcount\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
                dataDAO.ExecuteStore("MailServer", "SP_CMS__Sys_MailServer_List", d1, ref parameterOutput, ref json, ref errorCode, ref errorString, true);
                d1 = JObject.Parse(json);
                domain = Tools.GetDataJson(d1.MailServer.Items[0], "EmailDomain");
                smtp = Tools.GetDataJson(d1.MailServer.Items[0], "SMTPServer");
                port = Tools.GetDataJson(d1.MailServer.Items[0], "Port", "int");
                IsSSL = (Tools.GetDataJson(d1.MailServer.Items[0], "IsSSL", "int") == 1);
                return true;
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("GetMailServerInfo Is Error: " + ex.ToString());
                domain = ""; smtp = ""; port = 0; IsSSL = false;
                return false;
            }

        }
        private bool GetLdapServerInfo(string CompanyCode, string LdapServerID,
            out string LdapURL, out string LDAPDomain, out string LDAPBaseDN)// Ldap
        {
            try
            {
                ToolDAO dataDAO = new ToolDAO(CompanyCode, context);
                string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                dynamic d1 = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\": \"Sys_LDAPID\", \"ParamType\": \"0\", \"ParamInOut\": \"1\", \"ParamLength\": \"8\", \"InputValue\": \"" + LdapServerID + "\"}," +
                    "{\"ParamName\": \"Keyword\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                    "{\"ParamName\": \"Page\", \"ParamType\": \"8\", \"ParamInOut\": \"1\", \"ParamLength\": \"4\", \"InputValue\": \"1\"}," +
                    "{\"ParamName\": \"PageSize\", \"ParamType\": \"8\", \"ParamInOut\": \"1\", \"ParamLength\": \"4\", \"InputValue\": \"100\"}," +
                    "{\"ParamName\": \"Rowcount\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
                dataDAO.ExecuteStore("LdapServer", "SP_CMS__Sys_LDAP_List", d1, ref parameterOutput, ref json, ref errorCode, ref errorString, true);
                d1 = JObject.Parse(json);
                LdapURL = Tools.GetDataJson(d1.LdapServer.Items[0], "LdapURL");
                LDAPDomain = Tools.GetDataJson(d1.LdapServer.Items[0], "LDAPDomain");
                LDAPBaseDN = Tools.GetDataJson(d1.LdapServer.Items[0], "LDAPBaseDN");
                return true;
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("GetMailServerInfo Is Error: " + ex.ToString());
                LdapURL = ""; LDAPDomain = ""; LDAPBaseDN = "";
                return false;
            }
        }
        private bool EmailValidate(MailSocket m, string smtp, string Email, string UserPassword)
        {
            bool kt = false; string res = "";
            m.SendCommand("EHLO " + smtp); // EHLO mail.server.com
            res = m.GetFullResponse();
            m.SendCommand("AUTH LOGIN"); // AUTH LOGIN
            res = m.GetFullResponse();
            m.SendCommand(HTTP_CODE.Base64Encode(Email)); // Base64Encode - Email
            res = m.GetFullResponse();
            m.SendCommand(HTTP_CODE.Base64Encode(UserPassword)); // Base64Encode - Pwd
            res = m.GetFullResponse();
            if (res.IndexOf("Authentication successful") > 0) kt = true;
            m.SendCommand("QUIT"); // QUIT
            res = m.GetFullResponse();
            if (res.IndexOf("Authentication successful") > 0) kt = true;
            return kt;
        }
        #endregion

        public Authen(HRSContext _context, ToolDAO _bosDAO)
        {
            context = _context; bosDAO = _bosDAO;
        }

        public void LoginWithHistaffUser(string Username, string UserPassword, string DeviceID, ref string json, ref int errorCode, ref string errorString) // API Mobile
        {
            string parameterOutput = ""; 
            dynamic d1 = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\": \"UserName\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"60\", \"InputValue\": \"" + Username + "\"}," +
                "{\"ParamName\": \"Pwd\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"400\", \"InputValue\": \"" + UserPassword + "\"}," +
                "{\"ParamName\": \"DeviceID\", \"ParamType\": \"12\", \"ParamInOut\": \"1\", \"ParamLength\": \"100\", \"InputValue\": \"" + DeviceID + "\"}," +
                "{\"ParamName\": \"CompanyCode\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"60\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"UserID\", \"ParamType\": \"0\", \"ParamInOut\": \"3\", \"ParamLength\": \"8\", \"InputValue\": \"0\"}," +
                "{\"ParamName\": \"FullName\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Email\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Mobile\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"40\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Avatar\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"400\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Token\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"200\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"Message\", \"ParamType\": \"12\", \"ParamInOut\": \"3\", \"ParamLength\": \"600\", \"InputValue\": \"\"}," +
                "{\"ParamName\": \"ResponseStatus\", \"ParamType\": \"8\", \"ParamInOut\": \"3\", \"ParamLength\": \"4\", \"InputValue\": \"0\"}]}");
            bosDAO.ExecuteStoreAPI("login", "SP_API__Users_Login", d1, ref parameterOutput, ref json, ref errorCode, ref errorString);
        }
        public void LoginWithHistaffUser(string Username, string UserPassword, out dynamic d) //Web app
        {
            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d1 = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"UserName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + Username + "\"}," +
                    "{\"ParamName\":\"Pwd\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + UserPassword + "\"}," +
                    "{\"ParamName\":\"CompanyID\", \"ParamType\":\"0\", \"ParamInOut\":\"3\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                    "{\"ParamName\":\"CompanyCode\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"60\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"UserID\", \"ParamType\":\"0\", \"ParamInOut\":\"3\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                    "{\"ParamName\":\"UserType\", \"ParamType\":\"16\", \"ParamInOut\":\"3\", \"ParamLength\":\"2\", \"InputValue\":\"0\"}," +
                    "{\"ParamName\":\"ImageID\", \"ParamType\":\"0\", \"ParamInOut\":\"3\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                    "{\"ParamName\":\"StaffID\", \"ParamType\":\"0\", \"ParamInOut\":\"3\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                    "{\"ParamName\":\"Token\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"200\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
            bosDAO.ExecuteStore("Authen", "SP_CMS__Users_Login", d1, ref parameterOutput, ref json, ref errorCode, ref errorString);
            parameterOutput  = "\"ParameterOutput\": { \"CompanyID\":\"1\",\"CompanyCode\":\"\",\"UserID\":\"1\",\"UserType\":\"1\",\"ImageID\":\"0\",\"StaffID\":\"0\",\"Token\":\"100\",\"Message\":\"OK\",\"ResponseStatus\":\"1\"}";
            d = JObject.Parse("{" + parameterOutput + "}");
        }
        public bool LoginWithADUser(string LdapServerID, string Username, string UserPassword, string DeviceID, ref string json, ref int errorCode, ref string errorString)// API Mobile
        {
            bool kt = false;
            HTTP_CODE.WriteLogAction("LoginWithADUser Is Start");
            try
            {
                //1. Check email tồn tại => Username
                dynamic d;
                GetHistaffUserByEmail(Username, DeviceID, ref json, ref errorCode, ref errorString);// Web app
                d = JObject.Parse(json);
                if ((long)d.login.ResponseStatus < 1) return kt;
                string Email = d.login.ParameterOutput.Email.ToString(); // Tai khoan co email doc lap
                //2. Get thong tin mail server
                string CompanyCode = d.login.ParameterOutput.CompanyCode.ToString();
                string LdapURL = ""; string LDAPDomain = ""; string LDAPBaseDN = "";
                if (!GetLdapServerInfo(CompanyCode, LdapServerID, out LdapURL, out LDAPDomain, out LDAPBaseDN)) return kt;
                if (Email == "") Email = Username + "@" + LDAPDomain; // Email theo domain
                //3. Check LDap
                LDAP m = new LDAP(LdapURL, LDAPDomain, LDAPBaseDN, 0, Username, UserPassword); // 0 - ContextType.Machine
                kt = m.ValidateUser();
                HTTP_CODE.WriteLogAction("LoginWithADUser Is OK");
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("LoginWithADUser Is Error: " + ex.ToString());
                json = "{\"ResponseStatus\":\"-99\", \"Message\":\"SystemError\"}";
                errorCode = HTTP_CODE.HTTP_ACCEPT;
                errorString = context.GetLanguageLable("SystemError");
                kt = false;
            }
            return kt;
        }
        public bool LoginWithADUser(string LdapServerID, string Username, string UserPassword, out dynamic d) //Web app
        {
            bool kt = false;
            HTTP_CODE.WriteLogAction("LoginWithADUser Is Start");
            try
            {
                //1. Check email tồn tại => Username
                GetHistaffUserByEmail(Username, out d);// Web app
                if ((long)d.ParameterOutput.ResponseStatus < 1) return kt;
                string Email = d.ParameterOutput.Email.ToString(); // Tai khoan co email doc lap
                //2. Get thong tin mail server
                string CompanyCode = d.ParameterOutput.CompanyCode.ToString();
                string LdapURL = ""; string LDAPDomain = ""; string LDAPBaseDN = ""; 
                if (!GetLdapServerInfo(CompanyCode, LdapServerID, out LdapURL, out LDAPDomain, out LDAPBaseDN)) return kt;
                if (Email == "") Email = Username + "@" + LDAPDomain; // Email theo domain
                //3. Check LDap
                LDAP m = new LDAP(LdapURL, LDAPDomain, LDAPBaseDN, 0, Username, UserPassword); // 0 - ContextType.Machine
                kt = m.ValidateUser();
                HTTP_CODE.WriteLogAction("LoginWithADUser Is OK");
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("LoginWithADUser Is Error: " + ex.ToString());
                d = JObject.Parse("{\"ParameterOutput\":{\"ResponseStatus\":\"-99\", \"Message\":\"SystemError\"}}");
                kt = false;
            }
            return kt;
        }
        public bool LoginWithEmail(string MailServerID, string Username, string UserPassword, string DeviceID, ref string json, ref int errorCode, ref string errorString)// API Mobile
        {
            bool kt = false;
            HTTP_CODE.WriteLogAction("LoginWithEmail Is Start");
            try
            {
                //1. Check email tồn tại => Username
                dynamic d;
                GetHistaffUserByEmail(Username, DeviceID, ref json, ref errorCode, ref errorString);// Web app
                d = JObject.Parse(json);
                if ((long)d.login.ResponseStatus < 1) return kt;
                string Email = d.login.ParameterOutput.Email.ToString(); // Tai khoan co email doc lap
                //2. Get thong tin mail server
                string CompanyCode = d.login.ParameterOutput.CompanyCode.ToString();
                string domain = ""; string smtp = ""; int port = 0; bool IsSSL = false; 
                if (!GetMailServerInfo(CompanyCode, MailServerID, out domain, out smtp, out port, out IsSSL)) return kt;
                if (Email == "") Email = Username + "@" + domain;
                //3. Check socket mail server
                MailSocket m = new MailSocket(smtp, port);
                kt = EmailValidate(m, smtp, Email, UserPassword);
                //4. Return
                //string parameterOutput = "\"ParameterOutput\":{\"ResponseStatus\":\"-99\", \"Message\":\"SystemError\"}";
                //d = JObject.Parse("{" + parameterOutput + "}");
                HTTP_CODE.WriteLogAction("LoginWithEmail Is OK");
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("LoginWithEmail Is Error: " + ex.ToString());
                json = "{\"ResponseStatus\":\"-99\", \"Message\":\"SystemError\"}";
                errorCode = HTTP_CODE.HTTP_ACCEPT;
                errorString = context.GetLanguageLable("SystemError");
                kt = false;
            }
            return kt;
        }
        public bool LoginWithEmail(string MailServerID, string Username, string UserPassword, out dynamic d) //Web app
        {
            bool kt = false;
            HTTP_CODE.WriteLogAction("LoginWithEmail Is Start");
            try
            {
                //1. Check email tồn tại => Username
                GetHistaffUserByEmail(Username, out d);// Web app
                if ((long)d.ParameterOutput.ResponseStatus < 1) return kt;
                string Email = d.ParameterOutput.Email.ToString(); // Tai khoan co email doc lap
                //2. Get thong tin mail server
                string CompanyCode = d.ParameterOutput.CompanyCode.ToString();
                string domain = ""; string smtp = ""; int port = 0; bool IsSSL = false; 
                if (!GetMailServerInfo(CompanyCode, MailServerID, out domain, out smtp, out port, out IsSSL)) return kt;
                if (Email == "") Email = Username + "@" + domain;
                //3. Check socket mail server
                MailSocket m = new MailSocket(smtp, port);
                kt = EmailValidate(m, smtp, Email, UserPassword);
                //4. Return
                //string parameterOutput = "\"ParameterOutput\":{\"ResponseStatus\":\"-99\", \"Message\":\"SystemError\"}";
                //d = JObject.Parse("{" + parameterOutput + "}");
                HTTP_CODE.WriteLogAction("LoginWithEmail Is OK");
            }
            catch(Exception ex)
            {
                HTTP_CODE.WriteLogAction("LoginWithEmail Is Error: " + ex.ToString());
                d = JObject.Parse("{\"ParameterOutput\":{\"ResponseStatus\":\"-99\", \"Message\":\"SystemError\"}}");
                kt = false;
            }
            return kt;
        }
    }

    public class LDAP
    {
        #region Properties
        private string LdapURL; //ldap://tinhvan.com
        private string LDAPDomain; // tinhvan.com
        private string LDAPBaseDN; //ldap://tinhvan.com/path
        private string Username;
        private string Password;

        PrincipalContext insPrincipalContext;
        int LDAPType;
        #endregion

        public LDAP(string _LdapURL, string _LDAPDomain, string _LDAPBaseDN, int _LDAPType, 
            string _Username, string _Password)
        {
            LdapURL = _LdapURL; LDAPDomain = _LDAPDomain; LDAPBaseDN = _LDAPBaseDN;
            Username = _Username; Password = _Password; LDAPType = _LDAPType;
            try
            {
                switch (LDAPType)
                {
                    case 1: //ContextType.Domain
                        string d = GetLDAPDomain(LDAPDomain);
                        insPrincipalContext = new PrincipalContext(ContextType.Domain, LdapURL, d, Username, Password);
                        break;
                    case 2: //ContextType.ApplicationDirectory
                        insPrincipalContext = new PrincipalContext(ContextType.ApplicationDirectory, LDAPDomain, Username, Password);
                        break;
                    default: //ContextType.Machine
                        insPrincipalContext = new PrincipalContext(ContextType.Machine, LDAPDomain, Username, Password);
                        break;
                }
            }
            catch { }
        }

        public bool ValidateUser()
        {
            bool kt = false;
            try
            {
                PrincipalSearcher insPrincipalSearcher = new PrincipalSearcher();
                UserPrincipal insUserPrincipal = new UserPrincipal(insPrincipalContext);
                insUserPrincipal.Name = Username;
                insPrincipalSearcher.QueryFilter = insUserPrincipal;
                PrincipalSearchResult<Principal> results = insPrincipalSearcher.FindAll();
                if (results != null) kt = true;
                HTTP_CODE.WriteLogAction("LoginWithADUser Is OK");
            }
            catch (Exception ex) { kt = false; HTTP_CODE.WriteLogAction("LoginWithADUser Is Error: " + ex.ToString()); }
            return kt;
        }

        private string GetLDAPDomain(string Domain)
        {
            string r = "";
            string[] d = Domain.Split(new string[] { "." }, StringSplitOptions.None);
            for (int i = 0; i < d.Length - 1; i++) r = r + "DC=" + d[i] + ",";
            r = r + "DC=" + d[d.Length];
            return r;
        }

    }
    public class MailSocket
    {
        #region Properties
        private TcpClient client = null;
        private NetworkStream stream = null;
        private StreamReader reader = null;
        private StreamWriter writer = null;
        private string resp = "";
        private int state = -1;
        #endregion

        public MailSocket(TcpClient tc)
        {
            client = tc;
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        public MailSocket(string url, int port)
        {
            client = new TcpClient(url, port);
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        public void SendData(byte[] bts)
        {
            if (GetResponseState() != 221)
            {
                stream.Write(bts, 0, bts.Length);
                stream.Flush();
            }
        }
        public void SendCommand(string cmd)
        {
            if (GetResponseState() != 221)
            {
                writer.WriteLine(cmd);
                writer.Flush();
            }
        }
        public string GetFullResponse()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(RecvResponse());
            sb.Append("\r\n");
            while (HaveNextResponse())
            {
                sb.Append(RecvResponse());
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        #region Private Method //HTTP_CODE.Base64Encode(
        private bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
        }
        public int GetResponseState()
        {
            if (resp.Length >= 3 && IsNumber(resp[0]) && IsNumber(resp[1]) && IsNumber(resp[2]))
                state = Convert.ToInt32(resp.Substring(0, 3));

            return state;
        }
        private bool HaveNextResponse()
        {
            if (GetResponseState() > -1)
            {
                if (resp.Length >= 4 && resp[3] != ' ')
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        private string RecvResponse()
        {
            if (GetResponseState() != 221)
                resp = reader.ReadLine();
            else
                resp = "221 closed!";

            return resp;
        }
        #endregion
    }
}
