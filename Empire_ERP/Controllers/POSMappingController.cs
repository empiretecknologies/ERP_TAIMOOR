using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Text;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class POSMappingController : BaseController
    {
        public IPOSMappingService _POSMappingService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public POSMappingController(IPOSMappingService chartOfAccountService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _POSMappingService = chartOfAccountService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            var common = CommonHelper.GetValues(HttpContext);
            var Menu = _menuService.GetMenu(common.MenuID);
            int? pType = 0;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                pType = menu.PTYPE;
            }

            ViewBag.CashAccount = DropdownService.AcountNameDropdown();
            ViewBag.BankAccount = DropdownService.BanksNameDropdown(pType,common);
            ViewBag.PartAccount = common.RoleType == "A"
                ? DropdownService.GetPartyName(0, common.RoleType)
                : DropdownService.GetPartyName(common.RoleID, common.RoleType);
            ViewBag.Branch = DropdownService.GetBranchForStockReport();
            return View();
        }

        [HttpGet]
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _POSMappingService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult POSMappingByid(int id)
        {
            var data = _POSMappingService.GetPOSMappingById(id, CommonHelper.GetValues(HttpContext));
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

        [HttpPost]
        public JsonResult Save(POSMapping model)
        {
            try
            {
                var data = _POSMappingService.Save(model, CommonHelper.GetValues(HttpContext));
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
                var data = _POSMappingService.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetPrintReport(POSMappingReport model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                byte[] file = GenerateReport(model);
                if (file != null && file.Length > 0)
                {
                    string base64File = Convert.ToBase64String(file);
                    response.data = base64File;
                    response.msg = "Report generated successfully.";
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

        private byte[] GenerateReport(POSMappingReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
                {
                    Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable();
                    var responseMessage = _POSMappingService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return null;
                    }
                    var reportData = (POSMappingReport)responseMessage.data;

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

                                ReportParameter parameter = new ReportParameter("CompanyName", reportData.COMPANY_NAME);
                                ReportParameter parameter1 = new ReportParameter("CompanyAddress", reportData.COMPANY_ADDRESS);
                                ReportParameter parameter2 = new ReportParameter("CompanyPhone", reportData.COMPANY_PHONE);
                                ReportParameter parameter3 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.COMPANY_LOGO}")).AbsoluteUri);
                                ReportParameter parameter4 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                                ReportParameter parameter5 = new ReportParameter("Header", reportData.HEADER_NAME);
                                ReportParameter parameter11 = new ReportParameter("V_DATE", reportData.V_DATE.ToString());
                                ReportParameter parameter12 = new ReportParameter("VOUCHER_NO", reportData.VOUCHER_NO);
                                ReportParameter parameter13 = new ReportParameter("PARTY_CODE", reportData.PARTY_CODE);
                                ReportParameter parameter14 = new ReportParameter("DUE_NO", reportData.DUE_NO);
                                ReportParameter parameter15 = new ReportParameter("DRIVER", reportData.DRIVER);
                                ReportParameter parameter16 = new ReportParameter("VEHICLE", reportData.VEHICLE);
                                ReportParameter parameter17 = new ReportParameter("QUANTITY", reportData.QUANTITY.ToString());
                                ReportParameter parameter18 = new ReportParameter("LOT_NO", reportData.LOT_NO);
                                ReportParameter parameter19 = new ReportParameter("ITEM_CODE", reportData.ITEM_CODE);
                                ReportParameter parameter20 = new ReportParameter("UNIT", reportData.UNIT);
                                ReportParameter parameter21 = new ReportParameter("MENU_SIG1", reportData.MENU_SIG1);
                                ReportParameter parameter22 = new ReportParameter("MENU_SIG2", reportData.MENU_SIG2);
                                ReportParameter parameter23 = new ReportParameter("MENU_SIG3", reportData.MENU_SIG3);
                                ReportParameter parameter24 = new ReportParameter("MENU_SIG4", reportData.MENU_SIG4);
                                ReportParameter parameter25 = new ReportParameter("MENU_TERMS", reportData.MENU_TERMS);
                                

                                report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2, parameter3, parameter4, parameter5, parameter11, parameter12, parameter13, parameter14, parameter15,
                            parameter16, parameter17, parameter18, parameter19, parameter20, parameter21, parameter22, parameter23, parameter24, parameter25 });
                                report.Refresh();
                                //report.DataSources.Add(new ReportDataSource() { Name = "InvoiceReportDataSet", Value = reportData.Detail });

                                if (report.IsReadyForRendering)
                                {
                                    return report.Render("PDF");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        [HttpGet]
        public JsonResult GetUnits()
        {
            try
            {
                var data = DropdownService.UnitDropdown();
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
        public JsonResult GetLots()
        {
            try
            {
                var data = DropdownService.LotDropdown();
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
        public JsonResult GetParties()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = DropdownService.PartyTypeWithAccountCodeDynamic(common);
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

        [HttpPost]
        public async Task<IActionResult> SaveImage(string imageName)
        {

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            var filePath = "";
            try
            {
                IFormFile Image = Request.Form.Files[0];
                if (Image != null && Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", $"POSMapping");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + "_.png";
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                }
                response.msg = "File uploaded successfully.";
                response.msgType = 1;
                response.data = $"/Client/POSMapping/{uniqueFileName}";
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

    }
}