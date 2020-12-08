using System;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HRS.Models;
using Utils;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace HRS.Controllers
{
    public class HomeController : Controller
    {
        private HRSCache _cache;
        public HomeController(IMemoryCache memoryCache)
        {
            _cache = new HRSCache(memoryCache);
        }
        public IActionResult Index()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
                                                    //////////////XỬ LÝ DOCS/////////////////////////////////////
                                                    //WordProcessing w = new WordProcessing(@"E:\00.TinhVan\HRS\HRS\wwwroot\docRoot\Users\578ab07a-d4f5-4169-bfba-405c2fc99925\InCV.docx",
                                                    //    @"E:\00.TinhVan\HRS\HRS\wwwroot\docRoot\Users\578ab07a-d4f5-4169-bfba-405c2fc99925\InCV123.docx", _context,
                                                    //    "{ \"DataVar\": {   \"Count\": 0,   \"Items\": [     {       \"STT\": \"STT:1\",       \"VN_FULLNAME\": \"VN_FULLNAME:827252\",      " +
                                                    //    " \"VN_FULLNAME1\": \"VN_FULLNAME1:TQ\",       \"YHEIGHT\": \"YHEIGHT:China\",       \"YHEIGHT1\": \"YHEIGHT1:1\",       " +
                                                    //    "\"BIRTH_DATE\": \"BIRTH_DATE:827143\",       \"BIRTH_DATE2\": \"BIRTH_DATE2:NganDT\",       \"YWEIGHT\": \"YWEIGHT:2\",       " +
                                                    //    "\"YWEIGHT1\": \"YWEIGHT1:827252\",		\"BIRTH_PLACE1\": \"BIRTH_PLACE1:2\",       \"BIRTH_PLACE\": \"BIRTH_PLACE:827252\",		" +
                                                    //    "\"PER_ADDRESS1\": \"PER_ADDRESS1:2\",       \"PER_ADDRESS\": \"PER_ADDRESS:827252\"     }]}, \"DATA1\": {   \"Count\": 3,   \"Items\": [     {      " +
                                                    //    " \"STT\": \"STT-Data1:1\",       \"START_DATE\": \"START_DATE-Data1:827252\",       \"END_DATE\": \"END_DATE-Data1:TQ\",     " +
                                                    //    "  \"YSCHOOL_NAME\": \"YSCHOOL_NAME-Data1:China\",       \"YMAJOR\": \"YMAJOR-Data1:1\",       \"YTYPE\": \"YTYPE-Data1:827143\",       " +
                                                    //    "\"YLEVELS\": \"YLEVELS-Data1:NganDT\",       \"CreatorVersion\": \"CreatorVersion-Data1:2\",       \"OT_NationIDList\": \"OT_NationIDList-Data1: 827252\"     }, " +
                                                    //    "    {       \"STT\": \"2\",       \"START_DATE\": \"689\",       \"END_DATE\": \"VN\",       \"YSCHOOL_NAME\": \"VietName\",       \"YMAJOR\": \"1\",     " +
                                                    //    "  \"YTYPE\": \"92\",       \"YLEVELS\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"689\"     },    " +
                                                    //    " {       \"STT\": \"3\",       \"START_DATE\": \"139\",       \"END_DATE\": \"KR\",       \"YSCHOOL_NAME\": \"Korea\",     " +
                                                    //    "  \"YMAJOR\": \"1\",       \"YTYPE\": \"92\",       \"YLEVELS\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",      " +
                                                    //    " \"OT_NationIDList\": \"139\"     }   ] }, \"DATA3\": {   \"Count\": 3,   \"Items\": [     {       \"STT\": \"DATA3:1\",     " +
                                                    //    "  \"FROM_DATE\": \"DATA3-FROM_DATE:827252\",       \"TO_DATE\": \"DATA3-TO_DATE:TQ\",       \"COMPANY\": \"China\",       \"POSITION\": \"1\",     " +
                                                    //    "  \"GROSS_SAL\": \"DATA3:827143\",       \"TER_REASON\": \"NganDT\",       \"CreatorVersion\": \"2\",       \"OT_NationIDList\": \"827252\"     },   " +
                                                    //    "  {       \"STT\": \"2\",       \"FROM_DATE\": \"689\",       \"TO_DATE\": \"VN\",       \"COMPANY\": \"VietName\",       \"POSITION\": \"1\",    " +
                                                    //    "   \"GROSS_SAL\": \"92\",       \"TER_REASON\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"689\"     },     {      " +
                                                    //    " \"STT\": \"3\",       \"FROM_DATE\": \"139\",       \"TO_DATE\": \"KR\",       \"COMPANY\": \"Korea\",       \"POSITION\": \"1\",     " +
                                                    //    "  \"GROSS_SAL\": \"92\",       \"TER_REASON\": \"TVGKIENNT\",       \"CreatorVersion\": \"1\",       \"OT_NationIDList\": \"139\"     }   ] }}");
                                                    //w = null;
                                                    //return Redirect("/docRoot/Users/578ab07a-d4f5-4169-bfba-405c2fc99925/InCV123.docx");

            //////////////GUI MAIL/////////////////////////////////////
            //string[] fList = new string[1]; fList[0] = @"E:\00.TinhVan\HRS\LeaveExpAlertMail.txt";
            //string[] Eto = new string[1]; Eto[0] = @"chiennt@tinhvan.com";
            //string[] Ecc = new string[1]; Ecc[0] = @"hongdx1@tinhvan.com";
            //string[] EBcc = new string[1]; EBcc[0] = @"vungnt@tinhvan.com";
            //PlaintextProcessing p = new PlaintextProcessing("<div class=\"WordSection1\">	<p class=\"MsoListParagraph\" style=\"text-indent:-.25in;mso-list:l0 level1 lfo1\"></p>	<p class=\"MsoNormal\">Dear các anh/chị trưởng bộ phận,<o:p></o:p></p>	<p class=\"MsoNormal\">Hệ thống xin thông báo, các trường hợp xin nghỉ/ ra vào cổng sắp hết thời gian phê duyệt của bộ phận các anh/ chị như sau: <o:p></o:p></p>	<p class=\"MsoNormal\"><o:p>&nbsp;</o:p></p>	<table class=\"MsoNormalTable\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"827\" style=\"width:620.0pt;border-collapse:collapse\">		<tr style=\"height:23.25pt\">			<td width=\"827\" nowrap=\"\" colspan=\"10\" style=\"width:620.0pt;padding:0in 5.4pt 0in 5.4pt;height:23.25pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">					<b><span style=\"font-size:12.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#333333\">DANH SÁCH XIN NGHỈ / RA NGOÀI CHƯA XEM XÉT, PHÊ DUYỆT<o:p></o:p></span></b></p>			</td>		</tr>		<tr style=\"height:12.0pt\">			<td width=\"56\" nowrap=\"\" valign=\"bottom\" style=\"width:42.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"71\" nowrap=\"\" valign=\"bottom\" style=\"width:53.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"83\" nowrap=\"\" valign=\"bottom\" style=\"width:62.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"84\" nowrap=\"\" valign=\"bottom\" style=\"width:63.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"56\" nowrap=\"\" valign=\"bottom\" style=\"width:42.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"100\" nowrap=\"\" valign=\"bottom\" style=\"width:75.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"80\" nowrap=\"\" valign=\"bottom\" style=\"width:60.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"56\" nowrap=\"\" valign=\"bottom\" style=\"width:42.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"56\" nowrap=\"\" valign=\"bottom\" style=\"width:42.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>			<td width=\"73\" nowrap=\"\" valign=\"bottom\" style=\"width:55.0pt;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\"></td>		</tr>		<tr style=\"height:12.0pt\">			<td width=\"56\" rowspan=\"2\" style=\"width:42.0pt;border:solid #A6A6A6 1.0pt;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">MSNV<o:p></o:p></span></b></p>			</td>			<td width=\"71\" rowspan=\"2\" style=\"width:53.0pt;border:solid #A6A6A6 1.0pt;border-left:none;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Bộ phận<o:p></o:p></span></b></p>			</td>			<td width=\"83\" rowspan=\"2\" style=\"width:62.0pt;border:solid #A6A6A6 1.0pt;border-left:none;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Họ tên<o:p></o:p></span></b></p>			</td>			<td width=\"84\" rowspan=\"2\" style=\"width:63.0pt;border:solid #A6A6A6 1.0pt;border-left:none;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Ngày nghỉ<o:p></o:p></span></b></p>			</td>			<td width=\"56\" rowspan=\"2\" style=\"width:42.0pt;border:solid #A6A6A6 1.0pt;border-left:none;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Thời gian nghỉ<o:p></o:p></span></b></p>			</td>			<td width=\"100\" rowspan=\"2\" style=\"width:75.0pt;border:solid #A6A6A6 1.0pt;border-left:none;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Lý do<o:p></o:p></span></b></p>			</td>			<td width=\"80\" rowspan=\"2\" style=\"width:60.0pt;border:solid #A6A6A6 1.0pt;border-left:none;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Kết quả<o:p></o:p></span></b></p>			</td>			<td width=\"112\" colspan=\"2\" style=\"width:84.0pt;border:solid #A6A6A6 1.0pt;border-left:none;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Tình trạng<o:p></o:p></span></b></p>			</td>			<td width=\"73\" rowspan=\"2\" style=\"width:55.0pt;border:solid #A6A6A6 1.0pt;border-left:none;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:12.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Thời gian còn lại để phê duyệt<o:p></o:p></span></b></p>			</td>		</tr>		<tr style=\"height:24.0pt\">			<td width=\"56\" style=\"width:42.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:24.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Manager<o:p></o:p></span></b></p>			</td>			<td width=\"56\" style=\"width:42.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:#BFBFBF;padding:0in 5.4pt 0in 5.4pt;height:24.0pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><b><span style=\"font-size:9.0pt;font-family:&quot;Helvetica&quot;,sans-serif;color:#333333\">Director<o:p></o:p></span></b></p>			</td>    		</tr>				«TableStart:DATA»		<tr style=\"height:19.5pt\">			<td width=\"56\" style=\"width:42.0pt;border:solid #A6A6A6 1.0pt;border-top:none;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«EmployeeCode»</p></td>			<td width=\"71\" style=\"width:53.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«OrganizationName»</p>			</td>			<td width=\"100\" style=\"width:100.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«EmployeeName»</p>			</td>			<td width=\"84\" style=\"width:63.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«LeaveDate»</p>			</td>			<td width=\"56\" style=\"width:42.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«LeaveTime»</p>			</td>			<td width=\"100\" style=\"width:100.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«LeaveReason»</p>			</td>			<td width=\"80\" style=\"width:60.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«StatusName»</p>			</td>			<td width=\"56\" style=\"width:42.0pt;border:solid #A6A6A6 1.0pt;border-top:none;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«Manager»</p>			</td>			<td width=\"56\" style=\"width:42.0pt;border-top:none;border-left:none;border-bottom:solid #A6A6A6 1.0pt;border-right:solid #A6A6A6 1.0pt;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«Director»</p>			</td>			<td width=\"56\" style=\"width:42.0pt;border:solid #A6A6A6 1.0pt;border-top:none;background:white;padding:0in 5.4pt 0in 5.4pt;height:19.5pt\">				<p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\">«ApprovalDate»</p>			</td>		</tr>		«TableEnd:DATA»			</table>	<p class=\"MsoNormal\"><o:p>&nbsp;</o:p></p>	<p class=\"MsoNormal\">		Để phê duyệt, các anh/ chị 		<u><a href=\"«Var:URLPortal»/HR/Approval\"><span style=\"color:#4472C4\">click vào đây</span></a></u>		<span style=\"color:#4472C4\"> </span>để đăng nhập hệ thống và&nbsp; tiến hành phê duyệt.<o:p></o:p>	</p>	<p class=\"MsoNormal\">Trân trọng!<o:p></o:p></p>	<p class=\"MsoNormal\">«Var:AccountName»<o:p></o:p></p></div>",
            //    _context,
            //    "{ \"DataVar\": {\"ItemType\": {\"STT\":\"int\",\"DASHBOARDCONFIGID\":\"bigint\",\"EMPLOYEECODE\":\"nvarchar\",\"ORGANIZATIONNAME\":\"nvarchar\",\"FUNCTIONID\":\"bigint\",\"EMPLOYEENAME\":\"nvarchar\",\"REPORTNAME1\":\"nvarchar\",\"LEAVEDATE\":\"nvarchar\",\"REPORTNAME2\":\"nvarchar\",\"REPORTLIST2\":\"nvarchar\",\"REPORTNAME3\":\"nvarchar\",\"REPORTLIST3\":\"nvarchar\",\"REPORTNAME4\":\"nvarchar\",\"REPORTLIST4\":\"nvarchar\",\"REPORTNAME5\":\"nvarchar\",\"REPORTLIST5\":\"nvarchar\",\"CREATORID\":\"bigint\",\"CREATORNAME\":\"nvarchar\",\"CREATORVERSION\":\"int\"}, \"Items\":[{\"STT\":\"1\",\"DASHBOARDCONFIGID\":\"828497\",\"EMPLOYEECODE\":\"HU_Dashboard\",\"ORGANIZATIONNAME\":\"HU_Dashboard\",\"FUNCTIONID\":\"828495\",\"EMPLOYEENAME\":\"HU_Dashboard - HU_Dashboard\",\"REPORTNAME1\":\"Thống kê theo hợp đồng||Thống kê theo giới tính||Thống kê theo trình độ văn hóa^StatisticsBySeniority^Report__Reminder\",\"LEAVEDATE\":\"/Report/Chart?RptID=Report__Contract&MenuOn=Off||/Report/Chart?RptID=Report__Gender&MenuOn=Off||/Report/Chart?RptID=Report_Level&MenuOn=Off^/Report/Chart?RptID=Report__Seniority&MenuOn=Off^/Report?RptID=Report__Reminder&MenuOn=Off	\",\"REPORTNAME2\":\"Thống kê tổng số nhân viên theo năm||Thống kê nhân viên theo tháng||Tổng số lao động tăng mới theo tháng||Tổng số lao động nghỉ việc theo tháng^Report__Quickly\",\"REPORTLIST2\":\"/Report/Chart?RptID=Report_Total&MenuOn=Off||/Report/Chart?RptID=Report_Emp_Month&MenuOn=Off||/Report/Chart?RptID=Report_EmpAug_Month&MenuOn=Off||/Report/Chart?RptID=Report_EmpTer_Month&MenuOn=Off^/Report?RptID=Report__Quickly&MenuOn=Off\",\"REPORTNAME3\":\"\",\"REPORTLIST3\":\"\",\"REPORTNAME4\":\"\",\"REPORTLIST4\":\"\",\"REPORTNAME5\":\"\",\"REPORTLIST5\":\"\",\"CREATORID\":\"549357\",\"CREATORNAME\":\"AnhLT12\",\"CREATORVERSION\":\"6\"}]},\"DATA\": {\"ItemType\": {\"STT\":\"int\",\"DASHBOARDCONFIGID\":\"bigint\",\"EMPLOYEECODE\":\"nvarchar\",\"ORGANIZATIONNAME\":\"nvarchar\",\"FUNCTIONID\":\"bigint\",\"EMPLOYEENAME\":\"nvarchar\",\"REPORTNAME1\":\"nvarchar\",\"LEAVEDATE\":\"nvarchar\",\"REPORTNAME2\":\"nvarchar\",\"REPORTLIST2\":\"nvarchar\",\"REPORTNAME3\":\"nvarchar\",\"REPORTLIST3\":\"nvarchar\",\"REPORTNAME4\":\"nvarchar\",\"REPORTLIST4\":\"nvarchar\",\"REPORTNAME5\":\"nvarchar\",\"REPORTLIST5\":\"nvarchar\",\"CREATORID\":\"bigint\",\"CREATORNAME\":\"nvarchar\",\"CREATORVERSION\":\"int\"}, \"Items\":[{\"STT\":\"1\",\"DASHBOARDCONFIGID\":\"828497\",\"EMPLOYEECODE\":\"HU_Dashboard\",\"ORGANIZATIONNAME\":\"HU_Dashboard\",\"FUNCTIONID\":\"828495\",\"EMPLOYEENAME\":\"HU_Dashboard - HU_Dashboard\",\"REPORTNAME1\":\"Thống kê theo hợp đồng||Thống kê theo giới tính||Thống kê theo trình độ văn hóa^StatisticsBySeniority^Report__Reminder\",\"LEAVEDATE\":\"/Report/Chart?RptID=Report__Contract&MenuOn=Off||/Report/Chart?RptID=Report__Gender&MenuOn=Off||/Report/Chart?RptID=Report_Level&MenuOn=Off^/Report/Chart?RptID=Report__Seniority&MenuOn=Off^/Report?RptID=Report__Reminder&MenuOn=Off	\",\"REPORTNAME2\":\"Thống kê tổng số nhân viên theo năm||Thống kê nhân viên theo tháng||Tổng số lao động tăng mới theo tháng||Tổng số lao động nghỉ việc theo tháng^Report__Quickly\",\"REPORTLIST2\":\"/Report/Chart?RptID=Report_Total&MenuOn=Off||/Report/Chart?RptID=Report_Emp_Month&MenuOn=Off||/Report/Chart?RptID=Report_EmpAug_Month&MenuOn=Off||/Report/Chart?RptID=Report_EmpTer_Month&MenuOn=Off^/Report?RptID=Report__Quickly&MenuOn=Off\",\"REPORTNAME3\":\"\",\"REPORTLIST3\":\"\",\"REPORTNAME4\":\"\",\"REPORTLIST4\":\"\",\"REPORTNAME5\":\"\",\"REPORTLIST5\":\"\",\"CREATORID\":\"549357\",\"CREATORNAME\":\"AnhLT12\",\"CREATORVERSION\":\"6\"},{\"STT\":\"1\",\"DASHBOARDCONFIGID\":\"828497\",\"EMPLOYEECODE\":\"HU_Dashboard\",\"ORGANIZATIONNAME\":\"HU_Dashboard\",\"FUNCTIONID\":\"828495\",\"EMPLOYEENAME\":\"HU_Dashboard - HU_Dashboard\",\"REPORTNAME1\":\"Thống kê theo hợp đồng||Thống kê theo giới tính||Thống kê theo trình độ văn hóa^StatisticsBySeniority^Report__Reminder\",\"LEAVEDATE\":\"/Report/Chart?RptID=Report__Contract&MenuOn=Off||/Report/Chart?RptID=Report__Gender&MenuOn=Off||/Report/Chart?RptID=Report_Level&MenuOn=Off^/Report/Chart?RptID=Report__Seniority&MenuOn=Off^/Report?RptID=Report__Reminder&MenuOn=Off	\",\"REPORTNAME2\":\"Thống kê tổng số nhân viên theo năm||Thống kê nhân viên theo tháng||Tổng số lao động tăng mới theo tháng||Tổng số lao động nghỉ việc theo tháng^Report__Quickly\",\"REPORTLIST2\":\"/Report/Chart?RptID=Report_Total&MenuOn=Off||/Report/Chart?RptID=Report_Emp_Month&MenuOn=Off||/Report/Chart?RptID=Report_EmpAug_Month&MenuOn=Off||/Report/Chart?RptID=Report_EmpTer_Month&MenuOn=Off^/Report?RptID=Report__Quickly&MenuOn=Off\",\"REPORTNAME3\":\"\",\"REPORTLIST3\":\"\",\"REPORTNAME4\":\"\",\"REPORTLIST4\":\"\",\"REPORTNAME5\":\"\",\"REPORTLIST5\":\"\",\"CREATORID\":\"549357\",\"CREATORNAME\":\"AnhLT12\",\"CREATORVERSION\":\"6\"}, {\"STT\":\"1\",\"DASHBOARDCONFIGID\":\"828497\",\"EMPLOYEECODE\":\"HU_Dashboard\",\"ORGANIZATIONNAME\":\"HU_Dashboard\",\"FUNCTIONID\":\"828495\",\"EMPLOYEENAME\":\"HU_Dashboard - HU_Dashboard\",\"REPORTNAME1\":\"Thống kê theo hợp đồng||Thống kê theo giới tính||Thống kê theo trình độ văn hóa^StatisticsBySeniority^Report__Reminder\",\"LEAVEDATE\":\"/Report/Chart?RptID=Report__Contract&MenuOn=Off||/Report/Chart?RptID=Report__Gender&MenuOn=Off||/Report/Chart?RptID=Report_Level&MenuOn=Off^/Report/Chart?RptID=Report__Seniority&MenuOn=Off^/Report?RptID=Report__Reminder&MenuOn=Off	\",\"REPORTNAME2\":\"Thống kê tổng số nhân viên theo năm||Thống kê nhân viên theo tháng||Tổng số lao động tăng mới theo tháng||Tổng số lao động nghỉ việc theo tháng^Report__Quickly\",\"REPORTLIST2\":\"/Report/Chart?RptID=Report_Total&MenuOn=Off||/Report/Chart?RptID=Report_Emp_Month&MenuOn=Off||/Report/Chart?RptID=Report_EmpAug_Month&MenuOn=Off||/Report/Chart?RptID=Report_EmpTer_Month&MenuOn=Off^/Report?RptID=Report__Quickly&MenuOn=Off\",\"REPORTNAME3\":\"\",\"REPORTLIST3\":\"\",\"REPORTNAME4\":\"\",\"REPORTLIST4\":\"\",\"REPORTNAME5\":\"\",\"REPORTLIST5\":\"\",\"CREATORID\":\"549357\",\"CREATORNAME\":\"AnhLT12\",\"CREATORVERSION\":\"6\"}]}}");
            //SendmailProcessing mail = new SendmailProcessing("mail.tinhvan.com", 587, true,
            //    "Nguyen Thanh Chien", "chiennt@tinhvan.com", "pE@ug*m@n20u", "",
            //    Eto, Ecc, EBcc,
            //    "Test template", p.DestinationData, fList);
            //mail.EmailSending();
            //return Content(p.DestinationData, "text/html; charset=utf-8", Encoding.UTF8);

            ///////// LOGIN WITH MAIL//////////////////////////////
            //Authen authen = new Authen(_context, bosDAO); dynamic d = null;
            //authen.LoginWithEmail("842488", "chiennt@tinhvan.com", "pE@ug*m@n20u", out d);
            //return Content("");
            ///////////////////////////////////////////////////////
            try
            {
                ToolWeb toolWeb = new ToolWeb("https://app.bambooairways.com/api",
                "Authen||Flight_Crew||Flight_List", "POST||GET||GET",
                "/auth/token||/aims/flight_crew_list||/aims/flight_list?flight_date=2019-",
                "application/x-www-form-urlencoded||application/json||application/json",
                "application/json||application/json||application/json");

                DateTime date = DateTime.Parse("2019-07-10");

                string s = toolWeb.WebHTTPRequest("Authen", "", "client_id=100014&client_secret=0fa2a942a4af7471612810b7a3f15890&email=hrm@bambooairways.com&password=YTun2azmloP5RK4");

                dynamic d = JObject.Parse(s);

                s = toolWeb.WebHTTPRequest("Flight_List",
                    d.token_type.ToString() + " " + d.access_token.ToString(),
                    "flight_date=" + date.ToString("yyyy-MM-dd"));

                //dynamic d1 = JObject.Parse(s);

                s = toolWeb.WebHTTPRequest("Flight_Crew",
                    d.token_type + " " + d.access_token.ToString(),
                    "flight_date=" + date.ToString("yyyy-MM-dd") + "&flight_number=202");// + d1.FlightList[0].FlightNo.ToString());

                dynamic d2 = JObject.Parse(s);
            }
            catch(Exception e)
            {

            }

            //return;
            string l = _context.CheckLogin(ref bosDAO);
            if (l != "")
            {
                return Redirect(_context.ReturnUrlLogin(l));
            }
            else
            {
                /*
                dynamic d = JObject.Parse(_context.GetSession("json -language")); dynamic d1 = null;
                string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = "";
                l = _context.GetSession("language"); if (l == "") l = _context.AppConfig.GetLanguageDefault(_context);
                foreach (JProperty property in d.Properties())
                {
                    Console.WriteLine(property.Name + " - " + property.Value);
                    d1 = JObject.Parse("{\"parameterInput\":[" +
                        "{\"ParamName\":\"Creator\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                        "{\"ParamName\":\"LanguageTextID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                        "{\"ParamName\":\"LanguageTextCode\", \"ParamType\":\"22\", \"ParamInOut\":\"1\", \"ParamLength\":\"20\", \"InputValue\":\"" + property.Name + "\"}," +
                        "{\"ParamName\":\"LanguageTextName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"200\", \"InputValue\":\"" + property.Value + "\"}," +
                        "{\"ParamName\":\"Language\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"40\", \"InputValue\":\"" + l + "\"}," +
                        "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                        "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"0\", \"ParamInOut\":\"3\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}]}");
                    bosDAO.ExecuteStore("LanguageText", "SP_CMS__LanguageText_Update", d1, ref parameterOutput, ref json, ref errorCode, ref errorString);
                }*/
                bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                StringBuilder r1 = new StringBuilder(); string r = ""; string title = _context.GetLanguageLable("HomePageTitle");                
                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn));
                r1.Append(UIDef.UIContentTagOpen (ref bosDAO, _context,MenuOn, "0", false));
                r1.Append("<table class=\"table table-hover table-border flex\">");
                r1.Append("<tr><td valign=\"top\">Token: <td><b>" + _context.GetSession("Token") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">UserID: <td><b>" + _context.GetSession("UserID") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">UserName: <td><b>" + _context.GetSession("UserName") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">ImageID: <td><b>" + _context.GetSession("ImageID") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">StaffID: <td><b>" + _context.GetSession("StaffID") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">CompanyID: <td><b>" + _context.GetSession("CompanyID") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">CompanyCode: <td><b>" + _context.GetSession("CompanyCode") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">PeriodID: <td><b>" + _context.GetSession("PeriodID") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">PeriodStartDate: <td><b>" + _context.GetSession("PeriodStartDate") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">PeriodEndDate: <td><b>" + _context.GetSession("PeriodEndDate") + " </b></td></tr>");

                r1.Append("<tr><td valign=\"top\">CountUser: <b>" + _context.GetSession("CountUser") + " </b></td></tr>");
                r1.Append("<tr><td valign=\"top\">CountLogin: <b>" + _context.GetSession("CountUserLogin") + " </b></td></tr>");
                //r1.Append("<tr><td valign=\"top\">Cache-GetAllKey:<td>" + _context._cache.GetAllKey() + "</td></tr>");
                string[] a = _cache.GetAllKey();             
                if (a != null)
                {
                    r1.Append("<tr><td valign=\"top\">Cache: <td><a href='/Home/DeleteCache'>Xóa All</a>");
                    for (int i = 0; i < a.Length; i++)
                    {
                        r1.Append("<br><a href='/Home/DeleteCacheByKey?CacheKey=" + a[i] + "'>Xóa-" + a[i] + "</a>");
                    }
                    r1.Append("</td></tr>");
                }
                else
                    r1.Append("<tr><td valign=\"top\">No-Cache:<td></td></tr>");
                r1.Append("</table>");
                //r1.Append("<iframe name='subw' id='subw' height='400' width='100%' FrameBorder='0' src='/Report/Chart?RptID=MenuChart&MenuOn=Off' onload=\"resizeIframe(this)\"></iframe>");
                r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                //r1.Append(UIDef.UIFooter());
                r = r1.ToString();
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, _context.GetRequestVal("MenuID"));
                ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn()=="Min"?" sidebar-collapse":"");
                ViewData["IndexBody"] = r;
                ViewData["PageTitle"] = title;
                ViewData["iframe"] = _context.GetRequestVal("iframe");
                ViewData["txtClose"] = _context.GetLanguageLable("Close");
                r1 = null;
                return View();
            }             
        }
        public IActionResult DeleteCache()
        {
            _cache.RemoveAll();
            return Redirect("/Home");
        }
        public IActionResult DeleteCacheByKey()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            string cKey = _context.GetRequestVal("CacheKey");
            _cache.RemoveOne(cKey);
            return Redirect("/Home");
        }
        public IActionResult Submenu()
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
                string MenuID = _context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                //if (!_context.CheckPermistion(int.Parse(MenuID)))
                //{
                //    return Redirect("/Home/Index?Message=" + _context.ReturnMsg("{YouAreNotIsGrantToFunction}", "Error"));
                //}
                bool MenuOn = (_context.GetRequestMenuOn() != "Off");
                StringBuilder r1 = new StringBuilder(); string r = "";
                string title = _context.GetLanguageLable("HomePageTitle");
                //r1.Append(UIDef.UIMenu(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIHeader(ref bosDAO, _context, MenuOn, MenuID));
                r1.Append(UIDef.UIContentTagOpen(ref bosDAO, _context, MenuOn, MenuID, false));

                r1.Append(UIDef.UIMenuSubForm(_context, bosDAO, MenuOn, MenuID));

                r1.Append(UIDef.UIContentTagClose(_context, MenuOn, false));
                //r1.Append(UIDef.UIFooter());
                r = r1.ToString();
                ViewData["ListShortcut"] = UIDef.UIMenuRelated(_context, bosDAO, MenuOn, MenuID);
                ViewData["IsPageLogin"] = !MenuOn; ViewData["Secure"] = (_context.GetRequestMenuOn() == "Min" ? " sidebar-collapse" : "");
                ViewData["IndexBody"] = r;
                ViewData["PageTitle"] = title;
                ViewData["iframe"] = _context.GetRequestVal("iframe");
                ViewData["txtClose"] = _context.GetLanguageLable("Close");
                r1 = null;
                return View();
            }
        }
        #region Login || Logout || Authen
        [HttpPost]
        public IActionResult ExecChangePwd()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            
            ToolDAO bosDAO = new ToolDAO(_context);
            string UserName = _context.GetRequestVal("Username");
            string PwdOlded = _context.GetRequestVal("OlderPassword");
            string Pwd = _context.GetRequestVal("NewerPassword");
            string PwdConfirm = _context.GetRequestVal("ConfirmPassword");

            dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = ""; string l = "";
            d = JObject.Parse("{\"parameterInput\":[" +
                "{\"ParamName\":\"UserName\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + UserName + "\"}," +
                "{\"ParamName\":\"PwdOlded\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + PwdOlded + "\"}," +
                "{\"ParamName\":\"Pwd\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + Pwd + "\"}," +
                "{\"ParamName\":\"PwdConfirm\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + PwdConfirm + "\"}," +
                "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
            bosDAO.ExecuteStore("ChangedPwd", "SP_CMS__Users_ChangedPassword", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            try
            {
                d = JObject.Parse("{" + parameterOutput + "}");
                if ((long)d.ParameterOutput.ResponseStatus > 0)
                {
                    l = _context.GetLanguageLable("YouAreChangedPassword");
                    _context.ClearSession();
                    _context.DeleteAllCookie();
                    return Redirect(_context.ReturnUrlLogin(l, false, false, "alert-success"));
                }
                else
                {
                    l = _context.GetLanguageLable(d.ParameterOutput.Message.ToString());
                    _context.ClearSession();
                    _context.DeleteAllCookie();
                    return Redirect(_context.ReturnUrlLogin(l, false, true));
                }
            }
            catch (Exception Ex)
            {
                l = Ex.ToString();
                return Redirect(_context.ReturnUrlLogin(l, false, true));
            }            
        }
        [HttpGet]
        public IActionResult ChangePwd()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            StringBuilder r = new StringBuilder();
            string Message = _context.GetQueryString("Message");
            //if (Message == "") Message = _context.GetLanguageLable("ChangePassword");
            string Username = _context.GetRequestVal("Username");
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            r.Append(UIDef.UIHeader(ref bosDAO, _context, false));//&nbsp;&nbsp;&nbsp;
            r.Append(Environment.NewLine +
    "<div class=\"pagelogin\">" +
        "<div class=\"logincenter\">" +
            "<div class=\"logintitle\">" + _context.GetLanguageLable("ChangePassword") + "</div>" +
            "<div class=\"logo\"></div>");
            r.Append(Environment.NewLine + 
            "<div class=\"login\">" +
                "<div class=\"center\">");
            r.Append(Environment.NewLine + "<form autocomplete=\"off\" name=\"loginform\" action=\"/Home/ExecChangePwd\" method=\"POST\" style=\"margin:0px\">");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\"><input class=\"username\" placeholder=\"" + _context.GetLanguageLable("Username") + "\" autocomplete=\"off\" name=\"Username\" value=\"" + Username + "\" maxlength=20 onkeypress=\"if (isEnter(event)) document.loginform.OlderPassword.focus();else checkcapslock(event);\"></div>");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\"><input type=\"password\" class=\"password\" placeholder=\"" + _context.GetLanguageLable("OlderPassword") + "\" autocomplete=\"off\" name=\"OlderPassword\" maxlength=16 onkeypress=\"if (isEnter(event)) document.loginform.NewerPassword.focus();else checkcapslock(event);\"></div>");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\"><input type=\"password\" class=\"password\" placeholder=\"" + _context.GetLanguageLable("NewerPassword") + "\" autocomplete=\"off\" name=\"NewerPassword\" maxlength=16 onkeypress=\"if (isEnter(event)) document.loginform.ConfirmPassword.focus();else checkcapslock(event);\"></div>");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\"><input type=\"password\" class=\"password\" placeholder=\"" + _context.GetLanguageLable("ConfirmPassword") + "\" autocomplete=\"off\" name=\"ConfirmPassword\" maxlength=16 onkeypress=\"if (isEnter(event)) btnLogin();else checkcapslock(event);\"></div>");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\"><button type=\"button\" class=\"btnlogin\" onclick=\"btnLogin();\" >" + _context.GetLanguageLable("ChangePassword") + "</button></div>");
           
            r.Append(Environment.NewLine + "</form></div></div></div></div>");

            //r.Append(Environment.NewLine + "<script src=\"/js/jquery.min.js\"></script>" + Environment.NewLine +
            //    "<script src=\"/js/bootstrap.min.js\"></script><script src=\"/js/admin_intecom.js\"></script>" + Environment.NewLine +
            //    "<script src=\"/js/icheck.min.js\"></script>");

            r.Append(Environment.NewLine + "<script>");
            r.Append((Message != "" ? "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + Message.Replace(Environment.NewLine, "\\n") + "', '', '0');" : ""));
            //r.Append(Environment.NewLine + "$(function () {" +
            //        Environment.NewLine + " $('input').iCheck({ checkboxClass: 'icheckbox_square-blue', radioClass: 'iradio_square-blue', increaseArea: '20%' }); });");
            r.Append(Environment.NewLine + "function btnLogin() {" +
                    Environment.NewLine + " var f = document.loginform;" +
                    Environment.NewLine + " if (f.Username.value == \"\"){" +
                    Environment.NewLine + " JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("UsernameIsNull") + "', '', '0');" +//alert(\"" + _context.GetLanguageLable("UsernameIsNull") + "!\");
                    Environment.NewLine + " f.Username.focus();" +
                    Environment.NewLine + " return false; } " +
                    Environment.NewLine + "if (f.OlderPassword.value == \"\"){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("OlderPasswordIsNull") + "', '', '0');" +//alert(\"" + _context.GetLanguageLable("OlderPasswordIsNull") + "\");
                    Environment.NewLine + "f.OlderPassword.focus();" +
                    Environment.NewLine + "return false;}" +
                    Environment.NewLine + "if (f.NewerPassword.value == \"\"){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("NewerPasswordIsNull") + "', '', '0');" +//alert(\"" + _context.GetLanguageLable("NewerPasswordIsNull") + "\");
                    Environment.NewLine + "f.NewerPassword.focus();" +
                    Environment.NewLine + "return false;}" +
                    Environment.NewLine + "if (f.ConfirmPassword.value != f.NewerPassword.value){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("ConfirmPasswordIsWrong") + "', '', '0');" +//alert(\"" + _context.GetLanguageLable("ConfirmPasswordIsWrong") + "\");
                    Environment.NewLine + "f.ConfirmPassword.focus();" +
                    Environment.NewLine + "return false;}" +
                    Environment.NewLine + " f.submit();}");
            r.Append(Environment.NewLine + "function isEnter(e) {if (!e) var e=window.event; var keyCode = e.keyCode ? e.keyCode : (e.which ? e.which : 0);if (keyCode==13) return true; else return false;}");
            r.Append(Environment.NewLine + "function checkcapslock(e) {" +
                "if (!e) var e=window.event;var keyCode = e.keyCode ? e.keyCode : (e.which ? e.which : 0);" +
                "if (keyCode==8) return;" +
                "if(!e.shiftKey && keyCode>=65 && keyCode<=90 || e.shiftKey && keyCode>=97 && keyCode<=122) JsAlert('alert-info', '" + _context.GetLanguageLable("Alert-Warning") + "', '" + _context.GetLanguageLable("CapsLockIsOn") + "', '', '0');}  ");
            r.Append(Environment.NewLine + "document.loginform.Username.focus();</script>");

            ViewData["ListShortcut"] = "";
            ViewData["IsPageLogin"] = true;
            ViewData["PageTitle"] = _context.GetLanguageLable("PageTitleChangePassword");
            ViewData["PageBody"] = r.ToString();

            r = null;
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            ToolDAO bosDAO = new ToolDAO(_context);
            string Token = _context.GetSession("Token");
            dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"Token\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"100\", \"InputValue\":\"" + Token + "\"}," +
                    "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                    "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
            bosDAO.ExecuteStore("Authen", "SP_CMS__Users_LogoutByToken", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            string l = _context.GetLanguageLable("YouAreLogout");//_context.GetAllKeyCookie(";"); //
            _context.ClearSession();
            _context.DeleteAllCookie();
            return Redirect(_context.ReturnUrlLogin(l, false));
        }

        [HttpGet]
        public IActionResult Login()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            StringBuilder r = new StringBuilder();
            string Message = _context.GetQueryString("Message");
            string UrlBack = _context.GetQueryString("UrlBack");
            string MsgError = _context.GetQueryString("MsgError");if (MsgError == "") MsgError = "alert-error";
            string Username = _context.GetQueryString("Username");
            _context.SetSession("HRSStart", "HRS");
            DateTime now = DateTime.Now;
            bool IsBlock = (_context.GetSession("IsBlock")=="1");
            string TimeBlock = _context.GetSession("BlockTime");
            if (TimeBlock == "") TimeBlock = String.Format("{0:M/d/yyyy HH:mm:ss}", now);
            int time = 500;
            if (IsBlock)
            {
                //int result = DateTime.Compare(now, DateTime.Parse(TimeBlock));
                TimeSpan spanForMinutes = now - DateTime.Parse(TimeBlock);
                if ((int)spanForMinutes.TotalSeconds > 0)
                {
                    _context.SetSession("IsBlock", ""); IsBlock = false;
                    _context.SetSession("CountErrorLogin", "0");
                    _context.SetSession("BlockTime", "");
                }
                else
                {
                    time = (int)(DateTime.Parse(TimeBlock) - now).TotalSeconds;
                }
                
            }
            ToolDAO bosDAO = new ToolDAO(_context); // Default connectstring - Schema BOS
            r.Append(UIDef.UIHeader(ref bosDAO, _context, false));//&nbsp;&nbsp;&nbsp;
            r.Append(Environment.NewLine + 
    "<div class=\"pagelogin\">" +
        "<div class=\"logincenter\">" +
            "<div class=\"logintitle\">" + _context.GetLanguageLable("HRS") + "</div>" +
            "<div class=\"logo\"></div>"); //<a href=\"http://histaff.vn\"><img src=\"/images/logo_header2.png\" height=32></a>
            r.Append(Environment.NewLine +
            "<div class=\"login\">" +
                "<div class=\"center\">" +
                    "<!--p id=\"capslocklabel\" style=\"color:red\" class=\"login-box-msg\">" + 
                (IsBlock? _context.GetLanguageLable("LoginIsBlockTo") + " " + String.Format("{0:HH:mm:ss}", DateTime.Parse(TimeBlock)) + ". <span id=b name=b></span>" : Tools.HtmlEncode(Message)) + "</p-->");
            r.Append(Environment.NewLine + "<form autocomplete=\"off\" name=\"loginform\" action=\"/Home/Authority\" method=\"POST\" style=\"margin:0px\">");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\">" +
                "<input placeholder=\"" + _context.GetLanguageLable("Username") + "\" autocomplete=\"off\" name=\"Username\" " + (IsBlock ? " disabled class=\"bg-gray username\" " : " class=\"username\" ") + " value=\"" + Tools.HtmlEncode(Username) + "\" maxlength=20 onkeypress=\"if (isEnter(event)) document.loginform.UserPassword.focus();else checkcapslock(event);\"></div>");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\">" +
                "<input type=\"password\" placeholder=\"" + _context.GetLanguageLable("Password") + "\" " + (IsBlock ? " disabled class=\"bg-gray password\" " : " class=\"password\" ") + " autocomplete=\"off\" name=\"UserPassword\" maxlength=16 onkeypress=\"if (isEnter(event)) btnLogin();else checkcapslock(event);\"></div>");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\" style=\"display:none\">" +
                "GYC9OFeeGtnbaN: <input style=\"display:block;\" placeholder=\"" + _context.GetLanguageLable("SysCode") + "\" autocomplete=\"off\" value=\"GYC9OFeeGtnbaN\" id=\"SysCode\" name=\"SysCode\" " + (IsBlock ? " disabled class=\"bg-gray password\" " : " class=\"password\" ") + " maxlength=20 ></div>");
            string LoginProvider = _context.AppConfig.ReadConfigToJson(_context).System.LoginProvider.ToString();
            r.Append(Environment.NewLine + "<div class=\"rowlogin\">" +
                "<input type=\"radio\" id=\"hrm\"    name=\"Provider\" value=\"1\" " + (LoginProvider == "1" ? "checked" : "") + "><label for=\"hrm\">" + _context.GetLanguageLable("HRMAccount") + " </label>" +
                "<input type=\"radio\" id=\"ad\"     name=\"Provider\" value=\"2\" " + (LoginProvider == "2" ? "checked" : "") + "><label for=\"ad\">" + _context.GetLanguageLable("ADAccount") + " </label>" +
                "<input type=\"radio\" id=\"email\"  name=\"Provider\" value=\"3\" " + (LoginProvider == "3" ? "checked" : "") + "><label for=\"email\">" + _context.GetLanguageLable("EmailAccount") + " </label>" +
                "</div>");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\">" +
                UIDef.UISelect("ServerID", ref bosDAO, "TVG.SP_CMS__Sys_Server_List", "", "ID,Name,ParentID", LoginProvider, false, "", "") +
                "</div>");
            r.Append(Environment.NewLine + "<div class=\"rowlogin\">" +
                "<input type=\"checkbox\" id=\"rememberme\" name=\"rememberme\" value=\"1\">" +
                "<label for=\"rememberme\">" + _context.GetLanguageLable("Remember") + "</label>" +
                "<label class=\"forget\"><a href=\"javascript:changePwd();\">" + _context.GetLanguageLable("ChangePassword") + "</a></label>" +
                "</div>");
            
            r.Append(Environment.NewLine + "<div class=\"rowlogin\">" +
               "<input type=\"button\" value=\"" + _context.GetLanguageLable("Login") + "\" onclick=\"btnLogin();\" " + (IsBlock ? " disabled class=\"bg-gray btnlogin\" " : " class=\"btnlogin\" ") + ">" +
               "</div>");            
            r.Append(Environment.NewLine + UIDef.UIHidden("UrlBack", Compress.Zip(UrlBack)));
            r.Append(Environment.NewLine + " </form>" +
                "</div>" +
            "</div></div></div>");

            r.Append(Environment.NewLine + "<script>" +
                "/*$('#ServerID').select2({" +
                    Environment.NewLine + " placeholder: \"" + _context.GetLanguageLable("SelectServer") + "\", minimumResultsForSearch: -1 });*/" +
                    Environment.NewLine + "" + (IsBlock ? "JsErrorWithTimeout(" + (time+1) * 100 + ", '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("LoginIsBlockTo") + " " + Tools.SwapDate(TimeBlock) + "', '/Home/Login');" : (Message != "" ? "JsAlert('" + MsgError + "', '" + _context.GetLanguageLable("Alert-Error") + "', '" + Message.Replace(Environment.NewLine, "\\n") + "', '', '0');" : "")) + 
                "function changePwd(){" +
                "document.location = \"/Home/ChangePwd?Username=\" + document.loginform.Username.value.replace('\"', '').replace('\\'', '').replace('>', '').replace('<', '');}");

            r.Append(Environment.NewLine + "function btnLogin() {" + (IsBlock? "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("LoginIsBlockTo") + " " + Tools.SwapDate(TimeBlock) + "', '', '0');return;":"") +//alert('" + _context.GetLanguageLable("LoginIsBlockTo") + " " + Tools.SwapDate(TimeBlock) + "');
                    Environment.NewLine + " var f = document.loginform;" +
                    Environment.NewLine + " if (f.Username.value == \"\"){" +
                    Environment.NewLine + " JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("UsernameIsNull") + "', '', '0');" +//alert(\"" + _context.GetLanguageLable("UsernameIsNull") + "!\");
                    Environment.NewLine + " f.Username.focus();" +
                    Environment.NewLine + " return false; } " +
                    Environment.NewLine + "if (f.UserPassword.value == \"\"){" +
                    Environment.NewLine + "JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', '" + _context.GetLanguageLable("PasswordIsNull") + "', '', '0');" +//alert(\"" + _context.GetLanguageLable("PasswordIsNull") + "\");
                    Environment.NewLine + "f.UserPassword.focus();" +
                    Environment.NewLine + "return false;}" +
                    Environment.NewLine + "/* if (f.rememberme.checked) {" +
                    Environment.NewLine + " setCookie(\"versionays\", f.Username.value);" +
                    Environment.NewLine + " setCookie(\"buildays\", f.UserPassword.value);" +
                    Environment.NewLine + " } else {" +
                    Environment.NewLine + " setCookie(\"versionays\", \"\");" +
                    Environment.NewLine + " setCookie(\"buildays\", \"\"); }*/" +
                    Environment.NewLine + " f.submit();}");
            r.Append(Environment.NewLine + "function isEnter(e) {if (!e) var e=window.event; var keyCode = e.keyCode ? e.keyCode : (e.which ? e.which : 0);if (keyCode==13) return true; else return false;}");
            r.Append(Environment.NewLine + "function checkcapslock(e) {" +
                "if (!e) var e=window.event;" +
                "var keyCode = e.keyCode ? e.keyCode : (e.which ? e.which : 0);" +
                "if (keyCode==8) return;" +
                "if(!e.shiftKey && keyCode>=65 && keyCode<=90 || e.shiftKey && keyCode>=97 && keyCode<=122) " +
                "JsAlert('alert-info', '" + _context.GetLanguageLable("Alert-Warning") + "', '" + _context.GetLanguageLable("CapsLockIsOn") + "', '', '0');" +
                "}  ");
            r.Append(Environment.NewLine + "document.loginform.Username.focus();");
            r.Append(Environment.NewLine + "/*function setCookie(cookie_name, cookie_value) { var exdate=new Date(); exdate.setDate(exdate.getDate()+30); document.cookie = cookie_name + \"=\" + cookie_value + \";expires=\"+exdate.toUTCString(); }*/");
            r.Append(Environment.NewLine + "function readCookie(cookie_name){" +
                    Environment.NewLine + "var the_cookie = document.cookie;" +
                    Environment.NewLine + "if(the_cookie == null || the_cookie == \"\"){" +
                    Environment.NewLine + "var cookie_value = \"\";}" +
                    Environment.NewLine + "else{" +
                    Environment.NewLine + "var broken_cookie = the_cookie.split(cookie_name+\"=\");" +
                    Environment.NewLine + "var cookie_value = broken_cookie[1];" +
                    Environment.NewLine + "if(cookie_value != null){" +
                    Environment.NewLine + "var broken_cookie = cookie_value.split(\";\");" +
                    Environment.NewLine + "var cookie_value = unescape(broken_cookie[0]);}}" +
                    Environment.NewLine + "if(cookie_value == null) cookie_value = \"\";" +
                    Environment.NewLine + "return cookie_value;}" +
                    Environment.NewLine + "var PassVal='';");
            string UserCookie = _context.GetCookie("buildays"); if (UserCookie == null) UserCookie = "";
            string PassCookie = _context.GetCookie("versionays"); if (PassCookie == null) PassCookie = "";
            if (Username == "" && UserCookie != "") r.Append(Environment.NewLine + "PassVal = '" + UserCookie + "';" +
                    Environment.NewLine + "var UserVal = '" + PassCookie + "';" +
                    Environment.NewLine + "document.loginform.Username.value = UserVal;//readCookie('versionays');");
            r.Append(Environment.NewLine + "document.loginform.UserPassword.value = PassVal;" +
                    Environment.NewLine + "//readCookie('buildays');");
            r.Append(Environment.NewLine + "if (PassVal!='') document.loginform.rememberme.checked=true;</script>");
            //r.Append(UIDef.UIFooter());

            ViewData["ListShortcut"] = "";
            ViewData["IsPageLogin"] = true;
            ViewData["PageTitle"] = _context.GetLanguageLable("PageTitleLogin");
            ViewData["PageBody"] = r.ToString();
            
            r = null;
            return View();
        }

        [HttpPost]
        public IActionResult Authority()
        {
            HRSContext _context = new HRSContext(HttpContext, _cache);
            string UrlBack = Tools.UrlDecode(_context.GetFormValue("UrlBack"));
            string ClientIP = _context.RemoteIpAddress();
            string Username = _context.GetFormValue("Username");
            string HRSStart = _context.GetSession("HRSStart");
            string Provider = _context.GetFormValue("Provider");
            string ServerID = _context.GetFormValue("ServerID");
            HTTP_CODE.WriteLogAction("Authority: \nClientIP: " + ClientIP + "\nUsername: " + Username, _context);
            if (HRSStart == "") // StopHacking
            {
                HTTP_CODE.WriteLogAction("Authority: \nStopHacking: \nClientIP: " + ClientIP + "\nUsername: " + Username, _context);
                return Redirect(_context.ReturnUrlLogin(_context.GetLanguageLable("StopHacking"), false));
            }
            else // Start Authen
            {
                int rememberme = (_context.GetFormValue("rememberme") == "1"? 1:0);
                string UserPassword = _context.GetFormValue("UserPassword"); string EmailLogin;
                string SysCode = _context.GetFormValue("SysCode");
                UserPassword = UserPassword.Replace("'", "~");
                Username = Username.Replace("'", "~");
                EmailLogin = Username;

                dynamic d = null;// JObject.Parse(_context.AppConfig.ReadConfig(_context));
                ToolDAO bosDAO = new ToolDAO(_context);
                d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
                
                d = JObject.Parse("{\"parameterInput\":[" +
                    "{\"ParamName\":\"IP\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"60\", \"InputValue\":\"" + ClientIP + "\"}," +
                    "{\"ParamName\":\"AccessID\", \"ParamType\":\"16\", \"ParamInOut\":\"3\", \"ParamLength\":\"2\", \"InputValue\":\"0\"}]}");
                bosDAO.ExecuteStore("IPAuthen", "SP_CMS__IPPrivate_CheckIP", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                d = JObject.Parse("{" + parameterOutput + "}");
                int AccessID = int.Parse(d.ParameterOutput.AccessID.ToString());
                if (AccessID == 0) // Deny Access
                {
                    HTTP_CODE.WriteLogAction("Authority: \nYourIIPIsDenyAccess: " + ClientIP + "\nUsername: " + Username, _context);
                    return Redirect(_context.ReturnUrlLogin(_context.GetLanguageLable("YourIIPIsDenyAccess"), false));
                }
                else
                {
                    if (rememberme == 1 && AccessID != 1) rememberme = 2;
                }
                if (rememberme == 1)
                {
                    try
                    {
                        UserPassword = _context.enc.Decrypt(UserPassword);
                    }
                    catch {}                    
                }
                
                try
                {
                    Authen authen = new Authen(_context, bosDAO);
                    if (Provider == "3") // Email User
                    {
                        bool kt = authen.LoginWithEmail(ServerID, Username, UserPassword, out d);
                        if (!kt) d = JObject.Parse("{\"ParameterOutput\":{\"ResponseStatus\":\"-600\", \"Message\":\"UserOrPasswordIsWrong\"}}");
                    }
                    else if (Provider == "2") // AD User
                    {
                        bool kt = authen.LoginWithADUser(ServerID, Username, UserPassword, out d);
                        if (!kt) d = JObject.Parse("{\"ParameterOutput\":{\"ResponseStatus\":\"-600\", \"Message\":\"UserOrPasswordIsWrong\"}}");
                    }
                    else // Histaff User
                    {
                        authen.LoginWithHistaffUser(Username, UserPassword, out d);
                    }
                    
                    if ((long)d.ParameterOutput.ResponseStatus > 0)
                    {
                        //if ((_context.Application==2) && d.ParameterOutput.UserType.ToString().Substring(0,1) != "2")
                        //{
                        //    return Redirect(_context.ReturnUrlLogin(Username + " " + _context.GetLanguageLable("UserPortalIsWrong"), false));
                        //}
                        if (!(_context.Application==2) && d.ParameterOutput.UserType.ToString() == "2")
                        {
                            return Redirect(_context.ReturnUrlLogin(Username + " " + _context.GetLanguageLable("UserAppIsWrong"), false));
                        }
                        _context.SetSession("Token", d.ParameterOutput.Token.ToString());
                        _context.SetSession("UserID", d.ParameterOutput.UserID.ToString());
                        _context.SetSession("UserName", Username);
                        _context.SetSession("ImageID", d.ParameterOutput.ImageID.ToString());
                        _context.SetSession("StaffID", d.ParameterOutput.StaffID.ToString());
                        _context.SetSession("CompanyID", d.ParameterOutput.CompanyID.ToString());
                        _context.SetSession("CompanyCode", d.ParameterOutput.CompanyCode.ToString());
                        _context.SetSession("IsSysAdmin", (_context.SysCode == SysCode? "1":"0"));
                        // Ghi nho
                        if (rememberme == 1)
                        {
                            _context.SetCookie("versionays", Username, 43200);
                            _context.SetCookie("buildays", _context.enc.Encrypt(UserPassword), 43200);
                        }
                        HTTP_CODE.WriteLogAction("Authority: \nMessage: " + d.ParameterOutput.Message.ToString() + "\nClientIP: " + ClientIP + "\nUsername: " + Username, _context);
                        string UserType = d.ParameterOutput.UserType.ToString();

                        /*
                        // Max login một thời điểm
                        //      1. Xóa log login có token hết hạn
                        string SQL = "DELETE FROM [BOS].[UserLogin] WHERE [TimeOutBigint] < BOS.FN_ConvertDateToNumber(GETDATE())";
                        bosDAO.ExecuteSQL("DeleteExpriedLogin", SQL, ref json, ref errorCode, ref errorString);
                        //      2. Lấy tổng số token login
                        SQL = "SELECT COUNT([UserID]) Cnt, MIN([I]) FirstID FROM [BOS].[UserLogin] WHERE [UserID] IN (SELECT I FROM [BOS].[Users] WHERE [CompanyID]='" + _context.GetSession("CompanyID") + "')";
                        bosDAO.ExecuteSQL("CountLogin", SQL, ref json, ref errorCode, ref errorString);
                        long LoginFirstID = 0; int cntLogin = 0;
                        if (errorCode == 204)
                        {
                            d = JObject.Parse(json);
                            cntLogin = int.Parse(d.CountLogin.Items[0].Cnt.ToString());
                            LoginFirstID = long.Parse(d.CountLogin.Items[0].FirstID.ToString());
                        }                        
                        while (cntLogin > _context.MaxUserLoginCurrent)
                        {
                            //      2.1. Xóa Token login cũ
                            SQL = "DELETE FROM [BOS].[UserLogin] WHERE [I] = " + LoginFirstID;
                            bosDAO.ExecuteSQL("DeleteExpriedLogin", SQL, ref json, ref errorCode, ref errorString);
                            //      2.2. Đếm lại Token login
                            SQL = "SELECT COUNT([UserID]) Cnt, MIN([I]) FirstID FROM [BOS].[UserLogin] WHERE [UserID] IN (SELECT I FROM [BOS].[Users] WHERE [CompanyID]='" + _context.GetSession("CompanyID") + "')";
                            bosDAO.ExecuteSQL("CountLogin", SQL, ref json, ref errorCode, ref errorString);
                            d = JObject.Parse(json);
                            cntLogin = int.Parse(d.CountLogin.Items[0].Cnt.ToString());
                            LoginFirstID = long.Parse(d.CountLogin.Items[0].FirstID.ToString());
                        }
                        _context.SetSession("CountUserLogin", cntLogin.ToString() + "/" + _context.MaxUserLoginCurrent);
                        // Check số lượng user cho phép UserType
                        SQL = "SELECT COUNT([I]) Cnt FROM [BOS].[Users] WHERE [CompanyID]='" + _context.GetSession("CompanyID") + "' AND [Status] = 1 AND [Type] IN (" + (UserType == "2" ? "2" : "0,1,9") + ")";
                        bosDAO.ExecuteSQL("CountUsers", SQL, ref json, ref errorCode, ref errorString);
                        d = JObject.Parse(json);
                        int cntUser = int.Parse(d.CountUsers.Items[0].Cnt.ToString());
                        _context.SetSession("CountUser", cntUser.ToString() + "/" + (UserType == "2"? _context.MaxUserPortal.ToString() + " (Portal)" : _context.MaxUserHrm.ToString() + " (Hrm)"));
                        if (cntUser > _context.MaxUserPortal && UserType == "2" || cntUser > _context.MaxUserHrm && UserType == "0")
                        {
                            // Nếu số lượng Users vượt qúa số cho phép. Khóa login lại
                            _context.ClearSession();
                            _context.DeleteAllCookie();
                            return Redirect(_context.ReturnUrlLogin(_context.GetLanguageLable("OverMaxUsers"), false));
                        }
                        */
                        d = JObject.Parse("{\"parameterInput\":[" +
                            "{\"ParamName\":\"CompanyID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + _context.GetSession("CompanyID") + "\"}," +
                            "{\"ParamName\":\"UserType\", \"ParamType\":\"12\", \"ParamInOut\":\"1\", \"ParamLength\":\"400\", \"InputValue\":\"" + (UserType == "2" ? "2" : "0,1,9") + "\"}," +
                            "{\"ParamName\":\"MaxLogin\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + _context.MaxUserLoginCurrent + "\"}," +
                            "{\"ParamName\":\"MaxUsers\", \"ParamType\":\"8\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + (UserType == "2" ? _context.MaxUserPortal : _context.MaxUserHrm) + "\"}," +
                            "{\"ParamName\":\"Message\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"600\", \"InputValue\":\"\"}," +
                            "{\"ParamName\":\"ResponseStatus\", \"ParamType\":\"8\", \"ParamInOut\":\"3\", \"ParamLength\":\"4\", \"InputValue\":\"0\"}]}");
                        bosDAO.ExecuteStore("CountUser", "SP_CMS__Users_ClearLogin", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        d = JObject.Parse("{" + parameterOutput + "}");
                        if ((long)d.ParameterOutput.ResponseStatus < 1)
                        {
                            // Nếu số lượng Users vượt qúa số cho phép. Khóa login lại
                            _context.ClearSession();
                            _context.DeleteAllCookie();
                            return Redirect(_context.ReturnUrlLogin(_context.GetLanguageLable(d.ParameterOutput.Message.ToString()), false));
                        }

                        // get default Period
                        d = JObject.Parse("{\"parameterInput\":[" +
                            "{\"ParamName\":\"PeriodID\", \"ParamType\":\"0\", \"ParamInOut\":\"3\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}," +
                            "{\"ParamName\":\"PeriodStartDate\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"40\", \"InputValue\":\"\"}," +
                            "{\"ParamName\":\"PeriodEndDate\", \"ParamType\":\"12\", \"ParamInOut\":\"3\", \"ParamLength\":\"40\", \"InputValue\":\"\"}]}");
                        bosDAO.ExecuteStore("Period", _context.GetSession("CompanyCode") + ".SP_CMS__AT_Period_DefaultPeriod", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        try
                        {
                            d = JObject.Parse("{" + parameterOutput + "}");
                            _context.SetSession("PeriodID", d.ParameterOutput.PeriodID.ToString());
                            _context.SetSession("PeriodStartDate", d.ParameterOutput.PeriodStartDate.ToString());
                            _context.SetSession("PeriodEndDate", d.ParameterOutput.PeriodEndDate.ToString());
                        } catch { }

                        // Get default PageSize
                        //d = JObject.Parse(_context.AppConfig.ReadConfig(_context));
                        _context.SetSession("PageSizeReport", _context.PageSizeReport);
                        _context.SetSession("PageSizeBaseTab", _context.PageSizeBaseTab);

                        // get permisstion
                        d = JObject.Parse("{\"parameterInput\":[{\"ParamName\":\"UserID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"4\", \"InputValue\":\"" + _context.GetSession("UserID") + "\"},{\"ParamName\": \"grp\", \"ParamType\": \"16\", \"ParamInOut\": \"1\", \"ParamLength\": \"2\", \"InputValue\": \"" + _context.Application.ToString() +"\"}]}");
                        bosDAO.ExecuteStore("Authen", "SP_CMS__Users_ListPer", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                        _context.SetSession("PermissionList", json);

                        UrlBack = UrlBack.Replace("??", "?");
                        if(UrlBack == "?" || UrlBack == "/?" || UrlBack == "/") UrlBack = "";
                        if (rememberme == 2)
                        {
                            if (UrlBack != "")
                                if (UrlBack.IndexOf("?") > 0)
                                    return Redirect(UrlBack + "&Message=" + Compress.Zip(_context.GetLanguageLable("NotAlowRemember")));
                                else
                                    return Redirect(UrlBack + "?Message=" + Compress.Zip(_context.GetLanguageLable("NotAlowRemember")));
                            else
                                return Redirect("/Home" + "?Message=" + Compress.Zip(_context.GetLanguageLable("NotAlowRemember")));
                        }
                        else
                        {
                            if (UrlBack != "")
                                return Redirect(UrlBack);
                            else
                                return Redirect(_context.PageDefault);
                        }                            
                    }
                    else
                    {
                        _context.SetCookie("versionays", "", 43200);
                        _context.SetCookie("buildays", "", 43200);
                        _context.CheckCountErrorLogin();
                        HTTP_CODE.WriteLogAction("Authority: \nMessage: " + d.ParameterOutput.Message.ToString() + "\nClientIP: " + ClientIP + "\nUsername: " + Username, _context);
                        return Redirect(_context.ReturnUrlLogin(_context.GetLanguageLable(d.ParameterOutput.Message.ToString()), false));
                    }
                }
                catch (Exception Ex)
                {
                    _context.SetCookie("versionays", "", 43200);
                    _context.SetCookie("buildays", "", 43200);
                    _context.CheckCountErrorLogin();
                    HTTP_CODE.WriteLogAction("Authority: \nMessage: " + Ex.ToString() + "\nClientIP: " + ClientIP + "\nUsername: " + Username, _context);
                    return Redirect(_context.ReturnUrlLogin(Ex.ToString(), false));
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
