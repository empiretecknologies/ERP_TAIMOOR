using Empire_ERP.Core.Interfaces;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    public class AccountGroupController : BaseController
    {
        public IAccountGroupService _accountGroupService { get; set; }
        public AccountGroupController(IAccountGroupService accountGroupService, IMenuService menuService , IBaseService baseService) : base(menuService,baseService)
        {
            _accountGroupService = accountGroupService;
        }

        public JsonResult GetAccountGroups()
        {
            var data = _accountGroupService.GetAccountGroups();
            return Json(data);
        }
    }
}