using Common.Dto;

namespace Notification.BL.Services.IServices
{
    public interface ISendService
    {
        public Task SendEmail(EmailMessageDto emailMessageDto);
    }
}
