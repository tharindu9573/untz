using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using Untz.Database;
using UntzCommon.Database.Models;
using UntzCommon.Models.Dtos;

namespace UntzApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseAcknowledgementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly UntzDbContext _dbContext;
        public PurchaseAcknowledgementController(IServiceProvider serviceProvider, IConfiguration configuration, IMapper mapper, UntzDbContext untzDbContext)
        {
            _configuration = configuration;
            _mapper = mapper;
            _dbContext = untzDbContext;
            _serviceProvider = serviceProvider;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] PaymentAcknowledgement paymentAcknowledgement)
        {
            try
            {
                var paymentStatus = _mapper.Map<PaymentStatus>(paymentAcknowledgement);

                paymentStatus.TicketPurchaseId = paymentAcknowledgement.custom_2;
                await _dbContext.PaymentStatuses.AddAsync(paymentStatus);
                await _dbContext.SaveChangesAsync();

                new Thread(() =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var jsonString = JsonConvert.SerializeObject(new { TicketPurchasedId = paymentAcknowledgement.custom_2, IsAuthenticated = paymentAcknowledgement.custom_1 });
                        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/Json");
                        var uri = new Uri($"https://untz-email-pdf-service.azurewebsites.net/EmailPdfs");

                        client.Send(new HttpRequestMessage()
                        {
                            RequestUri = uri,
                            Content = httpContent,
                            Method = HttpMethod.Post
                        });
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@untzuntz.ae");

            message.To.Add(new MailAddress("tharindu9573@gmail.com"));

            message.Subject = "your subject";
            message.Body = "content of your email";

            SmtpClient client = new SmtpClient();
            client.Host = _configuration["email_host"]!;
            client.Port = Convert.ToInt32(_configuration["email_port"]!);
            client.EnableSsl = Convert.ToBoolean(_configuration["email_ssl_enabled"]!);
            client.Send(message);

            return Ok();
        }

        private void LogError(Exception ex)
        {
            System.IO.File.AppendAllLines($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", new List<string> { string.Empty, string.Empty });
            System.IO.File.AppendAllText($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", $"START===={DateTime.Now}====");
            System.IO.File.AppendAllLines($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", new List<string> { string.Empty, string.Empty });
            System.IO.File.AppendAllText($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", ex.StackTrace);
            System.IO.File.AppendAllLines($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", new List<string> { string.Empty, string.Empty });
            System.IO.File.AppendAllText($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", ex.Message);
            if (ex.InnerException is not null)
            {
                System.IO.File.AppendAllLines($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", new List<string> { string.Empty, string.Empty });
                System.IO.File.AppendAllText($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", "INNER EXCEPTION");
                System.IO.File.AppendAllText($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", ex.StackTrace);
                System.IO.File.AppendAllLines($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", new List<string> { string.Empty, string.Empty });
                System.IO.File.AppendAllText($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", ex.Message);
            }
            System.IO.File.AppendAllLines($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", new List<string> { string.Empty, string.Empty });
            System.IO.File.AppendAllText($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", $"END========");
            System.IO.File.AppendAllLines($"{Directory.GetCurrentDirectory()}/App_Data/logs.txt", new List<string> { string.Empty, string.Empty });
        }
    }
}
