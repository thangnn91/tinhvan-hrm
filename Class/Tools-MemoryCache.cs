using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Reflection;

namespace Utils
{
    public class HRSCache
    {
        public const double CacheByYear = 31104000;
        //public double CacheByMonth = 2592000;
        //public double CacheByDay = 86400;
        public double CacheByHour = 3600;
        public double CacheByMinute = 60;

        private IMemoryCache _cache;
        //private string _language;
        private const string KeyListProtect = ",,";
        public HRSCache(IMemoryCache cache)//, HttpContext httpContext)
        {
            _cache = cache;
            //_language = httpContext.Session.GetString("language");
            CacheByHour = 3600;
            CacheByMinute = 60;
        }

        public string[] GetAllKey()
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(_cache) as ICollection;
            var items = new System.Collections.Generic.List<string>();
            if (collection != null)
                foreach (var item in collection)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    items.Add(val.ToString());
                }
            items.Sort();
            return items.ToArray();
        }
        //private void SetListKey(string listKey)
        //{
        //    _cache.Set("KeyList", listKey, DateTime.Now.AddSeconds(43200));
        //}
        //private void SetListKeyAdd(string Key)
        //{
        //    string listKey;
        //    Get("KeyList", out listKey); if (listKey == null) listKey = "";
        //    if (listKey == "") listKey = Key; else if (("," + listKey + ",").IndexOf("," + Key + ",") < 0) listKey = listKey + "," + Key;
        //    SetListKey(listKey);
        //}
        //private void SetListKeyRemove(string Key)
        //{
        //    string listKey;
        //    Get("KeyList", out listKey); if (listKey == null) listKey = "";
        //    string[] arrKey = listKey.Split(new string[] { "," }, StringSplitOptions.None);
        //    listKey = arrKey[0]; for (int i = 1; i < arrKey.Length; i++) if (arrKey[i] != Key) listKey = listKey + "," + arrKey[i];
        //    SetListKey(listKey);
        //}
        public void Set(string Key, object Data, double iTime = CacheByYear)
        {
            if (Key == "KeyList") return; //Key = Key + "_"+ _language;
            //SetListKeyAdd(Key);
            _cache.Set(Key, Data, DateTime.Now.AddSeconds(iTime));// cacheEntryOptions);
        }
        public void Set(string Key, string Data, double iTime = CacheByYear)
        {
            if (Key == "KeyList") return; //Key = Key + "_" + _language;
            //SetListKeyAdd(Key);
            _cache.Set(Key, Data, DateTime.Now.AddSeconds(iTime));//cacheEntryOptions);
        }
        public bool Get(string Key, out object Data)
        {
            //Key = Key + "_" + _language;
            bool kt = _cache.TryGetValue(Key, out Data);
            return kt;
        }
        public bool Get(string Key, out string Data)
        {
            //Key = Key + "_" + _language;
            bool kt = _cache.TryGetValue(Key, out Data);
            if (Data == null) Data = "";
            return kt;
        }
        public void RemoveAll()
        {
            string[] arrKey = GetAllKey();
            for (int i = 0; i < arrKey.Length; i++)
            {
                RemoveOne(arrKey[i]); 
            }
        }
        public void RemoveOne(string Key)
        {
            int j = KeyListProtect.IndexOf("," + Key + ",");
            if (j < 0)
            //if (Key.Substring(0, 5) != "Menu_")
            {
                //SetListKeyRemove(Key);
                try { _cache.Remove(Key); } catch { }
            }                
        }
    }
}
