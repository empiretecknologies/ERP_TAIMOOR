using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    public class EntityController : BaseController
    {

        public IEntityService _EntityService { get; set; }
        public EntityController(IEntityService entityService, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {

            _EntityService = entityService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetEntity()
        {
            int menuid = 0;
            var data = _EntityService.GetEntity(menuid);
            return Json(data);
        }


    }
}
