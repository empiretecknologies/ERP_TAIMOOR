using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Empire_ERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using System.Data;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    public class BaseController : Controller
    {
        public IMenuService _menuService { get; set; }
        public IBaseService _baseService { get; set; }

        public BaseController(IMenuService menuService, IBaseService baseService)
        {
            _menuService = menuService;
            _baseService = baseService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var common = CommonHelper.GetValues(HttpContext);
            var host = Request.Host.Host.Split(".").FirstOrDefault();
            Controller controllerContext = (Controller)filterContext.Controller;
            bool isAjaxCall = filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (!isAjaxCall)
            {
                ViewBag.Name = HttpContext.Session.GetString("Username");
                ViewBag.ProfileImage = $"/images/upload/users/{HttpContext.Session.GetString("Picture")}";
                List<Menu> menuBuilders;
                if (HttpContext.Session.GetString("RoleType") == "A")
                {
                    menuBuilders = _menuService.GetMenu().Menu;
                }
                else
                {
                    menuBuilders = _menuService.GetMenuByRole(HttpContext.Session.GetInt32("RoleId")).Menu;
                }

                if (Request.Query.ContainsKey("Code"))
                {
                    int Code = CommonService.ParseInt(Request.Query["CODE"]);
                    if (!menuBuilders.Any(menu => menu.ID == Code))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Login",
                            action = "Index"
                        }));
                        return;
                    }
                    ViewBag.Code = Code;
                }

                string[] requestURL = filterContext.HttpContext.Request.Path.ToString().Split('/');
                string controllerURL = requestURL[1].ToLower();
                ViewBag.ControllerName = CommonService.UpperCaseWords(controllerURL);
                ViewBag.ControllerURL = controllerURL;
                string menuURL = controllerURL;
                string actionURL = string.Empty;
                string Menu = string.Empty;
                string MenuChild = string.Empty;
                string MenuId = string.Empty;
                Menu += "<ul class='nav ul-navbar'>";
                //var Master = menuBuilders.Where(x => x.MENU_PARENT_CODE == 0 && x.DLT == "T" && x.ASTATUS == "Y" && x.MENU_TYPE == 1).OrderBy(x => x.MENU_GRCODE).ToList();

                var MasterList = menuBuilders.Where(x => x.MENU_PARENT_CODE == 0 && x.DLT == "T" && x.ASTATUS == "Y" && x.MENU_TYPE == 1).OrderBy(x => x.MENU_GRCODE).ToList();

                foreach (var master in MasterList)
                {
                    Menu += "<li class='nav-item dropdown'>";
                    string masterUrl = "?MOID=" + master.ID + (Request.Query.ContainsKey("CODE") ? "&Code=" + Request.Query["CODE"] : "");
                    Menu += $"<a href='{masterUrl}' class='nav-link dropdown-toggle' data-bs-toggle='dropdown'><i class='{master.MPIC}'></i>{master.MENU_NAME} &#x2335</a>";

                    var parents = menuBuilders.Where(x => x.MENU_PARENT_CODE == master.ID && x.MENU_TYPE == 2 && x.DLT == "T" && x.ASTATUS == "Y").OrderBy(x => x.MENU_GRCODE).ToList();

                    var directChildren = menuBuilders.Where(x => x.MENU_PARENT_CODE == master.ID && x.MENU_TYPE == 3 && x.DLT == "T" && x.ASTATUS == "Y").OrderBy(x => x.MENU_GRCODE).ToList();

                    Menu += "<ul class='dropdown-menu'>";

                    if (parents.Any())
                    {
                        foreach (var parent in parents)
                        {
                            Menu += "<li class='dropdown-submenu'>";
                            string parentUrl = ViewBag.WebsiteURL + parent.MENU_PAGE + "?MOID=" + master.ID + "&Code=" + parent.ID;
                            Menu += $"<a href='{parentUrl}' class='dropdown-item dropdown-toggle'>{parent.MENU_NAME}</a>";

                            var children = menuBuilders.Where(x => x.MENU_PARENT_CODE == parent.ID && x.MENU_TYPE == 3 && x.DLT == "T" && x.ASTATUS == "Y").OrderBy(x => x.MENU_GRCODE).ToList();

                            if (children.Any())
                            {
                                Menu += "<ul class='dropdown-menu'>";
                                foreach (var child in children)
                                {
                                    string childUrl = ViewBag.WebsiteURL + child.MENU_PAGE + "?MOID=" + parent.ID + "&Code=" + child.ID;
                                    //Menu += $"<li><a class='dropdown-item' href='{childUrl}'>{child.MENU_NAME}</a></li>";
                                    Menu += $"<li><a class='dropdown-item' href='{childUrl}' data-menuurl='{childUrl}' data-mtype='{child.MTYPE}' data-menuid='{child.ID}' data-menuname='{child.MENU_NAME}' data-menuurl='{child.MENU_PAGE}' data-menutype='{child.MENU_TYPE}'>{child.MENU_NAME}</a></li>";

                                }
                                Menu += "</ul>";
                            }
                            Menu += "</li>";
                        }
                    }

                    if (directChildren.Any())
                    {
                        foreach (var child in directChildren)
                        {
                            string childUrl = ViewBag.WebsiteURL + child.MENU_PAGE + "?MOID=" + master.ID + "&Code=" + child.ID;
                            Menu += $"<li><a class='dropdown-item' href='{childUrl}' data-menuurl='{childUrl}' data-mtype='{child.MTYPE}' data-menuid='{child.ID}' data-menuname='{child.MENU_NAME}' data-menuurl='{child.MENU_PAGE}' data-menutype='{child.MENU_TYPE}'>{child.MENU_NAME}</a></li>";
                        }
                    }

                    Menu += "</ul>";
                    Menu += "</li>";
                }

                Menu += "</ul>";
                ViewBag.Menu = Menu;
                bool isSidebarOpen = isSidebarOpens();

                ViewBag.IsSidebarOpen = isSidebarOpen;
                ViewBag.PageURL = ViewBag.WebsiteURL + controllerURL;
                if (Request.Query.ContainsKey("MOID"))
                {
                    string MOID = Request.Query["MOID"];
                    MenuChild += "<ul class='ul-leftside'>";
                    var Child = menuBuilders.Where(x => x.MENU_TYPE == 3 && x.DLT == "T" && x.ASTATUS == "Y" && x.MENU_PARENT_CODE == Convert.ToInt32(MOID)).OrderBy(x => x.MENU_GRCODE).ToList();
                    foreach (var ChildRecord in Child)
                    {
                        string _menuUrl = ViewBag.WebsiteURL + ChildRecord.MENU_PAGE;
                        MenuChild += "<li>";
                        MenuChild += "<a href='" + _menuUrl + "?MOID=" + MOID + "&Code=" + ChildRecord.ID + "'>" + ChildRecord.MENU_NAME + "</a>";
                        MenuChild += "</li>";
                    }
                    MenuChild += "</ul>";
                    ViewBag.ChildMenu = MenuChild;
                }
                string BreadCrumbHtml = "<ol class='breadcrumb breadcrumb-custom  d-flex align-items-center position-relative'>";
                BreadCrumbHtml += "<ol class='breadcrumb position-relative'>";
                //BreadCrumbHtml += "<li class='breadcrumb-item'><a href='" + CommonService.ConvertToWebURL("Home") + "'><i class='fa fa-home'></i></a></li>";
                string url = Url.Action("Index", "Home");
                BreadCrumbHtml += "<li class='breadcrumb-item'><a href=' " + url + " '><i class='fa fa-home'></i></a></li>";
                var MenuPermissionList = menuBuilders.Select(o => o.MENU_NAME.ToLower()).Distinct().ToList();
                if (ViewBag.ControllerName != "Home")
                {
                    string CODE = Request.Query["Code"];
                    if (!String.IsNullOrWhiteSpace(CODE))
                    {
                        var BreadChild = menuBuilders.FirstOrDefault(x => x.ID == Convert.ToInt64(CODE));
                        if (BreadChild != null && !String.IsNullOrWhiteSpace(BreadChild.MENU_NAME))
                        {
                            ViewBag.MENU_NAME = BreadChild.MENU_NAME;
                            var href = CommonService.ConvertToWebURL(controllerURL);
                            BreadCrumbHtml += "<li class='breadcrumb-item'><a href='javascript::'>" + BreadChild.MENU_NAME + "</a></li>";
                            BreadCrumbHtml += $@"<li class='breadcrumb-item position-relative'><a href='javascript::' class='ms-1'
                                 id='gearBtn'><i class='fa-solid fa-gear' ></i></a>
                                </li>
                                ";
                        }
                    }
                }
                BreadCrumbHtml += "</ol>";
                ViewBag.BreadCrumbHTML = BreadCrumbHtml;
                ViewBag.Company = CommonHelper.GetCompanyName(common.Company);
                ViewBag.BranchForNav = CommonHelper.GetBranch(common.Branch);
                ViewBag.Period = CommonHelper.GetPeriod(common.Period);

                var common1 = CommonHelper.GetValues(HttpContext);
                //var Menu1 = _menuService.GetMenu(common.MenuID);
                //int? id = ;
                //if (Menu1.data != null)
                //{
                //    var menu = (Menu)Menu1.data;
                //    id = menu.ID,

                //}

                //var userName = HttpContext.Session.GetString("Username");
                //var mid = HttpContext.Session.GetString("MenuID");
                //var approval = _baseService.GetApproval(userName, common).GetAwaiter().GetResult();
                //ViewBag.StatusAppRR = approval;
                //var data11 = approval.data;

                //ViewBag.StatusApp = approval.data;
                base.OnActionExecuting(filterContext);
            }

            
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var userName = HttpContext.Session.GetString("Username");
            var common = CommonHelper.GetValues(HttpContext);
            var approval = _baseService.GetApproval(userName, common);
            ViewBag.StatusApp = approval.data;
            base.OnActionExecuted(filterContext);
        }
        public bool isSidebarOpens()
        {
            var user = CommonHelper.GetValues(HttpContext);
            string connectionString = new SQLService().getconnstring();
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
                                    SELECT TOP 1 iS_SIDEBAR_OPEN 
                                    FROM TBL_FAVORITE_MENUS 
                                    WHERE USER_ID = @UserID 
                                    ORDER BY ADD_DATE DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", $"{user.UserID}");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            bool isSidebarOpen = false;

            if (dt.Rows.Count > 0 && dt.Rows[0]["iS_SIDEBAR_OPEN"] != DBNull.Value)
            {
                isSidebarOpen = Convert.ToBoolean(dt.Rows[0]["iS_SIDEBAR_OPEN"]);
            }

            return isSidebarOpen;

        }
        [HttpPost]
        public JsonResult UpdateSettings(Base modelRecord)
        {
            try
            {
                var data = _baseService.UpdateSettings(modelRecord, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetSettings()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var Menu = _menuService.GetMenu(common.MenuID);
            string? search = string.Empty, rowlimit = string.Empty;
            int? DClear = 0;

            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                search = menu.SEARCH;
                rowlimit = menu.LIMIT;
                DClear = menu.DATA_CLEAR;

            }
            var data = new
            {
                SEARCH = search,
                ROW_LIMIT = rowlimit,
                DATA_CLEAR = DClear
            };
            return Json(data);

        }

    }
}