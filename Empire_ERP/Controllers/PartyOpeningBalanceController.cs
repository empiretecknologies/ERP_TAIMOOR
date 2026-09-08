using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class PartyOpeningBalanceController : BaseController
	{
		public IPartyOpeningService _partyOpeningService { get; set; }
        public IPeriodService _periodService { get; set; }
        public PartyOpeningBalanceController(IPartyOpeningService partyOpeningService, IMenuService menuService, IPeriodService periodService,IBaseService baseService) : base(menuService,baseService)
		{
			_partyOpeningService = partyOpeningService;
			_periodService = periodService;
		}

		public IActionResult Index()
		{
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            return View();
		}

		[HttpGet]
		public JsonResult GetPartyOpenings()
		{
			try
			{
				var data = _partyOpeningService.QuickSearch(CommonHelper.GetValues(HttpContext));
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

        [HttpGet]
        public JsonResult GetPartyOpeningDetails(int code)
        {
            try
            {
                var data = _partyOpeningService.GetPartyOpeningDetails(code, CommonHelper.GetValues(HttpContext));
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

        [HttpGet]
		public JsonResult GetCurrencies()
		{
			try
			{
				var data = DropdownService.CurrencyDropdownWithControlName();
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

		[HttpGet]
		public JsonResult GetPartyOpeningDetailByCode(int code)
		{
			try
			{
				var data = _partyOpeningService.GetPartyOpeningDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
		public JsonResult Save(PartyOpening partyOpening)
		{
			try
			{
				var data = _partyOpeningService.Save(partyOpening, CommonHelper.GetValues(HttpContext));
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

		[HttpGet]
		public JsonResult GetPartyOpeningByCode(int code)
		{
			try
			{
				var data = _partyOpeningService.GetPartyOpeningByCode(code, CommonHelper.GetValues(HttpContext));
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
		public JsonResult Delete(int code)
		{
			try
			{
				var data = _partyOpeningService.Delete(code, CommonHelper.GetValues(HttpContext));
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