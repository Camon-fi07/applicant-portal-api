using Common.Dto;
using Common.Models;
using Microsoft.Extensions.Configuration;
using Notification.BL.Models;
using Notification.BL.Services.IServices;
using System.Net.Mail;

namespace Notification.BL.Services
{
    public class SendService : ISendService
    {
        private readonly string senderEmail;
        private readonly SmtpConfiguration smtpConfiguration;

        public SendService(IConfiguration configuration) {
            senderEmail = configuration.GetSection("SenderEmail").Value ?? "";
            smtpConfiguration = new SmtpConfiguration();
            configuration.GetSection("SmtpClient").Bind(smtpConfiguration);
        }

        public async Task SendEmail(EmailMessageDto emailMessageDto)
        {
            SmtpClient smtpClient = new SmtpClient(smtpConfiguration.Host, smtpConfiguration.Port);
            MailMessage message = new MailMessage(senderEmail, emailMessageDto.Email, emailMessageDto.Subject, emailMessageDto.Body);
            smtpClient.EnableSsl = false;
            smtpClient.Send(message);
        }
    }
}
