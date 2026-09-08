using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;
using System.Text;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class BatchIssueController : BaseController
    {
        public IBatchIssueService _batchIssueService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
                public IPeriodService _periodService { get; set; }

        public BatchIssueController(IPeriodService periodService, IBatchIssueService BatchIssueService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _batchIssueService = BatchIssueService;
            _hostingEnvironment = hostingEnvironment;
            _periodService = periodService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);

            var response = _menuService.GetMenu(common.MenuID);
            if (response.msgType == 1)
            {
                ViewBag.stkStatus = ((Menu)response.data).STK_STATUS;
            }

            ViewBag.Items = DropdownService.RawItemsDropdown();
            //ViewBag.CurrentStock = DropdownService.GetCurrentStock();
            ViewBag.Units = DropdownService.UnitDropdown();
            ViewBag.FinishItems = DropdownService.FinishItemsDropdown();
            ViewBag.Processes = DropdownService.ProcessesDropdown();
            ViewBag.Unit = DropdownService.UnitDropdown();
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);

            int compCond = 2;
            string compCondQuery = "SELECT CON_QTY FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(compCondQuery, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                compCond = Convert.ToInt32(result);
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetBatchName(int itemCode)
        {
            try
            {
                var data = DropdownService.GetBatchName(itemCode);
                return Json(new { data = data, msgType = 1 });
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

        [HttpGet]
        public JsonResult GetCurrentStock()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);

                var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
                var sdate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
                var eDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
                var data = DropdownService.GetCurrentStock(sdate, eDate, common.Branch, common.Period);
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetBatchIssues()
        {
            try
            {
                var data = _batchIssueService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
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

        [HttpPost]
        public JsonResult Save(CustomBatchIssue modelRecord)
        {
            try
            {
                var data = _batchIssueService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }

        [HttpPost]
        public JsonResult BatchUpdate(BatchIssue model)
        {
            try
            {
                var data = _batchIssueService.BatchUpdate(model, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetBatchIssueByCode(int code)
        {
            try
            {
                var data = _batchIssueService.GetBatchIssueByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _batchIssueService.GetBatchIssueDetailByCode(code, CommonHelper.GetValues(HttpContext));
                return Json(new { Master = data, Detail = detailData });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetBatchIssueDetailByCode(int code)
        {
            try
            {
                var data = _batchIssueService.GetBatchIssueDetailByCode(code, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }

        [HttpPost]
        public JsonResult Delete(int code)
        {
            try
            {
                var data = _batchIssueService.Delete(code, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }

        [HttpPost]
        public JsonResult CopyRecord(CopyRecord record)
        {
            try
            {
                var data = _batchIssueService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }

        [HttpPost]
        public JsonResult DeleteBatchIssueDetailByCode(int code)
        {
            try
            {
                var data = _batchIssueService.DeleteBatchIssueDetailByCode(code, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }
        //[HttpPost]
        //public JsonResult GetPrintReport(BatchIssueRDLCReport model)
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
        //private string GenerateReport(BatchIssueRDLCReport model)
        //{
        //    var filePath = "";
        //    try
        //    {
        //        if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
        //        {
        //            Reports.Datasets.BarcodeReportDataset.BillOfMaterialDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.BillOfMaterialDataTable();
        //            var responseMessage = _batchIssueService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
        //            if (responseMessage.msgType != 1)
        //            {
        //                return "";
        //            }
        //            var reportData = (CustomBatchIssueForPrintReport)responseMessage.data;

        //            using (LocalReport report = new LocalReport())
        //            {
        //                var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.Master?.REPORT_NAME}.rdlc");
        //                using (var stReader = new StreamReader(path))
        //                {
        //                    string stringreader = stReader.ReadToEnd();
        //                    byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
        //                    using (var stream = new MemoryStream(byteArray))
        //                    {
        //                        report.EnableExternalImages = true;
        //                        report.LoadReportDefinition(stream);
        //                        report.DataSources.Clear();

        //                        var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}");
        //                        bool? showCompanyLogo = true;
        //                        if (!System.IO.File.Exists(companyLogoPath))
        //                        {
        //                            showCompanyLogo = false;
        //                        }

        //                        ReportParameter parameter = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
        //                        ReportParameter parameter1 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
        //                        ReportParameter parameter2 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
        //                        ReportParameter parameter3 = new ReportParameter("BranchAddress", reportData.Master?.BRANCH_ADDRESS);
        //                        ReportParameter parameter4 = new ReportParameter("BranchPhone", reportData.Master?.BRANCH_PHONE);
        //                        ReportParameter parameter5 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
        //                        ReportParameter parameter6 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
        //                        ReportParameter parameter7 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
        //                        ReportParameter parameter8 = new ReportParameter("InvoiceNumber", reportData.Master?.INVOICE_NUMBER);
        //                        ReportParameter parameter9 = new ReportParameter("Date", reportData.Master?.DATE);
        //                        ReportParameter parameter10 = new ReportParameter("ItemName", reportData.Master?.MITEM_NAME);
        //                        ReportParameter parameter11 = new ReportParameter("Reference", reportData.Master?.REF);
        //                        ReportParameter parameter12 = new ReportParameter("Sign1", reportData.Master?.SIG1);
        //                        ReportParameter parameter13 = new ReportParameter("Sign2", reportData.Master?.SIG2);
        //                        ReportParameter parameter14 = new ReportParameter("Sign3", reportData.Master?.SIG3);
        //                        ReportParameter parameter15 = new ReportParameter("Sign4", reportData.Master?.SIG4);
        //                        ReportParameter parameter16 = new ReportParameter("CompanyWater", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_WATER}")).AbsoluteUri);
        //                        ReportParameter parameter17 = new ReportParameter("Remarks", reportData.Master?.REMARKS);
        //                        ReportParameter parameter18 = new ReportParameter("TERM", reportData.Master?.TERM);
        //                        ReportParameter parameter19 = new ReportParameter("EDITUSERID", reportData.Master?.EDIT_USER_ID);
        //                        ReportParameter parameter20 = new ReportParameter("MENUTERMS", reportData.Master?.MENU_TERMS);
        //                        ReportParameter parameter21 = new ReportParameter("Cost", reportData.Master?.COST);
        //                        //ReportParameter parameter22 = new ReportParameter("Loss", reportData.Master?.LOSS);
        //                        ReportParameter parameter23 = new ReportParameter("Process", reportData.Master?.PROCESS);
        //                        ReportParameter parameter24 = new ReportParameter("BQty", reportData.Master?.BQTY);

        //                        report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, parameter19, parameter20, parameter21,
        //                            //parameter22, 
        //                            parameter23, parameter24 });
        //                        report.Refresh();

        //                        report.DataSources.Add(new ReportDataSource() { Name = "BatchIssueDataSet", Value = reportData.Detail });

        //                        byte[] file;
        //                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\BatchIssue");
        //                        if (!Directory.Exists(uploadsFolder))
        //                        {
        //                            Directory.CreateDirectory(uploadsFolder);
        //                        }

        //                        if (report.IsReadyForRendering)
        //                        {
        //                            string input = reportData.Master?.INVOICE_NUMBER;
        //                            string[] parts = input.Split('/');
        //                            string prefix = string.Empty;
        //                            string voucherNumber = string.Empty;
        //                            if (parts.Length >= 3)
        //                            {
        //                                prefix = parts[1];
        //                                voucherNumber = parts[^1];
        //                            }
        //                            file = report.Render("PDF");
        //                            filePath = $"{prefix} - {(reportData.Master?.MITEM_NAME).Replace(" / ", " - ")} - {voucherNumber}" + ".pdf";

        //                            stReader.Close();
        //                            stReader.Dispose();
        //                            stream.Flush();
        //                            stream.Close();
        //                            stream.Dispose();
        //                            report.Dispose();
        //                            string reportPath = Path.Combine(uploadsFolder, filePath);
        //                            System.IO.File.WriteAllBytes(reportPath, file);
        //                            filePath = $"/Client/BatchIssue/{filePath}";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exception
        //    }
        //    return filePath;
        //}

        [HttpPost]
        public JsonResult GetPrintReport(BatchIssueRDLCReport model)
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

        private string GenerateReport(BatchIssueRDLCReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0 && model.REPORT_NAME != null)
                {
                    Reports.Datasets.BarcodeReportDataset.BillOfMaterialDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.BillOfMaterialDataTable();
                    var responseMessage = _batchIssueService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (CustomBatchIssueForPrintReport)responseMessage.data;

                    using (LocalReport report = new LocalReport())
                    {
                        var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.Master?.REPORT_NAME}.rdlc");
                        var stReader = new StreamReader(path);
                        string stringreader = stReader.ReadToEnd();
                        byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
                        MemoryStream stream = new MemoryStream(byteArray);
                        report.EnableExternalImages = true;
                        report.LoadReportDefinition(stream);
                        report.DataSources.Clear();

                        if (reportData.Master?.REPORT_NAME == "BatchIssue")
                        {
                            var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}");
                            bool? showCompanyLogo = true;

                            if (!System.IO.File.Exists(companyLogoPath))
                            {
                                showCompanyLogo = false;
                            }

                            ReportParameter parameter1 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
                            ReportParameter parameter2 = new ReportParameter("InvoiceNumber", reportData.Master?.INVOICE_NUMBER);
                            ReportParameter parameter3 = new ReportParameter("Date", reportData.Master?.DATE);
                            ReportParameter parameter4 = new ReportParameter("MItem", reportData.Master?.M_ITEM);
                            ReportParameter parameter5 = new ReportParameter("Ref", reportData.Master?.REF);
                            ReportParameter parameter6 = new ReportParameter("Cost", Convert.ToString(reportData.Master?.COST));
                            ReportParameter parameter7 = new ReportParameter("Process", reportData.Master?.PROCESS);
                            ReportParameter parameter8 = new ReportParameter("BQty", Convert.ToString(reportData.Master?.BQTY));
                            ReportParameter parameter9 = new ReportParameter("Remarks", reportData.Master?.REMARKS);
                            ReportParameter parameter10 = new ReportParameter("BatchNo", reportData.Master?.BATCH_NO);
                            ReportParameter parameter11 = new ReportParameter("MfgDate", reportData.Master?.MFG_DATE);
                            ReportParameter parameter12 = new ReportParameter("ExpDate", reportData.Master?.EXP_DATE);
                            ReportParameter parameter13 = new ReportParameter("MUnit", reportData.Master?.M_UNIT);
                            ReportParameter parameter14 = new ReportParameter("User", reportData.Master?.USER_NAME);
                            ReportParameter parameter15 = new ReportParameter("MenuTerms", reportData.Master?.MENU_TERMS);
                            ReportParameter parameter16 = new ReportParameter("Sig1", reportData.Master?.SIG1);
                            ReportParameter parameter17 = new ReportParameter("Sig2", reportData.Master?.SIG2);
                            ReportParameter parameter18 = new ReportParameter("Sig3", reportData.Master?.SIG3);
                            ReportParameter parameter19 = new ReportParameter("Sig4", reportData.Master?.SIG4);
                            ReportParameter parameter20 = new ReportParameter("BName", reportData.Master?.B_NAME);
                            ReportParameter parameter21 = new ReportParameter("BAddress", reportData.Master?.B_ADDRESS);
                            ReportParameter parameter22 = new ReportParameter("BTell", reportData.Master?.B_TEL);
                            ReportParameter parameter23 = new ReportParameter("STRN", reportData.Master?.STRN);
                            ReportParameter parameter24 = new ReportParameter("BNTN", reportData.Master?.B_NTN);
                            ReportParameter parameter25 = new ReportParameter("BWeb", reportData.Master?.B_WEBSITE);
                            ReportParameter parameter26 = new ReportParameter("BEmail", reportData.Master?.EMAIL);
                            ReportParameter parameter27 = new ReportParameter("BranchTerms", reportData.Master?.B_TERMS);
                            ReportParameter parameter28 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                            ReportParameter parameter29 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                            ReportParameter parameter30 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                            report.SetParameters(new ReportParameter[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9,
                            parameter10, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, parameter19, parameter20, parameter21,
                            parameter22, parameter23, parameter24, parameter25, parameter26, parameter27, parameter28, parameter29, parameter30 });
                        }

                            report.Refresh();
                            report.DataSources.Add(new ReportDataSource() { Name = "BatchIssueDataSet", Value = reportData.Detail });

                            byte[] file;
                            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\BatchIssue");
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            if (report.IsReadyForRendering)
                            {
                                string input = reportData.Master?.INVOICE_NUMBER;
                                string[] parts = input.Split('/');
                                string prefix = string.Empty;
                                string voucherNumber = string.Empty;
                                if (parts.Length >= 3)
                                {
                                    prefix = parts[1];
                                    voucherNumber = parts[^1];
                                }
                                file = report.Render("PDF");
                                filePath = $"{prefix} - {voucherNumber}" + ".pdf";

                                stReader.Close();
                                stReader.Dispose();
                                stream.Flush();
                                stream.Close();
                                stream.Dispose();
                                report.Dispose();
                                string reportPath = Path.Combine(uploadsFolder, filePath);
                                System.IO.File.WriteAllBytes(reportPath, file);
                                filePath = $"/Client/BatchIssue/{filePath}";
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