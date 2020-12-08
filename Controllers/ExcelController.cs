using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text;
using OfficeOpenXml; 
using Utils;
using Newtonsoft.Json.Linq;
using System;
using HRS.Models;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace HRScripting.Controllers
{
    public class ExcelController : Controller 
    {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private HRSCache _cache;
        public ExcelController(IMemoryCache memoryCache)
        {
            _cache = new HRSCache(memoryCache);
        }
        public IActionResult Index()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            StringBuilder r1 = new StringBuilder(); string r;
            r1.Append(UIDef.UIHeaderPopup(_context, "Home"));
            r1.Append("<div class=\"formsearch\"></div>");
            r1.Append(UIDef.UIFooterPopup(_context, "ImportExcel"));
            r = r1.ToString();
            r1 = null;
            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
        }

        #region Export Data To Excel
        private void ExecuteData(BaseTab a, ref HRSContext context, ref ToolDAO DataDAO, 
            string QueryStringFilter, string ParamList, string ParamDefault, string QueryStringType, string[] StoreName, ref string parameterOutput,
            ref string ColumnName, ref string ColumnTextLable, ref string ColumnType,
            ref string json, ref int errorCode, ref string errorString, ref dynamic d, string FormType = "List")
        {
            string SPName = StoreName[0]; 
            switch (FormType)
            {
                case "Pivot":
                    string SPPivot =    Tools.GetDataJson(a.BTab.DBConfig.Items[0], "SPPivot");
                    string ParamPivot = Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ParamPivot");
                    string[] ColName =  Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnNamePivot").Split(new string[] { "^" }, StringSplitOptions.None);
                    string[] ColType =  Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnTypePivot").Split(new string[] { "^" }, StringSplitOptions.None);
                    Tools.ExecParam("PivotColumn", QueryStringFilter, ParamPivot, ParamDefault, QueryStringType, context,
                        SPPivot, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString);

                    string ColNamePivot = ""; string ColLablePivot = ""; string ColTypePivot = "";
                    if (errorCode == 200)
                    {
                        d = JObject.Parse(json);
                        for (int i = 0; i < d.PivotColumn.Items.Count; i++)
                        {
                            ColLablePivot = ColLablePivot + "^" + Tools.GetDataJson(d.PivotColumn.Items[i], ColName[1]);
                            ColNamePivot = ColNamePivot + "^" + Tools.GetDataJson(d.PivotColumn.Items[i], ColName[0]);
                            ColTypePivot = ColTypePivot + "^" + ColType[ColType.Length - 1];
                        }
                    }
                    ColumnName = ColumnName.Replace("[COLUMNNAMEPIVOT]", ColNamePivot);
                    ColumnTextLable = ColumnTextLable.Replace("[COLUMNLABLEPIVOT]", ColLablePivot);
                    ColumnType = ColumnType.Replace("[COLUMNTYPEPIVOT]", ColTypePivot);
                    parameterOutput = ""; json = ""; errorCode = 0; errorString = "";
                    Tools.ExecParam("ListForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, context,
                        SPName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, true);
                    d = JObject.Parse(json);
                    break;
                case "Edit":
                case "List":
                    Tools.ExecParam("ListForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, context,
                        SPName, DataDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, true);
                    d = JObject.Parse(json);
                    break;
            }
        }
        [HttpGet] //Excel/DbXlsx
        public IActionResult DbXlsx()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string TabIndex = _context.GetRequestTabIndex();
                HTTP_CODE.WriteLogAction("functionName:/Basetab/DbXlsx\nUserID" + _context.GetSession("UserID") + "\nUserName" + _context.GetSession("UserName") + "\nTabIndex" + TabIndex, _context);
                // check login
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect("/Home/Index?Message=" + _context.ReturnMsg(l, "Error"));
                }
                else
                {
                    // check quyen
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = _context.CheckPermistion("IsDelete", 0, 0, TabIndex);
                    bool IsGrant = _context.CheckPermistion("IsGrant", 0, 0, TabIndex);
                    string FormEditType =   Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormEditType");
                    string IsSysAdmin =     Tools.GetDataJson(a.BTab.DBConfig.Items[0], "IsSysAdmin");
                    _context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType, IsSysAdmin);
                    if (!(IsGrant || IsInsert || IsUpdate || IsDelete))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex), _context);
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }
                    string Author = _context.GetSession("UserName");
                    var package = new OfficeOpenXml.ExcelPackage();
                    package.Workbook.Properties.Title = _context.GetLanguageLable(TabIndex);
                    package.Workbook.Properties.Author = Author;
                    package.Workbook.Properties.Subject = _context.GetLanguageLable(TabIndex);
                    package.Workbook.Properties.Keywords = _context.GetLanguageLable(TabIndex);

                    //First add the headers
                    string EditColumn = "";
                    string EditColumnLable = "";
                    string EditColumnType = "";
                    EditColumn =        Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnName");
                    EditColumnLable =   Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnTextLable");
                    EditColumnType =    Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnType");

                    var worksheet = package.Workbook.Worksheets.Add(TabIndex);

                    string SPName =         Tools.GetDataJson(a.BTab.DBConfig.Items[0], "SchemaName");
                    string[] StoreName =    Tools.GetDataJson(a.BTab.DBConfig.Items[0], "spName").Split(new string[] { "$" }, StringSplitOptions.None);
                    for (int iSP = 0; iSP < StoreName.Length; iSP++) if (SPName != "" && SPName != null) StoreName[iSP] = SPName + "." + StoreName[iSP];
                    SPName = StoreName[0];

                    string FormType =           Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormType"); // List; Tree; Edit
                    string QueryStringFilter =  Tools.GetDataJson(a.BTab.DBConfig.Items[0], "QueryStringFilter");
                    string QueryStringType =    Tools.GetDataJson(a.BTab.DBConfig.Items[0], "QueryStringType");
                    string ParamList =          Tools.GetDataJson(a.BTab.DBConfig.Items[0], "paramList");
                    string ParamDefault =       Tools.GetDataJson(a.BTab.DBConfig.Items[0], "paramDefault");
                    string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                    dynamic d = null;
                    ExecuteData(a, ref _context, ref toolDAO, QueryStringFilter, ParamList, ParamDefault, QueryStringType, StoreName, ref parameterOutput,
                        ref EditColumn, ref EditColumnLable, ref EditColumnType,
                        ref json, ref errorCode, ref errorString, ref d, FormType);
                    //Tools.ExecParam("ListForm", QueryStringFilter, ParamList, ParamDefault, QueryStringType, ref _context, ref SPName, ref toolDAO, ref parameterOutput, ref json, ref errorCode, ref errorString, true);
                    d = null; d = JObject.Parse(json);
                    string[] a1; a1 = EditColumn.Split(new string[] { "^" }, StringSplitOptions.None);
                    string[] a2; a2 = EditColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
                    string[] a3; a3 = EditColumnType.Split(new string[] { "^" }, StringSplitOptions.None);

                    for (int i = 0; i < a2.Length; i++)
                    {
                        string[] a31 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None);
                        string Comment = _context.GetLanguageLable(a31[0]);
                        if (a2[i] != "[Action]")
                        {
                            string v = ""; string val = "";
                            if (a31.Length > 1) v = a31[1];
                            v = Tools.ParseValue(_context, v, false);
                            worksheet.Cells[1, i + 1].Value = _context.GetLanguageLable(a2[i]);
                            //worksheet.Cells[1, i + 1].AutoFitColumns();
                            worksheet.Column(i + 1).AutoFit();
                            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                            switch (a31[0])
                            {
                                case "-":
                                case "Hidden":
                                    val = _context.ReplaceSessionValue(v);
                                    if (val == "")
                                        Comment = Comment + "\nValue " + _context.GetLanguageLable("Null");
                                    else
                                        Comment = Comment + "\nValue = " + val;
                                    worksheet.Column(i + 1).Hidden = true;
                                    worksheet.Column(i + 1).Style.WrapText = true;
                                    break;
                                case "HREF":
                                    Comment = Comment + "\nValue " + _context.GetLanguageLable("Null");
                                    worksheet.Column(i + 1).Style.WrapText = true;
                                    break;
                                case "Numeric":
                                    worksheet.Column(i + 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                    break;
                                case "Date":
                                    Comment = Comment + "\nValue = 'dd/MM/yyyy'";
                                    worksheet.Column(i + 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;
                                case "Datetime":
                                    Comment = Comment + "\nValue = 'dd/MM/yyyy HH:mm'";
                                    worksheet.Column(i + 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;
                                case "Time":
                                    Comment = Comment + "\nValue = 'HH:mm'";
                                    worksheet.Column(i + 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;
                                case "Checkbox":
                                    Comment = Comment + "\nValue = " + val + " ==> Checked";
                                    break;
                                case "AttachFiles":
                                    Comment = Comment + "\nValue " + _context.GetLanguageLable("Null");
                                    worksheet.Column(i + 1).Hidden = true;
                                    break;
                                default:
                                    worksheet.Column(i + 1).Style.WrapText = true;
                                    break;
                            }
                            if (",Textarea,Textbox,Password,Algorithm,Numeric,".IndexOf("," + a31[0] + ",") < 1) worksheet.Cells[1, i + 1].AddComment(Comment, Author);
                        }
                    }
                    for (int j = 0; j < d.ListForm.Items.Count; j++)
                    {
                        for (int i = 0; i < a2.Length; i++)
                        {
                            if (a2[i] != "[Action]")
                            {
                                string[] a31 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None);
                                string val = Tools.GetDataJson(d.ListForm.Items[j], a1[i]);
                                switch (a31[0].ToLower())
                                {
                                    case "numeric":
                                        int iRound = 0; try { iRound = int.Parse(a31[1]); } catch { iRound = 0; }
                                        try { worksheet.Cells[j + 2, i + 1].Value = Tools.FormatNumber(val, iRound); } catch { };
                                        break;
                                    case "date":
                                        try { worksheet.Cells[j + 2, i + 1].Value = DateTime.Parse(val).ToString("dd/MM/yyyy"); } catch { };
                                        break;
                                    case "datetime":
                                        try { worksheet.Cells[j + 2, i + 1].Value = DateTime.Parse(val).ToString("dd/MM/yyyy HH:mm"); } catch { };
                                        break;
                                    case "time":
                                        try { worksheet.Cells[j + 2, i + 1].Value = DateTime.Parse(val).ToString("HH:mm"); } catch { };
                                        break;
                                    case "selectbox":
                                    case "checkbox":
                                        worksheet.Cells[j + 2, i + 1].Value = (val == "0"? _context.GetLanguageLable("Unactive") : (val == "1" ? _context.GetLanguageLable("Active") : val));
                                        break;
                                    default:
                                        worksheet.Cells[j + 2, i + 1].Value = val;
                                        break;
                                }
                            }
                        }
                    }

                    byte[] reportBytes; reportBytes = package.GetAsByteArray();
                    string MimeType = Tools.GetValueFormKeys(_context._appConfig.MimeType.ToString(), ".xlsx");
                    return File(reportBytes, MimeType, TabIndex + ".xlsx");
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Redirect("/Home/Index?Message=" + _context.ReturnMsg(r, "Error"));
            }
        }
        [HttpGet] //Excel/FormExcel
        public IActionResult FormExcel()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string l = _context.CheckLogin(ref bosDAO);
                if (l != "")
                {
                    return Redirect("/Home/Index?Message=" + _context.ReturnMsg(l, "Error"));
                }
                else
                {
                    string TabIndex = _context.GetRequestTabIndex();
                    if (TabIndex == "") return StatusCode(404);
                    // check quyen                
                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);

                    if (!(IsInsert || IsUpdate))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex));
                        return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error"));
                    }
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    string Author = _context.GetSession("UserName");

                    var package = new ExcelPackage();
                    package.Workbook.Properties.Title = _context.GetLanguageLable(TabIndex);
                    package.Workbook.Properties.Author = Author;
                    package.Workbook.Properties.Subject = _context.GetLanguageLable("Form");
                    package.Workbook.Properties.Keywords = _context.GetLanguageLable("Form");

                    var worksheet = package.Workbook.Worksheets.Add(TabIndex);
                    worksheet.View.FreezePanes(4, 1);
                    worksheet.Protection.AllowFormatColumns = true;
                    worksheet.Protection.IsProtected = true;
                    worksheet.Protection.AllowAutoFilter = true;
                    worksheet.Protection.SetPassword(TabIndex);
                    //First add the headers
                    string EditColumn = "";
                    string EditColumnLable = "";
                    string EditColumnType = "";
                    //string ReqJson; 
                    if (a.BTabGrp.DBConfigGrp.Items.Count > 0)
                    {
                        EditColumn = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumn");
                        EditColumnLable = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumnLable");
                        EditColumnType = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumnType");
                        for (int i = 1; i < a.BTabGrp.DBConfigGrp.Items.Count; i++)
                        {
                            EditColumn = EditColumn + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[i], "EditColumn");
                            EditColumnLable = EditColumnLable + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[i], "EditColumnLable");
                            EditColumnType = EditColumnType + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[i], "EditColumnType");
                        }
                    }
                    else
                    {
                        EditColumn =        Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnName");
                        EditColumnLable =   Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnTextLable");
                        EditColumnType =    Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnType");
                    }
                    string[] a1; a1 = EditColumn.Split(new string[] { "^" }, StringSplitOptions.None);
                    string[] a2; a2 = EditColumnLable.Split(new string[] { "^" }, StringSplitOptions.None);
                    string[] a3; a3 = EditColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
                    XlsxForm[] xlsx = new XlsxForm[a1.Length];
                    XlsxFormSelect[] xlsxSelect = new XlsxFormSelect[a1.Length];
                    int j = 1;
                    for (int i = 0; i < a2.Length; i++)
                    {
                        string[] a31 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None); bool IsRequire = false;
                        string Comment = _context.GetLanguageLable(a2[i]) + ": ";
                        Comment = Comment + "\n" + _context.GetLanguageLable("InputType") + ": " + a31[0];
                        string v = ""; string val = "";
                        if (a31.Length > 1) v = a31[1];
                        v = Tools.ParseValue(_context, v, false);

                        worksheet.Cells[2, (i + j)].Value = _context.GetLanguageLable(a2[i]);
                        worksheet.Cells[2, (i + j)].Style.Font.Bold = true;
                        worksheet.Cells[2, (i + j)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        if (a31.Length > 2) if (a31[2] == "1") IsRequire = true;
                        worksheet.Cells[1, (i + j)].Value = (i + j);
                        if (IsRequire)
                        {
                            Comment = Comment + "\n" + _context.GetLanguageLable("Column") + " [" + _context.GetLanguageLable(a2[i]) + "] " + _context.GetLanguageLable("IsRequire");
                            worksheet.Cells[2, (i + j)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[2, (i + j)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);

                            worksheet.Cells[1, (i + j)].Style.Font.Bold = true;
                            worksheet.Cells[1, (i + j)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[1, (i + j)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[1, (i + j)].Value = _context.GetLanguageLable("IsRequire");
                            worksheet.Column((i + j)).AutoFit();
                        }
                        //worksheet.Cells[2, (i + j)].AutoFitColumns();
                        worksheet.Cells[3, (i + j)].Value = a1[i];
                        var a9 = worksheet.Cells[2, 2].Address;
                        //UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex);
                        switch (a31[0])
                        {
                            case "-":
                            case "Hidden":
                                val = _context.ReplaceSessionValue(v);
                                if (val == "")
                                    Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = " + _context.GetLanguageLable("Null");
                                else
                                    Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = " + val;
                                //worksheet.Cells[4, (i + j)].Value = val;
                                UIDef.SetCellFormula(ref worksheet, (i + j), val, true, false, "", 0, "", "", 0, true);
                                worksheet.Column(i + j).Hidden = true;
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex);
                                break;
                            case "AutoNumber":
                                Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = " + _context.GetLanguageLable("Null");
                                worksheet.Column(i + j).Hidden = true;
                                worksheet.Cells[1, (i + j)].Value = "PrimaryKey";
                                UIDef.SetCellFormula(ref worksheet, (i + j), a3[i], true, false, "", 0, "", "", 0, true);
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex);
                                break;
                            case "Textarea":
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex, false);
                                break;
                            case "Textbox":
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex, false);
                                if (Tools.Right(a2[i], 4) == "Code") worksheet.Cells[1, (i + j)].Value = "PrimaryKey";
                                break;
                            case "Password":
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex);
                                break;
                            case "Algorithm":
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex);
                                break;
                            case "Numeric":
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex, false);
                                break;
                            case "Date":
                                Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = 'dd/MM/yyyy'";
                                //worksheet.Cells[4, (i + j)].Value = DateTime.Now.ToString("dd/MM/yyyy");
                                UIDef.SetCellFormula(ref worksheet, (i + j), DateTime.Now.ToString("dd/MM/yyyy"), true, false, "");
                                //worksheet.Cells[4, (i + j)].AutoFitColumns();
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex, false);
                                break;
                            case "Datetime":
                                Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = 'dd/MM/yyyy HH:mm'";
                                //worksheet.Cells[4, (i + j)].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                //worksheet.Cells[4, (i + j)].AutoFitColumns();
                                UIDef.SetCellFormula(ref worksheet, (i + j), DateTime.Now.ToString("dd/MM/yyyy HH:mm"), true, false, "");
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex, false);
                                break;
                            case "Time":
                                Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = 'HH:mm'";
                                //worksheet.Cells[4, (i + j)].Value = DateTime.Now.ToString("HH:mm");
                                UIDef.SetCellFormula(ref worksheet, (i + j), DateTime.Now.ToString("HH:mm"), true, false, "");
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex, false);
                                break;
                            case "Checkbox":
                                Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = " + a31[4] + " ==> Checked";
                                //worksheet.Cells[4, (i + j)].Value = val;
                                UIDef.SetCellFormula(ref worksheet, (i + j), a31[4], true, false, "");
                                UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex, false);
                                break;
                            case "Radio":
                                //Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = " + a31[4] + " ==> Checked";
                                //worksheet.Cells[4, (i + j)].Value = val;
                                //UIDef.SetCellFormula(ref worksheet, (i + j), val, true, false, "");
                                //UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex, false);
                                UIDef.SetCellVal(ref worksheet, ref package,
                                    a31[3], a31[4], i, ref j, TabIndex, a2, IsRequire, _context);
                                break;
                            case "SearchBox":
                            case "SearchBoxM":
                                UIDef.SetCellVal(ref worksheet, ref package,
                                    ref toolDAO, i, ref j, TabIndex, a2, IsRequire, _context,
                                    a31[5], a31[4], a31[8], a31[7], a31[11], ref xlsx);
                                break;
                            case "Actb":
                                UIDef.SetCellVal(ref worksheet, ref package,
                                    ref toolDAO, i, ref j, TabIndex, a2, IsRequire, _context,
                                    a31[6], a31[5], a31[7]);
                                break;
                            case "ActbText":
                                UIDef.SetCellVal(ref worksheet, ref package,
                                    a31[5], a31[6], i, ref j, TabIndex, a2, IsRequire, _context);
                                break;
                            case "MutilCheckbox":
                            case "DivMutilCheckbox":
                            case "Selectbox":
                                if (a31.Length > 6)
                                    UIDef.SetCellVal(ref worksheet, ref package,
                                        ref toolDAO, i, ref j, TabIndex, a2, IsRequire, _context,
                                        a31[4], a31[3], a31[5], a31[6], ref xlsxSelect);
                                else
                                    UIDef.SetCellVal(ref worksheet, ref package,
                                        ref toolDAO, i, ref j, TabIndex, a2, IsRequire, _context,
                                        a31[4], a31[3], a31[5], "", ref xlsxSelect);
                                ///UIDef.SetCellVall(ref worksheet, xlsxSelect, i, ref j, a2, TabIndex);
                                break;
                            case "SelectboxText":
                                UIDef.SetCellVal(ref worksheet, ref package,
                                    a31[3], a31[4], i, ref j, TabIndex, a2, IsRequire, _context);
                                break;
                            case "AttachImage":
                                Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = " + _context.GetLanguageLable("Null");
                                worksheet.Column(i + j).Hidden = true;
                                break;
                            case "AttachFiles":
                                Comment = Comment + "\n" + _context.GetLanguageLable("Value") + " = " + _context.GetLanguageLable("Null");
                                worksheet.Column(i + j).Hidden = true;
                                break;
                        }
                        //if (",Textarea,Textbox,Password,Algorithm,Numeric,".IndexOf("," + a31[0] + ",") < 1)
                        worksheet.Cells[2, (i + j)].AddComment(Comment, Author);
                        worksheet.Row(3).Hidden = true;
                        //UIDef.SetCellVall(ref worksheet, xlsx, i, ref j, a2, TabIndex);
                    }

                    byte[] reportBytes; reportBytes = package.GetAsByteArray();
                    string MimeType = Tools.GetValueFormKeys(_context._appConfig.MimeType.ToString(), ".xlsx");
                    return File(reportBytes, MimeType, TabIndex + ".xlsx");
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Redirect("/Home/Index?Message=" + _context.ReturnMsg(r, "Error"));
            }
        }
        #endregion

        #region Import Data From Excel
        [HttpGet] //Excel/ImportExcel
        public IActionResult ImportExcel()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect("/Excel/Index?Message=" + _context.ReturnMsg(l, "Error", "1"));
            }
            else
            {
                string TabIndex = _context.GetRequestTabIndex();
                if (TabIndex == "") return StatusCode(404);
                // check quyen                
                bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);

                if (!(IsInsert || IsUpdate))
                {
                    HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex));
                    return Redirect("/Excel/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error", "1"));
                }

                _context.SetSession("HRSUpload", "Excel");
                StringBuilder r1 = new StringBuilder(); string r; string v = "";
                r1.Append(UIDef.UIHeaderPopup(_context, _context.GetLanguageLable("ImportExcel"), "ImportExcel"));
                r1.Append(Environment.NewLine + "<form method=\"post\" role=\"form\" action=\"/Excel/ImportExcel\" id=\"frmAtt\" name=\"frmAtt\" enctype=\"multipart/form-data\">");
                r1.Append(Environment.NewLine + "<div id=\"pnlUpload\" class=\"row inline-input\">" +
                    "<div class=\"col-sm-4\">");
                r1.Append(UIDef.UIHidden("TabIndex", TabIndex));
                r1.Append(Environment.NewLine + "<div class=\"form-group row\">" + UIDef.UIButton("bntSearch", _context.GetLanguageLable("Upload"), "BtnUpload();", " class=\"btn inport\"") + "</div>");
                r1.Append(Environment.NewLine + "<div class=\"form-group row\">");
                r1.Append(Environment.NewLine + UIDef.UICheckbox("chkPrev", _context.GetLanguageLable("Preview"), "1", "0", UIDef.FocusAfter("chkPrev")) + "</div>");
                r1.Append(Environment.NewLine + "<div class=\"form-group row\">" +
                        Environment.NewLine + "<label class=\"col-form-label active\">" + _context.GetLanguageLable("ChoiceFile") + "<font color=\"red\">*</font></label>" +
                        Environment.NewLine + "<div class=\"file-field\">" +
                        Environment.NewLine + "<div class=\"btn btn-primary btn-sm float-left waves-effect waves-light\">" +
                        Environment.NewLine + "<span>Chọn file</span><input type=\"file\" name=\"Upload\" id=\"Upload\" />" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "<div class=\"file-path-wrapper\"><input class=\"file-path validate\" readonly type=\"text\" placeholder=\"" + _context.GetLanguageLable("ChoiceFile") + "\"></div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>");
                r1.Append(Environment.NewLine + "<div class=\"form-group row\"><label class=\"col-form-label active\">" + _context.GetLanguageLable("ChoiceSheets") + "<font color=\"red\">*</font></label>");//(*)
                r1.Append(Environment.NewLine + UIDef.UISelectStr("Sheets", "", "", ref v, true, "", "getData()", false) + "</div>");
                r1.Append(Environment.NewLine + "</div>");
                r1.Append(Environment.NewLine + "</div><div id=\"Excel-Preview\" style=\"overflow-y:auto\"></div>");
                r1.Append(Environment.NewLine + "</form>");
                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                r1.Append(Environment.NewLine + "function BtnUpload (){" +
                    Environment.NewLine + "if(document.frmAtt.Upload.value == '') {" +
                    Environment.NewLine + "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("FileIsNull") + "', '', '0');return;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "if(document.frmAtt.Sheets.value == '') {" +
                    Environment.NewLine + "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("SheetsIsNull") + "', '', '0');return;" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "document.frmAtt.submit();}");
                r1.Append(Environment.NewLine + "</script>");
                r1.Append(UIDef.UIFooterPopup(_context, "ImportExcel"));
                r = r1.ToString();
                r1 = null;
                return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
            }
        }
        [HttpPost]
        public IActionResult ImportExcel(IFormFile Upload)
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            try
            {
                ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                string l = _context.CheckLogin(ref bosDAO);
                string TabIndex = _context.GetRequestTabIndex();
                string SheetName = _context.GetRequestVal("Sheets");
                if (l != "")
                {
                    return Redirect(_context.ReturnUrlLogin(l, false));
                }
                else
                {
                    //string TabIndex = _context.GetRequestTabIndex();
                    if (TabIndex == "") return StatusCode(404);
                    ToolDAO toolDAO = new ToolDAO(_context.GetSession("CompanyCode"), _context); // Default connectstring - Schema BOS
                    BaseTab a = new BaseTab(_context, bosDAO, toolDAO);
                    string FormEditType =   Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormEditType");
                    string IsSysAdmin =     Tools.GetDataJson(a.BTab.DBConfig.Items[0], "IsSysAdmin");
                    // check quyen                
                    bool IsInsert = _context.CheckPermistion("IsInsert", 0, 0, TabIndex);
                    bool IsUpdate = _context.CheckPermistion("IsUpdate", 0, 0, TabIndex);
                    bool IsDelete = false; //string s = "";
                    _context.SetPermistion(ref IsInsert, ref IsUpdate, ref IsDelete, FormEditType, IsSysAdmin);
                    if (!(IsInsert || IsUpdate))
                    {
                        HTTP_CODE.WriteLogAction("GrantToFunction: " + _context.GetLanguageLable("YouAreNotIsGrantToFunction") + " " + _context.GetLanguageLable(TabIndex));
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction} {" + TabIndex + "}", "Error", "1"));
                    }
                    string HRSStart = _context.GetSession("HRSUpload");
                    if (HRSStart != "Excel") // StopHacking
                    {
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg("{StopHacking}", "Error", "1"));
                    }
                    // check file null
                    if (Upload == null)
                    {
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg("{FileIsNull}", "Error", "1"));
                    }
                    if (Upload.Length <= 0)
                    {
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg("{FileIsNull}", "Error", "1"));
                    }
                    // check extension
                    string FileExtension = _context.AppConfig.GetFileExtension(Upload.FileName).ToLower(); bool FileOK = false;
                    string[] allowedExtensions = _context._appConfig.ExcelFile.ToString().Split(new string[] { ";" }, StringSplitOptions.None);
                    FileOK = Tools.CheckFileExtension(FileExtension, allowedExtensions);
                    if (!FileOK)
                    {
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg("{FileExtensionIsNotAllowed}", "Error", "1"));
                    }
                    //Excel/ImportExcel
                    var memoryStream = _context.AppConfig.CreateMemoryStream();
                    Upload.CopyTo(memoryStream);
                    var package = new ExcelPackage(memoryStream);
                    ExcelWorksheet worksheet;
                    if (SheetName != "")
                        worksheet = package.Workbook.Worksheets[SheetName];
                    else
                        worksheet = package.Workbook.Worksheets[1];

                    string msg = ""; dynamic data = readExcelPackageToString(package, worksheet, ref msg, _context);
                    if (msg != "")
                    {
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg(msg, "Error", "1"));
                    }
                    //else
                    //{
                    //    return Json(readExcelPackageToString(package, worksheet, ref msg, _context));
                    //}
                    string[] jsdbcols =     Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnName").Split(new string[] { "^" }, StringSplitOptions.None);
                    string[] ColumnType =   Tools.GetDataJson(a.BTab.DBConfig.Items[0], "ColumnType").Split(new string[] { "^" }, StringSplitOptions.None);

                    string FormType =       Tools.GetDataJson(a.BTab.DBConfig.Items[0], "FormType");
                    if (FormType == "Edit")
                    {
                        string EditColumn = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumn");
                        string EditColumnType = Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[0], "EditColumnType");
                        for (int i = 1; i < a.BTabGrp.DBConfigGrp.Items.Count; i++)
                        {
                            EditColumn = EditColumn + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[i], "EditColumn");
                            EditColumnType = EditColumnType + "^" + Tools.GetDataJson(a.BTabGrp.DBConfigGrp.Items[i], "EditColumnType");
                            jsdbcols = EditColumn.Split(new string[] { "^" }, StringSplitOptions.None);
                            ColumnType = EditColumnType.Split(new string[] { "^" }, StringSplitOptions.None);
                        }
                    }
                    string SPUpdate =       Tools.GetDataJson(a.BTab.DBConfig.Items[0], "spUpdateName");
                    string ParamUpdate =    Tools.GetDataJson(a.BTab.DBConfig.Items[0], "paramUpdate");
                    string[] Params = ParamUpdate.Split(new string[] { "^" }, StringSplitOptions.None);
                    string Creator = _context.GetSession("UserID");
                    string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = ""; dynamic d = null; int CntSuccess = 0; int CntError = 0;
                    for (int j = 0; j < data.DataImport.Count; j++)
                    {
                        msg = msg + "«br»" + _context.GetLanguageLable("UpdateRow") + " " + (j + 1) + "....";
                        json = "";
                        for (int i = 0; i < Params.Length; i++)
                        {
                            string[] param = Params[i].Split(new string[] { ";" }, StringSplitOptions.None);
                            string val = "";
                            string frmVal = "";
                            try { frmVal = data.DataImport[j][param[0]].ToString(); } catch { frmVal = ""; }

                            if (param[0] == "Creator")
                                val = Creator;
                            else
                            {
                                val = frmVal;
                                int j11 = Tools.GetArrayPos(param[0], jsdbcols);
                                if (j11 > -1)
                                {
                                    string[] IColumnType = ColumnType[j11].Split(new string[] { ";" }, StringSplitOptions.None);
                                    switch (IColumnType[0].ToLower())
                                    {
                                        case "numeric":
                                            if (val == "")
                                                val = "0";
                                            else
                                                val = Tools.RemNumSepr(val);
                                            break;
                                        case "date":
                                            if (val == "")
                                                val = DateTime.Now.ToString("MM/dd/yyyy");
                                            else
                                                val = Tools.SwapDate(val);
                                            break;
                                        case "datetime":
                                            if (val == "")
                                                val = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                                            else
                                                val = Tools.SwapDate(val + ":00");
                                            break;
                                        case "algorithm":
                                            if (val != "") val = _context.enc.Encrypt(val);
                                            break;
                                    }
                                }
                                if (val == "") val = param[4]; // default from param
                                /*
                                if (val == "") //val = null;
                                {
                                    if (toolDAO.CheckParam(param[1])) val = "0";
                                    else if (toolDAO.CheckParam(param[1], "Date")) val = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                                    else if (toolDAO.CheckParam(param[1], "Time")) val = DateTime.Now.ToString("HH:mm:ss");
                                }
                                else if (toolDAO.CheckParam(param[1], "Date")) val = Tools.SwapDate(val);
                                else if (toolDAO.CheckParam(param[1]))
                                {
                                    int iActb = val.IndexOf(Tools.C_ActbSepr);
                                    if (iActb > -1) val = val.Substring(0, iActb - 1);
                                    val = Tools.RemNumSepr(val);
                                }*/
                            }
                            json = json + ",{\"ParamName\":\"" + param[0] + "\", \"ParamType\":\"" + param[1] + "\", \"ParamInOut\":\"" + param[2] + "\", \"ParamLength\":\"" + param[3] + "\", \"InputValue\":" + (val == null ? "null" : "\"" + val + "\"") + "}";
                        }

                        if (json != "")
                        {
                            json = "{\"parameterInput\":[" + Tools.RemoveFisrtChar(json) + "]}";
                            d = JObject.Parse(json);
                        }
                        toolDAO.ExecuteStore("Update", SPUpdate, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        if (errorCode == 500)
                        {
                            CntError = CntError + 1;
                            msg = msg + " ==» " + errorString;
                        }
                        else
                        {
                            d = JObject.Parse("{" + parameterOutput + "}");
                            if ((long)d.ParameterOutput.ResponseStatus > 0)
                                CntSuccess = CntSuccess + 1;
                            else
                                CntError = CntError + 1;

                            msg = msg + " ==» " + _context.GetLanguageLable(d.ParameterOutput.Message.ToString());
                        }
                    }
                    if (CntSuccess == 0 && CntError > 0)
                    {
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg(msg, "Error", "1", "1"));
                    }
                    else if (CntSuccess > 0 && CntError > 0)
                    {
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg(msg, "Warning", "1", "1"));
                    }
                    else
                    {
                        return Redirect("/Excel/Index?Message=" + _context.ReturnMsg(msg, "Success", "1", "1"));
                    }                    
                }
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("functionName:/Utils/Search\nException" + ex.ToString());
                string r = _context.RoleDebug(ex.ToString());
                return Redirect("/Excel/Index?Message=" + _context.ReturnMsg(r, "Error", "1", "1"));
            }
        }
        private string readCellValue(ExcelWorksheet w, int row, int col)
        {
            string r = "";
            if (!(w.Cells[row, col].Value == null)) if (w.Cells[row, col].Value.ToString() != "#N/A") r = w.Cells[row, col].Value.ToString();
            return r;
        }
        private string readHeaderToJson(ExcelWorksheet worksheet, int row, int rowLbl, int colCount)
        {
            string s = "{\"" + readCellValue(worksheet, row, 1) + "\": \"" + readCellValue(worksheet, rowLbl, 1) + "\"";
            for (int col = 2; col <= colCount; col++)
            {
                s = s + ", \"" + readCellValue(worksheet, row, col) + "\": \"" + readCellValue(worksheet, rowLbl, col) + "\"";
            }
            s = s + "}";
            return s;
        }
        private string readRowToJson(dynamic d, string txtColumn, string txtIsRequire, string txtUpdateRow, string NewRows,
            ExcelWorksheet worksheet, int ColPremaryKey, int row, int colCount, ref bool IsBreak, ref string msg)
        {
            string ErrorRows = ""; string s = "";
            if (ColPremaryKey > 0)
            {
                if (readCellValue(worksheet, row, ColPremaryKey) == "") IsBreak = true;
            }
            if (!IsBreak)
            {
                if (readCellValue(worksheet, 1, 1) == "bắt buộc")
                {
                    if (readCellValue(worksheet, row, 1) == "")// IsBreak = true;
                        ErrorRows = ", " + txtColumn + " [" + Tools.GetDataJson(d, readCellValue(worksheet, 3, 1)) + "] " + txtIsRequire;
                }
                s = "{\"" + readCellValue(worksheet, 3, 1) + "\": \"" + readCellValue(worksheet, row, 1) + "\"";
                for (int col = 2; col <= colCount; col++)
                {
                    if (readCellValue(worksheet, 1, col) == "bắt buộc")
                    {
                        if (readCellValue(worksheet, row, col) == "")
                            ErrorRows = ErrorRows + ", " + txtColumn + " [" + Tools.GetDataJson(d, readCellValue(worksheet, 3, col)) + "] " + txtIsRequire;
                    }
                    s = s + ", \"" + readCellValue(worksheet, 3, col) + "\": \"" + readCellValue(worksheet, row, col) + "\"";
                }
                s = s + "}";
                if (ErrorRows != "") msg = msg + txtUpdateRow + " " + row + ":" + Tools.RemoveFisrtChar(ErrorRows, 2) + NewRows;
            }
            return s;
        }
        private dynamic readExcelPackageToString(ExcelPackage package, ExcelWorksheet worksheet, ref string msg, HRSContext _context)
        {
            var rowCount = worksheet.Dimension?.Rows;
            var colCount = worksheet.Dimension?.Columns;

            if (!rowCount.HasValue || !colCount.HasValue)
            {
                return string.Empty;
            }

            if (rowCount.Value < 4 || colCount.Value < 2)
            {
                return string.Empty;
            }

            string txtColumn = _context.GetLanguageLable("Column");
            string txtIsRequire = _context.GetLanguageLable("IsRequire");
            string txtUpdateRow = _context.GetLanguageLable("UpdateRow");
            string NewRows = "«br»"; //string NewTab = "\\t";
            int ColPremaryKey = 0;
            int row = 1;
            while (row <= colCount.Value && ColPremaryKey == 0)
            {
                if (readCellValue(worksheet, 1, row) == "PrimaryKey") ColPremaryKey = row;
                row++;
            }
            if (ColPremaryKey == 0)
            {
                row = 1;
                while (row <= colCount.Value && worksheet.Column(row).Hidden) row++;
                if (row <= colCount.Value) ColPremaryKey = row;
            }

            dynamic d = null;
            try { d = JObject.Parse(readHeaderToJson(worksheet, 3, 2, colCount.Value)); } catch { d = null; }

            bool IsBreak = false; row = 4;//string ErrorRows = ""; 
            string json = ""; string s = "";
            /*
            if (ColPremaryKey > 0)
            {
                if (readCellValue(worksheet, 4, ColPremaryKey) == "") IsBreak = true;
            }
            if (!IsBreak)
            {
                if (readCellValue(worksheet, 1, 1) == "bắt buộc")
                {
                    if (readCellValue(worksheet, row, 1) == "")// IsBreak = true;
                    ErrorRows = ", " + txtColumn + " [" + readCellValue(worksheet, 3, 1) + "] " + txtIsRequire;
                }
                s = "{\"" + readCellValue(worksheet, 3, 1) + "\": \"" + readCellValue(worksheet, 4, 1) + "\"";
                for (int col = 2; col <= colCount.Value; col++)
                {
                    if (readCellValue(worksheet, 1, col) == "bắt buộc")
                    {
                        if (readCellValue(worksheet, row, col) == "")
                            ErrorRows = ErrorRows + ", " + txtColumn + " [" + readCellValue(worksheet, 3, col) + "] " + txtIsRequire;
                    }
                    s = s + ", \"" + readCellValue(worksheet, 3, col) + "\": \"" + readCellValue(worksheet, 4, col) + "\"";
                }
                s = s + "}";
                if (ErrorRows != "") msg = msg + txtUpdateRow + " " + row + ":" + Tools.RemoveFisrtChar(ErrorRows, 2) + NewRows;
            } 
            */

            s = readRowToJson(d, txtColumn, txtIsRequire, txtUpdateRow, NewRows,
                worksheet, ColPremaryKey, row, colCount.Value, ref IsBreak, ref msg);
            json = "{\"DataImport\": [" + s;
            row = 5;
            while (row <= rowCount.Value && !IsBreak)
            {
                /*
                if (ColPremaryKey > 0)
                {
                    if (readCellValue(worksheet, row, ColPremaryKey) == "") IsBreak = true;
                }
                if (!IsBreak)
                {
                    if (readCellValue(worksheet, 1, 1) == "bắt buộc")
                    {
                        if (readCellValue(worksheet, row, 1) == "")// IsBreak = true;
                            ErrorRows = ", " + txtColumn + " [" + readCellValue(worksheet, 3, 1) + "] " + txtIsRequire;
                    }
                    s = "{\"" + readCellValue(worksheet, 3, 1) + "\": \"" + readCellValue(worksheet, row, 1) + "\"";
                    for (int col = 2; col <= colCount.Value; col++)
                    {
                        if (readCellValue(worksheet, 1, col) == "bắt buộc")
                        {
                            if (readCellValue(worksheet, row, col) == "")
                                ErrorRows = ErrorRows + ", " + txtColumn + " [" + readCellValue(worksheet, 3, col) + "] " + txtIsRequire;
                        }
                        s = s + ", \"" + readCellValue(worksheet, 3, col) + "\": \"" + readCellValue(worksheet, row, col) + "\"";
                    }
                    s = s + "}";
                    json = json + "," + s;
                    if (ErrorRows != "") msg = msg + txtUpdateRow + " " + row + ":" + Tools.RemoveFisrtChar(ErrorRows, 2) + NewRows;
                }
                */
                s = readRowToJson(d, txtColumn, txtIsRequire, txtUpdateRow, NewRows,
                    worksheet, ColPremaryKey, row, colCount.Value, ref IsBreak, ref msg);
                json = json + "," + s;
                row++;
            }
            json = json + "]}";
            return JObject.Parse(json);
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