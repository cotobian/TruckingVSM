using System;
using System.Net.Mail;

namespace TruckingVSM.Common
{
    public class Email
    {
        
        public void SendEmail(string sender, string senderPass, string recipient, string subject, string body, string attachFile)
        {
            try
            {
                MailMessage mail = new MailMessage();
                string mailserver = "";
                if (sender.Contains("viconshipdng"))
                {
                    mailserver = "mail.viconshipdng.com.vn";
                }
                else mailserver = "mail.viconship.com";
                SmtpClient SmtpServer = new SmtpClient(mailserver);
                mail.From = new MailAddress(sender);
                mail.To.Add(sender);
                mail.To.Add(recipient);
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = body + @"<br>
                <p>Thanks & Best Regards,</p>
                <p><i>This email is automatically created!</i></p>
                <p><b>BIZ Department</b></p>
                <p style=""color:green;font-weight:bold;"">VICONSHIP DA NANG</p>
                <p style=""color:blue;""><i>A member of VICONSHIP</i></p>
                <p style=""color:blue;""><i><u>http://viconshipdanang.com</u> | Tel: (84-236) 3822 922</i></p>
                </body></html>";
                Attachment attachment;
                if(attachFile.Length > 0)
                {
                    attachment = new Attachment(attachFile);
                    mail.Attachments.Add(attachment);
                }
                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential(sender, senderPass);
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogError(ex);
            }
        }
    }
}