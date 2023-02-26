using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace ShopM4_Utility
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration configuration;
        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendEmailAsync(string emaiAdressl, string subject, string htmlMessage)
        {
            GMailSettings m = configuration.GetSection("GmailSettings").Get<GMailSettings>();
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(m.Sender));
            email.To.Add(MailboxAddress.Parse(emaiAdressl));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587);
            smtp.Authenticate(m.Sender, m.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
