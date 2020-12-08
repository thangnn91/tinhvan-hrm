using System;
using System.Data.SqlClient;
using System.Data;

namespace Utils
{
    public class ToolMSSQL {
        #region Properties
        private string ConnStr;
        private HRSContext context;
        public SqlConnection Conn;
        #endregion

        #region Method Contruction
        public ToolMSSQL(string _connStr, HRSContext _context) {
            ConnStr = _connStr;
            context = _context;
            OpenDB(out Conn);
        }
        public ToolMSSQL (string _connStr)
        {
            ConnStr = _connStr;
            context = null;
            OpenDB(out Conn);
        }
        ~ToolMSSQL()
        {
            ConnStr = null;
            context = null;
            CloseDB(ref Conn);
        }
        #endregion

        #region Private Method
        private void OpenDB(out SqlConnection Conn) {
            Conn = new SqlConnection(ConnStr);
            Conn.Open();
        }
        private void CloseDB(ref SqlConnection Conn) {
            try { Conn.Close(); } catch { }
            Conn = null;
        }
        #endregion

        #region Method For Service
        public void ExecuteSQLService(string functionName, string SQL, ref string json, ref int errorCode, ref string errorString)
        {
            HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nSQL:" + SQL);
            int RSCount = 0; SqlCommand cmd; //SqlConnection Conn; 
            //OpenDB(out Conn);
            cmd = new SqlCommand()
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
                    RSCount = 0; 
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
                HTTP_CODE.WriteLogAction("json:" + json);
            }
            catch (Exception e)
            {
                RSCount = 0; errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"Items\":[]}}";
                errorString = e.ToString();
                HTTP_CODE.WriteLogAction("errorString:" + errorString + "\njson:" + json);
            }
        }
        public void ExecuteStoreService(string functionName, string storeName, dynamic parameterInput,
            ref string parameterOutput, ref string json, ref int errorCode, ref string errorString)
        {
            bool kt = false; dynamic d; int RSCount = 0; SqlCommand cmd; SqlParameter[] param = new SqlParameter[300];
            cmd = new SqlCommand()
            {
                CommandText = storeName,
                CommandType = CommandType.StoredProcedure,
                Connection = Conn
            };
            string ItemType = ""; bool IsSetItemType = false; 
            json = ""; parameterOutput = ""; errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = "Không có giá trị"; 

            d = parameterInput;
            if (d == null)
                HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput: null");
            else
            {
                kt = true;
                HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput:" + parameterInput.ToString());
            }

            try
            {
                // Create parameter
                if (kt)
                {
                    for (var i = 0; i < d.parameterInput.Count; i++)
                    {
                        if (d.parameterInput[i].ParamType.ToString() != "999")
                        {
                            param[i] = new SqlParameter();
                            param[i].ParameterName = "@" + d.parameterInput[i].ParamName.ToString();
                            param[i].SqlDbType = (SqlDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                            param[i].Value = d.parameterInput[i].InputValue.ToString();
                            param[i].Size = int.Parse(d.parameterInput[i].ParamLength.ToString());
                            cmd.Parameters.Add(param[i]);
                        }
                    }
                }
                using (var reader = cmd.ExecuteReader())
                {
                    RSCount = 0;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsonSub = ""; RSCount = RSCount + 1;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (!IsSetItemType)
                                {
                                    ItemType = ItemType + ",\"" + reader.GetName(i).ToUpper() + "\":\"" + reader.GetDataTypeName(i) + "\"";
                                }
                                jsonSub = jsonSub + "," + "\"" + reader.GetName(i).ToUpper() + "\":\"" + reader.GetValue(i).ToString() + "\"";
                            }
                            json = json + ",{" + Tools.RemoveFisrtChar(jsonSub) + "}";
                            IsSetItemType = true;
                        }
                        json = Tools.RemoveFisrtChar(json);
                        errorCode = HTTP_CODE.HTTP_ACCEPT; errorString = "OK";//"Truy vấn thành công";
                    }
                    if (ItemType != "") ItemType = Tools.RemoveFisrtChar(ItemType);
                    ItemType = "{" + ItemType + "}";
                    reader.Close();
                }

                parameterOutput = "";
                if (kt)
                {
                    for (var i = 0; i < d.parameterInput.Count; i++)
                    {
                        if (((d.parameterInput[i].ParamInOut.ToString() == "3") || (d.parameterInput[i].ParamInOut.ToString() == "2")) && d.parameterInput[i].ParamType.ToString() != "999")
                        {
                            if (d.parameterInput[i].ParamName.ToString().ToLower() == "responsestatus")
                            {
                                long longCode = long.Parse(param[i].Value.ToString());
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
                                parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":\"" + param[i].Value.ToString().Replace("\"", "\\\"") + "\"";
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
                HTTP_CODE.WriteLogAction("\njson:" + json);
            }
            catch (Exception e)
            {
                json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"Items\":[]}}"; RSCount = 0; errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                errorString = e.ToString();
                HTTP_CODE.WriteLogAction("errorString:" + errorString + "\njson:" + json);
            }
            //CloseDB(ref Conn);
            //RS = null;
        }
        #endregion

        #region Method For App
        public void ExecuteSQL(string functionName, string SQL, ref string json, ref int errorCode, ref string errorString)
        {
            HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nSQL:" + SQL, context);
            int RSCount = 0; SqlCommand cmd; //SqlConnection Conn; 
            //OpenDB(out Conn);
            cmd = new SqlCommand()
            {
                CommandText = SQL,
                CommandType = CommandType.Text,
                Connection = Conn
            };
            string ItemType = ""; bool IsSetItemType = false;
            json = ""; errorCode = HTTP_CODE.HTTP_ACCEPT/*HTTP_NO_CONTENT*/; errorString = context.GetLanguageLable("NOCONTENT");// "Không có giá trị";
            try
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
        public void ExecuteStore(string functionName, string storeName, dynamic parameterInput, 
            ref string parameterOutput, ref string json, ref int errorCode, ref string errorString, bool IsNoCache = false)
        {
            bool kt = false; dynamic d; int RSCount = 0; SqlCommand cmd; SqlParameter [] param = new SqlParameter [300];//SqlConnection Conn; 
            //OpenDB(out Conn);
            cmd = new SqlCommand()
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
                HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput: null", context);
            else
            {
                kt = true;
                HTTP_CODE.WriteLogAction("functionName:" + functionName + "\nstoreName:" + storeName + "\nparameterInput:" + parameterInput.ToString(), context);
            }
                
            try
            {
                // Create parameter
                if (kt)
                {
                    for (var i = 0; i < d.parameterInput.Count; i++)
                    {
                        if (d.parameterInput[i].ParamType.ToString() != "999")
                        {
                            if (d.parameterInput[i].ParamInOut.ToString() != "3") keyCache = keyCache + "." + d.parameterInput[i].ParamName.ToString() + "." + d.parameterInput[i].InputValue.ToString();
                            param[i] = new SqlParameter();
                            param[i].ParameterName = "@" + d.parameterInput[i].ParamName.ToString();
                            param[i].SqlDbType = (SqlDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                            param[i].Value = d.parameterInput[i].InputValue.ToString();
                            param[i].Size = int.Parse(d.parameterInput[i].ParamLength.ToString());
                            cmd.Parameters.Add(param[i]);
                        }                        
                    }
                }
                IsCacheReq = (keyCache.Length < 100 && context.TimeCache > 0);
                if (!IsNoCache) if(IsCacheReq) IsCache = context._cache.Get(keyCache + "_" + context.GetSession("language"), out json);
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
                                    if (!IsSetItemType)
                                    {
                                        ItemType = ItemType + ",\"" + reader.GetName(i).ToUpper() + "\":\"" + reader.GetDataTypeName(i) + "\"";
                                    }
                                    jsonSub = jsonSub + "," + "\"" + reader.GetName(i).ToUpper() + "\":\"" + context.ReplaceStringLangValue(reader.GetValue(i).ToString()) + "\"";
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
                                }
                                if (d.parameterInput[i].ParamName.ToString().ToLower() == "message") {
                                    parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":\"" + context.GetLanguageLable(param[i].Value.ToString()).Replace("\"", "\\\"") + "\"";
                                    errorString = context.GetLanguageLable(param[i].Value.ToString());
                                }
                                else
                                    parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":\"" + context.ParamLanguage(d.parameterInput[i].ParamName.ToString(), param[i].Value.ToString()).Replace("\"", "\\\"") + "\"";
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
                json = "{\"" + functionName + "\": {\"Count\": " + RSCount + ", \"Items\":[]}}"; RSCount = 0; errorCode = HTTP_CODE.HTTP_ERROR_SERVER;
                errorString = context.RoleDebug(e.ToString());
                HTTP_CODE.WriteLogAction("errorString:" + errorString + "\njson:" + json, context);
            }
            //CloseDB(ref Conn);
            //RS = null;
        }
        #endregion

        #region Method For API
        public void ExecuteStoreAPI(string functionName, string storeName, dynamic parameterInput,
            ref string parameterOutput, ref string json, ref int errorCode, ref string errorString, bool IsNoCache = true)
        {
            bool kt = false; dynamic d; int RSCount = 0; SqlCommand cmd; SqlParameter[] param = new SqlParameter[300];//SqlConnection Conn; 
            //OpenDB(out Conn);
            cmd = new SqlCommand()
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
                        if (d.parameterInput[i].ParamType.ToString() != "999")
                        {
                            if (d.parameterInput[i].ParamInOut.ToString() != "3") keyCache = keyCache + "." + d.parameterInput[i].ParamName.ToString() + "." + d.parameterInput[i].InputValue.ToString();
                            param[i] = new SqlParameter();
                            param[i].ParameterName = "@" + d.parameterInput[i].ParamName.ToString();
                            param[i].SqlDbType = (SqlDbType)d.parameterInput[i].ParamType;
                            param[i].Direction = (ParameterDirection)d.parameterInput[i].ParamInOut;
                            param[i].Value = d.parameterInput[i].InputValue.ToString();
                            param[i].Size = int.Parse(d.parameterInput[i].ParamLength.ToString());
                            cmd.Parameters.Add(param[i]);
                        }
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
                                    if (",char,nchar,ntext,nvarchar,text,varchar,".IndexOf("," + dataType.ToLower() + ",")>-1)
                                        jsonSub = jsonSub + "," + "\"" + reader.GetName(i).ToUpper() + "\":\"" + context.ReplaceStringLangValue(reader.GetValue(i).ToString()) + "\"";
                                    else if (",datetime,smalldatetime,datetime2,datetimeoffset,".IndexOf("," + dataType.ToLower() + ",") > -1)
                                        jsonSub = jsonSub + "," + "\"" + reader.GetName(i).ToUpper() + "\":\"" + context.ReplaceStringLangValue(DateTime.Parse(reader.GetValue(i).ToString()).ToString("MM/dd/yyyy HH:mm:ss")) + "\"";
                                    else if (",date,".IndexOf("," + dataType.ToLower() + ",") > -1)
                                        jsonSub = jsonSub + "," + "\"" + reader.GetName(i).ToUpper() + "\":\"" + context.ReplaceStringLangValue(DateTime.Parse(reader.GetValue(i).ToString()).ToString("MM/dd/yyyy")) + "\"";
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

                    parameterOutput = ""; long StatusCode = 0; string Msg = ""; bool IsStatus = false ;
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
                                    parameterOutput = parameterOutput + ",\"" + d.parameterInput[i].ParamName + "\":" + 
                                        (param[i].DbType.ToString().ToLower() == "string" ? 
                                            "\"" + context.ParamLanguage(d.parameterInput[i].ParamName.ToString(), param[i].Value.ToString()).Replace("\"", "\\\"") + "\"" : 
                                            param[i].Value.ToString());
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
                        //parameterOutput = Tools.RemoveFisrtChar(parameterOutput) + ","; //json = Tools.RemoveFisrtChar(json);
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