using Android.App;
using Firebase;
using Plugin.Firebase.Analytics;
using Plugin.Firebase.Bundled.Shared;


namespace MauiNotificationApp.Platforms.Android
{
    public static class CrossFirebase
    {
        public static void Initialize(
    Activity activity,
    CrossFirebaseSettings settings,
    FirebaseOptions firebaseOptions = null,
    string name = null)
        {
            if (firebaseOptions == null)
            {
                FirebaseApp.InitializeApp(activity);
            }
            else if (name == null)
            {
                FirebaseApp.InitializeApp(activity, firebaseOptions);
            }
            else
            {
                FirebaseApp.InitializeApp(activity, firebaseOptions, name);
            }

            if (settings.IsAnalyticsEnabled)
            {
                FirebaseAnalyticsImplementation.Initialize(activity);
            }

            Console.WriteLine($"Plugin.Firebase initialized with the following settings:\n{settings}");
        }
    }
}
