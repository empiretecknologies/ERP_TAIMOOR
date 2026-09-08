using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class BranchController : BaseController
    {
        public IBranchService _branchService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public BranchController(IBranchService branchService, IMenuService IMenuService, IWebHostEnvironment hostingEnvironment, IBaseService baseService) : base(IMenuService , baseService)
        {
            _branchService = branchService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Companyy = DropdownService.CompanyDropdown();
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
        }
        [HttpGet]
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _branchService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetBranchById(int id)
        {
            var data = _branchService.GetBranchById(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(Branch model)
        {
            try
            {
                var data = _branchService.Save(model, CommonHelper.GetValues(HttpContext));
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
                var data = _branchService.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetBranchesByCompany(int id)
        {
            //var branches = _branchService.GetBranchByCompany(id); 
            bool isAdmin = CommonHelper.GetValues(HttpContext).RoleType == "A";
            if (isAdmin)
            {
                var branches = _branchService.GetBranchByCompany(id);
                return Json(branches);
            }
            else
            {
                var branches = _branchService.GetBranchByCompanyWithRole(id, CommonHelper.GetValues(HttpContext).RoleID);
                return Json(branches);
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
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "BranchFiles");
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
                response.data = $"/Client/BranchFiles/{uniqueFileName}";
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
