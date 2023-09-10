//using QRCoder;
//using SelectPdf;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Security.Cryptography;
//using System.Text;
//using Untz.Database.Models;
//using UntzEmailPdfEngine.Services.Interface;

//namespace UntzEmailPdfEngine.Services.Implementation
//{
//    public class PdfService : IPdfService
//    {
//        private readonly IConfiguration _configuration;
//        public PdfService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task<string> GenerateQrCodeAsync(string reference)
//        {
//            using MemoryStream memoryStream = new();
//            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
//            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(reference, QRCodeGenerator.ECCLevel.Q);
//            QRCode qRCode = new QRCode(qRCodeData);
//            using Bitmap bitmap = qRCode.GetGraphic(20);
//            bitmap.Save(memoryStream, ImageFormat.Jpeg);

//            return await Task.FromResult(Convert.ToBase64String(memoryStream.ToArray()));
//        }

//        public async Task<byte[]?> GenerateReciptAsync(TicketPurchase ticketPurchase, bool isAuthenticated)
//        {
//            var name = isAuthenticated ? $"{ticketPurchase.User!.FirstName} {ticketPurchase.User!.LastName}"
//                : $"{ticketPurchase.GuestUser!.FirstName} {ticketPurchase.GuestUser!.LastName}";
//            var email = isAuthenticated ? ticketPurchase.User!.Email : ticketPurchase.GuestUser!.Email;
//            var reference = ticketPurchase.Reference;
//            var category = ticketPurchase.Ticket.Name;
//            var price = ticketPurchase.Ticket.Price;
//            var location = ticketPurchase.Ticket.Event.Location;
//            var startTime = ticketPurchase.Ticket.Event.EventStartTime;
//            var count = ticketPurchase.NoOfTickets;
//            var sum = ticketPurchase.Ticket.Price * ticketPurchase.NoOfTickets;
//            var purchasedDate = ticketPurchase.CreatedTime;
//            var entrance = ticketPurchase.Ticket.Event.Entrance;

//            string content = string.Empty;

//            content += "<div style=\"width: 100%; height: 100%; margin: 0 auto; font-family: 'Open Sans', sans-serif; color: black; margin-top:10px;\">";
//            content += "<div style=\"height:40px; margin-top: 5px;\"> <h1 style=\"margin-top: 5px; left: 0;\"><b>UNTZ UNTZ</b></h1>";
//            content += "<img style=\"width: 100px; height: 100px; float: right; margin-top: -90px; right: 0; position: absolute;\" src = \"" + UntzApi.Utility.Image.GetSecondLogo() + "\" > </div>";
//            content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" >";
//            content += "<div style=\"border: 2px solid red; height: 30px; width: 183px; \"> ";
//            content += "<p style=\"font-size: 15px; color: red; margin: 7px;\" ><b>ELECTRONIC RECEIPT</b></p> </div>";
//            content += "<br>";
//            content += "<div style=\"margin-top: 15px;\" >";
//            content += "<div> <table style=\"width: 100%; border: none; border-collapse: collapse;\" >";
//            content += "<tr>";
//            content += "<th style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b> Category </b></th>";
//            content += "<th style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b> Time </b></th>";
//            content += "</tr>";
//            content += "<tr>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" > " + category.ToUpper() + " </td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" > " + startTime.ToString() + " </td>";
//            content += "</tr>";
//            content += "</table>";
//            content += "</div>";
//            content += "</div>";
//            content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" />";
//            content += "<div style=\"margin-top: 20px;\" >";
//            content += "<table style=\"width: 100%; border: none; border-collapse: collapse;\" >";
//            content += "<tr> <th style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ></th>";
//            content += "<th style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ></th> </tr>";
//            content += "<tr style=\"background-color: #FDEDEC;\" >";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>NAME</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"> " + name + " </td>";
//            content += "</tr>";
//            content += "<tr>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>REFERENCE</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\">#" + reference + "</td>";
//            content += "</tr>";
//            content += "<tr style= \"background-color: #FDEDEC;\" >";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>TYPE</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"> " + category + " </td>";
//            content += "</tr>";
//            content += "<tr>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>PRICE</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\">" + price + " </td>";
//            content += "</tr>";
//            content += "<tr style=\"background-color: #FDEDEC;\" >";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>LOCATION</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\">" + location + "</td>";
//            content += "</tr>";
//            content += "<tr>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>COUNT</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\">" + count + "</td>";
//            content += "</tr>";
//            content += "<tr style=\"background-color: #FDEDEC;\" >";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>PURCHASED</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\">" + purchasedDate + "</td>";
//            content += "</tr>";
//            content += "<tr>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>ENTRANCE</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\">" + entrance + "</td>";
//            content += "</tr>";
//            content += "<tr style=\"background-color: #FADBD8;\" >";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\"><b>TOTAL</b></td>";
//            content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\">" + sum + "</td>";
//            content += "</tr>";
//            content += "</table>";
//            content += "</div>";
//            content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" >";
//            content += @"<div style = ""margin-top: 0px;"" >";
//            content += "<p><b> Terms & Conditions </b></p>";
//            content += "<p> This Attachment is confidential.It may also be legally privileged.If you are not the addressee you may";
//            content += "not copy, forward, disclose or use any part of it.If you have received this message in error, please";
//            content += "delete it and all copies from your system and notify the sender immediately by return E-mail.";
//            content += "Internet communications cannot be guaranteed to be timely secure, error or virus-free.The sender does";
//            content += "not accept liability for any errors or omissions.</p>";
//            content += "</div>";
//            content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" >";
//            content += "</div>";

//            using (MemoryStream stream = new())
//            {
//                var htmlContent = System.String.Format(content, DateTime.Now);
//                HtmlToPdf converter = new HtmlToPdf();
//                converter.Options.MarginLeft = 35;
//                converter.Options.MarginTop = 35;
//                converter.Options.MarginRight = 35;
//                converter.Options.MarginBottom = 35;
//                PdfDocument doc = converter.ConvertHtmlString(content);
//                doc.Save(stream);
//                doc.Close();

//                return await Task.FromResult(stream.ToArray());
//            }

//        }

//        public string GenerateHash(string order_id, decimal amount, string currency)
//        {
//            var merchant_id = _configuration["merchant_id"]!;
//            var merchant_secret = ComputeMD5(_configuration["merchant_secret"]!).ToUpper();
//            var amountFormated = amount.ToString("####0.00");

//            var hash1 = ComputeMD5($"{merchant_id}{order_id}{amountFormated}{currency}{merchant_secret}");

//            return hash1;
//        }

//        private string ComputeMD5(string s)
//        {
//            StringBuilder sb = new StringBuilder();
//            using (MD5 md5 = MD5.Create())
//            {
//                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
//                foreach (byte b in hashValue)
//                {
//                    sb.Append($"{b:X2}");
//                }
//            }
//            return sb.ToString();
//        }

//        public string GenerateUniqueReference()
//        {
//            var now = DateTime.Now;
//            var zeroDate = DateTime.MinValue.AddHours(now.Hour)
//                .AddMinutes(now.Minute)
//                .AddSeconds(now.Second)
//                .AddMilliseconds(now.Millisecond);
//            return ((long)(zeroDate.Ticks / 100)).ToString();
//        }

//        public async Task<List<byte[]?>> GenerateTicketsAsync(TicketPurchase ticketPurchase, Dictionary<string, string> qrCodeList, bool isAuthenticated)
//        {
//            List<byte[]?> result = new();
//            foreach (var qrCode in qrCodeList)
//            {
//                string qrImage = "data:image/png;base64, " + qrCode.Value + "";
//                var name = isAuthenticated ? $"{ticketPurchase.User!.FirstName} {ticketPurchase.User!.LastName}"
//                    : $"{ticketPurchase.GuestUser!.FirstName} {ticketPurchase.GuestUser!.LastName}";
//                var email = isAuthenticated ? ticketPurchase.User!.Email : ticketPurchase.GuestUser!.Email;
//                var reference = qrCode.Key;
//                var category = ticketPurchase.Ticket.Name;
//                var price = ticketPurchase.Ticket.Price;
//                var location = ticketPurchase.Ticket.Event.Location;
//                var startTime = ticketPurchase.Ticket.Event.EventStartTime;
//                var purchasedDate = ticketPurchase.CreatedTime;
//                var entrance = ticketPurchase.Ticket.Event.Entrance;
//                var eventName = ticketPurchase.Ticket.Event.Name;

//                string content = string.Empty;

//                content += "<div style=\"width: 100%; height: 100%; margin: 0 auto; font-family: 'Open Sans', sans-serif; color: black; margin-top:10px;\" >";
//                content += "<div style=\"height:40px; margin-top: 5px;\">";
//                content += "<h1 style=\"margin-top: 5px; left: 0;\" ><b>UNTZ UNTZ </b> </h1>";
//                content += "<img style=\"width: 100px; height: 100px; float: right; margin-top: -90px; right: 0; position: absolute;\" src = \"" + UntzApi.Utility.Image.GetSecondLogo() + "\" >";
//                content += "</div>";
//                content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" >";
//                content += "<div>";
//                content += "<table style=\"width: 100%; border: none; border-collapse: collapse;\" >";
//                content += "<tr>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >";
//                content += "<div style=\"width: 100%; text-align: left;\" >";
//                content += "<img style=\"min-height: 330px; max-height: 330px; min-width: 330px; max-width: 330px;\" src = \"" + UntzApi.Utility.Image.GetFirstLogo() + "\" />";
//                content += "</div>";
//                content += "</td>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >";
//                content += "<div style=\"width: 100%; text-align: right;\" >";
//                content += "<img style=\"min-height: 330px; max-height: 330px; min-width: 330px; max-width: 330px;\" src = \"" + qrImage + "\" >";
//                content += "</div>";
//                content += "</td>";
//                content += "</tr>";
//                content += "</table>";
//                content += "</div>";
//                content += "<div style=\"border: 2px solid red; height: 30px; width: 173px; \">";
//                content += "<p style=\"font-size: 15px; color: red; margin: 7px;\" ><b>ELECTRONIC TICKET</b></p>";
//                content += "</div>";
//                content += "<br>";
//                content += "<div style=\"margin-top:0px;\" >";
//                content += "<p style=\"font-size: 25px; margin: 0px;\" ><b> " + eventName.ToUpper() + " </b ></p>";
//                content += "</div>";
//                content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" >";
//                content += "<div style=\"margin-top: 20px;\" >";
//                content += "<div style=\"max-width: 100%;\" >";
//                content += "<table style=\"width: 100%; border: none; border-collapse: collapse;\" >";
//                content += "<tr>";
//                content += "<th style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>Category</b></th>";
//                content += "<th style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>Time</b></th>";
//                content += "</tr>";
//                content += "<tr>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" > " + category.ToUpper() + "</td>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >" + startTime.ToString() + "</td>";
//                content += "</tr>";
//                content += "</table>";
//                content += "</div>";
//                content += "</div>";
//                content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" >";
//                content += "<div style=\"margin-top: 20px;\" >";
//                content += "<table style=\"width: 100%; border: none; border-collapse: collapse;\" >";
//                content += "<tr>";
//                content += "<th style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ></th>";
//                content += "<th style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ></th>";
//                content += "</tr>";
//                content += "<tr style=\"background-color: #FDEDEC;\" >";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>NAME</b></td>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >" + name + "</td>";
//                content += "</tr>";
//                content += "<tr>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>TICKET REFERENCE</b>";
//                content += "</td >";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >#" + reference + "</td>";
//                content += "</tr>";
//                content += "<tr style=\"background-color: #FDEDEC;\" >";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>TYPE</b></td>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" > " + category + " </td>";
//                content += "</tr>";
//                content += "<tr>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>PRICE</b></td>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >" + price + "</td>";
//                content += "</tr>";
//                content += "<tr style=\"background-color: #FDEDEC;\" >";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>LOCATION</b></td>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >" + location + "</td>";
//                content += "</tr>";
//                content += "<tr>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>PURCHASED</b></td>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >" + purchasedDate + "</td>";
//                content += "</tr>";
//                content += "<tr style=\"background-color: #FDEDEC;\" >";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" ><b>ENTRANCE</b></td>";
//                content += "<td style=\"text-align: left; padding: 6px; width: 50%; text-align: left;\" >" + entrance + "</td>";
//                content += "</tr>";
//                content += "</table>";
//                content += "</div>";
//                content += "<br>";
//                content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" >";
//                content += "<div style=\"margin-top: 0px;\" >";
//                content += "<p><b>Terms & Conditions</b></p>";
//                content += "<p> This Attachment is confidential.It may also be legally privileged. If you are not the addressee you may";
//                content += "not copy, forward, disclose or use any part of it. If you have received this message in error, please";
//                content += "delete it and all copies from your system and notify the sender immediately by return E - mail.";
//                content += "Internet communications cannot be guaranteed to be timely secure, error or virus - free.The sender does";
//                content += "not accept liability for any errors or omissions.</p>";
//                content += "</div>";
//                content += "<hr style=\"margin-top: 15px; margin-bottom: 15px; border: 1px solid;\" >";
//                content += "</div>";

//                using (MemoryStream stream = new())
//                {
//                    var htmlContent = System.String.Format(content, DateTime.Now);
//                    HtmlToPdf converter = new HtmlToPdf();
//                    converter.Options.MarginLeft = 35;
//                    converter.Options.MarginTop = 35;
//                    converter.Options.MarginRight = 35;
//                    converter.Options.MarginBottom = 35;
//                    PdfDocument doc = converter.ConvertHtmlString(content);
//                    doc.Save(stream);
//                    doc.Close();

//                    result.Add(stream.ToArray());
//                }
//            }

//            return await Task.FromResult(result);
//        }
//    }
//}
