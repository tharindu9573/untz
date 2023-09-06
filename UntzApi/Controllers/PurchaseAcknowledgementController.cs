using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using Untz.Database;
using Untz.Endpoints.Dtos;
using UntzApi.Database.Models;
using UntzApi.Services.Interfaces;
using System;

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
            var customerName = paymentAcknowledgement.custom_1 ? ticketPurchase.User!.FirstName : ticketPurchase.GuestUser!.FirstName;
            var emailBody = $"Dear {customerName},\r\nThank you for purchasing your ticket(s) from untzuntz.ae\r\nYour electronic ticket(s) are attached with this email.\r\nPlease print and present your electronic ticket(s) at the venue.\r\nIn order to view and print your electronic ticket(s) now or at any time, you will need Adobe Acrobat Reader. If you do not have Adobe Acrobat Reader installed visit: http://www.adobe.com for a free copy.\r\n\r\nTo contact us, email: info@untzuntz.ae\r\nThis E-mail is confidential. It may also be legally privileged. If you are not the addressee you may not copy, forward, disclose or use any part of it. If you have received this message in error, please delete it and all copies from your system and notify the sender immediately by return E-mail.\r\nInternet communications cannot be guaranteed to be timely secure, error or virus-free. The sender does not accept liability for any errors or omissions.";
            var emailStatus = await _emailService.SendEmailWithAttachmentsAsync("Your order was confirmed!", emailBody, ticketPdfs, email!, ticketPurchase.Ticket.Event.Name);

            ticketPurchase.IsEmailSent = emailStatus;
            ticketPurchase.IsProcessCompleted = true;

            await _dbContext.SaveChangesAsync();

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
    }
}
