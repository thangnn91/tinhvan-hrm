using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Utils
{
    public class ToolOracle
    {
        #region Properties
        private string ConnStr;
        private HRSContext context;
        public OracleConnection Conn;
        #endregion

        #region Method Contruction
        public ToolOracle(string _connStr, HRSContext _context)
        {
            ConnStr = _connStr;
            context = _context;
            OpenDB(out Conn);
        }
        public ToolOracle(string _connStr)
        {
            ConnStr = _connStr;
            context = null;
            OpenDB(out Conn);
        }
        ~ToolOracle()
        {
            ConnStr = null;
            context = null;
            CloseDB(ref Conn);
        }
        #endregion

        #region Private Method
        private int SwapDbType(dynamic ParamType, dynamic ParamLength, out int nl)
        {
            int n = int.Parse(ParamType.ToString());
            nl = int.Parse(ParamLength.ToString());
            int n1 = 0;
            switch (n)
            {
                case 999:
                    n1 = 121; nl = -1;
                    break;
                case 0:
                    //n1 = 113;
                    n1 = 107;
                    nl = 19;
                    break;
                case 8:
                    //n1 = 112;
                    n1 = 107;
                    nl = 10;
                    break;
                case 16:
                    //n1 = 111;
                    n1 = 107;
                    nl = 5;
                    break;
                case 20:
                case 5:
                case 6:
                case 13:
                    n1 = 107;
                    nl = 19;
                    break;
                case 31:
                    n1 = 106; nl = -1;
                    break;
                case 4:
                    n1 = 123; nl = -1;
                    break;
                case 32:
                    n1 = 123; nl = -1;
                    break;
                case 9:
                    n1 = 107;
                    nl = 19;
                    break;
                case 17:
                    n1 = 107;
                    nl = 10;
                    break;
                case 10:
                    n1 = 117;
                    break;
                case 12:
                    n1 = 119; if (nl == -1) n1 = 105;
                    break;
                case 22:
                    n1 = 126;
                    break;
                default:
                    n1 = n;
                    break;
            }
            return n1;
        }
        private void OpenDB(out OracleConnection Conn)
        {
            Conn = new OracleConnection(ConnStr);
            Conn.Open();
        }
        private void CloseDB(ref OracleConnection Conn)
        {
            try { Conn.Close(); } catch { }
            Conn = null;
        }
        #endregion

        #region Method For Service
        public void ExecuteSQLService(string functionName, string SQL, ref string json, ref int errorCode, ref string errorString)
        {
            HTTP_CODE.WriteLogServiceIn("functionName:" + functionName + "\nSQL:" + SQL);
            int RSCount = 0; OracleCommand cmd; //OracleConnection Conn; 
            //OpenDB(out Conn);
            cmd = new OracleCommand()
            {
                CommandText = SQL,
                CommandType = CommandType.Text,
                Connection = Conn
            };
            string ItemType = ""; bool IsSetItemType = false;
            json = ""; errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = "Không có giá trị";
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    RSCount = 0; //RS.RecordCount;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsonSub = ""; RSCount = RSCount + 1;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (!IsSetItemType)
                                {
                                    ItemType = ItemType + ",\"" + reader.GetName(i) + "\":\"" + reader.GetDataTypeName(i) + "\"";
                                }
                                jsonSub = jsonSub + "," + "\"" + reader.GetName(i) + "\":\"" + reader.GetValue(i).ToString() + "\"";
                            }
                            json = json + ",{" + Tools.RemoveFisrtChar(jsonSub) + "}";
                            IsSetItemType = true;
                        }
                        json = Tools.RemoveFisrtChar(json);
                        errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = "OK";//"Truy vấn thành công";
                    }
                    reader.Close();
                }
                if (ItemType != "") ItemType = Tools.RemoveFisrtChar(ItemType);
                ItemType = "{" + ItemType + "}";
                json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"ItemType\": " + ItemType + ", \"Items\":[" + json + "]}}";
                HTTP_CODE.WriteLogServiceIn("json:" + json);
            }
            catch (Exception e)
            {
                RSCount = 0; errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"Items\":[]}}";
                errorString = e.ToString();
                HTTP_CODE.WriteLogServiceIn("errorString:" + errorString + "\njson:" + json);
            }
            //CloseDB(ref Conn);
        }
        public void ExecuteStoreService(string functionName, string storeName, dynamic parameterInput, 
            ref string parameterOutput, ref string json, ref int errorCode, ref string errorString)
        {
            bool kt = false; dynamic d; int RSCount = 0; OracleDataReader reader; OracleCommand cmd; OracleParameter[] param = new OracleParameter[300]; 
            cmd = new OracleCommand()
            {
                CommandText = storeName,
                CommandType = CommandType.StoredProcedure,
                Connection = Conn
            };
            if (parameterInput != null)
                HTTP_CODE.WriteLogServiceIn("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput:" + parameterInput.ToString());
            else
                HTTP_CODE.WriteLogServiceIn("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput:");

            string ItemType = ""; bool IsSetItemType = false; 
            json = ""; parameterOutput = ""; errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = "Không có giá trị";
            
            d = parameterInput;
            if (d != null) kt = true;
            try
            {
                // Create parameter
                if (kt)
                {
                    for (var i = 0; i < d.parameterInput.Count; i++)
                    {
                        int nl = int.Parse(d.parameterInput[i].ParamLength.ToString());
                        int n1 = SwapDbType(d.parameterInput[i].ParamType, d.parameterInput[i].ParamLength, out nl);
                        if (n1 == 121)
                        {
                            param[i] = new OracleParameter();
                            param[i].ParameterName = "p_" + d.parameterInput[i].ParamName.ToString();
                            param[i].OracleDbType = (OracleDbType)n1;// (OracleDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                        }
                        else
                        {
                            param[i] = new OracleParameter();
                            param[i].ParameterName = "p_" + d.parameterInput[i].ParamName.ToString();
                            param[i].OracleDbType = (OracleDbType)n1;// (OracleDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                            param[i].Value = d.parameterInput[i].InputValue.ToString();
                            if (nl > 0) param[i].Size = nl;
                        }
                        cmd.Parameters.Add(param[i]);
                    }
                }

                using (reader = cmd.ExecuteReader())
                {
                    RSCount = 0; //RS.RecordCount;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsonSub = ""; RSCount = RSCount + 1;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (!IsSetItemType)
                                {
                                    ItemType = ItemType + ",\"" + reader.GetName(i) + "\":\"" + reader.GetDataTypeName(i) + "\"";
                                }
                                jsonSub = jsonSub + "," + "\"" + reader.GetName(i) + "\":\"" + context.ReplaceStringLangValue(reader.GetValue(i).ToString()) + "\"";
                            }
                            json = json + ",{" + Tools.RemoveFisrtChar(jsonSub) + "}";
                            IsSetItemType = true;
                        }
                        json = Tools.RemoveFisrtChar(json);
                        errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = context.GetLanguageLable("OK");//"Truy vấn thành công";
                    }
                    if (ItemType != "") ItemType = Tools.RemoveFisrtChar(ItemType);
                    ItemType = "{" + ItemType + "}";
                    //json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"ItemType\": " + ItemType + ", \"Items\":[" + json + "]}}";
                    reader.Close();
                }

                parameterOutput = "";
                if (kt)
                {
                    for (var i = 0; i < d.parameterInput.Count; i++)
                    {
                        if ((d.parameterInput[i].ParamInOut.ToString() == "3") || (d.parameterInput[i].ParamInOut.ToString() == "2"))
                        {
                            if (d.parameterInput[i].ParamName.ToString().ToLower() == "responsestatus")
                            {
                                long longCode = long.Parse(param[i].Value.ToString());
                                //errorCode = long.Parse(resStatus);
                                if (longCode > 0)
                                    errorCode = HTTP_CODE.HTTP_ACCEPT;
                                else if (longCode == -99)
                                    errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                                else
                                    errorCode = HTTP_CODE.HTTP_ACCEPT; //HTTP_CODE.HTTP_BAD_REQUEST;
                            }
                            if (d.parameterInput[i].ParamName.ToString().ToLower() == "message")
                            {
                                parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":\"" + param[i].Value.ToString().Replace("\"", "\\\"") + "\"";
                                errorString = param[i].Value.ToString();
                            }
                            else
                            {
                                int nll = 0;
                                parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":" +
                                "\"" + (SwapDbType(d.parameterInput[i].ParamType, d.parameterInput[i].ParamLength, out nll) == 105 ? ((Oracle.ManagedDataAccess.Types.OracleClob)param[i].Value).Value : param[i].Value.ToString()).ToString().Replace("\"", "\\\"") + "\"";
                            }
                        }
                    }
                }
                if (parameterOutput != "")
                {
                    parameterOutput = "\"ParameterOutput\": {" + Tools.RemoveFisrtChar(parameterOutput) + "}"; //json = Tools.RemoveFisrtChar(json);
                }
                else
                {
                    parameterOutput = "\"ParameterOutput\": null";
                }

                // Ghep response
                json = "{\"" + functionName + "\":{" + parameterOutput + ", \"Count\": " + RSCount + ", \"ItemType\": " + ItemType + ", \"Items\":[" + json + "]}}";

                HTTP_CODE.WriteLogServiceIn("\njson:" + json);
            }
            catch (Exception e)
            {
                json = "{\"" + functionName + "\": {\"ParameterOutput\": null, \"Count\": " + RSCount + ", \"Items\":[]}}"; RSCount = 0; errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                errorString = e.ToString();
                HTTP_CODE.WriteLogServiceIn("errorString:" + errorString + "\njson:" + json);
            }
            //CloseDB(ref Conn);
        }
        #endregion

        #region Method For App
        public void ExecuteSQL(string functionName, string SQL, ref string json, ref int errorCode, ref string errorString)
        {
            HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nSQL:" + SQL, context);
            int RSCount = 0; OracleCommand cmd; //OracleConnection Conn; 
            //OpenDB(out Conn);
            cmd = new OracleCommand()
            {
                CommandText = SQL,
                CommandType = CommandType.Text,
                Connection = Conn
            };
            string ItemType = ""; bool IsSetItemType = false;
            json = ""; errorCode = HTTP_CODE.HTTP_ACCEPT/*HTTP_NO_CONTENT*/; errorString = context.GetLanguageLable("NOCONTENT");//"Không có giá trị";
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    RSCount = 0; //RS.RecordCount;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsonSub = ""; RSCount = RSCount + 1;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (!IsSetItemType)
                                {
                                    ItemType = ItemType + ",\"" + reader.GetName(i) + "\":\"" + reader.GetDataTypeName(i) + "\"";
                                }
                                jsonSub = jsonSub + "," + "\"" + reader.GetName(i) + "\":\"" + context.ReplaceStringLangValue(reader.GetValue(i).ToString()) + "\"";
                            }
                            json = json + ",{" + Tools.RemoveFisrtChar(jsonSub) + "}";
                            IsSetItemType = true;
                        }
                        json = Tools.RemoveFisrtChar(json);
                        errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = context.GetLanguageLable("OK");//"Truy vấn thành công";
                    }
                    reader.Close();
                }
                if (ItemType != "") ItemType = Tools.RemoveFisrtChar(ItemType);
                ItemType = "{" + ItemType + "}";
                json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"ItemType\": " + ItemType + ", \"Items\":[" + json + "]}}";
                HTTP_CODE.WriteLogAction("json:" + json, context);
            }
            catch (Exception e)
            {
                RSCount = 0; errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"Items\":[]}}";
                errorString = context.RoleDebug(e.ToString());
                HTTP_CODE.WriteLogAction("errorString:" + errorString + "\njson:" + json, context);
            }
            //CloseDB(ref Conn);
        }
        public void ExecuteStore(string functionName, string storeName, dynamic parameterInput, ref string parameterOutput, ref string json, ref int errorCode, ref string errorString, bool IsNoCache = false)
        {
            bool kt = false; dynamic d; int RSCount = 0; OracleDataReader reader; OracleCommand cmd; OracleParameter[] param = new OracleParameter[300]; //OracleConnection Conn; 
            //OpenDB(out Conn);
            //storeName = storeName.ToUpper();
            cmd = new OracleCommand()
            {
                CommandText = storeName,
                CommandType = CommandType.StoredProcedure,
                Connection = Conn
            };
            if (parameterInput != null)
                HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput:" + parameterInput.ToString(), context);
            else
                HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput:", context);

            string ItemType = ""; bool IsSetItemType = false; string keyCache = functionName + "." + storeName; bool IsCacheReq = false; bool IsCache = false;
            json = ""; parameterOutput = ""; errorCode = HTTP_CODE.HTTP_ACCEPT/*HTTP_NO_CONTENT*/; errorString = context.GetLanguageLable("NOCONTENT");//"Không có giá trị";
            //try {
            //@UserID;3;1;4;0^@WebID;3;1;4;0^@GrpId;3;1;4;0^@Status;2;1;2;0^@Page;3;1;4;0^@PageSize;3;1;4;0^@Rowcount;3;3;4;0
            //{"parameterInput":[{"ParamName": "@UserID", "ParamType": "3", "ParamInOut": "1", "ParamLength": "4", "InputValue": "3"}]}
            //{"parameterOutput":{"@UserID": "3", "@IsActive": "1"}}
            //d = JObject.Parse (parameterInput);
            //kt = true;
            //} catch {
            //	kt = false; d = null;
            //}
            d = parameterInput;
            if (d != null) kt = true;
            try
            {
                // Create parameter
                if (kt)
                {
                    for (var i = 0; i < d.parameterInput.Count; i++)
                    {
                        if (d.parameterInput[i].ParamInOut.ToString() != "3") keyCache = keyCache + "." + d.parameterInput[i].ParamName.ToString() + "." + d.parameterInput[i].InputValue.ToString();
                        int nl = int.Parse(d.parameterInput[i].ParamLength.ToString());
                        int n1 = SwapDbType(d.parameterInput[i].ParamType, d.parameterInput[i].ParamLength, out nl);
                        if (n1 == 121)
                        {
                            param[i] = new OracleParameter();
                            param[i].ParameterName = "p_" + d.parameterInput[i].ParamName.ToString();
                            param[i].OracleDbType = (OracleDbType)n1;// (OracleDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                        }
                        else
                        {
                            param[i] = new OracleParameter();
                            param[i].ParameterName = "p_" + d.parameterInput[i].ParamName.ToString();
                            param[i].OracleDbType = (OracleDbType)n1;// (OracleDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                            param[i].Value = d.parameterInput[i].InputValue.ToString();
                            if (nl > 0) param[i].Size = nl;
                        }                        
                        cmd.Parameters.Add(param[i]);
                    }
                }

                IsCacheReq = (keyCache.Length < 100 && context.TimeCache > 0);
                if (!IsNoCache) if (IsCacheReq) IsCache = context._cache.Get(keyCache + "_" + context.GetSession("language"), out json);
                if (!IsCache)
                {
                    using (reader = cmd.ExecuteReader())
                    {
                        RSCount = 0; //RS.RecordCount;
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string jsonSub = ""; RSCount = RSCount + 1;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (!IsSetItemType)
                                    {
                                        ItemType = ItemType + ",\"" + reader.GetName(i) + "\":\"" + reader.GetDataTypeName(i) + "\"";
                                    }
                                    jsonSub = jsonSub + "," + "\"" + reader.GetName(i) + "\":\"" + context.ReplaceStringLangValue(reader.GetValue(i).ToString()) + "\"";
                                }
                                json = json + ",{" + Tools.RemoveFisrtChar(jsonSub) + "}";
                                IsSetItemType = true;
                            }
                            json = Tools.RemoveFisrtChar(json);
                            errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = context.GetLanguageLable("OK");//"Truy vấn thành công";
                        }
                        if (ItemType != "") ItemType = Tools.RemoveFisrtChar(ItemType);
                        ItemType = "{" + ItemType + "}";
                        //json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"ItemType\": " + ItemType + ", \"Items\":[" + json + "]}}";
                        reader.Close();
                    }

                    parameterOutput = "";
                    if (kt)
                    {
                        for (var i = 0; i < d.parameterInput.Count; i++)
                        {
                            if ((d.parameterInput[i].ParamInOut.ToString() == "3") || (d.parameterInput[i].ParamInOut.ToString() == "2"))
                            {                                
                                if (d.parameterInput[i].ParamName.ToString().ToLower() == "responsestatus")
                                {
                                    long longCode = long.Parse(param[i].Value.ToString());
                                    //errorCode = long.Parse(resStatus);
                                    if (longCode > 0)
                                        errorCode = HTTP_CODE.HTTP_ACCEPT;
                                    else if (longCode == -99)
                                        errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                                    else
                                        errorCode = HTTP_CODE.HTTP_ACCEPT; //HTTP_CODE.HTTP_BAD_REQUEST;
                                }
                                if (d.parameterInput[i].ParamName.ToString().ToLower() == "message")
                                {
                                    parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":\"" + context.GetLanguageLable(param[i].Value.ToString()).Replace("\"", "\\\"") + "\"";
                                    errorString = context.GetLanguageLable(param[i].Value.ToString());
                                }
                                else
                                {
                                    int nll = 0;
                                    parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":" +
                                    "\"" + context.ParamLanguage(d.parameterInput[i].ParamName.ToString(), (SwapDbType(d.parameterInput[i].ParamType, d.parameterInput[i].ParamLength, out nll) == 105 ? ((Oracle.ManagedDataAccess.Types.OracleClob)param[i].Value).Value : param[i].Value.ToString())).ToString().Replace("\"", "\\\"") + "\"";
                                }                                    
                            }
                        }
                    }
                    if (parameterOutput != "")
                    {
                        parameterOutput = "\"ParameterOutput\": {" + Tools.RemoveFisrtChar(parameterOutput) + "}"; //json = Tools.RemoveFisrtChar(json);
                    }
                    else
                    {
                        parameterOutput = "\"ParameterOutput\": null";
                    }

                    // Ghep response
                    json = "{\"" + functionName + "\":{" + parameterOutput + ", \"Count\": " + RSCount + ", \"ItemType\": " + ItemType + ", \"Items\":[" + json + "]}}";

                    if (!IsNoCache && errorCode == 200)
                    {
                        context._cache.Set(keyCache + "_" + context.GetSession("language"), json, context.TimeCache);
                        context._cache.Set(keyCache + "_ParameterOutput" + "_" + context.GetSession("language"), parameterOutput, context.TimeCache);
                        context._cache.Set(keyCache + "_errorCode" + "_" + context.GetSession("language"), errorCode, context.TimeCache);
                        context._cache.Set(keyCache + "_errorString" + "_" + context.GetSession("language"), errorString, context.TimeCache);
                    }
                    HTTP_CODE.WriteLogAction("No-Cache: " + keyCache + " => ParameterOutput:" + parameterOutput + "\njson:" + json, context);
                }
                else
                {
                    context._cache.Get(keyCache + "_ParameterOutput" + "_" + context.GetSession("language"), out parameterOutput);
                    object a;
                    context._cache.Get(keyCache + "_errorCode" + "_" + context.GetSession("language"), out a); errorCode = int.Parse(a.ToString());
                    context._cache.Get(keyCache + "_errorString" + "_" + context.GetSession("language"), out errorString);
                    HTTP_CODE.WriteLogAction("Cache: " + keyCache + "_" + context.GetSession("language") + " => ParameterOutput:" + parameterOutput + "\njson:" + json, context);
                }
            }
            catch (Exception e)
            {
                json = "{\"" + functionName + "\": {\"ParameterOutput\": null, \"Count\": " + RSCount + ", \"Items\":[]}}"; RSCount = 0; errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                errorString = context.RoleDebug(e.ToString());
                HTTP_CODE.WriteLogAction("errorString:" + errorString + "\njson:" + json, context);
            }
            //CloseDB(ref Conn);
        }
        #endregion

        #region Method For API
        public void ExecuteStoreAPI(string functionName, string storeName, dynamic parameterInput,
            ref string parameterOutput, ref string json, ref int errorCode, ref string errorString, bool IsNoCache = true)
        {
            bool kt = false; dynamic d; int RSCount = 0; OracleCommand cmd; OracleParameter[] param = new OracleParameter[300];//OracleConnection Conn; 
            //OpenDB(out Conn);
            cmd = new OracleCommand()
            {
                CommandText = storeName,
                CommandType = CommandType.StoredProcedure,
                Connection = Conn
            };
            string ItemType = ""; bool IsSetItemType = false; string keyCache = functionName + "." + storeName; bool IsCacheReq = false; bool IsCache = false;
            json = ""; parameterOutput = ""; errorCode = HTTP_CODE.HTTP_ACCEPT/*HTTP_NO_CONTENT*/; errorString = context.GetLanguageLable("NOCONTENT");//"Không có giá trị"; 

            //try {
            //@UserID;3;1;4;0^@WebID;3;1;4;0^@GrpId;3;1;4;0^@Status;2;1;2;0^@Page;3;1;4;0^@PageSize;3;1;4;0^@Rowcount;3;3;4;0
            //{"parameterInput":[{"ParamName": "@UserID", "ParamType": "3", "ParamInOut": "1", "ParamLength": "4", "InputValue": "3"}]}
            //{"parameterOutput":{"@UserID": "3", "@IsActive": "1"}}
            //d = JObject.Parse (parameterInput);
            //kt = true;
            //} catch {
            //	kt = false; d = null;
            //}
            d = parameterInput;
            if (d == null)
                HTTP_CODE.WriteLogServiceIn("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput: null", context);
            else
            {
                kt = true;
                HTTP_CODE.WriteLogServiceIn("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput:" + parameterInput.ToString(), context);
            }

            try
            {
                // Create parameter
                if (kt)
                {
                    for (var i = 0; i < d.parameterInput.Count; i++)
                    {
                        if (d.parameterInput[i].ParamInOut.ToString() != "3") keyCache = keyCache + "." + d.parameterInput[i].ParamName.ToString() + "." + d.parameterInput[i].InputValue.ToString();
                        int nl = int.Parse(d.parameterInput[i].ParamLength.ToString());
                        int n1 = SwapDbType(d.parameterInput[i].ParamType, d.parameterInput[i].ParamLength, out nl);
                        if (n1 == 121)
                        {
                            param[i] = new OracleParameter();
                            param[i].ParameterName = "p_" + d.parameterInput[i].ParamName.ToString();
                            param[i].OracleDbType = (OracleDbType)n1;// (OracleDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                        }
                        else
                        {
                            param[i] = new OracleParameter();
                            param[i].ParameterName = "p_" + d.parameterInput[i].ParamName.ToString();
                            param[i].OracleDbType = (OracleDbType)n1;// (OracleDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                            param[i].Value = d.parameterInput[i].InputValue.ToString();
                            if (nl > 0) param[i].Size = nl;
                        }
                        cmd.Parameters.Add(param[i]);
                    }
                }
                IsCacheReq = (keyCache.Length < 100 && context.TimeCache > 0);
                if (!IsNoCache) if (IsCacheReq) IsCache = context._cache.Get(keyCache + "_" + context.GetSession("language"), out json);
                if (!IsCache)
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        RSCount = 0; //RS.RecordCount;
                        if (reader.HasRows)
                        {
                            /*
                            DataTable schemaTable = reader.GetSchemaTable();
                            foreach (DataRow row in schemaTable.Rows)
                            {
                                json = json + "," + JsonAssign(row, schemaTable);
                                RSCount = RSCount + 1;
                            }
                            json = Tools.RemoveFisrtChar(json);
                            */
                            while (reader.Read())
                            {
                                string jsonSub = ""; RSCount = RSCount + 1;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string dataType = reader.GetDataTypeName(i).ToString();
                                    if (!IsSetItemType)
                                    {
                                        ItemType = ItemType + ",\"" + reader.GetName(i).ToUpper() + "\":\"" + dataType + "\"";
                                    }
                                    if (",char,nchar,nclob,nvarchar2,clob,varchar2,".IndexOf("," + dataType.ToLower() + ",") > -1)
                                        jsonSub = jsonSub + "," + "\"" + reader.GetName(i).ToUpper() + "\":\"" + context.ReplaceStringLangValue(reader.GetValue(i).ToString()) + "\"";
                                    else if (",date,timestamp,timestampltz,timestamptz,".IndexOf("," + dataType.ToLower() + ",") > -1)
                                        jsonSub = jsonSub + "," + "\"" + reader.GetName(i).ToUpper() + "\":\"" + context.ReplaceStringLangValue(DateTime.Parse(reader.GetValue(i).ToString()).ToString("MM/dd/yyyy HH:mm:ss")) + "\"";
                                    else
                                        jsonSub = jsonSub + "," + "\"" + reader.GetName(i).ToUpper() + "\":" + reader.GetValue(i).ToString();
                                }
                                json = json + ",{" + Tools.RemoveFisrtChar(jsonSub) + "}";
                                IsSetItemType = true;
                            }
                            json = Tools.RemoveFisrtChar(json);
                            errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = context.GetLanguageLable("OK");//"Truy vấn thành công";
                        }
                        if (ItemType != "") ItemType = Tools.RemoveFisrtChar(ItemType);
                        ItemType = "{" + ItemType + "}";
                        //json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"ItemType\": " + ItemType + ", \"Items\":[" + json + "]}}";
                        reader.Close();
                    }

                    parameterOutput = ""; long StatusCode = 0; string Msg = ""; bool IsStatus = false;
                    if (kt)
                    {
                        for (var i = 0; i < d.parameterInput.Count; i++)
                        {
                            if (((d.parameterInput[i].ParamInOut.ToString() == "3") || (d.parameterInput[i].ParamInOut.ToString() == "2")) && d.parameterInput[i].ParamType.ToString() != "999")
                            {
                                if (d.parameterInput[i].ParamName.ToString().ToLower() == "responsestatus")
                                {
                                    long longCode = long.Parse(param[i].Value.ToString());
                                    //errorCode = long.Parse(resStatus);
                                    if (longCode > 0)
                                        errorCode = HTTP_CODE.HTTP_ACCEPT;
                                    else if (longCode == -99)
                                        errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                                    else
                                        errorCode = HTTP_CODE.HTTP_ACCEPT; //HTTP_CODE.HTTP_BAD_REQUEST;
                                    //parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":" + param[i].Value.ToString(); - Ko dùng
                                    StatusCode = long.Parse(param[i].Value.ToString());
                                    IsStatus = true;
                                }
                                else if (d.parameterInput[i].ParamName.ToString().ToLower() == "message")
                                {
                                    //parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":\"" + context.GetLanguageLable(param[i].Value.ToString()).Replace("\"", "\\\"") + "\""; - Ko dùng
                                    errorString = context.GetLanguageLable(param[i].Value.ToString());
                                    Msg = errorString;
                                }
                                else
                                {
                                    int nll = 0;
                                    parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":" +
                                        (param[i].DbType.ToString().ToLower() == "string" ?
                                            "\"" + context.ParamLanguage(d.parameterInput[i].ParamName.ToString(), (SwapDbType(d.parameterInput[i].ParamType, d.parameterInput[i].ParamLength, out nll) == 105 ? ((Oracle.ManagedDataAccess.Types.OracleClob)param[i].Value).Value : param[i].Value.ToString())).ToString().Replace("\"", "\\\"") + "\"" :
                                            param[i].Value.ToString());
                                }                                    
                            }
                        }
                    }
                    if (parameterOutput != "" || IsStatus)
                    {
                        if (!IsStatus) // ko co ResponseStatus
                            parameterOutput = "\"ParameterOutput\": {" + Tools.RemoveFisrtChar(parameterOutput) + "}, ";
                        else if (StatusCode > 0) // Co ResponseStatus
                            parameterOutput = "\"ParameterOutput\": {" + Tools.RemoveFisrtChar(parameterOutput) + "}, " +
                                "\"ResponseStatus\": " + StatusCode.ToString() + ", " +
                                "\"Message\": \"" + Msg + "\", ";
                        else
                            parameterOutput = "\"ParameterOutput\": null, " +
                                "\"ResponseStatus\": " + StatusCode.ToString() + ", " +
                                "\"Message\": \"" + Msg + "\", ";
                        //parameterOutput = Tools.RemoveFisrtChar(parameterOutput)+ ","; //json = Tools.RemoveFisrtChar(json);
                    }
                    //else
                    //{
                    //    //parameterOutput = "\"ParameterOutput\": null";
                    //}

                    // Ghep response
                    json = "{\"" + functionName + "\":{" + parameterOutput + " \"Count\": " + RSCount + ", \"Items\":[" + json + "]}}";// + ", \"ItemType\": " + ItemType

                    if (!IsNoCache && errorCode == 200)
                    {
                        context._cache.Set(keyCache + "_" + context.GetSession("language"), json, context.TimeCache);
                        context._cache.Set(keyCache + "_ParameterOutput" + "_" + context.GetSession("language"), parameterOutput, context.TimeCache);
                        context._cache.Set(keyCache + "_errorCode" + "_" + context.GetSession("language"), errorCode, context.TimeCache);
                        context._cache.Set(keyCache + "_errorString" + "_" + context.GetSession("language"), errorString, context.TimeCache);
                    }
                    HTTP_CODE.WriteLogServiceIn("No-Cache: " + keyCache + " => ParameterOutput:" + parameterOutput + "\njson:" + json, context);
                }
                else
                {
                    context._cache.Get(keyCache + "_ParameterOutput" + "_" + context.GetSession("language"), out parameterOutput);
                    object a;
                    context._cache.Get(keyCache + "_errorCode" + "_" + context.GetSession("language"), out a); errorCode = int.Parse(a.ToString());
                    context._cache.Get(keyCache + "_errorString" + "_" + context.GetSession("language"), out errorString);
                    HTTP_CODE.WriteLogServiceIn("Cache: " + keyCache + "_" + context.GetSession("language") + " => ParameterOutput:" + parameterOutput + "\njson:" + json, context);
                }
            }
            catch (Exception e)
            {
                json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"Items\":[]}}"; RSCount = 0; errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                errorString = context.RoleDebug(e.ToString());
                HTTP_CODE.WriteLogServiceIn("errorString:" + errorString + "\njson:" + json, context);
            }
            //CloseDB(ref Conn);
            //RS = null;
        }
        #endregion       
    }
}    