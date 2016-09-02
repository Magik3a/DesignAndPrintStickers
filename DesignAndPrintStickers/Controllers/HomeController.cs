using AutoMapper.QueryableExtensions;
using Data;
using DataServices;
using DesignAndPrintStickers.Helpers;
using DesignAndPrintStickers.Models;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DesignAndPrintStickers.Controllers
{

    public class HomeController : Controller
    {
        private readonly ITemplatesService templatesService;
        private readonly IPaperSizesService paperSizesService;
        public HomeController(ITemplatesService templatesService, IPaperSizesService paperSizesService)
        {
            this.templatesService = templatesService;
            this.paperSizesService = paperSizesService;
        }

        public ActionResult Index()
        {
            var model = templatesService.GetTemplates().ProjectTo<IndexViewModels>().ToList();

            return View(model);
        }

        public ActionResult GetAddImagesModal(string TemplateName, string PaperSize)
        {
            // TODO: Simplify this with ICustomMapping for model 

            var model = new AddImagesModalPartialViewModel();

            var template = templatesService.GetTemplateByName(TemplateName).FirstOrDefault();
            var paperSize = paperSizesService.GetPaperSizeByName(PaperSize).FirstOrDefault();

            model.PaperSizeName = paperSize.Name;
            model.PaperSizeHeight = paperSize.Height;
            model.PaperSizeWith = paperSize.WIdth;
            model.TemplateClass = template.CssClass;
            model.BoxesCount = template.BoxCount;
            model.TemplateName = TemplateName;
            model.BoxHeight = template.BoxHeight;
            model.BoxWidth = template.BoxWidth;
            model.BorderRadiusPercent = template.BorderRadiusPercent;
            return PartialView("ModalTemplates/AddImagesModal", model);
        }


        [HttpPost]
        public virtual ActionResult CropImage(
     string imagePath,
     int? cropPointX,
     int? cropPointY,
     int? imageCropWidth,
     int? imageCropHeight,
     string templateName)
        {
            if (string.IsNullOrEmpty(imagePath)
                || !cropPointX.HasValue
                || !cropPointY.HasValue
                || !imageCropWidth.HasValue
                || !imageCropHeight.HasValue)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var borderRadius = templatesService.GetTemplateByName(templateName).FirstOrDefault().BorderRadiusPercent;

            byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath(imagePath));
            byte[] croppedImage = ImageHelper.CropImage(imageBytes, cropPointX.Value, cropPointY.Value, imageCropWidth.Value, imageCropHeight.Value);

            if (borderRadius > 50)
            {
                croppedImage = ImageHelper.RoundCornersImage(croppedImage, borderRadius);
            }
            else
            {
                croppedImage = ImageHelper.ImageWithBorderRadius(croppedImage, borderRadius);
            }
            string fileName = Path.GetFileName(imagePath);

            try
            {
                FileHelper.SaveFile(croppedImage, Server.MapPath(imagePath));
            }
            catch (Exception ex)
            {
                //Log an error     
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            string photoPath = string.Concat("/", fileName);
            return Json(new { photoPath = imagePath }, JsonRequestBehavior.AllowGet);
        }


        #region Mailing functions

        public ActionResult SentEmailAjax(string Email, string NameUser, string Subject, string Body)
        {
            if (Request.IsAjaxRequest())
            {
                var sb = new StringBuilder();
                sb.AppendLine("<br />");
                sb.AppendLine("From Email : " + Email);
                sb.AppendLine("<br />");
                sb.AppendLine("----------------------------------");
                sb.AppendLine("<br />");
                sb.AppendLine("User Name : " + NameUser);
                sb.AppendLine("<br />");
                sb.AppendLine("----------------------------------");
                sb.AppendLine("<br />");
                sb.AppendLine("Subject : " + Subject);
                sb.AppendLine("<br />");
                sb.AppendLine("----------------------------------");
                sb.AppendLine("<br />");
                sb.AppendLine("Message : " + Body);
                if (!SendEmail(ConfigurationManager.AppSettings["ContactEmail"], "New Email - " + DateTime.Now.ToString("dd-MM-yyyy mm-ss"), sb.ToString()))
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

        public static Boolean SendEmail(string reciever, string subject, string body, byte[] FileByte, string NameAtachemnt)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add(reciever);
                mail.Subject = subject;
                mail.Body = body;
                Attachment file = new Attachment(new MemoryStream(FileByte), NameAtachemnt);
                mail.Attachments.Add(file);
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

        #region Download

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DownloadStickers(string html, string pagesize, string templateName)
        {
            if (String.IsNullOrWhiteSpace(pagesize))
                return Json(false);
            try
            {
                var template = templatesService.GetTemplateByName(templateName).FirstOrDefault();
                byte[] bytes = GenerateImagesPDF(
                    html,
                    pagesize,
                    template.BoxesPerRow,
                    float.Parse(template.MarginTop),
                    float.Parse(template.MarginBottom),
                    float.Parse(template.MarginLeft), float.Parse(template.MarginRIght));

                // Generate a new unique identifier against which the file can be stored
                string handle = Guid.NewGuid().ToString();
                Session[handle] = bytes.ToArray();

                //return File(bytes, "application/pdf", DateTime.Now.ToString(CultureInfo.InvariantCulture));

                // Note we are returning a filename as well as the handle
                return new JsonResult()
                {
                    Data = new { fileGuid = handle }
                };
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid)
        {
            if (Session[fileGuid] != null)
            {
                byte[] data = Session[fileGuid] as byte[];
                return File(data, "application/pdf", DateTime.Now.ToString(CultureInfo.InvariantCulture) + ".pdf");
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }

        #endregion

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendToEmailWithAtachmnent(string html, string pagesize, string templateName, string reciever)
        {
            if (String.IsNullOrWhiteSpace(pagesize))
                return Json(false);

            try
            {
                var template = templatesService.GetTemplateByName(templateName).FirstOrDefault();
                byte[] bytes = GenerateImagesPDF(
                    html,
                    pagesize,
                    template.BoxesPerRow,
                    float.Parse(template.MarginTop),
                    float.Parse(template.MarginBottom),
                    float.Parse(template.MarginLeft), float.Parse(template.MarginRIght));
                SendEmail(reciever, "New email with stickers", "<a href='http://app.yeppey.com' title='app.yeppey.com'> <img src='http://app.yeppey.com/Images/Logo.png' style='background: white;' /> </a>", bytes, DateTime.Now.ToString(CultureInfo.InvariantCulture) + ".pdf");
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        #region Generate PDF

        public byte[] GeneratePDF(string html, string pageSize)
        {



            #region Generate PDF
            Byte[] bytes;
            var ms = new MemoryStream();
            //Create an iTextSharp Document wich is an abstraction of a PDF but **NOT** a PDF
            var doc = new Document();
            if (pageSize == "A4")
                doc = new Document(PageSize.A4);
            else
                doc = new Document(PageSize.LETTER);
            var writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();
            doc.NewPage();
            var hDocument = new HtmlDocument()
            {
                OptionWriteEmptyNodes = true,
                OptionAutoCloseOnEnd = true
            };
            hDocument.LoadHtml(html);
            List<string> xpaths = new List<string>();
            foreach (HtmlNode node in hDocument.DocumentNode.DescendantNodes())
            {
                if (node.Name.ToLower() == "img")
                {
                    var src = node.Attributes["src"].Value.Split('?')[0];
                    src = Server.MapPath(src);
                    node.SetAttributeValue("src", src);
                    node.SetAttributeValue("style", "display: inline; float: left;");
                }
                else if (node.Name.ToLower() == "a")
                {
                    xpaths.Add(node.XPath);
                }
            }
            foreach (string xpath in xpaths)
            {
                hDocument.DocumentNode.SelectSingleNode(xpath).Remove();
            }

            var closedTags = hDocument.DocumentNode.WriteTo();
            var example_html = closedTags;
            var example_css = System.IO.File.ReadAllText(Server.MapPath("~/Content/Site.css"));
            example_css += System.IO.File.ReadAllText(Server.MapPath("~/Content/Templates.min.css"));
            var msCss = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(example_css));
            var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(example_html));
            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, msCss, Encoding.UTF8);
            doc.Close();
            bytes = ms.ToArray();

            //  var testFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "test.pdf");
            //  System.IO.File.WriteAllBytes(testFile, bytes);
            #endregion

            return bytes;
        }

        public byte[] GenerateImagesPDF(string html, string pageSize, int itemsPerRow, float MarginTop, float MarginBottom, float MarginLeft, float MarginRight)
        {



            #region Generate PDF
            Byte[] bytes;
            var ms = new MemoryStream();
            //Create an iTextSharp Document wich is an abstraction of a PDF but **NOT** a PDF
            var doc = new Document();
            if (pageSize == "A4")
                doc = new Document(PageSize.A4, MarginLeft * 2.7f, MarginRight * 2.7f - 28, MarginTop * 2.7f, MarginBottom * 2.7f - 40);
            else
                doc = new Document(PageSize.LETTER, MarginLeft * 2.7f, MarginRight * 2.7f - 28, MarginTop * 2.7f, MarginBottom * 2.7f - 40);
            var writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();
            doc.NewPage();
            var hDocument = new HtmlDocument()
            {
                OptionWriteEmptyNodes = true,
                OptionAutoCloseOnEnd = true
            };
            hDocument.LoadHtml(html);
            int counter = 0;

            var pdfTable = new PdfPTable(itemsPerRow);
            pdfTable.WidthPercentage = 100;
            foreach (HtmlNode node in hDocument.DocumentNode.SelectNodes("//div"))
            {
                counter++;
                var cell = new PdfPCell();
                cell.PaddingRight = 28;

                cell.PaddingBottom = 28;

                cell.Border = 0;
                var lastChild = node.LastChild;
                if (lastChild.Name == "img")
                {
                    var src = node.LastChild.Attributes["src"].Value.Split('?')[0];
                    src = Server.MapPath(src);
                    Image jpg = Image.GetInstance(src);

                    cell.AddElement(jpg);
                }
                else
                {
                    cell.AddElement(new Chunk());
                }

                //if (itemsPerRow == 2)
                //    jpg.ScaleAbsolute(350 / itemsPerRow, 70f);
                //else if (itemsPerRow == 4)
                //    jpg.ScaleAbsolute(350 / itemsPerRow, 115f);


                // cell.Padding = 28;

                pdfTable.AddCell(new PdfPCell(cell));

            }

            doc.Add(pdfTable);
            var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));
            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, Encoding.UTF8);

            doc.Close();
            bytes = ms.ToArray();

            //  var testFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "test.pdf");
            //  System.IO.File.WriteAllBytes(testFile, bytes);
            #endregion

            return bytes;
        }



        #endregion



        public JsonResult CheckIfUserIsLogged()
        {
            if (User.Identity.IsAuthenticated)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLoginForm()
        {
            return PartialView("LoginPartial", new LoginViewModel());
        }
        public ActionResult GetRegisterForm()
        {
            return PartialView("RegisterPartial", new RegisterViewModel());
        }
    }


}