using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiNotificationApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace MauiNotificationApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NotificationService> _logger;
        private readonly string _serverKey;

        public NotificationService(IHttpClientFactory httpClientFactory, ILogger<NotificationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            
            // Your Firebase Cloud Messaging Server Key from Firebase Console
            // Project Settings > Cloud Messaging > Server key
            _serverKey = "YOUR_FIREBASE_SERVER_KEY_HERE";
        }

        public async Task<bool> SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string> data = null)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceToken))
                {
                    _logger.LogWarning("Device token is empty");
                    return false;
                }

                var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
                
                // Add server key to authorization header
                request.Headers.Add("Authorization", $"key={_serverKey}");
                request.Headers.Add("Content-Type", "application/json");

                // Create notification payload
                var payload = new
                {
                    to = deviceToken,
                    notification = new
                    {
                        title = title,
                        body = body,
                        sound = "default",
                        badge = 1
                    },
                    data = data ?? new Dictionary<string, string>
                    {
                        { "click_action", "FLUTTER_NOTIFICATION_CLICK" },
                        { "id", "1" },
                        { "status", "done" }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"FCM Response: {responseContent}");
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification");
                return false;
            }
        }
    }
}