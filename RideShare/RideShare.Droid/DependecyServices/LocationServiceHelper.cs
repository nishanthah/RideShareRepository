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
using RideShare.SharedInterfaces;
using Android.Locations;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.LocationServiceHelper))]
namespace RideShare.Droid.DependecyServices
{
    public class LocationServiceHelper : ILocationServiceHelper
    {
        public bool IsGPSAvailable
        {
            get
            {
                LocationManager lm = (LocationManager)MainApp.Context.GetSystemService(Context.LocationService);
                return lm.IsProviderEnabled(LocationManager.GpsProvider);
            }
        }

        public void ShowLocationSettings()
        {
            Xamarin.Forms.Forms.Context.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
        }
    }
}