using System;
using Android.App;
using Android.Runtime;
using Firebase;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Plugin.Firebase.Crashlytics;

namespace MauiNotificationApp
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            try
            {
                FirebaseApp.InitializeApp(this);

                if (CrossFirebaseCrashlytics.Current != null)
                {
                    CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
                    System.Diagnostics.Debug.WriteLine("Crashlytics initialized successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Crashlytics not available");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase initialization error: {ex.Message}");
            }
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
