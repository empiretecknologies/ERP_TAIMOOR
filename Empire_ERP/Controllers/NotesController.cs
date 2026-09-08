using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class NotesController : BaseController
    {
        public INotesService _regionService { get; set; }
        public ICompanyService _companyService { get; set; }
        public NotesController(INotesService regionService, IMenuService menuService, ICompanyService companyService,IBaseService baseService) : base(menuService,baseService)
        {
            _regionService = regionService;
            _companyService = companyService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
            var currentCompany = (Company)currentCompanyResponse.data;
            ViewBag.CompanyName = currentCompany.C_NAME;
            ViewBag.CompanyAddress = currentCompany.C_ADDRESS;
            return View();
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
        public JsonResult NotesByid(int id)
        {
            var data = _regionService.GetNotesById(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(Notes model)
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