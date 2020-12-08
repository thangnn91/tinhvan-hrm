using System;
using System.Text;
namespace Utils
{
    public static class HTTP_CODE
    {
        #region Properties
        public const int HTTP_NOT_FOUND = 404;
        public const int HTTP_NOT_AUTHEN = 401;
        public const int HTTP_BAD_REQUEST = 400;
        public const int HTTP_ERROR_SERVER = 500;
        public const int HTTP_NO_CONTENT = 204;
        public const int HTTP_ACCEPT = 200;
        #endregion

        #region Method
        public static string Base64Encode(string txt)
        {
            byte[] data;
            data = Encoding.UTF8.GetBytes(txt);
            return Convert.ToBase64String(data);
        }

        public static string Base64Decode(string txtBase64Encode)
        {
            byte[] data;
            data = Convert.FromBase64String(txtBase64Encode);
            return Encoding.UTF8.GetString(data);
        }

        public static void WriteLogAction(string logMessage, HRSContext context)
        {
            context.APIConfig.WriteLogAction(logMessage, context);
        }

        public static void WriteLogAction(string logMessage)
        {
            ToolFolder t = new ToolFolder();
            t.WriteLogAction(logMessage);
        }

        public static void WriteLogServiceIn(string logMessage, HRSContext context)
        {
            context.APIConfig.WriteLogServiceIn(logMessage, context);
        }

        public static void WriteLogServiceIn(string logMessage)
        {
            ToolFolder t = new ToolFolder();
            t.WriteLogServiceIn(logMessage);
        }

        public static void WriteLogServiceOut(string logMessage)
        {
            ToolFolder t = new ToolFolder();
            t.WriteLogServiceOut(logMessage);
        }
        #endregion
    }
}    