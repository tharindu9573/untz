using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Untz.Database;
using UntzApi.Services.Interfaces;
using UntzEmailPdfEngine.Models;

namespace UntzEmailPdfEngine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailPdfsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailPdfsController> _logger;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly UntzDbContext _dbContext;
        public EmailPdfsController(IConfiguration configuration, ILogger<EmailPdfsController> logger, IPdfService pdfService, IEmailService emailService, IMapper mapper, UntzDbContext untzDbContext)
        {
            _logger = logger;
            _pdfService = pdfService;
            _emailService = emailService;
            _mapper = mapper;
            _dbContext = untzDbContext;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePdfAndSendEmail(SendPdfDto sendPdfDto)
        {
            var ticketPurchase = _dbContext.TicketPurchases
                                .Include(_ => _.User)
                                .Include(_ => _.GuestUser)
                                .Include(_ => _.Ticket)
                                .ThenInclude(_ => _.Event)
                                .FirstOrDefault(_ => _.Id.Equals(sendPdfDto.TicketPurchasedId));

            var noOfTickets = ticketPurchase!.NoOfTickets;

            Dictionary<string, string> qrCodeList = new();
            for (int i = 0; i < noOfTickets; i++)
            {
                var uniqueReference = _pdfService.GenerateUniqueReference();
                var qrCode = await _pdfService.GenerateQrCodeAsync($"{_configuration["host_name"]!}/ticketview/{ticketPurchase.Reference}/{uniqueReference}");
                await _dbContext.QrCodes.AddAsync(new() { Reference = uniqueReference, QrCodeImage = qrCode, TicketPurchaseId = ticketPurchase.Id });
                qrCodeList.Add(uniqueReference, qrCode);
            }

            var recipt = await _pdfService.GenerateReciptAsync(ticketPurchase, sendPdfDto.IsAuthenticated);
            var base64Recipt = Convert.ToBase64String(recipt!);

            await _dbContext.Recipts.AddAsync(new() { ReciptPdf = base64Recipt, TicketPurchaseId = ticketPurchase.Id });

            await _dbContext.SaveChangesAsync();

            //Generate tickts for each QR
            var ticketPdfs = await _pdfService.GenerateTicketsAsync(ticketPurchase, qrCodeList, sendPdfDto.IsAuthenticated);

            //Send mail attaching those tickets and recipt
            ticketPdfs.Add(recipt);
            var email = sendPdfDto.IsAuthenticated ? ticketPurchase.User!.Email : ticketPurchase.GuestUser!.Email;
            var customerName = sendPdfDto.IsAuthenticated ? ticketPurchase.User!.FirstName : ticketPurchase.GuestUser!.FirstName;
            var emailBody = $"Dear {customerName},\r\nThank you for purchasing your ticket(s) from untzuntz.ae\r\nYour electronic ticket(s) are attached with this email.\r\nPlease print and present your electronic ticket(s) at the venue.\r\nIn order to view and print your electronic ticket(s) now or at any time, you will need Adobe Acrobat Reader. If you do not have Adobe Acrobat Reader installed visit: http://www.adobe.com for a free copy.\r\n\r\nTo contact us, email: info@untzuntz.ae\r\nThis E-mail is confidential. It may also be legally privileged. If you are not the addressee you may not copy, forward, disclose or use any part of it. If you have received this message in error, please delete it and all copies from your system and notify the sender immediately by return E-mail.\r\nInternet communications cannot be guaranteed to be timely secure, error or virus-free. The sender does not accept liability for any errors or omissions.";
            var emailStatus = await _emailService.SendEmailWithAttachmentsAsync("Your order was confirmed!", emailBody, ticketPdfs, email!, ticketPurchase.Ticket.Event.Name);

            ticketPurchase.IsEmailSent = emailStatus;
            ticketPurchase.IsProcessCompleted = true;

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}