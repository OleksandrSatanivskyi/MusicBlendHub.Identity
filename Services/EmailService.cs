using System.Net;
using System.Net.Mail;

namespace MusicBlendHub.Identity.Services
{
    public class EmailService
    {
        private SmtpClient SmtpClient { get; set; }
        private MailAddress FromAddress { get; set; }

        public EmailService()
        {
            FromAddress = new MailAddress("cprog3321@gmail.com", "Pastebin.Identity");

            SmtpClient = new SmtpClient();
            SmtpClient.Host = "smtp.gmail.com";
            SmtpClient.Port = 587;
            SmtpClient.EnableSsl = true;
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpClient.UseDefaultCredentials = false;
            SmtpClient.Credentials = new NetworkCredential(FromAddress.Address, "hlaavdzwszaaohmh");
        }

        public async Task SendEmailAsync(string toEmail, string confirmationLink)
        {
            string emailBody = $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title style=""color: black;"">Pastebin Registration Confirmed</title>
            </head>
            <body>
                <h1 style=""color: black;"">Registration Confirmed on Pastebin</h1>
                <p style=""color: black;"">Thank you for registering on Pastebin! Your account has been created.</p>
                <p style=""color: black;"">To confirm your registration, please click the button below:</p>
                <a href=""{confirmationLink}"" style=""display: inline-block; padding: 10px 20px; background-color: #007BFF; color: #fff; text-decoration: none;"">Confirm Registration</a>
                <p style=""color: black;"">If you didn't request this registration, you can safely ignore this email.</p>
                <p style=""margin-top: 20px; margin-bottom: 20px; color: black;"">Best regards,<br>The Pastebin Team</p>
            </body>
            </html>";

            var mailMessage = new MailMessage
            {
                From = FromAddress,
                Subject = "Pastebin Registration Confirmation",
                Body = emailBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            SmtpClient.Send(mailMessage);
        }
    }
}
