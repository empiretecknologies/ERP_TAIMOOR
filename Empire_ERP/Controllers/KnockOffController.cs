using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using static Empire_ERP.Core.Entities.KnockOff;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class KnockOffController : BaseController
    {
        public IKnockOffService _knockOffService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public KnockOffController(IKnockOffService knockOffService, IMenuService menuService, IWebHostEnvironment hostingEnvironment, IBaseService baseService) : base(menuService, baseService)
        {
            _knockOffService = knockOffService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index(KnockOff model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            ViewBag.PickData = model;

            return PartialView("_KnockOffPartial");
        }

        [HttpPost]
        public JsonResult GetAllSaleInvoices(KnockOff model)
        {
            try
            {
                var data = _knockOffService.GetAllSaleInvoices(model, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetAllKnockOff(KnockOff model)
        {
            try
            {
                var data = _knockOffService.GetAllKnockOff(model, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpGet]
        //public JsonResult GetCostCenterByID(int id)
        //{
        //    var data = _knockOffService.GetCostCenterByID(id, CommonHelper.GetValues(HttpContext));
        //    return Json(data);
        //}

        [HttpPost]
        public JsonResult Save(CustomKnockOff model)
        {
            try
            {
                var data = _knockOffService.Save(model, CommonHelper.GetValues(HttpContext));
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
                var data = _knockOffService.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveImage()
        //{

        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    var uniqueFileName = "";
        //    try
        //    {
        //        IFormFile Image = Request.Form.Files[0];
        //        if (Image != null && Image.Length > 0)
        //        {
        //            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "SetupSubTypeFiles");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }
        //            uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + "_.png";
        //            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await Image.CopyToAsync(fileStream);
        //            }
        //        }
        //        response.msg = "File uploaded successfully.";
        //        response.msgType = 1;
        //        response.data = $"/Client/SetupSubTypeFiles/{uniqueFileName}";
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        response.msg = _catchMessage;
        //        response.msgType = 2;
        //    }
        //    return Json(response);
        //}
    }
}
