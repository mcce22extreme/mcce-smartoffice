using System.Reflection;
using MailKit.Net.Smtp;
using MailKit.Security;
using Mcce.SmartOffice.Bookings.Configs;
using Mcce.SmartOffice.Bookings.Models;
using MimeKit;
using Serilog;

namespace Mcce.SmartOffice.Bookings.Services
{
    public interface IEmailService
    {
        Task SendMail(BookingConfirmationModel model, string activationLink);
    }

    public class EmailService : IEmailService
    {
        private const string TITLE = "[MCCE-Smart-Office] Workspace ready for activation!";
        private const string PLACEHOLDER_FIRSTNAME = "{FIRSTNAME}";
        private const string PLACEHOLDER_LASTNAME = "{LASTNAME}";
        private const string PLACEHOLDER_WORKSPACENUMBER = "{WORKSPACENUMBER}";
        private const string PLACEHOLDER_STARTTIME = "{STARTTIME}";
        private const string PLACEHOLDER_ENDTIME = "{ENDTIME}";
        private const string PLACEHOLDER_LINK = "{LINK}";
        private const string PLACEHOLDER_TIMESTAMP = "{TIMESTAMP}";

        private readonly EmailConfig _emailConfig;

        public EmailService(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendMail(BookingConfirmationModel model, string activationLink)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailConfig.SenderName, _emailConfig.UserName));
                message.To.Add(new MailboxAddress($"{model.FirstName} {model.LastName}", model.Email));
                message.Subject = TITLE;

                var content = await LoadTemplate();

                content = content
                    .Replace(PLACEHOLDER_FIRSTNAME, model.FirstName)
                    .Replace(PLACEHOLDER_LASTNAME, model.LastName)
                    .Replace(PLACEHOLDER_WORKSPACENUMBER, model.WorkspaceNumber)
                    .Replace(PLACEHOLDER_STARTTIME, $"{model.StartDateTime.ToShortDateString()} {model.StartDateTime.ToShortTimeString()}")
                    .Replace(PLACEHOLDER_ENDTIME, $"{model.EndDateTime.ToShortDateString()} {model.EndDateTime.ToShortTimeString()}")
                    .Replace(PLACEHOLDER_LINK, $"{activationLink}?activationcode={model.ActivationCode}")
                    .Replace(PLACEHOLDER_TIMESTAMP, DateTime.Now.ToString());

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = content;

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(_emailConfig.HostName, _emailConfig.Port, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                Log.Debug($"Sending invitation email to '{model.Email}'...");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                Log.Debug($"Successfully sent invitation email to '{model.Email}'.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private async Task<string> LoadTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Mcce.SmartOffice.Bookings.Templates.email-template.txt";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}
