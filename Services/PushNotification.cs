using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Serilog;
using Message = FirebaseAdmin.Messaging.Message;
using Notification = FirebaseAdmin.Messaging.Notification;

namespace MauiNotificationApp.Services
{
    public class PushNotification
    {
        private readonly FirebaseMessaging _firebaseMessaging;

        public PushNotification()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    
                    Credential = GoogleCredential.FromFile("firebase-adminsdk.json")
                });
            }

            _firebaseMessaging = FirebaseMessaging.DefaultInstance;
        }

        public async Task SendPushNotification(string deviceToken, string title, string body, object data)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body,
                    ImageUrl =
                        "https://media.istockphoto.com/id/472909414/photo/demo-sign-colorful-tags.jpg?s=612x612&w=0&k=20&c=JD4iYaMoe4x--zJrltwiQzZdDyvtrRuNBK0uQ80tdXg="
                },
                Data = new Dictionary<string, string>
                {
                    { "click_action", "FLUTTER_NOTIFICATION_CLICK" },
                    { "target", "counter" },
                    { "extraData", JsonConvert.SerializeObject(data) }
                }
            };

            try
            {
                var response = await _firebaseMessaging.SendAsync(message);
                Log.Information($"Push Notification sent successfully. Message ID: {response}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error sending push notification: {ex.Message}");
                throw; // Re-throw the exception so that the caller knows something went wrong
            }
        }
    }
}