namespace Utils
{
    public class ToolDAO
    {
        #region Properties
        private string DBType;
        private string ConnStr;
        public dynamic toolDAO; // Call object SQLConnection
        private HRSContext context;
        #endregion

        #region Method Contruction
        public ToolDAO(HRSContext _context)
        {
            context = _context;
            dynamic d = context._appConfig;// JObject.Parse(context.AppConfig.ReadConfig(context));
            DBType = d.ConnectionStrings.Type.ToString();
            ConnStr = context.enc.Decrypt(d.ConnectionStrings.ConnectionString.Default.ToString());
            switch (DBType)
            {
                case "SQLSERVER": toolDAO = new ToolMSSQL(ConnStr, context); break;
                case "ORACLE": toolDAO = new ToolOracle(ConnStr, context); break;
            }
        }
        public ToolDAO()
        {
            context = null;
            ToolFolder toolFolder = new ToolFolder("appsettings.json");
            dynamic d = toolFolder.ReadConfigToJson();
            Algorithm enc;
            string PrivateKey = d.ConnectionStrings.PrivateKey.ToString();
            if (PrivateKey == "")
                enc = new Algorithm();
            else
                enc = new Algorithm(PrivateKey);
            DBType = d.ConnectionStrings.Type.ToString();
            ConnStr = enc.Decrypt(d.ConnectionStrings.ConnectionString.Default.ToString());
            switch (DBType)
            {
                case "SQLSERVER": toolDAO = new ToolMSSQL(ConnStr); break;
                case "ORACLE": toolDAO = new ToolOracle(ConnStr); break;
            }
        }
        
        public ToolDAO(string CompanyCode)
        {
            context = null;
            ToolFolder toolFolder = new ToolFolder("appsettings.json");
            dynamic d = toolFolder.ReadConfigToJson();
            Algorithm enc;
            string PrivateKey = d.ConnectionStrings.PrivateKey.ToString();
            if (PrivateKey == "")
                enc = new Algorithm();
            else
                enc = new Algorithm(PrivateKey);
            DBType = d.ConnectionStrings.Type.ToString();
            ConnStr = enc.Decrypt(d.ConnectionStrings.ConnectionString[CompanyCode].ToString());
            switch (DBType)
            {
                case "SQLSERVER": toolDAO = new ToolMSSQL(ConnStr, context); break;
                case "ORACLE": toolDAO = new ToolOracle(ConnStr, context); break;
            }
        }
        public ToolDAO(string CompanyCode, HRSContext _context, bool IsDBLoad = false)
        {
            context = _context;
            dynamic d = context._appConfig;// JObject.Parse(context.AppConfig.ReadConfig(context));
            if (IsDBLoad)
                DBType = d.ConnectionStrings.ConnectionString.DBLoadType.ToString();
            else
                DBType = d.ConnectionStrings.Type.ToString();
            ConnStr = context.enc.Decrypt(d.ConnectionStrings.ConnectionString[CompanyCode].ToString());
            switch (DBType)
            {
                case "SQLSERVER": toolDAO = new ToolMSSQL(ConnStr, context); break;
                case "ORACLE": toolDAO = new ToolOracle(ConnStr, context); break;
            }
        }

        ~ToolDAO()
        {
            DBType = null; ConnStr = null; toolDAO = null; context = null;
        }
        #endregion

        #region Method For Service
        public void ExecuteSQLService(string functionName, string SQL, ref string json, ref int errorCode, ref string errorString)
        {
            toolDAO.ExecuteSQLService(functionName, SQL, ref json, ref errorCode, ref errorString);
        }
        public void ExecuteStoreService(string functionName, string storeName, dynamic parameterInput, ref string parameterOutput, ref string json, ref int errorCode, ref string errorString)
        {
            toolDAO.ExecuteStoreService(functionName, storeName, parameterInput, ref parameterOutput, ref json, ref errorCode, ref errorString);
        }
        #endregion

        #region Method For App
        public void ExecuteSQL(string functionName, string SQL, ref string json, ref int errorCode, ref string errorString)
        {
            toolDAO.ExecuteSQL(functionName, SQL, ref json, ref errorCode, ref errorString);
        }
        public void ExecuteStore(string functionName, string storeName, dynamic parameterInput, ref string parameterOutput, ref string json, ref int errorCode, ref string errorString, bool IsStoreCache = true)
        {
            toolDAO.ExecuteStore(functionName, storeName, parameterInput, ref parameterOutput, ref json, ref errorCode, ref errorString, IsStoreCache);
        }
        #endregion

        #region Method For API
        public void ExecuteStoreAPI(string functionName, string storeName, dynamic parameterInput, ref string parameterOutput, ref string json, ref int errorCode, ref string errorString, bool IsStoreCache = true)
        {
            toolDAO.ExecuteStoreAPI(functionName, storeName, parameterInput, ref parameterOutput, ref json, ref errorCode, ref errorString, IsStoreCache);
        }
        #endregion
    }
}    