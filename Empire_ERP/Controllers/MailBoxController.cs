using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using MailKit.Net.Imap;
using MailKit.Security;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;

namespace Empire_ERP.Controllers
{
    [ExtractMenuCode]
    [CheckSession]
    public class MailBoxController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public MailBoxController(IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendMail(EmailModel model)
        {
            try
            {
                var email = model.To;
                MailMessage emailMessage = new MailMessage();
                emailMessage.To.Add(new MailAddress(email));
                emailMessage.Subject = model.Subject;
                emailMessage.Body = model.Message;
                //Attachment attachment = new Attachment(Path.Combine(_hostingEnvironment.WebRootPath, "Client", "TempDatabaseBackupZips", path));
                //emailMessage.Attachments.Add(attachment);
                var emailResponse = CommonController.SendEmail(emailMessage);

                return Json(new { data = "Mail Send Successfully ", msgType = 1,name = model.To });
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
        public JsonResult GetInboxEmails(int count)
        {
            try
            {
                var emails = CommonController.FetchInboxEmailsAsync(count);
                return Json(new { data = emails, msgType = 1 });

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
        public async Task<IActionResult> GetFullEmail(int id)
        {
            try
            {
                var email = await FetchFullEmailFromServer(id);
                return Ok(new { msgType = 1, data = new { result = email } });
            }
            catch (Exception ex)
            {
                return Ok(new { msgType = 2, data = ex.Message });
            }
        }

        private async Task<FullEmail> FetchFullEmailFromServer(int id)
        {
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.titan.email", 993, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync("Support@empiretecknologies.com", "DS&T5*lKS&^SalesMNT24");

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);
                var uid = new UniqueId((uint)id); // Ensure the ID is valid

                var message = await inbox.GetMessageAsync(uid);

                var email = new FullEmail
                {
                    Id = id,
                    From = message.From.ToString(),
                    Subject = message.Subject,
                    Date = message.Date.DateTime,
                    Body = message.TextBody ?? message.HtmlBody,
                    Attachments = new List<EmailAttachment>()
                };

                foreach (var attachment in message.Attachments)
                {
                    var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                    var memoryStream = new MemoryStream();

                    if (attachment is MessagePart messagePart)
                    {
                        await messagePart.Message.WriteToAsync(memoryStream);
                    }
                    else
                    {
                        var part = (MimePart)attachment;
                        await part.Content.DecodeToAsync(memoryStream);
                    }

                    email.Attachments.Add(new EmailAttachment
                    {
                        Id = Guid.NewGuid().ToString(),
                        FileName = fileName,
                        ContentType = attachment.ContentType.MimeType,
                        Size = memoryStream.Length,
                        Data = memoryStream.ToArray()
                    });
                }

                await client.DisconnectAsync(true);
                return email;
            }
        }

        [HttpGet]
        public IActionResult DownloadAttachment(int emailId, string attachmentId)
        {
            // In a real app, you'd fetch this from your storage/database
           // var attachment = _attachmentService.GetAttachment(emailId, attachmentId);

            //if (attachment == null)
                return NotFound();

            //return File(attachment.Data, attachment.ContentType, attachment.FileName);
        }
    }

    public class EmailModel
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class FullEmail
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public string Body { get; set; }
        public List<EmailAttachment> Attachments { get; set; }
    }

    public class EmailAttachment
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; } 
        public byte[] Data { get; set; } 
    }
}
