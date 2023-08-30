namespace UntzApi.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailWithAttachmentsAsync(string subject, string body, List<byte[]?> attachments, string to);
        Task<bool> SendEmailAsync(string subject, string body, string to);
    }
}
