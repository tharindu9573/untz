using PdfSharpCore;
using PdfSharpCore.Pdf;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using Untz.Database.Models;
using UntzApi.Services.Interfaces;

namespace UntzApi.Services.Implementations
{
    public class PdfService : IPdfService
    {
        private readonly IConfiguration _configuration;
        public PdfService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateQrCodeAsync(string reference)
        {
            using MemoryStream memoryStream = new();
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(reference, QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(qRCodeData);
            using Bitmap bitmap = qRCode.GetGraphic(20);
            bitmap.Save(memoryStream, ImageFormat.Jpeg);

            return await Task.FromResult(Convert.ToBase64String(memoryStream.ToArray()));
        }

        public async Task<byte[]?> GenerateReciptAsync(TicketPurchase ticketPurchase, bool isAuthenticated)
        {
            var document = new PdfDocument();
            var name = isAuthenticated ? $"{ticketPurchase.User!.FirstName} {ticketPurchase.User!.LastName}"
                : $"{ticketPurchase.GuestUser!.FirstName} {ticketPurchase.GuestUser!.LastName}";
            var email = isAuthenticated ? ticketPurchase.User!.Email : ticketPurchase.GuestUser!.Email;
            var reference = ticketPurchase.Reference;
            var category = ticketPurchase.Ticket.Name;
            var price = ticketPurchase.Ticket.Price;
            var location = ticketPurchase.Ticket.Event.Location;
            var startTime = ticketPurchase.Ticket.Event.EventStartTime;
            var count = ticketPurchase.NoOfTickets;
            var sum = ticketPurchase.Ticket.Price * ticketPurchase.NoOfTickets;
            var purchasedDate = ticketPurchase.CreatedTime;
            var entrance = ticketPurchase.Ticket.Event.Entrance;

            string content = string.Empty;
            content += "<div style=\"width: 100%; margin: 0 auto; height: auto;font-family: 'Open Sans', sans-serif; color: black;\">";
            content += "<div style=\"padding-left: 0px; width: 100%;\">";
            content += "<p style=\"font-size: 30px; margin: 0px;\"><b>UNTZ UNTZ</b></p>";
            content += "</div>";
            content += "<hr style=\"margin-bottom: 10px;\">";
            content += "<div>";
            content += "<p style=\"font-size: 15px; margin-top: 15px;\"><b><u>ELECTRONIC RECIPT</u></b></p>";
            content += "</div>";
            content += "<br>";
            content += "<div style=\"margin-top: 20px;\">";
            content += "<div style=\"max-width: 100%;\">";
            content += "<table style=\"width: 100%;\">";
            content += "<tr>";
            content += "<th style=\"width: 50%; text-align: left\"><b>Category</b></th>";
            content += "<th style=\"width: 50%; text-align: left\"><b>Time</b></th>";
            content += "</tr>";
            content += "<tr>";
            content += "<td style=\"width: 50%; text-align: left\">" + category + "</td>";
            content += "<td style=\"width: 50%; text-align: left\">" + startTime.ToString("MM-DD-YYYY: HH:mm") + "</td>";
            content += "</tr>";
            content += "</table>";
            content += "</div>";
            content += "</div>";
            content += "<div style=\"margin-top: 20px;\">";
            content += "<table style=\"width: 100%;\">";
            content += "<tr>";
            content += "<td style=\"width: 100%;\">";
            content += "<div>";
            content += "<table style=\"width: 100%;\">";
            content += "<tr>";
            content += "<th style=\"width: 50%;\"></th>";
            content += "<th style=\"width: 50%;\"></th>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b>NAME</b></td>";
            content += "<td>" + name + "</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b>REFERENCE</b></td>";
            content += "<td>#" + reference + "</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b>TYPE</b></td>";
            content += "<td>" + category + "</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b>PRICE</b></td>";
            content += "<td>" + price + "</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b>LOCATION</b></td>";
            content += "<td>" + location + "</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b>COUNT</b></td>";
            content += "<td>" + count + "</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b>PURCHASED</b></td>";
            content += "<td>" + purchasedDate + "</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b>ENTRANCE</b></td>";
            content += "<td>" + entrance + "</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><b styles='font-weight:900;'>TOTAL</b></td>";
            content += "<td>" + sum + "</td>";
            content += "</tr>";
            content += "</table>";
            content += "</div>";
            content += "</td>";
            content += "</tr>";
            content += "</table>";
            content += "</div>";
            content += "</div>";

            PdfGenerator.AddPdfPages(document, content, PageSize.A4);

            byte[]? response = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                document.Save(memoryStream);
                response = memoryStream.ToArray();
            }

            return await Task.FromResult(response);
        }

        public string GenerateHash(string order_id, decimal amount, string currency)
        {
            var merchant_id = _configuration["merchant_id"]!;
            var merchant_secret = ComputeMD5(_configuration["merchant_secret"]!).ToUpper();
            var amountFormated = amount.ToString("####0.00");

            var hash1 = ComputeMD5($"{merchant_id}{order_id}{amountFormated}{currency}{merchant_secret}");
            var hash2 = ComputeMD5(merchant_id + order_id + amountFormated + currency + merchant_secret);

            return hash1;
        }

        private string ComputeMD5(string s)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                foreach (byte b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }
            return sb.ToString();
        }

        public string GenerateUniqueReference()
        {
            var now = DateTime.Now;
            var zeroDate = DateTime.MinValue.AddHours(now.Hour)
                .AddMinutes(now.Minute)
                .AddSeconds(now.Second)
                .AddMilliseconds(now.Millisecond);
            return ((long)(zeroDate.Ticks / 100)).ToString();
        }

        public async Task<List<byte[]?>> GenerateTicketsAsync(TicketPurchase ticketPurchase, Dictionary<string, string> qrCodeList, bool isAuthenticated)
        {
            List<byte[]?> result = new();
            foreach (var qrCode in qrCodeList)
            {
                var document = new PdfDocument();
                string qrImage = "data:image/png;base64, " + qrCode.Value + "";
                var name = isAuthenticated ? $"{ticketPurchase.User!.FirstName} {ticketPurchase.User!.LastName}"
                    : $"{ticketPurchase.GuestUser!.FirstName} {ticketPurchase.GuestUser!.LastName}";
                var email = isAuthenticated ? ticketPurchase.User!.Email : ticketPurchase.GuestUser!.Email;
                var reference = qrCode.Key;
                var category = ticketPurchase.Ticket.Name;
                var price = ticketPurchase.Ticket.Price;
                var location = ticketPurchase.Ticket.Event.Location;
                var startTime = ticketPurchase.Ticket.Event.EventStartTime;
                var purchasedDate = ticketPurchase.CreatedTime;
                var entrance = ticketPurchase.Ticket.Event.Entrance;
                var eventName = ticketPurchase.Ticket.Event.Name;

                string content = string.Empty;
                content += "<div style=\"width: 100%; margin: 0 auto; height: auto;font-family: 'Open Sans', sans-serif; color: black;\">";
                content += "<div style=\"padding-left: 0px; width: 100%;\">";
                content += "<p style=\"font-size: 30px; margin: 0px;\"><b>UNTZ UNTZ</b></p>";
                content += "</div>";
                content += "<hr style=\"margin-bottom: 10px;\">";
                content += "<div>";
                content += "<table style=\"width: 100%;\">";
                content += "<tr>";
                content += "<td style=\"width: 50%;\">";
                content += "<div style=\"width: 100%;\">";
                content += "</div>";
                content += "</td>";
                content += "<td style=\"width: 50%;\">";
                content += "<div style=\"width: 100%; text-align: right;\">";
                content += "<img style=\"width: 90%; min-height: 350px; min-width:350px;\" src=\"" + qrImage + "\">";
                content += "</div>";
                content += "</td>";
                content += "</tr>";
                content += "</table>";
                content += "</div>";
                content += "<div>";
                content += "<p style=\"font-size: 15px; margin-top: 15px;\"><b>ELECTRONIC TICKET</b></p>";
                content += "</div>";
                content += "<br>";
                content += "<div style=\"margin-top:0px;\">";
                content += "<p style=\"font-size: 25px; margin: 0px;\"><b>" + eventName + "</b></p>";
                content += "</div>";
                content += "<hr>";
                content += "<div style=\"margin-top: 20px;\">";
                content += "<div style=\"max-width: 100%;\">";
                content += "<table style=\"width: 100%;\">";
                content += "<tr>";
                content += "<th style=\"width: 50%; text-align: left\"><b>Category</b></th>";
                content += "<th style=\"width: 50%; text-align: left\"><b>Time</b></th>";
                content += "</tr>";
                content += "<tr>";
                content += "<td style=\"width: 50%; text-align: left\">" + category + "</td>";
                content += "<td style=\"width: 50%; text-align: left\">" + startTime.ToString() + "</td>";
                content += "</tr>";
                content += "</table>";
                content += "</div>";
                content += "</div>";
                content += "<div style=\"margin-top: 20px;\">";
                content += "<table style=\"width: 100%;\">";
                content += "<tr>";
                content += "<td style=\"width: 100%;\">";
                content += "<div>";
                content += "<table style=\"width: 100%;\">";
                content += "<tr>";
                content += "<th style=\"width: 50%;\"></th>";
                content += "<th style=\"width: 50%;\"></th>";
                content += "</tr>";
                content += "<tr>";
                content += "<td><b>NAME</b></td>";
                content += "<td>" + name + "</td>";
                content += "</tr>";
                content += "<tr>";
                content += "<td><b>TICKET REFERENCE</b></td>";
                content += "<td>#" + reference + "</td>";
                content += "</tr>";
                content += "<tr>";
                content += "<td><b>TYPE</b></td>";
                content += "<td>" + category + "</td>";
                content += "</tr>";
                content += "<tr>";
                content += "<td><b>PRICE</b></td>";
                content += "<td>" + price + "</td>";
                content += "</tr>";
                content += "<tr>";
                content += "<td><b>LOCATION</b></td>";
                content += "<td>" + location + "</td>";
                content += "</tr>";
                content += "<tr>";
                content += "<td><b>PURCHASED</b></td>";
                content += "<td>" + purchasedDate + "</td>";
                content += "</tr>";
                content += "<tr>";
                content += "<td><b>ENTRANCE</b></td>";
                content += "<td>" + entrance + "</td>";
                content += "</tr>";
                content += "</table>";
                content += "</div>";
                content += "</td>";
                content += "</tr>";
                content += "</table>";
                content += "</div>";
                content += "<br>";
                content += "<div style=\"margin-top: 0px;\">";
                content += "<p><b>Terms & Conditions</b></p>";
                content += "<p>Terms and Conditions agreements serve as a legally binding contract between you (the business) who manages a mobile application or a website and the customer who uses your app or visits your website. ";
                content += "They are otherwise called Terms of Use or Terms of Service agreements and are shortened as T&Cs (or ToU and ToS, respectively).</p>";
                content += "</div>";
                content += "</div>";

                PdfGenerator.AddPdfPages(document, content, PageSize.A4);

                byte[]? response = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    document.Save(memoryStream);
                    response = memoryStream.ToArray();
                }

                result.Add(response);
            }

            return await Task.FromResult(result);
        }
    }
}
