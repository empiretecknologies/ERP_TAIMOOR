using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class HRMasterController : BaseController
    {
        public IHRMasterService _HRMasterService { get; set; }
        public HRMasterController(IHRMasterService HRMasterService, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _HRMasterService = HRMasterService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            ViewBag.HRMasterTables = DropdownService.HRMasterDropdown(common);
            return View();
        }

        [HttpGet]
        public JsonResult QuickSearch(string TableName)
        {
            try
            {
                var data = _HRMasterService.QuickSearch(CommonHelper.GetValues(HttpContext) , TableName);
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetHRMasterById(int id , string TableName)
        {
            var data = _HRMasterService.GetHRMasterById(id , TableName, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(HRMaster model)
        {
            try
            {
                var data = _HRMasterService.Save(model, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult Delete(int id , string TableName)
        {
            try
            {
                var data = _HRMasterService.Delete(id, TableName, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
