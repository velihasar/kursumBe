using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IPushNotificationService
    {
        Task SendToUserAsync(int userId, string title, string body, IDictionary<string, string> data = null);
        Task SendToUserWithTokensAsync(int userId, List<string> deviceTokens, string title, string body, IDictionary<string, string> data = null);
    }
}
