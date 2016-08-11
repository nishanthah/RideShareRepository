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
    [IntentFilter(new[] { "com.virtusa.driverlocatorforms.NOTIFICATION", "com.virtusa.driverlocatorforms.LOADINGCOMPLETED" }, Categories = new[] { "android.intent.category.DEFAULT" })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {

        public const string KEY_REQUEST_ID_EXTRA = "request_id";
        public const string KEY_SOURCE_NAME_EXTRA = "source_name";
        public const string KEY_SOURCE_LONGITUDE_EXTRA = "source_longitude";
        public const string KEY_SOURCE_LATITUDE_EXTRA = "source_latitude";
        public const string KEY_DESTINATION_NAME_EXTRA = "destination_name";
        public const string KEY_DESTINATION_LONGITUDE_EXTRA = "destination_longitude";
        public const string KEY_DESTINATION_LATITUDE_EXTRA = "destination_latitude";

        protected override void OnCreate(Bundle bundle)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            base.OnCreate(bundle); 
            //Getting rid of the App ICON appearing on the taskbar
            ActionBar.SetIcon(null);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            
            if(Intent.Action == "com.virtusa.driverlocatorforms.LOADINGCOMPLETED")
            {
                LoadApplication(new App(false));
            }
            else if(Intent.Action == "com.virtusa.driverlocatorforms.NOTIFICATION")
            {
                if (Intent.HasExtra(KEY_REQUEST_ID_EXTRA))
                {
                    NotificationInfo notificationInfo = new NotificationInfo();
                    notificationInfo.RequestId = Intent.GetStringExtra(KEY_REQUEST_ID_EXTRA);
                    notificationInfo.Source.LocationName = Intent.GetStringExtra(KEY_SOURCE_NAME_EXTRA);
                    notificationInfo.Source.Longitude = double.Parse(Intent.GetStringExtra(KEY_SOURCE_LONGITUDE_EXTRA));
                    notificationInfo.Source.Latitude = double.Parse(Intent.GetStringExtra(KEY_SOURCE_LATITUDE_EXTRA));
                    notificationInfo.Destination.LocationName = Intent.GetStringExtra(KEY_DESTINATION_NAME_EXTRA);
                    notificationInfo.Destination.Longitude = double.Parse(Intent.GetStringExtra(KEY_DESTINATION_LONGITUDE_EXTRA));
                    notificationInfo.Destination.Latitude = double.Parse(Intent.GetStringExtra(KEY_DESTINATION_LATITUDE_EXTRA));

                    LoadApplication(new App(notificationInfo));
                }
            }
            else
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                string channelId = prefs.GetString("urban_airship_client_id", null);
                if(!String.IsNullOrEmpty(channelId))
                {
                    LoadApplication(new App(false));
                }
                else
                {
                    LoadApplication(new App(true));
                }
                
            }
          
        }
    }
}

