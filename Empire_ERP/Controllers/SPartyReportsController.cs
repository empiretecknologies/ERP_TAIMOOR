using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [ExtractMenuCode]
    [CheckSession]
    public class SPartyReportsController : BaseController
    {
        public IPeriodService _periodService { get; set; }
        public ISPartyReportService _partyReportService { get; set; }
        public ICompanyService _companyService { get; set; }
        public SPartyReportsController(IMenuService menuService, IPeriodService periodService, ISPartyReportService partyReportService, ICompanyService companyService,IBaseService baseService) : base(menuService,baseService)
        {
            _periodService = periodService;
            _partyReportService = partyReportService;
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
                var data = _partyReportService.GetReportTypes(CommonHelper.GetValues(HttpContext));
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
        public JsonResult GenerateReport(SPartyReport report)
        {
            try
             {
                var data = _partyReportService.GetReportData(report, CommonHelper.GetValues(HttpContext));
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
        public JsonResult UpdateSodeBookFeedingReport(SodePartyReport modelrecord)
        {
            try
            {
                var data = _partyReportService.UpdateSodeBookFeedingReport(modelrecord, CommonHelper.GetValues(HttpContext));
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