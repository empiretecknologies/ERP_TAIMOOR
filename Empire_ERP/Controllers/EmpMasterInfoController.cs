using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class EmpMasterInfoController : BaseController
    {
        public IEmpMasterInfoService _EmpMasterInfoService { get; set; }
        public EmpMasterInfoController(IEmpMasterInfoService EmpMasterInfoService, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _EmpMasterInfoService = EmpMasterInfoService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            ViewBag.EmpMasterInfoTables = DropdownService.HRMasterDropdown(common);
            ViewBag.EducationRecords = DropdownService.EducationRecords();
            return View();
        }

        [HttpGet]
        public JsonResult QuickSearch(string TableName)
        {
            try
            {
                var data = _EmpMasterInfoService.QuickSearch(CommonHelper.GetValues(HttpContext) , TableName);
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetEmpMasterInfoById(int id , string TableName)
        {
            var data = _EmpMasterInfoService.GetEmpMasterInfoById(id , TableName, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetEmployeeGroups()
        {
            var data = DropdownService.GetEmplyeeGroup();
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(EmpMasterInfo model)
        {
            try
            {
                var data = _EmpMasterInfoService.Save(model, CommonHelper.GetValues(HttpContext));
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
                var data = _EmpMasterInfoService.Delete(id, TableName, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
