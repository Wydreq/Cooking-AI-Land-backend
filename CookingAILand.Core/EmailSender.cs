using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CookingAILand.Core;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("cookingailand@gmail.com", "tqua psrg xdyz mfyz")
        };

        return client.SendMailAsync(
            new MailMessage(from: "cookingailand@gmail.com",
                to: email,
                "Password reset",
                "test"
            ));
    }
}