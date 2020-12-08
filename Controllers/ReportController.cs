using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using HRS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Utils;

namespace HRScripting.Controllers
{
    public class ReportController : Controller
    {
        private HRSCache _cache;
        public ReportController(IMemoryCache memoryCache)
        {
            _cache = new HRSCache(memoryCache);
        }
        public IActionResult Dashboard()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string RptID = _context.GetRequestRptID();
                string _MenuOn = "Min";
                string _MenuID = _context.GetRequestOneValue("MenuID");
                string DataOutput = _context.GetRequestOneValue("DataOutput");
                DataOutput = Tools.UrlDecode(DataOutput);
                if (DataOutput == "") DataOutput = "text/html";
                HTTP_CODE.WriteLogAction("functionName:/Report\nUserID: " + _context.GetSession("UserID") +
                    "\nUserName: " + _context.GetSession("UserName") + "\nReportIndex: " + RptID);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, RptID);
                    if (!(IsGrant))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(RptID));
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + RptID + "}", "Error"));
                    }
                    bool MenuOn = (_MenuOn != "Off"); bool IsHTML = (DataOutput == "text/html"); if (!IsHTML) MenuOn = false;
                    string Company = _context.GetSession("CompanyCode");
                    StringBuilder r1 = new StringBuilder(); string r = "";

                    ToolDAO DataDAO = new ToolDAO(Company, _context); // Default connectstring - Schema BOS
                    Report a = new Report(_context, bosDAO, DataDAO, RptID, _MenuOn, _MenuID); //HRSContext _context, ToolDAO _ConfigDAO, ToolDAO _DataDAO

                    
                    //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, a.MenuID));
                    r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, a.MenuID));
                    r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, a.MenuID, false));
                    r1.Append(a.UIDashboard());
                    r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                    r = r1.ToString();
                    ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, a.MenuID);
                    ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                    ViewData["IndexBody"] = r;
                    ViewData["PageTitle"] = _context.GetLanguageLable(RptID);
                    ViewData["iframe"] = _context.GetRequestVal("iframe");
                    ViewData["txtClose"] = _context.GetLanguageLable("Close");
                    r1 = null;
                    return View();
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        public IActionResult Index()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string RptID = _context.GetRequestRptID();
                string _MenuOn = _context.GetRequestMenuOn();
                string _MenuID = _context.GetRequestOneValue("MenuID");
                string DataOutput = _context.GetRequestOneValue("DataOutput");
                DataOutput = Tools.UrlDecode(DataOutput);
                if (DataOutput == "") DataOutput = "text/html";
                HTTP_CODE.WriteLogAction("functionName:/Report\nUserID: " + _context.GetSession("UserID") +
                    "\nUserName: " + _context.GetSession("UserName") + "\nReportIndex: " + RptID);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, RptID);
                    if (!(IsGrant))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(RptID));
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + RptID + "}", "Error"));
                    }
                    bool MenuOn = (_MenuOn != "Off"); bool IsHTML = (DataOutput == "text/html"); if (!IsHTML) MenuOn = false;
                    string Company = _context.GetSession("CompanyCode");
                    int height = 0;
                    string h = _context.GetRequestVal("height"); try { height = int.Parse(h); } catch { }
                    StringBuilder r1 = new StringBuilder(); string r = "";
                    ToolDAO DataDAO = new ToolDAO(Company, _context); // Default connectstring - Schema BOS
                    Report a = new Report(_context, bosDAO, DataDAO, RptID, _MenuOn, _MenuID, DataOutput); //HRSContext _context, ToolDAO _ConfigDAO, ToolDAO _DataDAO
                    r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, a.MenuID));
                    r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, a.MenuID, false));
                    
                    string ReportType = Tools.GetDataJson(a.Rpt.RptConfig.Items[0], "ReportType"); // 1||2||3||4;List||Pivot||Form||Other
                    bool IsPost = (_context.GetFormValue("RptID") != "");
                    if (ReportType == "3" && IsPost) DataOutput = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    r1.Append(a.UIFilterForm());
                    switch (ReportType)
                    {
                        case "1":
                            r1.Append(a.UIListForm(IsHTML));
                            break;
                        case "2":
                            r1.Append(a.UIPivotForm(IsHTML));
                            break;
                        case "3":
                            var package = a.UIFillToExcelFormV01(IsPost);
                            if (package == null) { IsPost = false; DataOutput = "text/html"; }
                            if (IsPost) return File(package.GetAsByteArray(), DataOutput, RptID + ".xlsx");
                            break;
                        default:
                            r1.Append(_context.GetLanguageLable("WrongReport"));
                            break;
                    }
                    r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                    r = r1.ToString();
                    r1 = null;
                    if (IsHTML)
                    {
                        string title = _context.GetLanguageLable(RptID);
                        ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, a.MenuID);
                        ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                        ViewData["IndexBody"] = r;
                        ViewData["PageTitle"] = title;
                        ViewData["iframe"] = _context.GetRequestVal("iframe");
                        ViewData["txtClose"] = _context.GetLanguageLable("Close");
                        return View();
                    }                       
                    else
                    {
                        byte[] fileContents = Encoding.UTF8.GetBytes(r); string fileName = "";
                        switch (DataOutput) //text/html||application/vnd.ms-excel||application/vnd.openxmlformats-officedocument.spreadsheetml.sheet||application/msword||application/pdf
                        {
                            case "application/vnd.ms-excel":
                                fileName = RptID + "report.xls";
                                break;
                            case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                                fileName = RptID + "report.xlsx";
                                break;
                            case "application/msword":
                                fileName = RptID + "report.doc";
                                break;
                            case "application/pdf":
                                fileName = RptID + "report.pdf";
                                break;
                        }
                        return File(fileContents, DataOutput, fileName);
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
        public IActionResult Chart()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string RptID = _context.GetRequestRptID();
                string _MenuOn = _context.GetRequestMenuOn();
                string _MenuID = _context.GetRequestOneValue("MenuID");
                HTTP_CODE.WriteLogAction("functionName:/Report-Chart\nUserID: " + _context.GetSession("UserID") +
                    "\nUserName: " + _context.GetSession("UserName") + "\nReportIndex: " + RptID);
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l));
                }
                else
                {
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, RptID);
                    if (!(IsGrant))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(RptID));
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + RptID + "}", "Error"));
                    }
                    bool MenuOn = (_MenuOn != "Off"); string DataOutput = "text/html";
                    string Company = _context.GetSession("CompanyCode");
                    //int height = 0;
                    //string h = _context.GetRequestVal("height"); try { height = int.Parse(h); } catch { height = 700; }
                    //int width = 0;
                    //string w = _context.GetRequestVal("width"); try { width = int.Parse(h); } catch { width = 900; }
                    ToolDAO DataDAO = new ToolDAO(Company, _context); // Default connectstring - Schema BOS
                    Report a = new Report(_context, bosDAO, DataDAO, RptID, _MenuOn, _MenuID, DataOutput);
                    StringBuilder r1 = new StringBuilder(); string r = "";
                    r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, a.MenuID));
                    r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, a.MenuID, false));
                    string title = _context.GetLanguageLable(RptID);
                    string ReportType = Tools.GetDataJson(a.Rpt.RptConfig.Items[0], "ReportType"); // 1||2||3||4;List||Pivot||Form||Other
                    r1.Append(a.UIFilterForm());
                    switch (ReportType)
                    {
                        case "41":
                        case "42":
                        case "43":
                            r1.Append(a.UIChartPie(/*width, height*/));
                            break;
                        case "51":
                        case "52":
                            r1.Append(a.UIChartBar(/*width, height*/));
                            break;
                        case "53":
                            r1.Append(a.UIChartLine(/*width, height*/));
                            break;
                        case "6":
                            r1.Append(a.UIChartOrg(/*width, height*/));// UIChartOrganization());
                            break;
                        case "9":
                            r1.Append(a.UIChartCombo(/*width, height*/));// UIChartOrganization());
                            break;
                        default:
                            r1.Append(_context.GetLanguageLable("WrongReport"));
                            break;
                    }
                    r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                    r = r1.ToString();
                    ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, a.MenuID);
                    ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                    ViewData["IndexBody"] = r;
                    ViewData["PageTitle"] = title;
                    ViewData["iframe"] = _context.GetRequestVal("iframe");
                    ViewData["txtClose"] = _context.GetLanguageLable("Close");
                    r1 = null;
                    return View();
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return Redirect("/Home");
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}