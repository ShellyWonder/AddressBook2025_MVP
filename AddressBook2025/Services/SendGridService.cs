using AddressBook2025.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;
using System.Text.RegularExpressions;

namespace AddressBook2025.Services
{
    public class SendGridService(IConfiguration config) : IEmailSender, IEmailSender<ApplicationUser>
    {

        private readonly string _sendGridKey = config["SendGridKey"] ?? Environment.GetEnvironmentVariable("SendGridKey")
                ?? throw new InvalidOperationException("SendGridKey not found.");
        private readonly string _fromAddress = config["SendGridEmail"] ?? throw new InvalidOperationException("Email address not found");
        private readonly string _fromName = config["SendGridName"] ?? throw new InvalidOperationException("Email sender not found");

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SendGridClient client = new SendGridClient(_sendGridKey);
            EmailAddress from = new EmailAddress(_fromAddress, _fromName);
            string plainTextContent = Regex.Replace(htmlMessage, "<[a-zA-Z].*?>", "").Trim();
            //works for singular email or multiples
            List<string> emails = [.. email.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];
            List<EmailAddress> addresses = [.. emails.Select(e => new EmailAddress(e))];

            SendGridMessage message = MailHelper.CreateSingleEmailToMultipleRecipients(from, addresses, subject, plainTextContent, htmlMessage);
            //this snippet sends the email
            Response response = await client.SendEmailAsync(message);

            if (response.IsSuccessStatusCode == false)
            {
                Console.WriteLine("******** EMAIL SERVICE ERROR *********");
                Console.WriteLine(await response.Body.ReadAsStringAsync());
                Console.WriteLine("******** EMAIL SERVICE ERROR *********");
                throw new BadHttpRequestException("SendGrid Email could not be sent.");
            }
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
           SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
    }
}
