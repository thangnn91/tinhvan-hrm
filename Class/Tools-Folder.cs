using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Utils
{
    public class ToolFolder
    {
        #region Properties
        public string FolderRoot;
        private string FileConfigName; // "config.json"
        #endregion

        #region Contruction
        public ToolFolder()
        {
            FolderRoot = Directory.GetCurrentDirectory();
            FileConfigName = "config.json";
        }

        public ToolFolder(string configFile)
        {
            FolderRoot = Directory.GetCurrentDirectory();
            FileConfigName = configFile;
        }
        #endregion

        #region Method
        public bool PathIsDirectory(string path)
        {
            return Directory.Exists(path);
        }
        public Stream CreateFile(string path)
        {
            return new FileStream(path, FileMode.Create);
        }
        public MemoryStream CreateMemoryStream()
        {
            return new MemoryStream();
        }
        public MemoryStream CreateMemoryStream(byte[] buffer, int index, int count)
        {
            return new MemoryStream(buffer, index, count);
        }
        public bool PathIsFile(string path)
        {
            return File.Exists(path);
        }
        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
        public string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }
        public void PathCreateDirectory(string path)
        {
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
        public byte[] FileReadAllBytes(string srcPath)
        {
            if (PathIsFile(srcPath))
                return File.ReadAllBytes(srcPath);
            else
                return null;
        }
        public bool FileDelete(string srcPath)
        {
            bool kt = true;
            if (PathIsFile(srcPath))
            {
                try { File.Delete(srcPath); } catch { kt = false; }
            }
            return kt;
        }
        public void FileRename(string path, string newPath)
        {
            try
            {
                FileInfo f = new FileInfo(path);
                f.MoveTo(newPath);
            }
            catch
            {

            }
        }
        public void FileWrite(string path, string data)
        {
            try
            {
                using (StreamWriter w = File.AppendText(path))
                {
                    w.Write(data);
                    w.Close();
                }
            }
            catch
            {

            }
        }
        public string FileReadAllText(string srcPath)
        {
            if (PathIsFile(srcPath))
                return File.ReadAllText(srcPath);
            else
                return "";
        }

        public string ReadConfig(HRSContext context)
        {
            //string s = context.GetSession(FileConfigName);
            string s = "";
            bool IsCached = context._cache.Get(FileConfigName, out s);
            if (!IsCached) // (s=="")
            {
                WriteLogAction("Open file: " + FolderRoot + "\\" + FileConfigName, context);
                s = FileReadAllText(FolderRoot + "\\" + FileConfigName);
                //context.SetSession(FileConfigName, s);
                context._cache.Set(FileConfigName, s);
            }
            
            return s;
        }

        public string ReadConfig()
        {
            return FileReadAllText(FolderRoot + "\\" + FileConfigName); 
        }

        public FileStream GetFileStream(string path)
        {
            FileStream r = null;
            try {r = File.Open(path, FileMode.Open); } catch { }
            return r;
        }

        public dynamic ReadConfigToJson(HRSContext context)
        {
            dynamic d = null;
            bool IsCached = context._cache.Get(FileConfigName + "_Obj", out d);
            if (!IsCached) // (s=="")
            {
                string s = ReadConfig(context);//FileReadAllText(FolderRoot + "\\" + FileConfigName);
                d = JObject.Parse(s);
                context._cache.Set(FileConfigName + "_Obj", d);
            }
            return d;
        }

        public dynamic ReadConfigToJson()
        {
            string s = ReadConfig();//FileReadAllText(FolderRoot + "\\" + FileConfigName);
            dynamic d = JObject.Parse(s);
            return d;
        }

        public string GetLanguageDefault(HRSContext context)// bỏ
        {
            //dynamic d = ReadConfigToJson(context);
            return context.LanguageDefault; //d.LanguageDefault.ToString();
        }

        public void WriteLogAction(string logMessage, HRSContext context)
        {
            //string s = context.GetSession("appsettings.json");
            //if (s == "")
            //{
            //    s = FileReadAllText(FolderRoot + "\\" + FileConfigName);
            //    context.SetSession(FileConfigName, s);
            //}
            //dynamic d1 = JObject.Parse(s);
            string ip = context.RemoteIpAddress();
            if (context.WriteLog == "1")
            {
                string LogPath = FolderRoot + "\\" + "LOGS";
                PathCreateDirectory(LogPath);
                DateTime d = DateTime.Now;
                LogPath = LogPath + "\\" + d.ToString("yyyyMMddHH") + ip.Replace(".", "_").Replace(":", "_") + ".log";
                try
                {
                    using (StreamWriter w = File.AppendText(LogPath))
                    {                        
                        w.Write("\r\nLog Entry {0} {1} {2}: ", ip, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.WriteLine(logMessage);
                        //w.WriteLine ("-------------------------------");
                        w.Close();
                    }
                }
                catch
                {

                }
            }
        }
        public void WriteLogAction(string logMessage)
        {
            string LogPath = FolderRoot + "\\" + "LOGS";
            PathCreateDirectory(LogPath);
            DateTime d = DateTime.Now;
            LogPath = LogPath + "\\" + d.ToString("yyyyMMddHH") + ".log";
            try
            {
                using (StreamWriter w = File.AppendText(LogPath))
                {
                    w.Write("\r\nLog Entry {0} {1} {2}: ", "No-IP", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    w.WriteLine(logMessage);
                    //w.WriteLine ("-------------------------------");
                    w.Close();
                }
            }
            catch
            {

            }
        }
        public void WriteLogServiceIn(string logMessage)
        {
            string LogPath = FolderRoot + "\\" + "LOGS\\IN";
            PathCreateDirectory(LogPath);
            DateTime d = DateTime.Now;
            LogPath = LogPath + "\\" + d.ToString("yyyyMMddHH") + ".log";
            try
            {
                using (StreamWriter w = File.AppendText(LogPath))
                {
                    w.Write("\r\nLog Entry {0} {1} {2}: ", "No-IP", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    w.WriteLine(logMessage);
                    //w.WriteLine ("-------------------------------");
                    w.Close();
                }
            }
            catch
            {

            }
        }
        public void WriteLogServiceIn(string logMessage, HRSContext context)
        {
            //string s = context.GetSession("appsettings.json");
            //if (s == "")
            //{
            //    s = FileReadAllText(FolderRoot + "\\" + FileConfigName);
            //    context.SetSession(FileConfigName, s);
            //}
            //dynamic d1 = JObject.Parse(s);
            string ip = context.RemoteIpAddress();
            if (context.WriteLog == "1")
            {
                string LogPath = FolderRoot + "\\" + "LOGS\\IN";
                PathCreateDirectory(LogPath);
                DateTime d = DateTime.Now;
                LogPath = LogPath + "\\" + d.ToString("yyyyMMddHH") + ip.Replace(".", "_").Replace(":", "_") + ".log";
                try
                {
                    using (StreamWriter w = File.AppendText(LogPath))
                    {
                        w.Write("\r\nLog Entry {0} {1} {2}: ", ip, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.WriteLine(logMessage);
                        //w.WriteLine ("-------------------------------");
                        w.Close();
                    }
                }
                catch
                {

                }
            }
        }
        public void WriteLogServiceOut(string logMessage)
        {
            string LogPath = FolderRoot + "\\" + "LOGS\\OUT";
            PathCreateDirectory(LogPath);
            DateTime d = DateTime.Now;
            LogPath = LogPath + "\\" + d.ToString("yyyyMMddHH") + ".log";
            try
            {
                using (StreamWriter w = File.AppendText(LogPath))
                {
                    w.Write("\r\nLog Entry {0} {1} {2}: ", "No-IP", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    w.WriteLine(logMessage);
                    //w.WriteLine ("-------------------------------");
                    w.Close();
                }
            }
            catch
            {

            }
        }
        public void WriteLogService(string logMessage)
        {
            string LogPath = FolderRoot + "\\" + "LOGS\\Service";
            PathCreateDirectory(LogPath);
            DateTime d = DateTime.Now;
            LogPath = LogPath + "\\" + d.ToString("yyyyMMddHH") + ".log";
            try
            {
                using (StreamWriter w = File.AppendText(LogPath))
                {
                    w.Write("\r\nLog Entry {0} {1} {2}: ", "No-IP", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    w.WriteLine(logMessage);
                    //w.WriteLine ("-------------------------------");
                    w.Close();
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}    