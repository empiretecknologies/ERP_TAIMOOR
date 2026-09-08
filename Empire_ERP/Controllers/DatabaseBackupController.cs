using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mail;

namespace Empire_ERP.Controllers
{
    [ExtractMenuCode]
    [CheckSession]
    public class DatabaseBackupController : BaseController
    {
        public IDatabaseBackupService _databaseBackupService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ICompanyService _companyService { get; set; }
        public DatabaseBackupController(IMenuService menuService, IDatabaseBackupService databaseBackupService, IWebHostEnvironment hostingEnvironment, IBaseService baseService, ICompanyService companyService) : base(menuService, baseService)
        {
            _databaseBackupService = databaseBackupService;
            _hostingEnvironment = hostingEnvironment;
            _companyService = companyService;
        }

        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
        }

        [HttpGet]
        public JsonResult GetDBInformation()
        {
            try
            {
                var data = _databaseBackupService.GetDatabaseInformation();
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

        [HttpGet]
        public JsonResult GenerateDBBackup()
        {
            try
            {
                var companydata = CommonHelper.GetValues(HttpContext);
                var currentCompanyResponse = _companyService.GetCompanyByCode(companydata.Company);
                var currentCompany = (Company)currentCompanyResponse.data;
                var data = _databaseBackupService.GenerateDatabaseBackup(_hostingEnvironment.WebRootPath, CommonHelper.GetValues(HttpContext));
                var email = data.data2.ToString();
                var path = data.data.ToString();
                MailMessage emailMessage = new MailMessage();
                emailMessage.To.Add(new MailAddress(email));
                //emailMessage.Bcc.Add(new MailAddress("Support@empiretecknologies.com", "Empire Tecknologies"));
                emailMessage.Subject = $@"Database Backup - {currentCompany.C_NAME}";
                emailMessage.Body = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.WebRootPath, "Client", "NotificationTemplates", "DatabaseBackup.html"));
                emailMessage.Body = emailMessage.Body.Replace("[[OTP]]", Convert.ToString(123456));
                Attachment attachment = new Attachment(Path.Combine(_hostingEnvironment.WebRootPath, "Client", "TempDatabaseBackupZips", path));
                emailMessage.Attachments.Add(attachment);
                var emailResponse = CommonController.SendEmail(emailMessage);

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
    }
}
