using System;

using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using RideShare;
using Android.App;
using Android.Preferences;
using Android.Content;

namespace RideShare.Droid
{
    //, Icon = "@drawable/app_icon"
    [Activity(Label = "RideShare", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    [IntentFilter(new[] { "com.virtusa.driverlocatorforms.NOTIFICATIONACCEPTED",
        "com.virtusa.driverlocatorforms.NOTIFICATIONREJECTED",
        "com.virtusa.driverlocatorforms.NOTIFICATIONOPENED",
        "com.virtusa.driverlocatorforms.LOADINGCOMPLETED" }, Categories = new[] { "android.intent.category.DEFAULT" })]
    [IntentFilter(new[] { "android.intent.action.VIEW" }, Categories = new[] { "android.intent.category.DEFAULT" }, DataScheme = "http",
    DataHost = "rideshareresetpassword")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {

        public const string KEY_REQUEST_ID_EXTRA = "request_id";

        protected override void OnCreate(Bundle bundle)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            base.OnCreate(bundle);
            //Getting rid of the App ICON appearing on the taskbar
            ActionBar.SetIcon(null);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            App.DeviceType = App.DeviceTypes.Android;
            App.DeviceVersion = Convert.ToInt32(Build.VERSION.Sdk);
           // App.ApplicationVersion = PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;            

            //**ANDROID_ID - this id is newly generated once the device is wiped
            App.DeviceUniqueID = Android.Provider.Settings.Secure.GetString(MainApp.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            ////**MacAddress - Needs Wifi
            //Android.Net.Wifi.WifiManager wManager = (Android.Net.Wifi.WifiManager)GetSystemService(Context.WifiService);
            //Android.Net.Wifi.WifiInfo wInfo = wManager.ConnectionInfo;
            //App.DeviceUniqueID = wInfo.MacAddress;

            ////**DeviceID - What happens in a DUAL SIM device, since device id is per SIM
            //Android.Telephony.TelephonyManager tMgr = (Android.Telephony.TelephonyManager)this.GetSystemService(Android.Content.Context.TelephonyService);
            //App.DeviceUniqueID = tMgr.GetDeviceId(1);

            ////**SerialNumber - non-telephony devices does not have this number
            //App.DeviceUniqueID = Android.Provider.Settings.Secure.GetString(MainApp.Context.ContentResolver, Android.OS.Build.Serial);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            string channelId = prefs.GetString("urban_airship_client_id", null);
            if (!String.IsNullOrEmpty(channelId))
            {
                LoadApplication(new App(false, null));
            }
            else
            {
#if WithNotification
                    LoadApplication(new App(false, null));
#else
                LoadApplication(new App(true, null));
#endif

            }



        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Intent = intent;
            if(Intent.Action == "com.virtusa.driverlocatorforms.LOADINGCOMPLETED")
            {
                LoadApplication(new App(false, null));
            }
            else if(Intent.Action == "com.virtusa.driverlocatorforms.NOTIFICATIONREJECTED")
            {
                if (Intent.HasExtra(KEY_REQUEST_ID_EXTRA))
                {
                    NotificationInfo notificationInfo = new NotificationInfo();
                    notificationInfo.NotificationStatus = NotificationStatus.Rejected;
                    notificationInfo.RequestId = Intent.GetStringExtra(KEY_REQUEST_ID_EXTRA);
                    LoadApplication(new App(notificationInfo));
                }
            }
            else if (Intent.Action == "com.virtusa.driverlocatorforms.NOTIFICATIONACCEPTED")
            {
                if (Intent.HasExtra(KEY_REQUEST_ID_EXTRA))
                {
                    NotificationInfo notificationInfo = new NotificationInfo();
                    notificationInfo.NotificationStatus = NotificationStatus.Accepted;
                    notificationInfo.RequestId = Intent.GetStringExtra(KEY_REQUEST_ID_EXTRA);
                    LoadApplication(new App(notificationInfo));
                }
            }
            else if (Intent.Data != null && Intent.Data.Host != null && Intent.Data.Host == "rideshareresetpassword")
            {

                string newguid = Intent.Data.Query.Split('=')[1];        
                LoadApplication(new App(false, newguid));
            }
            else if (Intent.Action == "com.virtusa.driverlocatorforms.NOTIFICATIONOPENED")
            {
                if (Intent.HasExtra(KEY_REQUEST_ID_EXTRA))
                {
                    NotificationInfo notificationInfo = new NotificationInfo();
                    notificationInfo.NotificationStatus = NotificationStatus.Opened;
                    notificationInfo.RequestId = Intent.GetStringExtra(KEY_REQUEST_ID_EXTRA);
                    LoadApplication(new App(notificationInfo));
                }
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RideShare.SharedInterfaces.IAppDataService appDataService = Xamarin.Forms.DependencyService.Get<RideShare.SharedInterfaces.IAppDataService>();
            if (appDataService.Get("is_user_logged_in") == "true")
            {
                App.LogoutUser(); 
             
                appDataService.Save("access_token", null);
                appDataService.Save("current_user", null);
                appDataService.Save("is_user_logged_in", "false");
                Session.ClearAuthenticationInstance();
                Session.CurrentUserName = null;
                
                RideShare.SharedInterfaces.ILocationService locationService = Xamarin.Forms.DependencyService.Get<RideShare.SharedInterfaces.ILocationService>();
                RideShare.SharedInterfaces.IHistoryUpdator historyUpdator = Xamarin.Forms.DependencyService.Get<RideShare.SharedInterfaces.IHistoryUpdator>();
                locationService.StopLocationService();
                historyUpdator.StopHistoryUpdatorService();
            }

        }

        public override void OnBackPressed()
        {
            Intent main = new Intent(Intent.ActionMain);
            main.AddCategory(Intent.CategoryHome);
            StartActivity(main);
        }
    }
}

