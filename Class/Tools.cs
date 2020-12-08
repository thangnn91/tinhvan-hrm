using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils
{
    public class XlsxForm
    {
        public int iCol;
        public string colName;
        public string[] colList;
        public string[] colParamList;
        public XlsxForm()
        {
            colList = new string[1];
            colList[0] = "";
            colParamList = new string[1];
            colParamList[0] = "";
        }
        public int GetColPos(string val)
        {
            return Tools.GetArrayPos(val, colList);
        }
        public int GetColParamPos(string val)
        {
            return Tools.GetArrayPos(val, colParamList);
        }
    }
    public class XlsxFormSelect
    {
        public int iCol;
        public int iColChild;
        public string colName;
        public string colNameChild;
        public XlsxFormSelect()
        {
            iCol = -1;
            iColChild = -1;
            colName = "";
            colNameChild = "";
        }
    }
    public static class Tools
    {
        #region Private Properties
        private static readonly string HRMDateFormat = "^[0-3]?[0-9]/[0-1]?[0-9]/[0-9]{4}$";
        private static readonly string HRMDateTimeFormat = "^[0-3]?[0-9]/[0-1]?[0-9]/[0-9]{4} [0-2]?[0-9]:[0-5]?[0-9](:[0-5]?[0-9])?$";
        private static readonly string HRMNumberFormat = "[0-9]";
        private static readonly string HRMEmailFormat = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        private static readonly string HRMTimeFormat = "^[0-2]?[0-9]:[0-5]?[0-9](:[0-5]?[0-9])?$";
        public const string C_ActbSepr = " - ";
        public const string NumberSepr = ",";
        private const int MaxWhileCount = 10000;
        #endregion

        #region Method Check Valid
        public static bool IsHRMNumber(string value)
        {
            if (Regex.IsMatch(value, HRMNumberFormat)) return true; else return false;
        }
        public static bool IsHRMEmail(string value)
        {
            if (Regex.IsMatch(value, HRMEmailFormat)) return true; else return false;
        }
        public static bool IsHRMTime(string value)
        {
            if (Regex.IsMatch(value, HRMTimeFormat)) return true; else return false;
        }
        public static bool IsHRMDate(int y, int m, int d)
        {
            bool r = false; string dStr = d.ToString() + "/" + m.ToString() + "/" + y.ToString();
            if (m > 0 && m < 13)
            {
                if (d > 0 && d <= MDays(dStr))
                {
                    r = true;
                }
            }
            return r;
        }
        public static bool IsHRMDateExpr(string d)
        {
            bool r = false;
            if (d != "")
            {
                if (Regex.IsMatch(d, HRMDateFormat))
                {
                    string[] a; a = d.Split(new string[] { "/" }, StringSplitOptions.None);
                    if (a.Length == 3)
                    {
                        r = IsHRMDate(int.Parse(a[2]), int.Parse(a[1]), int.Parse(a[0]));
                    }
                }
            }
            return r;
        }
        public static bool IsHRMDateTimeExpr(string d)
        {
            bool r = false;
            if (d != "")
            {
                if (Regex.IsMatch(d, HRMDateTimeFormat))
                {
                    string[] a; a = ExtractDatePart(d).Split(new string[] { "/" }, StringSplitOptions.None);
                    if (a.Length == 3)
                    {
                        r = IsHRMDate(int.Parse(a[2]), int.Parse(a[1]), int.Parse(a[0]));
                    }
                }
            }
            return r;
        }
        #endregion

        #region Method Numeric
        public static string insertSepr(string d0)
        {
            var d = "" + d0; // convert to string
            var i = 0;
            var d2 = "";
            var ic = 0;
            var ofs = d.Length - 1;
            var decimalpoint = d.IndexOf('.');
            if (decimalpoint >= 0) ofs = decimalpoint - 1;
            for (i = ofs; i >= 0; i--)
            {
                if (d[i].ToString() != NumberSepr)
                {
                    if (ic++ % 3 == 0 && i != ofs && d[i] != '-') d2 = NumberSepr + d2;
                    d2 = d[i] + d2;
                }
            }

            if (decimalpoint >= 0)
            {
                for (i = decimalpoint; i < d.Length; i++)
                    d2 += d[i];
            }
            return d2;
        }
        public static string RemNumSepr(string num)
        {
            return num.Replace(NumberSepr, "");
        }
        public static string FormatNumber(string num, int iRound = 0)
        {
            double a = 0;
            try {
                a = double.Parse(num);
                a = Math.Round(a, iRound);
            }
            catch
            {
                a = 0;
            }
            if (a == 0) return "";
            return insertSepr(a.ToString());
            /*
            if (!IsHRMNumber(num)) num = "0";
            if (num == "0") return "";
            if (IsRound)
                return string.Format("{0:n0}", Double.Parse(num));
            else
                return string.Format("{0:n}", Double.Parse(num));
                */
        }
        public static int CIntNull (dynamic num)
        {
            string a = "";
            if (num == null)
                a = "0";
            else if (num == "")
                a = "0";
            else
                a = num.ToString();
            if (a == "") a = "0";
            int r = 0; try { r = int.Parse(a); } catch { }
            return r;
        }
        public static double CDblNull(dynamic num)
        {
            string a = num.ToString();
            if (num == null)
                a = "0";
            else
                a = num.ToString();
            if (a == "") a = "0";
            double r = 0; try { r = double.Parse(a); } catch { }
            return r;
        }
        #endregion

        #region Method Datetime
        public static string SwapDate(string d)
        {
            string r = ""; string r1 = ""; string r2 = ""; string r3 = ""; int i1 = 0; int i2 = 0;
            if (!(d == null || d == ""))
            {
                string []a = d.Split(new string[] { "/" }, StringSplitOptions.None);
                i1 = d.IndexOf("/");
                i2 = d.LastIndexOf("/");
                if (!(i1 == -1 | i2 == -1 | i1 == i2))
                {
                    r1 = a[0]; // d.Substring(0, i1-1);
                    r2 = a[1]; // d.Substring(i1+1, i2 - i1 - 1);
                    r3 = a[2]; // d.Substring(i2+1);
                    r = r2 + "/" + r1 + "/" + r3;
                }
            }
            return r;
        }
        public static string DatePrompt(HRSContext context)
        {
            return Environment.NewLine + "function vietDateTimeValid(d) {" +
                    Environment.NewLine + "    var vietDateTimeExpr = new RegExp('^[0-3]?[0-9]/[0-1]?[0-9]/[0-9]{4} [0-2]?[0-9]:[0-5]?[0-9](:00)?$');" +
                    Environment.NewLine + "    if(d.value != '') if (!d.value.match(vietDateTimeExpr)) {" +
                    Environment.NewLine + "        d.focus();" +
                    Environment.NewLine + "        JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("DateValid") + "', '', '0');" +
                    Environment.NewLine + "        return false;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    return true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function vietDateValid(d) {" +
                    Environment.NewLine + "    var vietDateExpr = new RegExp('^[0-3]?[0-9]/[0-1]?[0-9]/[0-9]{4}$');" +
                    Environment.NewLine + "    if(d.value != '') if (!d.value.match(vietDateExpr)) {" +
                    Environment.NewLine + "        d.focus();" +
                    Environment.NewLine + "        JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("DateValid") + "', '', '0');" +
                    Environment.NewLine + "        return false;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    return true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function phoneValid(d) {" +
                    Environment.NewLine + "    var phoneExpr = new RegExp(/^[(]{0,1}[0-9]{3}[)]{0,1}[-\\s\\.]{0,1}[0-9]{3}[-\\s\\.]{0,1}[0-9]{4}$/);" +
                    Environment.NewLine + "    if(d.value != '') if (!d.value.match(phoneExpr)) {" +
                    Environment.NewLine + "        d.focus();" +
                    Environment.NewLine + "        JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("PhoneValid") + "', '', '0');" +
                    Environment.NewLine + "        return false;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    return true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function emailValid(d) {" +
                    Environment.NewLine + "    var emailExpr = new RegExp(\"^[A-Za-z0-9](([_\\.\\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\\.\\-]?[a-zA-Z0-9]+)*)\\.([A-Za-z]{2,})$\");" + //^\\w+([\\.-]?\\w+)*@\\w+([\\.-]?\\w+)*(\\.\\w{2,3})+$
                    Environment.NewLine + "    if(d.value != '') if (!d.value.match(emailExpr)) {" +
                    Environment.NewLine + "        d.focus();" +
                    Environment.NewLine + "        JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("EmailValid") + "', '', '0');" +
                    Environment.NewLine + "        return false;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    return true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function timeValid(d) {" +
                    Environment.NewLine + "    var vietTimeExpr = new RegExp('^[0-2]?[0-9]:[0-5]?[0-9](:00)?$');" +
                    Environment.NewLine + "    if(d.value != '') if (!d.value.match(vietTimeExpr)) {" +
                    Environment.NewLine + "        d.focus();" +
                    Environment.NewLine + "        JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("TimeValid") + "', '', '0');" +
                    Environment.NewLine + "        return false;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    return true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function numericValid(d) {" +
                    Environment.NewLine + "    var numericExpr = new RegExp(\"^([-]?)[0-9]+(" + NumberSepr + "[0-9]+(" + NumberSepr + "[0-9]+(" + NumberSepr + "[0-9]+)?)?)?([.][0-9]+)?$\"); // max 3 spaces - billion" +
                    Environment.NewLine + "    if(d.value != '') if (!d.value.match(numericExpr)) {" +
                    Environment.NewLine + "        d.focus();" +
                    Environment.NewLine + "        JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("NumericValid") + "', '', '0');" +
                    Environment.NewLine + "        return false;" +
                    Environment.NewLine + "    };" +
                    Environment.NewLine + "    return true;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function datePrompt(v) {" +
                    Environment.NewLine + "    var v1 = v.value;" +
                    Environment.NewLine + "    if (v1 == '') return '';" +
                    Environment.NewLine + "    var o1 = '';" +
                    Environment.NewLine + "    var monthdays = 0;" +
                    Environment.NewLine + "    var dateexpr1 = new RegExp('^[0-3][0-9][0-1][0-9]$');" +
                    Environment.NewLine + "    var dateexpr2 = new RegExp('^[0-3][0-9][0-1][0-9][0-9][0-9]$');" +
                    Environment.NewLine + "    var dateexpr3 = new RegExp('^[0-3]?[0-9]/[0-1]?[0-9]/[0-9][0-9]$');" +
                    Environment.NewLine + "    var dateexpr4 = new RegExp('^[0-3]?[0-9]/[0-1]?[0-9]$');" +
                    Environment.NewLine + "    var dateexpr5 = new RegExp('^[0-3][0-9][0-1][0-9][0-9][0-9][0-9][0-9]$');" +
                    Environment.NewLine + "    if (isVietDate(v1))" +
                    Environment.NewLine + "        o1 = v1;" +
                    Environment.NewLine + "    else {" +
                    Environment.NewLine + "        if (v1.match(dateexpr1)) {" +
                    Environment.NewLine + "            var d = new Date();" +
                    Environment.NewLine + "            o1 = v1.substring(0, 2) + '/' + v1.substring(2, 4) + '/' + d.getFullYear();" +
                    Environment.NewLine + "        }" +
                    Environment.NewLine + "        else {" +
                    Environment.NewLine + "            if (v1.match(dateexpr2))" +
                    Environment.NewLine + "                o1 = v1.substring(0, 2) + '/' + v1.substring(2, 4) + '/20' + v1.substring(4, 6);" +
                    Environment.NewLine + "            else {" +
                    Environment.NewLine + "                if (v1.match(dateexpr3)) {" +
                    Environment.NewLine + "                    var i1 = v1.lastIndexOf('/');" +
                    Environment.NewLine + "                    o1 = v1.substring(0, i1) + '/20' + v1.substring(i1 + 1, v1.length);" +
                    Environment.NewLine + "                }" +
                    Environment.NewLine + "                else {" +
                    Environment.NewLine + "                    if (v1.match(dateexpr4)) {" +
                    Environment.NewLine + "                        var d = new Date();" +
                    Environment.NewLine + "                        o1 = v1 + '/' + d.getFullYear();" +
                    Environment.NewLine + "                    } else if (v1.match(dateexpr5)) {" +
                    Environment.NewLine + "                        o1 = v1.substring(0, 2) + '/' + v1.substring(2, 4) + '/' + v1.substring(4, 8);" +
                    Environment.NewLine + "                    }" +
                    Environment.NewLine + "                }" +
                    Environment.NewLine + "            }" +
                    Environment.NewLine + "        }" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    if (o1 == '' && v1 != '') {" +
                    Environment.NewLine + "        if (!isVietDateTime(v1)) JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("DateValid") + "', '', '0');" +//alert('" + context.GetLanguageLable("DateValid") + "');
                    Environment.NewLine + "        return v1;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    // Now check date validity" +
                    Environment.NewLine + "    strDate1 = o1.split(\"/\");" +
                    Environment.NewLine + "    if (eval(strDate1[1]) < 1 || eval(strDate1[1]) > 12) {" +
                    Environment.NewLine + "        JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("MonthValid") + "', '', '0');" +//alert('" + context.GetLanguageLable("MonthValid") + "');
                    Environment.NewLine + "        return v1;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    monthdays = monthLastDay(eval(strDate1[1]), eval(strDate1[2]));" +
                    Environment.NewLine + "    if (eval(strDate1[0]) < 1 || parseInt(strDate1[0]) > monthdays) {" +
                    Environment.NewLine + "        JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '(' + eval(strDate1[0]) + ') " + context.GetLanguageLable("DayValid") + ": 1-' + monthdays, '', '0');" +//alert('(' + eval(strDate1[0]) + ') " + context.GetLanguageLable("DayValid") + ": 1-' + monthdays);
                    Environment.NewLine + "        return v1;" +
                    Environment.NewLine + "    }" +
                    Environment.NewLine + "    return o1;" +
                    Environment.NewLine + "}";
        }
        public static int MDays(string d)
        {
            int i1; int i2; int i3; int r = 0; int m; int y;
            if (!(d == null || d == ""))
            {
                i1 = d.IndexOf("/");
                i2 = d.LastIndexOf("/");
                i3 = d.LastIndexOf(" ");
                if (!(i1 == -1 | i2 == -1 | i1 == i2))
                {
                    m = int.Parse(d.Substring(i1+1, i2 - i1 - 1));
                    if (i3 == -1)
                    {
                        y = int.Parse(d.Substring(i2+1));
                    }
                    else
                    {
                        y = int.Parse(d.Substring(i2+1, i3 - i2 - 1));
                    }
                    switch (m)
                    {
                        case 1: r = 31; break;
                        case 2: if (y % 4 == 0) r = 29; else r = 28; break;
                        case 3: r = 31; break;
                        case 4: r = 30; break;
                        case 5: r = 31; break;
                        case 6: r = 30; break;
                        case 7: r = 31; break;
                        case 8: r = 31; break;
                        case 9: r = 30; break;
                        case 10: r = 31; break;
                        case 11: r = 30; break;
                        case 12: r = 31; break;
                    }
                }
            }

            return r;
        }
        public static string ExtractDatePart(string d)
        {
            string r = d; int i = 0;
            i = d.IndexOf(" ");
            if (i > 0)
            {
                r = d.Substring(0, i - 1);
            }
            return r;
        }
        public static string ExtractTimePart(string d)
        {
            string r = "00:00"; int i = 0;
            i = d.IndexOf(" ");
            if (i > 0)
            {
                r = d.Substring(i);
            }
            return r;
        }       
        public static string HRMDateTime()
        {
            return string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);
        }
        public static string HRMDate()
        {
            return string.Format("{0:dd/MM/yyyy}", DateTime.Now);
        }
        public static string HRMDateTime(DateTime d)
        {
            return string.Format("{0:dd/MM/yyyy HH:mm}", d);
        }
        public static string HRMDate(DateTime d)
        {
            return string.Format("{0:dd/MM/yyyy}", d);
        }
        public static string HRMTime()
        {
            return string.Format("{0:HH:mm}", DateTime.Now);
        }
        public static string HRMTime(DateTime d)
        {
            return string.Format("{0:HH:mm}", d);
        }
        public static string HRMDate(string val)
        {
            DateTime dNow = DateTime.Now; int iDay = 0;
            if (val == "@")
            {
                val = string.Format("{0:dd/MM/yyyy}", dNow);
            }
            else if (Left(val, 1) == "@")//(val.Length > 2 && val.Substring(0, 1) == "@")
            {
                if (Left(val, 2) == "@+") iDay = int.Parse(Right(val, 2, false));
                if (Left(val, 2) == "@-") iDay = 0 - int.Parse(Right(val, 2, false));
                val = string.Format("{0:dd/MM/yyyy}", dNow.AddDays(iDay));
            }
            else
            {
                DateTime val1;
                try
                {
                    val1 = DateTime.Parse(val);
                    val = HRMDate(val1);
                }
                catch
                {
                    ///val1 = DateTime.Now;
                    val = "";// Tools.HRMDate(val1);
                }
            }
            return val;
        }
        public static string HRMDateTime(string val)
        {
            DateTime dNow1 = DateTime.Now; int iDay1 = 0;
            if (val == "@")
            {
                val = string.Format("{0:dd/MM/yyyy HH:mm}", dNow1);
            }
            else if (Left(val, 1) == "@")//(val.Length > 2 && val.Substring(0, 1) == "@")
            {
                if (Left(val, 2) == "@+") iDay1 = int.Parse(Right(val, 2, false));
                if (Left(val, 2) == "@-") iDay1 = 0 - int.Parse(Right(val, 2, false));
                val = string.Format("{0:dd/MM/yyyy HH:mm}", dNow1.AddDays(iDay1));
            }
            else
            {
                DateTime val1;
                try
                {
                    val1 = DateTime.Parse(val);
                    val = HRMDateTime(val1);
                }
                catch
                {
                    //val1 = DateTime.Now;
                    val = "";// Tools.HRMDateTime(val1);
                }
            }
            return val;
        }
        #endregion

        #region String
        public static string GetValId(string[] b, int j) // Lay chuoi theo id tu dau mang
        {
            string s = b[0];
            for (int i = 1; i <= j; i++)
            {
                s = s + "-" + b[i];
            }
            return s;
        }
        public static string Left(string s, int n)
        {
            if (s.Length > n)
            {
                s = s.Substring(0, n);
            }
            return s;
        }
        public static string Right(string s, int n, bool IsN = true)
        {
            if (IsN)
            {
                s = Right1(s, n);
            }
            else
            {
                s = s.Substring(n);
            }
            return s;
        }
        private static string Right1(string s, int n)
        {
            if (s.Length > n)
            {
                s = s.Substring(s.Length - n, n);
            }
            return s;
        }
        public static string JavaBack(string msg)
        {
            if (msg == null) msg = "";
            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            if (msg != "") r1.Append(Environment.NewLine + "alert('" + msg + "');");
            r1.Append(Environment.NewLine + "history.back();");
            r1.Append(Environment.NewLine + "</script>");
            r = r1.ToString(); r1 = null;
            return r;
        }
        public static string JavaRedirect(string msg, string url)
        {
            if (msg == null) msg = "";
            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            if (msg != "") r1.Append(Environment.NewLine + "alert('" + msg + "');");
            r1.Append(Environment.NewLine + "window.location.href='" + url + "';");
            r1.Append(Environment.NewLine + "</script>");
            r = r1.ToString(); r1 = null;
            return r;
        }
        public static string CloseWindow(string msg, bool IsParentReload = false)
        {
            if (msg == null) msg = "";
            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            if (msg != "") r1.Append(Environment.NewLine + "alert('" + msg  + "');");
            if (IsParentReload)
            {
                r1.Append(Environment.NewLine + "var parent=window.opener;if (parent==null) parent=dialogArguments;parent.focus();parent.location.reload();");
            }
            r1.Append(Environment.NewLine + "window.close();");
            r1.Append(Environment.NewLine + "</script>");
            r = r1.ToString(); r1 = null;
            return r;
        }
        public static string ShowMessage(string msg)
        {
            if (msg == null) msg = "";
            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            if (msg != "") r1.Append(Environment.NewLine + "alert('" + msg + "');");
            r1.Append(Environment.NewLine + "</script>");
            r = r1.ToString(); r1 = null;
            return r;
        }
        public static string JoinArray(string [] a, string c = ",")
        {
            return String.Join(c, a);
        }
        public static string RemoveFisrtChar(string s, int n)
        {
            string s1 = "";
            if (s != null && s != "") s1 = s.Substring(n);
            return s1;
        }
        public static string RemoveFisrtChar(string s)
        {
            string s1 = "";
            if (s != null && s != "") s1 = s.Substring(1);
            return s1;
        }
        public static string RemoveLastChar(string s, int n)
        {
            string s1 = "";
            if (s != null && s != "") s1 = s.Substring(0, s.Length - n);
            return s1;
        }
        public static string RemoveLastChar(string s)
        {
            string s1 = "";
            if (s != null && s != "") s1 = s.Substring(0, s.Length - 1);
            return s1;
        }
        public static bool IsUnicode(string s)
        {
            bool r = false;
            char[] arr; arr = s.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                if (Convert.ToInt32(arr[i]) > 254)
                {
                    r = true;
                    break;
                }
            }
            return r;
        }
        public static string StrNull(dynamic s)
        {
            string s1 = "";
            if (s != null) s1 = s.ToString();
            return s1;
        }
        public static int GetArrayPos(string val, string[] a)
        {
            int i = 0; bool kt = false;
            while (!kt && i < a.Length)
            {
                if (val == a[i]) kt = true;
                i++;
            }
            if (kt) i--; else i = -1;
            return i;
        }

        //gen script js function reset input form
        public static string GenResetFunc(string f = "btnReset")
        {
            string s = Environment.NewLine + "function InputFocus (input){" +
                       Environment.NewLine + "try {" +
                       Environment.NewLine + "  if(input.type == 'select-one' || input.type == 'select-multiple'){" +
                       Environment.NewLine + "      setTimeout(function(){$(input).select2('open');$(input).select2('close');}, 100);" +
                       Environment.NewLine + "  } else {" +
                       Environment.NewLine + "      input.focus();" +
                       Environment.NewLine + "  }" +
                       Environment.NewLine + "} catch (ex) {" +
                       Environment.NewLine + "  try {input[0].focus();}catch(ex){}" +
                       Environment.NewLine + "}" +
                       Environment.NewLine + "}";
            switch (f.ToLower())
            {
                case "btnreset":
                    s = Environment.NewLine + "function btnReset (f){" +
                        Environment.NewLine + "    var i=0;" +
                        Environment.NewLine + "    while (i <= f.elements.length) {" +
                        Environment.NewLine + "        i++; " +
                        Environment.NewLine + "        var input = f.elements[i]; " +
                        Environment.NewLine + "        if (input) {" +
                        Environment.NewLine + "            if (input.type!=\"select-one\" && input.type!=\"select-multiple\"){" +
                        Environment.NewLine + "            if (input.length){" +
                        Environment.NewLine + "                for(var j = 0; j < input.length; j++){" +
                        Environment.NewLine + "                    switch (input[j].type){" +
                        Environment.NewLine + "                        case \"text\":" +
                        Environment.NewLine + "                        case \"textarea\":" +
                        Environment.NewLine + "                            input[j].value = '';" +
                        Environment.NewLine + "                            break;" +
                        Environment.NewLine + "                        case \"select-one\":" +
                        Environment.NewLine + "                            if(input[j].name != 'PageSize'){input[j].selectedIndex = -1;$('#' + input[j].id).change();}" +
                        Environment.NewLine + "                            break;" +
                        Environment.NewLine + "                        case \"checkbox\":" +
                        Environment.NewLine + "                            input[j].checked = false;" +
                        Environment.NewLine + "                            break;" +
                        Environment.NewLine + "                    }" +
                        Environment.NewLine + "                }" +
                        Environment.NewLine + "            }else{" +
                        Environment.NewLine + "                    switch (input.type){" +
                        Environment.NewLine + "                        case \"text\":" +
                        Environment.NewLine + "                        case \"textarea\":" +
                        Environment.NewLine + "                            input.value = '';" +
                        Environment.NewLine + "                            break;" +
                        Environment.NewLine + "                        case \"select-one\":" +
                        Environment.NewLine + "                            if(input.name != 'PageSize'){input.selectedIndex = -1;$('#' + input.id).change();}" +
                        Environment.NewLine + "                            break;" +
                        Environment.NewLine + "                        case \"checkbox\":" +
                        Environment.NewLine + "                            input.checked = false;" +
                        Environment.NewLine + "                            break;" +
                        Environment.NewLine + "                    }" +
                        Environment.NewLine + "            }" +
                        Environment.NewLine + "            } else {" +
                        Environment.NewLine + "            if(input.name != 'PageSize'){input.selectedIndex = -1;$('#' + input.id).change();}" +
                        Environment.NewLine + "            }" +
                        Environment.NewLine + "        }" +
                        Environment.NewLine + "    }try{f.PageSize.value = '100'; f.Page.value='1';}catch(ex){}" +
                        Environment.NewLine + "}";
                    break;
                case "nextfocus":
                    s = s + Environment.NewLine + "function nextfocus(fe,c) {" +
                        Environment.NewLine + "    var i=0;" +
                        Environment.NewLine + "    while (i<jsdbcols.length && c != jsdbcols[i]) i++;" +
                        Environment.NewLine + "    i++;" +
                        Environment.NewLine + "    if (i>=jsdbcols.length) return;" +
                        Environment.NewLine + "    try{while (i<jsdbcols.length && fe.elements[jsdbcols[i]].type=='hidden'||fe.elements[jsdbcols[i]].readOnly==true||fe.elements[jsdbcols[i]].disabled==true) i++;}catch(ex){}" +
                        Environment.NewLine + "    if (i>=jsdbcols.length) return;" +
                        Environment.NewLine + "    InputFocus (fe.elements[jsdbcols[i]]);" +
                        Environment.NewLine + "}";
                    break;
                case "cnextfocus":
                    s = s + Environment.NewLine + "function cnextfocus(e, fe, col) {" +
                        Environment.NewLine + " if(isEnterOnly(e)) nextfocus(fe, col);" +
                        Environment.NewLine + "}";
                    break;
                case "focusfirst":
                    s = s + Environment.NewLine + "function focusfirst(fe) {" +
                        Environment.NewLine + " var i=0; " +
                        Environment.NewLine + " var f = fe.elements; " +
                        Environment.NewLine + " var fl = f.length;" +
                        Environment.NewLine + " while (i<fl && (f[i].type=='hidden'||f[i].type=='button'||f[i].readOnly==true||f[i].disabled==true)) i++;" +
                        Environment.NewLine + " if (i<fl) {" +
                        Environment.NewLine + "     InputFocus (f[i]);" +
                        Environment.NewLine + " }" +
                        Environment.NewLine + "}";
                    break;
                case "btnsearch":
                    s = Environment.NewLine + "function btnSearch (){document.SearchFrm.submit();}";
                    break;
            }
            return s;
        }
        #endregion

        #region Method WebUtility
        private static bool IsEncoded(string text, bool IsHtmlEncodeCheck = false)
        {
            // below fixes false positive &lt;<> 
            // you could add a complete blacklist, 
            // but these are the ones that cause HTML injection issues
            if (text == "") return false;
            if (text.Contains("<")) return false;
            if (text.Contains(">")) return false;
            if (text.Contains("\"")) return false;
            if (text.Contains("'")) return false; //%27
            if (text.Contains("script")) return false;// chặn script

            byte[] r1;
            try { r1 = Convert.FromBase64String(text); return false; } catch { }

            //if (text.Contains(@"%22")) return false;
            //if (text.Contains(@"%27")) return false;
            //if (text.Contains(@"%2F")) return false;
            //if (text.Contains(@"%5C")) return false;
            //if (text.Contains(@"%3E")) return false;
            //if (text.Contains(@"%3C")) return false;
            
            
            // if decoded string == original string, it is already encoded
            if (IsHtmlEncodeCheck)
                return (WebUtility.HtmlDecode(text) != text);
            else
                return (WebUtility.UrlDecode(text) != text);
        }
        public static string UrlEncode (string s) // Tools.UrlEncode
        {
            //if (!IsEncoded(s))
                return WebUtility.UrlEncode(s);
            //else
            //    return s;
        }
        public static string UrlDecode(string s) // Tools.UrlDecode
        {
            string s1 = s;
            //while (IsEncoded(s1))
            //{
                s1 = WebUtility.UrlDecode(s1);
            //}
            return s1; //WebUtility.UrlDecode(s);
        }
        public static string HtmlEncode(string s) // Tools.HtmlEncode
        {
            if (!IsEncoded(s, true))
                return WebUtility.HtmlEncode(s);
            else
                return s;
        }
        public static string HtmlDecode(string s) // Tools.HtmlDecode
        {
            string s1 = s; int iWhile = 0; // chặn lỗi Out Of Memory
            while (IsEncoded(s1, true) && iWhile < MaxWhileCount)
            {
                s1 = WebUtility.HtmlDecode(s1); iWhile++;
            }
            return s1; //WebUtility.HtmlDecode(s);
        }
        public static bool CheckFileExtension(string FileExtension, string[] allowedExtensions)
        {
            int i = 0; int j = allowedExtensions.Length; bool FileOK = false;
            while (i < j && !FileOK)
            {
                if (FileExtension == allowedExtensions[i])
                {
                    FileOK = true;
                }
                i = i + 1;
            }
            return FileOK;
        }
        public static string GetValueFormKeys(string[] b, string[] c, string sKey) // Tools.HtmlDecode
        {
            string r = ""; 
            int i = 0; bool kt = false;
            while (i < b.Length && !kt)
            {
                if (b[i] == sKey) kt = true;
                i++;
            }
            if (kt)
            {
                i = i - 1;
                r = c[i];
            }
            else r = sKey;
            return r;
        }
        public static string GetValueFormKeys(string s, string sKey) // Tools.HtmlDecode
        {
            s = s.ToLower(); sKey = sKey.ToLower();
            string []a = s.Split(new string[] { "^" }, StringSplitOptions.None);
            string []b = a[0].Split(new string[] { "||" }, StringSplitOptions.None);
            string []c = a[1].Split(new string[] { "||" }, StringSplitOptions.None);

            return GetValueFormKeys(b, c, sKey);
        }
        #endregion

        #region Method Data
        public static dynamic GetMedia(string ImageId, ToolDAO bosDAO)
        {
            string parameterOutput = "";string json = ""; int errorCode = 0; string errorString = "";
            dynamic d = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\":\"AttachmentID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + ImageId + "\"}," +
                "{\"ParamName\":\"Keyword\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"\"}," +
                "{\"ParamName\":\"Author\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"-1\"}," +
                "{\"ParamName\":\"Status\", \"ParamType\":\"16\", \"ParamInOut\":\"1\", \"ParamLength\":\"2\", \"InputValue\":\"-1\"}," +
                "{\"ParamName\":\"Page\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                "{\"ParamName\":\"PageSize\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"10000\"}," +
                "{\"ParamName\":\"Rowcount\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
            bosDAO.ExecuteStoreService("GetMedia", "SP_CMS__Attachment_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = null;
            if (errorCode == HTTP_CODE.HTTP_ACCEPT)
            {
                d = JObject.Parse(json);
            }
            return d;
        }
        public static dynamic GetMedia(HRSContext context, string ImageId, ToolDAO bosDAO, ref int errorCode, ref string errorString)
        {
            string UserID = context.GetSession("UserID"); dynamic d;
            try { ImageId = (int.Parse(ImageId)).ToString(); } catch { ImageId = "0"; }
            string parameterOutput = "";
            bool IsCached = context._cache.Get("GetImage_" + UserID + "_" + ImageId, out d);
            string json = "";// context.GetSession("GetImage_" + UserID + "_" + ImageId);
            errorCode = 200; errorString = "GetCache";
            if (!IsCached)
            {
                d = JObject.Parse("{\"parameterInput\":[" +
                   "{\"ParamName\":\"AttachmentID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + ImageId + "\"}," +
                   "{\"ParamName\":\"Keyword\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"\"}," +
                   "{\"ParamName\":\"Author\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"-1\"}," +
                   "{\"ParamName\":\"Status\", \"ParamType\":\"16\", \"ParamInOut\":\"1\", \"ParamLength\":\"2\", \"InputValue\":\"-1\"}," +
                   "{\"ParamName\":\"Page\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                   "{\"ParamName\":\"PageSize\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"10000\"}," +
                   "{\"ParamName\":\"Rowcount\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
                bosDAO.ExecuteStore("GetImage", "SP_CMS__Attachment_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                d = null;
                if (errorCode == 200) { d = JObject.Parse(json); context._cache.Set("GetImage_" + UserID + "_" + ImageId, d, context._cache.CacheByHour); }
                //context.SetSession("GetImage_" + UserID + "_" + ImageId, json);
            }            
            //else d = JObject.Parse(json);
            return d;
        }
        public static string GetDataFormatJson(dynamic d, dynamic d1, string sKey)
        {
            string r; sKey = sKey.ToUpper();
            try
            {
                switch (d1[sKey].ToString())
                {
                    case "smallint": // string integer
                    case "int": // string integer
                        r = FormatNumber(d[sKey].ToString(), 0);
                        break;
                    case "bigint": // string long
                        //r = long.Parse(d[sKey].ToString());
                        r = FormatNumber(d[sKey].ToString(), 0);
                        break;
                    case "real": // string double
                    case "float": // string double
                    case "double": // string double
                    case "decimal": // string double
                    case "numeric": // string double
                        //r = double.Parse(d[sKey].ToString());
                        r = FormatNumber(d[sKey].ToString(), 2);
                        break;
                    case "date": // string date
                        r = HRMDate(DateTime.Parse(d[sKey].ToString()));
                        break;
                    case "datetime": // string date
                        r = HRMDateTime(DateTime.Parse(d[sKey].ToString()));
                        break;
                    default:
                        r = d[sKey].ToString();
                        break;
                }
            }
            catch
            {
                r = sKey;
            }
            return r;
        }
        public static object GetDataJson(dynamic d, string sKey, string DataType = "string")
        {
            object r; sKey = sKey.ToUpper();
            try
            {
                if (DataType == "object")
                {
                    r = d[sKey];
                }
                else
                {
                    string val = d[sKey].ToString().Trim();
                    switch (DataType)
                    {
                        case "smallint": // string integer
                        case "int": // string integer
                            r = int.Parse(val);
                            break;
                        case "bigint": // string long
                            r = long.Parse(val);
                            break;
                        case "double": // string double
                        case "decimal": // string double
                        case "numeric": // string double
                            r = double.Parse(val);
                            break;
                        case "date": // string date
                        case "datetime": // string date
                            r = DateTime.Parse(val);
                            break;
                        default:
                            r = val;
                            break;
                    }
                }                
            }
            catch
            {
                switch (DataType)
                {
                    case "smallint": // string integer
                    case "int": // string integer
                        r = 0;
                        break;
                    case "bigint": // string long
                        r = 0;
                        break;
                    case "double": // string double
                    case "decimal": // string double
                    case "numeric": // string double
                        r = 0;
                        break;
                    case "object":
                    case "date": // string date
                    case "datetime": // string date
                        r = null;
                        break;
                    default:
                        r = ""; //sKey
                        break;
                }
            }
            return r;
        }
        public static string ParseValue(HRSContext context, string val, bool IsSession = false)
        {
            if (val == "@YEAR")
            {
                DateTime a = DateTime.Now;
                val = a.Year.ToString();
            }
            else if (val == "@MONTH")
            {
                DateTime a = DateTime.Now;
                val = a.Month.ToString();
            }
            else if (val == "@MONTHYEAR")
            {
                DateTime a = DateTime.Now;
                val = a.Year.ToString() + (a.Month<10?"0"+ a.Month.ToString(): a.Month.ToString());
            }
            else if (val.Length > 7)
            {
                if (val.Substring(0, 7) == "REQUEST")
                {
                    string reqV = val.Substring(7);
                    string reqP = context.GetRequestVal(reqV);
                    val = reqP;
                }
            }
            
            if (IsSession) val = context.ReplaceSessionValue(val);
            return val;
        }
        public static string ParseValue(HRSContext context, string InputType = "", string val = "", string valDefault = "SESSIONUserID")
        {
            string s = "";
            if (val == "" && valDefault != "") val = valDefault;
            switch (InputType)
            {
                case "Hidden":
                    s = context.ReplaceSessionValue(valDefault);
                    val = context.ReplaceSessionValue(val);
                    if (s != valDefault || val == "") val = s;
                    break;
                case "Numeric":
                    if (val == "")
                        val = "0";
                    else
                        val = RemNumSepr(val);
                    break;
                case "Datetime":
                    DateTime dNow1 = DateTime.Now; int iDay1 = 0;
                    if (val == "@" || val == "")
                    {
                        val = string.Format("{0:M/d/yyyy HH:mm}", dNow1);
                    }
                    else if (val.Length > 2 && val.Substring(0, 1) == "@")
                    {
                        if (val.Substring(0, 2) == "@+")
                        {
                            iDay1 = int.Parse(Tools.Right(val, 2, false));
                            val = string.Format("{0:M/d/yyyy HH:mm}", dNow1.AddDays(iDay1));
                        }
                        else if (val.Substring(0, 2) == "@-")
                        {
                            iDay1 = 0 - int.Parse(Tools.Right(val, 2, false));
                            val = string.Format("{0:M/d/yyyy HH:mm}", dNow1.AddDays(iDay1));
                        }
                    }
                    else val = SwapDate(val);
                    val = val + ":00";
                    break;
                case "Date":
                    DateTime dNow = DateTime.Now; int iDay = 0;
                    if (val == "@" || val == "")
                    {
                        val = string.Format("{0:M/d/yyyy}", dNow);
                    }
                    else if (val.Length > 2 && val.Substring(0, 1) == "@")
                    {
                        if (val.Substring(0, 2) == "@+")
                        {
                            iDay = int.Parse(Tools.Right(val, 2, false));
                            val = string.Format("{0:M/d/yyyy}", dNow.AddDays(iDay));
                        }
                        else if (val.Substring(0, 2) == "@-")
                        {
                            iDay = 0 - int.Parse(Tools.Right(val, 2, false));
                            val = string.Format("{0:M/d/yyyy}", dNow.AddDays(iDay));
                        }
                    }
                    else val = SwapDate(val);
                    break;
            }
            return val;
        }
        public static string ReadParam(string ParamList, dynamic d, bool IsUpcase = true)
        {
            string json = "";
            string[] b = ParamList.Split(new string[] { "^" }, StringSplitOptions.None);
            if (ParamList != "")
            {
                for (int i = 0; i < b.Length; i++)
                {
                    string[] b1 = b[i].Split(new string[] { ";" }, StringSplitOptions.None);
                    string val;
                    try { if (IsUpcase) val = GetDataJson(d, b1[0]);  else val = d[b1[0]].ToString(); } catch { val = b1[4]; }
                    json = json + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":\"" + val + "\"}";
                }
            }
            return json;
        }
        public static string ReadParam(HRSContext context, string QueryStringFilter, string ParamList, string ParamDefault, string QueryStringType, 
            string ColName = "", string ColVal = "", bool IsXlsx = false)
        {
            string json = "";
            string[] a = QueryStringFilter.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] b = ParamList.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] c = ParamDefault.Split(new string[] { "^" }, StringSplitOptions.None);
            string[] ac = QueryStringType.Split(new string[] { "^" }, StringSplitOptions.None);
            if (ParamList != "")
            {
                for (int i = 0; i < b.Length; i++)
                {
                    string[] b1 = b[i].Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] aType = ";".Split(new string[] { ";" }, StringSplitOptions.None);
                    //if (i < ac.Length) aType = ac[i].Split(new string[] { ";" }, StringSplitOptions.None);
                    string val = ""; // Khoi tao
                    int j = GetArrayPos(b1[0], a);
                    if (j > -1 && j < ac.Length) aType = ac[j].Split(new string[] { ";" }, StringSplitOptions.None);
                    if (j < a.Length && j > -1)
                    {
                        val = context.GetRequestVal(b1[0]); // lay tu request
                        if (j < c.Length && val == "") val = ParseValue(context, c[j], true); // lay default
                        if (val == "") val = ParseValue(context, b1[4], true); // lay default
                        val = ParseValue(context, aType[0], val, aType[1]);
                    }
                    else
                    {
                        val = b1[4];
                    }
                    if (b1[0] == ColName && ColVal != "") val = ColVal;
                    if (val == "0")
                    {
                        if (b1[0] == "Page") val = "1"; else if (b1[0] == "PageSize") val = context.GetSession("PageSizeReport");
                    }
                    if (b1[0] == "PageSize" && IsXlsx) val = context.GetSession("PageSizeReport");
                    if (b1[0] == "Creator") val = context.GetSession("UserID");
                    json = json + ",{\"ParamName\":\"" + b1[0] + "\", \"ParamType\":\"" + b1[1] + "\", \"ParamInOut\":\"" + b1[2] + "\", \"ParamLength\":\"" + b1[3] + "\", \"InputValue\":\"" + val + "\"}";
                }
            }
            return json;
        }
        public static void ExecParam(string FunctionName, string QueryStringFilter, string ParamList, string ParamDefault, string QueryStringType,
            HRSContext context, string SPName, 
            ToolDAO DataDAO, ref string parameterOutput, ref string json, ref int errorCode, ref string errorString, 
            bool IsXlsx = false, string ColName = "", string ColVal = "", bool IsStoreCache = true) // Excel; PartColumn
        {
            json = ReadParam(context, QueryStringFilter, ParamList, ParamDefault, QueryStringType, ColName, ColVal, IsXlsx);
            dynamic d = null;
            if (json != "")
            {
                json = "{\"parameterInput\":[" + RemoveFisrtChar(json) + "]}";
                d = JObject.Parse(json);
            }
            DataDAO.ExecuteStore(FunctionName, SPName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
        }
        public static void SetGroupData(string PrevColName3, ref string PrevCol3, string PrevColLable3, ref double[] SumCol3, ref StringBuilder r13, 
            ref dynamic dList, int i, HRSContext context,
            ref StringBuilder r12, int ColumnSpan, int ColumnSumStart,
            string[] b, string [] c)
        {
            if (PrevColName3 != "")
            {
                if (PrevCol3 != "" && PrevCol3 != Tools.GetDataJson(dList.ListForm.Items[i], PrevColName3))
                {
                    r12.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "_3_" + PrevCol3 + "\">");
                    r12.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + " >" + context.GetLanguageLable(PrevColLable3) + ": <i>" + PrevCol3 + "</i>");
                    for (int j = ColumnSumStart; j < b.Length; j++)
                    {
                        string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                        switch (a4[0])
                        {
                            case "Numeric":
                                if (a4.Length > 2) if (a4[2] == "1") r12.Append(Environment.NewLine + "<td class=\"text-right\"><i>" + FormatNumber(SumCol3[j].ToString(), int.Parse(a4[1])) + "</i>");
                                break;
                            default:
                                r12.Append(Environment.NewLine + "<td>");
                                break;
                        }
                        SumCol3[j] = 0;
                    }
                    r12.Append(r13.ToString()); r13 = new StringBuilder();
                }
                PrevCol3 = Tools.GetDataJson(dList.ListForm.Items[i], PrevColName3);
            }
            else
            { r12.Append(r13.ToString()); r13 = new StringBuilder(); }
        }
        public static void SetGroupData(string PrevColName3, string PrevCol3, string PrevColLable3, ref double[] SumCol3, ref StringBuilder r13,
            HRSContext context, 
            ref StringBuilder r12, int ColumnSpan, int ColumnSumStart,
            string[] b, string[] c)
        {
            if (PrevColName3 != "")
            {
                if (PrevCol3 != "")
                {
                    r12.Append(Environment.NewLine + "<tr id=\"trrowid_3_" + PrevCol3 + "\">");
                    r12.Append(Environment.NewLine + "<td" + (ColumnSpan != 0 ? " colspan = \"" + ColumnSpan + "\"" : "") + ">" + context.GetLanguageLable(PrevColLable3) + ": <i>" + PrevCol3 + "</i>");
                    for (int j = ColumnSumStart; j < b.Length; j++)
                    {
                        string[] a4 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                        switch (a4[0])
                        {
                            case "Numeric":
                                int iRound = 0; try { iRound = int.Parse(a4[1]); } catch { iRound = 0; }
                                if (a4.Length > 2) if (a4[2] == "1") r12.Append(Environment.NewLine + "<td class=\"text-right\"><i>" + Tools.FormatNumber(SumCol3[j].ToString(), iRound) + "</i>");
                                break;
                            default:
                                r12.Append(Environment.NewLine + "<td>");
                                break;
                        }
                        SumCol3[j] = 0;
                    }
                    r12.Append(r13.ToString()); r13 = new StringBuilder();
                }
            }
            else
            { r12.Append(r13.ToString()); r13 = new StringBuilder(); }
        }
        #endregion
    }
}    