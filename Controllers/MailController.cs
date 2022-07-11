using System;
using System.Threading;
using System.Net;
using System.Net.Mail;

namespace Backend.Controllers
{
    public class Mail
    {

            public static bool Send(string UserEmail, string Subject, string MailBody)
            {

                try
                {
                    SendMail("codojoDEV@gmail.com", UserEmail, MailBody, Subject);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }

            public static void SendMail(string SenderMail, string ReceiverMail, string Body, string Subject)
            {
                MailAddress to = new MailAddress(ReceiverMail);
                MailAddress from = new MailAddress(SenderMail);
                MailMessage mail = new MailMessage(from, to);

                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;

                smtp.Credentials = new NetworkCredential(
                    "codojoDEV@gmail.com", "pkxffhyhmxdduvai");
                smtp.EnableSsl = true;
                smtp.Send(mail);

        }
        
    }
}
