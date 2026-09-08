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
    public class BinariesController : BaseController
    {
        public IBinariesService _binariesService { get; set; }
        public BinariesController(IBinariesService binariesService, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _binariesService = binariesService;
        }

        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
        }

        [HttpPost]
        public IActionResult SourceDatabaseDDL(Binaries model)
        {
            try
            {
                var data = _binariesService.SourceDatabaseDDL(model);
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

        public IActionResult DestinationDatabaseDDL(Binaries model)
        {
            try
            {
                var data = _binariesService.DestinationDatabaseDDL(model);
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

        [HttpPost]
        public IActionResult GetDatabaseObjects(Binaries model)
        {
            try
            {
                var data = _binariesService.GetDatabaseObjects(model);
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

        //[HttpPost]
        //public IActionResult GetDatabaseObjects(Binaries model)
        //{
        //    try
        //    {
        //        var data = _binariesService.GetDatabaseObjects(model);
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        //[HttpGet]
        //public JsonResult GetAccounts()
        //{
        //    try
        //    {
        //        var data = DropdownService.WareHouseDropdownWthControlName();
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        //[HttpGet]
        //public JsonResult GetWarehouseAccountsForTreeView()
        //{
        //    try
        //    {
        //        var data = _binariesService.GetWarehouseAccountsForTreeView(CommonHelper.GetValues(HttpContext));
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        //[HttpGet]
        //public JsonResult QuickSearch()
        //{
        //    try
        //    {
        //        var data = _binariesService.QuickSearch(CommonHelper.GetValues(HttpContext));
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        //[HttpGet]
        //public JsonResult GetWarehouseAccountById(int code)
        //{
        //    try
        //    {
        //        var data = _binariesService.GetWarehouseAccountById(code, CommonHelper.GetValues(HttpContext));
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        //[HttpPost]
        //public JsonResult Save(Warehouse model)
        //{
        //    try
        //    {
        //        var data = _binariesService.Save(model, CommonHelper.GetValues(HttpContext));
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        //[HttpPost]
        //public JsonResult CopyRecord(CopyRecord record)
        //{
        //    try
        //    {
        //        var data = _binariesService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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

        //[HttpPost]
        //public JsonResult Delete(int code)
        //{
        //    try
        //    {
        //        var data = _binariesService.Delete(code, CommonHelper.GetValues(HttpContext));
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}
    }
}