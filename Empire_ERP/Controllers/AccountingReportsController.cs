using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [ExtractMenuCode]
    [CheckSession]
    public class AccountingReportsController : BaseController
    {
        public IPeriodService _periodService { get; set; }
        public IAccountingReportService _accountingReportService { get; set; }
        public ICompanyService _companyService { get; set; }
        public AccountingReportsController(IMenuService menuService, IPeriodService periodService, IAccountingReportService accountingReportService, ICompanyService companyService ,IBaseService baseService) : base(menuService,baseService)
        {
            _periodService = periodService;
            _accountingReportService = accountingReportService;
            _companyService = companyService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
            var currentCompany = (Company)currentCompanyResponse.data;
            ViewBag.CompanyName = currentCompany.C_NAME;
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            ViewBag.BranchText = Convert.ToInt32(common.Branch);
            ViewBag.currentBranch = common.Branch;

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
                    ? DropdownService.GetAccountsForAccountingReport(true, 0, common.Branch, 0)
                    : DropdownService.GetAccountsForAccountingReport(true, common.RoleID, common.Branch, common.ShowSelected);
                //var data = DropdownService.GetAccountsForAccountingReport(true);
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
        public JsonResult GetSubsidiarities()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = common.RoleType == "A"
                    ? DropdownService.GetAccountsForAccountingReport(false, 0, common.Branch, 0)
                    : DropdownService.GetAccountsForAccountingReport(false, common.RoleID, common.Branch, common.ShowSelected);
                //var data = DropdownService.GetAccountsForAccountingReport(false);
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
                var data = _accountingReportService.GetReportTypes(CommonHelper.GetValues(HttpContext));
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
        public JsonResult GenerateReport(AccountingReport report)
        {
            try
            {
                var data = _accountingReportService.GetReportData(report, CommonHelper.GetValues(HttpContext));
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
