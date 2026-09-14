using System;
using System.Threading.Tasks;
using Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs
{
    /// <summary>
    /// Genel bildirimler ve gerçek zamanlı güncellemeler için SignalR Hub.
    /// Finder projesindeki ChatHub altyapısı temel alınmıştır.
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub
    {
        public NotificationHub()
        {
        }

        private static string GetUserGroupName(int userId) => $"user-{userId}";
        private static string GetRoomGroupName(string roomType, string roomId) => $"room-{roomType}-{roomId}";

        /// <summary>
        /// Kullanıcının kendi özel bildirim grubuna katılması
        /// </summary>
        public async Task JoinUserGroup()
        {
            try
            {
                var userId = UserInfoExtensions.GetUserId();
                await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
                
                // Debug log
                System.Diagnostics.Debug.WriteLine($"SignalR: User {userId} joined their personal group.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignalR JoinUserGroup Error: {ex.Message}");
                throw new HubException("User identity could not be verified.");
            }
        }

        /// <summary>
        /// Belirli bir "Oda" veya "Sipariş" grubuna katılma
        /// </summary>
        public async Task JoinRoom(string roomType, string roomId)
        {
            // İleride burada yetki kontrolü yapılabilir (Sipariş sahibi mi? vb.)
            await Groups.AddToGroupAsync(Context.ConnectionId, GetRoomGroupName(roomType, roomId));
        }

        /// <summary>
        /// Gruptan ayrılma
        /// </summary>
        public async Task LeaveRoom(string roomType, string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetRoomGroupName(roomType, roomId));
        }

        public override async Task OnConnectedAsync()
        {
            // Bağlantı kurulduğunda yapılacak işlemler
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Bağlantı koptuğunda yapılacak işlemler
            await base.OnDisconnectedAsync(exception);
        }
    }
}
