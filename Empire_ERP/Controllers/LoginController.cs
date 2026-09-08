using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace Empire_ERP.Controllers
{
    public class LoginController : BaseController
    {
        public ILoginService _loginService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public LoginController(ILoginService loginService, IMenuService IMenuService, IWebHostEnvironment hostingEnvironment, IBaseService baseService) : base(IMenuService, baseService)
        {
            _loginService = loginService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Id")))
            {
                if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Company")))
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Details", "Login");
            }
            ViewBag.IsRememberMe = false;
            //ViewBag.WebsiteName = GetSettingContentByName(dbContext, "Website Name");
            //string loginCookieValue = Empire_Helper.ParseString(Request.Cookies[Empire_Helper.LoginUserCookie]);
            string loginCookieValue = "";
            //string loginCookieValue = Request.Cookies[CommonService.LoginUserCookie];
            if (!string.IsNullOrWhiteSpace(loginCookieValue))
            {
                ViewBag.IsRememberMe = true;
            }
            else
            {
                ViewBag.IsRememberMe = false;
                //Response.Cookies.Delete(CommonService.LoginUserCookie);
                //Response.Cookies.Delete(Empire_Helper.LoginUserCookie);
            }
            Info data = _loginService.GetBackGroundAndLogo();
            ViewBag.Logo = data.MLOGO;
            ViewBag.Icon = data.ICON;
            ViewBag.Background = data.MSCREEN;
            ViewBag.pb_image = data.PB_LOGO;
            ViewBag.c_name = data.C_NAME;
            ViewBag.COPY_RIGHTS = data.COPY_RIGHTS;
            ViewBag.TEL = data.TEL;
            ViewBag.WEBSITE = data.WEBSITE;
            ViewBag.WEBSITE_LINK = "http://" + data.WEBSITE + "/";
            ViewBag.ABOUT_LABEL = data.ABOUT_LABEL;
            ViewBag.ABOUT_LINK = data.ABOUT_LINK;
            ViewBag.CONTACT_LABEL = data.CONTACT_LABEL;
            ViewBag.CONTACT_LINK = data.CONTACT_LINK;
            return View();
        }

        [HttpGet]
        public JsonResult LoginAttempt(string username, string password, bool isremember)
        {
            var data = _loginService.CheckCredentials(username, password, isremember);
            var logo = _loginService.GetBackGroundAndLogo();
            if (data.msgType != 2)
            {
                List<User> users = (List<User>)data.data;
                if (users.Count > 0)
                {
                    HttpContext.Session.Clear();
                    CommonHelper.SetValues(HttpContext);
                    HttpContext.Session.SetString("Username", users[0].USERNAME);
                    HttpContext.Session.SetString("Id", users[0].U_ID.ToString());
                    HttpContext.Session.SetString("Email", users[0].EMAIL);
                    HttpContext.Session.SetString("Picture", users[0].PICTURES);
                    HttpContext.Session.SetString("Name", users[0].FULLNAME);
                    HttpContext.Session.SetInt32("RoleId", users[0].ROLEID);
                    HttpContext.Session.SetString("RoleType", users[0].ROLE_TYPE);
                    HttpContext.Session.SetString("Logo", logo.MLOGO);
                    HttpContext.Session.SetString("Icon", logo.ICON);
                    HttpContext.Session.SetInt32("ShowSelected", users[0].SHOW_SELECTED);
                }

                if (isremember)
                {
                    string cookieGUIDValue = Guid.NewGuid().ToString();
                    var cookieOption = new CookieOptions();
                    cookieOption.Expires = DateTime.UtcNow.AddDays(7);
                    Response.Cookies.Append(CommonService.LoginUserCookie, cookieGUIDValue, cookieOption);
                }
                else
                {
                    Response.Cookies.Delete(CommonService.LoginUserCookie);
                }

                return Json(data);
            }

            return Json(data);
        }

        [CheckSession]
        public IActionResult Details()
        {
            var data = _loginService.GetBackGroundAndLogo();
            ViewBag.Logo = data.MLOGO;
            ViewBag.Background = data.MSCREEN;
            ViewBag.pb_image = data.PB_LOGO;
            ViewBag.c_name = data.C_NAME;
            ViewBag.COPY_RIGHTS = data.COPY_RIGHTS;
            ViewBag.TEL = data.TEL;
            ViewBag.WEBSITE = data.WEBSITE;
            ViewBag.WEBSITE_LINK = "http://" + data.WEBSITE + "/";
            ViewBag.ABOUT_LABEL = data.ABOUT_LABEL;
            ViewBag.ABOUT_LINK = data.ABOUT_LINK;
            ViewBag.CONTACT_LABEL = data.CONTACT_LABEL;
            ViewBag.CONTACT_LINK = data.CONTACT_LINK;

            return View();
        }

        [CheckSession]
        public ActionResult LogOut()
        {
            HttpContext.Session.Remove("Id");
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [CheckSession]
        public async Task<JsonResult> SaveLoginUserDetails(string company, string branch, string period, double lat, double lon)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "client");
            List<string> folderNames = new List<string>
                {
                    "DailyProduction",
                    "DeliveryFeedingReport",
                    "DeliveryFormat",
                    "DeliveryOrder",
                    "MaterialRequisition",
                    "MerchantPurchaseOrderDetail",
                    "MpoLayout",
                    "PurchaseBillReport",
                    "PurchaseOrder",
                    "PurchaseSaleFormatList",
                    "SaleTaxInvoice",
                    "StockReport"
                };

            List<string> results = new List<string>();

            foreach (var folderName in folderNames)
            {
                string folderPath = Path.Combine(basePath, folderName);

                if (Directory.Exists(folderPath))
                {
                    try
                    {
                        Directory.Delete(folderPath, true);
                    }
                    catch (Exception ex)
                    {
                        results.Add($"Error deleting '{folderName}': {ex.Message}");
                    }
                }

            }
            MyHttpResponseMessage res = new MyHttpResponseMessage();
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Id")))
            {
                try
                {
                    await CommonHelper.SetPostalCode(HttpContext, lat, lon);
                }
                catch (Exception ex)
                {
                    //res.msg = "Failed.";
                    //res.msgType = 2;
                    //return Json(res);
                }
                if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Company")))
                    HttpContext.Session.SetString("Company", "");
                if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Branch")))
                    HttpContext.Session.SetString("Branch", "");
                if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Period")))
                    HttpContext.Session.SetString("Period", "");
                HttpContext.Session.SetString("Company", company);
                HttpContext.Session.SetString("Branch", branch);
                HttpContext.Session.SetString("Period", period);
                res.msg = "Saved.";
                res.msgType = 1;
                return Json(res);
            }
            res.msg = "Failed.";
            res.msgType = 2;

        

            return Json(res);
        }

        [HttpGet]
        public JsonResult UsernameVerification(string username)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            if (System.IO.File.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "Client", "NotificationTemplates", "ForgetPassword.html")))
            {
                var data = _loginService.GetUserByUsername(username);
                if (data.msgType == 1)
                {
                    User user = (User)data.data;
                    if (user.U_ID > 0)
                    {
                        OTPUser otpUser = new OTPUser();
                        otpUser.U_ID = user.U_ID;
                        otpUser.STATUS = "EXPIRED";
                        var updateOTPReponse = _loginService.UpdateUserOTPStatus(otpUser);
                        if (updateOTPReponse.msgType == 1)
                        {
                            otpUser = new OTPUser();
                            otpUser.U_ID = user.U_ID;
                            otpUser.EMAIL = user.EMAIL;
                            otpUser.OTP = new Random().Next(100000, 1000000);
                            var otpReponse = _loginService.SaveUserOTP(otpUser);
                            if (otpReponse.msgType == 1)
                            {
                                MailMessage emailMessage = new MailMessage();
                                emailMessage.To.Add(new MailAddress(user.EMAIL, user.FULLNAME));
                                //emailMessage.Bcc.Add(new MailAddress("Support@empiretecknologies.com", "Empire Tecknologies"));
                                emailMessage.Subject = "Password Reset Request";
                                emailMessage.Body = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.WebRootPath, "Client", "NotificationTemplates", "ForgetPassword.html"));
                                emailMessage.Body = emailMessage.Body.Replace("[[FullName]]", user.FULLNAME);
                                emailMessage.Body = emailMessage.Body.Replace("[[OTP]]", Convert.ToString(otpUser.OTP));
                                var emailResponse = CommonController.SendEmail(emailMessage);
                                if (emailResponse)
                                {
                                    response.data = CommonController.MaskEmail(otpUser.EMAIL);
                                    response.msgType = 1;
                                }
                                else
                                {
                                    response.msg = "Something went wrong while sending the email! please try again later.";
                                    response.msgType = 2;
                                }
                            }
                            else
                            {
                                response.msg = "Something went wrong while generating the OTP! please try again later.";
                                response.msgType = 2;
                            }
                        }
                        else
                        {
                            response.msg = "Something went wrong while generating the OTP! please try again later.";
                            response.msgType = 2;
                        }
                    }
                    else
                    {
                        response.msg = "Something went wrong! please try again later.";
                        response.msgType = 2;
                    }
                }
                else
                {
                    response.msg = data.msg;
                    response.msgType = 2;
                }
            }
            else
            {
                response.msg = "Something went wrong! please try again later.";
                response.msgType = 2;
            }
            return Json(response);
        }

        [HttpGet]
        public JsonResult OTPVerification(string username, int? OTP)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var data = _loginService.GetUserByUsername(username);
                if (data.msgType == 1)
                {
                    User user = (User)data.data;
                    if (user.U_ID > 0)
                    {
                        OTPUser otpUser = new OTPUser();
                        otpUser.U_ID = user.U_ID;
                        otpUser.OTP = OTP;
                        response = _loginService.OTPVerification(otpUser);
                    }
                    else
                    {
                        response.msg = "Something went wrong! please try again later.";
                        response.msgType = 2;
                    }
                }
                else
                {
                    response.msg = data.msg;
                    response.msgType = 2;
                }

            }
            catch (Exception ex)
            {
                response.msg = "Something went wrong! please try again later.";
                response.msgType = 2;
            }

            return Json(response);
        }

        [HttpGet]
        public JsonResult UpdatePassword(string username, string password)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var data = _loginService.GetUserByUsername(username);
                if (data.msgType == 1)
                {
                    User user = (User)data.data;
                    if (user.U_ID > 0)
                    {
                        user.UPASS = password;
                        response = _loginService.UpdatePassword(user);
                    }
                    else
                    {
                        response.msg = "Something went wrong! please try again later.";
                        response.msgType = 2;
                    }
                }
                else
                {
                    response.msg = data.msg;
                    response.msgType = 2;
                }

            }
            catch (Exception ex)
            {
                response.msg = "Something went wrong! please try again later.";
                response.msgType = 2;
            }

            return Json(response);
        }

        [HttpGet]
        public JsonResult GetAllDDL()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var AllDDL = _loginService.GetAllDDL(common);
            //var branches = _branchService.GetBranchByCompanyWithRole(id, CommonHelper.GetValues(HttpContext).RoleID);
            return Json(AllDDL);

        }
    }
}