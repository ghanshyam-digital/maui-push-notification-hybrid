### Add Firebase Configuration Files

- Add google-services.json for Android
- Go to the Firebase Console.
- Create a new project or select an existing one.
- Navigate to Project Settings > General.
- In the "Your apps" section, register your Android app.

### Set Build Action

- Right-click the Google-services.json file in Solution Explorer.
- Choose Properties.
- Set Build Action to GoogleServicesJson.

### Install Required NuGet Packages

Plugin.Firebase
Plugin.Firebase.Crashlytics
Plugin.Firebase.CloudMessaging

### Add Required Resources


```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <string name="com.google.firebase.crashlytics.mapping_flied_id">none</string>
    <string name="com.crashlytics.android.build_id">1</string>
</resources>
```

### Check blog 

https://blog.ghanshyamdigital.com/how-to-implement-firebase-push-notifications-in-a-net-maui-hybrid-app
