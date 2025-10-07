using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string to, string subject, string body, byte[] pdfBytes = null, string pdfFileName = "ShipmentDetails.pdf")
    {
        var settings = _config.GetSection("EmailSettings");
        var smtp = new SmtpClient(settings["SmtpServer"], int.Parse(settings["SmtpPort"]))
        {
            Credentials = new NetworkCredential(settings["SmtpUser"], settings["SmtpPass"]),
            EnableSsl = true
        };

        var mail = new MailMessage(settings["From"], to, subject, body);
        if (pdfBytes != null)
        {
            mail.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "ShipmentDetails.pdf", "application/pdf"));
        }
        await smtp.SendMailAsync(mail);
    }
}
