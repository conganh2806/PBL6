using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using WebNovel.API.Core.Services.Schemas;

namespace WebNovel.API.Core.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
    public class SmtpMailService : IEmailService
    {
        private readonly MailSettings _settings;
        private readonly ILogger<SmtpMailService> _logger;

        public SmtpMailService(
            IOptions<MailSettings> settings,
            ILogger<SmtpMailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
                {
                    EnableSsl = _settings.Ssl,
                    Host = _settings.Host,
                    Port = _settings.Port,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settings.Email, _settings.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                var message = new MailMessage
                {
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    From = string.IsNullOrEmpty(_settings.Name) ? new MailAddress(_settings.Email) : new MailAddress(_settings.Email, _settings.Name),
                    Subject = request.Subject,
                    Body = request.Body,
                    Priority = MailPriority.Normal
                };
                message.To.Add(request.ToMail);
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmailService: Unhandled Exception for Request {@Request}", request);
            }
        }
    }
}