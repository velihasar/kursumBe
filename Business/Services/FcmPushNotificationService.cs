using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Configuration;
using DataAccess.Abstract;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace Business.Services
{
    public class FcmPushNotificationService : IPushNotificationService
    {
        private string FcmV1Endpoint => $"https://fcm.googleapis.com/v1/projects/{_fcmSettings.ProjectId}/messages:send";
        private const int MaxRetryAttempts = 3;
        private const int BaseDelaySeconds = 2;

        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly HttpClient _httpClient;
        private readonly FcmSettings _fcmSettings;
        private GoogleCredential _googleCredential;
        private DateTime? _tokenExpiryTime;
        private string _cachedAccessToken;

        public FcmPushNotificationService(
            IUserDeviceRepository userDeviceRepository,
            IOptions<FcmSettings> fcmSettings)
        {
            _userDeviceRepository = userDeviceRepository;
            _fcmSettings = fcmSettings.Value;
            _httpClient = new HttpClient();
            InitializeGoogleCredential();
        }

        private void InitializeGoogleCredential()
        {
            try
            {
                var serviceAccountJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
                if (!string.IsNullOrWhiteSpace(serviceAccountJson))
                {
                    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serviceAccountJson));
                    _googleCredential = GoogleCredential.FromStream(stream)
                        .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_fcmSettings?.ServiceAccountJsonPath)) return;

                var fullPath = Path.GetFullPath(_fcmSettings.ServiceAccountJsonPath);
                if (!File.Exists(fullPath)) return;

                using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                _googleCredential = GoogleCredential.FromStream(fileStream)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
            }
            catch { }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrWhiteSpace(_cachedAccessToken) && _tokenExpiryTime.HasValue && _tokenExpiryTime.Value > DateTime.UtcNow.AddMinutes(5))
                return _cachedAccessToken;

            if (_googleCredential == null) return null;

            var token = await _googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            _cachedAccessToken = token;
            _tokenExpiryTime = DateTime.UtcNow.AddHours(1);
            return token;
        }

        public async Task SendToUserAsync(int userId, string title, string body, IDictionary<string, string> data = null)
        {
            var devices = await _userDeviceRepository.GetListAsync(x => x.UserId == userId && x.IsActive);
            var tokens = devices.Select(d => d.DeviceToken).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
            if (tokens.Any()) await SendToUserWithTokensAsync(userId, tokens, title, body, data);
        }

        public async Task SendToUserWithTokensAsync(int userId, List<string> deviceTokens, string title, string body, IDictionary<string, string> data = null)
        {
            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(accessToken) || deviceTokens == null || !deviceTokens.Any()) return;

            foreach (var token in deviceTokens)
            {
                var payload = new
                {
                    message = new
                    {
                        token = token,
                        notification = new { title = title, body = body },
                        data = data?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string>(),
                        android = new { priority = "high", notification = new { sound = "default", channel_id = "fcm_default_channel" } },
                        apns = new { payload = new { aps = new { sound = "default", badge = 1 } } }
                    }
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                using var request = new HttpRequestMessage(HttpMethod.Post, FcmV1Endpoint);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                await SendPushWithRetryAsync(request);
            }
        }

        private async Task SendPushWithRetryAsync(HttpRequestMessage request)
        {
            try { await _httpClient.SendAsync(request); } catch { }
        }
    }
}
