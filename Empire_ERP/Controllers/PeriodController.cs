using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class PeriodController : BaseController
    {
        public IPeriodService _periodService { get; set; }
        public PeriodController(IPeriodService periodService, IMenuService IMenuService, IBaseService baseService) : base(IMenuService, baseService)
        {
            _periodService = periodService;
        }
        
        public IActionResult Index()
        {
            ViewBag.Branch = DropdownService.BranchDropdown();
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
        }

        [HttpGet]
        public JsonResult GetPeriodsByBranch(int branchid)
        {
            var periods = _periodService.GetPeriodsByBranch(branchid);
            return Json(periods);
        }
        [HttpGet]
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _periodService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetPeriodById(int id)
        {
            var data = _periodService.GetPeriodById(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(Period model)
        {
            try
            {
                var data = _periodService.Save(model, CommonHelper.GetValues(HttpContext));
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
                var data = _periodService.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
