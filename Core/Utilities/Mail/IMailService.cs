using System.Threading.Tasks;

namespace Core.Utilities.Mail
{
    public interface IMailService
    {
        void Send(EmailMessage emailMessage);
        Task SendAsync(EmailMessage emailMessage);
    }
}