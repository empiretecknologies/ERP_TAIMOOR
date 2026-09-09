using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Text;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class SalesQutationController : BaseController
    {
        public ISalesQutationService _salesQutationService { get; set; }
        public IPOSTransactionService _posTransactionService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public SalesQutationController(ISalesQutationService salesQutationService, IPOSTransactionService posTransactionService, IWebHostEnvironment hostingEnvironment, IMenuService menuService, IBaseService baseService) : base(menuService, baseService)
        {
            _salesQutationService = salesQutationService;
            _posTransactionService = posTransactionService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            ViewBag.Items = DropdownService.ItemMasterDropdown(common.RoleID, common.RoleType);
            ViewBag.PartyType = common.RoleType == "A"
                ? DropdownService.PartyTypeDropdownForInvoice(0, common.Branch, 0)
                : DropdownService.PartyTypeDropdownForInvoice(common.RoleID, common.Branch, common.ShowSelected);
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            ViewBag.ItemsGroup = _posTransactionService.GetItemsGroup(common).data;
            var response = _menuService.GetMenu(common.MenuID);
            if (response.msgType == 1)
            {
                ViewBag.DATA_CLEAR = ((Menu)response.data).DATA_CLEAR;
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetItemsMasterByGroup(int groupId)
        {
            try
            {
                var data = _posTransactionService.GetItemsMasterByGroup(groupId, CommonHelper.GetValues(HttpContext)).data;
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

        [HttpGet]
        public JsonResult GetItems()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = DropdownService.ItemMasterDropdown(common.RoleID, common.RoleType);
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

        [HttpGet]
        public JsonResult GetParties()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = common.RoleType == "A"
                    ? DropdownService.PartyTypeDropdownForInvoice(0, common.Branch, 0)
                    : DropdownService.PartyTypeDropdownForInvoice(common.RoleID, common.Branch, common.ShowSelected);
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
        public JsonResult GetSalesQutations()
        {
            try
            {
                var data = _salesQutationService.QuickSearch(CommonHelper.GetValues(HttpContext));
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

        [HttpGet]
        public JsonResult GetSalesQutationByCode(int code)
        {
            try
            {
                var data = _salesQutationService.GetSalesQutationByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _salesQutationService.GetSalesQutationDetailByCode(code, CommonHelper.GetValues(HttpContext));
                return Json(new { Master = data, Detail = detailData });
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

        [HttpGet]
        public JsonResult GetSalesQutationDetailByCode(int code)
        {
            try
            {
                var data = _salesQutationService.GetSalesQutationDetailByCode(code, CommonHelper.GetValues(HttpContext));
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

        [HttpPost]
        public JsonResult Save(CustomSalesQutation modelRecord)
        {
            try
            {
                var data = _salesQutationService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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

        [HttpPost]
        public JsonResult Delete(int code)
        {
            try
            {
                var data = _salesQutationService.Delete(code, CommonHelper.GetValues(HttpContext));
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

        [HttpPost]
        public JsonResult DeleteSalesQutationDetailByCode(int code)
        {
            try
            {
                var data = _salesQutationService.DeleteSalesQutationDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPrintReport(RDLCReport model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var filePath = GenerateReport(model);
                if (!string.IsNullOrEmpty(filePath))
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

        private string GenerateReport(RDLCReport model)
        {
            var filePath = "";
            if (model != null && model.TRAN_ID > 0)
            {
                    DataTable reportDetails = CreateSalesQutationDetailTable();
                    var responseMessage = _salesQutationService.GetDataForPrintReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        throw new Exception(string.IsNullOrWhiteSpace(responseMessage.msg)
                            ? "Unable to generate report. Please try again later."
                            : responseMessage.msg);
                    }
                    var reportData = (CustomSalesQutationForPrintReport)responseMessage.data;
                    using (LocalReport report = new LocalReport())
                    {
                        var path = Path.Combine(_hostingEnvironment.ContentRootPath, @"Reports\SalesQutationPrintReport.rdlc");
                        using (var stReader = new StreamReader(path))
                        {
                            string stringreader = stReader.ReadToEnd();
                            byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
                            using (var stream = new MemoryStream(byteArray))
                            {
                                report.EnableExternalImages = true;
                                report.LoadReportDefinition(stream);
                                report.DataSources.Clear();
                                var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}");
                                bool showCompanyLogo = System.IO.File.Exists(companyLogoPath);
                                string companyLogoUri = "";
                                try
                                {
                                    if (showCompanyLogo)
                                    {
                                        companyLogoUri = new Uri(companyLogoPath).AbsoluteUri;
                                    }
                                }
                                catch
                                {
                                    showCompanyLogo = false;
                                    companyLogoUri = "";
                                }
                                ReportParameter parameter1 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME ?? "");
                                ReportParameter parameter2 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS ?? "");
                                ReportParameter parameter3 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE ?? "");
                                ReportParameter parameter4 = new ReportParameter("Header", reportData.Master?.HEADER_NAME ?? "");
                                ReportParameter parameter5 = new ReportParameter("TransactionDate", reportData.Master?.V_DATE?.ToString("dd/MM/yyyy") ?? "");
                                ReportParameter parameter6 = new ReportParameter("VoucherNo", reportData.Master?.VOUCHER_NO ?? "");
                                ReportParameter parameter7 = new ReportParameter("PartyName", reportData.Master?.PARTY_NAME ?? "");
                                ReportParameter parameter8 = new ReportParameter("Remarks", reportData.Master?.REMARKS ?? "");
                                ReportParameter parameter9 = new ReportParameter("CompanyLogo", companyLogoUri);
                                ReportParameter parameter10 = new ReportParameter("ShowSignature1", Convert.ToString(reportData.Master?.MENU_SIG1 ?? true));
                                ReportParameter parameter11 = new ReportParameter("ShowSignature2", Convert.ToString(reportData.Master?.MENU_SIG2 ?? true));
                                ReportParameter parameter12 = new ReportParameter("ShowSignature3", Convert.ToString(reportData.Master?.MENU_SIG3 ?? true));
                                ReportParameter parameter13 = new ReportParameter("ShowSignature4", Convert.ToString(reportData.Master?.MENU_SIG4 ?? true));
                                ReportParameter parameter14 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));

                                report.SetParameters(new ReportParameter[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10, parameter11, parameter12, parameter13, parameter14 });
                                report.Refresh();
                                report.DataSources.Add(new ReportDataSource() { Name = "SalesQutation", Value = reportData.Detail });
                                byte[] file;
                                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\SalesQutationReport");
                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }

                                if (!report.IsReadyForRendering)
                                {
                                    throw new Exception("Report is not ready for printing. Please verify report parameters and dataset.");
                                }
                                string input = reportData.Master?.VOUCHER_NO ?? "";
                                string[] parts = input.Split('/');
                                string prefix = "SQ";
                                string voucherNumber = Convert.ToString(model.TRAN_ID);
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
                                filePath = $"/Client/SalesQutationReport/{filePath}";
                            }
                        }
                    }
            }
            return filePath;
        }

        private DataTable CreateSalesQutationDetailTable()
        {
            DataTable table = new DataTable("SalesQutation");
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("Qty", typeof(decimal));
            table.Columns.Add("Rate", typeof(decimal));
            table.Columns.Add("Amount", typeof(decimal));
            return table;
        }
    }
}
