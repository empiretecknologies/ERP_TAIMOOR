using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class OpeningBalanceController : BaseController
	{
		public IAccountOpeningService _accountOpeningService { get; set; }
		public OpeningBalanceController(IAccountOpeningService accountOpeningService, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
		{
            _accountOpeningService = accountOpeningService;
		}

		public IActionResult Index()
		{
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
		}

		[HttpGet]
		public JsonResult GetAccountOpenings()
		{
			try
			{
				var data = _accountOpeningService.GetAccountOpenings(CommonHelper.GetValues(HttpContext));
				return Json(data);
			}
			catch (Exception ex)
			{
				string _catchMessage = ex.Message;
				if (ex.InnerException != null)
				{
					_catchMessage += "<br/>" + ex.InnerException.Message;
				}
				return Json(_catchMessage);
			}
		}

        [HttpPost]
        public JsonResult Save(List<AccountOpening> accountOpenings)
        {
            try
            {
                var data = _accountOpeningService.Save(accountOpenings, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }
    }
}