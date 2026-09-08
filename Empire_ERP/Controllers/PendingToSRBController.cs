using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using ZXing.QrCode.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class PendingToSRBController : BaseController
    {
        public IPOSTransactionService _POSTransactionService { get; set; }
        public IBranchService _branchService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICompositeViewEngine _viewEngine;
        public PendingToSRBController(IMenuService menuService, IBranchService branchService, IPOSTransactionService POSTransactionService, IWebHostEnvironment hostingEnvironment, ICompositeViewEngine viewEngine,IBaseService baseService) : base(menuService,baseService)
        {
            _POSTransactionService = POSTransactionService;
            _branchService = branchService;
            _hostingEnvironment = hostingEnvironment;
            _viewEngine = viewEngine;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            string nextId = "";
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);

            return View();
        }

        [HttpGet]

        public IActionResult GetPendingRecords(DateTime FromDate , DateTime ToDate)
        {
            try
            {
                var data = _POSTransactionService.GetPendingRecords(FromDate , ToDate, CommonHelper.GetValues(HttpContext));
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
        public IActionResult UpdateClosedData(DateTime FromDate , DateTime ToDate)
        {
            try
            {
                //var data = _POSTransactionService.UpdateClosedData(FromDate , ToDate, CommonHelper.GetValues(HttpContext));
                return Json("");
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

        [HttpGet]
        public JsonResult GetReportTypes()
        {
            try
            {
                var data = _menuService.GetMenuDetails(CommonHelper.GetValues(HttpContext).MenuID);
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }
        [HttpPost]
        public IActionResult PostToSRB([FromBody] SRBPostModel model)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                    // ❗ Blocking call without async/await
                    var response = client.PostAsync(model.SrbUrl, content).Result;

                    var responseText = response.Content.ReadAsStringAsync().Result;

                    return Content(responseText, "application/json");
                }
            }
            catch (Exception ex)
            {
                // Optionally return error message
                return Content("{\"resCode\":\"99\",\"err\":\"" + ex.Message + "\"}", "application/json");
            }
        }
    }


}