using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using RideShare.SharedInterfaces;
using RideShare.Droid.DependecyServices;
using RideShare.Utilities;

namespace RideShare.Droid.Services
{

    [Service]
    public class HistoryRouteUpdatorService : Service
    {
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("HistoryRouteUpdatorService", "HistoryRouteUpdatorService started");
            DoWork();
            return StartCommandResult.Sticky;
        }


        public void DoWork()
        {
            ILocationService locService = new LocationServiceDroid();
            locService.StartLocationService();
            LocationUtility locationUtility = new LocationUtility(locService);

            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    locationUtility.AddHistoryLocation();
                    System.Threading.Thread.Sleep(5000);
                }
            });
            t.IsBackground = false;
            t.Start();
        }

        public override void OnDestroy()
        {
            Intent intent = new Intent("com.virtusa.driverlocatorforms.HISTORY_ROUTE_SERVICE_STOPPED");
            SendBroadcast(intent);
            //base.OnDestroy();
            // cleanup code
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}