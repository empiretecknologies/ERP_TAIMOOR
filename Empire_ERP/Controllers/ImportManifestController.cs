using System.Text;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Reporting.NETCore;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class ImportManifestController : BaseController
    {
        public IImportManifestService _importManifestService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ImportManifestController(IMenuService menuService, IImportManifestService purchaseSaleFormatService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _importManifestService = purchaseSaleFormatService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Accounts = DropdownService.GetAccountsForCashReceiptVoucher(common.RoleID, common.RoleType);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            ViewBag.Parties = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);
            var response = _menuService.GetMenu(common.MenuID);
            if (response.msgType == 1)
            {
                ViewBag.DC_TYPE = ((Menu)response.data).DCTYPE;
                ViewBag.DATA_CLEAR = ((Menu)response.data).DATA_CLEAR;
            }
            ViewBag.Warehouses = DropdownService.SubWareHouseDropdown();
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            return View();
        }

        [HttpGet]
        public JsonResult GetItems()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = DropdownService.ItemMasterDropdown(common.RoleID, common.RoleType);

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
        public JsonResult GetImportManifests()
        {
            try
            {
                var data = _importManifestService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        public JsonResult Save(CustomImportManifest modelRecord)
        {
            try
            {
                var data = _importManifestService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetImportManifestByCode(int code)
        {
            try
            {
                var data = _importManifestService.GetImportManifestByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _importManifestService.GetImportManifestDetailsByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetImportManifestDetailsByCode(int code)
        {
            try
            {
                var data = _importManifestService.GetImportManifestDetailsByCode(code, CommonHelper.GetValues(HttpContext));
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
                var data = _importManifestService.Delete(code, CommonHelper.GetValues(HttpContext));
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
                var data = _importManifestService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeleteImportManifestDetailByCode(int tranID, int code)
        {
            try
            {
                var data = _importManifestService.DeleteImportManifestDetailByCode(tranID, code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPrintReport(ListPrintReport model)
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

        private string GenerateReport(ListPrintReport model)
        {
            var filePath = "";
            try
            {
                if (model.MD_ID > 0)
                {
                    if (model.TRAN_ID > 0 || model.MD_ID == 26)
                    {
                        Reports.Datasets.BarcodeReportDataset.IgmReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.IgmReportDataTable();
                        var responseMessage = _importManifestService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                        if (responseMessage.msgType != 1)
                        {
                            return "";
                        }
                        var reportData = (CustomPrintReport)responseMessage.data;

                        using (LocalReport report = new LocalReport())
                        {
                            var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.Master?.REPORT_NAME}.rdlc");
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
                                    bool? showCompanyLogo = false;
                                    if (!System.IO.File.Exists(companyLogoPath))
                                    {
                                        showCompanyLogo = false;
                                    }

                                    ReportParameter parameter = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                                    ReportParameter parameter1 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
                                    ReportParameter parameter2 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
                                    ReportParameter parameter3 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                                    ReportParameter parameter4 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                                    ReportParameter parameter5 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);

                                    report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2, parameter3, parameter4, parameter5 });
                                    report.Refresh();
                                    report.DataSources.Add(new ReportDataSource() { Name = "ListReportDataSet", Value = reportData.Detail });
                                    byte[] file;
                                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\ImportManifestReport");
                                    if (!Directory.Exists(uploadsFolder))
                                    {
                                        Directory.CreateDirectory(uploadsFolder);
                                    }

                                    if (report.IsReadyForRendering)
                                    {
                                        string itemName =  reportData.Detail.Rows[0]["Item"].ToString();
                                        file = report.Render("PDF");
                                        filePath = $"IGM {itemName.Replace("/", "-")}"+".pdf";

                                        stReader.Close();
                                        stReader.Dispose();
                                        stream.Flush();
                                        stream.Close();
                                        stream.Dispose();
                                        report.Dispose();
                                        string reportPath = Path.Combine(uploadsFolder, filePath);
                                        System.IO.File.WriteAllBytes(reportPath, file);
                                        filePath = $"/Client/ImportManifestReport/{filePath}";
                                    }
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
