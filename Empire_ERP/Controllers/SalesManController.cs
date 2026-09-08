using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    public class SalesManController : BaseController
    {

        public ISalesManService _salesmanservice { get; set; }
        public SalesManController(ISalesManService salesmanservice, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _salesmanservice = salesmanservice;
        }

        public IActionResult Index()
        {
            return View();
        }


        public JsonResult GetAllSalesMan()
        {
            int menuid = 9;
            var data = _salesmanservice.GetAllSalesMan(menuid);
            return Json(data);
        }
    }
}
