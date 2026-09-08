using Empire_ERP.Core.Interfaces;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    public class AccountNatureController : BaseController
    {
        public IAccountNatureService _accountNatureService { get; set; }
        public AccountNatureController(IAccountNatureService accountNatureService, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _accountNatureService = accountNatureService;
        }

        public JsonResult GetAccountNature()
        {
            var data = _accountNatureService.GetAccountNature();
            return Json(data);
        }
    }
}
