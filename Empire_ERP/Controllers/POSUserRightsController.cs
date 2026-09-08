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
    public class POSUserRightsController : BaseController
    {
        //public ICommMapService _posUserRight { get; set; }
        public IPOSUserRightsService _posUserRight { get; set; }


        public IPeriodService _periodService { get; set; }
        public POSUserRightsController(IMenuService menuService, IPOSUserRightsService posUserRight, IPeriodService periodService,IBaseService baseService) : base(menuService,baseService)
        {
            _posUserRight = posUserRight;
            _periodService = periodService;
        }
        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            var Menu = _menuService.GetMenu(common.MenuID);

            //var response = _menuService.GetMenu(common.MenuID);
            if (Menu.msgType == 1)
            {
                ViewBag.DC_TYPE = ((Menu)Menu.data).DCTYPE;
                ViewBag.DATA_CLEAR = ((Menu)Menu.data).DATA_CLEAR;
            }

            
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            ViewBag.Users = DropdownService.GetPosUsers();



            return View();
        }

        [HttpGet]
        public JsonResult GetSalesMan(Common common)
        {
            try
            {
                var data = DropdownService.SalesmanNameDropdown(common);
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
        
        public JsonResult GetPosUsers()
        {
            try
            {
                var data = _posUserRight.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        public JsonResult Save(POSUserright modelRecord)
        {
            try
            {
                var data = _posUserRight.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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

        public JsonResult GetPosUserByCode(int code)
        {
            try
            {
                var data = _posUserRight.GetPosUserByCode(code, CommonHelper.GetValues(HttpContext));
                return Json(new { Master = data});
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
                var data = _posUserRight.Delete(code, CommonHelper.GetValues(HttpContext));
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
        //public JsonResult CopyRecord(CopyRecordSalesman record)
        //{
        //    try
        //    {
        //        var data = _posUserRight.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeleteCommisionMapDetailByCode(int gcode, int code)
        {
            try
            {
                var data = _posUserRight.DeleteCommisionMapDetailByCode(gcode, code, CommonHelper.GetValues(HttpContext));
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

        
    }
}
