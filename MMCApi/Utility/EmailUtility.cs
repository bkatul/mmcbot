using System.Net;
using System.Net.Mail;

namespace MMCApi.Utility
{
    public class EmailUtility
    {
        public bool SendInvitationOnEmail(string email, string password, string mailFrom, string mailTo, string Password, string bodyNew)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(mailFrom, Password);
                string link = "http://mmcbot7.azurewebsites.net/User/ChangePassword/?email=" + email;
                // string html = "<html><body><p>Click <a href=\"" + link + "\"> here</a> for change password.</p></body></html>";
                string html = "<html><body><p style='font-family:Calibri Light;font-size:16px;'>Welcome to MMC Bot mobile app.</p></br><p style='font-family:Calibri Light;font-size:16px;'>To start, please click <a href=\"" + link + "\"> here</a> and set your password.</p></body></html>";

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(mailFrom),
                    Subject = "MMC Bot Mobile App Invitation",
                    //   Body = "Please use this password '" + password + "'  to do sign in on MMC Bot for emailId '" + email + " '"
                    Body = bodyNew
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(mailTo));
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    Credentials = credentials,
                    EnableSsl = true,
                };
                client.Send(mail);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public bool SendForgotPasswordEmail(string mailFrom, string mailTo, string Password, string bodyNew)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(mailFrom, Password);
                string link = "http://mmcbot7.azurewebsites.net/User/ChangePassword/?email=" + mailTo;
                // string html = "<html><body><p>Click <a href=\"" + link + "\"> here</a> for change password.</p></body></html>";
                string html = "<html><body><p style='font-family:Calibri Light;font-size:16px;'>You recently requested to reset your password for MMC Bot mobile app. Click  <a href=\"" + link + "\"> here</a> to reset it.</p><p style='font-family:Calibri Light;font-size:16px;'>Thank you.</p></body></html>";
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(mailFrom),
                    Subject = "Reset your password – MMC Bot",
                    //   Body = "Please use this password '" + password + "'  to do sign in on MMC Bot for emailId '" + email + " '"
                    Body = bodyNew
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(mailTo));
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    Credentials = credentials,
                    EnableSsl = true,
                };
                client.Send(mail);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public bool SendUploadReceiptEmail(string mailFrom, string mailTo, string Password, string ReceiptName, string UploadBy, string bodyNew)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(mailFrom, Password);

                string html = "<html><body><p style='font-family:Calibri Light;font-size:16px;'>A new receipt is uploaded for processing, please review and approve.</p><p style='font-family:Calibri Light;font-size:16px;'>Vendor Name: " + ReceiptName + "</p><p style='font-family:Calibri Light;font-size:16px;'>Thank you.</p></body></html>";
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(mailFrom),
                    Subject = "Receipt Uploaded – " + ReceiptName,
                    Body = bodyNew
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(mailTo));
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    Credentials = credentials,
                    EnableSsl = true,
                };
                client.Send(mail);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

    }
}
