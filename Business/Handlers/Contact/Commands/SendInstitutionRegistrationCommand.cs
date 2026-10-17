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
    /// <summary>Kurum kayıt / demo başvurusu talebi. Alıcı: vhas_ar@hotmail.com</summary>
    public class SendInstitutionRegistrationCommand : IRequest<IResult>
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string InstitutionName { get; set; }
        public string City { get; set; }
        public string InstitutionType { get; set; }
        public string StudentCount { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }

    public class SendInstitutionRegistrationCommandHandler : IRequestHandler<SendInstitutionRegistrationCommand, IResult>
    {
        private const string PrimaryRecipient = "vhas_ar@hotmail.com";

        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;

        public SendInstitutionRegistrationCommandHandler(IMailService mailService, IConfiguration configuration)
        {
            _mailService = mailService;
            _configuration = configuration;
        }

        [ValidationAspect(typeof(SendInstitutionRegistrationValidator), Priority = 1)]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> Handle(SendInstitutionRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var section = _configuration.GetSection("EmailConfiguration");

                var targetEmail = Environment.GetEnvironmentVariable("REGISTRATION_TARGET_EMAIL")
                    ?? MailManager.ResolveConfigurationValue(section["RegistrationTargetEmail"]);

                if (string.IsNullOrWhiteSpace(targetEmail) || targetEmail.StartsWith("{", StringComparison.Ordinal))
                {
                    targetEmail = PrimaryRecipient;
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
                    senderName = "Kursum Başvuru Sistemi";
                }

                var safeFullName = WebUtility.HtmlEncode((request.FullName ?? string.Empty).Trim());
                var safePhone = WebUtility.HtmlEncode((request.Phone ?? string.Empty).Trim());
                var safeEmail = WebUtility.HtmlEncode((request.Email ?? string.Empty).Trim());
                var safeInstName = WebUtility.HtmlEncode((request.InstitutionName ?? string.Empty).Trim());
                var safeCity = WebUtility.HtmlEncode((request.City ?? "Belirtilmedi").Trim());
                var safeInstTypeRow = !string.IsNullOrWhiteSpace(request.InstitutionType)
                    ? $@"<div class='row'><span class='label'>Kurum Türü:</span><span class='value'>{WebUtility.HtmlEncode(request.InstitutionType.Trim())}</span></div>"
                    : string.Empty;
                var safeStudentCount = WebUtility.HtmlEncode((request.StudentCount ?? "Belirtilmedi").Trim());
                var safeSource = WebUtility.HtmlEncode((request.Source ?? "Web Panel").Trim());
                var bodyText = request.Message ?? string.Empty;
                var safeMessage = string.IsNullOrWhiteSpace(bodyText)
                    ? "<em>Not eklenmedi.</em>"
                    : WebUtility.HtmlEncode(bodyText).Replace("\n", "<br>", StringComparison.Ordinal);

                var nowFormatted = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                var emailContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; line-height: 1.6; color: #1E293B; background-color: #F1F5F9; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: #FFFFFF; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 25px rgba(0,0,0,0.08); border: 1px solid #E2E8F0; }}
        .header {{ background: linear-gradient(135deg, #1E3A8A 0%, #2563EB 50%, #4F46E5 100%); color: #FFFFFF; padding: 28px 24px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 22px; font-weight: 800; letter-spacing: -0.3px; }}
        .badge {{ display: inline-block; background: rgba(255,255,255,0.22); padding: 5px 14px; border-radius: 20px; font-size: 12px; font-weight: 600; margin-top: 10px; letter-spacing: 0.3px; }}
        .content {{ padding: 26px 24px; }}
        .alert-pill {{ background: #ECFDF5; border: 1px solid #A7F3D0; color: #065F46; padding: 12px 16px; border-radius: 10px; font-size: 13px; font-weight: 600; margin-bottom: 22px; display: flex; align-items: center; }}
        .lead-card {{ background-color: #F8FAFC; border-radius: 12px; padding: 18px; border: 1px solid #E2E8F0; margin-bottom: 20px; }}
        .row {{ padding: 8px 0; border-bottom: 1px solid #EDF2F7; font-size: 14px; }}
        .row:last-child {{ border-bottom: none; }}
        .label {{ font-weight: 700; color: #475569; display: inline-block; width: 140px; }}
        .value {{ color: #0F172A; font-weight: 600; }}
        .value a {{ color: #2563EB; text-decoration: none; }}
        .value strong {{ color: #1E3A8A; font-weight: 800; }}
        .message-box {{ background-color: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 10px; padding: 16px; margin-top: 10px; font-size: 13px; color: #334155; line-height: 1.6; }}
        .cta-container {{ text-align: center; margin: 26px 0 10px; }}
        .cta-btn {{ display: inline-block; background: #2563EB; color: #FFFFFF !important; font-weight: 700; font-size: 14px; padding: 12px 24px; border-radius: 10px; text-decoration: none; box-shadow: 0 4px 10px rgba(37,99,235,0.3); }}
        .footer {{ padding: 16px 24px; background: #F8FAFC; border-top: 1px solid #E2E8F0; text-align: center; font-size: 12px; color: #64748B; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎓 Yeni Kurum Kayıt / Demo Talebi</h1>
            <div class='badge'>Kaynak: {safeSource} • {nowFormatted}</div>
        </div>
        <div class='content'>
            <div class='alert-pill'>
                ⚡ Yeni bir eğitim kurumu Kursum sistemine dahil olmak için kayıt talebinde bulundu.
            </div>

            <div class='lead-card'>
                <div class='row'>
                    <span class='label'>Kurum Adı:</span>
                    <span class='value'><strong>{safeInstName}</strong></span>
                </div>
                <div class='row'>
                    <span class='label'>Yetkili Kişi:</span>
                    <span class='value'>{safeFullName}</span>
                </div>
                <div class='row'>
                    <span class='label'>Telefon:</span>
                    <span class='value'><a href='tel:{safePhone}'>{safePhone}</a></span>
                </div>
                <div class='row'>
                    <span class='label'>E-posta:</span>
                    <span class='value'><a href='mailto:{safeEmail}'>{safeEmail}</a></span>
                </div>
                <div class='row'>
                    <span class='label'>Şehir / Lokasyon:</span>
                    <span class='value'>{safeCity}</span>
                </div>
                {safeInstTypeRow}
                <div class='row'>
                    <span class='label'>Tahmini Öğrenci:</span>
                    <span class='value'>{safeStudentCount}</span>
                </div>
            </div>

            <div style='font-size: 13px; font-weight: 700; color: #475569; margin-top: 14px;'>Ek Açıklama / Talepler:</div>
            <div class='message-box'>
                {safeMessage}
            </div>

            <div class='cta-container'>
                <a href='tel:{safePhone}' class='cta-btn'>📞 Yetkiliyi Hemen Ara ({safePhone})</a>
            </div>
        </div>
        <div class='footer'>
            Bu talep <strong>Kursum Yönetim Portalı</strong> kayıt formu üzerinden iletilmiştir.
        </div>
    </div>
</body>
</html>";

                var emailMessage = new EmailMessage
                {
                    ToAddresses = new List<EmailAddress>
                    {
                        new EmailAddress { Name = "Veli Hasar", Address = targetEmail }
                    },
                    FromAddresses = new List<EmailAddress>
                    {
                        new EmailAddress { Name = senderName, Address = senderEmail }
                    },
                    Subject = $"🎓 Kursum - Yeni Kurum Kayıt Başvurusu: {request.InstitutionName.Trim()}",
                    Content = emailContent
                };

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    emailMessage.ReplyToAddresses.Add(
                        new EmailAddress
                        {
                            Name = string.IsNullOrWhiteSpace(request.FullName) ? "Yetkili" : request.FullName.Trim(),
                            Address = request.Email.Trim()
                        });
                }

                await _mailService.SendAsync(emailMessage);
                return new SuccessResult("Kurum kayıt talebiniz başarıyla alındı. Yetkililerimiz en kısa sürede sizinle iletişime geçecektir.");
            }
            catch (Exception ex)
            {
                return new ErrorResult("Kayıt talebi gönderilirken bir hata oluştu: " + ex.Message);
            }
        }
    }
}
