using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Plugin.Firebase.CloudMessaging;

namespace MauiPushNotificationHybrid
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static string PendingNavigationTarget { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CreateNotificationChannelIfNeeded();
            RequestPushNotificationsPermission();

            // Handle the intent that started this activity
            HandleIntent(Intent);
        }

        private void HandleIntent(Intent intent)
        {
            if (intent?.Extras != null && intent.Extras.ContainsKey("target"))
            {
                var target = intent.Extras.GetString("target");
                if (!string.IsNullOrEmpty(target))
                {
                    // Store the target in preferences
                    Android.Preferences.PreferenceManager
                        .GetDefaultSharedPreferences(this)
                        .Edit()
                        .PutString("NotificationTarget", target)
                        .Commit();

                    // Store for immediate navigation in App.xaml.cs
                    PendingNavigationTarget = target;

                    // If the app is already running, navigate immediately
                    NavigateToTarget(target);
                }
            }
        }

        private void NavigateToTarget(string target)
        {
            // Ensure we have an active Shell/MainPage to navigate with
            if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        // Check if target is "Count" or a full route
                        if (target.Equals("Count", StringComparison.OrdinalIgnoreCase))
                        {
                            await Shell.Current.GoToAsync("//counter");
                        }
                        else
                        {
                            await Shell.Current.GoToAsync(target);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                    }
                });
            }
        }

        private void RequestPushNotificationsPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.PostNotifications }, 0);
            }
        }

        private void CreateNotificationChannelIfNeeded()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel();
            }
        }

        private void CreateNotificationChannel()
        {
            var channelId = $"{PackageName}.general";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
            FirebaseCloudMessagingImplementation.ChannelId = channelId;
            //FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.ic_push_small;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            HandleIntent(intent);
        }
    }
}
