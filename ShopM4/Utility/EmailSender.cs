using Mailjet.Client.TransactionalEmails;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using Mailjet.Client.Resources;

namespace ShopM4.Utility
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration configuration;
        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }
        public async Task Execute(string email, string subject, string htmlMessage)
        {
            MailJetSettings m = configuration.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(m.ApiKey, m.SecretKey);
            
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource
            };

            var emailMessage = new TransactionalEmailBuilder().
                WithFrom(new SendContact("viosagmir@gmail.com")).
                WithSubject(subject).
                WithHtmlPart(htmlMessage).
                WithTo(new SendContact(email)).
                Build();

            var response = await client.SendTransactionalEmailAsync(emailMessage);
        }
    }

}
