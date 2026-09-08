using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Empire_ERP.Core.Services;
using Empire_ERP.Core.Entities;
using System.Collections.Specialized;

namespace Empire_ERP.Helpers
{
    public class CheckSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.Session.GetString("Id");
            if (user == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Index"
                }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public class ExtractMenuCode : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            StringValues parameterValue;
            var request = filterContext.HttpContext.Request;
            var path = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
            if (path.Contains("Code=") && filterContext.HttpContext.Request.Query.TryGetValue("Code", out parameterValue))
            {
                filterContext.HttpContext.Session.SetString("MenuID", Convert.ToString(parameterValue));
            }
            else
            {
                string? url = filterContext.HttpContext.Request.Headers.Referer;

                if (!String.IsNullOrWhiteSpace(url))
                {
                    Uri uri = new Uri(url);

                    NameValueCollection queryParameters = System.Web.HttpUtility.ParseQueryString(uri.Query);

                    string? code = queryParameters["Code"];

                    if (!String.IsNullOrWhiteSpace(code))
                    {
                        filterContext.HttpContext.Session.SetString("MenuID", code);
                    }
                    else
                    {
                        if (!url.Contains("Login/Details"))
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                            {
                                controller = "Home",
                                action = "Index"
                            }));
                        }
                    }
                }
            }
            //else
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            //    {
            //        controller = "Home",
            //        action = "Index"
            //    }));
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}