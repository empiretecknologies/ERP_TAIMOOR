using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    public class CategoryController : BaseController
    {
        public ICategoryService _CategoryService { get; set; }   
        public CategoryController(ICategoryService categoryService,IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {

            _CategoryService = categoryService;
        }

        [ExtractMenuCode]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCategory()
        {
            int menuid = 0;
            var data = _CategoryService.GetCategoryies(menuid);
            return Json(data);
        }
    }
}
