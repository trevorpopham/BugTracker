using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace Bug_Tracker.Helpers
{
    public class SendEmail
    {
        public static void Send(MailMessage mailMessage)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.UseDefaultCredentials = false;
                var credential = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["username"],
                    Password = WebConfigurationManager.AppSettings["password"]
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 25;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Send(mailMessage);
            }
        }
    }
}