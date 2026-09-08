using Empire_ERP.Core.Entities;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.Concurrent;
using System.Net.Mail;

namespace Empire_ERP.Controllers
{
    public class CommonController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CommonController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public static bool SendEmail(MailMessage emailMessage)
        {
            string mailFrom = "Support@empiretecknologies.com",
                mailFromName = "Empire Tecknologies",
                password = "DS&T5*lKS&^SalesMNT24",
                port = "587",
                host = "smtp.titan.email";
            var isSent = false;
            try
            {
                emailMessage.Body += "<br/>";
                emailMessage.IsBodyHtml = true;
                emailMessage.From = new MailAddress(mailFrom, mailFromName);

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
                client.Port = Convert.ToInt32(port);
                client.Host = host;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.Timeout = 600000;
                client.EnableSsl = true;
                client.Send(emailMessage);
                isSent = true;
            }
            catch (Exception ex)
            {

            }
            return isSent;
        }

        public static string MaskEmail(string email)
        {
            var parts = email.Split('@');
            if (parts.Length != 2) return email;

            string local = parts[0];
            string domain = parts[1];

            int showChars = 2;
            if (local.Length <= showChars) return email;

            string maskedLocal = local.Substring(0, showChars) + new string('*', local.Length - showChars);

            return $"{maskedLocal}@{domain}";
        }

        public static string ToAccountingFormat(decimal? value)
        {
            return value.HasValue ? value >= 0 ? value.Value.ToString("#,##0.00") : "(" + Math.Abs(value.Value).ToString("#,##0.00") + ")" : "0";
        }

        [HttpPost]
        public async Task<IActionResult> UploadVoucherDocs()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var uniqueFileName = "";
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile uploadedFile = Request.Form.Files[0];

                    if (uploadedFile != null && uploadedFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Client", "Docs");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string fileExtension = Path.GetExtension(uploadedFile.FileName);

                        uniqueFileName = Guid.NewGuid().ToString().Substring(0, 25) + fileExtension;

                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }

                        response.msg = "Doc uploaded successfully.";
                        response.msgType = 1;
                        response.data = $"/Client/Docs/{uniqueFileName}";
                    }
                    else
                    {
                        response.msg = "No file to upload or file is empty.";
                        response.msgType = 2;
                    }
                }
                else
                {
                    response.msg = "No file found in the request.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = errorMessage;
                response.msgType = 2;
            }
            return Json(response);
        }

        //public static async Task<List<ReceivedEmail>> FetchInboxEmailsAsync(int maxCount)
        //{
        //    var emails = new List<ReceivedEmail>();

        //    using (var client = new ImapClient())
        //    {
        //        await client.ConnectAsync("imap.titan.email", 993, SecureSocketOptions.SslOnConnect);

        //        await client.AuthenticateAsync("Support@empiretecknologies.com", "DS&T5*lKS&^SalesMNT24");

        //        var inbox = client.Inbox;
        //        await inbox.OpenAsync(FolderAccess.ReadOnly);

        //        var uids = await inbox.SearchAsync(SearchQuery.All);
        //        var count = Math.Min(maxCount, uids.Count);

        //        for (int i = uids.Count - 1; i >= uids.Count - count; i--)
        //        {
        //            var message = await inbox.GetMessageAsync(uids[i]);

        //            emails.Add(new ReceivedEmail
        //            {
        //                Id = uids[i].Id,
        //                From = message.From.ToString(),
        //                Subject = message.Subject,
        //                Date = message.Date.DateTime,
        //                Body = message.TextBody ?? message.HtmlBody
        //            });
        //        }

        //        await client.DisconnectAsync(true);
        //    }

        //    return emails;
        //}

        public static async Task<List<ReceivedEmail>> FetchInboxEmailsAsync(int maxCount)
        {
            var emails = new ConcurrentBag<ReceivedEmail>();

            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.titan.email", 993, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync("Support@empiretecknologies.com", "DS&T5*lKS&^SalesMNT24");

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                var uids = await inbox.SearchAsync(SearchQuery.All);
                var recentUids = uids.TakeLast(Math.Min(maxCount, uids.Count)).ToList();

                var summaries = await inbox.FetchAsync(recentUids, MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope);

                await Parallel.ForEachAsync(summaries, new ParallelOptions { MaxDegreeOfParallelism = 5 },
                    async (summary, cancellationToken) =>
                    {
                        try
                        {
                            string body = null;

                            // Check if the email has a text body
                            if (summary.TextBody != null)
                            {
                                var textPart = await inbox.GetBodyPartAsync(summary.UniqueId, summary.TextBody, cancellationToken);
                                body = (textPart as TextPart)?.Text;
                            }
                            // Fallback to HTML body if no text body exists
                            else if (summary.HtmlBody != null)
                            {
                                var htmlPart = await inbox.GetBodyPartAsync(summary.UniqueId, summary.HtmlBody, cancellationToken);
                                body = (htmlPart as TextPart)?.Text;
                            }

                            emails.Add(new ReceivedEmail
                            {
                                Id = summary.UniqueId.Id,
                                From = summary.Envelope.From.ToString(),
                                Subject = summary.Envelope.Subject,
                                Date = summary.Envelope.Date.Value,
                                Body = body ?? "[No text content]"
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error fetching email {summary.UniqueId}: {ex.Message}");
                        }
                    });

                await client.DisconnectAsync(true);
            }

            return emails.ToList();
        }
    }
    public class ReceivedEmail
    {
        public uint Id { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Body { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public string ImapHost { get; set; }
        public int ImapPort { get; set; }
        public bool ImapEnableSsl { get; set; }
        public string MailFrom { get; set; }
        public string MailFromName { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; } = 30000;
        public string PickupDirectory { get; set; }
    }
}
