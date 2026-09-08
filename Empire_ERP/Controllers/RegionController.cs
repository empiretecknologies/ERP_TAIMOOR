using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class RegionController : BaseController
    {
        public IRegionService _regionService { get; set; }
        public RegionController(IRegionService regionService, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _regionService = regionService;
        }

        public IActionResult Index()
        {
            ViewBag.Cheque = JsonConvert.SerializeObject(DropdownService.ChequeFormatDropdown());
            ViewBag.Currency = DropdownService.CurrencyDropdown();
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
        }

        [HttpGet]
        public JsonResult GetRegionsDropDown()
        {
            int menuid = 0;
            var data = _regionService.GetRegionsDropDown(menuid);
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetRegions()
        {
            var data = _regionService.GetRegions(CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetAccountsForTreeView()
        {
            try
            {
                var data = _regionService.GetAccountsForTreeView(CommonHelper.GetValues(HttpContext));
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
                var data = _regionService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult RegionByid(int id)
        {
            var data = _regionService.GetRegionById(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(Region model)
        {
            try
            {
                var data = _regionService.Save(model, CommonHelper.GetValues(HttpContext));
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
                var data = _regionService.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}