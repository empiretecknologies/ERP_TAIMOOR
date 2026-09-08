using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class PartyTypesController : BaseController
    {
        public IPartyService _partyService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PartyTypesController(IPartyService partyService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _partyService = partyService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
        }

        [HttpGet]
        public JsonResult GetChartOfAccounts()
        {
            try
            {
                var data = _partyService.GetChartOfAccounts(CommonHelper.GetValues(HttpContext));
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
        public JsonResult SaveMainOtherInfo(PartyTypes partyTypes)
        {
            try
            {
                var data = _partyService.Save(partyTypes, CommonHelper.GetValues(HttpContext));
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
        public JsonResult Delete(int partycode, int actCode)
        {
            try
            {
                var data = _partyService.Delete(partycode, actCode, CommonHelper.GetValues(HttpContext));
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
        public JsonResult CopyRecord(CopyRecord record)
        {
            try
            {
                var data = _partyService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage()
        {

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            try
            {
                IFormFile Image = Request.Form.Files[0];
                if (Image != null && Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "PartyDocuments");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string extension = Path.GetExtension(Image.FileName);
                    uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + "_" + extension;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                }
                response.msg = "File uploaded seccessfully.";
                response.msgType = 1;
                response.data = uniqueFileName;
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

        [HttpGet]
        public JsonResult GetPartyTypeByPartyCode(int partyCode)
        {
            try
            {
                var partyData = _partyService.GetPartyTypeByPartyCode(partyCode, CommonHelper.GetValues(HttpContext));
                var branchInfoData = _partyService.GetBranchesInfoByPartyCode(partyCode, CommonHelper.GetValues(HttpContext));
                return Json(new { partyData = partyData, branchInfoData = branchInfoData });
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
        public JsonResult QuickSearch(int partyCode)
        {
            var data = _partyService.QuickSearch(partyCode, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult QuickSearchParty()
        {
            var data = _partyService.QuickSearchParty(CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        //[HttpPost]
        //public JsonResult QuickSearchLazyLoading(int skip = 0, int take = 12, string sort = null, string filter = null, string group = null)
        //{
        //    var data = _partyService.QuickSearchLazyLoading(CommonHelper.GetValues(HttpContext), skip, take, filter, group);
        //    return Json(data);
        //}

        [HttpPost]
        public JsonResult SaveBranchInfo(PartyTypeBranch partyTypeBranch)
        {
            try
            {
                var data = _partyService.SaveBranchInfo(partyTypeBranch, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetBranchInfoByBranch(int branchCode, int partyCode)
        {
            try
            {
                var branchInfoData = _partyService.GetBranchInfoByBranchId(branchCode, partyCode, CommonHelper.GetValues(HttpContext));
                return Json(branchInfoData);
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
        public JsonResult GetBranchesByPartyCode(int partyCode)
        {
            try
            {
                var branchInfoData = _partyService.GetBranchesInfoByPartyCode(partyCode, CommonHelper.GetValues(HttpContext));
                return Json(branchInfoData);
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
        public JsonResult DeleteBranchInfo(int branchId, int partyCode)
        {
            try
            {
                var data = _partyService.DeleteBranchInfo(branchId, partyCode, CommonHelper.GetValues(HttpContext));
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