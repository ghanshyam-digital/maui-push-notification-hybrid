using MauiNotificationApp.Interfaces;
using MauiNotificationApp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;
using Plugin.Firebase.CloudMessaging;
#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Bundled.Platforms.Android;
#endif

namespace MauiNotificationApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterFirebaseServices()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            
            builder.Services.AddHttpClient();
            
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddSingleton<PushNotification>();
                

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            CrossFirebaseCloudMessaging.Current.NotificationTapped += static (s, e) =>
            {
                if (e.Notification.Data.TryGetValue("target", out var target))
                {
                    Preferences.Set("NotificationTarget", target);
                }
            };


            return builder.Build();
        }

        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.WillFinishLaunching((app, launchOptions) => {
                CrossFirebase.Initialize(CreateCrossFirebaseSettings());
                return false;
            }));
#elif ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) => {
                    var settings = CreateCrossFirebaseSettings();
                    CrossFirebase.Initialize(activity, settings);

                    CrossFirebaseCloudMessaging.Current.NotificationTapped += async (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Notification Tapped");
 
                //await MainThread.InvokeOnMainThreadAsync(async () =>
                //{
                //    await Shell.Current.GoToAsync($"///{nameof(counter)}");
                //});
            };
                }));
#endif
            });

            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
            builder.Services.AddSingleton(_ => CrossFirebaseCloudMessaging.Current);


            return builder;
        }

        private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        {
            return new CrossFirebaseSettings(
                isAuthEnabled: true,
                isCloudMessagingEnabled: true,
                isCrashlyticsEnabled: true
            );
        }
    }
}
