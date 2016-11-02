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
using Android.Locations;
using Common;

namespace RideShare.Droid.BroadcastRecivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { LocationManager.ProvidersChangedAction })]
    public class LocationOnOffReciver : BroadcastReceiver
    {
        
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == LocationManager.ProvidersChangedAction && Session.LocationServiceStatusCallback != null)
            {
                LocationManager lm = (LocationManager)MainApp.Context.GetSystemService(Context.LocationService);
                if(lm.IsProviderEnabled(LocationManager.GpsProvider))
                {

                    Session.LocationServiceStatusCallback.OnLocationStatusChange(LocationServiceStatus.Available);
                }
                else
                {
                    Session.LocationServiceStatusCallback.OnLocationStatusChange(LocationServiceStatus.OutOfService);
                }

            }
        }
    }
}