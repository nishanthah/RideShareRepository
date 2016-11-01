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
    [Activity(Label = "RideShare", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { "com.virtusa.driverlocatorforms.NOTIFICATIONACCEPTED", "com.virtusa.driverlocatorforms.NOTIFICATIONREJECTED", "com.virtusa.driverlocatorforms.NOTIFICATIONOPENED", "com.virtusa.driverlocatorforms.LOADINGCOMPLETED", "android.intent.action.VIEW" }, Categories = new[] { "android.intent.category.DEFAULT" }, DataScheme = "http",
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
            if (Intent.Data != null && Intent.Data.Host != null && Intent.Data.Host == "rideshareresetpassword")
            {
                string newguid = "9c122999-e0ae-2a35-de96-b410fc1c2d91";//Intent.GetStringExtra("id");
            
            //App.DeviceVersion = Build.VERSION;
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
            else
            {
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
          
        }
    }
}

