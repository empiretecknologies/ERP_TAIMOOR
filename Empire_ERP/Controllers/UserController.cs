using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class UserController : BaseController
    {
        public IUserService _userService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public UserController(IUserService userService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            ViewBag.Branch = DropdownService.BranchDropdown();
            ViewBag.Role = DropdownService.RoleDropdown();
            return View();
        }

        [HttpGet]
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _userService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetUserByID(int id)
        {
            var data = _userService.GetUserById(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetRolesByType(string type)
        {
            var data = DropdownService.RoleDropdownByType(type);
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(User model)
        {
            try
            {
                var data = _userService.Save(model, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var data = _userService.Delete(id, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult UserEmailVerification(string email)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            if (System.IO.File.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "Client", "NotificationTemplates", "ConfirmEmail.html")))
            {
                OTPUser otpUser = new OTPUser();
                otpUser.EMAIL = email;
                otpUser.OTP = new Random().Next(100000, 1000000);
                var otpReponse = _userService.SaveUserOTP(otpUser);
                if (otpReponse.msgType == 1)
                {
                    MailMessage emailMessage = new MailMessage();
                    emailMessage.To.Add(new MailAddress(email));
                    //emailMessage.Bcc.Add(new MailAddress("Support@empiretecknologies.com", "Empire Tecknologies"));
                    emailMessage.Subject = "Email Confirmation";
                    emailMessage.Body = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.WebRootPath, "Client", "NotificationTemplates", "ConfirmEmail.html"));
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


            return Json(response);
        }

        [HttpGet]
        public JsonResult OTPVerification(string email, int? OTP)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                if (email is not null)
                {
                    OTPUser otpUser = new OTPUser();
                    otpUser.U_ID = 0;
                    otpUser.EMAIL = email;
                    otpUser.OTP = OTP;
                    response = _userService.OTPVerification(otpUser);
                }
                else
                {
                    response.msg = "Something went wrong! please try again later.";
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

        [HttpPost]
        public async Task<IActionResult> SaveImage()
        {

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            try
            {
                IFormFile Image = Request.Form.Files[0];
                if (Image != null && Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "upload", "users");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + "_.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                }
                response.msg = "File uploaded successfully.";
                response.msgType = 1;
                response.data = $"{uniqueFileName}";
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
