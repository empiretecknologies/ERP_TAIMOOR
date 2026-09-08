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
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using ZXing.QrCode.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class ClosingShopController : BaseController
    {
        public IClosingShopService _ClosingShopService { get; set; }
        public IBranchService _branchService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICompositeViewEngine _viewEngine;
        public ClosingShopController(IMenuService menuService, IBranchService branchService, IClosingShopService ClosingShopService, IWebHostEnvironment hostingEnvironment, ICompositeViewEngine viewEngine,IBaseService baseService) : base(menuService,baseService)
        {
            _ClosingShopService = ClosingShopService;
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

        public IActionResult GetClosingData(DateTime FromDate , DateTime ToDate)
        {
            try
            {
                var data = _ClosingShopService.GetClosingData(FromDate , ToDate, CommonHelper.GetValues(HttpContext));
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
                var data = _ClosingShopService.UpdateClosedData(FromDate , ToDate, CommonHelper.GetValues(HttpContext));
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
        public JsonResult PrintModal(ClosingShop closingData)
        {
            try
            {
                Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable();
                var responseMessage = _ClosingShopService.GetDataForReport(closingData, reportDetails, CommonHelper.GetValues(HttpContext));
                if (responseMessage.msgType != 1)
                {
                    return Json("");
                }
                var reportData = (CustomMenuDetail)responseMessage.data;

                string slipHtml = RenderPartialViewToString(reportData.REPORT_NAME, responseMessage.viewModel);

                responseMessage.SlipHtml = slipHtml;

                return Json(responseMessage);
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
        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"View '{viewName}' not found.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).Wait();
                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public IActionResult SaveClosing([FromBody] SaveClosingRequest request)
        {
            if (request == null || request.Master == null || request.Detail == null)
            {
                return BadRequest("Invalid data.");
            }

            if (string.IsNullOrWhiteSpace(request.Master.FromDate) || string.IsNullOrWhiteSpace(request.Master.ToDate))
            {
                return BadRequest("FromDate and ToDate are required.");
            }

            var data = _ClosingShopService.SaveClosingData(request, CommonHelper.GetValues(HttpContext));

            return Json(new { success = true, message = "Closing data saved successfully." });
        }



    }


}