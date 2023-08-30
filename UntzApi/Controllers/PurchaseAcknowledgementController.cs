using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Untz.Database;
using Untz.Endpoints.Dtos;
using UntzApi.Database.Models;
using UntzApi.Services.Interfaces;

namespace UntzApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseAcknowledgementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly UntzDbContext _dbContext;
        public PurchaseAcknowledgementController(IConfiguration configuration, IPdfService pdfService, IEmailService emailService, IMapper mapper, UntzDbContext untzDbContext)
        {
            _configuration = configuration;
            _pdfService = pdfService;
            _emailService = emailService;
            _mapper = mapper;
            _dbContext = untzDbContext;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] PaymentAcknowledgement paymentAcknowledgement)
        {

            var paymentStatus = _mapper.Map<PaymentStatus>(paymentAcknowledgement);

            var ticketPurchase = _dbContext.TicketPurchases
                .Include(_ => _.User)
                .Include(_ => _.GuestUser)
                .Include(_ => _.Ticket)
                .ThenInclude(_ => _.Event)
                .FirstOrDefault(_ => _.Id.Equals(paymentAcknowledgement.custom_2));

            if (ticketPurchase is null)
                return NotFound();

            paymentStatus.TicketPurchaseId = ticketPurchase.Id;
            await _dbContext.PaymentStatuses.AddAsync(paymentStatus);
            await _dbContext.SaveChangesAsync();

            var noOfTickets = ticketPurchase!.NoOfTickets;

            Dictionary<string, string> qrCodeList = new();
            for (int i = 0; i < noOfTickets; i++)
            {
                var uniqueReference = _pdfService.GenerateUniqueReference();
                var qrCode = await _pdfService.GenerateQrCodeAsync($"{_configuration["host_name"]!}/ticketview/{ticketPurchase.Reference}/{uniqueReference}");
                await _dbContext.QrCodes.AddAsync(new() { Reference = uniqueReference, QrCodeImage = qrCode, TicketPurchaseId = ticketPurchase.Id });
                qrCodeList.Add(uniqueReference, qrCode);
            }

            var recipt = await _pdfService.GenerateReciptAsync(ticketPurchase, paymentAcknowledgement.custom_1);
            var base64Recipt = Convert.ToBase64String(recipt!);

            await _dbContext.Recipts.AddAsync(new() { ReciptPdf = base64Recipt, TicketPurchaseId = ticketPurchase.Id });

            await _dbContext.SaveChangesAsync();

            //Generate tickts for each QR
            var ticketPdfs = await _pdfService.GenerateTicketsAsync(ticketPurchase, qrCodeList, paymentAcknowledgement.custom_1);

            //Send mail attaching those tickets and recipt
            ticketPdfs.Add(recipt);
            var email = paymentAcknowledgement.custom_1 ? ticketPurchase.User!.Email : ticketPurchase.GuestUser!.Email;
            var emailStatus = await _emailService.SendEmailWithAttachmentsAsync("Your order was confirmed!", "Congratulations... Your seat was booked. please find the attached ticket.", ticketPdfs, email!);

            ticketPurchase.IsEmailSent = emailStatus;
            ticketPurchase.IsProcessCompleted = true;

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
