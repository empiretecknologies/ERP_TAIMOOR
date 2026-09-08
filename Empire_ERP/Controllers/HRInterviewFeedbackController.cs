using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.Reporting.NETCore;
using Org.BouncyCastle.Ocsp;
using System.Text;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class HRInterviewFeedbackController : BaseController
    {
        public IHRInterviewFeedbackService _HRInterviewFeedbackService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IPeriodService _periodService { get; set; }
        public HRInterviewFeedbackController(IHRInterviewFeedbackService chartOfAccountService, IMenuService menuService, IWebHostEnvironment hostingEnvironment, IPeriodService periodService,IBaseService baseService) : base(menuService,baseService)
        {
            _HRInterviewFeedbackService = chartOfAccountService;
            _hostingEnvironment = hostingEnvironment;
            _periodService = periodService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);

            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            //ViewBag.Education = DropdownService.EducationDropdown();
            //ViewBag.Job = DropdownService.JobDropdown();
            ViewBag.Candidate = DropdownService.CandidateDropdown();
            //ViewBag.Interviews = DropdownService.InterviewsByEmpDropdown(empId);
            //ViewBag.Employee = DropdownService.EmpDropdown();
            //ViewBag.Employee = DropdownService.GetEmplyeeGroup();
			//ViewBag.EmpMails = EmpMails();
			//ViewBag.CandidateMails = CandidateMails();
			ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public JsonResult InterviewsByEmpDropdown(int id)
        {
            try
            {
                var data = DropdownService.InterviewsByEmpDropdown(id);
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        public static List<KeyValuePair<int, string>> EmpMails()
		{
			var list = new List<KeyValuePair<int, string>>();
			using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
			{
				string query = "SELECT EMP_CODE, EMAIL FROM TBL_EMP_REG WHERE DLT = 'T' AND ASTATUS = 'Y' AND MSTAFF = 'Y'";
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							list.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
						}
					}
				}
			}
			return list;
		}

		public static List<KeyValuePair<int, string>> CandidateMails()
		{
			var list = new List<KeyValuePair<int, string>>();
			using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
			{
				string query = "SELECT TRAN_ID,EMAIL FROM TBL_CANDIDATES WHERE DLT = 'T' AND ASTATUS = 'Y' ORDER BY 1 ASC";
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							list.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
						}
					}
				}
			}
			return list;
		}


		[HttpGet]
        public JsonResult GetHRJobPosts()
        {
            var data = _HRInterviewFeedbackService.GetHRJobPosts(CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetHRMails()
        {
            //var data = _HRInterviewFeedbackService.GetEmpMails(CommonHelper.GetValues(HttpContext));
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var jsonDataList = new List<object>();
            jsonDataList.Add(new
            {
                EMP_CODE = 1,
                EMP_ID = "HR-01",
                ENAME = "Muhammad Ammar",
                EMAIL = "ammarkhanzada77@gmail.com",
            });

            response.msg = "";
            response.msgType = 1;
            response.data = jsonDataList;

            return Json(response);
        }

        //[HttpGet]
        //public JsonResult GetShifts()
        //{
        //    try
        //    {
        //        var data = DropdownService.ShiftDropdown();
        //        return Json(new { data = data, msgType = 1 });
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(new { data = _catchMessage, msgType = 2 });
        //    }
        //}

        //[HttpGet]
        //public JsonResult GetAccountsForTreeView()
        //{
        //    try
        //    {
        //        var data = _HRInterviewFeedbackService.GetAccountsForTreeView(CommonHelper.GetValues(HttpContext));
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet]
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _HRInterviewFeedbackService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult HRJobPostByid(int id)
        {
            var data = _HRInterviewFeedbackService.GetHRJobPostById(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetReportTypes()
        {
            try
            {
                var data = _menuService.GetMenuDetails(CommonHelper.GetValues(HttpContext).MenuID);
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        //[HttpGet]
        //public JsonResult GetItems()
        //{
        //    try
        //    {
        //        var data = DropdownService.ItemMasterDropdown(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).RoleType);
        //        return Json(new { data = data, msgType = 1 });
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(new { data = _catchMessage, msgType = 2 });
        //    }
        //}

        //[HttpGet]
        //public JsonResult GenerateCardNo()
        //{
        //    try
        //    {
        //        var data = _HRInterviewFeedbackService.GenerateCardNo(CommonHelper.GetValues(HttpContext));
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public JsonResult Save(HRInterviewFeedback model)
        {
            try
            {
                var data = _HRInterviewFeedbackService.Save(model, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var data = _HRInterviewFeedbackService.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //public JsonResult GetPrintReport(HRJobPostReport model)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var filePath = GenerateReport(model);
        //        if (!String.IsNullOrEmpty(filePath))
        //        {
        //            response.data = filePath;
        //            response.msg = "";
        //            response.msgType = 1;
        //        }
        //        else
        //        {
        //            response.msg = "Unable to generate report. Please try again later.";
        //            response.msgType = 2;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        response.msg = _catchMessage;
        //        response.msgType = 2;
        //    }
        //    return Json(response);
        //}

        //private string GenerateReport(HRJobPostReport model)
        //{
        //    var filePath = "";
        //    try
        //    {
        //        if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
        //        {
        //            Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable();
        //            var responseMessage = _HRInterviewFeedbackService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
        //            if (responseMessage.msgType != 1)
        //            {
        //                return "";
        //            }
        //            var reportData = (HRJobPostReport)responseMessage.data;

        //            using (LocalReport report = new LocalReport())
        //            {
        //                var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.REPORT_NAME}.rdlc");
        //                using (var stReader = new StreamReader(path))
        //                {
        //                    string stringreader = stReader.ReadToEnd();
        //                    byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
        //                    using (var stream = new MemoryStream(byteArray))
        //                    {
        //                        report.EnableExternalImages = true;
        //                        report.LoadReportDefinition(stream);
        //                        report.DataSources.Clear();
        //                        var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.COMPANY_LOGO}");
        //                        bool? showCompanyLogo = true;
        //                        if (!System.IO.File.Exists(companyLogoPath))
        //                        {
        //                            showCompanyLogo = false;
        //                        }
        //                        var TotalBag = CommonController.ToAccountingFormat(reportData.TBAG);

        //                        ReportParameter parameter = new ReportParameter("CompanyName", reportData.COMPANY_NAME);
        //                        ReportParameter parameter1 = new ReportParameter("CompanyAddress", reportData.COMPANY_ADDRESS);
        //                        ReportParameter parameter2 = new ReportParameter("CompanyPhone", reportData.COMPANY_PHONE);
        //                        ReportParameter parameter3 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.COMPANY_LOGO}")).AbsoluteUri);
        //                        ReportParameter parameter4 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
        //                        ReportParameter parameter5 = new ReportParameter("Header", reportData.HEADER_NAME);

        //                        ReportParameter parameter11 = new ReportParameter("V_DATE", reportData.V_DATE.ToString());
        //                        ReportParameter parameter12 = new ReportParameter("VOUCHER_NO", reportData.VOUCHER_NO);
        //                        ReportParameter parameter13 = new ReportParameter("PARTY_CODE", reportData.PARTY_CODE);
        //                        ReportParameter parameter14 = new ReportParameter("S_DATE", reportData.S_DATE.ToString());
        //                        ReportParameter parameter15 = new ReportParameter("S_NO", reportData.S_NO);
        //                        ReportParameter parameter16 = new ReportParameter("BROKER_CODE", reportData.BROKER_CODE);
        //                        ReportParameter parameter17 = new ReportParameter("GODOWN", reportData.GODOWN);
        //                        ReportParameter parameter18 = new ReportParameter("KANTA", reportData.KANTA);
        //                        ReportParameter parameter19 = new ReportParameter("COND", reportData.COND);
        //                        ReportParameter parameter20 = new ReportParameter("C_NAME", reportData.C_NAME);
        //                        ReportParameter parameter21 = new ReportParameter("CELL", reportData.CELL);
        //                        ReportParameter parameter22 = new ReportParameter("LOT_NO", reportData.LOT_NO);
        //                        ReportParameter parameter23 = new ReportParameter("ORIGIN", reportData.ORIGIN);
        //                        ReportParameter parameter24 = new ReportParameter("ITEM_CODE", reportData.ITEM_CODE);
        //                        ReportParameter parameter25 = new ReportParameter("TBAG", TotalBag);
        //                        ReportParameter parameter26 = new ReportParameter("UNIT", reportData.UNIT);

        //                        report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2, parameter3, parameter4, parameter5, parameter11, parameter12, parameter13, parameter14, parameter15,
        //                    parameter16, parameter17, parameter18, parameter19, parameter20, parameter21, parameter22, parameter23, parameter24, parameter25, parameter26 });
        //                        report.Refresh();
        //                        //report.DataSources.Add(new ReportDataSource() { Name = "InvoiceReportDataSet", Value = reportData.Detail });

        //                        byte[] file;
        //                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\HRJobPost");
        //                        if (!Directory.Exists(uploadsFolder))
        //                        {
        //                            Directory.CreateDirectory(uploadsFolder);
        //                        }

        //                        if (report.IsReadyForRendering)
        //                        {
        //                            string input = reportData.VOUCHER_NO;
        //                            string[] parts = input.Split('/');
        //                            string prefix = string.Empty;
        //                            string voucherNumber = string.Empty;
        //                            if (parts.Length >= 3)
        //                            {
        //                                prefix = parts[1];
        //                                voucherNumber = parts[^1];
        //                            }
        //                            file = report.Render("PDF");
        //                            filePath = $"{prefix} - {(reportData.PARTY_CODE).Replace(" / "," - ")} - {voucherNumber}" + ".pdf";

        //                            stReader.Close();
        //                            stReader.Dispose();
        //                            stream.Flush();
        //                            stream.Close();
        //                            stream.Dispose();
        //                            report.Dispose();
        //                            string reportPath = Path.Combine(uploadsFolder, filePath);
        //                            System.IO.File.WriteAllBytes(reportPath, file);
        //                            filePath = $"/Client/HRJobPost/{filePath}";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return filePath;
        //}

        //[HttpGet]
        //public JsonResult GetUnits()
        //{
        //    try
        //    {
        //        var data = DropdownService.UnitDropdown();
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}
    }
}