using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class HRMasterTableController : BaseController
    {
        public IHrMasterTableServices _hrMasterTableServices  { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IMenuDetailsService _menuDetailsService { get; set; }
        public HRMasterTableController(IHrMasterTableServices hrMasterTableServices , IMenuDetailsService menuDetailsService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _hrMasterTableServices = hrMasterTableServices;
            _hostingEnvironment = hostingEnvironment;
            _menuDetailsService = menuDetailsService;
        }
        
        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                 ? "Admin"
                 : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
                ViewBag.MenuList = _menuDetailsService.QuickSearch(CommonHelper.GetValues(HttpContext));
            return View();
        }

       

        [HttpGet]
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _hrMasterTableServices.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetHrTableById(int id)
        {
            var data = _hrMasterTableServices.GetHrTableById(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(HrMasterTable model)
        {
            try
            {
                var data = _hrMasterTableServices.Save(model, CommonHelper.GetValues(HttpContext));
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
                var data = _hrMasterTableServices.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveImage()
        {

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            try
            {
                IFormFile Image = Request.Form.Files[0];
                if (Image != null && Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "SetupSubTypeFiles");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + "_.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                }
                response.msg = "File uploaded successfully.";
                response.msgType = 1;
                response.data = $"/Client/SetupSubTypeFiles/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return Json(response);
        }
    }
}
