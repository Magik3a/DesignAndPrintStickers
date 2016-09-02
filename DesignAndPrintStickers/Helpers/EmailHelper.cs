using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace DesignAndPrintStickers.Helpers
{
    public class EmailHelper
    {
        public static async Task SendAsync(IdentityMessage message)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(mail);
            }
        }
    }
}