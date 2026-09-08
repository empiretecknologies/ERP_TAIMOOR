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
using System.Data;
using System.Text;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class HROfferLetterController : BaseController
    {
        public IHROfferLetterService _HRInterviewFeedbackService { get; set; }
        public ILoginService _loginService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IPeriodService _periodService { get; set; }
        public HROfferLetterController(IHROfferLetterService chartOfAccountService, IMenuService menuService, IWebHostEnvironment hostingEnvironment, IPeriodService periodService, ILoginService loginService,IBaseService baseService) : base(menuService,baseService)
        {
            _HRInterviewFeedbackService = chartOfAccountService;
            _hostingEnvironment = hostingEnvironment;
            _periodService = periodService;
            _loginService = loginService;
        }

        public IActionResult Index()
        {
            var data = _loginService.GetBackGroundAndLogo();
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);

            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            ViewBag.Candidate = DropdownService.CandidateDropdown();
            ViewBag.InterviewsDropdown = DropdownService.InterviewsDropdown();
            ViewBag.Department = DropdownService.DepartmentDropdown();
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
            ViewBag.company = data.C_NAME;
            //ViewBag.company2 = FetchCompanies();
            return View();
        }

        public DataSet FetchCompanies()
        {
            string query = "select * from [dbo].[TBL_COMPANY] WHERE DLT = 'T'";
            DataSet data = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            return data;
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

        [HttpPost]
        public JsonResult GetCandidateEmailById(int canId)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var jsonDataList = new List<object>();

            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT CAN.EMAIL, CAN.FULL_NAME 
                          FROM TBL_INTERVIEW_SCH A
                          LEFT OUTER JOIN TBL_CANDIDATES CAN ON CAN.TRAN_ID = A.CON_ID
                          WHERE A.TRAN_ID = {canId}";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                jsonDataList.Add(new
                                {
                                    EMAIL = reader["EMAIL"] != DBNull.Value ? reader["EMAIL"].ToString() : string.Empty,
                                    FULL_NAME = reader["FULL_NAME"] != DBNull.Value ? reader["FULL_NAME"].ToString() : string.Empty
                                });
                            }
                        }
                    }
                }
            }

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
        public JsonResult Save(HROfferLetter model)
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

        [HttpPost]
        public JsonResult GetPrintReport(HROfferLetterReport model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var filePath = GenerateReport(model);
                if (!String.IsNullOrEmpty(filePath))
                {
                    response.data = filePath;
                    response.msg = "";
                    response.msgType = 1;
                }
                else
                {
                    response.msg = "Unable to generate report. Please try again later.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return Json(response);
        }

        private string GenerateReport(HROfferLetterReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
                {
                    Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable();
                    var responseMessage = _HRInterviewFeedbackService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (HROfferLetterReport)responseMessage.data;

                    using (LocalReport report = new LocalReport())
                    {
                        var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.REPORT_NAME}.rdlc");
                        using (var stReader = new StreamReader(path))
                        {
                            string stringreader = stReader.ReadToEnd();
                            byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
                            using (var stream = new MemoryStream(byteArray))
                            {
                                report.EnableExternalImages = true;
                                report.LoadReportDefinition(stream);
                                report.DataSources.Clear();
                                var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.COMPANY_LOGO}");
                                bool? showCompanyLogo = true;
                                if (!System.IO.File.Exists(companyLogoPath))
                                {
                                    showCompanyLogo = false;
                                }
                                //var TotalBag = CommonController.ToAccountingFormat(reportData.TBAG);

                                ReportParameter parameter = new ReportParameter("CompanyName", reportData.COMPANY_NAME);
                                ReportParameter parameter1 = new ReportParameter("CompanyAddress", reportData.COMPANY_ADDRESS);
                                ReportParameter parameter2 = new ReportParameter("CompanyPhone", reportData.COMPANY_PHONE);
                                //ReportParameter parameter3 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.COMPANY_LOGO}")).AbsoluteUri);
                                //ReportParameter parameter4 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                                ReportParameter parameter4 = new ReportParameter("TOTAL_PACKAGE", reportData.TOTAL_PACKAGE);
                                ReportParameter parameter5 = new ReportParameter("Header", reportData.HEADER_NAME);
                                ReportParameter parameter6 = new ReportParameter("Sig1", reportData.SIG1);
                                ReportParameter parameter7 = new ReportParameter("Sig2", reportData.SIG2);
                                ReportParameter parameter8 = new ReportParameter("Sig3", reportData.SIG3);
                                ReportParameter parameter9 = new ReportParameter("Sig4", reportData.SIG4);

                                ReportParameter parameter11 = new ReportParameter("CAN_NAME", reportData.CAN_NAME.ToString());
                                ReportParameter parameter12 = new ReportParameter("JOB_TITLE", reportData.JOB_TITLE);
                                ReportParameter parameter13 = new ReportParameter("DEP_NAME", reportData.DEP_NAME);
                                ReportParameter parameter14 = new ReportParameter("BASIC_SALARY", reportData.BASIC_SALARY.ToString());
                                ReportParameter parameter15 = new ReportParameter("ALLOWANCES", reportData.ALLOWANCES);
                                ReportParameter parameter16 = new ReportParameter("JOINING_DATE", reportData.JOINING_DATE);
                                ReportParameter parameter17 = new ReportParameter("OFFER_DATE", reportData.OFFER_DATE);
                                ReportParameter parameter18 = new ReportParameter("DUTY_START", reportData.DUTY_START);
                                ReportParameter parameter19 = new ReportParameter("DUTY_END", reportData.DUTY_END);
                                ReportParameter parameter20 = new ReportParameter("WEEKLY_OFF", reportData.WEEKLY_OFF);
                                ReportParameter parameter21 = new ReportParameter("WEEKLY_DESC", reportData.WEEKLY_DESC);

                                report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2,
                                    //parameter3,
                                    parameter4, 
                                    parameter5,
                                    parameter6,
                                    parameter7,
                                    parameter8,
                                    parameter9,
                                    parameter11, 
                                    parameter12, 
                                    parameter13, 
                                    parameter14, 
                                    parameter15,
                                    parameter16, 
                                    parameter17, 
                                    parameter18, 
                                    parameter19, 
                                    parameter20, 
                                    parameter21
                                });
                                report.Refresh();
                                //report.DataSources.Add(new ReportDataSource() { Name = "OfferLetter", Value = reportData.Detail });

                                byte[] file;
                                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\HROfferLetter");
                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }

                                if (report.IsReadyForRendering)
                                {
                                    string input = reportData.CAN_NAME;
                                    string[] parts = input.Split('/');
                                    string prefix = string.Empty;
                                    string voucherNumber = string.Empty;
                                    if (parts.Length >= 3)
                                    {
                                        prefix = parts[1];
                                        voucherNumber = parts[^1];
                                    }
                                    file = report.Render("PDF");
                                    filePath = $"{prefix} - {(reportData.CAN_NAME).Replace(" / ", " - ")} - {voucherNumber}" + ".pdf";

                                    stReader.Close();
                                    stReader.Dispose();
                                    stream.Flush();
                                    stream.Close();
                                    stream.Dispose();
                                    report.Dispose();
                                    string reportPath = Path.Combine(uploadsFolder, filePath);
                                    System.IO.File.WriteAllBytes(reportPath, file);
                                    filePath = $"/Client/HROfferLetter/{filePath}";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return filePath;
        }

    }
}