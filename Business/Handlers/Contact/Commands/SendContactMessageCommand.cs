using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.Contact.ValidationRules;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Mail;
using Core.Utilities.Results;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Business.Handlers.Contact.Commands
{
    /// <summary>İletişim formu: gönderen yapılandırmadaki no-reply, alıcı support@masavtech.com.</summary>
    public class SendContactMessageCommand : IRequest<IResult>
    {
        private string _fullName;

        public string FullName
        {
            get => _fullName;
            set => _fullName = value;
        }

        /// <summary>Mobil ve diğer istemcilerle geriye dönük uyumluluk için alias</summary>
        public string UserName
        {
            get => _fullName;
            set => _fullName = value;
        }

        public string UserEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class SendContactMessageCommandHandler : IRequestHandler<SendContactMessageCommand, IResult>
    {
        private const string DefaultSupportEmail = "support@masavtech.com";

        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;

        public SendContactMessageCommandHandler(IMailService mailService, IConfiguration configuration)
        {
            _mailService = mailService;
            _configuration = configuration;
        }

        [ValidationAspect(typeof(SendContactMessageValidator), Priority = 1)]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> Handle(SendContactMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var section = _configuration.GetSection("EmailConfiguration");

                var supportEmail = Environment.GetEnvironmentVariable("SUPPORT_EMAIL")
                    ?? MailManager.ResolveConfigurationValue(section["SupportEmail"]);

                if (string.IsNullOrWhiteSpace(supportEmail) || supportEmail.StartsWith("{", StringComparison.Ordinal))
                {
                    supportEmail = DefaultSupportEmail;
                }

                var senderEmail = MailManager.ResolveConfigurationValue(section["SenderEmail"]);
                var senderName = MailManager.ResolveConfigurationValue(section["SenderName"]);

                if (string.IsNullOrWhiteSpace(senderEmail) || senderEmail.StartsWith("{", StringComparison.Ordinal))
                {
                    senderEmail = Environment.GetEnvironmentVariable("EMAIL_SENDER_ADDR")
                        ?? Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL")
                        ?? "no-reply@masavtech.com";
                }

                if (string.IsNullOrWhiteSpace(senderName) || senderName.StartsWith("{", StringComparison.Ordinal))
                {
                    senderName = Environment.GetEnvironmentVariable("EMAIL_SENDER_NAME")
                        ?? Environment.GetEnvironmentVariable("SMTP_SENDER_NAME")
                        ?? "Kursum Mobil";
                }

                var appName = senderName;
                var safeName = WebUtility.HtmlEncode((request.FullName ?? string.Empty).Trim());
                var safeMail = WebUtility.HtmlEncode((request.UserEmail ?? string.Empty).Trim());
                var safeSubject = WebUtility.HtmlEncode((request.Subject ?? string.Empty).Trim());
                var bodyText = request.Message ?? string.Empty;
                var safeBody = WebUtility.HtmlEncode(bodyText).Replace("\n", "<br>", StringComparison.Ordinal);

                var emailContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; line-height: 1.6; color: #1E293B; background-color: #F8FAFC; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: #FFFFFF; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.06); border: 1px solid #E2E8F0; }}
        .header {{ background: linear-gradient(135deg, #1E3A8A 0%, #3B82F6 100%); color: #FFFFFF; padding: 24px; text-align: center; }}
        .header h2 {{ margin: 0; font-size: 20px; font-weight: 700; }}
        .badge {{ display: inline-block; background: rgba(255,255,255,0.2); padding: 4px 12px; border-radius: 20px; font-size: 12px; margin-top: 8px; }}
        .content {{ padding: 24px; }}
        .meta-card {{ background-color: #F1F5F9; border-radius: 8px; padding: 16px; margin-bottom: 20px; border-left: 4px solid #3B82F6; }}
        .meta-row {{ margin-bottom: 6px; font-size: 14px; }}
        .meta-row:last-child {{ margin-bottom: 0; }}
        .meta-label {{ font-weight: 600; color: #475569; display: inline-block; width: 90px; }}
        .message-box {{ background-color: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 8px; padding: 18px; font-size: 14px; line-height: 1.7; color: #0F172A; }}
        .footer {{ padding: 16px 24px; background: #F8FAFC; border-top: 1px solid #E2E8F0; text-align: center; font-size: 12px; color: #94A3B8; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Yeni İletişim / Şikayet Formu</h2>
            <div class='badge'>{WebUtility.HtmlEncode(appName)}</div>
        </div>
        <div class='content'>
            <div class='meta-card'>
                <div class='meta-row'><span class='meta-label'>Ad Soyad:</span> {safeName}</div>
                <div class='meta-row'><span class='meta-label'>E-posta:</span> <a href='mailto:{safeMail}' style='color:#2563EB;'>{safeMail}</a></div>
                <div class='meta-row'><span class='meta-label'>Konu:</span> <strong>{safeSubject}</strong></div>
            </div>
            <div class='message-box'>
                <div style='font-weight: 600; margin-bottom: 8px; color: #475569;'>Mesaj:</div>
                {safeBody}
            </div>
        </div>
        <div class='footer'>
            Bu e-posta {WebUtility.HtmlEncode(appName)} mobil uygulaması iletişim formu aracılığıyla gönderilmiştir.
        </div>
    </div>
</body>
</html>";

                var emailMessage = new EmailMessage
                {
                    ToAddresses = new List<EmailAddress>
                    {
                        new EmailAddress { Name = "MasavTech Destek", Address = supportEmail }
                    },
                    FromAddresses = new List<EmailAddress>
                    {
                        new EmailAddress { Name = senderName, Address = senderEmail }
                    },
                    Subject = $"{appName} - İletişim: {request.Subject.Trim()}",
                    Content = emailContent
                };

                if (!string.IsNullOrWhiteSpace(request.UserEmail))
                {
                    emailMessage.ReplyToAddresses.Add(
                        new EmailAddress
                        {
                            Name = string.IsNullOrWhiteSpace(request.FullName)
                                ? "Kullanıcı"
                                : request.FullName.Trim(),
                            Address = request.UserEmail.Trim()
                        });
                }

                await _mailService.SendAsync(emailMessage);
                return new SuccessResult("Mesajınız başarıyla iletildi.");
            }
            catch (Exception ex)
            {
                return new ErrorResult("Mesaj gönderilirken bir hata oluştu: " + ex.Message);
            }
        }
    }
}
