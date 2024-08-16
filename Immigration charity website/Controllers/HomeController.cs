using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Immigration_charity_website.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string name, string email, string message, HttpPostedFileBase attachment)
        {
            try
            {
                // 创建邮件消息
                var mail = new MailMessage
                {
                    From = new MailAddress("zbh2926@outlook.com"),
                    Subject = "Contact Form Submission",
                    Body = $"Name: {name}\nEmail: {email}\nMessage: {message}",
                    IsBodyHtml = false
                };

                // 收件人
                mail.To.Add(new MailAddress("zbh2926@outlook.com"));

                // 如果有附件，添加到邮件
                if (attachment != null && attachment.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(attachment.FileName);
                    mail.Attachments.Add(new Attachment(attachment.InputStream, fileName));
                }

                // 配置SMTP客户端
                using (var smtp = new SmtpClient("smtp-mail.outlook.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential("zbh2926@outlook.com", "26zbh999999999");
                    smtp.Send(mail);
                }

                // 成功信息
                TempData["Message"] = "Your message has been sent successfully!";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                // 处理错误并显示
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Contact");
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Contact Us";
            ViewBag.Message = "Get in touch with us.";
            return View();
        }
    }
}