using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace SocialNetworkBe.Services.EmailServices
{
    public class EmailSender : IEmailSender
    {
        private string mailSend = "baolongabi1409@gmail.com";
        private string password = "quhuehihphqxijxz";
        private int smtpPort = 587; // Smtp Port

        private string appName = "FriCon";
        private string smtpHost = "smtp.gmail.com";
        public Task SendEmailAsync(string email, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(mailSend, appName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(email));

            SmtpClient smtpClient = new SmtpClient()
            {
                Port = smtpPort,
                EnableSsl = true,
                Host = smtpHost,
                Credentials = new NetworkCredential(mailSend, password)
            };

            return smtpClient.SendMailAsync(message);
        }
    }
}
