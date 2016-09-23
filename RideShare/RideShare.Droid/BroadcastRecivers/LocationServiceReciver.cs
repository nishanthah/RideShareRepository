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
using Java.Lang;
using System.Threading;
using RideShare.Droid.Services;

namespace RideShare.Droid.BroadcastRecivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "com.virtusa.driverlocatorforms.LOCATION_SERVICE_STOPPED" })]
    public class LocationServiceReciver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if(intent.Action == "com.virtusa.driverlocatorforms.LOCATION_SERVICE_STOPPED")
            {
                context.StartService(new Intent(context, typeof(LocationUpdatorService)));                
            }
        }
    }
}