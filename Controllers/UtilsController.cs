using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text;
using Utils;
using Newtonsoft.Json.Linq;
using HRS.Models;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace HRScripting.Controllers
{
    public class UtilsController : Controller
    {
        public const string FolderRoot = @"\RootFolder\Json";
        public const string FolderMSSQL = @"\MSSQL";
        public const string FolderOracle = @"\Oracle";
        private HRSCache _cache;
        public UtilsController(IMemoryCache memoryCache)
        {
            _cache = new HRSCache(memoryCache);
        }
        
        #region UIDatetime
        public IActionResult DatePickerFeed()// Utils/DatePickerFeed
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            int y; int m ; int d; int j; int[] days = new int[43]; 
            StringBuilder r = new StringBuilder(); string r1 = "";
            string DateStr = _context.GetRequestVal("date");
            string Timetoo = _context.GetRequestVal("timetoo");
            if (DateStr == null) DateStr = "";
            if (Timetoo == null) Timetoo = "";
            if (!Tools.IsHRMDateExpr(DateStr)) DateStr = Tools.HRMDate();
            string[] DParts; string MonthStart; int StartWd; int LastMdays; int StartWd1; string Cellb; string Cellc; int Dayd; int Direction;
            DParts = DateStr.Split(new string[] { "/" }, StringSplitOptions.None);
            y = int.Parse(DParts[2]); m = int.Parse(DParts[1]); d = int.Parse(DParts[0]);

            r.Append("<div class=\"datepicker-days\"><table width=\"100%\" class=\"table-condensed\">");//width=\"100%\" 
            r.Append("<tr><td valign=\"top\">" + _context.GetLanguageLable("Year"));
            //int yStart = y - 6; int yEnd = y + 4; 
            int yStart = y - 2; int yEnd = y + 3;
            for (int i = yStart; i < yEnd; i++)
            {
                //if (i - yStart == 5)
                //{
                //    r.Append("<td class=\"dow text-right\" valign=\"top\">" +
                //        "<a href=\"javascript:removeDoCal();\" id=\"datepickerbtn\">" +
                //        "<img src=\"../images/CloseW.gif\" class=\"imggo\" border=\"0\"></a>");
                //    r.Append("<tr><td valign=\"top\" class=\"dow\">");
                //}
                if (i == y)
                    r.Append("<td valign=\"top\" class=\"active day\">" + i);
                else
                    r.Append("<td valign=\"top\" class=\"day\" onclick=\"feedd('" + d + "/" + m + "/" + i + "', '" + Timetoo + "');\">" + i);
                //if (i == y-2) r.Append("<br>");
            }
            //r.Append("<td valign=\"top\" class=\"dow\">");
            r.Append("<td class=\"dow text-right\" valign=\"top\">" +
                "<a href=\"javascript:removeDoCal();\" id=\"datepickerbtn\">" +
                "<img src=\"../images/CloseW.gif\" class=\"imggo\" border=\"0\"></a>");
            r.Append("<tr><td valign=\"top\" class=\"dow\">" + _context.GetLanguageLable("Month"));
            for (int i = 1; i < 13; i++)
            {
                if (i == 7) r.Append("<tr><td valign=\"top\" class=\"dow\">");
                if (i == m)
                    r.Append("<td valign=\"top\" class=\"active day\">" + i + " </td>");
                else
                    r.Append("<td valign=\"top\" class=\"day\" onclick=\"feedd('" + d + "/" + i + "/" + y + "', '" + Timetoo + "');\">" + i);
                //if (i == y - 2) r.Append("<tr><td valign=\"top\" colspan=7 nowrap>");
            }
            if (Timetoo == "1")
            {
                int HH; int MM;
                string Time0 = Tools.ExtractTimePart(DateStr);
                if (Time0 == "00:00")
                {
                    HH = DateTime.Now.Hour;
                    MM = DateTime.Now.Minute;
                } else
                {
                    string [] a = Time0.Split(new string[] { ":" }, StringSplitOptions.None);
                    HH = int.Parse(a[0]);
                    MM = int.Parse(a[1]);
                }
                r.Append("<tr><td colspan=4 class=\"text-right\">" + _context.GetLanguageLable("Hour") + " <select id=\"DoCalHH\" class=\"select2 form-control\">");
                for (int i=0; i<24; i++)
                {
                    string aOption = "00" + i.ToString(); aOption = aOption.Substring(aOption.Length-2,2);
                    if (i == HH)
                        r.Append("<option value = \"" + aOption + "\" selected> " + aOption);
                    else
                        r.Append("<option value = \"" + aOption + "\"> " + aOption);
                }
                r.Append("</select>");
                r.Append("<tr><td colspan=3 class=\"text-right\">" + _context.GetLanguageLable("Minute") + " <select id=\"DoCalMM\" class=\"select2 form-control\">");
                for (int i = 0; i <60; i++)
                {
                    string aOption = "00" + i.ToString(); aOption = aOption.Substring(aOption.Length - 2, 2);
                    if (i == HH)
                        r.Append("<option value = \"" + aOption + "\" selected> " + aOption);
                    else
                        r.Append("<option value = \"" + aOption + "\"> " + aOption);
                }
                r.Append("</select>");
            }
            //r.Append("</table>");
            r.Append("<!--table class=\"table-custom\" width=\"100%\"-->" +
                "<tr><td class=\"dow\">" + _context.GetLanguageLable("MondayShort") + "<td class=\"dow\">" +
                _context.GetLanguageLable("TuesdayShort") + "<td class=\"dow\">" + _context.GetLanguageLable("WednesdayShort") + "<td class=\"dow\">" +
                _context.GetLanguageLable("ThursdayShort") + "<td class=\"dow\">" + _context.GetLanguageLable("FridayShort") + "<td style=\"color:blue\" class=\"dow\">" +
                _context.GetLanguageLable("SaturdayShort") + "<td style=\"color:red\" class=\"dow\">" +
                _context.GetLanguageLable("SundayShort") + "");

            MonthStart = m + "/1/" + y;
            StartWd = (int)DateTime.Parse(MonthStart).DayOfWeek;
            LastMdays = Tools.MDays(String.Format("{0:dd/MM/yyyy}", DateTime.Parse(MonthStart).AddDays(-1)));
            StartWd1 = StartWd;
            while (StartWd1 > 1)
            {
                StartWd1 -= 1;
                days[StartWd1] = -LastMdays;
                LastMdays -= 1;
            }
            j = 1;
            for (int i= StartWd; i<= StartWd+ Tools.MDays(Tools.SwapDate(MonthStart)); i++)
            {
                days[i] = j; j += 1;
            }
            j = 1;
            for (int i = StartWd + Tools.MDays(Tools.SwapDate(MonthStart)); i <= 42; i++)
            {
                days[i] = j * 100; j += 1;
            }
            for (int i = 1; i <= 6; i++)
            {
                if (i==6 && days[36] < 100 || i < 6)
                {
                    r.Append("<tr>");
                    for(j = 1; j <= 7; j++)
                    {
                        Dayd = days[(i - 1) * 7 + j];
                        Cellb = "";
                        Cellc = "";
                        if (Dayd < 0)
                        {
                            //Cellc = " style='color:#999'";
                            Cellc = " class=\"old day\"";
                            Dayd = -Dayd; Direction = -1;
                        } else if (Dayd >= 100)
                        {
                            //Cellc = " style='color:#999'";
                            Cellc = " class=\"old day\"";
                            Direction = 1; Dayd = Dayd / 100;
                        }
                        else
                        {
                            //Cellc = " style='color:#000'";
                            Cellc = " class=\"day\"";
                            if (Dayd == d)
                            {
                                //Cellc = " bgcolor='#ffcc00'"; 
                                Cellc = " class=\"active day\"";
                            }
                            Direction = 0;
                        }
                        r.Append("<td" + Cellb + " onclick=\"pickd(" + Dayd + ", " + Direction + ", " + m + ", " + y + ", ");
                        if (Timetoo == "1")
                            r.Append("'@'");
                        else
                            r.Append("''");
                        r.Append(");\"" + Cellc + " align=\"center\">" + Dayd);
                        //r.Append("<a href=\"javascript:pickd(" + Dayd + ", " + Direction + ", " + m + ", " + y + ", ");
                        //if (Timetoo == "1")
                        //    r.Append("'@'");
                        //else
                        //    r.Append("''");
                        //r.Append(");\"" + Cellc + ">" + Dayd + "</a>");
                    }
                }
            }
            r.Append("</table></div>");
            r.Append("<!--JS-->");
            r.Append("function pickd(d, direction, m, y, t) {");
            r.Append("if (t == '@') t = ' ' + document.getElementById('DoCalHH').value + ':' + document.getElementById('DoCalMM').value;");
            r.Append("if (direction == -1) {");
            r.Append(" m -= 1;");
            r.Append("if (m == 0) {");
            r.Append("m = 12;y -= 1; } } else if (direction == 1){");
            r.Append("m += 1;");
            r.Append("if (m == 13){m = 1; y += 1; }}");
            r.Append("if (!(DoCalTarget.readOnly || DoCalTarget.disabled)) {DoCalTarget.value = (d<10?\"0\"+d:d) + '/' + (m<10?\"0\"+m:m) + '/' + y + t;} else JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("ColumnIsReadOnly") + "', '', '0');");//alert('" + _context.GetLanguageLable("ColumnIsReadOnly") + "!');
            r.Append("eval(DoCalFollowFunc);");
            r.Append("DoCalFollowFunc = null;");
            r.Append("removeDoCal();}");
            r1 = r.ToString();
            r = null;
            return Content(r1);
        }
        private string GenTimeText(int i, int j, int h, int m)
        {
            return "<td class=\"day " + (i == h && j == m? "active":"") + "\">" + Tools.Right("00" + i.ToString(), 2) + ":" + Tools.Right("00" + j.ToString(), 2);
        }
        public IActionResult ClockPicker()// Utils/ClockPicker
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            StringBuilder r = new StringBuilder(); string r1 = "";
            // default
            int h;
            int m;
            // get time by request
            string TimeStr = _context.GetRequestVal("time");
            string[] a = TimeStr.Split(new string[] { ":" }, StringSplitOptions.None);
            try
            {
                h = int.Parse(a[0]);
                m = int.Parse(a[1]);
            }
            catch
            {
                h = DateTime.Now.Hour;
                m = DateTime.Now.Minute;
            }
            // tinh lai time
            if (m < 5) m = 0;
            else if (m < 15) m = 10;
            else if (m < 25) m = 20;
            else if (m < 35) m = 30;
            else if (m < 45) m = 40;
            else if (m < 55) m = 50;
            else
            {
                m = 0; h = h + 1; if (h > 23 || h < 0) h = 0;
            }

            r.Append(Environment.NewLine + "<div align=\"right\">" +
                Environment.NewLine + "<a href=\"javascript:removeDoCal();\" id=\"datepickerbtn\"><img src=\"../images/CloseW.gif\" class=\"imggo\" border=0></a>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "<div id=\"idDivScroll\" style=\"height:270px; overflow: auto;\">" +
                Environment.NewLine + "<TABLE class=\"table-condensed\" width=\"100%\">" +
                Environment.NewLine + "<TBODY ID=\"dayList\"ALIGN=\"CENTER\" ONCLICK=\"getCalTime(event, '" + _context.GetLanguageLable("ColumnIsReadOnly") + "')\">");
            for(int i = 0; i < 24; i++)
            {
                r.Append(Environment.NewLine + "<tr>");
                for (int j = 0; j < 6; j++)
                {
                    r.Append(Environment.NewLine + "    " + GenTimeText(i, (j * 10), h, m));

                }
            }
            ////r.Append(Environment.NewLine + "<tr><td class=\"day\">00:00</td><td class=\"day\">00:10</td><td class=\"day\">00:20</td><td class=\"day\">00:30</td><td class=\"day\">00:40</td><td class=\"day\">00:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">01:00</td><td class=\"day\">01:10</td><td class=\"day\">01:20</td><td class=\"day\">01:30</td><td class=\"day\">01:40</td><td class=\"day\">01:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">02:00</td><td class=\"day\">02:10</td><td class=\"day\">02:20</td><td class=\"day\">02:30</td><td class=\"day\">02:40</td><td class=\"day\">02:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">03:00</td><td class=\"day\">03:10</td><td class=\"day\">03:20</td><td class=\"day\">03:30</td><td class=\"day\">03:40</td><td class=\"day\">03:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">04:00</td><td class=\"day\">04:10</td><td class=\"day\">04:20</td><td class=\"day\">04:30</td><td class=\"day\">04:40</td><td class=\"day\">04:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">05:00</td><td class=\"day\">05:10</td><td class=\"day\">05:20</td><td class=\"day\">05:30</td><td class=\"day\">05:40</td><td class=\"day\">05:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">06:00</td><td class=\"day\">06:10</td><td class=\"day\">06:20</td><td class=\"day\">06:30</td><td class=\"day\">06:40</td><td class=\"day\">06:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">07:00</td><td class=\"day\">07:10</td><td class=\"day\">07:20</td><td class=\"day\">07:30</td><td class=\"day\">07:40</td><td class=\"day\">07:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">08:00</td><td class=\"day\">08:10</td><td class=\"day\">08:20</td><td class=\"day\">08:30</td><td class=\"day\">08:40</td><td class=\"day\">08:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">09:00</td><td class=\"day\">09:10</td><td class=\"day\">09:20</td><td class=\"day\">09:30</td><td class=\"day\">09:40</td><td class=\"day\">09:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">10:00</td><td class=\"day\">10:10</td><td class=\"day\">10:20</td><td class=\"day\">10:30</td><td class=\"day\">10:40</td><td class=\"day\">10:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">11:00</td><td class=\"day\">11:10</td><td class=\"day\">11:20</td><td class=\"day\">11:30</td><td class=\"day\">11:40</td><td class=\"day\">11:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">12:00</td><td class=\"day\">12:10</td><td class=\"day\">12:20</td><td class=\"day\">12:30</td><td class=\"day\">12:40</td><td class=\"day\">12:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">13:00</td><td class=\"day\">13:10</td><td class=\"day\">13:20</td><td class=\"day\">13:30</td><td class=\"day\">13:40</td><td class=\"day\">13:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">14:00</td><td class=\"day\">14:10</td><td class=\"day\">14:20</td><td class=\"day\">14:30</td><td class=\"day\">14:40</td><td class=\"day\">14:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">15:00</td><td class=\"day\">15:10</td><td class=\"day\">15:20</td><td class=\"day\">15:30</td><td class=\"day\">15:40</td><td class=\"day\">15:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">16:00</td><td class=\"day\">16:10</td><td class=\"day\">16:20</td><td class=\"day\">16:30</td><td class=\"day\">16:40</td><td class=\"day\">16:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">17:00</td><td class=\"day\">17:10</td><td class=\"day\">17:20</td><td class=\"day\">17:30</td><td class=\"day\">17:40</td><td class=\"day\">17:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">18:00</td><td class=\"day\">18:10</td><td class=\"day\">18:20</td><td class=\"day\">18:30</td><td class=\"day\">18:40</td><td class=\"day\">18:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">19:00</td><td class=\"day\">19:10</td><td class=\"day\">19:20</td><td class=\"day\">19:30</td><td class=\"day\">19:40</td><td class=\"day\">19:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">20:00</td><td class=\"day\">20:10</td><td class=\"day\">20:20</td><td class=\"day\">20:30</td><td class=\"day\">20:40</td><td class=\"day\">20:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">21:00</td><td class=\"day\">21:10</td><td class=\"day\">21:20</td><td class=\"day\">21:30</td><td class=\"day\">21:40</td><td class=\"day\">21:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">22:00</td><td class=\"day\">22:10</td><td class=\"day\">22:20</td><td class=\"day\">22:30</td><td class=\"day\">22:40</td><td class=\"day\">22:50</td></tr>" +
            ////    Environment.NewLine + "<tr><td class=\"day\">23:00</td><td class=\"day\">23:10</td><td class=\"day\">23:20</td><td class=\"day\">23:30</td><td class=\"day\">23:40</td><td class=\"day\">23:50</td></tr>");
            r.Append(Environment.NewLine + "</TBODY>" +
                Environment.NewLine + "</TABLE>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "<!--JS-->" +
                Environment.NewLine + "$('#idDivScroll').scrollTop($('td.day.active').position().top-100)");
            r1 = r.ToString();
            r = null;
            return Content (r1);
        }
        #endregion

        #region Setting Language
        [HttpPost]
        public IActionResult Language()// Utils/Language
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            string l = _context.GetFormValue("language");
            string Url = Tools.UrlDecode(_context.GetFormValue("UrlBack"));
            if (Url == "") Url = "/Home/Index";
            if (l != "")
            {
                // Ghi nhớ Session vào biến tạm
                string s = "Token^UserID^UserName^ImageID^StaffID^CompanyID^CompanyCode^CountUser^CountUserLogin^PeriodID^PeriodStartDate^PeriodEndDate^PageSizeReport^PageSizeBaseTab^PermissionList^config.json^appsettings.json";
                string[] a = s.Split(new string[] { "^" }, StringSplitOptions.None);
                string[] b = s.Split(new string[] { "^" }, StringSplitOptions.None);
                for (int i = 0; i < a.Length; i++)
                {
                    b[i] = _context.GetSession(a[i]);
                }

                // Xóa cache
                _context.ClearSession();
                //_context.DeleteAllCookie();

                // set lại session
                for (int i = 0; i < a.Length; i++)
                {
                    _context.SetSession(a[i], b[i]);
                }
                _context.SetSession("language", l);
                //HttpContext.Session.Set("language", Encoding.UTF8.GetBytes(l));
                //ToolFolder t = new ToolFolder("language." + l + ".json");
                //string lang = t.ReadConfig();
                //_context.SetSession("json-language", lang);
                ToolDAO bos = new ToolDAO(_context);
                dynamic d1 = null; string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = "";
                d1 = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\":\"Language\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + l + "\"}," +
                "{\"ParamName\":\"Json\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"-1\", \"InputValue\":\"\"}]}");
                bos.ExecuteStore("LanguageText", "SP_CMS__LanguageText_ListAll", d1, ref parameterOutput, ref json, ref errorCode, ref errorString);
                //SetSession("json-language", lang);
                d1 = JObject.Parse("{" + parameterOutput + "}");
                //l = d1.ParameterOutput.Json.ToString();
                //_context.SetSession("json-language", l);
                _context._cache.Set("json-language_" + _context.GetLanguage(), d1.ParameterOutput.Json.ToString());
                //HttpContext.Session.Set("json-language", Encoding.UTF8.GetBytes(lang));
            }
            return Redirect(Url);
        }
        #endregion

        #region configuration interface
        public IActionResult PopupStoreChoice()// Utils/PopupStoreChoice
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context);
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                //string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                //if (!_context.CheckPermistion(int.Parse(MenuID)))
                //{
                //    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
                //}
                StringBuilder r = new StringBuilder(); string r1 = "";
                ToolDAO DBLoad = new ToolDAO("DBLoad", _context, true);
                string Schema = _context.GetRequestVal("ListSchema");
                string StoreName = _context.GetRequestVal("StoreName");
                string parameterOutput = ""; int errorCode = 0; string errorString = ""; string json = ""; dynamic d;
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"Company\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"40\", \"InputValue\":\"" + Schema + "\"}," +
                    "{\"ParamName\":\"StoreName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + StoreName + "\"}," +
                    "{\"ParamName\":\"query\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"2000\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"RequestJson\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"30000\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"ResponseJson\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"30000\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"Param\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"30000\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"Column\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"30000\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"ParamExecInput\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"30000\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"ParamInput\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"30000\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"ColumnInput\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"30000\", \"InputValue\":\"\"}]}");
                DBLoad.ExecuteStore("getParamAPI", "BOS.SP_CONFIG__ExecuteStore", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                d = JObject.Parse("{" + parameterOutput + "}");
                r.Append("<table width=\"100%\" class=\"table table-hover table-border flex\">");
                r.Append("<tr><td><a href=\"javascript:removeDoExecStore();\" id=\"datepickerbtn\"><img src=\"../images/CloseW.gif\" class=\"imggo\" border=\"0\"></a></td><td>" + UIDef.UIButton("DBTypeAction", _context.GetLanguageLable("Choice"), "DBTypeAction();", " class=\"btn select\"") + "</td></tr>" +
                "<tr><td nowrap width='150'>Schema</td><td>: " +
                UIDef.UITextbox("Schema", "Schema", Schema, "", "", "") + "</td></tr>" +
                "<tr><td nowrap width='150'>StoreName</td><td>: " +
                UIDef.UITextbox("StoreName", "StoreName", StoreName, "", "", "") + "</td></tr>" +
                "<tr><td nowrap width='150'>RequestJson</td><td>: " +
                UIDef.UITextbox("RequestJson", "RequestJson", Tools.HtmlEncode(d.ParameterOutput.RequestJson.ToString()), "", "", "") + "</td></tr>" +
                "<tr><td nowrap width='150'>ResponseJson</td><td>: " +
                UIDef.UITextbox("ResponseJson", "ResponseJson", Tools.HtmlEncode(d.ParameterOutput.ResponseJson.ToString()), "", "", "") + "</td></tr>" +
                "<tr><td nowrap width='150'>Param</td><td>: " +
                "<tr><td nowrap width='150'>ParamInput</td><td>: " +
                UIDef.UITextbox("ParamInput", "ParamInput", Tools.HtmlEncode(d.ParameterOutput.ParamInput.ToString()), "", "", "") + "</td></tr>" +
                UIDef.UITextbox("Param", "Param", Tools.HtmlEncode(d.ParameterOutput.Param.ToString()), "", "", "") + "</td></tr>" +
                "<tr><td nowrap width='150'>Column</td><td>: " +
                UIDef.UITextbox("Column", "Column", Tools.HtmlEncode(d.ParameterOutput.Column.ToString()), "", "", "") + "</td></tr>" +
                "<tr><td nowrap width='150'>ParamExecInput</td><td>: " +
                UIDef.UITextbox("ParamExecInput", "ParamExecInput", Tools.HtmlEncode(d.ParameterOutput.ParamExecInput.ToString()), "", "", "") + "</td></tr>" +
                "<tr><td nowrap width='150'>ColumnInput</td><td>: " +
                UIDef.UITextbox("ColumnInput", "ColumnInput", Tools.HtmlEncode(d.ParameterOutput.ColumnInput.ToString()), "", "", "") + "</td></tr>" +
                "</table> ");
                r1 = r.ToString();
                r = null;
                return Content(r1);
            }
        }
        public IActionResult PopupDataTypeChoice()// Utils/PopupDataTypeChoice
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context);
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                //string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                //if (!_context.CheckPermistion(int.Parse(MenuID)))
                //{
                //    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
                //}
                StringBuilder r = new StringBuilder(); string r1 = "";
                string val1 = ""; string val = "Actb||ActbText||Algorithm||AttachFiles||AttachImage||AutoNumber||Checkbox||Date||Datetime||DivMutilCheckbox||Hidden||MutilCheckbox||Numeric||NumericRpt||Password||Radio||SearchBox||SearchBoxC||SearchBoxM||Selectbox||SelectboxText||Textarea||Textbox||Time";
                string[] ElmType = _context.GetRequestVal("ElmType").Split(new string[] { ";" }, StringSplitOptions.None);
                val1 = (ElmType[0] != "" ? ElmType[0] : "-1");
                string ElmType1 = ""; if (ElmType.Length > 1) ElmType1 = ElmType[1];
                string ElmType2 = ""; if (ElmType.Length > 2) ElmType2 = ElmType[2];
                string ElmType3 = ""; if (ElmType.Length > 3) ElmType3 = ElmType[3];
                string ElmType4 = ""; if (ElmType.Length > 4) ElmType4 = ElmType[4];
                string ElmType5 = ""; if (ElmType.Length > 5) ElmType5 = ElmType[5];
                string ElmType6 = ""; if (ElmType.Length > 6) ElmType6 = ElmType[6];
                string ElmType7 = ""; if (ElmType.Length > 7) ElmType7 = ElmType[7];
                string ElmType8 = ""; if (ElmType.Length > 8) ElmType8 = ElmType[8];
                string ElmType9 = ""; if (ElmType.Length > 9) ElmType9 = ElmType[9];
                string ElmType10 = ""; if (ElmType.Length > 10) ElmType10 = ElmType[10];
                string ElmType11 = ""; if (ElmType.Length > 11) ElmType11 = ElmType[11];
                string ElmType12 = ""; if (ElmType.Length > 12) ElmType12 = ElmType[12];
                r.Append(UIDef.UIHidden("ListSchema", _context.GetRequestVal("ListSchema")));
                r.Append("<table width=\"100%\" class=\"table table-hover table-border flex\">");
                r.Append("<tr><td><a href=\"javascript:removeDoCal();\" id=\"datepickerbtn\"><img src=\"../images/CloseW.gif\" class=\"imggo\" border=\"0\"></a></td><td>" + UIDef.UIButton("DBTypeAction", _context.GetLanguageLable("Choice"), "DBTypeAction();", " class=\"btn select\"") + "</td></tr>" +
                "<tr><td nowrap width='150'>Loại input</td><td>: " +
                UIDef.UISelectStr("ElmType0", val, val, ref val1, true, "", "LoadTypeForm(this.value, 'LoadType');") +
                "</td></tr><tr><td id=\"LoadType\" colspan=2></td></tr>" +
                "</table> ");
                val1 = ElmType2;
                string UISelectStr = UIDef.UISelectStr("ElmType2", "0||1", "Không||Có", ref val1, true, "", "").Replace("\"", "'").Replace(Environment.NewLine, "");
                r.Append("<!--JS-->");
                r.Append("var UIList1 = new Array();var UIList2 = new Array();UIList1[0] = \"Hidden\";" +
                    Environment.NewLine + "UIList2[0] =\"<table width=100%><tr><td nowrap width='150'>Giá trị</td>        <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr></table>\";" +
                    Environment.NewLine + "UIList1[1] = \"AutoNumber\";" +
                    Environment.NewLine + "UIList2[1] =\"<table width=100%><tr><td nowrap width='150'>Tên bảng</td>       <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Tên cột-Tiền tố</td><td>: <input autocomplete='off' name='ElmType2' value='" + ElmType2 + "' id='ElmType2' size=50 maxlength=500></td></tr> <tr><td>Độ rộng số tự tăng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[2] = \"Textarea\";" +
                    Environment.NewLine + "UIList2[2] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Số cột</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Số dòng</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[3] = \"Textbox\";" +
                    Environment.NewLine + "UIList2[3] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Độ dài tối đa</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[4] = \"Password\";" +
                    Environment.NewLine + "UIList2[4] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Độ dài tối đa</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[5] = \"Algorithm\";" +
                    Environment.NewLine + "UIList2[5] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Độ dài tối đa</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[6] = \"Numeric\";" +
                    Environment.NewLine + "UIList2[6] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Độ dài tối đa</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>Số nhỏ nhất</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> <tr><td>Số lớn nhất</td><td>: <input autocomplete='off' name='ElmType7' value='" + ElmType7 + "' id='ElmType7' size=50 maxlength=500></td></tr> <tr><td>Số chữ số sau dấu phảy</td><td>: <input autocomplete='off' name='ElmType8' value='" + ElmType8 + "' id='ElmType8' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[7] = \"NumericRpt\";" +
                    Environment.NewLine + "UIList2[7] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Số chữ số sau dấu phảy</td><td>: <input autocomplete='off' name='ElmType2' value='" + ElmType2 + "' id='ElmType2' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[8] = \"Date\";" +
                    Environment.NewLine + "UIList2[8] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[9] = \"Datetime\";" +
                    Environment.NewLine + "UIList2[9] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[10] = \"Time\";" +
                    Environment.NewLine + "UIList2[10] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[11] = \"Checkbox\";" +
                    Environment.NewLine + "UIList2[11] =\"<table width=100%><tr><td nowrap width='150'>G.trị so sánh</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Nhãn</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Giá trị</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[12] = \"MutilCheckbox\";" +
                    Environment.NewLine + "UIList2[12] =\"<table width=100%><tr><td nowrap width='150'>G.trị so sánh</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Tên store</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500>" +
                    UIDef.UIButton("bntElmType3", "", "DoExecStore(document.getElementById('ElmType3'),document.getElementById('ListSchema').value);", " class=\"btn find\"").Replace("\"", "\\\"").Replace(Environment.NewLine, "") + "</td></tr> <tr><td>Param ||</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>Tên cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[13] = \"DivMutilCheckbox\";" +
                    Environment.NewLine + "UIList2[13] =\"<table width=100%><tr><td nowrap width='150'>G.trị so sánh</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Tên store</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500>" +
                    UIDef.UIButton("bntElmType3", "", "DoExecStore(document.getElementById('ElmType3'),document.getElementById('ListSchema').value);", " class=\"btn find\"").Replace("\"", "\\\"").Replace(Environment.NewLine, "") + "</td></tr> <tr><td>Param ||</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>Tên cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>Chiều cao</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> <tr><td>Rộng</td><td>: <input autocomplete='off' name='ElmType7' value='" + ElmType7 + "' id='ElmType7' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType8' value='" + ElmType8 + "' id='ElmType8' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[14] = \"Radio\";" +
                    Environment.NewLine + "UIList2[14] =\"<table width=100%><tr><td nowrap width='150'>G.trị so sánh</td> <td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>D.sách nhãn</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>D.sách giá trị</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>D.sách màu</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[15] = \"Actb\";" +
                    Environment.NewLine + "UIList2[15] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Độ dài tối đa</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>Tên store</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500>" +
                    UIDef.UIButton("bntElmType5", "", " DoExecStore(document.getElementById('ElmType5'),document.getElementById('ListSchema').value); ", " class=\"btn find\"").Replace("\"", "\\\"").Replace(Environment.NewLine, "") + "</td></tr> <tr><td>Param ||</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> <tr><td>Tên cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType7' value='" + ElmType7 + "' id='ElmType7' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType8' value='" + ElmType8 + "' id='ElmType8' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[16] = \"ActbText\";" +
                    Environment.NewLine + "UIList2[16] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Độ dài tối đa</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>D.sách giá trị</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>D.sách nhãn</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType7' value='" + ElmType7 + "' id='ElmType7' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[17] = \"Selectbox\";" +
                    Environment.NewLine + "UIList2[17] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Tên store</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500>" +
                    UIDef.UIButton("bntElmType3", "", "DoExecStore(document.getElementById('ElmType3'),document.getElementById('ListSchema').value);", " class=\"btn find\"").Replace("\"", "\\\"").Replace(Environment.NewLine, "") + "</td></tr> <tr><td>Param ||</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>Tên cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType7' value='" + ElmType7 + "' id='ElmType7' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[18] = \"SelectboxText\";" +
                    Environment.NewLine + "UIList2[18] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>D.sách giá trị</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>D.sách nhãn</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> <tr><td>Hành động</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[19] = \"AttachImage\";" +
                    Environment.NewLine + "UIList2[19] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng ảnh</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Chiều cao ảnh</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[20] = \"AttachFiles\";" +
                    Environment.NewLine + "UIList2[20] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[21] = \"SearchBox\";" +
                    Environment.NewLine + "UIList2[21] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Tên store</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500>" +
                    UIDef.UIButton("bntElmType4", "", "DoExecStore(document.getElementById('ElmType4'),document.getElementById('ListSchema').value);", " class=\"btn find\"").Replace("\"", "\\\"").Replace(Environment.NewLine, "") + "</td></tr> <tr><td>Param ||</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>Nhãn param</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> <tr><td>Loại Input Param</td><td>: <input autocomplete='off' name='ElmType7' value='" + ElmType7 + "' id='ElmType7' size=50 maxlength=500></td></tr> <tr><td>Tên cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType8' value='" + ElmType8 + "' id='ElmType8' size=50 maxlength=500></td></tr> <tr><td>Nhãn cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType9' value='" + ElmType9 + "' id='ElmType9' size=50 maxlength=500></td></tr> <tr><td>Kiểu dữ liệu</td><td>: <input autocomplete='off' name='ElmType10' value='" + ElmType10 + "' id='ElmType10' size=50 maxlength=500></td></tr> <tr><td>D.sách cột nhận giá trị</td><td>: <input autocomplete='off' name='ElmType11' value='" + ElmType11 + "' id='ElmType11' size=50 maxlength=500></td></tr> <tr><td>Đường dẫn tìm kiếm</td><td>: <input autocomplete='off' name='ElmType12' id='ElmType12' value='" + ElmType12 + "' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[22] = \"SearchBoxM\";" +
                    Environment.NewLine + "UIList2[22] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Tên store</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500>" +
                    UIDef.UIButton("bntElmType4", "...", "DoExecStore(document.getElementById('ElmType4'),document.getElementById('ListSchema').value);", " class=\"btn find\"").Replace("\"", "\\\"").Replace(Environment.NewLine, "") + "</td></tr> <tr><td>Param ||</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>Nhãn param</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> <tr><td>Loại Input Param</td><td>: <input autocomplete='off' name='ElmType7' value='" + ElmType7 + "' id='ElmType7' size=50 maxlength=500></td></tr> <tr><td>Tên cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType8' value='" + ElmType8 + "' id='ElmType8' size=50 maxlength=500></td></tr> <tr><td>Nhãn cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType9' value='" + ElmType9 + "' id='ElmType9' size=50 maxlength=500></td></tr> <tr><td>Kiểu dữ liệu</td><td>: <input autocomplete='off' name='ElmType10' value='" + ElmType10 + "' id='ElmType10' size=50 maxlength=500></td></tr> <tr><td>D.sách cột nhận giá trị</td><td>: <input autocomplete='off' name='ElmType11' value='" + ElmType11 + "' id='ElmType11' size=50 maxlength=500></td></tr> <tr><td>Đường dẫn tìm kiếm</td><td>: <input autocomplete='off' name='ElmType12' id='ElmType12' value='" + ElmType12 + "' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "UIList1[23] = \"SearchBoxC\";" +
                    Environment.NewLine + "UIList2[23] =\"<table width=100%><tr><td nowrap width='150'>G.trị mặc định</td><td>: <input autocomplete='off' name='ElmType1' value='" + ElmType1 + "' id='ElmType1' size=50 maxlength=500></td></tr> <tr><td>Bắt buộc</td><td>: " + UISelectStr + "</td></tr> <tr><td>Độ rộng</td><td>: <input autocomplete='off' name='ElmType3' value='" + ElmType3 + "' id='ElmType3' size=50 maxlength=500></td></tr> <tr><td>Tên store</td><td>: <input autocomplete='off' name='ElmType4' value='" + ElmType4 + "' id='ElmType4' size=50 maxlength=500>" +
                    UIDef.UIButton("bntElmType4", "", "DoExecStore(document.getElementById('ElmType4'),document.getElementById('ListSchema').value);", " class=\"btn find\"").Replace("\"", "\\\"").Replace(Environment.NewLine, "") + "</td></tr> <tr><td>Param ||</td><td>: <input autocomplete='off' name='ElmType5' value='" + ElmType5 + "' id='ElmType5' size=50 maxlength=500></td></tr> <tr><td>Nhãn param</td><td>: <input autocomplete='off' name='ElmType6' value='" + ElmType6 + "' id='ElmType6' size=50 maxlength=500></td></tr> <tr><td>Loại Input Param</td><td>: <input autocomplete='off' name='ElmType7' value='" + ElmType7 + "' id='ElmType7' size=50 maxlength=500></td></tr> <tr><td>Tên cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType8' value='" + ElmType8 + "' id='ElmType8' size=50 maxlength=500></td></tr> <tr><td>Nhãn cột dữ liệu</td><td>: <input autocomplete='off' name='ElmType9' value='" + ElmType9 + "' id='ElmType9' size=50 maxlength=500></td></tr> <tr><td>Kiểu dữ liệu</td><td>: <input autocomplete='off' name='ElmType10' value='" + ElmType10 + "' id='ElmType10' size=50 maxlength=500></td></tr> <tr><td>D.sách cột nhận giá trị</td><td>: <input autocomplete='off' name='ElmType11' value='" + ElmType11 + "' id='ElmType11' size=50 maxlength=500></td></tr> <tr><td>Đường dẫn tìm kiếm</td><td>: <input autocomplete='off' name='ElmType12' id='ElmType12' value='" + ElmType12 + "' size=50 maxlength=500></td></tr> </table> \";" +
                    Environment.NewLine + "function LoadTypeForm(a, b) {" +
                    Environment.NewLine + "var id = document.getElementById(b);" +
                    Environment.NewLine + "if (id) {    " +
                    Environment.NewLine + "id.innerHTML = \"\";for (var i = 0; i < UIList1.length; i++) {        " +
                    Environment.NewLine + "if (UIList1[i] == a) {            " +
                    Environment.NewLine + "id.innerHTML = UIList2[i];        }    }}} LoadTypeForm('" + ElmType[0] + "', 'LoadType');" +
                    Environment.NewLine + "function DBTypeAction (){" +
                    Environment.NewLine + "var ElmVal = '';" +
                    Environment.NewLine + "var id = document.getElementById('ElmType0');" +
                    Environment.NewLine + "if (id){ElmVal = id.value;" +
                    Environment.NewLine + "for (var i = 1; i < 13; i ++){" +
                    Environment.NewLine + "id = document.getElementById('ElmType' + i);" +
                    Environment.NewLine + "/*var id1 = document.getElementById('ElmType' + (i + 1));*/" +
                    Environment.NewLine + "if (id){" +
                    Environment.NewLine + "/*if (id1 || id.value != '') */ElmVal = ElmVal + ';' + id.value;" +
                    Environment.NewLine + "}}}" +
                    Environment.NewLine + "if(ElmVal != '' && !(DoCalTarget.readOnly || DoCalTarget.disabled))DoCalTarget.value = ElmVal;removeDoCal();}");
                r1 = r.ToString();
                r = null;
                return Content(r1);
            }
        }
        public IActionResult DBLoad() // Utils/ToolEnc
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context);
            ToolDAO DBLoad = new ToolDAO("DBLoad", _context, true);
            HTTP_CODE.WriteLogAction("functionName:/Utils/DBLoad", _context);
            //ToolDAO bosDAO = new ToolDAO(_context);
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                //if (!_context.CheckPermistion(int.Parse(MenuID)))
                //{
                //    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
                //}
                bool MenuOn = (_context.GetRequestMenuOn() != "Off");

                StringBuilder r1 = new StringBuilder(); string r = "";

                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, MenuID, false));

                // DBLoad
                string[] DataVal = new string[4] { "", "", "", "" };
                string[] DataTxt = new string[4] { "", "", "", "" };
                string[] DataParent = new string[4] { "", "", "", "" };
                r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                        Environment.NewLine + "var DataParent = new Array();" +
                        Environment.NewLine + "var DataTxt = new Array();" +
                        Environment.NewLine + "var DataVal = new Array();");

                UIDef.OptionStringVal(ref DBLoad, "SP_SYS__SchemaLoad", "", "SchemaID,SchemaName", ref DataVal[0], ref DataTxt[0], ref DataParent[0]);
                UIDef.OptionStringVal(ref DBLoad, "SP_SYS__TableFisrtCharLoad", "", "TableID,TableName,ParentID", ref DataVal[1], ref DataTxt[1], ref DataParent[1]);
                UIDef.OptionStringVal(ref DBLoad, "SP_SYS__TableLoad", "", "TableID,TableName,ParentID", ref DataVal[2], ref DataTxt[2], ref DataParent[2]);
                string val = "-1";
                r1.Append(Environment.NewLine + "DataParent[0] = '" + DataParent[0] + "';");
                r1.Append(Environment.NewLine + "DataTxt[0] = '" + DataTxt[0] + "';");
                r1.Append(Environment.NewLine + "DataVal[0] = '" + DataVal[0] + "';");
                r1.Append(Environment.NewLine + "DataParent[1] = '" + DataParent[1] + "';");
                r1.Append(Environment.NewLine + "DataTxt[1] = '" + DataTxt[1] + "';");
                r1.Append(Environment.NewLine + "DataVal[1] = '" + DataVal[1] + "';");
                r1.Append(Environment.NewLine + "DataParent[2] = '" + DataParent[2] + "';");
                r1.Append(Environment.NewLine + "DataTxt[2] = '" + DataTxt[2] + "';");
                r1.Append(Environment.NewLine + "DataVal[2] = '" + DataVal[2] + "';</script>");

                r1.Append("<b>" + _context.GetLanguageLable("DBloadHeaderText") + "</b>");
                r1.Append("<form name=\"DBLoadFrm\" method=\"POST\" action=\"/Utils/DBLoadPost\">");
                r1.Append(UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()));
                r1.Append(UIDef.UIHidden("MenuID", _context.GetRequestVal("MenuID")));
                r1.Append(Environment.NewLine + "<divclass=\"row inline-input\">" +
                        Environment.NewLine + "<div class=\"col-sm-4\">" + 
                        "<div class=\"form-group row\">" + UIDef.UIButton("bntSubmit", _context.GetLanguageLable("DBload"), true, " class=\"btn select\"") + UIDef.UIButton("bntReset", _context.GetLanguageLable("Reset"), false, " class=\"btn refresh\"") + "</div>");
                r1.Append(Environment.NewLine + "<div class=\"form-group row\"><label class=\"col-form-label active\">" + _context.GetLanguageLable("ChoiceSchema") + "</label>" +
                    UIDef.UISelectStr("ListSchema", DataVal[0], DataTxt[0], ref val, true, "", "getChild(this.value,this.form.TableFisrtCharLoad,this.form.ListTable,1)", "document.DBLoadFrm") + "</div>" +
                    Environment.NewLine + "<div class=\"form-group row\"><label class=\"col-form-label active\">" + _context.GetLanguageLable("TableFisrtCharLoad") + "</label>" +
                    UIDef.UISelectStr("TableFisrtCharLoad", DataVal[1], DataTxt[1], ref val, true, "", "getChild(this.value,this.form.ListTable,null,2)", "document.DBLoadFrm") + "</div>" +
                    Environment.NewLine + "<div class=\"form-group row\"><label class=\"col-form-label active\">" + _context.GetLanguageLable("ChoiceTable") + "</label>" +
                    UIDef.UISelectStr("ListTable", "", "", ref val, true, "", "", "document.DBLoadFrm") +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "</div>");
                r1.Append("</form>");
                r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                r = r1.ToString();
                r1 = null;
                ViewData["DBLoad"] = r;
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
                ViewData["IsPageLogin"] = !MenuOn;
                ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                ViewData["PageTitle"] = _context.GetLanguageLable("DBLoadTitlePage");
                r1 = null;
                return View();
            }
        }
        [HttpPost]
        public IActionResult DBLoadPost() // Utils/ToolEnc
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context);
            ToolDAO DBLoad = new ToolDAO("DBLoad", _context, true);
            HTTP_CODE.WriteLogAction("functionName:/Utils/DBLoad", _context);
            //ToolDAO bosDAO = new ToolDAO(_context);
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                //string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                //if (!_context.CheckPermistion(int.Parse(MenuID)))
                //{
                //    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
                //}
                string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                string TableID = _context.GetRequestVal("ListTable");

                StringBuilder r1 = new StringBuilder(); string r = "";

                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, MenuID, false));

                // DBLoad
                string[] a; a = new string[16] { "STT", "ColTableName", "ColTableType", "ColTableLenght", "InputFormFillter", "InputFormFillterInline", "ColList", "ColListType", "ColListInput", "IsDisplay", "TableParent", "TableChild", "InputFormAdd", "InputFormAddInline", "InputGroup", "Orderby" };
                string[] b; b = new string[16] { "STT", "ColTable Name", "ColTable Type", "Lenght", "Input Form Fillter", "Inline", "ColList", "ColList Type", "ColList Input", "Display", "Table Parent", "Table Child", "InputFormAdd", "Inline", "Group", "Order" };
                string[] c; c = new string[16] { "", "Textbox;;1;14;200", "Selectbox;-1;1;SP_SYS__DataType;;TypeID,TypeName", "Numeric;;0;10;10", "DBTypePopup;;1;20;4000", "Checkbox;;0;;1", "Textbox;;1;14;2000", "Selectbox;-1;1;SP_SYS__DataType;;TypeID,TypeName", "DBTypePopup;;1;20;4000", "Checkbox;;0;;1", "Textbox;;0;14;2000", "Textbox;;0;14;2000", "DBTypePopup;;1;20;4000", "Checkbox;;0;;1", "Textbox;;0;14;2000", "Numeric;;0;10;10" };
                //new string[8] {"", "Textbox;;1;14;20", "Textbox;;1;14;20", "Selectbox;-1;1;SP_SYS__DataType;;TypeID,TypeName", "Numeric;;0;10;10", "Textbox;;1;20;4000", "DBTypePopup;;1;20;4000", "Textbox;;1;14;20", "Checkbox;;0;;1" };
                string[] DataVal; DataVal = new string[16] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
                string[] DataTxt; DataTxt = new string[16] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
                string[] DataParent; DataParent = new string[16] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
                dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = ""; //string l = "";
                string ListSchema = _context.GetRequestVal("ListSchema"); string ListTable = _context.GetRequestVal("ListTable");
                string fileName = _context.AppConfig.FolderRoot + "\\" + FolderRoot + "\\" + ListSchema + "\\" + ListTable + "\\List.json";
                if (_context.AppConfig.PathIsFile(fileName))
                {
                    d = JObject.Parse(_context.AppConfig.FileReadAllText(fileName));
                }
                else
                {
                    d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"TableID\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + TableID + "\"}]}");
                    DBLoad.ExecuteStore("ColumnLoad", "SP_SYS__TableLoadColumn", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                    d = JObject.Parse(json);
                }

                r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                            Environment.NewLine + "var ColumnName = new Array();" +
                            Environment.NewLine + "var ColumnLable = new Array();" +
                            Environment.NewLine + "var ColumnRequire = new Array();" +
                            Environment.NewLine + "var ColumnType = new Array();" +
                            Environment.NewLine + "var ColumnData = new Array();" +
                            Environment.NewLine + "var DataParent = new Array();" +
                            Environment.NewLine + "var DataTxt = new Array();" +
                            Environment.NewLine + "var DataVal = new Array();</script>");
                r1.Append("<b>" + _context.GetLanguageLable("DBLoadPostHeaderText") + "</b>");
                r1.Append("<form name=\"ListForm\" method=\"POST\" action=\"/Utils/DBLoadGenScript\">");
                r1.Append(UIDef.UIHidden("ListColumn", "STT,ColTableName,ColTableType,ColTableLenght,InputFormFillter,InputFormFillterInline,ColList,ColListType,ColListInput,IsDisplay,TableParent,TableChild,InputFormAdd,InputFormAddInline,InputGroup,Orderby"));
                r1.Append(UIDef.UIHidden("ListSchema", ListSchema));
                r1.Append(UIDef.UIHidden("ListTable", ListTable));
                r1.Append(UIDef.UIHidden("SysUID", ""));
                r1.Append(UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()));
                r1.Append(UIDef.UIHidden("MenuID", _context.GetRequestVal("MenuID")));
                r1.Append("<div class=\"form-group row\">" +
                    UIDef.UIButton("bntAddnew", _context.GetLanguageLable("AddNew"), "insertTabRow('dbtablist');", " class=\"btn add\" ") +
                    UIDef.UIButton("bntSubmit", _context.GetLanguageLable("GenScript"), "onUpdateRow(this.form);", " class=\"btn select\"") +
                    UIDef.UICheckbox("IsMultiLang", "IsMultiLang", "1", "", "") +
                    Environment.NewLine + "</div>");
                r1.Append("<table width=\"100%\" class=\"table table-hover table-border flex\" id=\"dbtablist\">");
                r1.Append("<thead class=\"thead-light\"><tr>");
                string mandantoryLabel = ""; string ReqJson = "";
                //r1.Append("<th align=\"center\">STT");
                for (int i = 0; i < a.Length; i++)
                {
                    string[] a4 = c[i].Split(new string[] { ";" }, StringSplitOptions.None);
                    string s = ""; string val = "";
                    DataVal[i] = ""; DataTxt[i] = ""; DataParent[i] = "";
                    r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                            Environment.NewLine + "ColumnRequire[" + i + "] = '_';" +
                            Environment.NewLine + "ColumnName[" + i + "] = '" + a[i] + "';" +
                            Environment.NewLine + "ColumnType[" + i + "] = '" + c[i] + "';" +
                            Environment.NewLine + "ColumnLable[" + i + "] = '" + _context.GetLanguageLable(b[i]) + "';</script>");
                    mandantoryLabel = "";
                    if (a4.Length > 2)
                    {
                        if (Tools.CIntNull(a4[2]) == 1)
                        {
                            r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                                Environment.NewLine + "ColumnRequire[" + i + "] = '" + a[i] + "';</script>");
                            mandantoryLabel = " <font color='red'>*</font>";//(*)
                        }
                    }
                    if (a4[0] != "Hidden") r1.Append("<th align=\"center\">" + _context.GetLanguageLable(b[i]) + mandantoryLabel + "</th>");
                    switch (a4[0])
                    {
                        case "Selectbox":
                            ReqJson = _context.InputDataSetParam(a4[4]);
                            UIDef.OptionStringVal(ref DBLoad, a4[3], ReqJson, a4[5], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                            s = UIDef.SelectStrOption(DataVal[i], DataTxt[i], ref val, true);
                            break;
                        case "SelectboxText":
                            s = UIDef.SelectStrOption(a4[3], a4[4], ref val, true);
                            break;
                    }
                    r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                    r1.Append(Environment.NewLine + "ColumnData[" + i + "] = '" + s + "'");
                    r1.Append(Environment.NewLine + "DataParent[" + i + "] = '" + DataParent[i] + "'");
                    r1.Append(Environment.NewLine + "DataTxt[" + i + "] = '" + DataTxt[i] + "'");
                    r1.Append(Environment.NewLine + "DataVal[" + i + "] = '" + DataVal[i] + "'");
                    r1.Append(Environment.NewLine + "</script>");
                }
                r1.Append("</tr></thead>");
                r1.Append("<tbody>");
                for (int i = 0; i < d.ColumnLoad.Items.Count; i++)
                {
                    r1.Append("<tr id=\"trrowid" + i + "\">");

                    for (int j = 0; j < a.Length; j++)
                    {
                        string val = ""; string[] a31 = c[j].Split(new string[] { ";" }, StringSplitOptions.None);
                        try { val = Tools.GetDataJson(d.ColumnLoad.Items[i], a[j]); } catch { val = ""; }
                        if (val == null) val = "";
                        if (a31[0] == "Hidden")
                            r1.Append(UIDef.UIHidden(a[j] + i, val));
                        else
                        {
                            r1.Append("<td>");
                            switch (a31[0])
                            {
                                case "DBTypePopup":
                                    if (a31.Length > 5)
                                        r1.Append(UIDef.UITextbox(_context.GetLanguageLable(b[i]), a[j] + i, val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", "", a31[5]));
                                    else
                                        r1.Append(UIDef.UITextbox(_context.GetLanguageLable(b[i]), a[j] + i, val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", ""));
                                    r1.Append(UIDef.UIButton(a[j] + "_DBP" + i, "", "DoDBType(this.form.elements['" + a[j] + i + "'],this.form.elements['ListSchema'].value);", " class=\"btn find\""));
                                    break;
                                case "Textbox":
                                    if (a31.Length > 5)
                                        r1.Append(UIDef.UITextbox(_context.GetLanguageLable(b[i]), a[j] + i, val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", "", a31[5]));
                                    else
                                        r1.Append(UIDef.UITextbox(_context.GetLanguageLable(b[i]), a[j] + i, val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", ""));
                                    break;
                                case "Numeric":
                                    if (a31.Length > 5)
                                        r1.Append(UIDef.UINumeric(_context.GetLanguageLable(b[i]), a[j] + i, val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", "", "" + a31[5]));
                                    else
                                        r1.Append(UIDef.UINumeric(_context.GetLanguageLable(b[i]), a[j] + i, val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", "", ""));
                                    break;
                                case "Selectbox":
                                    //ReqJson = _context.InputDataSetParam(a31[4]);
                                    r1.Append(UIDef.UISelectStr(a[j] + i, DataVal[j], DataTxt[j], ref val, true, " " + " markline(" + i + ", true, true);", "document.ListForm"));
                                    break;
                                case "SelectboxText":
                                    r1.Append(UIDef.UISelectStr(a[j] + i, a31[3], a31[4], ref val, true, " " + " markline(" + i + ", true, true);", "document.ListForm"));
                                    break;
                                case "Checkbox":
                                    r1.Append(UIDef.UICheckbox(a[j] + i, a31[3], a31[4], val, ""));
                                    break;
                                default:
                                    r1.Append(val);
                                    break;
                            }
                        }
                    }
                }
                r1.Append(UIDef.UIHidden("ItemsCnt", d.ColumnLoad.Items.Count.ToString()));
                r1.Append("</tbody>");
                r1.Append("</table>");
                r1.Append("</form>");
                r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                r1.Append(Environment.NewLine + "function onUpdateRow(frm){" +
                        Environment.NewLine + "var cnt = parseInt(frm.ItemsCnt.value);" +
                        Environment.NewLine + "var msg = '';" +
                        Environment.NewLine + "var inputFocus;" +
                        Environment.NewLine + "for (var i = 0; i < cnt; i++){" +
                        Environment.NewLine + "for (var j = 0; j < ColumnRequire.length; j++){" +
                        Environment.NewLine + "var inputName = ColumnRequire[j] + i;" +
                        Environment.NewLine + "var input = frm.elements[inputName];" +
                        Environment.NewLine + "if (input){" +
                        Environment.NewLine + "var IsReq = true;" +
                        Environment.NewLine + "if (input.type == \"select-one\"){" +
                        Environment.NewLine + "if (input.selectedIndex >= 0 && input.options[input.selectedIndex].value != '' && input.options[input.selectedIndex].value != '-1' && input.options[input.selectedIndex].text != '')IsReq = false;" +
                        Environment.NewLine + "} else if (input.type == \"checkbox\"){" +
                        Environment.NewLine + "if(input.checked == true) " +
                        Environment.NewLine + "IsReq = false;} else {" +
                        Environment.NewLine + "if(input.value != '') " +
                        Environment.NewLine + "IsReq = false;}" +
                        Environment.NewLine + "if (IsReq) {" +
                        Environment.NewLine + "msg=msg+'" + _context.GetLanguageLable("UpdateRow") + " ' + (i+1) + ' " + _context.GetLanguageLable("Column") + " [' + ColumnLable[j] + '] " + _context.GetLanguageLable("IsNull") + "!<br>';" +
                        Environment.NewLine + "if (!inputFocus) inputFocus = input;}}}}" +
                        Environment.NewLine + "/*if (msg != ''){" +
                        Environment.NewLine + "if (!inputFocus) input.focus(); " +
                        Environment.NewLine + "else inputFocus.focus();" +
                        Environment.NewLine + "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', msg, '', '0');" +//alert (msg);
                        Environment.NewLine + "return;} */" +
                        Environment.NewLine + "frm.submit();" +
                        Environment.NewLine + "}");
                r1.Append(Environment.NewLine + "</script>");
                r = r1.ToString();
                r1 = null;
                ViewData["DBLoadPost"] = r;
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
                ViewData["IsPageLogin"] = !MenuOn;
                ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                ViewData["PageTitle"] = _context.GetLanguageLable("DBLoadTitlePage");
                r1 = null;
                return View();
            }
        }
        [HttpPost]
        public IActionResult DBLoadGenScript() // Utils/ToolEnc
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            HTTP_CODE.WriteLogAction("functionName:/Utils/DBLoadGenScript", _context);
            ToolDAO bosDAO = new ToolDAO(_context);
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                //string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                //if (!_context.CheckPermistion(int.Parse(MenuID)))
                //{
                //    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
                //}
                string IsMultiLang = _context.GetFormValue("IsMultiLang"); if (IsMultiLang == "") IsMultiLang = "0";
                string ListSchema = _context.GetFormValue("ListSchema");
                string ListTable = _context.GetFormValue("ListTable");
                string[] a = _context.GetFormValue("ListColumn").Split(new string[] { "," }, StringSplitOptions.None);
                string json = "";
                string CreateTableMSSQL = ""; string CreateTableOracle = "";
                string InsertParentTreeMSSQL = ""; string InsertParentTreeOracle = "";
                string UpdateParentTreeMSSQL = ""; string UpdateParentTreeOracle = "";
                string ListFieldInsert = ""; string ListFieldUpdateOracle = ""; string ListFieldUpdateMSSQL = "";
                string ListParamMSSQL = ""; string ListParamOracle = "";
                string StoreAddEditParamMSSQL = ""; string StoreAddEditParamOracle = ""; string StoreAddEditVariableOracle = "";
                string StoreAddEditParamCheckMSSQL = ""; string StoreAddEditParamCheckOracle = "";
                string StoreListParamMSSQL = ""; string StoreListParamOracle = ""; string StoreListVariableOracle = "";
                string StoreListTableMSSQL = ""; string StoreListTableOracle = "";
                string StoreListTableParentMSSQL = ""; string StoreListTableParentOracle = "";
                string StoreListTableColMSSQL = ""; string StoreListTableColOracle = ""; int iParent = 0;
                string StoreDeleteParamMSSQL = ""; string StoreDeleteParamOracle = ""; string StoreDeleteVariableOracle = ""; bool IsTyped = false;
                int Cnt = int.Parse((_context.GetFormValue("ItemsCnt") == "" ? "0" : _context.GetFormValue("ItemsCnt")));
                for (int i = 0; i < Cnt; i++)
                {
                    //Create table
                    string ColTableType = _context.GetFormValue("ColTableType" + i.ToString());
                    string ColTableName = _context.GetFormValue("ColTableName" + i.ToString());
                    string ColTableLenght = _context.GetFormValue("ColTableLenght" + i.ToString()); ColTableLenght = Tools.RemNumSepr(ColTableLenght);
                    string InputFormFillter = _context.GetFormValue("InputFormFillter" + i.ToString());
                    switch (ColTableName.ToUpper())
                    {
                        case "I":
                            StoreAddEditParamCheckOracle = StoreAddEditParamCheckOracle + "	v_" + ListTable + "ID := NVL(v_" + ListTable + "ID, 0) ;\n" +
                                "	IF ( v_" + ListTable + "ID > 0 ) THEN\n" +
                                "		DECLARE v_SysS NUMBER(10, 0);\n" +
                                "		BEGIN\n" +
                                "			SELECT NVL(SysS, -1) INTO v_SysS FROM " + ListTable + " WHERE I = v_" + ListTable + "ID;\n" +
                                "			EXCEPTION WHEN NO_DATA_FOUND THEN v_SysS:= -1;\n" +
                                "			IF(v_SysS > 0) THEN\n" +
                                "				p_Message := u'DataIsAdministratorBuild';\n" +
                                "				p_ResponseStatus:= -600;\n" +
                                "				RETURN;\n" +
                                "			END IF;\n" +
                                "		END;\n" +
                                "	END IF; \n";
                            StoreAddEditParamCheckMSSQL = StoreAddEditParamCheckMSSQL + "	SET @" + ListTable + "ID = ISNULL(@" + ListTable + "ID, 0) ;\n" +
                                "	IF (@" + ListTable + "ID > 0)\n" +
                                "		BEGIN\n" +
                                "		    DECLARE @SysS int;\n" +
                                "			SELECT @SysS = ISNULL(SysS, -1) FROM " + ListTable + " WHERE I = @" + ListTable + "ID\n" +
                                "			SET @SysS = ISNULL(@SysS, -1)\n" +
                                "			IF(@SysS > 0)\n" +
                                "		    BEGIN\n" +
                                "				SET @Message = N'DataIsAdministratorBuild';\n" +
                                "				SET @ResponseStatus = -600;\n" +
                                "				RETURN;\n" +
                                "			END\n" +
                                "		END\n";
                            break;
                        case "C":
                            StoreAddEditVariableOracle = StoreAddEditVariableOracle +
                                "	v_ColUnique NVARCHAR2(30) := ' ';\n" +
                                "	v_ColUniqueVal NVARCHAR2(30) := ' ';\n" +
                                "	v_Table NVARCHAR2(30) := '" + ListTable + "';\n";
                            StoreAddEditParamCheckOracle = StoreAddEditParamCheckOracle + "	v_" + ListTable + "Code := NVL(v_" + ListTable + "Code, ' ') ;\n" +
                                "	SP_SUB__TableCode_Check \n" +
                                "	(\n		p_Id => v_" + ListTable + "ID,\n" +
                                "		p_Code => v_" + ListTable + "Code,\n" +
                                "		p_ColUnique => v_ColUnique,\n" +
                                "		p_ColUniqueVal => v_ColUniqueVal,\n" +
                                "		p_Table => v_Table,\n" +
                                "		p_Message => p_Message,\n" +
                                "		p_ResponseStatus => p_ResponseStatus\n" +
                                "	); \n" +
                                "	IF (p_ResponseStatus < 0) THEN\n" +
                                "		RETURN;\n" +
                                "	END IF; \n";
                            StoreAddEditParamCheckMSSQL = StoreAddEditParamCheckMSSQL +
                                "	DECLARE @ColUnique NVARCHAR(30) = '',\n" +
                                "	    @ColUniqueVal NVARCHAR(30) = '',\n" +
                                "	    @Table NVARCHAR(30) = '" + ListTable + "';\n" +
                                "	SET @" + ListTable + "Code = ISNULL(@" + ListTable + "Code, '') ;\n" +
                                "	EXEC SP_SUB__TableCode_Check \n" +
                                "		@Id = @" + ListTable + "ID,\n" +
                                "		@Code = @" + ListTable + "Code,\n" +
                                "		@ColUnique = @ColUnique,\n" +
                                "		@ColUniqueVal = @ColUniqueVal,\n" +
                                "		@Table = @Table,\n" +
                                "		@Message = @Message OUTPUT,\n" +
                                "		@ResponseStatus = @ResponseStatus OUTPUT\n" +
                                "   ; \n" +
                                "	IF (@ResponseStatus < 0) RETURN;\n";
                            break;
                        case "N":
                            if (IsMultiLang == "1")
                            {
                                StoreAddEditParamCheckOracle = StoreAddEditParamCheckOracle + "	v_Vietnamese := NVL(v_Vietnamese, ' ') ;\n" +
                                    "	IF ( v_Vietnamese = ' ' ) THEN\n" +
                                    "		p_Message:= u'VietnameseIsNull';\n" +
                                    "		p_ResponseStatus:= -600;\n" +
                                    "		RETURN;\n" +
                                    "	END IF;\n " +
                                    "	v_English := NVL(v_English, ' ') ;\n" +
                                    "	IF ( v_English = ' ' ) THEN\n" +
                                    "		p_Message:= u'EnglishIsNull';\n" +
                                    "		p_ResponseStatus:= -600;\n" +
                                    "		RETURN;\n" +
                                    "	END IF;\n ";
                                StoreAddEditParamCheckMSSQL = StoreAddEditParamCheckMSSQL + "	SET @Vietnamese = ISNULL(@Vietnamese, '') ;\n" +
                                    "	IF (@Vietnamese = '' ) \n" +
                                    "   BEGIN\n" +
                                    "		SET @Message = N'VietnameseIsNull'\n" +
                                    "		SET @ResponseStatus = -600\n" +
                                    "		RETURN\n" +
                                    "	END\n " +
                                    "	SET @English = ISNULL(@English, '') ;\n" +
                                    "	IF (@English = '' ) \n" +
                                    "   BEGIN\n" +
                                    "		SET @Message = N'EnglishIsNull'\n" +
                                    "		SET @ResponseStatus = -600\n" +
                                    "		RETURN\n" +
                                    "	END\n ";
                            }
                            else
                            {
                                StoreAddEditParamCheckOracle = StoreAddEditParamCheckOracle + "	v_" + ListTable + "Name := NVL(v_" + ListTable + "Name, ' ') ;\n" +
                                "	IF ( v_" + ListTable + "Name = ' ' ) THEN\n" +
                                "		p_Message:= u'" + ListTable + "NameIsNull';\n" +
                                "		p_ResponseStatus:= -600;\n" +
                                "		RETURN;\n" +
                                "	END IF;\n ";
                                StoreAddEditParamCheckMSSQL = StoreAddEditParamCheckMSSQL + "	SET @" + ListTable + "Name = ISNULL(@" + ListTable + "Name, '') ;\n" +
                                    "	IF (@" + ListTable + "Name = '' ) \n" +
                                    "   BEGIN\n" +
                                    "		SET @Message = N'" + ListTable + "NameIsNull'\n" +
                                    "		SET @ResponseStatus = -600\n" +
                                    "		RETURN\n" +
                                    "	END\n ";
                            }
                            break;
                        case "ISACTIVE":
                        case "ISDISPLAY":
                        case "STATUS":
                            StoreAddEditParamCheckOracle = StoreAddEditParamCheckOracle + "	v_" + ColTableName + " := NVL(v_" + ColTableName + ", 1) ;\n" +
                                "	IF (v_" + ColTableName + " NOT IN ( 0,1 )) THEN\n" +
                                "		p_Message:= u'StatusIsWrong';\n" +
                                "		p_ResponseStatus:= -600;\n" +
                                "		RETURN;\n" +
                                "	END IF; \n";
                            StoreAddEditParamCheckMSSQL = StoreAddEditParamCheckMSSQL + "	SET @" + ColTableName + " = ISNULL(@" + ColTableName + ", 1) ;\n" +
                                "	IF (@" + ColTableName + " NOT IN ( 0,1 )) \n" +
                                "   BEGIN\n" +
                                "		SET @Message:= N'StatusIsWrong';\n" +
                                "		SET @ResponseStatus:= -600;\n" +
                                "		RETURN;\n" +
                                "	END \n";
                            if (ColTableName.ToUpper() == "STATUS")
                            {
                                StoreDeleteParamMSSQL = "	@Type Smallint, -- 1. Active; 0. Unactive; -1. Delete\n";
                                StoreDeleteParamOracle = "	p_Type IN NUMERIC;\n";
                                StoreDeleteVariableOracle = "	v_Type NUMERIC(5) := p_Type; \n";
                                IsTyped = true;
                            }
                            break;
                        case "PARENTID":
                            StoreAddEditParamCheckOracle = StoreAddEditParamCheckOracle + "	v_ParentID := NVL(v_ParentID, 0) ;\n" +
                                "	BEGIN\n" +
                                "		SELECT NameSpace, OrderBy INTO v_NameSpace, v_Order FROM " + ColTableName + " WHERE I = v_ParentID;\n" +
                                "		EXCEPTION WHEN NO_DATA_FOUND THEN\n" +
                                "		BEGIN\n" +
                                "			v_NameSpace := ' ';\n" +
                                "			v_Order:= ' ';\n" +
                                "		END;\n" +
                                "	END;\n ";
                            StoreAddEditParamCheckMSSQL = StoreAddEditParamCheckMSSQL + "	SET @ParentID = ISNULL(@ParentID, 0) ;\n" +
                                "		SELECT @NameSpace = NameSpace, @Order = OrderBy FROM " + ColTableName + " WHERE I = @ParentID;\n" +
                                "		SET @NameSpace = ISNULL(@NameSpace, '');\n" +
                                "		SET @Order = ISNULL(@Order, '');\n";
                            break;
                    }
                    switch (ColTableType.ToLower())
                    {
                        case "bigint":
                            CreateTableMSSQL = CreateTableMSSQL + "	[" + ColTableName + "] [" + ColTableType + "] " + (ColTableName.ToUpper() == "I" ? " NOT NULL PRIMARY KEY " : (ColTableName.ToUpper() == "C" ? " NOT NULL UNIQUE " : "")) + ",\n";
                            CreateTableOracle = CreateTableOracle + "	" + ColTableName + " NUMERIC (19) " + (ColTableName.ToUpper() == "I" ? " NOT NULL PRIMARY KEY " : (ColTableName.ToUpper() == "C" ? " NOT NULL UNIQUE " : "")) + ",\n";
                            break;
                        case "int":
                            CreateTableMSSQL = CreateTableMSSQL + "	[" + ColTableName + "] [" + ColTableType + "] " + (ColTableName.ToUpper() == "I" ? " NOT NULL PRIMARY KEY " : (ColTableName.ToUpper() == "C" ? " NOT NULL UNIQUE " : "")) + ",\n";
                            CreateTableOracle = CreateTableOracle + "	" + ColTableName + " NUMERIC (10) " + (ColTableName.ToUpper() == "I" ? " NOT NULL PRIMARY KEY " : (ColTableName.ToUpper() == "C" ? " NOT NULL UNIQUE " : "")) + ",\n";
                            break;
                        case "smallint":
                            CreateTableMSSQL = CreateTableMSSQL + "	[" + ColTableName + "] [" + ColTableType + "],\n";
                            CreateTableOracle = CreateTableOracle + "	" + ColTableName + " NUMERIC (5),\n";
                            break;
                        case "bit":
                            CreateTableMSSQL = CreateTableMSSQL + "	[" + ColTableName + "] [" + ColTableType + "],\n";
                            CreateTableOracle = CreateTableOracle + "	" + ColTableName + " NUMERIC (1),\n";
                            break;
                        case "date":
                        case "datetime":
                            CreateTableMSSQL = CreateTableMSSQL + "	[" + ColTableName + "] [" + ColTableType + "],\n";
                            CreateTableOracle = CreateTableOracle + "	" + ColTableName + " DATE,\n";
                            break;
                        case "varchar":
                            CreateTableMSSQL = CreateTableMSSQL + "	[" + ColTableName + "] [" + ColTableType + "](" + ColTableLenght + ") " + (ColTableName.ToUpper() == "I" ? " NOT NULL PRIMARY KEY " : (ColTableName.ToUpper() == "C" ? " NOT NULL UNIQUE " : "")) + ",\n";
                            CreateTableOracle = CreateTableOracle + "	" + ColTableName + " VARCHAR2 (" + ColTableLenght + ") " + (ColTableName.ToUpper() == "I" ? " NOT NULL PRIMARY KEY " : (ColTableName.ToUpper() == "C" ? " NOT NULL UNIQUE " : "")) + ",\n";
                            break;
                        case "nvarchar":
                            CreateTableMSSQL = CreateTableMSSQL + "	[" + ColTableName + "] [" + ColTableType + "](" + ColTableLenght + ") " + (ColTableName.ToUpper() == "I" ? " NOT NULL PRIMARY KEY " : (ColTableName.ToUpper() == "C" ? " NOT NULL UNIQUE " : "")) + ",\n";
                            CreateTableOracle = CreateTableOracle + "	" + ColTableName + " NVARCHAR2 (" + ColTableLenght + ") " + (ColTableName.ToUpper() == "I" ? " NOT NULL PRIMARY KEY " : (ColTableName.ToUpper() == "C" ? " NOT NULL UNIQUE " : "")) + ",\n";
                            break;
                        default:
                            CreateTableMSSQL = CreateTableMSSQL + "	[" + ColTableName + "] [" + ColTableType + "],\n";
                            CreateTableOracle = CreateTableOracle + "	" + ColTableName + " " + ColTableType + ",\n";
                            break;
                    }
                    //Create Store Edit-Add-List
                    string ColList = _context.GetFormValue("ColList" + i.ToString());
                    string ColListType = _context.GetFormValue("ColListType" + i.ToString());

                    switch (ColListType.ToLower())
                    {
                        case "bigint":
                            if (ColTableName.ToUpper() != "I" && InputFormFillter != "")
                            {
                                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ColList + " [" + ColListType + "] \n";
                                StoreListParamOracle = StoreListParamOracle + "	p_" + ColList + " IN NUMERIC, \n";
                                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ColList + " IN NUMERIC(19) := p_" + ColList + ";\n";
                            }
                            StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@" + ColList + " [" + ColListType + "],\n";
                            StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_" + ColList + " IN NUMERIC,\n";
                            StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ColList + " NUMERIC (19) :=p_" + ColList + ";\n";
                            StoreListTableMSSQL = StoreListTableMSSQL + "	[" + ColList + "] [" + ColListType + "],\n";
                            StoreListTableOracle = StoreListTableOracle + "	" + ColList + " NUMERIC (19),\n";

                            StoreListTableColMSSQL = StoreListTableColMSSQL + "C.[" + ColTableName + "], ";
                            StoreListTableColOracle = StoreListTableColOracle + "C." + ColTableName + ", ";
                            // Field list
                            ListFieldInsert = ListFieldInsert + ColTableName + ", ";
                            if (ColTableName.ToUpper() != "I")
                            {
                                ListParamMSSQL = ListParamMSSQL + "@" + ColList + ", ";
                                ListParamOracle = ListParamOracle + "v_" + ColList + ", ";
                                ListFieldUpdateOracle = ListFieldUpdateOracle + ColTableName + " = " + "v_" + ColList + ", ";
                                ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[" + ColTableName + "] = " + "@" + ColList + ", ";
                            }
                            break;
                        case "int":
                            if (ColTableName.ToUpper() != "I" && InputFormFillter != "")
                            {
                                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ColList + " [" + ColListType + "] \n";
                                StoreListParamOracle = StoreListParamOracle + "	p_" + ColList + " IN NUMERIC, \n";
                                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ColList + " IN NUMERIC(10) := p_" + ColList + ";\n";
                            }
                            StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@" + ColList + " [" + ColListType + "],\n";
                            StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_" + ColList + " IN NUMERIC,\n";
                            StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ColList + " NUMERIC (10) :=p_" + ColList + ";\n";
                            StoreListTableMSSQL = StoreListTableMSSQL + "	[" + ColList + "] [" + ColListType + "],\n";
                            StoreListTableOracle = StoreListTableOracle + "	" + ColList + " NUMERIC (10),\n";

                            StoreListTableColMSSQL = StoreListTableColMSSQL + "C.[" + ColTableName + "], ";
                            StoreListTableColOracle = StoreListTableColOracle + "C." + ColTableName + ", ";
                            // Field list
                            ListFieldInsert = ListFieldInsert + ColTableName + ", ";
                            if (ColTableName.ToUpper() != "I")
                            {
                                ListParamMSSQL = ListParamMSSQL + "@" + ColList + ", ";
                                ListParamOracle = ListParamOracle + "v_" + ColList + ", ";
                                ListFieldUpdateOracle = ListFieldUpdateOracle + ColTableName + " = " + "v_" + ColList + ", ";
                                ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[" + ColTableName + "] = " + "@" + ColList + ", ";
                            }
                            break;
                        case "smallint":
                            if (ColTableName.ToUpper() != "I" && InputFormFillter != "")
                            {
                                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ColList + " [" + ColListType + "] \n";
                                StoreListParamOracle = StoreListParamOracle + "	p_" + ColList + " IN NUMERIC, \n";
                                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ColList + " IN NUMERIC(5) := p_" + ColList + ";\n";
                            }
                            StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@" + ColList + " [" + ColListType + "],\n";
                            StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_" + ColList + " IN NUMERIC,\n";
                            StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ColList + " NUMERIC (5) :=p_" + ColList + ";\n";
                            StoreListTableMSSQL = StoreListTableMSSQL + "	[" + ColList + "] [" + ColListType + "],\n";
                            StoreListTableOracle = StoreListTableOracle + "	" + ColList + " NUMERIC (5),\n";

                            StoreListTableColMSSQL = StoreListTableColMSSQL + "C.[" + ColTableName + "], ";
                            StoreListTableColOracle = StoreListTableColOracle + "C." + ColTableName + ", ";
                            // Field list
                            ListFieldInsert = ListFieldInsert + ColTableName + ", ";
                            if (ColTableName.ToUpper() != "I")
                            {
                                ListParamMSSQL = ListParamMSSQL + "@" + ColList + ", ";
                                ListParamOracle = ListParamOracle + "v_" + ColList + ", ";
                                ListFieldUpdateOracle = ListFieldUpdateOracle + ColTableName + " = " + "v_" + ColList + ", ";
                                ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[" + ColTableName + "] = " + "@" + ColList + ", ";
                            }
                            break;
                        case "bit":
                            if (ColTableName.ToUpper() != "I" && InputFormFillter != "")
                            {
                                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ColList + " [" + ColListType + "] \n";
                                StoreListParamOracle = StoreListParamOracle + "	p_" + ColList + " IN NUMERIC, \n";
                                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ColList + " IN NUMERIC(1) := p_" + ColList + ";\n";
                            }
                            StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@" + ColList + " [" + ColListType + "],\n";
                            StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_" + ColList + " IN NUMERIC,\n";
                            StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ColList + " NUMERIC (1) :=p_" + ColList + ";\n";
                            StoreListTableMSSQL = StoreListTableMSSQL + "	[" + ColList + "] [" + ColListType + "],\n";
                            StoreListTableOracle = StoreListTableOracle + "	" + ColList + " NUMERIC (1),\n";

                            StoreListTableColMSSQL = StoreListTableColMSSQL + "C.[" + ColTableName + "], ";
                            StoreListTableColOracle = StoreListTableColOracle + "C." + ColTableName + ", ";
                            // Field list
                            ListFieldInsert = ListFieldInsert + ColTableName + ", ";
                            if (ColTableName.ToUpper() != "I")
                            {
                                ListParamMSSQL = ListParamMSSQL + "@" + ColList + ", ";
                                ListParamOracle = ListParamOracle + "v_" + ColList + ", ";
                                ListFieldUpdateOracle = ListFieldUpdateOracle + ColTableName + " = " + "v_" + ColList + ", ";
                                ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[" + ColTableName + "] = " + "@" + ColList + ", ";
                            }
                            break;
                        case "date":
                        case "datetime":
                            if (ColTableName.ToUpper() != "I" && InputFormFillter != "")
                            {
                                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ColList + " [" + ColListType + "] \n";
                                StoreListParamOracle = StoreListParamOracle + "	p_" + ColList + " IN DATE, \n";
                                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ColList + " IN DATE := p_" + ColList + ";\n";
                            }
                            StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@" + ColList + " [" + ColListType + "],\n";
                            StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_" + ColList + " IN DATE,\n";
                            StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ColList + " DATE :=p_" + ColList + ";\n";
                            StoreListTableMSSQL = StoreListTableMSSQL + "	[" + ColList + "] [" + ColListType + "],\n";
                            StoreListTableOracle = StoreListTableOracle + "	" + ColList + " DATE,\n";

                            StoreListTableColMSSQL = StoreListTableColMSSQL + "BOS.FN_ConvertNumberToDate(C.[" + ColTableName + "]), ";
                            StoreListTableColOracle = StoreListTableColOracle + "BOS.FN_ConvertNumberToDate(C." + ColTableName + "), ";
                            // Field list
                            ListFieldInsert = ListFieldInsert + ColTableName + ", ";
                            if (ColTableName.ToUpper() != "I")
                            {
                                ListParamMSSQL = ListParamMSSQL + "BOS.FN_ConvertNumberToDate(@" + ColList + "), ";
                                ListParamOracle = ListParamOracle + "BOS.FN_ConvertNumberToDate(v_" + ColList + "), ";
                                ListFieldUpdateOracle = ListFieldUpdateOracle + ColTableName + " = " + "BOS.FN_ConvertNumberToDate(v_" + ColList + "), ";
                                ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[" + ColTableName + "] = " + "BOS.FN_ConvertNumberToDate(@" + ColList + "), ";
                            }
                            break;
                        case "varchar":
                            if (ColTableName.ToUpper() != "I" && InputFormFillter != "")
                            {
                                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ColList + " [" + ColListType + "](" + ColTableLenght + ") \n";
                                StoreListParamOracle = StoreListParamOracle + "	p_" + ColList + " IN VARCHAR2, \n";
                                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ColList + " IN VARCHAR2(" + ColTableLenght + ") := p_" + ColList + ";\n";
                            }
                            StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@" + ColList + " [" + ColListType + "](" + ColTableLenght + "),\n";
                            StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_" + ColList + " IN VARCHAR2,\n";
                            StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ColList + " VARCHAR2 (" + ColTableLenght + ") :=p_" + ColList + ";\n";


                            StoreListTableMSSQL = StoreListTableMSSQL + "	[" + ColList + "] [" + ColListType + "](" + ColTableLenght + "),\n";
                            StoreListTableOracle = StoreListTableOracle + "	" + ColList + " VARCHAR2 (" + ColTableLenght + "),\n";

                            StoreListTableColMSSQL = StoreListTableColMSSQL + "C.[" + ColTableName + "], ";
                            StoreListTableColOracle = StoreListTableColOracle + "C." + ColTableName + ", ";
                            // Field list
                            ListFieldInsert = ListFieldInsert + ColTableName + ", ";
                            if (ColTableName.ToUpper() != "I")
                            {
                                ListParamMSSQL = ListParamMSSQL + "@" + ColList + ", ";
                                ListParamOracle = ListParamOracle + "v_" + ColList + ", ";
                                ListFieldUpdateOracle = ListFieldUpdateOracle + ColTableName + " = " + "v_" + ColList + ", ";
                                ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[" + ColTableName + "] = " + "@" + ColList + ", ";
                            }
                            break;
                        case "nvarchar":
                            if (ColTableName.ToUpper() != "I" && InputFormFillter != "")
                            {
                                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ColList + " [" + ColListType + "](" + ColTableLenght + ") \n";
                                StoreListParamOracle = StoreListParamOracle + "	p_" + ColList + " IN NVARCHAR2, \n";
                                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ColList + " NVARCHAR2(" + ColTableLenght + ") := p_" + ColList + ";\n";
                            }
                            if (IsMultiLang == "1" && ColList == ListTable + "Name")
                            {
                                StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@Vietnamese [" + ColListType + "](" + ColTableLenght + "),\n	@English [" + ColListType + "](" + ColTableLenght + "),\n";
                                StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_Vietnamese IN NVARCHAR2 ,\n	p_English IN NVARCHAR2 ,\n";
                                StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_Vietnamese NVARCHAR2 (" + ColTableLenght + ") :=p_Vietnamese;\n	v_English NVARCHAR2 (" + ColTableLenght + ") :=p_English;\n";
                            }
                            else
                            {
                                StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@" + ColList + " [" + ColListType + "](" + ColTableLenght + "),\n";
                                StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_" + ColList + " IN NVARCHAR2 ,\n";
                                StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ColList + " NVARCHAR2 (" + ColTableLenght + ") :=p_" + ColList + ";\n";
                            }

                            StoreListTableMSSQL = StoreListTableMSSQL + "	[" + ColList + "] [" + ColListType + "](" + ColTableLenght + "),\n";
                            StoreListTableOracle = StoreListTableOracle + "	" + ColList + " NVARCHAR2 (" + ColTableLenght + "),\n";

                            StoreListTableColMSSQL = StoreListTableColMSSQL + "C.[" + ColTableName + "], ";
                            StoreListTableColOracle = StoreListTableColOracle + "C." + ColTableName + ", ";
                            // Field list
                            ListFieldInsert = ListFieldInsert + ColTableName + ", ";
                            if (ColTableName.ToUpper() != "I")
                            {
                                ListParamMSSQL = ListParamMSSQL + "@" + ColList + ", ";
                                ListParamOracle = ListParamOracle + "v_" + ColList + ", ";
                                ListFieldUpdateOracle = ListFieldUpdateOracle + ColTableName + " = " + "v_" + ColList + ", ";
                                ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[" + ColTableName + "] = " + "@" + ColList + ", ";
                            }
                            break;
                        default:
                            if (ColTableName.ToUpper() != "I" && InputFormFillter != "")
                            {
                                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ColList + " [" + ColListType + "] \n";
                                StoreListParamOracle = StoreListParamOracle + "	p_" + ColList + " IN " + ColListType + ", \n";
                                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ColList + " IN " + ColListType + " := p_" + ColList + ";\n";
                            }
                            StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "	@" + ColList + " [" + ColListType + "],\n";
                            StoreAddEditParamOracle = StoreAddEditParamOracle + "	p_" + ColList + " IN " + ColListType + ",\n";
                            StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ColList + " " + ColListType + " :=p_" + ColList + ";\n";
                            StoreListTableMSSQL = StoreListTableMSSQL + "	[" + ColList + "] [" + ColListType + "],\n";
                            StoreListTableOracle = StoreListTableOracle + "	" + ColList + " " + ColListType + ",\n";

                            StoreListTableColMSSQL = StoreListTableColMSSQL + "C.[" + ColTableName + "], ";
                            StoreListTableColOracle = StoreListTableColOracle + "C." + ColTableName + ", ";
                            // Field list
                            ListFieldInsert = ListFieldInsert + ColTableName + ", ";
                            if (ColTableName.ToUpper() != "I")
                            {
                                ListParamMSSQL = ListParamMSSQL + "@" + ColList + ", ";
                                ListParamOracle = ListParamOracle + "v_" + ColList + ", ";
                                ListFieldUpdateOracle = ListFieldUpdateOracle + ColTableName + " = " + "v_" + ColList + ", ";
                                ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[" + ColTableName + "] = " + "@" + ColList + ", ";
                            }
                            break;
                    }

                    string TableParent = _context.GetFormValue("TableParent" + i.ToString());
                    if (ColTableName.ToUpper() == "PARENTID")
                    {
                        iParent = iParent + 1;
                        StoreListTableMSSQL = StoreListTableMSSQL + "	ParentName [nvarchar](200),\n";
                        StoreListTableOracle = StoreListTableOracle + "	ParentName nvarchar2(200),\n";
                        StoreListTableColMSSQL = StoreListTableColMSSQL + "'{' + P" + iParent + ".N + '}' ParentName, ";
                        StoreListTableColOracle = StoreListTableColOracle + "'{' || P" + iParent + ".N || '}' ParentName, ";
                        StoreListTableParentMSSQL = StoreListTableParentMSSQL + " LEFT JOIN [" + ListTable + "] P" + iParent + " ON P" + iParent + ".I = C.PARENTID ";
                        StoreListTableParentOracle = StoreListTableParentOracle + " LEFT JOIN " + ListTable + " P" + iParent + " ON P" + iParent + ".I = C.PARENTID ";
                        InsertParentTreeOracle = "		v_NameSpace := NVL(v_NameSpace || '&' || 'nbsp;&' || 'nbsp;&' || 'nbsp;', ' ') ;\n" +
                            "		v_Order:= NVL(v_Order || '\' || TO_CHAR(p_ResponseStatus), TO_CHAR(p_ResponseStatus)) ;\n" +
                            "		UPDATE " + ListTable + "\n" +
                            "		SET NameSpace = v_NameSpace, OrderBy = v_Order \n" +
                            "		WHERE  I = p_ResponseStatus; \n";
                        StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_C1 NVARCHAR2(30);\n" +
                            "	v_C2 NVARCHAR2(30);\n" +
                            "	v_ShemaName NVARCHAR2(30) := '" + ListSchema + "';  \n";
                        UpdateParentTreeOracle = "		v_NameSpace := NVL(v_NameSpace || '&' || 'nbsp;&' || 'nbsp;&' || 'nbsp;', ' ') ;\n" +
                            "		v_Order:= NVL(v_Order || '\' || TO_CHAR(v_" + TableParent + "ID), TO_CHAR(v_" + TableParent + "ID));\n" +
                            "		v_C2:= v_Order ;\n" +
                            "		SELECT OrderBy INTO v_C1 FROM " + TableParent + " WHERE I = v_" + TableParent + "ID; \n" +
                            "		SP_SUB__OrderBy_Update(p_C1 => v_C1,\n" +
                            "		p_C2 => v_C2,\n" +
                            "		p_ShemaName => v_ShemaName,\n" +
                            "		p_Table => v_Table) ; \n";
                        InsertParentTreeMSSQL = "		SET @NameSpace = ISNULL(@NameSpace + '&' + 'nbsp;&' + 'nbsp;&' + 'nbsp;', '') ;\n" +
                            "		SET @Order = ISNULL(@Order + '\' + CAST(@ResponseStatus AS nvarchar(20)), CAST(@ResponseStatus AS nvarchar(20))) ;\n" +
                            "		UPDATE " + ListTable + "\n" +
                            "		SET NameSpace = @NameSpace, OrderBy = @Order \n" +
                            "		WHERE  I = @ResponseStatus; \n";
                        UpdateParentTreeMSSQL = "		SET @NameSpace = ISNULL(@NameSpace + '&' + 'nbsp;&' + 'nbsp;&' + 'nbsp;', '') ;\n" +
                            "		SET @Order = ISNULL(@Order + '\' + CAST(@" + TableParent + "ID AS nvarchar(20)), CAST(@" + TableParent + "ID AS nvarchar(20)));\n" +
                            "		SET @C2 = @Order ;\n" +
                            "		SELECT @C1 = OrderBy FROM " + TableParent + " WHERE I = @" + TableParent + "ID; \n" +
                            "		SP_SUB__OrderBy_Update(@C1 = @C1,\n" +
                            "		@C2 = @C2,\n" +
                            "		@ShemaName = @ShemaName,\n" +
                            "		@Table = @Table) ; \n";
                    }
                    if (TableParent != "")
                    {
                        iParent = iParent + 1;
                        StoreListTableMSSQL = StoreListTableMSSQL + "	" + TableParent + "Name [nvarchar](200),\n";
                        StoreListTableOracle = StoreListTableOracle + "	" + TableParent + "Name nvarchar2(200),\n";
                        StoreListTableColMSSQL = StoreListTableColMSSQL + "'{' + P" + iParent + ".N + '}' " + TableParent + "Name, ";
                        StoreListTableColOracle = StoreListTableColOracle + "'{' + P" + iParent + ".N + '}' " + TableParent + "Name, ";
                        StoreListTableParentMSSQL = StoreListTableParentMSSQL + " LEFT JOIN [" + TableParent + "] P" + iParent + " ON P" + iParent + ".I = C." + ColTableName + " ";
                        StoreListTableParentOracle = StoreListTableParentOracle + " LEFT JOIN " + TableParent + " P" + iParent + " ON P" + iParent + ".I = C." + ColTableName + " ";
                    }

                    string jsonItems = "";
                    for (int j = 0; j < a.Length; j++)
                    {
                        jsonItems = jsonItems + ",\"" + a[j].ToUpper() + "\":\"" + (a[j].ToUpper() == "STT" ? (i + 1).ToString() : _context.GetFormValue(a[j] + i.ToString())) + "\"";
                    }
                    if (jsonItems != "")
                    {
                        jsonItems = Tools.RemoveFisrtChar(jsonItems);
                        json = json + ",{" + jsonItems + "}";
                    }
                }
                if (IsMultiLang == "1")
                {
                    StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_" + ListTable + "Name NVARCHAR2(200) :=' ';\n";
                    StoreListTableMSSQL = StoreListTableMSSQL + "	Vietnamese [nvarchar](200),\n	English [nvarchar](200),\n";
                    StoreListTableOracle = StoreListTableOracle + "	Vietnamese nvarchar2(200),\n	English nvarchar2(200),\n";
                    StoreListTableColMSSQL = StoreListTableColMSSQL + "Vi.N Vietnamese, En.N English, ";
                    StoreListTableColOracle = StoreListTableColOracle + "Vi.N Vietnamese, En.N English, ";
                    StoreListTableParentMSSQL = StoreListTableParentMSSQL + " LEFT JOIN BOS.[LanguageText] Vi ON Vi.C = C.N AND Vi.[Language]='vi'" +
                        " LEFT JOIN BOS.[LanguageText] En ON En.C = C.N AND En.[Language]='en' ";
                    StoreListTableParentOracle = StoreListTableParentOracle + " LEFT JOIN BOS.LanguageText Vi ON Vi.C = C.N AND UPPER(Vi.Language)='VI'" +
                        " LEFT JOIN BOS.LanguageText En ON En.C = C.N AND UPPER(En.Language)='EN' ";

                    InsertParentTreeOracle = "		v_" + ListTable + "Name := '" + ListTable.Substring(0, 4) + "' || TO_CHAR(p_ResponseStatus) ;\n" +
                            "		BOS.SP_CMS__LanguageText_UpdateTitle (\n" +
                            "		p_Creator => p_Creator,\n" +
                            "		p_RegDate => v_RegDate,\n" +
                            "		p_LanguageTextCode => v_" + ListTable + "Name,\n" +
                            "		p_LanguageTextName => p_Vietnamese,\n" +
                            "		p_Language => 'vi'\n" +
                            "       ); \n" +
                            "		BOS.SP_CMS__LanguageText_UpdateTitle (\n" +
                            "		p_Creator => p_Creator,\n" +
                            "		p_RegDate => v_RegDate,\n" +
                            "		p_LanguageTextCode => v_" + ListTable + "Name,\n" +
                            "		p_LanguageTextName => p_English,\n" +
                            "		p_Language => 'en'\n" +
                            "       ); \n";
                    UpdateParentTreeOracle = "		v_" + ListTable + "Name := '" + ListTable.Substring(0, 4) + "' || TO_CHAR(v_" + ListTable + "ID) ;\n" +
                            "		BOS.SP_CMS__LanguageText_UpdateTitle (\n" +
                            "		p_Creator => p_Creator,\n" +
                            "		p_RegDate => v_RegDate,\n" +
                            "		p_LanguageTextCode => v_" + ListTable + "Name,\n" +
                            "		p_LanguageTextName => p_Vietnamese,\n" +
                            "		p_Language => 'vi'\n" +
                            "       ); \n" +
                            "		BOS.SP_CMS__LanguageText_UpdateTitle (\n" +
                            "		p_Creator => p_Creator,\n" +
                            "		p_RegDate => v_RegDate,\n" +
                            "		p_LanguageTextCode => v_" + ListTable + "Name,\n" +
                            "		p_LanguageTextName => p_English,\n" +
                            "		p_Language => 'en'\n" +
                            "       ); \n";
                    InsertParentTreeMSSQL = InsertParentTreeMSSQL + "       SET @" + ListTable + "Name = '" + ListTable.Substring(0, 4) + "'+CAST(@I AS varchar(20))\n" +
                        "		EXEC [BOS].[SP_CMS__LanguageText_UpdateTitle] \n" +
                        "       @Creator = @Creator, \n" +
                        "       @RegDate = @RegDate, \n" +
                        "       @LanguageTextCode = @" + ListTable + "Name, \n" +
                        "       @LanguageTextName = @English, \n" +
                        "       @Language = 'en'\n" +
                        "		EXEC [BOS].[SP_CMS__LanguageText_UpdateTitle] \n" +
                        "       @Creator = @Creator, \n" +
                        "       @RegDate = @RegDate, \n" +
                        "       @LanguageTextCode = @" + ListTable + "Name, \n" +
                        "       @LanguageTextName = @Vietnamese, \n" +
                        "       @Language = 'vi'\n";
                    UpdateParentTreeMSSQL = UpdateParentTreeMSSQL + "       SET @" + ListTable + "Name = '" + ListTable.Substring(0, 4) + "'+CAST(@I AS varchar(20))\n" +
                        "		EXEC [BOS].[SP_CMS__LanguageText_UpdateTitle] \n" +
                        "       @Creator = @Creator, \n" +
                        "       @RegDate = @RegDate, \n" +
                        "       @LanguageTextCode = @" + ListTable + "Name, \n" +
                        "       @LanguageTextName = @English, \n" +
                        "       @Language = 'en'\n" +
                        "		EXEC [BOS].[SP_CMS__LanguageText_UpdateTitle] \n" +
                        "       @Creator = @Creator, \n" +
                        "       @RegDate = @RegDate, \n" +
                        "       @LanguageTextCode = @" + ListTable + "Name, \n" +
                        "       @LanguageTextName = @Vietnamese, \n" +
                        "       @Language = 'vi'\n";
                }
                // Json Data
                if (json != "") json = Tools.RemoveFisrtChar(json);
                dynamic d = JObject.Parse("{\"IsMultiLang\":\"" + IsMultiLang + "\", \"ColumnLoad\":{\"Items\":[" + json + "]}}");
                // Create table
                CreateTableMSSQL = CreateTableMSSQL + "	[SysU] [bigint] ,\n" +
                    "	[SysD] [bigint] ,\n" +
                    "	[SysV] [int] ,\n" +
                    "	[SysS] [int] ,\n";
                CreateTableOracle = CreateTableOracle + "	SysU NUMERIC(19) ,\n" +
                    "	SysD NUMERIC(19) ,\n" +
                    "	SysV NUMERIC(10) ,\n" +
                    "	SysS NUMERIC(10) \n";
                // Create table store list temp
                StoreListTableMSSQL = StoreListTableMSSQL + "	[CreatorID] [bigint] ,\n" +
                    "	[CreatorName] nvarchar(30) ,\n" +
                    "	[CreatorVersion] [int] ,\n" +
                    "	[" + ListTable + "IDList] nvarchar(200)\n";

                StoreListTableOracle = StoreListTableOracle + "	CreatorID NUMERIC(19) ,\n" +
                    "	CreatorName nvarchar2(30) ,\n" +
                    "	CreatorVersion NUMERIC(10), \n" +
                    "	" + ListTable + "IDList nvarchar2(200)\n";

                StoreListTableColMSSQL = StoreListTableColMSSQL + "C.[SysU], (SELECT [C] FROM BOS.Users WHERE [I] = C.[SysU]), C.[SysV], C.[I] [" + ListTable + "IDList] ";

                StoreListTableColOracle = StoreListTableColOracle + "C.SysU, (SELECT C FROM BOS.Users WHERE I = C.SysU), C.SysV, C.I " + ListTable + "IDList ";

                ListFieldUpdateOracle = ListFieldUpdateOracle + "SysV = SysV + 1"; ; ListFieldUpdateMSSQL = ListFieldUpdateMSSQL + "[SysV]=[SysV]+1";

                StoreListParamMSSQL = StoreListParamMSSQL + "	@" + ListTable + "ID bigint, \n" +
                    "	@Keyword nvarchar(100), \n" +
                    "	@Page int, \n" +
                    "	@PageSize int, \n" +
                    "	@Rowcount int OUTPUT \n";

                StoreListParamOracle = StoreListParamOracle + "	p_" + ListTable + "ID IN NUMERIC, \n" +
                    "	p_Keyword IN NVARCHAR2, \n" +
                    "	p_Page IN NUMERIC, \n" +
                    "	p_PageSize IN NUMERIC, \n" +
                    "	p_Rowcount OUT NUMERIC \n";

                StoreListVariableOracle = StoreListVariableOracle + "	v_" + ListTable + "ID NUMERIC(19) := p_" + ListTable + "ID; \n" +
                    "	v_Keyword NVARCHAR2(100) := p_Keyword; \n" +
                    "	v_Page NUMERIC(10) := p_Page; \n" +
                    "	v_PageSize NUMERIC(10) := p_PageSize;\n" +
                    "   v_cursor SYS_REFCURSOR; \n";
                // Store Edit-Add
                StoreAddEditParamMSSQL = StoreAddEditParamMSSQL + "		@Creator bigint,\n@Message nvarchar(300) OUTPUT,\n" +
                    "	@ResponseStatus INT OUTPUT \n";
                StoreAddEditParamOracle = StoreAddEditParamOracle + "   p_Creator IN NUMERIC,\n	p_Message OUT nvarchar2,\n" +
                    "	p_ResponseStatus OUT NUMERIC\n";
                StoreAddEditVariableOracle = StoreAddEditVariableOracle + "	v_Today DATE := SYSDATE;\n" +
                    "	v_RegDate NUMBER(19,0) := BOS.FN_ConvertDateToNumber(v_Today);\n" +
                    "	v_StoreName NVARCHAR2(200) := '" + ListSchema + ".SP_CMS__" + ListTable + "_Update';\n";
                // Store Delete
                StoreDeleteParamMSSQL = StoreDeleteParamMSSQL + "	@Creator bigint,\n" +
                    "	@" + ListTable + "IDList varchar(200),\n" +
                    "	@Message nvarchar(300) OUTPUT,\n" +
                    "	@ResponseStatus INT OUTPUT \n";
                StoreDeleteParamOracle = StoreDeleteParamOracle + "	p_Creator IN NUMERIC,\n" +
                    "	p_" + ListTable + "IDList IN varchar2,\n" +
                    "	p_Message OUT nvarchar2,\n" +
                    "	p_ResponseStatus OUT NUMERIC\n";
                StoreDeleteVariableOracle = StoreDeleteVariableOracle + "	v_" + ListTable + "IDList varchar2(200) := p_" + ListTable + "IDList; \n" +
                    "	v_SysD NUMBER(19,0); \n" +
                    "	v_StoreName NVARCHAR2(200) := 'BOS.SP_CMS__" + ListTable + "_Delete'; \n";

                // Zip file 
                string[] fFullName = new string[3];
                byte[] fileBytes = null;
                //public const string FolderRoot = @"\RootFolder\Json";
                DateTime da = DateTime.Now;
                string folderRoot = _context.AppConfig.FolderRoot + "\\" + FolderRoot + "\\" + ListSchema + "\\" + ListTable;
                _context.AppConfig.PathCreateDirectory(folderRoot);
                string fileNewName = folderRoot + "\\List" + da.ToString("yyyyMMddHHmmss") + ".json";
                fFullName[0] = folderRoot + "\\List.json";
                _context.AppConfig.FileRename(fFullName[0], fileNewName);
                _context.AppConfig.FileWrite(fFullName[0], d.ToString());
                //public const string FolderMSSQL = @"\MSSQL";
                fileNewName = folderRoot + "\\MSSQL" + da.ToString("yyyyMMddHHmmss") + ".sql";
                fFullName[1] = folderRoot + "\\MSSQL" + ".sql";
                _context.AppConfig.FileRename(fFullName[1], fileNewName);
                // Create table data
                _context.AppConfig.FileWrite(fFullName[1], "\nGO\n-- CREATE TABLE " + ListTable + "\n");
                _context.AppConfig.FileWrite(fFullName[1], "\nGO\nDROP TABLE " + ListSchema + "." + ListTable + ";\n");
                _context.AppConfig.FileWrite(fFullName[1], "\nGO\nCREATE TABLE " + ListSchema + "." + ListTable + " (\n");
                _context.AppConfig.FileWrite(fFullName[1], CreateTableMSSQL + "\n");
                _context.AppConfig.FileWrite(fFullName[1], ");\n\n");
                // Create table hist
                _context.AppConfig.FileWrite(fFullName[1], "-- CREATE TABLE " + ListTable + "_Hist\n");
                _context.AppConfig.FileWrite(fFullName[1], "\nGO\nDROP TABLE " + ListSchema + "." + ListTable + "_Hist;\n");
                _context.AppConfig.FileWrite(fFullName[1], "\nGO\nCREATE TABLE " + ListSchema + "." + ListTable + "_Hist (\n");
                _context.AppConfig.FileWrite(fFullName[1], "	ChangeU bigint, \n");
                _context.AppConfig.FileWrite(fFullName[1], "	ChangeD bigint, \n");
                _context.AppConfig.FileWrite(fFullName[1], "	ChangeText nvarchar (200), \n");
                _context.AppConfig.FileWrite(fFullName[1], CreateTableMSSQL.Replace("PRIMARY KEY", "").Replace("UNIQUE", "") + "\n");
                _context.AppConfig.FileWrite(fFullName[1], ");\n\n");
                // Create Store Add-Edit
                _context.AppConfig.FileWrite(fFullName[1], "\nGO\n-- CREATE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_Update \n");
                _context.AppConfig.FileWrite(fFullName[1], "CREATE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_Update \n");
                _context.AppConfig.FileWrite(fFullName[1], StoreAddEditParamMSSQL);
                _context.AppConfig.FileWrite(fFullName[1], "AS\n");
                _context.AppConfig.FileWrite(fFullName[1], "BEGIN\n --Gan Default?\n    ---------------\n   SET NOCOUNT,XACT_ABORT ON;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	DECLARE @RegDate bigint = BOS.FN_ConvertDateToNumber(GETDATE());\n");
                if (IsMultiLang == "1") _context.AppConfig.FileWrite(fFullName[1], "	DECLARE @" + ListTable + "Name nvarchar(200)\n");
                _context.AppConfig.FileWrite(fFullName[1], StoreAddEditParamCheckMSSQL + "\n");
                _context.AppConfig.FileWrite(fFullName[1], "	BEGIN TRY\n");
                _context.AppConfig.FileWrite(fFullName[1], "	BEGIN TRANSACTION\n");
                _context.AppConfig.FileWrite(fFullName[1], "	IF (@" + ListTable + "ID < 1 ) \n" +
                    "   BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[1], "		DECLARE @I bigint\n" +
                    "       EXEC[BOS].[SP_SUB__GetID] @I = @I OUTPUT\n" +
                    "		INSERT INTO " + ListTable + " (" + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS)\n" +
                    "		SELECT @I, " + ListParamMSSQL + " @Creator,@RegDate,1,NULL\n" +
                    "		SET @Message = N'AddnewIsSuccess' ;\n" +
                    "		SET @ResponseStatus = @I;\n" +
                    InsertParentTreeMSSQL);
                _context.AppConfig.FileWrite(fFullName[1], "	END\n ELSE\nBEGIN\n");
                _context.AppConfig.FileWrite(fFullName[1], "		INSERT INTO " + ListTable + "_Hist( ChangeU, ChangeD, ChangeText, " + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS)\n" +
                    "		SELECT @Creator, @RegDate, N'S\\1eeda', " + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS \n" +
                    "		FROM " + ListTable + " \n" +
                    "		WHERE  I = @" + ListTable + "ID;\n " +
                    UpdateParentTreeMSSQL + "\n" +
                    "		UPDATE " + ListTable + "\n" +
                    "		SET " + ListFieldUpdateMSSQL + "\n" +
                    "		WHERE  I = @" + ListTable + "ID;\n" +
                    "		SET @Message = N'UpdateIsSuccess' ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	END;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	COMMIT;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	END TRY\n");
                _context.AppConfig.FileWrite(fFullName[1], "	BEGIN CATCH\n");
                _context.AppConfig.FileWrite(fFullName[1], "		ROLLBACK;\n");
                _context.AppConfig.FileWrite(fFullName[1], "		SET @ResponseStatus = -99 ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "		SET @Message = N'DatabaseError' ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "		EXEC BOS.SP_SUB__LogError;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	END CATCH;\n");
                _context.AppConfig.FileWrite(fFullName[1], "END\n\n");
                // Create Store List
                _context.AppConfig.FileWrite(fFullName[1], "\nGO\n-- CREATE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_List \n");
                _context.AppConfig.FileWrite(fFullName[1], "CREATE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_List \n");
                _context.AppConfig.FileWrite(fFullName[1], StoreListParamMSSQL);
                _context.AppConfig.FileWrite(fFullName[1], "AS\n");
                _context.AppConfig.FileWrite(fFullName[1], "BEGIN\n --Gan Default?\n    ---------------\n   SET NOCOUNT,XACT_ABORT ON;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	SET @" + ListTable + "ID = ISNULL(@" + ListTable + "ID, 0);\n");
                _context.AppConfig.FileWrite(fFullName[1], "	SET @Keyword = ISNULL(@Keyword, '');\n");
                _context.AppConfig.FileWrite(fFullName[1], "	IF (@Keyword <> '') \n" +
                    "   BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[1], "		SET @Keyword = '%' + @Keyword + '%' ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	END;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	SET @Page = ISNULL(@Page, 1) ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	SET @PageSize = ISNULL(@PageSize, 100) ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	SET @Rowcount = 0 ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	-----------Get list\n");
                _context.AppConfig.FileWrite(fFullName[1], "	DECLARE @" + ListTable + "_Temp AS TABLE(\n" +
                    "	STT NUMERIC(10) NOT NULL PRIMARY KEY IDENTITY(1,1), \n" +
                    StoreListTableMSSQL + "\n" +
                    "   );\n");
                _context.AppConfig.FileWrite(fFullName[1], "	IF ( @" + ListTable + "ID > 0 ) \n" +
                    "   BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[1], "		INSERT INTO @" + ListTable + "_Temp\n");
                _context.AppConfig.FileWrite(fFullName[1], "		SELECT " + StoreListTableColMSSQL + "\n");
                _context.AppConfig.FileWrite(fFullName[1], "		FROM " + ListTable + " C " + StoreListTableParentMSSQL + "\n");
                _context.AppConfig.FileWrite(fFullName[1], "		WHERE C.I = @" + ListTable + "ID;\n");
                _context.AppConfig.FileWrite(fFullName[1], "		SET @Rowcount = @@ROWCOUNT ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	END\n" +
                    "   ELSE \n" +
                    "   BEGIN\n" +
                                                           "		IF ( @Keyword <> '') \n" +
                                                           "        BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[1], "			INSERT INTO @" + ListTable + "_Temp\n");
                _context.AppConfig.FileWrite(fFullName[1], "			SELECT " + StoreListTableColMSSQL + "\n");
                _context.AppConfig.FileWrite(fFullName[1], "			FROM " + ListTable + " C " + StoreListTableParentMSSQL + "\n");
                _context.AppConfig.FileWrite(fFullName[1], "			WHERE (C.C LIKE @Keyword OR C.N LIKE @Keyword );\n");
                _context.AppConfig.FileWrite(fFullName[1], "			SET @Rowcount = @@ROWCOUNT ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "		END\n" +
                    "       ELSE \n" +
                    "       BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[1], "			INSERT INTO @" + ListTable + "_Temp\n");
                _context.AppConfig.FileWrite(fFullName[1], "			SELECT " + StoreListTableColMSSQL + "\n");
                _context.AppConfig.FileWrite(fFullName[1], "			FROM " + ListTable + " C " + StoreListTableParentMSSQL + ";\n");
                _context.AppConfig.FileWrite(fFullName[1], "			--WHERE C.I = @" + ListTable + "ID;\n");
                _context.AppConfig.FileWrite(fFullName[1], "			SET @Rowcount = @@ROWCOUNT ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "		END;--v_Keyword\n");
                _context.AppConfig.FileWrite(fFullName[1], "	END;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	SELECT * FROM @" + ListTable + "_Temp \n");
                _context.AppConfig.FileWrite(fFullName[1], "	WHERE STT BETWEEN (((@Page - 1) * @PageSize) + 1) AND (@Page * @PageSize);\n");
                _context.AppConfig.FileWrite(fFullName[1], "END\n");

                // Create Store Delete
                _context.AppConfig.FileWrite(fFullName[1], "\nGO\n-- CREATE OR REPLACE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_Delete \n");
                _context.AppConfig.FileWrite(fFullName[1], "CREATE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_Delete \n");
                _context.AppConfig.FileWrite(fFullName[1], StoreDeleteParamMSSQL + "\n"); // Create store
                _context.AppConfig.FileWrite(fFullName[1], "AS\n");
                _context.AppConfig.FileWrite(fFullName[1], "BEGIN" +
                    "\n SET NOCOUNT,XACT_ABORT ON;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	DECLARE @SysD bigint;\n");
                _context.AppConfig.FileWrite(fFullName[1], "	SET @SysD = BOS.FN_ConvertDateToNumber(GETDATE()) ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "    SET @" + ListTable + "IDList = ISNULL(@" + ListTable + "IDList, '') ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "    IF (@" + ListTable + "IDList = '' ) \n" +
                    "   BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[1], "        SET @Message = N'" + ListTable + "IDListIsNotNull' ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "        SET @ResponseStatus = -600 ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "        RETURN;\n");
                _context.AppConfig.FileWrite(fFullName[1], "    END;\n");
                if (IsTyped)
                {
                    _context.AppConfig.FileWrite(fFullName[1], "    SET @Type = ISNULL(@Type, 0) ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    IF (@Type NOT IN (-1,0,1)) \n" +
                        "   BEGIN\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        SET @Message = N'RequestActionIsWrong' ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        SET @ResponseStatus = -600 ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        RETURN;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    END;\n");
                }
                _context.AppConfig.FileWrite(fFullName[1], "    BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[1], "        -- Bam chuoi ID\n");
                _context.AppConfig.FileWrite(fFullName[1], "        DECLARE @T AS TABLE ([ID] bigint)" +
                    "       INSERT INTO @T (ID)\n");
                _context.AppConfig.FileWrite(fFullName[1], "        SELECT [Value] FROM BOS.FN_Parameters_Split (@" + ListTable + "IDList, ',')\n");
                _context.AppConfig.FileWrite(fFullName[1], "        -- Xoa ID co menu con\n");
                _context.AppConfig.FileWrite(fFullName[1], "        -- DELETE @T WHERE ID IN (SELECT ParentID FROM " + ListTable + ");\n");
                _context.AppConfig.FileWrite(fFullName[1], "        -- DELETE @T WHERE ID IN (SELECT ParentID FROM " + ListTable + ");\n");
                _context.AppConfig.FileWrite(fFullName[1], "        DELETE @T WHERE ID IN (SELECT I FROM " + ListTable + " WHERE  SysS IS NOT NULL);  \n");
                _context.AppConfig.FileWrite(fFullName[1], "    END;\n");
                _context.AppConfig.FileWrite(fFullName[1], "    BEGIN TRY\n");
                _context.AppConfig.FileWrite(fFullName[1], "    BEGIN TRANSACTION\n");
                _context.AppConfig.FileWrite(fFullName[1], "    INSERT INTO " + ListTable + "_Hist( ChangeU, ChangeD, ChangeText, " + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS )\n");
                _context.AppConfig.FileWrite(fFullName[1], "    SELECT @Creator, @SysD, N'X\\00f3a - Active - UnActive', " + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS\n");
                _context.AppConfig.FileWrite(fFullName[1], "    FROM " + ListTable + " \n");
                _context.AppConfig.FileWrite(fFullName[1], "    WHERE  I IN (SELECT ID FROM @T);\n");
                if (IsTyped)
                {
                    _context.AppConfig.FileWrite(fFullName[1], "    IF ( @Type = -1 ) \n" +
                        "   BEGIN\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        DELETE " + ListTable + " WHERE  I IN (SELECT ID FROM @T);\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        SET ResponseStatus = @@ROWCOUNT ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        IF ( @ResponseStatus < 1 ) \n");
                    _context.AppConfig.FileWrite(fFullName[1], "            SET @Message = N'" + ListTable + "IDListIsNotNull' ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[1], "            SET Message = N'DeleteIsSuccess' ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    END" +
                        "   ELSE IF ( @Type = 0 ) \n" +
                        "   BEGIN \n");
                    _context.AppConfig.FileWrite(fFullName[1], "        UPDATE " + ListTable + " SET Status = 0 WHERE  I IN (SELECT ID FROM @T);\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        SET @ResponseStatus = @@ROWCOUNT ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        IF ( @ResponseStatus < 1 ) \n");
                    _context.AppConfig.FileWrite(fFullName[1], "            SET @Message = N'" + ListTable + "IDListIsNotNull' ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[1], "            SET @Message = N'UnactiveIsSuccess' ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    END;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    ELSE\n" +
                        "   BEGIN\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        UPDATE " + ListTable + " SET Status = 1 WHERE  I IN (SELECT ID FROM @T);\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        SET @ResponseStatus = @@ROWCOUNT;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        IF ( @ResponseStatus < 1 ) THEN\n");
                    _context.AppConfig.FileWrite(fFullName[1], "            SET @Message = N'" + ListTable + "IDListIsNotNull' ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[1], "            SET @Message = N'ActiveIsSuccess' ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    END; \n");
                }
                else
                {
                    _context.AppConfig.FileWrite(fFullName[1], "    DELETE " + ListTable + " WHERE  I IN (SELECT ID FROM @T);\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    SET @ResponseStatus = @@ROWCOUNT ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    IF ( @ResponseStatus < 1 ) \n");
                    _context.AppConfig.FileWrite(fFullName[1], "        SET @Message = N'" + ListTable + "IDListIsNotNull' ;\n");
                    _context.AppConfig.FileWrite(fFullName[1], "    ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[1], "        SET @Message = N'DeleteIsSuccess' ;\n");
                }
                _context.AppConfig.FileWrite(fFullName[1], "    COMMIT;\n" +
                    "   END TRY\n");
                _context.AppConfig.FileWrite(fFullName[1], "    BEGIN CATCH\n");
                _context.AppConfig.FileWrite(fFullName[1], "        ROLLBACK;\n");
                _context.AppConfig.FileWrite(fFullName[1], "        SET @ResponseStatus = -99 ;\n");
                _context.AppConfig.FileWrite(fFullName[1], "        SET @Message = N'DatabaseError' ;   \n");
                _context.AppConfig.FileWrite(fFullName[1], "        EXEC BOS.SP_SUB__LogError;\n");
                _context.AppConfig.FileWrite(fFullName[1], "    END CATCH;\n");
                _context.AppConfig.FileWrite(fFullName[1], "END;\n\n");
                //public const string FolderOracle = @"\Oracle";
                fileNewName = folderRoot + "\\Oracle" + da.ToString("yyyyMMddHHmmss") + ".sql";
                fFullName[2] = folderRoot + "\\Oracle" + ".sql";
                _context.AppConfig.FileRename(fFullName[2], fileNewName);

                // Create table data
                _context.AppConfig.FileWrite(fFullName[2], "-- CREATE TABLE " + ListTable + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "DROP TABLE " + ListSchema + "." + ListTable + ";\n");
                _context.AppConfig.FileWrite(fFullName[2], "CREATE TABLE " + ListSchema + "." + ListTable + " (\n");
                _context.AppConfig.FileWrite(fFullName[2], CreateTableOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], ");\n\n");
                // Create table hist
                _context.AppConfig.FileWrite(fFullName[2], "-- CREATE TABLE " + ListTable + "_Hist\n");
                _context.AppConfig.FileWrite(fFullName[2], "DROP TABLE " + ListSchema + "." + ListTable + "_Hist;\n");
                _context.AppConfig.FileWrite(fFullName[2], "CREATE TABLE " + ListSchema + "." + ListTable + "_Hist (\n");
                _context.AppConfig.FileWrite(fFullName[2], "	ChangeU NUMERIC(19), \n");
                _context.AppConfig.FileWrite(fFullName[2], "	ChangeD NUMERIC(19), \n");
                _context.AppConfig.FileWrite(fFullName[2], "	ChangeText nvarchar2 (200), \n");
                _context.AppConfig.FileWrite(fFullName[2], CreateTableOracle.Replace("PRIMARY KEY", "").Replace("UNIQUE", "") + "\n");
                _context.AppConfig.FileWrite(fFullName[2], ");\n\n");
                // Create table tempList
                _context.AppConfig.FileWrite(fFullName[2], "-- CREATE TABLE " + ListTable + "_Temp\n");
                _context.AppConfig.FileWrite(fFullName[2], "DROP TABLE " + ListSchema + "." + ListTable + "_Temp;\n");
                _context.AppConfig.FileWrite(fFullName[2], "CREATE TABLE " + ListSchema + "." + ListTable + "_Temp (\n");
                _context.AppConfig.FileWrite(fFullName[2], "	STT NUMERIC(10) NOT NULL PRIMARY KEY, \n");
                _context.AppConfig.FileWrite(fFullName[2], StoreListTableOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], ");\n\n");
                // Create Store Add-Edit
                _context.AppConfig.FileWrite(fFullName[2], "-- CREATE OR REPLACE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_Update \n");
                _context.AppConfig.FileWrite(fFullName[2], "CREATE OR REPLACE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_Update \n(\n");
                _context.AppConfig.FileWrite(fFullName[2], StoreAddEditParamOracle);
                _context.AppConfig.FileWrite(fFullName[2], ")\nAS\n");
                _context.AppConfig.FileWrite(fFullName[2], StoreAddEditVariableOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "BEGIN\n --Gan Default?\n    ---------------\n");
                _context.AppConfig.FileWrite(fFullName[2], StoreAddEditParamCheckOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "	IF ( v_" + ListTable + "ID < 1 ) THEN\n");
                _context.AppConfig.FileWrite(fFullName[2], "		SELECT BOS.BASETAB_SEQ.NEXTVAL INTO p_ResponseStatus FROM DUAL;\n" +
                    "		INSERT INTO " + ListTable + " (" + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS)\n" +
                    "		SELECT p_ResponseStatus, " + ListParamOracle + " p_Creator,v_RegDate,1,NULL FROM DUAL;\n" +
                    "		p_Message := u'AddnewIsSuccess' ;\n" +
                    InsertParentTreeOracle);
                _context.AppConfig.FileWrite(fFullName[2], "	ELSE\n");
                _context.AppConfig.FileWrite(fFullName[2], "		INSERT INTO " + ListTable + "_Hist( ChangeU, ChangeD, ChangeText, " + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS)\n" +
                    "		SELECT p_Creator, v_RegDate, u'S\\1eeda', " + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS \n" +
                    "		FROM " + ListTable + " \n" +
                    "		WHERE  I = v_" + ListTable + "ID;\n " +
                    UpdateParentTreeOracle + "\n" +
                    "		UPDATE " + ListTable + "\n" +
                    "		SET " + ListFieldUpdateOracle + "\n" +
                    "		WHERE  I = v_" + ListTable + "ID;\n" +
                     "		p_Message := u'UpdateIsSuccess' ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	END IF;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	COMMIT;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	EXCEPTION WHEN OTHERS THEN\n");
                _context.AppConfig.FileWrite(fFullName[2], "	BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[2], "		ROLLBACK;\n");
                _context.AppConfig.FileWrite(fFullName[2], "		p_ResponseStatus := -99 ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "		p_Message := u'DatabaseError' ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "		BOS.SP_SUB__LogError(v_StoreName => v_StoreName);\n");
                _context.AppConfig.FileWrite(fFullName[2], "	END;\n");
                _context.AppConfig.FileWrite(fFullName[2], "END SP_CMS__" + ListTable + "_Update;\n\n");
                // Create Store List
                _context.AppConfig.FileWrite(fFullName[2], "-- CREATE OR REPLACE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_List \n");
                _context.AppConfig.FileWrite(fFullName[2], "CREATE OR REPLACE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_List \n(\n");
                _context.AppConfig.FileWrite(fFullName[2], StoreListParamOracle);
                _context.AppConfig.FileWrite(fFullName[2], ")\nAS\n");
                _context.AppConfig.FileWrite(fFullName[2], StoreListVariableOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "BEGIN\n --Gan Default?\n    ---------------\n");
                _context.AppConfig.FileWrite(fFullName[2], "	v_" + ListTable + "ID := NVL(v_" + ListTable + "ID, 0);\n");
                _context.AppConfig.FileWrite(fFullName[2], "	v_Keyword := NVL(v_Keyword, ' ');\n");
                _context.AppConfig.FileWrite(fFullName[2], "	IF (v_Keyword <> ' ') THEN\n");
                _context.AppConfig.FileWrite(fFullName[2], "		v_Keyword := '%' || UPPER(v_Keyword) || '%' ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	END IF;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	v_Page := NVL(v_Page, 1) ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	v_PageSize := NVL(v_PageSize, 10) ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	p_Rowcount := 0 ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	-----------Get list\n");
                _context.AppConfig.FileWrite(fFullName[2], "	DELETE FROM " + ListTable + "_Temp;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	IF ( v_" + ListTable + "ID > 0 ) THEN\n");
                _context.AppConfig.FileWrite(fFullName[2], "		INSERT INTO " + ListTable + "_Temp\n");
                _context.AppConfig.FileWrite(fFullName[2], "		SELECT ROWNUM, " + StoreListTableColOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "		FROM " + ListTable + " C " + StoreListTableParentOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "		WHERE C.I = v_" + ListTable + "ID;\n");
                _context.AppConfig.FileWrite(fFullName[2], "		p_Rowcount := SQL%ROWCOUNT ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	ELSE \n" +
                                                           "		IF ( v_Keyword <> ' ') THEN\n");
                _context.AppConfig.FileWrite(fFullName[2], "			INSERT INTO " + ListTable + "_Temp\n");
                _context.AppConfig.FileWrite(fFullName[2], "			SELECT ROWNUM, " + StoreListTableColOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "			FROM " + ListTable + " C " + StoreListTableParentOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "			WHERE ( UPPER(C.C) LIKE v_Keyword OR UPPER(C.N) LIKE v_Keyword );\n");
                _context.AppConfig.FileWrite(fFullName[2], "			p_Rowcount := SQL%ROWCOUNT ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "		ELSE \n");
                _context.AppConfig.FileWrite(fFullName[2], "			INSERT INTO " + ListTable + "_Temp\n");
                _context.AppConfig.FileWrite(fFullName[2], "			SELECT ROWNUM, " + StoreListTableColOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "			FROM " + ListTable + " C " + StoreListTableParentOracle + "\n");
                _context.AppConfig.FileWrite(fFullName[2], "			;--WHERE C.I = v_" + ListTable + "ID;\n");
                _context.AppConfig.FileWrite(fFullName[2], "			p_Rowcount := SQL%ROWCOUNT ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "		END IF;--v_Keyword\n");
                _context.AppConfig.FileWrite(fFullName[2], "	END IF;--v_" + ListTable + "ID\n");
                _context.AppConfig.FileWrite(fFullName[2], "	COMMIT;\n");
                _context.AppConfig.FileWrite(fFullName[2], "	OPEN  v_cursor FOR\n");
                _context.AppConfig.FileWrite(fFullName[2], "	SELECT * FROM " + ListTable + "_Temp \n");
                _context.AppConfig.FileWrite(fFullName[2], "	WHERE STT BETWEEN (((v_Page - 1) * v_PageSize) + 1) AND (v_Page * v_PageSize);\n");
                _context.AppConfig.FileWrite(fFullName[2], "	DBMS_SQL.RETURN_RESULT(v_cursor);\n");
                _context.AppConfig.FileWrite(fFullName[2], "END SP_CMS__" + ListTable + "_List;\n\n");

                // Create Store Delete
                _context.AppConfig.FileWrite(fFullName[2], "-- CREATE OR REPLACE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_Delete \n");
                _context.AppConfig.FileWrite(fFullName[2], "CREATE OR REPLACE PROCEDURE " + ListSchema + ".SP_CMS__" + ListTable + "_Delete \n(\n");
                _context.AppConfig.FileWrite(fFullName[2], StoreDeleteParamOracle + "\n"); // Create store
                _context.AppConfig.FileWrite(fFullName[2], ")\nAS\n");
                _context.AppConfig.FileWrite(fFullName[2], StoreDeleteVariableOracle + "\n" +
                    "BEGIN" +
                    "\n");
                _context.AppConfig.FileWrite(fFullName[2], "	v_SysD := BOS.FN_ConvertDateToNumber(SYSDATE) ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "    v_" + ListTable + "IDList := NVL(v_" + ListTable + "IDList, ' ') ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "    IF ( v_" + ListTable + "IDList = ' ' ) THEN\n");
                _context.AppConfig.FileWrite(fFullName[2], "        p_Message := u'" + ListTable + "IDListIsNotNull' ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "        p_ResponseStatus := -600 ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "        RETURN;\n");
                _context.AppConfig.FileWrite(fFullName[2], "    END IF;\n");
                if (IsTyped)
                {
                    _context.AppConfig.FileWrite(fFullName[2], "    v_Type := NVL(v_Type, 0) ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    IF (v_Type NOT IN (-1,0,1)) THEN\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        p_Message := u'RequestActionIsWrong' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        p_ResponseStatus := -600 ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        RETURN;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    END IF;\n");
                }
                _context.AppConfig.FileWrite(fFullName[2], "    BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[2], "        -- Bam chuoi ID\n");
                _context.AppConfig.FileWrite(fFullName[2], "        INSERT INTO BOS.tt_DeleteRows (ID)\n");
                _context.AppConfig.FileWrite(fFullName[2], "        SELECT TO_NUMBER(iValue) FROM (\n");
                _context.AppConfig.FileWrite(fFullName[2], "            SELECT level AS Iab, regexp_substr(v_" + ListTable + "IDList,'[^,]+', 1, level) AS iValue FROM DUAL\n");
                _context.AppConfig.FileWrite(fFullName[2], "            CONNECT BY regexp_substr(v_" + ListTable + "IDList, '[^,]+', 1, level) IS NOT NULL);\n");
                _context.AppConfig.FileWrite(fFullName[2], "        -- Xoa ID co menu con\n");
                _context.AppConfig.FileWrite(fFullName[2], "        -- DELETE BOS.tt_DeleteRows WHERE ID IN (SELECT ParentID FROM " + ListTable + ");\n");
                _context.AppConfig.FileWrite(fFullName[2], "        -- DELETE BOS.tt_DeleteRows WHERE ID IN (SELECT ParentID FROM " + ListTable + ");\n");
                _context.AppConfig.FileWrite(fFullName[2], "        DELETE BOS.tt_DeleteRows WHERE ID IN (SELECT I FROM " + ListTable + " WHERE  SysS IS NOT NULL);  \n");
                _context.AppConfig.FileWrite(fFullName[2], "        EXCEPTION WHEN OTHERS THEN\n");
                _context.AppConfig.FileWrite(fFullName[2], "        BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[2], "            p_Message := u'" + ListTable + "IDListIsNotNull' ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "            p_ResponseStatus := -600 ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "            RETURN;\n");
                _context.AppConfig.FileWrite(fFullName[2], "        END;\n");
                _context.AppConfig.FileWrite(fFullName[2], "    END;\n");
                _context.AppConfig.FileWrite(fFullName[2], "    INSERT INTO " + ListTable + "_Hist( ChangeU, ChangeD, ChangeText, " + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS )\n");
                _context.AppConfig.FileWrite(fFullName[2], "    SELECT p_Creator, v_SysD, u'X\\00f3a - Active - UnActive', " + ListFieldInsert + " SYSU,SYSD,SYSV,SYSS\n");
                _context.AppConfig.FileWrite(fFullName[2], "    FROM " + ListTable + " \n");
                _context.AppConfig.FileWrite(fFullName[2], "    WHERE  I IN (SELECT ID FROM BOS.tt_DeleteRows);\n");
                if (IsTyped)
                {
                    _context.AppConfig.FileWrite(fFullName[2], "    IF ( v_Type = -1 ) THEN\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        DELETE " + ListTable + " WHERE  I IN (SELECT ID FROM BOS.tt_DeleteRows);\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        p_ResponseStatus := SQL%ROWCOUNT ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        IF ( p_ResponseStatus < 1 ) THEN\n");
                    _context.AppConfig.FileWrite(fFullName[2], "            p_Message := u'" + ListTable + "IDListIsNotNull' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[2], "            p_Message := u'DeleteIsSuccess' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        END IF;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    ELSIF ( v_Type = 0 ) THEN \n");
                    _context.AppConfig.FileWrite(fFullName[2], "        UPDATE " + ListTable + " SET Status = 0 WHERE  I IN (SELECT ID FROM BOS.tt_DeleteRows);\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        p_ResponseStatus := SQL%ROWCOUNT ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        IF ( p_ResponseStatus < 1 ) THEN\n");
                    _context.AppConfig.FileWrite(fFullName[2], "            p_Message := u'" + ListTable + "IDListIsNotNull' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[2], "            p_Message := u'UnactiveIsSuccess' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        END IF;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        UPDATE " + ListTable + " SET Status = 1 WHERE  I IN (SELECT ID FROM BOS.tt_DeleteRows);\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        p_ResponseStatus := SQL%ROWCOUNT ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        IF ( p_ResponseStatus < 1 ) THEN\n");
                    _context.AppConfig.FileWrite(fFullName[2], "            p_Message := u'" + ListTable + "IDListIsNotNull' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[2], "            p_Message := u'ActiveIsSuccess' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        END IF;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    END IF; \n");
                }
                else
                {
                    _context.AppConfig.FileWrite(fFullName[2], "    DELETE " + ListTable + " WHERE  I IN (SELECT ID FROM BOS.tt_DeleteRows);\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    p_ResponseStatus := SQL%ROWCOUNT ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    IF ( p_ResponseStatus < 1 ) THEN\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        p_Message := u'" + ListTable + "IDListIsNotNull' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    ELSE\n");
                    _context.AppConfig.FileWrite(fFullName[2], "        p_Message := u'DeleteIsSuccess' ;\n");
                    _context.AppConfig.FileWrite(fFullName[2], "    END IF;\n");
                }
                _context.AppConfig.FileWrite(fFullName[2], "    COMMIT;\n");
                _context.AppConfig.FileWrite(fFullName[2], "    EXCEPTION WHEN OTHERS THEN \n");
                _context.AppConfig.FileWrite(fFullName[2], "    BEGIN\n");
                _context.AppConfig.FileWrite(fFullName[2], "        ROLLBACK;\n");
                _context.AppConfig.FileWrite(fFullName[2], "        p_ResponseStatus := -99 ;\n");
                _context.AppConfig.FileWrite(fFullName[2], "        p_Message := u'DatabaseError' ;   \n");
                _context.AppConfig.FileWrite(fFullName[2], "        BOS.SP_SUB__LogError(v_StoreName => v_StoreName);\n");
                _context.AppConfig.FileWrite(fFullName[2], "    END;\n");
                _context.AppConfig.FileWrite(fFullName[2], "END SP_CMS__" + ListTable + "_Delete;\n\n");

                fileBytes = Compress.ZipAddStream(fFullName);
                return File(fileBytes, "application/zip");
            }
        }
        #endregion

        #region Mã hóa giải mã - Nén/ giải nén
        [HttpGet]
        [HttpPost]
        public IActionResult ToolEnc() // Utils/ToolEnc
        {
            HRSContext _context = new HRSContext(HttpContext, _cache); ToolDAO bosDAO = new ToolDAO(_context);
            HTTP_CODE.WriteLogAction("functionName:/Utils/ToolEnc\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName"), _context);
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                string MenuID = _context.GetRequestVal("MenuID");if (MenuID == "") MenuID = "0";
                if (!_context.CheckPermistion(int.Parse(MenuID)))
                {
                    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
                }
                bool MenuOn = (_context.GetRequestMenuOn() != "Off");

                StringBuilder r1 = new StringBuilder(); string r = ""; string pass = ""; string en = ""; string de = ""; Algorithm enc;
                pass = _context.GetFormValue("PassPhrase");
                if (pass != "") enc = new Algorithm(pass); else enc = new Algorithm();
                
                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, MenuID) +
                    UIDef.UIContentTagOpen (ref bosDAO, _context, MenuOn, MenuID, false));

                // Encrypt
                en = _context.GetFormValue("PlainText");
                if (en != "") de = enc.Encrypt(en);
                r1.Append("<b>" + _context.GetLanguageLable("EncryptHeaderText") + "</b>" +
                    Environment.NewLine + "<form name=\"Encryptfrm\" method=\"POST\" action=\"/Utils/ToolEnc\">" +
                    Environment.NewLine + "<table class=\"table table-hover table-border flex\">" +
                    UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()) +
                    UIDef.UIHidden("MenuID", _context.GetRequestVal("MenuID")) +
                    Environment.NewLine + "<tr><td>" + _context.GetLanguageLable("PrivateKey") + "</td>" +
                    Environment.NewLine + "<td>: " + UIDef.UITextbox(_context.GetLanguageLable("PrivateKey"), "PassPhrase", pass, " size=150 autocomplete=\"off\"") + "</td></tr> " +
                    Environment.NewLine + "<tr><td>" + _context.GetLanguageLable("EncryptText") + "</td><td>: " + UIDef.UITextbox(_context.GetLanguageLable("EncryptText"), "PlainText", en, " size=150 autocomplete=\"off\"") + "</td></tr> " +
                    Environment.NewLine + "<tr><td>" + _context.GetLanguageLable("CipherText") + "</td><td>: " + de + "</td></tr> " +
                    Environment.NewLine + "<tr><td></td><td> " + UIDef.UIButton("bntSubmit", _context.GetLanguageLable("Encrypt"), true, " class=\"btn select\"") + " " + 
                    UIDef.UIButton("bntReset", _context.GetLanguageLable("Reset"), false, " class=\"btn refresh\"") + "</td></tr> " +
                    Environment.NewLine + "</table>" +
                    Environment.NewLine + "</form>");
                r = r1.ToString();
                r1 = null;
                ViewData["EncryptBody"] = r;
                // Decrypt
                r1 = new StringBuilder(); r = "";
                en = _context.GetFormValue("CipherText");
                if (en != "") de = enc.Decrypt(en);
                if (en == "") { en = de; de = ""; }
                r1.Append("<b>" + _context.GetLanguageLable("DecryptHeaderText") + "</b>" +
                    Environment.NewLine + "<form name=\"Decryptfrm\" method=\"POST\" action=\"/Utils/ToolEnc\">" +
                    Environment.NewLine + "<table class=\"table table-hover table-border flex\">" +
                    Environment.NewLine + UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()) +
                    Environment.NewLine + UIDef.UIHidden("MenuID", _context.GetRequestVal("MenuID")) +
                    Environment.NewLine + "<tr><td>" + _context.GetLanguageLable("PrivateKey") + "</td><td>: " + UIDef.UITextbox(_context.GetLanguageLable("PrivateKey"), "PassPhrase", pass, " size=150 autocomplete=\"off\"") + "</td></tr> " +
                    Environment.NewLine + "<tr><td>" + _context.GetLanguageLable("DecryptText") + "</td><td>: " + UIDef.UITextbox(_context.GetLanguageLable("DecryptText"), "CipherText", en, " size=150 autocomplete=\"off\"") + "</td></tr> " +
                    Environment.NewLine + "<tr><td>" + _context.GetLanguageLable("PlainText") + "</td><td>: " + de + "</td></tr> " +
                    Environment.NewLine + "<tr><td></td><td> " + UIDef.UIButton("bntSubmit", _context.GetLanguageLable("Decrypt"), true, " class=\"btn select\"") + " " + UIDef.UIButton("bntReset", _context.GetLanguageLable("Reset"), false, " class=\"btn refresh\"") + "</td></tr> " +
                    Environment.NewLine + "</table></form>" + UIDef.UIContentTagClose(_context, MenuOn, false));

                //r1.Append(UIDef.UIFooter());
                r = r1.ToString();
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
                ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn()=="Min"?" sidebar-collapse":"");
                ViewData["DecryptBody"] = r;
                ViewData["PageTitle"] = _context.GetLanguageLable("EncTitlePage");
                r1 = null;
                return View();
        }
    }
        [HttpGet]
        [HttpPost]
        public IActionResult ToolCompress() // Utils/ToolCompress
        {
            HRSContext _context = new HRSContext(HttpContext, _cache); ToolDAO bosDAO = new ToolDAO(_context);
            HTTP_CODE.WriteLogAction("functionName:/Utils/ToolCompress\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName"), _context);
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                if (!_context.CheckPermistion(int.Parse(MenuID)))
                {
                    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
                }
                bool MenuOn = (_context.GetRequestMenuOn() != "Off");

                StringBuilder r1 = new StringBuilder(); string r = ""; string Zip = ""; string UnZip = "";
                
                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, MenuID, false));

                // Encrypt
                Zip = _context.GetFormValue("ZipText");
                if (Zip != "") UnZip = Compress.Zip(Zip);
                r1.Append("<b>" + _context.GetLanguageLable("ZipHeaderText") + "</b>");
                r1.Append(Environment.NewLine + "<form name=\"Zipfrm\" method=\"POST\" action=\"/Utils/ToolCompress\">" +
                    Environment.NewLine + "<table class=\"table table-hover table-border flex\">" +
                    Environment.NewLine + UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()) +
                    Environment.NewLine + UIDef.UIHidden("MenuID", _context.GetRequestVal("MenuID")) +
                    Environment.NewLine + "<tr><td>" + _context.GetLanguageLable("ZipText") + "</td><td>: " + 
                    UIDef.UITextbox(_context.GetLanguageLable("ZipText"), "ZipText", Zip, " size=150 autocomplete=\"off\"") + "</td></tr> " +
                    Environment.NewLine + "<tr><td>" + _context.GetLanguageLable("UnZipText") + "</td><td>: " + UnZip + "</td></tr> " +
                    Environment.NewLine + "<tr><td></td><td> " + UIDef.UIButton("bntSubmit", _context.GetLanguageLable("Zip"), true, " class=\"btn select\"") + " " + 
                    UIDef.UIButton("bntReset", _context.GetLanguageLable("Reset"), false, " class=\"btn refresh\"") + "</td></tr> " +
                    Environment.NewLine + "</table>" +
                    Environment.NewLine + "</form>");
                r = r1.ToString();
                r1 = null;
                ViewData["ZipBody"] = r;
                // Decrypt
                r1 = new StringBuilder(); r = "";
                Zip = _context.GetFormValue("UnZipText");
                if (Zip != "") UnZip = Compress.UnZip(Zip);
                if (Zip == "") { Zip = UnZip; UnZip = ""; }
                r1.Append("<b>" + _context.GetLanguageLable("UnZipHeaderText") + "</b>");
                r1.Append("<form name=\"UnZipfrm\" method=\"POST\" action=\"/Utils/ToolCompress\"><table>");
                r1.Append(UIDef.UIHidden("MenuOn", _context.GetRequestMenuOn()));
                r1.Append(UIDef.UIHidden("MenuID", _context.GetRequestVal("MenuID")));
                r1.Append("<tr><td>" + _context.GetLanguageLable("UnZipText") + "</td><td>: " + UIDef.UITextbox(_context.GetLanguageLable("UnZipText"), "UnZipText", Zip, " size=150 autocomplete=\"off\"") + "</td></tr> ");
                r1.Append("<tr><td>" + _context.GetLanguageLable("ZipText") + "</td><td>: " + UnZip + "</td></tr> ");
                r1.Append("<tr><td></td><td> " + UIDef.UIButton("bntSubmit", _context.GetLanguageLable("UnZip"), true, " class=\"btn select\"") + " " + UIDef.UIButton("bntReset", _context.GetLanguageLable("Reset"), false, " class=\"btn refresh\"") + "</td></tr> ");
                r1.Append("</table></form>" + UIDef.UIContentTagClose(_context, MenuOn, false));

                //r1.Append(UIDef.UIFooter());
                r = r1.ToString();
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
                ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn()=="Min"?" sidebar-collapse":"");
                ViewData["UnZipBody"] = r;
                ViewData["PageTitle"] = _context.GetLanguageLable("CompressTitlePage");
                r1 = null;
                return View();
            }
        }
        #endregion

        #region UI Search
        [HttpGet]
        public IActionResult Search() // Utils/Search
        {
            string r = "";
            try
            {
                HRSContext _context = new HRSContext(HttpContext, _cache);
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS

                string SearchIndex = _context.GetRequestVal("SearchIndex");
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nSearchIndex:" + SearchIndex, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nLogin-fail: " + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName"), _context);
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    ToolDAO DataDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context);
                    StringBuilder r1 = new StringBuilder();

                    string jsColAdd = Tools.UrlDecode(_context.GetRequestVal("jsColAdd"));
                    string jsColAddVal = "";
                    string InputName = _context.GetRequestVal("InputName");
                    string[] sBox = SearchIndex.Split(new string[] { ";" }, StringSplitOptions.None);
                    string SPName; string sParam; string sParamLable; string sParamType; string sCol; string sColLable; string sColType; string UrlSearch;
                    string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = ""; dynamic d = null;
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nStart\nSearchIndex:" + SearchIndex, _context);
                    if (sBox.Length > 10)
                    {
                        SPName = sBox[4]; sParam = sBox[5]; sParamLable = sBox[6]; sParamType = sBox[7]; sCol = sBox[8]; sColLable = sBox[9]; sColType = sBox[10];
                        if (sBox.Length > 12)
                            UrlSearch = sBox[12];
                        else
                            UrlSearch = "/Utils/Search";
                        r1.Append("<!DOCTYPE html><html><head>" + UIDef.UIHeaderPage(_context.GetLanguageLable("SearchForm"), true) + "</head>");
                        r1.Append(UIDef.UIBodyTagOpen());
                        
                        r1.Append(UIDef.UIHeader(ref bosDAO, _context, false) +
                            UIDef.UIContentTagOpen(ref bosDAO, _context, false, "0", false) +
                            Environment.NewLine + "<div class=\"row\">" +
                            Environment.NewLine + "<div class=\"box\">" +
                            "<form name=\"SearchFrm\" method=\"GET\" action=\"" + UrlSearch + "\">" +
                            UIDef.UIHidden("InputName", InputName) +
                            UIDef.UIHidden("SearchIndex", Compress.Zip(SearchIndex)));//jsImage
                        string jsImage = _context.GetRequestVal("jsImage");
                        r1.Append(UIDef.UIHidden("jsImage", jsImage) +
                            UIDef.UIHidden("jsColAdd", Compress.Zip(jsColAdd)));
                        string[] a1; string[] a2; string[] a3; 
                        a1 = sParam.Split(new string[] { "||" }, StringSplitOptions.None);
                        a2 = sParamLable.Split(new string[] { "||" }, StringSplitOptions.None);
                        a3 = sParamType.Split(new string[] { "*" }, StringSplitOptions.None);
                        string[] a44 = new string[a1.Length];
                        StringBuilder jsdbcols; jsdbcols = new StringBuilder();
                        jsdbcols.Append("var jsdbcols = new Array();" +
                            Environment.NewLine + "var DataVal = new Array();" +
                            Environment.NewLine + "var DataTxt = new Array();" +
                            Environment.NewLine + "var DataParent = new Array();");
                        
                        ////bool IsFilterForm = false;
                        StringBuilder bx = new StringBuilder();
                        UIFormElements.UIFillterForm(ref bx, ref jsdbcols, ref json, _context, DataDAO, a1, a2, a3, a44, ":", true);
                        ////if (IsFilterForm)
                        ////{
                        ////    r1.Append(Environment.NewLine + "<div class=\"action\">");
                        ////    r1.Append(UIDef.UIButton("bntSearch", _context.GetLanguageLable("Search"),
                        ////        "var a=this.form.elements['Page'];if(a)a.value=1;btnSearch('');", " class=\"btn find\"") +
                        ////        UIDef.UIButton("bntSearchReset", _context.GetLanguageLable("Reset"), "btnReset(this.form);", " class=\"btn refresh\""));
                        ////    r1.Append(Environment.NewLine + "</div>");
                        ////}
                        r1.Append(bx.ToString()); bx = null;
                        r1.Append("</form>");//</table>
                        r1.Append(Environment.NewLine + "<script language=\"javascript\">" + jsdbcols.ToString() +
                            Tools.GenResetFunc("nextfocus") +
                            Tools.GenResetFunc("cnextfocus") +
                            Tools.GenResetFunc("btnsearch") +
                            Environment.NewLine + "</script>");
                        if (json != "")
                        {
                            json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                            d = JObject.Parse(json);
                        }
                        DataDAO.ExecuteStore("SearchBox", SPName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nExecuteStore: parameterOutput->" + parameterOutput, _context);
                        if (errorCode == HTTP_CODE.HTTP_ACCEPT || errorCode == HTTP_CODE.HTTP_NO_CONTENT)
                        {
                            r1.Append(Environment.NewLine + "<table class=\"table table-hover table-border flex\" width=\"100%\" cellspacing=\"1\" cellpadding=\"1\">");
                            a1 = sCol.Split(new string[] { "||" }, StringSplitOptions.None);
                            a2 = sColLable.Split(new string[] { "||" }, StringSplitOptions.None);
                            a3 = sColType.Split(new string[] { "*" }, StringSplitOptions.None);
                            d = JObject.Parse(json);
                            r1.Append("<thead class=\"thead-light\"><tr>");
                            for (int i = 0; i < a1.Length; i++)
                            {
                                string[] a4 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None);
                                if (Tools.Left(a4[0],1) != "-") r1.Append("<th nowrap>" + _context.GetLanguageLable(a2[i]) + "</th>");
                            }
                            r1.Append("</tr></thead>");
                            for (int i = 0; i < d.SearchBox.Items.Count; i++)
                            {
                                //if (i % 2 == 0)
                                r1.Append("<tr>");
                                //else
                                //    r1.Append("<tr>");
                                bool IsBlock = false; string Msg = "";
                                for (int j = 0; j < a1.Length; j++)
                                {
                                    try
                                    {
                                        if (Tools.GetDataJson(d.SearchBox.Items[i], "IsBlock") == "1") IsBlock = true;
                                        Msg = Tools.GetDataJson(d.SearchBox.Items[i], "Msg");
                                    }
                                    catch { }
                                    if (Msg == "") Msg = "ColumnIsReadOnly";
                                    string[] a4 = a3[j].Split(new string[] { ":" }, StringSplitOptions.None);
                                    string val = "";
                                    try { val = Tools.GetDataJson(d.SearchBox.Items[i], a1[j]); } catch { val = ""; }
                                    if (val == null) val = "";
                                    switch (a4[0])
                                    {
                                        case "":
                                            r1.Append("<td>" + val);
                                            break;
                                        case "HREF":
                                            string[] a = jsColAdd.Split(new string[] { "||" }, StringSplitOptions.None);
                                            try { jsColAddVal = Tools.GetDataJson(d.SearchBox.Items[i], a[0]); } catch { jsColAddVal = ""; }

                                            for (var il = 1; il < a.Length; il++)
                                            {
                                                string sa = "";
                                                try
                                                {
                                                    sa = Tools.GetDataJson(d.SearchBox.Items[i], a[il]);
                                                    int ik = Tools.GetArrayPos(a[il], a1);
                                                    if (ik > -1)
                                                    {
                                                        string[] sa4 = a3[ik].Split(new string[] { ":" }, StringSplitOptions.None);
                                                        switch (sa4[0])
                                                        {
                                                            case "-Date":
                                                            case "Date":
                                                                try { sa = (DateTime.Parse(sa)).ToString("dd/MM/yyyy"); } catch { sa = ""; }
                                                                break;
                                                            case "-Datetime":
                                                            case "Datetime":
                                                                try { sa = (DateTime.Parse(sa)).ToString("dd/MM/yyyy HH:mm"); } catch { sa = ""; }
                                                                break;
                                                            case "-Numeric":
                                                            case "Numeric":
                                                                try { sa = Tools.FormatNumber(sa); } catch { sa = ""; }
                                                                break;
                                                        }
                                                    }
                                                }
                                                catch { sa = ""; }
                                                jsColAddVal = jsColAddVal + "||" + sa;
                                            }
                                            if (jsImage != "")
                                            {
                                                string Img = Tools.GetDataJson(d.SearchBox.Items[i], jsImage);
                                                if (IsBlock)
                                                    r1.Append("<td><a id=\"a_s" + i + "\" href=\"javascript:JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable(Msg) + "', '', '0');;\">" + val + "</a>");
                                                else
                                                    r1.Append("<td><a id=\"a_s" + i + "\" href=\"javascript:abc('" + Tools.GetDataJson(d.SearchBox.Items[i], a1[j - int.Parse(a4[1])]) + "', '" + val + "', '" + Img + "', '" + jsColAdd + "', '" + jsColAddVal + "');\">" + val + "</a>");
                                            }
                                            else
                                            {
                                                if (IsBlock)
                                                    r1.Append("<td><a id=\"a_s" + i + "\" href=\"javascript:JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable(Msg) + "', '', '0');;\">" + val + "</a>");
                                                else
                                                    r1.Append("<td><a id=\"a_s" + i + "\" href=\"javascript:abc('" + Tools.GetDataJson(d.SearchBox.Items[i], a1[j - int.Parse(a4[1])]) + "', '" + val + "', '" + jsColAdd + "', '" + jsColAddVal + "');\">" + val + "</a>");
                                            }                                                
                                            break;
                                        case "Date":
                                            DateTime val1;
                                            try
                                            {
                                                val1 = DateTime.Parse(val);
                                                r1.Append("<td class=\"text-center\">" + val1.ToString("dd/MM/yyyy"));
                                            }
                                            catch
                                            {
                                                val1 = DateTime.Now;
                                                r1.Append("<td class=\"text-center\">" + val1.ToString("dd/MM/yyyy"));
                                            }
                                            break;
                                        case "Datetime":
                                            DateTime val2;
                                            try
                                            {
                                                val2 = DateTime.Parse(val);
                                                r1.Append("<td class=\"text-center\">" + val2.ToString("dd/MM/yyyy HH:mm:ss"));
                                            }
                                            catch
                                            {
                                                val2 = DateTime.Now;
                                                r1.Append("<td class=\"text-center\">" + val2.ToString("dd/MM/yyyy HH:mm:ss"));
                                            }
                                            break;
                                        case "Time":
                                            DateTime val3;
                                            try
                                            {
                                                val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                                r1.Append("<td class=\"text-center\">" + val3.ToString("HH:mm:ss"));
                                            }
                                            catch
                                            {
                                                val3 = DateTime.Now;
                                                r1.Append("<td class=\"text-center\">" + val3.ToString("HH:mm:ss"));
                                            }
                                            break;
                                        case "Numeric":
                                            if (val == null) val = "0";
                                            r1.Append("<td class=\"right\">" + Tools.FormatNumber(val));
                                            break;
                                        case "Checkbox":
                                            r1.Append("<td>" + UIDef.UICheckbox(a1[j], "", val, "1", "", false, i));
                                            break;
                                            //default:
                                            //    r1.Append("<td>" + val);
                                            //    break;
                                    }
                                }
                            }
                            r1.Append("</table></div></div>");
                            r1.Append(Environment.NewLine + "<script language='javascript'>" +
                                Environment.NewLine + "try{document.getElementById('a_s0').focus();}catch(ex){}" +
                                Tools.GenResetFunc() +
                                Environment.NewLine + "function abc(id, name" + (jsImage != "" ? "," + jsImage : "") + ", a, b){" +
                            Environment.NewLine + "var parent=window.opener;" +
                            Environment.NewLine + "if (parent==null) parent=dialogArguments;" +
                            Environment.NewLine + "parent.focus();" +
                            Environment.NewLine + "parent.SearchResult_" + InputName + "(id, name" + (jsImage != "" ? "," + jsImage : "") + ", a, b);" +
                            Environment.NewLine + "window.close();" +
                            Environment.NewLine + "}" +
                            Environment.NewLine + "</script>");
                        }
                    }
                    else
                    {
                        r1.Append("<script language='javascript'>" +
                            "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("ConfigSearchError") + "', '', '0');" +//alert('" + _context.GetLanguageLable("ConfigSearchError") + "');
                            "window.close();</script>");
                    }
                    r1.Append(UIDef.UIContentTagClose(_context, false, false) + UIDef.UIBodyTagClose("", _context.GetLanguageLable("Close")) + "</html>");
                    r = r1.ToString(); r1 = null;
                    //HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nEnd: " + r, _context);
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                r = ex.ToString();
            }
            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
        }
        public IActionResult SearchMulti() // Utils/SearchMulti
        {
            string r = "";
            try
            {
                HRSContext _context = new HRSContext(HttpContext, _cache);
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS

                string SearchIndex = _context.GetRequestVal("SearchIndex");
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nSearchIndex:" + SearchIndex, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nLogin-fail: " + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName"), _context);
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    ToolDAO DataDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context);
                    StringBuilder r1 = new StringBuilder();

                    string InputName = _context.GetRequestVal("InputName");
                    string IsRadio = _context.GetRequestVal("IsRadio");
                    string[] sBox = SearchIndex.Split(new string[] { ";" }, StringSplitOptions.None);
                    string SPName; string sParam; string sParamLable; string sParamType; string sCol; string sColLable; string sColType; string UrlSearch;
                    string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = ""; dynamic d = null;
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nStart\nSearchIndex:" + SearchIndex, _context);
                    if (sBox.Length > 10)
                    {
                        SPName = sBox[4]; sParam = sBox[5]; sParamLable = sBox[6]; sParamType = sBox[7]; sCol = sBox[8]; sColLable = sBox[9]; sColType = sBox[10];
                        if (sBox.Length > 12)
                            UrlSearch = sBox[12];
                        else
                            UrlSearch = "/Utils/SearchMulti";
                        r1.Append("<!DOCTYPE html><html><head>" + UIDef.UIHeaderPage(_context.GetLanguageLable("SearchForm"), true) + "</head>");
                        r1.Append(UIDef.UIBodyTagOpen());
                        
                        //r1.Append(UIDef.UIMenu(ref bosDAO, _context, false));
                        r1.Append(UIDef.UIHeader(ref bosDAO, _context, false));
                        r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, false, "0", false) + "<div class=\"row\">");
                        r1.Append("<form name=\"SearchFrm\" method=\"GET\" action=\"" + UrlSearch + "\">");
                        r1.Append(UIDef.UIHidden("InputName", InputName));
                        r1.Append(UIDef.UIHidden("SearchIndex", Compress.Zip(SearchIndex)));
                        string[] a1; string[] a2; string[] a3;
                        a1 = sParam.Split(new string[] { "||" }, StringSplitOptions.None);
                        a2 = sParamLable.Split(new string[] { "||" }, StringSplitOptions.None);
                        a3 = sParamType.Split(new string[] { "*" }, StringSplitOptions.None);
                        string[] a44 = new string[a1.Length];
                        StringBuilder jsdbcols; jsdbcols = new StringBuilder();
                        jsdbcols.Append("var jsdbcols = new Array();" +
                            Environment.NewLine + "var DataVal = new Array();" +
                            Environment.NewLine + "var DataTxt = new Array();" +
                            Environment.NewLine + "var DataParent = new Array();");
                        
                        ////bool IsFilterForm = false;
                        StringBuilder bx = new StringBuilder();
                        UIFormElements.UIFillterForm(ref bx, ref jsdbcols, ref json, _context, DataDAO, a1, a2, a3, a44, ":", true);
                        ////if (IsFilterForm)
                        ////{
                        ////    r1.Append(Environment.NewLine + "<div class=\"action no-margin-top\">");
                        ////    r1.Append(UIDef.UIButton("bntSearch", _context.GetLanguageLable("Search"),
                        ////        "var a=this.form.elements['Page'];if(a)a.value=1;btnSearch('');", " class=\"btn find\"") +
                        ////        UIDef.UIButton("bntSearchReset", _context.GetLanguageLable("Reset"), "btnReset(this.form);", " class=\"btn refresh\""));
                        ////    r1.Append(Environment.NewLine + "</div>");
                        ////}
                        r1.Append(bx.ToString()); bx = null;
                        r1.Append("</form>" +
                            Environment.NewLine + "<script language=\"javascript\">" + jsdbcols.ToString() +
                            Tools.GenResetFunc("nextfocus") +
                            Tools.GenResetFunc("cnextfocus") +
                            Tools.GenResetFunc("btnsearch") +
                            Environment.NewLine + "</script>");
                        if (json != "")
                        {
                            json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                            d = JObject.Parse(json);
                        }
                        DataDAO.ExecuteStore("SearchBox", SPName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nExecuteStore: parameterOutput->" + parameterOutput, _context);
                        if (errorCode == HTTP_CODE.HTTP_ACCEPT || errorCode == HTTP_CODE.HTTP_NO_CONTENT)
                        {
                            r1.Append(Environment.NewLine + "<form name=\"ListSearchFrm\">" +
                                UIDef.UIHidden("txtId", ""));
                            r1.Append(Environment.NewLine + "<div class=\"form-group row\">" +
                                UIDef.UIHrefButton(_context.GetLanguageLable("Choice"), "btnChoice();", "tvc-clipboard xanhla", "", "bntChoice") +
                                Environment.NewLine + "</div>" +
                                Environment.NewLine + "<div class=\"datatable\">" +
                                Environment.NewLine + "<table id=\"tbl_" + InputName + "Info\" class=\"table table-hover table-border flex\" width =\"100%\">");
                            a1 = sCol.Split(new string[] { "||" }, StringSplitOptions.None);
                            a2 = sColLable.Split(new string[] { "||" }, StringSplitOptions.None);
                            a3 = sColType.Split(new string[] { "*" }, StringSplitOptions.None);
                            d = JObject.Parse(json);
                            r1.Append("<thead id=\"theadId\" class=\"thead-light\"><tr>");
                            for (int i = 0; i < a1.Length; i++)
                            {
                                if (InputName == a1[i])
                                {
                                    r1.Append("<th>" + (IsRadio != "1"? UIDef.UICheckbox(a1[i] + "_ChkAll", "", "1", "1", "onclick=\"chkAll(this,'" + InputName + "');\"") + " <b>" + _context.GetLanguageLable("CHECKALL") : ""));
                                }
                                else
                                {
                                    string[] a4 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None);
                                    if (a4[0] != "-") r1.Append("<th nowrap><b>" + _context.GetLanguageLable(a2[i]) + "</b></td>");
                                }
                            }
                            r1.Append("<th></th>");
                            r1.Append("</tr></thead>");
                            bool IsBlock = false; string Msg = ""; 
                            for (int i = 0; i < d.SearchBox.Items.Count; i++)
                            {
                                string disabled = "";
                                try
                                {
                                    if (Tools.GetDataJson(d.SearchBox.Items[i], "IsBlock") == "1") IsBlock = true;
                                    Msg = Tools.GetDataJson(d.SearchBox.Items[i], "Msg");
                                } catch { }
                                if (Msg == "") Msg = "ColumnIsReadOnly";
                                if (i % 2 == 0)
                                    r1.Append("<tr class=\"basetabol\" id =\"tr" + i.ToString() + "\">");
                                else
                                    r1.Append("<tr id =\"tr" + i.ToString() + "\">");
                                if (IsBlock) disabled = " disabled ";
                                for (int j = 0; j < a1.Length; j++)
                                {
                                    string[] a4 = a3[j].Split(new string[] { ":" }, StringSplitOptions.None);
                                    string val = ""; 
                                    try { val = Tools.GetDataJson(d.SearchBox.Items[i], a1[j]); } catch { val = ""; }
                                    if (val == null) val = "";
                                    if (InputName == a1[j])
                                    {
                                        if (IsBlock)
                                        {
                                            r1.Append("<td>" + UIDef.UICheckbox(a1[j], "", val, "0", " disabled onclick=\"JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable(Msg) + "', '', '0');\"", false, i));
                                        }
                                        else
                                        {
                                            r1.Append("<td>" + UIDef.UICheckbox(a1[j], "", val, val, "onclick=\"chk('" + InputName + "');\"", false, i));
                                        }
                                    }
                                    else
                                        switch (a4[0])
                                        {
                                            case "HREF":
                                            case "":
                                                r1.Append("<td>" + val);
                                                break;
                                            case "Date":
                                            case "DateInput":
                                                DateTime val1;
                                                try
                                                {
                                                    val1 = DateTime.Parse(val);
                                                    if (a4[0] == "DateInput")
                                                    {
                                                        r1.Append("<td>" + UIDef.UIDate(_context.GetLanguageLable(a2[j]), a1[j], val1.ToString("dd/MM/yyyy"), disabled + " autocomplete=\"off\" ", UIDef.CNextFocusAction(a1[j]), "ListSearchFrm"));
                                                    }
                                                    else
                                                    {
                                                        r1.Append("<td class=\"text-center\">" + val1.ToString("dd/MM/yyyy"));
                                                    }                                                        
                                                }
                                                catch
                                                {
                                                    if (a4[0] == "DateInput")
                                                    {
                                                        r1.Append("<td>" + UIDef.UIDate(_context.GetLanguageLable(a2[j]), a1[j], "", disabled + " autocomplete=\"off\" ", UIDef.CNextFocusAction(a1[j]), "ListSearchFrm"));
                                                    }
                                                    else
                                                    {
                                                        r1.Append("<td class=\"text-center\">");
                                                    }                                                        
                                                }
                                                break;
                                            case "Datetime":
                                            case "DatetimeInput":
                                                DateTime val2;
                                                try
                                                {
                                                    val2 = DateTime.Parse(val);
                                                    if (a4[0] == "DatetimeInput")
                                                    {
                                                        r1.Append("<td>" + UIDef.UIDateTime(_context.GetLanguageLable(a2[j]), a1[j], val2.ToString("dd/MM/yyyy HH:mm"), disabled + " autocomplete=\"off\" ", UIDef.CNextFocusAction(a1[j]), "ListSearchFrm"));
                                                    }
                                                    else
                                                    {
                                                        r1.Append("<td class=\"text-center\">" + val2.ToString("dd/MM/yyyy HH:mm"));
                                                    }
                                                }
                                                catch
                                                {
                                                    val2 = DateTime.Now;
                                                    if (a4[0] == "DatetimeInput")
                                                    {
                                                        r1.Append("<td>" + UIDef.UIDateTime(_context.GetLanguageLable(a2[j]), a1[j], "", disabled + " autocomplete=\"off\" ", UIDef.CNextFocusAction(a1[j]), "ListSearchFrm"));
                                                    }
                                                    else
                                                    {
                                                        r1.Append("<td class=\"text-center\">");
                                                    }
                                                }
                                                break;
                                            case "Time":
                                                r1.Append("<td>" + val);
                                                break;
                                            case "Numeric":
                                            case "NumericInput":
                                                if (val == null) val = "0";
                                                if (a4[0] == "NumericInput")
                                                {
                                                    r1.Append("<td>" + UIDef.UINumeric(_context.GetLanguageLable(a2[j]), a1[j], val, disabled + " autocomplete=\"off\" ", "", ""));
                                                }
                                                else
                                                {
                                                    r1.Append("<td class=\"text-right\">" + Tools.FormatNumber(val));
                                                }                                                    
                                                break;
                                            case "Checkbox":
                                                r1.Append("<td>" + UIDef.UICheckbox(a1[j], "", val, "1", disabled + "", (IsRadio == "1"), i));
                                                break;
                                            default:
                                                //r1.Append("<td>" + val);
                                                break;
                                        }
                                }
                                r1.Append("<td>" + 
                                    UIDef.UIHrefButton(_context.GetLanguageLable("Delete"), "DeleteRow('tbl_" + InputName + "Info', document.getElementById('bntDelete" + i + "').parentNode.parentNode.rowIndex);", "fa fa-trash-o do", "", "bntDelete" + i));
                            }
                            r1.Append(Environment.NewLine + "</table>" +
                                Environment.NewLine + "<div class=\"form-group row\">" +
                                UIDef.UIHrefButton(_context.GetLanguageLable("Choice"), "btnChoice();", "tvc-clipboard xanhla", "", "bntChoice") +
                                Environment.NewLine + "</div>" +
                                Environment.NewLine + "</form>" +
                                Environment.NewLine + "</div>");
                            r1.Append(Environment.NewLine + "<script language='javascript'>" +
                                Tools.GenResetFunc() +
                                Environment.NewLine + "" +
                                Environment.NewLine + "function chkAll(t, c) {" +
                                Environment.NewLine + "  var txt = document.getElementById(\"txtId\");" +
                                Environment.NewLine + "  var input = document.ListSearchFrm.elements[c];" +
                                Environment.NewLine + "  txt.value = \"\";" +
                                Environment.NewLine + "  if (input){" +
                                Environment.NewLine + "      if(input.length){" +
                                Environment.NewLine + "          for(var i = 0; i < input.length; i++){" +
                                Environment.NewLine + "              if (t.checked==true && !input[i].disabled) {" +
                                Environment.NewLine + "                  input[i].checked = true; txt.value = txt.value + \"<tr>\" + input[i].parentNode.parentNode.parentNode.innerHTML;" +
                                Environment.NewLine + "              } else input[i].checked = false;" +
                                Environment.NewLine + "          }" +
                                Environment.NewLine + "      } else {" +
                                Environment.NewLine + "          if (t.checked==true && !input.disabled) {" +
                                Environment.NewLine + "              input.checked = true;" +
                                Environment.NewLine + "              txt.value = txt.value + \"<tr>\" + input.parentNode.parentNode.parentNode.innerHTML;" +
                                Environment.NewLine + "              } else " +
                                Environment.NewLine + "                  input.checked = false;" +
                                Environment.NewLine + "      }" +
                                Environment.NewLine + "  }" +
                                Environment.NewLine + "}" +
                                Environment.NewLine + "function chk(c) {" +
                                Environment.NewLine + "  var txt = document.getElementById(\"txtId\");" +
                                Environment.NewLine + "  var input = document.ListSearchFrm.elements[c];" +
                                Environment.NewLine + "  txt.value = \"\";" +
                                Environment.NewLine + "  if (input){" +
                                Environment.NewLine + "      //if(input.disabled) {JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("ColumnIsReadOnly") + "', '', '0'); return;};" +
                                Environment.NewLine + "      if(input.length){" +
                                Environment.NewLine + "          for(var i = 0; i < input.length; i++){" +
                                Environment.NewLine + "              if (input[i].checked==true && !input[i].disabled) txt.value = txt.value + \"<tr>\" + input[i].parentNode.parentNode.parentNode.innerHTML;" +
                                Environment.NewLine + "          }" +
                                Environment.NewLine + "      } else {" +
                                Environment.NewLine + "          if (input.checked==true && !input.disabled) txt.value = txt.value + \"<tr>\" + input.parentNode.parentNode.parentNode.innerHTML;" +
                                Environment.NewLine + "      }" +
                                Environment.NewLine + "  }" +
                                Environment.NewLine + "}" +
                                Environment.NewLine + "function btnChoice(){" +
                                Environment.NewLine + "chk('" + InputName + "'); var txt = document.getElementById(\"txtId\");" +
                                Environment.NewLine + "if (txt.value == \"\"){JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("DataIsNull") + "', '', '0'); return;}" +//alert('" + _context.GetLanguageLable("DataIsNull") + "');
                                Environment.NewLine + "var parent=window.opener;" +
                                Environment.NewLine + "if (parent==null) parent=dialogArguments;" +
                                Environment.NewLine + "parent.focus();" +
                                Environment.NewLine + "parent.SearchResult_" + InputName + "(document.getElementById(\"theadId\").innerHTML+txt.value);" +
                                Environment.NewLine + "window.close();}</script>");
                        }
                    }
                    else
                    {
                        r1.Append("<script language='javascript'>" +
                            "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("ConfigSearchError") + "', '', '0');" +//alert('" + _context.GetLanguageLable("ConfigSearchError") + "');
                            "window.close();</script>");
                    }
                    r1.Append("</div>" + UIDef.UIContentTagClose(_context, false, false) + UIDef.UIBodyTagClose("", _context.GetLanguageLable("Close")) + "</html>");
                    r = r1.ToString(); r1 = null;
                    //HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nEnd: " + r, _context);
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                r = ex.ToString();
            }
            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
        }
        public IActionResult SearchCountEmp() // Utils/SearchMulti
        {
            string r = "";
            try
            {
                HRSContext _context = new HRSContext(HttpContext, _cache);
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS

                string SearchIndex = _context.GetRequestVal("SearchIndex");
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nSearchIndex:" + SearchIndex, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nLogin-fail: " + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName"), _context);
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    ToolDAO DataDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context);
                    StringBuilder r1 = new StringBuilder();

                    string jsColAdd = Tools.UrlDecode(_context.GetRequestVal("jsColAdd"));
                    string jsColAddVal = "";
                    string InputName = _context.GetRequestVal("InputName");
                    string[] sBox = SearchIndex.Split(new string[] { ";" }, StringSplitOptions.None);
                    string SPName; string sParam; string sParamLable; string sParamType; string sCol; string sColLable; string sColType; string UrlSearch;
                    string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = ""; dynamic d = null;
                    HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nStart\nSearchIndex:" + SearchIndex, _context);
                    if (sBox.Length > 10)
                    {
                        SPName = sBox[4]; sParam = sBox[5]; sParamLable = sBox[6]; sParamType = sBox[7]; sCol = sBox[8]; sColLable = sBox[9]; sColType = sBox[10];
                        if (sBox.Length > 12)
                            UrlSearch = sBox[12];
                        else
                            UrlSearch = "/Utils/SearchMulti";
                        r1.Append("<!DOCTYPE html><html><head>" + UIDef.UIHeaderPage(_context.GetLanguageLable("SearchForm"), true) + "</head>");
                        r1.Append(UIDef.UIBodyTagOpen());
                        
                        //r1.Append(UIDef.UIMenu(ref bosDAO, _context, false));
                        r1.Append(UIDef.UIHeader(ref bosDAO, _context, false) +
                            UIDef.UIContentTagOpen(ref bosDAO, _context, false, "0", false) +
                            Environment.NewLine + "<div class=\"formsearch\">" +
                            Environment.NewLine + "<form name=\"SearchFrm\" method=\"GET\" action=\"" + UrlSearch + "\">" +
                            Environment.NewLine + "<table class=\"table table-hover table-border flex\">" +
                            UIDef.UIHidden("InputName", InputName) +
                            UIDef.UIHidden("SearchIndex", Compress.Zip(SearchIndex)));

                        string[] a1; string[] a2; string[] a3;
                        a1 = sParam.Split(new string[] { "||" }, StringSplitOptions.None);
                        a2 = sParamLable.Split(new string[] { "||" }, StringSplitOptions.None);
                        a3 = sParamType.Split(new string[] { "*" }, StringSplitOptions.None);
                        string[] a44 = new string[a1.Length];
                        StringBuilder jsdbcols; jsdbcols = new StringBuilder();
                        jsdbcols.Append("var jsdbcols = new Array();" +
                            Environment.NewLine + "var DataVal = new Array();" +
                            Environment.NewLine + "var DataTxt = new Array();" +
                            Environment.NewLine + "var DataParent = new Array();");
                        
                        ////bool IsFilterForm = false;
                        StringBuilder bx = new StringBuilder();
                        UIFormElements.UIFillterForm(ref bx, ref jsdbcols, ref json, _context, DataDAO, a1, a2, a3, a44, ":", true);
                        ////if (IsFilterForm)
                        ////{
                        ////    r1.Append(Environment.NewLine + "<div class=\"action no-margin-top\">");
                        ////    r1.Append(UIDef.UIButton("bntSearch", _context.GetLanguageLable("Search"),
                        ////        "var a=this.form.elements['Page'];if(a)a.value=1;btnSearch('');", " class=\"btn find\"") +
                        ////        UIDef.UIButton("bntSearchReset", _context.GetLanguageLable("Reset"), "btnReset(this.form);", " class=\"btn refresh\""));
                        ////    r1.Append(Environment.NewLine + "</div>");
                        ////}
                        r1.Append(bx.ToString()); bx = null;
                        r1.Append("</table></form>" +
                            Environment.NewLine + "<script language=\"javascript\">" + jsdbcols.ToString() +
                            Tools.GenResetFunc("nextfocus") +
                            Tools.GenResetFunc("cnextfocus") +
                            Tools.GenResetFunc("btnsearch") +
                            Environment.NewLine + "</script>");
                        if (json != "")
                        {
                            json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                            d = JObject.Parse(json);
                        }
                        DataDAO.ExecuteStore("SearchBox", SPName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nExecuteStore: parameterOutput->" + parameterOutput, _context);
                        if (errorCode == HTTP_CODE.HTTP_ACCEPT || errorCode == HTTP_CODE.HTTP_NO_CONTENT)
                        {
                            r1.Append("<form name=\"ListSearchFrm\">" +
                                UIDef.UIHidden("txtId", "") +
                                UIDef.UIHrefButton(_context.GetLanguageLable("Choice"), "btnChoice();", "tvc-clipboard xanhla", "", "bntChoice") +
                                "<table id=\"tbl_" + InputName + "Info\" width=\"100%\" class=\"table table-hover table-border flex\">");
                            a1 = sCol.Split(new string[] { "||" }, StringSplitOptions.None);
                            a2 = sColLable.Split(new string[] { "||" }, StringSplitOptions.None);
                            a3 = sColType.Split(new string[] { "*" }, StringSplitOptions.None);
                            d = JObject.Parse(json);
                            r1.Append("<thead id=\"theadId\"><tr class=\"gridhdr\">");
                            for (int i = 0; i < a1.Length; i++)
                            {
                                if (InputName == a1[i])
                                {
                                    r1.Append("<td>" + UIDef.UICheckbox(a1[i] + "_ChkAll", "", "1", "1", "onclick=\"chkAll(this,'" + InputName + "');\"") + " <b>" + _context.GetLanguageLable("CHECKALL"));
                                }
                                else
                                {
                                    string[] a4 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None);
                                    if (a4[0] != "-") r1.Append("<td align=\"center\" nowrap><b>" + _context.GetLanguageLable(a2[i]) + "</b></td>");
                                }

                            }
                            r1.Append("</tr></thead>");
                            bool IsBlock = false; string Msg = "";
                            for (int i = 0; i < d.SearchBox.Items.Count; i++)
                            {
                                try
                                {
                                    if (Tools.GetDataJson(d.SearchBox.Items[i], "IsBlock") == "1") IsBlock = true;
                                    Msg = Tools.GetDataJson(d.SearchBox.Items[i], "Msg");
                                }
                                catch { }
                                if (Msg == "") Msg = "ColumnIsReadOnly";
                                if (i % 2 == 0)
                                    r1.Append("<tr class=\"basetabol\">");
                                else
                                    r1.Append("<tr>");
                                for (int j = 0; j < a1.Length; j++)
                                {
                                    string[] a4 = a3[j].Split(new string[] { ":" }, StringSplitOptions.None);
                                    string val = "";
                                    try { val = Tools.GetDataJson(d.SearchBox.Items[i], a1[j]); } catch { val = ""; }
                                    if (val == null) val = "";
                                    if (InputName == a1[j])
                                    {
                                        string[] a = jsColAdd.Split(new string[] { "||" }, StringSplitOptions.None);
                                        try { jsColAddVal = Tools.GetDataJson(d.SearchBox.Items[i], a[0]); } catch { jsColAddVal = ""; }

                                        for (var il = 1; il < a.Length; il++)
                                        {
                                            string sa = "";
                                            try
                                            {
                                                sa = Tools.GetDataJson(d.SearchBox.Items[i], a[il]);
                                                int ik = Tools.GetArrayPos(a[il], a1);
                                                if (ik > -1)
                                                {
                                                    string[] sa4 = a3[ik].Split(new string[] { ":" }, StringSplitOptions.None);
                                                    switch (sa4[0])
                                                    {
                                                        case "Date":
                                                            try { sa = (DateTime.Parse(sa)).ToString("dd/MM/yyyy"); } catch { sa = ""; }
                                                            break;
                                                        case "Datetime":
                                                            try { sa = (DateTime.Parse(sa)).ToString("dd/MM/yyyy HH:mm"); } catch { sa = ""; }
                                                            break;
                                                        case "Numeric":
                                                            try { sa = Tools.FormatNumber(sa); } catch { sa = ""; }
                                                            break;
                                                    }
                                                }
                                            }
                                            catch { sa = ""; }
                                            jsColAddVal = jsColAddVal + "||" + sa;
                                        }
                                        if (IsBlock)
                                            r1.Append("<td>" + UIDef.UICheckbox(a1[j], "", jsColAddVal, "0", " disabled onclick=\"JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable(Msg) + "', '', '0');\"", false, i));
                                        else
                                            r1.Append("<td>" + UIDef.UICheckbox(a1[j], "", jsColAddVal, jsColAddVal, "onclick=\"chk('" + InputName + "');\"", false, i));
                                    }
                                    else
                                        switch (a4[0])
                                        {
                                            case "HREF":
                                            case "":
                                                r1.Append("<td>" + val);
                                                break;
                                            case "Date":
                                                DateTime val1;
                                                try
                                                {
                                                    val1 = DateTime.Parse(val);
                                                    r1.Append("<td align=center>" + val1.ToString("dd/MM/yyyy"));
                                                }
                                                catch
                                                {
                                                    val1 = DateTime.Now;
                                                    r1.Append("<td align=center>" + val1.ToString("dd/MM/yyyy"));
                                                }
                                                break;
                                            case "Datetime":
                                                DateTime val2;
                                                try
                                                {
                                                    val2 = DateTime.Parse(val);
                                                    r1.Append("<td align=center>" + val2.ToString("dd/MM/yyyy HH:mm:ss"));
                                                }
                                                catch
                                                {
                                                    val2 = DateTime.Now;
                                                    r1.Append("<td align=center>" + val2.ToString("dd/MM/yyyy HH:mm:ss"));
                                                }
                                                break;
                                            case "Time":
                                                DateTime val3;
                                                try
                                                {
                                                    val3 = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " " + val);
                                                    r1.Append("<td align=center>" + val3.ToString("HH:mm:ss"));
                                                }
                                                catch
                                                {
                                                    val3 = DateTime.Now;
                                                    r1.Append("<td align=center>" + val3.ToString("HH:mm:ss"));
                                                }
                                                break;
                                            case "Numeric":
                                                if (val == null) val = "0";
                                                r1.Append("<td align=right>" + Tools.FormatNumber(val));
                                                break;
                                            case "Checkbox":
                                                r1.Append("<td align=center>" + UIDef.UICheckbox(a1[j], "", val, "1", "", false, i));
                                                break;
                                                //default:
                                                //    r1.Append("<td>" + val);
                                                //    break;
                                        }
                                }
                                //r1.Append("<td>" + UIDef.UIButton("bntDelete", _context.GetLanguageLable("Delete"), "DeleteRow('tbl_" + InputName + "Info', this.parentNode.parentNode.rowIndex);"));
                            }
                            r1.Append("</table>" + 
                                UIDef.UIHrefButton(_context.GetLanguageLable("Choice"), "btnChoice();", "tvc-clipboard xanhla", "", "bntChoice") + 
                                "</form></div>");
                            r1.Append("<script language='javascript'" +
                                Tools.GenResetFunc() +
                                Environment.NewLine + ">" +
                                Environment.NewLine + "var StudentCount = 0; " +
                                Environment.NewLine + "var InputStr = '" + jsColAdd + "';" +
                                Environment.NewLine + "var InputArr = InputStr.split ('||');" +
                                Environment.NewLine + "function chkAll(t, c) {" +
                                Environment.NewLine + "	var txt = document.getElementById(\"txtId\");" +
                                Environment.NewLine + "	var input = document.ListSearchFrm.elements[c];" +
                                Environment.NewLine + "	txt.value = \"\";" +
                                Environment.NewLine + "	if (input){" +
                                Environment.NewLine + "		if(input.length){" +
                                Environment.NewLine + "			for(var i = 0; i < input.length; i++){" +
                                Environment.NewLine + "				if (t.checked==true && !input[i].disabled) {" +
                                Environment.NewLine + "					input[i].checked = true; " +
                                Environment.NewLine + "					txt.value = 1;" +
                                Environment.NewLine + "					chkSub (InputArr, input[i].value);" +
                                Environment.NewLine + "				} else input[i].checked = false;" +
                                Environment.NewLine + "			}" +
                                Environment.NewLine + "		} else {" +
                                Environment.NewLine + "			if (t.checked==true && !input.disabled) {" +
                                Environment.NewLine + "				input.checked = true;" +
                                Environment.NewLine + "				txt.value = 1;" +
                                Environment.NewLine + "				chkSub (InputArr, input[i].value);" +
                                Environment.NewLine + "			} else input.checked = false;" +
                                Environment.NewLine + "		}" +
                                Environment.NewLine + "	}" +
                                Environment.NewLine + "}" +
                                Environment.NewLine + "function chk(c) {" +
                                Environment.NewLine + "	var txt = document.getElementById(\"txtId\");" +
                                Environment.NewLine + "	var input = document.ListSearchFrm.elements[c];" +
                                Environment.NewLine + "	txt.value = \"\";" +
                                Environment.NewLine + "	chkReset (InputArr);" +
                                Environment.NewLine + "	if (input){" +
                                Environment.NewLine + "		if(input.length){" +
                                Environment.NewLine + "			for(var i = 0; i < input.length; i++){" +
                                Environment.NewLine + "				if (input[i].checked==true && !input[i].disabled) {" +
                                Environment.NewLine + "					txt.value = 1;" +
                                Environment.NewLine + "					chkSub (InputArr, input[i].value);" +
                                Environment.NewLine + "				}" +
                                Environment.NewLine + "			}" +
                                Environment.NewLine + "		} else {" +
                                Environment.NewLine + "			if (input.checked==true && !input.disabled) {" +
                                Environment.NewLine + "				txt.value = 1;" +
                                Environment.NewLine + "				chkSub (InputArr, input[i].value);" +
                                Environment.NewLine + "			}" +
                                Environment.NewLine + "		}" +
                                Environment.NewLine + "	}" +
                                Environment.NewLine + "}" +
                                Environment.NewLine + "function chkReset (InputArr){" +
                                Environment.NewLine + "	for (var i = 0; i < InputArr.length; i++)InputArr[i] = '';" +
                                Environment.NewLine + "	StudentCount = 0;" +
                                Environment.NewLine + "}" +
                                Environment.NewLine + "function StringRemoveLastChar (str){" +
                                Environment.NewLine + "	if(str != ''){str = str.substring(0, str.length - 1);str = str.replace(/;/g, '\\n');} return str;" +
                                Environment.NewLine + "}" +
                                Environment.NewLine + "function ArrayRemoveLastChar (InputArr){" +
                                Environment.NewLine + "	for (var i = 0; i < InputArr.length; i++)InputArr[i] = StringRemoveLastChar (InputArr[i]);" +
                                Environment.NewLine + "}" +
                                Environment.NewLine + "function chkSub (InputArr, val){" +
                                Environment.NewLine + "	var ValArr = val.split ('||');" +
                                Environment.NewLine + "	for (var i = 0; i < ValArr.length; i++){" +
                                Environment.NewLine + "		if (InputArr[i].indexOf(ValArr[i]) < 0)InputArr[i] = InputArr[i] + ValArr[i] + ';';" +
                                Environment.NewLine + "	}" +
                                Environment.NewLine + "	StudentCount = StudentCount +1;" +
                                Environment.NewLine + "}" +
                                Environment.NewLine + "function btnChoice(){" +
                                Environment.NewLine + "    chkReset (StudentCount, InputArr);" +
                                Environment.NewLine + "    chk('" + InputName + "'); " +
                                Environment.NewLine + "    var txt = document.getElementById(\"txtId\");" +
                                Environment.NewLine + "    if (txt.value == \"\"){" +
                                Environment.NewLine + "        JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("DataIsNull") + "', '', '0'); " +
                                Environment.NewLine + "        return;" +
                                Environment.NewLine + "    }" +//alert('" + _context.GetLanguageLable("DataIsNull") + "');
                                Environment.NewLine + "    ArrayRemoveLastChar (InputArr);//alert(StudentCount);" +
                                Environment.NewLine + "    //alert(InputStr);" +
                                Environment.NewLine + "    //JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', InputArr.join('||'), '', '0');" +
                                Environment.NewLine + "    var parent=window.opener;" +
                                Environment.NewLine + "    if (parent==null) parent=dialogArguments;" +
                                Environment.NewLine + "    parent.focus();" +
                                Environment.NewLine + "    parent.SearchResult_" + InputName + "(StudentCount, InputStr, InputArr.join('||'));" +
                                Environment.NewLine + "    window.close();" +
                                Environment.NewLine + "}</script>");
                        }
                    }
                    else
                    {
                        r1.Append("<script language='javascript'>" +
                            "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("ConfigSearchError") + "', '', '0');" +//alert('" + _context.GetLanguageLable("ConfigSearchError") + "');
                            "window.close();</script>");
                    }
                    r1.Append("</div>" + UIDef.UIContentTagClose(_context, false, false) + UIDef.UIBodyTagClose("", _context.GetLanguageLable("Close")) + "</html>");
                    r = r1.ToString(); r1 = null;
                    //HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nEnd: " + r, _context);
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                r = ex.ToString();
            }
            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
        }
        #endregion

        #region Custom
        public IActionResult Index() // Utils
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string FormName = _context.GetRequestVal("FormName");
                string StoreName = _context.GetRequestVal("StoreName");
                string StoreParam = Tools.UrlDecode(_context.GetRequestVal("StoreParam"));
                string ParamOut = Tools.UrlDecode(_context.GetRequestVal("ParamOut"));
                string ParamTpyeOut = Tools.UrlDecode(_context.GetRequestVal("ParamTpyeOut"));
                string ParamTpyeIn = Tools.UrlDecode(_context.GetRequestVal("ParamTpyeIn"));
                HTTP_CODE.WriteLogAction("functionName:/Utils\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") +
                    "\nFormName:" + FormName + "; StoreName:" + StoreName + "; StoreParam:" + StoreParam
                     + "; ParamOut:" + ParamOut + "; ParamTpyeOut:" + ParamTpyeOut, _context);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Content("#DSJSADDON <!--JS--> JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + l + "', '', '0');");//alert('" + l + "');
                }
                else
                {
                    try
                    {
                        string r1 = ""; string r = "";
                        string[] po = ParamOut.Split(new string[] { "||" }, StringSplitOptions.None);
                        string[] poType = ParamTpyeOut.Split(new string[] { "||" }, StringSplitOptions.None);
                        dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                        bosDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context);
                        StoreParam = _context.InputDataSetParam(StoreParam, ParamTpyeIn);
                        if (StoreParam != "")
                        {
                            //string[] a1; a1 = StoreParam.Split(new string[] { "||" }, StringSplitOptions.None);
                            //for (var i = 0; i < a1.Length; i++)
                            //{                            
                            //    string[] a11 = a1[i].Split(new string[] { "|" }, StringSplitOptions.None);
                            //    string val = a11[4];
                            //    val = Tools.ParseValue(_context, val, true);
                            //    json = json + ",{\"ParamName\":\"" + a11[0] + "\", \"ParamType\":\"" + a11[1] + "\", \"ParamInOut\":\"" + a11[2] + "\", \"ParamLength\":\"" + a11[3] + "\", \"InputValue\":\"" + val + "\"}";
                            //}
                            //if (json != "")
                            //{
                            //    json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                            //    d = JObject.Parse(json);
                            //}
                            d = JObject.Parse(StoreParam);
                        }
                        bosDAO.ExecuteStore("Utils", StoreName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        if (errorCode == 200 || errorCode == 204)
                        {
                            d = JObject.Parse(json);
                            if (d.Utils.Items.Count < 1) // biến Output
                            {
                                d = JObject.Parse("{" + parameterOutput + "}");
                                try
                                    {
                                        string t = t = poType[0];
                                        r1 = d.ParameterOutput[po[0]].ToString();
                                        if (t == "Numeric")
                                        {
                                            try { r1 = Tools.FormatNumber(r1); } catch { r1 = ""; }
                                        }
                                        else
                                        if (t == "Date")
                                        {
                                            try
                                            {
                                                r1 = DateTime.Parse(r1).ToString("dd/MM/yyyy");
                                            }
                                            catch
                                            {
                                                r1 = "";
                                            }
                                        }
                                        else if (t == "Datetime")
                                        {
                                            try
                                            {
                                                r1 = DateTime.Parse(r1).ToString("dd/MM/yyyy HH:mm");
                                            }
                                            catch
                                            {
                                                r1 = "";
                                            }
                                        }
                                    }
                                    catch { r1 = ""; }
                                r = r1;
                                for (int i = 1; i < po.Length; i++)
                                {
                                    try
                                    {
                                        string t = "";
                                        if (i < poType.Length) t = poType[i];
                                        r1 = d.ParameterOutput[po[i]].ToString();
                                        if (t == "Numeric")
                                        {
                                            try { r1 = Tools.FormatNumber(r1); } catch { r1 = ""; }
                                        }
                                        else
                                        if (t == "Date")
                                        {
                                            try
                                            {
                                                r1 = DateTime.Parse(r1).ToString("dd/MM/yyyy");
                                            }
                                            catch
                                            {
                                                r1 = "";
                                            }
                                        }
                                        else if (t == "Datetime")
                                        {
                                            try
                                            {
                                                r1 = DateTime.Parse(r1).ToString("dd/MM/yyyy HH:mm");
                                            }
                                            catch
                                            {
                                                r1 = "";
                                            }
                                        }
                                    }
                                    catch { r1 = ""; }
                                    r = r + "||" + r1;
                                }
                            }
                            else
                            {
                                d = JObject.Parse(json);
                                r1 = Tools.GetDataJson(d.Utils.Items[0], po[0]);
                                r = r1;
                                for (int i = 1; i < po.Length; i++)
                                {
                                    try
                                    {
                                        string t = "";
                                        if (i < poType.Length) t = poType[i];
                                        r1 = Tools.GetDataJson(d.Utils.Items[0], po[i]);
                                        if (t == "Date")
                                        {
                                            try
                                            {
                                                r1 = DateTime.Parse(r1).ToString("dd/MM/yyyy");
                                            }
                                            catch
                                            {
                                                r1 = "";
                                            }
                                        }
                                        else if (t == "Datetime")
                                        {
                                            try
                                            {
                                                r1 = DateTime.Parse(r1).ToString("dd/MM/yyyy HH:mm");
                                            }
                                            catch
                                            {
                                                r1 = "";
                                            }
                                        }
                                    }
                                    catch { r1 = ""; }
                                    r = r + "||" + r1;
                                }
                            }
                        }
                        else
                        {
                            return Content("#DSJSADDON <!--JS--> JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + errorString + "', '', '0');");//alert('" + errorString + "');
                        }
                        //r = r1.ToString();
                        //r1 = null;
                        return Content(FormName + "$" + ParamOut + "$" + r);
                    }
                    catch (Exception ex)
                    {
                        HTTP_CODE.WriteLogAction("functionName:/Utils\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") +
                        "\nFormName:" + FormName + "; StoreName:" + StoreName + "; StoreParam:" + StoreParam
                         + "; ParamOut:" + ParamOut + "\nError: " + ex.ToString(), _context);
                        return Content("#DSJSADDON <!--JS--> JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("OnError") + "', '', '0');");//alert('" + _context.GetLanguageLable("OnError") + "');
                    }
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return Redirect("/Home");
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}