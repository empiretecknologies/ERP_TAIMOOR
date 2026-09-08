using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Text;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class CashBookVoucherController : BaseController
    {
        public ICashBookVoucherService _cashBookVoucherService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IPeriodService _periodService { get; set; }
        public CashBookVoucherController(IMenuService menuService, ICashBookVoucherService cashReceiptVoucherService, IWebHostEnvironment hostingEnvironment, IPeriodService periodService,IBaseService baseService) : base(menuService,baseService)
        {
            _cashBookVoucherService = cashReceiptVoucherService;
            _hostingEnvironment = hostingEnvironment;
            _periodService = periodService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Accounts = DropdownService.GetAccountsForCashReceiptVoucher(common.RoleID, common.RoleType);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            var response = _menuService.GetMenu(common.MenuID);
            if (response.msgType == 1)
            {
                ViewBag.DC_TYPE = ((Menu)response.data).DCTYPE;
                ViewBag.DATA_CLEAR = ((Menu)response.data).DATA_CLEAR;
            }
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public JsonResult GetCurrencies()
        {
            try
            {
                var data = DropdownService.CurrencyDropdownWithControlName();
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
        public JsonResult GetBookTypes()
        {
            try
            {
                var data = _cashBookVoucherService.GetChartOfAccounts(CommonHelper.GetValues(HttpContext));
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

        [HttpGet]
        public JsonResult GetCashBookVouchers()
        {
            try
            {
                var data = _cashBookVoucherService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        public JsonResult Save(List<CashBookVoucher> modelRecord)
        {
            try
            {
                var data = _cashBookVoucherService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetCashBookVoucherByCode(int code)
        {
            try
            {
                var data = _cashBookVoucherService.GetCashBookVoucherByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _cashBookVoucherService.GetCashBookVoucherDetailsByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetCashBookVoucherDetailsByCode(int code)
        {
            try
            {
                var data = _cashBookVoucherService.GetCashBookVoucherDetailsByCode(code, CommonHelper.GetValues(HttpContext));
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
                var data = _cashBookVoucherService.Delete(code, CommonHelper.GetValues(HttpContext));
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
                var data = _cashBookVoucherService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeleteCashBookVoucherDetailByCode(int tranID, int code)
        {
            try
            {
                var data = _cashBookVoucherService.DeleteCashBookVoucherDetailByCode(tranID, code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPrintReport(CashBookRDLCReport model)
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

        private string GenerateReport(CashBookRDLCReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
                {
                    Reports.Datasets.BarcodeReportDataset.CashPaymentDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.CashPaymentDataTable();
                    var responseMessage = _cashBookVoucherService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (CustomCashBookForPrintReport)responseMessage.data;

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

                        ReportParameter parameter1 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
                        ReportParameter parameter2 = new ReportParameter("InvoiceNumber", reportData.Master?.INVOICE_NUMBER);
                        ReportParameter parameter3 = new ReportParameter("Date", reportData.Master?.DATE);
                        ReportParameter parameter4 = new ReportParameter("Comment", reportData.Master?.COMMENT);
                        ReportParameter parameter5 = new ReportParameter("Status", reportData.Master?.STATUS);
                        ReportParameter parameter6 = new ReportParameter("Sig1", reportData.Master?.SIG1);
                        ReportParameter parameter7 = new ReportParameter("Sig2", reportData.Master?.SIG2);
                        ReportParameter parameter8 = new ReportParameter("Sig3", reportData.Master?.SIG3);
                        ReportParameter parameter9 = new ReportParameter("Sig4", reportData.Master?.SIG4);
                        ReportParameter parameter10 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                        ReportParameter parameter11 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
                        ReportParameter parameter12 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
                        ReportParameter parameter13 = new ReportParameter("User", reportData.Master?.USER);
                        ReportParameter parameter14 = new ReportParameter("Party", reportData.Master?.PARTY_NAME);

                        report.SetParameters(new ReportParameter[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10, parameter11, parameter12, parameter13, parameter14 });
                        report.Refresh();
                        report.DataSources.Add(new ReportDataSource() { Name = "CashPaymentReport", Value = reportData.Detail });

                        byte[] file;
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\CashPaymentReport");
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
                            filePath = $"{prefix} - {(reportData.Master?.PARTY_NAME).Replace("/","-")} - {voucherNumber}" + ".pdf";

                            stReader.Close();
                            stReader.Dispose();
                            stream.Flush();
                            stream.Close();
                            stream.Dispose();
                            report.Dispose();
                            string reportPath = Path.Combine(uploadsFolder, filePath);
                            System.IO.File.WriteAllBytes(reportPath, file);
                            filePath = $"/Client/CashPaymentReport/{filePath}";
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
