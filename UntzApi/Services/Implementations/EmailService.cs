using System.Net;
using System.Net.Mail;
using UntzApi.Services.Interfaces;

namespace UntzApi.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string subject, string body, string to)
        {
            SmtpClient client = new SmtpClient();
            client.Host = _configuration["email_host"]!;
            client.Port = Convert.ToInt32(_configuration["email_port"]!);
            client.EnableSsl = Convert.ToBoolean(_configuration["email_ssl_enabled"]!);

            MailMessage mailMessage = new();
            mailMessage.Body = body;
            mailMessage.Subject = subject;
            mailMessage.From = new MailAddress(_configuration["email_user_name"]!);

            mailMessage.To.Add(new MailAddress(to));

            await client.SendMailAsync(mailMessage);
            return true;
        }

        public async Task<bool> SendEmailWithAttachmentsAsync(string subject, string body, List<byte[]?> attachments, string to)
        {
            SmtpClient client = new SmtpClient();
            client.Host = _configuration["email_host"]!;
            client.Port = Convert.ToInt32(_configuration["email_port"]!);
            client.EnableSsl = Convert.ToBoolean(_configuration["email_ssl_enabled"]!);

            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
            contentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Octet;
            contentType.Name = "UntzEventRecipt.pdf";

            MailMessage mailMessage = new();
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.From = new MailAddress(_configuration["email_user_name"]!);
            if (attachments is not null)
            {
                attachments.ForEach(_ => mailMessage.Attachments.Add(new Attachment(new MemoryStream(_!), contentType)));
            }

            mailMessage.To.Add(new MailAddress(to));

            await client.SendMailAsync(mailMessage);
            return true;
        }
    }
}
