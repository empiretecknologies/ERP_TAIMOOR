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
    public class FDeliveryFeedingController : BaseController
    {
        public IFDeliveryFeedingService _deliveryFeedingService { get; set; }
        public IBranchService _branchService { get; set; }
        public IMenuRepository _menuRepository { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IPeriodService _periodService { get; set; }
        public FDeliveryFeedingController(IMenuRepository menuRepository, IMenuService menuService, IBranchService branchService, IFDeliveryFeedingService deliveryFeedingService, IWebHostEnvironment hostingEnvironment, IPeriodService periodService,IBaseService baseService) : base(menuService,baseService)
        {
            _menuRepository = menuRepository;
            _deliveryFeedingService = deliveryFeedingService;
            _branchService = branchService;
            _hostingEnvironment = hostingEnvironment;
            _periodService = periodService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var Menu = _menuRepository.GetMenu(common.MenuID);
            string? pickData = string.Empty;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                pickData = menu.PICK_DATA;
            }

            //ViewBag.Items = DropdownService.ItemMasterDropdown();
            ViewBag.Items = DropdownService.ItemMasterDropdown(common.RoleID, common.RoleType);
            //ViewBag.Units = DropdownService.UnitDropdownWithQuantity();
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            Branch branch = (Branch)_branchService.GetBranchByCode(common.Branch).data;
            ViewBag.Units = DropdownService.UnitDropdownWithQuantity();
            ViewBag.Warehouse = DropdownService.WareHouseDropdownWthControlNameWithGr();
            ViewBag.Branch_RT_TYPE = branch.RT_TYPE;
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
            ViewBag.PICK_DATA = pickData;
            var response = _menuService.GetMenu(common.MenuID);
            if (response.msgType == 1)
            {
                ViewBag.DATA_CLEAR = ((Menu)response.data).DATA_CLEAR;
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetParties()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);
                //var data = DropdownService.CustomPartyTypeDropdownWithAccountCode();
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
        public JsonResult GetCOA()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = common.RoleType == "A"
                    ? DropdownService.GetAccountsForAccountingReport(false, 0, common.Branch, 0)
                    : DropdownService.GetAccountsForAccountingReport(false, common.RoleID, common.Branch, common.ShowSelected);
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
        public JsonResult GetDeliveryFeedings()
        {
            try
            {
                var data = _deliveryFeedingService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetDeliveryFeeding()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = _deliveryFeedingService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        //public JsonResult GetDeliveryFeeding(int skip = 0, int take = 12, string sort = null, string filter = null, string group = null)
        //{
        //    try
        //    {
        //        var common = CommonHelper.GetValues(HttpContext);
        //        var data = _deliveryFeedingService.QuickSearchLazyLoading(CommonHelper.GetValues(HttpContext), skip, take, filter, group);
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(new { msg = _catchMessage, msgType = 2 });
        //    }
        //}

        [HttpPost]
        public JsonResult Save(CustomFDeliveryFeeding modelRecord)
        {
            try
            {
                var data = _deliveryFeedingService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetDeliveryFeedingByCode(int code)
        {
            try
            {
                var data = _deliveryFeedingService.GetDeliveryFeedingByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _deliveryFeedingService.GetDeliveryFeedingDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetDeliveryFeedingDefaultOperators()
        {
            try
            {
                var data = _deliveryFeedingService.GetDeliveryFeedingDefaultOperators(CommonHelper.GetValues(HttpContext));
                return Json(new { data = data, msgType = 1 });
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
        public JsonResult GetDeliveryFeedingDetailByCode(int code)
        {
            try
            {
                var data = _deliveryFeedingService.GetDeliveryFeedingDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
                var data = _deliveryFeedingService.Delete(code, CommonHelper.GetValues(HttpContext));
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
                var data = _deliveryFeedingService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeleteDeliveryFeedingDetailByCode(int code)
        {
            try
            {
                var data = _deliveryFeedingService.DeleteDeliveryFeedingDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate)
        {
            try
            {
                var data = _deliveryFeedingService.GetSodaBookFeedingDetailBySodaDate(sodaDate, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPrintReport(FDeliveryFeedingReport model)
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

        private string GenerateReport(FDeliveryFeedingReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
                {
                    Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable();
                    var responseMessage = _deliveryFeedingService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return null;
                    }
                    var reportData = (CustomDeliveryFeedingForPrintReport)responseMessage.data;

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
                                var BrokerAmount = CommonController.ToAccountingFormat(reportData.Master?.BR_AMOUNT);
                                var WeightAmount = CommonController.ToAccountingFormat(reportData.Master?.WT_AMOUNT);
                                var NetAmount = CommonController.ToAccountingFormat(reportData.Master?.NET_AMOUNT);
                                var Bardana = CommonController.ToAccountingFormat(reportData.Master?.BARDANA);
                                var GodCharges = CommonController.ToAccountingFormat(reportData.Master?.GOD_CHARGES);
                                var Labour = CommonController.ToAccountingFormat(reportData.Master?.LABOUR);
                                var Frieght = CommonController.ToAccountingFormat(reportData.Master?.FRIEGHT);
                                var Fumigation = CommonController.ToAccountingFormat(reportData.Master?.FUMIGATION);

                                //if (!WeightAmount.Contains("("))
                                //{
                                //    WeightAmount = $"({WeightAmount})";
                                //}

                                ReportParameter parameter = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                                ReportParameter parameter1 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
                                ReportParameter parameter2 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
                                ReportParameter parameter3 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                                ReportParameter parameter4 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                                ReportParameter parameter5 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
                                ReportParameter parameter6 = new ReportParameter("InvoiceNumber", reportData.Master?.INVOICE_NUMBER);
                                ReportParameter parameter7 = new ReportParameter("SodaDate", reportData.Master?.SODA_DATE);
                                ReportParameter parameter8 = new ReportParameter("DeliveryDate", reportData.Master?.DELIVERY_DATE);
                                ReportParameter parameter9 = new ReportParameter("Condition", reportData.Master?.CONDITION);
                                ReportParameter parameter10 = new ReportParameter("PartyName", reportData.Master?.PARTY_NAME);
                                ReportParameter parameter11 = new ReportParameter("TruckNumber", reportData.Master?.TRUCK_NO);
                                ReportParameter parameter12 = new ReportParameter("BrokerAmount", BrokerAmount);
                                ReportParameter parameter13 = new ReportParameter("WeightAmount", WeightAmount);
                                ReportParameter parameter14 = new ReportParameter("NetAmount", NetAmount);
                                ReportParameter parameter15 = new ReportParameter("Bardana", Bardana);
                                ReportParameter parameter16 = new ReportParameter("GodCharges", GodCharges);
                                ReportParameter parameter17 = new ReportParameter("Labour", Labour);
                                ReportParameter parameter18 = new ReportParameter("Frieght", Frieght);
                                ReportParameter parameter19 = new ReportParameter("Fumigation", Fumigation);
                                ReportParameter parameter20 = new ReportParameter("MENU_TERMS", reportData.Master?.MENU_TERMS);
                                ReportParameter parameter21 = new ReportParameter("MENU_SIG1", reportData.Master?.MENU_SIG1);
                                ReportParameter parameter22 = new ReportParameter("BROKER_NAME", reportData.Master?.BROKER_NAME);
                                ReportParameter parameter23 = new ReportParameter("COMMENT", reportData.Master?.COMMENT);

                                report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, parameter19, parameter20, parameter21, parameter22, parameter23 });
                                report.Refresh();
                                report.DataSources.Add(new ReportDataSource() { Name = "InvoiceReportDataSet", Value = reportData.Detail });

                                byte[] file;
                                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\DeliveryFeedingReport");
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
                                    filePath = $"{prefix} - {(reportData.Master?.PARTY_NAME).Replace("/", "-").Replace("#", "-")} - {voucherNumber}" + ".pdf";

                                    stReader.Close();
                                    stReader.Dispose();
                                    stream.Flush();
                                    stream.Close();
                                    stream.Dispose();
                                    report.Dispose();
                                    string reportPath = Path.Combine(uploadsFolder, filePath);
                                    System.IO.File.WriteAllBytes(reportPath, file);
                                    filePath = $"/Client/DeliveryFeedingReport/{filePath}";
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