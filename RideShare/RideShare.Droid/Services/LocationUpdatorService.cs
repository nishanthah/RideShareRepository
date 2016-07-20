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
using Java.Lang;
using RideShare.Utilities;
using Android.App;

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
            Thread t = new Thread(() => {
                while(true)
                {
                    LocationUtility.UpdateCurrentLocation();
                    Thread.Sleep(5000);
                }
            });
            t.Start();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            // cleanup code
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}