using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;

// Add this method to your EmailService or a new SmsService class
public class SmsService
{
    private readonly IConfiguration _config;
    public SmsService(IConfiguration config) => _config = config;

    public Task SendSmsAsync(string to, string message)
    {
        var twilioSection = _config.GetSection("Twilio");
        var accountSid = twilioSection["AccountSid"];
        var authToken = twilioSection["AuthToken"];
        var fromNumber = twilioSection["FromNumber"];

        TwilioClient.Init(accountSid, authToken);

        return MessageResource.CreateAsync(
            body: message,
            from: new Twilio.Types.PhoneNumber(fromNumber),
            to: new Twilio.Types.PhoneNumber(to)
        );
    }
}
