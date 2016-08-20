using AutoMapper.QueryableExtensions;
using Data;
using DataServices;
using DesignAndPrintStickers.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DesignAndPrintStickers.Controllers
{

    public class HomeController : Controller
    {
        private readonly ITemplatesService templatesService;
        public HomeController(ITemplatesService templatesService)
        {
            this.templatesService = templatesService;
        }

        public ActionResult Index()
        {
            var model = templatesService.GetTemplates().ProjectTo<IndexViewModels>().ToList();

            return View(model);
        }


        #region Mailing functions

        public ActionResult SentEmailAjax(string Email, string NameUser, string Subject, string Body)
        {
            if (Request.IsAjaxRequest())
            {
                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("From Email : " + Email);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("User Name : " + NameUser);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Subject : " + Subject);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(Body);
                if (!SendEmail(ConfigurationManager.AppSettings["ContactEmail"], "New Email - " + DateTime.Now.ToString("dd-MM-yyyy"), sb.ToString()))
                {

                    return Json(false);
                }

                return Json(true);

            }
            return Json(false);
        }


        public Boolean SendEmail(string reciever, string subject, string body)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add(reciever);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
    }


}