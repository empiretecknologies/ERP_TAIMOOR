using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class ItemOpeningController : BaseController
    {
		public IItemOpeningService _itemOpeningService { get; set; }
        public IPeriodService _periodService { get; set; }
        public ItemOpeningController(IItemOpeningService itemOpeningService, IMenuService menuService, IPeriodService periodService,IBaseService baseService) : base(menuService,baseService)
        {
			_itemOpeningService = itemOpeningService;
            _periodService = periodService;
        }
		
        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            
			var common = CommonHelper.GetValues(HttpContext);
            string b_i = "";
            string maxIdQuery = "SELECT B_I FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(maxIdQuery, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                b_i = Convert.ToString(result);
            }
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");

            ViewBag.BI = b_i;
            return View();
        }

		[HttpGet]
		public JsonResult GetItemOpenings()
		{
			try
			{
				var data = _itemOpeningService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
		public JsonResult GetUnits()
		{
			try
			{
				var data = DropdownService.UnitDropdown();
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
		public JsonResult GetColors()
		{
			try
			{
				var data = DropdownService.ColorDropdown();
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
		public JsonResult GetSizes()
		{
			try
			{
				var data = DropdownService.SizeDropdown();
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
		public JsonResult GetGrades()
		{
			try
			{
				var data = DropdownService.GradeDropdown();
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
		public JsonResult GetWarehouses()
		{
			try
			{
				var data = DropdownService.WareHouseDropdownWthControlNameSub();
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
		public JsonResult Save(ItemOpening itemOpening)
		{
			try
			{
				var data = _itemOpeningService.Save(itemOpening, CommonHelper.GetValues(HttpContext));
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
				var data = _itemOpeningService.Delete(code, CommonHelper.GetValues(HttpContext));
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

		//[HttpPost]
		//public async Task<IActionResult> UploadImage()
		//{

		//	MyHttpResponseMessage response = new MyHttpResponseMessage();
		//	var uniqueFileName = "";
		//	try
		//	{
		//		IFormFile Image = Request.Form.Files[0];
		//		if (Image != null && Image.Length > 0)
		//		{
		//			string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "ItemMaster");
		//			if (!Directory.Exists(uploadsFolder))
		//			{
		//				Directory.CreateDirectory(uploadsFolder);
		//			}
		//			uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + "_.png";
		//			string filePath = Path.Combine(uploadsFolder, uniqueFileName);
		//			using (var fileStream = new FileStream(filePath, FileMode.Create))
		//			{
		//				await Image.CopyToAsync(fileStream);
		//			}
		//		}
		//		response.msg = "File uploaded seccessfully.";
		//		response.msgType = 1;
		//		response.data = $"/Client/ItemMaster/{uniqueFileName}";
		//	}
		//	catch (Exception ex)
		//	{
		//		string _catchMessage = ex.Message;
		//		if (ex.InnerException != null)
		//		{
		//			_catchMessage += "<br/>" + ex.InnerException.Message;
		//		}
		//		response.msg = _catchMessage;
		//		response.msgType = 2;
		//	}

		//	return Json(response);
		//}

		[HttpGet]
		public JsonResult GetItemOpeningByCode(int code)
		{
			try
			{
				var data = _itemOpeningService.GetItemOpeningByCode(code, CommonHelper.GetValues(HttpContext));
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
		public JsonResult GetItemOpeningDetailByCode(int code)
		{
			try
			{
				var data = _itemOpeningService.GetItemOpeningDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
		public JsonResult GetBarcodes(int itemCode)
		{
			try
			{
				var data = DropdownService.BarcodeDropdown(itemCode);
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

		public JsonResult GetBarcodeDetailByCode(int code)
		{
			try
			{
				var data = _itemOpeningService.GetBarcodeDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
		public JsonResult SaveBarcode(BarcodeOpening barcodeOpening)
		{
			try
			{
				var data = _itemOpeningService.SaveBarcode(barcodeOpening, CommonHelper.GetValues(HttpContext));
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
		public JsonResult GetBarcodeByCode(int code)
		{
			try
			{
				var data = _itemOpeningService.GetBarcodeByCode(code, CommonHelper.GetValues(HttpContext));
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
		public JsonResult DeleteBarcode(int code)
		{
			try
			{
				var data = _itemOpeningService.DeleteBarcode(code, CommonHelper.GetValues(HttpContext));
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
