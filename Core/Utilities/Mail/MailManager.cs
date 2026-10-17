using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace Core.Utilities.Mail
{
    public class MailManager : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Ortam değişkenlerini (${VAR} veya {VAR}) çözer veya doğrudan değeri döndürür.
        /// </summary>
        public static string ResolveConfigurationValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var trimmed = value.Trim();

            // ${VAR} formatı
            if (trimmed.StartsWith("${", StringComparison.Ordinal) && trimmed.EndsWith("}", StringComparison.Ordinal) && trimmed.Length > 3)
            {
                var envVarName = trimmed.Substring(2, trimmed.Length - 3);
                var envValue = Environment.GetEnvironmentVariable(envVarName);
                if (!string.IsNullOrEmpty(envValue))
                    return envValue;
            }

            // {VAR} formatı
            if (trimmed.StartsWith("{", StringComparison.Ordinal) && trimmed.EndsWith("}", StringComparison.Ordinal) && trimmed.Length > 2)
            {
                var envVarName = trimmed.Substring(1, trimmed.Length - 2);
                var envValue = Environment.GetEnvironmentVariable(envVarName);
                if (!string.IsNullOrEmpty(envValue))
                    return envValue;
            }

            return trimmed;
        }

        private (string smtpServer, int smtpPort, string userName, string password) GetSmtpConfig()
        {
            var section = _configuration.GetSection("EmailConfiguration");

            var smtpServer = ResolveConfigurationValue(section["SmtpServer"]);
            if (string.IsNullOrWhiteSpace(smtpServer) || smtpServer.StartsWith("{", StringComparison.Ordinal))
            {
                smtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP_SERVER")
                    ?? Environment.GetEnvironmentVariable("SMTP_SERVER")
                    ?? "smtp.gmail.com";
            }

            var smtpPortRaw = ResolveConfigurationValue(section["SmtpPort"]);
            if (string.IsNullOrWhiteSpace(smtpPortRaw) || smtpPortRaw.StartsWith("{", StringComparison.Ordinal))
            {
                smtpPortRaw = Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT")
                    ?? Environment.GetEnvironmentVariable("SMTP_PORT")
                    ?? "587";
            }

            if (!int.TryParse(smtpPortRaw, out var smtpPort) || smtpPort <= 0)
            {
                smtpPort = 587;
            }

            var userName = ResolveConfigurationValue(section["UserName"] ?? section["SmtpUsername"]);
            if (string.IsNullOrWhiteSpace(userName) || userName.StartsWith("{", StringComparison.Ordinal))
            {
                userName = Environment.GetEnvironmentVariable("EMAIL_USER")
                    ?? Environment.GetEnvironmentVariable("SMTP_USERNAME");
            }

            var password = ResolveConfigurationValue(section["Password"] ?? section["SmtpPassword"]);
            if (string.IsNullOrWhiteSpace(password) || password.StartsWith("{", StringComparison.Ordinal))
            {
                password = Environment.GetEnvironmentVariable("EMAIL_PASS")
                    ?? Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            }

            return (smtpServer, smtpPort, userName, password);
        }

        private MimeMessage CreateMimeMessage(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            if (emailMessage.ReplyToAddresses != null && emailMessage.ReplyToAddresses.Any())
            {
                message.ReplyTo.AddRange(emailMessage.ReplyToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            }

            message.Subject = emailMessage.Subject ?? string.Empty;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content ?? string.Empty
            };

            return message;
        }

        public void Send(EmailMessage emailMessage)
        {
            var (smtpServer, smtpPort, userName, password) = GetSmtpConfig();
            var message = CreateMimeMessage(emailMessage);

            using var emailClient = new SmtpClient();
            emailClient.Timeout = 10000;
            emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

            var secureSocket = smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
            emailClient.Connect(smtpServer, smtpPort, secureSocket);

            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                emailClient.Authenticate(userName, password);
            }

            emailClient.Send(message);
            emailClient.Disconnect(true);
        }

        public async Task SendAsync(EmailMessage emailMessage)
        {
            var (smtpServer, smtpPort, userName, password) = GetSmtpConfig();
            var message = CreateMimeMessage(emailMessage);

            using var emailClient = new SmtpClient();
            emailClient.Timeout = 10000;
            emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var secureSocket = smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
            await emailClient.ConnectAsync(smtpServer, smtpPort, secureSocket, cts.Token);

            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                await emailClient.AuthenticateAsync(userName, password, cts.Token);
            }

            await emailClient.SendAsync(message, cts.Token);
            await emailClient.DisconnectAsync(true, cts.Token);
        }
    }
}