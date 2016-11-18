using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using RideShare.Utilities;
using Android.App;
using RideShare.SharedInterfaces;
using Xamarin.Forms;
using System.Threading;
using RideShare.Droid.DependecyServices;

namespace RideShare.Droid.Services
{
    [Service]
    public class LocationUpdatorService : Service
    {
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("LocationUpdatorService", "LocationUpdatorService started");
            DoWork();      
            return StartCommandResult.Sticky;
        }

        
        public void DoWork()
        {
            ILocationService locService = new LocationServiceDroid();
            locService.InitLocationService();
            LocationUtility locationUtility = new LocationUtility(locService);

            Thread t = new Thread(() => { 
                while(true)
                {
                    locationUtility.UpdateCurrentLocation();
                    Thread.Sleep(10000);                  
                }
            });
            t.IsBackground = false;
            t.Start();
        }

        public override void OnDestroy()
        {
            IAppDataService appDataService = DependencyService.Get<IAppDataService>();
            if (appDataService.Get("is_user_logged_in") == "true")
            {
                Intent intent = new Intent("com.virtusa.driverlocatorforms.LOCATION_SERVICE_STOPPED");
                SendBroadcast(intent);
            }
            //base.OnDestroy();
            // cleanup code
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}