using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [ExtractMenuCode]
    [CheckSession]
    public class InvoiceReportsController : BaseController
    {
        public IPeriodService _periodService { get; set; }
        public IInvoiceReportsService _InvoiceReportsService { get; set; }
        public ICompanyService _companyService { get; set; }
        public InvoiceReportsController(IMenuService menuService, IPeriodService periodService, IInvoiceReportsService InvoiceReportsService, ICompanyService companyService,IBaseService baseService) : base(menuService,baseService)
        {
            _periodService = periodService;
            _InvoiceReportsService = InvoiceReportsService;
            _companyService = companyService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
            var currentCompany = (Company)currentCompanyResponse.data;
            ViewBag.BranchText = Convert.ToInt32(common.Branch);
            ViewBag.CompanyName = currentCompany.C_NAME;
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            //ViewBag.EndDate = ((Period)periodInfo.data).CLOSING == 1
            //   ? ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd")
            //   : DateTime.Now.ToString("yyyy-MM-dd");

            ViewBag.EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");

            return View();
        }

        [HttpGet]
        public JsonResult GetControls()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = common.RoleType == "A"
                    ? DropdownService.GetAccountsForPartyReport(0, common.Branch,0)
                    : DropdownService.GetAccountsForPartyReport(common.RoleID, common.Branch, common.ShowSelected);
                //var data = DropdownService.GetAccountsForPartyReport();
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
        public JsonResult GetControlsForSalesman()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = common.RoleType == "A" ? DropdownService.GetAccountsForSalesmanReport(0, common.Branch, 0) : DropdownService.GetAccountsForSalesmanReport(common.RoleID, common.Branch, common.ShowSelected);
                //var data = DropdownService.GetAccountsForPartyReport();
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
        public JsonResult GetSalesman()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = common.RoleType == "A"
                    ? DropdownService.SalesmanDropdownForPOS(0, common.Branch, 0)
                    : DropdownService.SalesmanDropdownForPOS(common.RoleID, common.Branch, common.ShowSelected);
                //var data = DropdownService.PartyTypeDropdownForPartyReport();
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
        public JsonResult GetItemGroup()
        {
            try
            {
                var data = DropdownService.GetItemGroupsForPartyReport();
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
        public JsonResult GetBranch()
        {
            try
            {
                var data = DropdownService.GetBranchForStockReport();
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
        public JsonResult GetItemMaster()
        {
            try
            {
                var data = DropdownService.GetItemMasterForPartyReport();
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
        public JsonResult GetItemIds()
        {
            try
            {
                var data = DropdownService.GetItemIds();
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
        public JsonResult GetCategories()
        {
            try
            {
                var data = DropdownService.CategoryDropdown();
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
        public JsonResult GetSubCategories()
        {
            try
            {
                var data = DropdownService.SubCategoryDropdown();
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
        public JsonResult GetBarcodes()
        {
            try
            {
                var data = DropdownService.ItemBarcodeDropdown();
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
        public JsonResult GetColors()
        {
            try
            {
                var data = DropdownService.ColorDropdown();
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
        public JsonResult GetLots()
        {
            try
            {
                //var data2 = DropdownService.LotDropdown();
                var data = DropdownService.WareHouseDropdownWthControlNameWithGr();
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
        public JsonResult GetSizes()
        {
            try
            {
                var data = DropdownService.SizeDropdown();
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
                var data = common.RoleType == "A"
                    ? DropdownService.PartyTypeDropdownForPartyReport(0, common.Branch, 0)
                    : DropdownService.PartyTypeDropdownForPartyReport(common.RoleID, common.Branch, common.ShowSelected);
                //var data = DropdownService.PartyTypeDropdownForPartyReport();
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
        public JsonResult GetReportTypes()
        {
            try
            {
                var data = _InvoiceReportsService.GetReportTypes(CommonHelper.GetValues(HttpContext));
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
        public JsonResult GenerateReport(InvoiceReports report)
        {
            try
            {
                var data = _InvoiceReportsService.GetReportData(report, CommonHelper.GetValues(HttpContext));
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
    }
}