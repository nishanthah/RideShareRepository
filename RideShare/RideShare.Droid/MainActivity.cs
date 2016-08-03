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

