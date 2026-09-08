using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Reporting.NETCore;
using System.Text;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class MerchantPurchaseOrderController : BaseController
    {
        public IMerchantPurchaseOrderService _merchantPurchaseOrderService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IPeriodService _periodService { get; set; }
        public MerchantPurchaseOrderController(IMerchantPurchaseOrderService merchantPurchaseOrderService, IMenuService menuService, IWebHostEnvironment hostingEnvironment, IPeriodService periodService,IBaseService baseService) : base(menuService,baseService)
        {
            _merchantPurchaseOrderService = merchantPurchaseOrderService;
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
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
            var response = _menuService.GetMenu(common.MenuID);
            ViewBag.Units = DropdownService.UnitDropdown();
            ViewBag.Departments = DropdownService.DepartmentDropdownWithControlName();
            ViewBag.Employee = DropdownService.EmpDropdown();
            ViewBag.ClientPO = DropdownService.ClientPODropdown();
            ViewBag.Fabric = DropdownService.GetFabricData();
            ViewBag.GSMData = DropdownService.GetGSMData();
            if (response.msgType == 1)
            {
                ViewBag.DATA_CLEAR = ((Menu)response.data).DATA_CLEAR;
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetDeliveryFormats()
        {
            var data = _merchantPurchaseOrderService.GetDeliveryFormats(CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetAccountsForTreeView()
        {
            try
            {
                var data = _merchantPurchaseOrderService.GetAccountsForTreeView(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _merchantPurchaseOrderService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //public JsonResult QuickSearchLazyLoading(int skip = 0, int take = 12, string sort = null, string filter = null, string group = null)
        //{
        //    try
        //    {  
        //        var data = _merchantPurchaseOrderService.QuickSearch(CommonHelper.GetValues(HttpContext), skip, take, filter, group);
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet]
        public JsonResult MerchantPurchaseOrderByid(int id)
        {
            var data = _merchantPurchaseOrderService.GetMerchantPurchaseOrderById(id, CommonHelper.GetValues(HttpContext));
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
                var data = DropdownService.ItemMasterDropdown(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).RoleType);
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
        public JsonResult Save(MerchantPurchaseOrder model)
        {
            try
            {
                var data = _merchantPurchaseOrderService.Save(model, CommonHelper.GetValues(HttpContext));
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
                var data = _merchantPurchaseOrderService.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult CopyRecord(CopyRecord record)
        {
            try
            {
                var data = _merchantPurchaseOrderService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPrintReport(MerchantPurchaseOrderReport model)
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

        private string GenerateReport(MerchantPurchaseOrderReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
                {
                    //Reports.Datasets.BarcodeReportDataset.MPOMasterDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.MPOMasterDataTable();
                    var responseMessage = _merchantPurchaseOrderService.GetDataForReport(model, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (MerchantPurchaseOrderReport)responseMessage.data;

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

                                ReportParameter parameter6 = new ReportParameter("Sig1", reportData.SIG1);
                                ReportParameter parameter7 = new ReportParameter("Sig2", reportData.SIG2);
                                ReportParameter parameter8 = new ReportParameter("Sig3", reportData.SIG3);
                                ReportParameter parameter9 = new ReportParameter("Sig4", reportData.SIG4);
                                ReportParameter parameter10 = new ReportParameter("User", reportData.USER);

                                ReportParameter parameter11 = new ReportParameter("Date", reportData.V_DATE.ToString());
                                ReportParameter parameter12 = new ReportParameter("VoucherNo", reportData.VOUCHER_NO);
                                ReportParameter parameter13 = new ReportParameter("Ref", reportData.REF);
                                ReportParameter parameter14 = new ReportParameter("ClientPo", reportData.CLIENT_PO);
                                ReportParameter parameter15 = new ReportParameter("ClientName", reportData.CLIENT_NAME);

                                ReportParameter parameter16 = new ReportParameter("Dep", reportData.DEP);
                                ReportParameter parameter17 = new ReportParameter("SuppName", reportData.SUPPLIER_NAME);
                                ReportParameter parameter18 = new ReportParameter("JobNo", reportData.JOB_NO);
                                ReportParameter parameter19 = new ReportParameter("EmpName", reportData.EMP_NAME);
                                ReportParameter parameter20 = new ReportParameter("TermsName", reportData.TERMS_NAME);

                                ReportParameter parameter21 = new ReportParameter("Curr", reportData.CURR);
                                ReportParameter parameter22 = new ReportParameter("CurrRate", Convert.ToString(reportData.CURR_RATE));
                                ReportParameter parameter23 = new ReportParameter("ItemName", reportData.ITEM_NAME);
                                ReportParameter parameter24 = new ReportParameter("Brand", reportData.BRAND);
                                ReportParameter parameter25 = new ReportParameter("Qty", Convert.ToString(reportData.QTY));

                                ReportParameter parameter26 = new ReportParameter("UnitName", reportData.UNIT_NAME);
                                ReportParameter parameter27 = new ReportParameter("Rate", Convert.ToString(reportData.RATE));
                                ReportParameter parameter28 = new ReportParameter("Amt", Convert.ToString(reportData.AMT));
                                ReportParameter parameter29 = new ReportParameter("CommUnit", reportData.COMM_UNIT);
                                ReportParameter parameter30 = new ReportParameter("Comm", Convert.ToString(reportData.COMM));

                                ReportParameter parameter31 = new ReportParameter("Remarks", reportData.REMARKS);
                                ReportParameter parameter32 = new ReportParameter("CommVal", Convert.ToString(reportData.COMM_VAL));

                                report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, 
                                    parameter9, parameter10, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, parameter19, parameter20, parameter21, 
                                    parameter22, parameter23, parameter24, parameter25, parameter26, parameter27, parameter28, parameter29, parameter30, parameter31, parameter32 });


                                var reportParams = report.GetParameters();
                                foreach (var p in reportParams)
                                {
                                    System.Diagnostics.Debug.WriteLine(
                                        $"Param: {p.Name} | HasValue: {(p.Values?.FirstOrDefault() ?? "NULL")}"
                                    );
                                }

                                report.DataSources.Clear();
                                //report.Refresh();
                                //report.DataSources.Add(new ReportDataSource() { Name = "MPODetail", Value = null });

                                byte[] file;
                                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\DeliveryFormat");
                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }

                                if (report.IsReadyForRendering)
                                {
                                    string input = reportData.VOUCHER_NO;
                                    string[] parts = input.Split('/');
                                    string prefix = string.Empty;
                                    string voucherNumber = string.Empty;
                                    if (parts.Length >= 3)
                                    {
                                        prefix = parts[1];
                                        voucherNumber = parts[^1];
                                    }
                                    file = report.Render("PDF");
                                    filePath = $"{prefix} - {(reportData.CLIENT_NAME).Replace(" / ", " - ")} - {voucherNumber}" + ".pdf";

                                    stReader.Close();
                                    stReader.Dispose();
                                    stream.Flush();
                                    stream.Close();
                                    stream.Dispose();
                                    report.Dispose();
                                    string reportPath = Path.Combine(uploadsFolder, filePath);
                                    System.IO.File.WriteAllBytes(reportPath, file);
                                    filePath = $"/Client/DeliveryFormat/{filePath}";
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

        [HttpGet]
        public JsonResult GetEmp()
        {
            try
            {
                var data = DropdownService.EmpDropdown();
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
        public JsonResult PayTermsDropdown()
        {
            try
            {
                var data = DropdownService.PayTermsDropdown();
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
        public JsonResult GetBrand()
        {
            try
            {
                var data = DropdownService.BrandDropdown();
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
        public JsonResult GetDepartments()
        {
            try
            {
                var data = DropdownService.DepartmentDropdownWithControlName();
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
    }
}