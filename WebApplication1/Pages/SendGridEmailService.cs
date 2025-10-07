using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

public class SendGridEmailService
{
    private readonly string _apiKey;
    private readonly string _from;
    public SendGridEmailService(IConfiguration config)
    {
        _apiKey = config["SendGrid:ApiKey"];
        _from = config["SendGrid:From"];
    }

    public async Task SendEmailAsync(string to, string subject, string body, byte[] pdfBytes = null, string pdfFileName = "ShipmentDetails.pdf")
    {
        var client = new SendGridClient(_apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_from),
            Subject = subject,
            PlainTextContent = body
        };
        msg.AddTo(new EmailAddress(to));

        if (pdfBytes != null)
        {
            string base64Pdf = Convert.ToBase64String(pdfBytes);
            msg.AddAttachment(pdfFileName, base64Pdf, "application/pdf");
        }

        await client.SendEmailAsync(msg);
    }
}
