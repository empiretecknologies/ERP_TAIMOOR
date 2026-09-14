using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class UploadItemImagesController : BaseController
	{
		public IUploadItemImagesService _UploadItemImagesService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IPeriodService _periodService { get; set; }

        public UploadItemImagesController(IPeriodService periodService, IUploadItemImagesService UploadItemImagesService, IMenuService menuService,IBaseService baseService, IWebHostEnvironment hostingEnvironment) : base(menuService,baseService)
		{
            _UploadItemImagesService = UploadItemImagesService;
            _hostingEnvironment = hostingEnvironment;
            _periodService = periodService;

        }

		public IActionResult Index()
		{
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            ViewBag.ItemGroups = DropdownService.SubsidiaritiesItemGroups();
            ViewBag.Parties = common.RoleType == "A"
                ? DropdownService.PartyTypeDropdownForPartyReport(0, common.Branch, 0)
                : DropdownService.PartyTypeDropdownForPartyReport(common.RoleID, common.Branch, common.ShowSelected);
            return View();
		}

        [HttpGet]
        public JsonResult GetPartyBranches(int partyCode, int actCode)
        {
            try
            {
                var data = _UploadItemImagesService.GetPartyBranches(partyCode, actCode);
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
		public JsonResult GetItemMaster()
		{
			try
			{
                var common = CommonHelper.GetValues(HttpContext);

                var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
                var sDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
                var currentDAte = DateTime.Now.ToString("yyyy-MM-dd");
                var data = _UploadItemImagesService.GetItemMaster(sDate,currentDAte,common);
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
        public JsonResult Save(List<UploadItemImages> UploadItemImages)
        {
            try
            {
                var data = _UploadItemImagesService.Save(UploadItemImages, CommonHelper.GetValues(HttpContext));
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
        public async Task<IActionResult> SaveImage()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            var filePath = "";

            try
            {
                IFormFile uploadedFile = Request.Form.Files[0];
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "ItemMaster");

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

                    response.msg = "Image uploaded successfully.";
                    response.msgType = 1;
                    response.data = $"/Client/ItemMaster/{uniqueFileName}"; // Return file path
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