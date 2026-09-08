using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class EmpLeavesController : BaseController
    {
        public IEmpLeavesService _EmpLeavesService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EmpLeavesController(IEmpLeavesService EmpLeavesService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _EmpLeavesService = EmpLeavesService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            ViewBag.EmpLeavesTables = DropdownService.HRMasterDropdown(common);
            ViewBag.EducationRecords = DropdownService.EducationRecords();
            ViewBag.LeaveTypes = DropdownService.GetLeaveType();
            return View();
        }

        [HttpGet]
        public JsonResult QuickSearch(string TableName)
        {
            try
            {
                var data = _EmpLeavesService.QuickSearch(CommonHelper.GetValues(HttpContext) , TableName);
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetEmpLeavesRecord(int id)
        {
            var data = _EmpLeavesService.GetEmpLeavesRecord(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetEmployeeGroups()
        {
            var data = DropdownService.GetEmplyeeGroup();
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(EmpLeaves model)
        {
            try
            {
                var data = _EmpLeavesService.Save(model, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            try
            {
                var data = _EmpLeavesService.Delete(id, CommonHelper.GetValues(HttpContext));
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
            var filePath = "";

            try
            {
                IFormFile uploadedFile = Request.Form.Files[0]; // Get the uploaded file
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "uploads", "EmpLeaves");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Get the file extension
                    string fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();


                    // Generate a unique filename for the PDF
                    uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + fileExtension;
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the PDF file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    response.msg = "PDF uploaded successfully.";
                    response.msgType = 1;
                    response.data = $"/Client/uploads/EmpLeaves/{uniqueFileName}"; // Return file path
                }
                else
                {
                    response.msg = "No file uploaded.";
                    response.msgType = 2;
                }
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
