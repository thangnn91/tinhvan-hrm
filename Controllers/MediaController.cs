using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Utils;
using System.Drawing.Drawing2D;
using Newtonsoft.Json.Linq;
using HRS.Models;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace HRScripting.Controllers
{
    public class MediaController : Controller
    {
        public const string FolderRoot = @"\RootFolder\";
        public const string FolderRootImage = @"\wwwroot\docRoot\";

        private HRSCache _cache;
        public MediaController(IMemoryCache memoryCache)
        {
            _cache = new HRSCache(memoryCache);
        }
        public IActionResult Index()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            StringBuilder r1 = new StringBuilder(); string r;
            r1.Append(UIDef.UIHeaderPopup(_context, "Media"));
            r1.Append("<div class=\"formsearch\"></div>");
            r1.Append(UIDef.UIFooterPopup(_context, "Media"));
            r = r1.ToString();
            r1 = null;
            return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
        }
        public IActionResult RenderImage()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string ImageId = _context.GetRequestVal("ImageId");
            if (ImageId == "") ImageId = "0";
            bool IsCrop = !(_context.GetRequestVal("NoCrop") == "On");
            int errorCode = 0; string errorString = "";
            dynamic d = Tools.GetMedia(_context, ImageId, bosDAO, ref errorCode, ref errorString); // JObject.Parse(json);

            string UrlFileName = ""; string contentType = ""; string TabIndex = "";
            try
            {
                TabIndex = Tools.GetDataJson(d.GetImage.Items[0], "TabIndex");
                contentType = Tools.GetDataJson(d.GetImage.Items[0], "MimeType");//Tools.GetDataJson(d.GetImage.Items[0], "AttachmentCode") + Tools.GetDataJson(d.GetImage.Items[0], "FileExtention");
                if (IsCrop)
                    UrlFileName = _context.AppConfig.FolderRoot + Tools.GetDataJson(d.GetImage.Items[0], "UrlCrop").Replace("||", "\\");
                else
                    UrlFileName = _context.AppConfig.FolderRoot + Tools.GetDataJson(d.GetImage.Items[0], "UrlOriginal").Replace("||", "\\");
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("Attachments-file - 2.1: " + ex.ToString());
                return File("/images/NoImage.jpg", contentType);
            }
            if (!_context.AppConfig.PathIsFile(UrlFileName))
            {
                HTTP_CODE.WriteLogAction("Attachments-file - 2.1: ");
                return File("/images/NoImage.jpg", contentType);
            }
            HTTP_CODE.WriteLogAction("Attachments-file - 2: " + _context.AppConfig.PathIsFile(UrlFileName) + " contentType: " + contentType + " URL: " + UrlFileName);
            try
            {
                //var fileStream = _context.AppConfig.GetFileStream(UrlFileName);
                //return File(fileStream, contentType);
                System.IO.FileStream r = null;
                r = System.IO.File.Open(UrlFileName, System.IO.FileMode.Open);
                return File(r, contentType);
            }
            catch (Exception ex)
            {
                HTTP_CODE.WriteLogAction("Attachments-file - 2.3: " + ex.ToString());
                return File("/images/NoImage.jpg", contentType);
            }
        }
        public IActionResult RenderFile()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string ImageId = _context.GetRequestVal("ImageId");
            if (ImageId == "") ImageId = "0";
            bool IsCrop = !(_context.GetRequestVal("NoCrop") == "On");
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect("/Media/Index?Message=" + _context.ReturnMsg(l, "Error", "1"));
            }
            else
            {
                /*dynamic d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"AttachmentID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + ImageId + "\"}," +
                    "{\"ParamName\":\"Keyword\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"Author\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"-1\"}," +
                    "{\"ParamName\":\"Status\", \"ParamType\":\"16\", \"ParamInOut\":\"1\", \"ParamLength\":\"2\", \"InputValue\":\"-1\"}," +
                    "{\"ParamName\":\"Page\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                    "{\"ParamName\":\"PageSize\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + _context.GetSession("PageSizeBaseTab") + "\"}," +
                    "{\"ParamName\":\"Rowcount\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
                string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                bosDAO.ExecuteStore("GetImage", "SP_CMS__Attachment_List", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                */
                int errorCode = 0; string errorString = "";
                dynamic d = Tools.GetMedia(_context, ImageId, bosDAO, ref errorCode, ref errorString); // JObject.Parse(json);
                //ToolFolder t = new ToolFolder();
                string UrlFileName = ""; string contentType = ""; string TabIndex = "";
                try
                {
                    TabIndex = Tools.GetDataJson(d.GetImage.Items[0], "TabIndex");
                    contentType = Tools.GetDataJson(d.GetImage.Items[0], "MimeType");//Tools.GetDataJson(d.GetImage.Items[0], "AttachmentCode") + Tools.GetDataJson(d.GetImage.Items[0], "FileExtention");
                    if (IsCrop)
                        UrlFileName = _context.AppConfig.FolderRoot + Tools.GetDataJson(d.GetImage.Items[0], "UrlCrop").Replace("||", "\\");
                    else
                        UrlFileName = _context.AppConfig.FolderRoot + Tools.GetDataJson(d.GetImage.Items[0], "UrlOriginal").Replace("||", "\\");
                }
                catch
                {
                    contentType = "image/jpg"; UrlFileName = _context.AppConfig.FolderRoot + "\\wwwroot\\images\\NoImage.jpg";
                }
                HTTP_CODE.WriteLogAction("Attachments-file: " + _context.AppConfig.PathIsFile(UrlFileName) + " contentType: " + contentType + " URL: " + UrlFileName);
                /*
                if (!_context.CheckPermistion("IsGrant", 0, 0, TabIndex))
                {
                    UrlFileName = _context.AppConfig.FolderRoot + "\\wwwroot\\images\\permission__1x__573526.png";
                    contentType = "image/jpg";
                    HTTP_CODE.WriteLogAction("Attachments-file -1 : " + _context.AppConfig.PathIsFile(UrlFileName) + " contentType: " + contentType + " URL: " + UrlFileName);
                    try
                    {
                        //var fileStream = _context.AppConfig.GetFileStream(UrlFileName);
                        //return File(fileStream, contentType);
                        System.IO.FileStream r = null;
                        r = System.IO.File.Open(UrlFileName, System.IO.FileMode.Open);
                        return File(r, contentType);
                    }
                    catch (Exception ex)
                    {
                        return Content(ex.ToString());
                    }
                }
                else
                {*/
                if (!_context.AppConfig.PathIsFile(UrlFileName))
                {
                    UrlFileName = _context.AppConfig.FolderRoot + "\\wwwroot\\images\\NoImage.jpg";
                    contentType = "image/jpg";
                }
                HTTP_CODE.WriteLogAction("Attachments-file - 2: " + _context.AppConfig.PathIsFile(UrlFileName) + " contentType: " + contentType + " URL: " + UrlFileName);
                try
                {
                    //var fileStream = _context.AppConfig.GetFileStream(UrlFileName);
                    //return File(fileStream, contentType);
                    System.IO.FileStream r = null;
                    r = System.IO.File.Open(UrlFileName, System.IO.FileMode.Open);
                    return File(r, contentType);
                }
                catch (Exception ex)
                {
                    HTTP_CODE.WriteLogAction("Attachments-file - 2: " + ex.ToString());
                    return File("/images/NoImage.jpg", contentType);
                }
                //}                
            }
        }

        #region UploadImage Method
        [HttpGet]
        public IActionResult UploadImage()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect("/Media/Index?Message=" + _context.ReturnMsg(l, "Error", "1"));
            }
            else
            {
                if (!_context.CheckRoles(805))//804 - Overwrite;805 - Attachments
                {
                    HTTP_CODE.WriteLogAction("GrantToAttachments: " + _context.GetLanguageLable("YouAreNotIsGrantToAttachments") + " " + _context.GetLanguageLable("Attachments"));
                    return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToAttachments} {Attachments}", "Error", "1"));
                }
                else
                {
                    string TabIndex = _context.GetRequestTabIndex();
                    string InputName = _context.GetRequestVal("InputName");
                    _context.SetSession("HRSUpload", "Images");
                    StringBuilder r1 = new StringBuilder(); string r;
                    r1.Append(UIDef.UIHeaderPopup(_context, _context.GetLanguageLable("AttachmentImage"), "UploadImage") +
                        Environment.NewLine + "<form method=\"post\" role=\"form\" action=\"/Media/UploadImage\" " +
                        "id=\"frmAtt\" name=\"frmAtt\" enctype=\"multipart/form-data\">" +
                        Environment.NewLine + "<div id=\"pnlUpload\" class=\"row inline-input\">" +
                        Environment.NewLine + "<div class=\"col-sm-4\">" +
                        UIDef.UIHidden("TabIndex", TabIndex) +
                        UIDef.UIHidden("InputName", InputName) +
                        Environment.NewLine + "<div class=\"form-group row\">" + 
                        UIDef.UIButton("bntSearch", _context.GetLanguageLable("Upload"), true, " class=\"btn inport\"") + "</div>" +
                        Environment.NewLine + "<div class=\"form-group row\">" +
                        Environment.NewLine + "<label class=\"col-form-label active\">" + _context.GetLanguageLable("ChoiceFile") + "</label>" +
                        Environment.NewLine + "<div class=\"file-field\">" +
                        Environment.NewLine + "<div class=\"btn btn-primary btn-sm float-left waves-effect waves-light\">" +
                        Environment.NewLine + "<span>Chọn file</span><input type=\"file\" name=\"Upload\" id=\"Upload\" accept=\"image/*\" />" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "<div class=\"file-path-wrapper\"><input class=\"file-path validate\" readonly type=\"text\" placeholder=\"" + _context.GetLanguageLable("ChoiceFile") + "\"></div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" + 
                        Environment.NewLine + "</form>" +
                        UIDef.UIFooterPopup(_context, "UploadImage"));
                    r = r1.ToString();
                    r1 = null;
                    return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                }
            }
        }
        [HttpPost]
        public IActionResult UploadImage(IFormFile Upload)
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect("/Media/Index?Message=" + _context.ReturnMsg(l, "Error", "1"));
            }
            else
            {
                if (!_context.CheckRoles(805))//804 - Overwrite;805 - Attachments
                {
                    HTTP_CODE.WriteLogAction("GrantToAttachments: " + _context.GetLanguageLable("YouAreNotIsGrantToAttachments") + " " + _context.GetLanguageLable("Attachments"));
                    return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToAttachments} {Attachments}", "Error", "1"));
                }
                else
                {
                    //var file = _context.GetFormValue("Upload");
                    string HRSStart = _context.GetSession("HRSUpload");
                    //var file1 = Request.Form["Upload"];
                    if (HRSStart != "Images") // StopHacking
                    {
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{StopHacking}", "Error", "1"));
                    }

                    // check file null
                    if (Upload == null)
                    {
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{FileIsNull}", "Error", "1"));
                    }
                    if (Upload.Length <= 0)
                    {
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{FileIsNull}", "Error", "1"));
                    }
                    // check extension
                    string FileExtension = _context.AppConfig.GetFileExtension(Upload.FileName).ToLower(); bool FileOK = false;
                    string[] allowedExtensions = _context._appConfig.ImageFile.ToString().Split(new string[] { ";" }, StringSplitOptions.None);
                    FileOK = Tools.CheckFileExtension(FileExtension, allowedExtensions);
                    if (!FileOK)
                    {
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{FileExtensionIsNotAllowed}", "Error", "1"));
                    }
                    string MimeType = Tools.GetValueFormKeys(_context._appConfig.MimeType.ToString(), FileExtension);
                    string TabIndex = _context.GetRequestTabIndex();
                    string InputName = _context.GetRequestVal("InputName");
                    //string Creator = _context.GetSession("UserID");
                    string guid = Guid.NewGuid().ToString();
                    string FileName = _context.AppConfig.GetFileName(Upload.FileName);
                    string FilePath = _context.AppConfig.FolderRoot + FolderRootImage + TabIndex + "\\" + guid;//Path.Combine(Directory.GetCurrentDirectory(), "docRoot/", fileName);
                    _context.AppConfig.PathCreateDirectory(FilePath);
                    string HRSImageWork = FilePath + "\\" + FileName;
                    //_context.SetSession("HRSImageWork", FilePath + "\\" + FileName);
                    //_context.SetSession("HRSFileName", FileName);
                    using (var stream = _context.AppConfig.CreateFile(HRSImageWork))
                    {
                        Upload.CopyTo(stream);
                        stream.Close();
                    }

                    StringBuilder r1 = new StringBuilder(); string r;

                    r1.Append(UIDef.UIHeaderPopup(_context, _context.GetLanguageLable("AttachmentImage"), "UploadImagePost") +
                        Environment.NewLine + "<form method=\"post\" action=\"/Media/Crop\" id=\"frmAtt\" name=\"frmAtt\" enctype=\"multipart/form-data\">" +
                        UIDef.UIHidden("HRSImageWork", HRSImageWork) +
                        UIDef.UIHidden("HRSFileName", FileName) +
                        UIDef.UIHidden("FileExtension", FileExtension) +
                        UIDef.UIHidden("MimeType", MimeType) +
                        UIDef.UIHidden("guid", guid) +
                        UIDef.UIHidden("TabIndex", TabIndex) +
                        UIDef.UIHidden("InputName", InputName) +
                        Environment.NewLine + "<div class=\"row inline-input\">" +
                        Environment.NewLine + "<div class=\"col-sm-12\">" +
                        Environment.NewLine + "<div class=\"form-group row\">" +
                        UIDef.UIButton("bntSearch", _context.GetLanguageLable("Crop")) + "</div>" +
                        Environment.NewLine + "<div class=\"form-group row\">" +
                        Environment.NewLine + "<div id=\"pnlCrop\">" +
                        Environment.NewLine + "<img id=\"imgCrop\" src=\"" + "/docRoot/" + TabIndex + "/" + guid + "/" + FileName + "\" />" +
                        Environment.NewLine + "<input type=\"hidden\" name=\"X\" id=\"X\" />" +
                        Environment.NewLine + "<input type=\"hidden\" name=\"Y\" id=\"Y\" />" +
                        Environment.NewLine + "<input type=\"hidden\" name=\"W\" id=\"W\" />" +
                        Environment.NewLine + "<input type=\"hidden\" name=\"H\" id=\"H\" />" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</form>" +
                        UIDef.UIFooterPopup(_context, "UploadImagePost"));
                    r = r1.ToString();
                    return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                }
            }
        }
        static byte[] Crop(ToolFolder t, string Img, int Width, int Height, int X, int Y)
        {
            try
            {
                using (Image OriginalImage = Image.FromFile(Img))
                {
                    using (Bitmap bmp = new Bitmap(Width, Height))
                    {
                        bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                        using (Graphics Graphic = Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(OriginalImage, new Rectangle(0, 0, Width, Height), X, Y, Width, Height, GraphicsUnit.Pixel);
                            var ms = t.CreateMemoryStream();
                            bmp.Save(ms, OriginalImage.RawFormat);
                            return ms.GetBuffer();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }
        [HttpPost]
        public IActionResult Crop()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect("/Media/Index?Message=" + _context.ReturnMsg(l, "Error", "1"));
            }
            else
            {
                if (!_context.CheckRoles(805))//804 - Overwrite;805 - Attachments
                {
                    HTTP_CODE.WriteLogAction("GrantToAttachments: " + _context.GetLanguageLable("YouAreNotIsGrantToAttachments") + " " + _context.GetLanguageLable("Attachments"));
                    return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToAttachments} {Attachments}", "Error", "1"));
                }
                else
                {
                    string ImageName = _context.GetFormValue("HRSImageWork");
                    string FileName = _context.GetFormValue("HRSFileName");
                    string FileExtension = _context.GetFormValue("FileExtension");
                    string MimeType = _context.GetFormValue("MimeType");
                    string guid = _context.GetFormValue("guid");
                    if (!_context.AppConfig.PathIsFile(ImageName)) // StopHacking
                    {
                        HTTP_CODE.WriteLogAction("ImageName: " + ImageName);
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{FileCropingIsNull}", "Error", "1"));
                    }
                    int w = int.Parse(_context.GetRequestVal("W"));
                    int h = int.Parse(_context.GetRequestVal("H"));
                    int x = int.Parse(_context.GetRequestVal("X"));
                    int y = int.Parse(_context.GetRequestVal("Y"));
                    string TabIndex = _context.GetRequestTabIndex();
                    string InputName = _context.GetRequestVal("InputName");
                    string FilePath = _context.AppConfig.FolderRoot + FolderRootImage + TabIndex + "\\" + guid + "\\Crop";
                    _context.AppConfig.PathCreateDirectory(FilePath);
                    string path = FilePath + "\\" + FileName;
                    byte[] CropImage = Crop(_context.AppConfig, ImageName, w, h, x, y);
                    using (var ms = _context.AppConfig.CreateMemoryStream(CropImage, 0, CropImage.Length))
                    {
                        ms.Write(CropImage, 0, CropImage.Length);
                        using (Image CroppedImage = Image.FromStream(ms, true))
                        {
                            CroppedImage.Save(path, CroppedImage.RawFormat);
                        }
                    }
                    ImageName = FolderRootImage + TabIndex + "\\" + guid + "\\" + FileName;
                    path = FolderRootImage + TabIndex + "\\" + guid + "\\Crop" + "\\" + FileName;
                    dynamic d = JObject.Parse("{\"parameterInput\":[" +
                        "{\"ParamName\":\"Creator\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + _context.GetSession("UserID") + "\"}," +
                        "{\"ParamName\":\"AttachmentID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                        "{\"ParamName\":\"AttachmentCode\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"AutoNumber;Attachment;Image;000000\"}," +
                        "{\"ParamName\":\"AttachmentName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"Images\"}," +
                        "{\"ParamName\":\"TabIndex\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + TabIndex + "\"}," +
                        "{\"ParamName\":\"InputName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + InputName + "\"}," +
                        "{\"ParamName\":\"Keywords\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"Images\"}," +
                        "{\"ParamName\":\"UrlOriginal\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + ImageName.Replace("\\", "||") + "\"}," +
                        "{\"ParamName\":\"UrlCrop\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + path.Replace("\\", "||") + "\"}," +
                        "{\"ParamName\":\"FileExtention\", \"ParamType\":\"22\", \"ParamInOut\":\"1\", \"ParamLength\":\"10\", \"InputValue\":\"" + FileExtension + "\"}," +
                        "{\"ParamName\":\"MimeType\", \"ParamType\":\"22\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"" + MimeType + "\"}," +
                        "{\"ParamName\":\"Status\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                        "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                        "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"0\", \"ParamInOut\":\"3\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}]}");
                    string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                    bosDAO.ExecuteStore("UploadImage", "SP_CMS__Attachment_Update", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                    d = JObject.Parse("{" + parameterOutput + "}");
                    string r = ""; long docId = long.Parse(d.ParameterOutput.ResponseStatus.ToString());
                    if (docId > 0)
                    {
                        r = "<script language='javascript'>" +
                            "var parent=window.opener;" +
                            "if (parent==null) parent=dialogArguments;" +
                            "parent.focus();" +
                            "parent.addattachedfile_" + InputName + "('" + docId + "');" +
                            "window.close();</script>";
                    }
                    else
                    {
                        r = "<script language='javascript'>" +
                            "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable(d.ParameterOutput.Message.ToString()) + "', '', '0');" + //alert('" + _context.GetLanguageLable(d.ParameterOutput.Message.ToString()) + "');
                            "window.close();</script>";
                    }
                    return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                    //return Json(JObject.Parse("{\"ResponseCode\":" + d.ParameterOutput.ResponseStatus.ToString() + 
                    //    ", \"Message\":\"" + _context.GetLanguageLable(d.ParameterOutput.Message.ToString()) + "\", \"ImageId\":\"" + d.ParameterOutput.ResponseStatus.ToString() + "\"}"));
                }
            }
        }
        #endregion

        #region UploadFile Method
        [HttpGet]
        public IActionResult UploadFile()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect("/Media/Index?Message=" + _context.ReturnMsg(l, "Error", "1"));
            }
            else
            {
                if (!_context.CheckRoles(805))//804 - Overwrite;805 - Attachments
                {
                    HTTP_CODE.WriteLogAction("GrantToAttachments: " + _context.GetLanguageLable("YouAreNotIsGrantToAttachments") + " " + _context.GetLanguageLable("Attachments"));
                    return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToAttachments} {Attachments}", "Error", "1"));
                }
                else
                {
                    string TabIndex = _context.GetRequestTabIndex();
                    if (TabIndex == "") TabIndex = "DBConfig";
                    string InputName = _context.GetRequestVal("InputName");
                    _context.SetSession("HRSUpload", "Files");
                    StringBuilder r1 = new StringBuilder(); string r;
                    r1.Append(UIDef.UIHeaderPopup(_context, _context.GetLanguageLable("AttachmentFiles"), "UploadFile"));
                    r1.Append(Environment.NewLine + "<form method=\"post\" role=\"form\" action=\"/Media/UploadFile\" id=\"frmAtt\" name=\"frmAtt\" enctype=\"multipart/form-data\">" +
                        Environment.NewLine + "<div id=\"pnlUpload\" class=\"row inline-input\">" +
                        Environment.NewLine + "<div class=\"col-sm-4\">" +
                        UIDef.UIHidden("TabIndex", TabIndex) +
                        UIDef.UIHidden("InputName", InputName) +
                        Environment.NewLine + "<div class=\"form-group row\">" +
                        UIDef.UIButton("bntSearch", _context.GetLanguageLable("Upload"), true, " class=\"btn inport\"") + "</div>" +
                        Environment.NewLine + "<div class=\"form-group row\">" +
                        Environment.NewLine + "<label class=\"col-form-label active\">" + _context.GetLanguageLable("ChoiceFile") + "</label>" +
                        Environment.NewLine + "<div class=\"file-field\">" +
                        Environment.NewLine + "<div class=\"btn btn-primary btn-sm float-left waves-effect waves-light\">" +
                        Environment.NewLine + "<span>Chọn file</span><input type=\"file\" name=\"Upload\" id=\"Upload\" />" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "<div class=\"file-path-wrapper\"><input class=\"file-path validate\" readonly type=\"text\" placeholder=\"" + _context.GetLanguageLable("ChoiceFile") + "\"></div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" +
                        Environment.NewLine + "</div>" + 
                        Environment.NewLine + "</form>" +
                        UIDef.UIFooterPopup(_context, "UploadFile"));
                    r = r1.ToString();
                    r1 = null;
                    return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                }
            }
        }
        [HttpPost]
        public IActionResult UploadFile(IFormFile Upload)
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            string l = _context.CheckLogin(ref bosDAO);
            string TabIndex = _context.GetRequestTabIndex();
            string InputName = _context.GetRequestVal("InputName");
            if (l != "")
            {
                return Redirect("/Media/Index?Message=" + _context.ReturnMsg(l, "Error", "1"));
            }
            else
            {
                if (!_context.CheckRoles(805))//804 - Overwrite;805 - Attachments
                {
                    HTTP_CODE.WriteLogAction("GrantToAttachments: " + _context.GetLanguageLable("YouAreNotIsGrantToAttachments") + " " + _context.GetLanguageLable("Attachments"));
                    return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToAttachments} {Attachments}", "Error", "1"));
                }
                else
                {
                    //var file = _context.GetFormValue("Upload");
                    string HRSStart = _context.GetSession("HRSUpload");
                    //var file1 = Request.Form["Upload"];
                    if (HRSStart != "Files") // StopHacking
                    {
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{StopHacking}", "Error", "1"));
                    }

                    // check file null
                    if (Upload == null)
                    {
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{FileIsNull}", "Error", "1"));
                    }
                    if (Upload.Length <= 0)
                    {
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{FileIsNull}", "Error", "1"));
                    }
                    // check extension
                    string FileExtension = _context.AppConfig.GetFileExtension(Upload.FileName).ToLower(); bool FileOK = false;
                    string[] allowedExtensions = _context._appConfig.DocsFile.ToString().Split(new string[] { ";" }, StringSplitOptions.None);
                    FileOK = Tools.CheckFileExtension(FileExtension, allowedExtensions);
                    if (!FileOK)
                    {
                        return Redirect("/Media/Index?Message=" + _context.ReturnMsg("{FileExtensionIsNotAllowed}", "Error", "1"));
                    }


                    //string Creator = _context.GetSession("UserID");
                    string guid = Guid.NewGuid().ToString();
                    string FileName = _context.AppConfig.GetFileName(Upload.FileName);
                    string FilePath = _context.AppConfig.FolderRoot + FolderRoot + TabIndex + "\\" + guid;//Path.Combine(Directory.GetCurrentDirectory(), "docRoot/", fileName);
                    _context.AppConfig.PathCreateDirectory(FilePath);
                    string HRSImageWork = FilePath + "\\" + FileName;
                    string MimeType = Tools.GetValueFormKeys(_context._appConfig.MimeType.ToString(), FileExtension);
                    //_context.SetSession("HRSImageWork", FilePath + "\\" + FileName);
                    //_context.SetSession("HRSFileName", FileName);
                    using (var stream = _context.AppConfig.CreateFile(HRSImageWork))
                    {
                        Upload.CopyTo(stream);
                        stream.Close();
                    }
                    HRSImageWork = FolderRoot + TabIndex + "\\" + guid + "\\" + FileName;
                    dynamic d = JObject.Parse("{\"parameterInput\":[" +
                        "{\"ParamName\":\"Creator\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + _context.GetSession("UserID") + "\"}," +
                        "{\"ParamName\":\"AttachmentID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                        "{\"ParamName\":\"AttachmentCode\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"AutoNumber;Attachment;Image;000000\"}," +
                        "{\"ParamName\":\"AttachmentName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"Images\"}," +
                        "{\"ParamName\":\"TabIndex\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + TabIndex + "\"}," +
                        "{\"ParamName\":\"InputName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + InputName + "\"}," +
                        "{\"ParamName\":\"Keywords\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"Images\"}," +
                        "{\"ParamName\":\"UrlOriginal\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + HRSImageWork.Replace("\\", "||") + "\"}," +
                        "{\"ParamName\":\"UrlCrop\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + HRSImageWork.Replace("\\", "||") + "\"}," +
                        "{\"ParamName\":\"FileExtention\", \"ParamType\":\"22\", \"ParamInOut\":\"1\", \"ParamLength\":\"10\", \"InputValue\":\"" + FileExtension + "\"}," +
                        "{\"ParamName\":\"MimeType\", \"ParamType\":\"22\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"" + MimeType + "\"}," +
                        "{\"ParamName\":\"Status\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"1\"}," +
                        "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                        "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"0\", \"ParamInOut\":\"3\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}]}");
                    string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                    bosDAO.ExecuteStore("UploadImage", "SP_CMS__Attachment_Update", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                    d = JObject.Parse("{" + parameterOutput + "}");
                    string r = ""; long docId = long.Parse(d.ParameterOutput.ResponseStatus.ToString());
                    if (docId > 0)
                    {
                        r = "<script language='javascript'>" +
                            "var parent=window.opener;" +
                            "if (parent==null) parent=dialogArguments;" +
                            "parent.focus();" +
                            "parent.addattachedfile_" + InputName + "('" + docId + "', '" + FileName + "');" +
                            "window.close();</script>";
                    }
                    else
                    {
                        r = "<script language='javascript'>" +
                            "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable(d.ParameterOutput.Message.ToString()) + "', '', '0');" +//alert('" + _context.GetLanguageLable(d.ParameterOutput.Message.ToString()) + "');
                            "window.close();</script>";
                    }
                    return Content(r, "text/html; charset=utf-8", Encoding.UTF8);
                    //return Json(Newtonsoft.Json.Linq.JObject.Parse("{\"ResponseCode\":200, \"Message\":\"UploadIsSuccess\", \"ImageId\":\"1\", \"ImageUrl\":\"" +
                    //    _context.GetHost() + "/docRoot/" + TabIndex + "/" + guid + "/Crop" + "/" + FileName + "\"}"));
                }
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