using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class ApprovalController : BaseController
    {
        public IApprovalService _ApprovalService { get; set; }
        private readonly ICompositeViewEngine _viewEngine;
        public ApprovalController(IApprovalService ApprovalService, IMenuService menuService, ICompositeViewEngine viewEngine,IBaseService baseService) : base(menuService,baseService)
        {
            _ApprovalService = ApprovalService;
            _viewEngine = viewEngine;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var ID = HttpContext.Session.GetString("Branch");
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            ViewBag.BookTypes = common.RoleType == "A"
                ? DropdownService.GetAllBookTypesForReceiptVoucher(0, common.Branch, 0)
            : DropdownService.GetAllBookTypesForReceiptVoucher(common.RoleID, common.Branch, common.ShowSelected);
            ViewBag.BranchTo = DropdownService.WithOutCurrentBrachDropdown(Convert.ToInt32(ID));
            return View();
        }
        [HttpGet]
        public JsonResult GetApprovals(int Branch)
        {
            try
            {
                var data = _ApprovalService.GetApprovals(Branch, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetApprovalSetup(int Branch)
        {
            try
            {
                var data = _ApprovalService.GetApprovalSetup(Branch, CommonHelper.GetValues(HttpContext));
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
        public JsonResult Save(List<Approval> modelRecord)
        {
            try
            {
                var data = _ApprovalService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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
        public JsonResult SaveSetup(List<Approval> modelRecord)
        {
            try
            {
                var data = _ApprovalService.SaveSetup(modelRecord, CommonHelper.GetValues(HttpContext));
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

    }
}