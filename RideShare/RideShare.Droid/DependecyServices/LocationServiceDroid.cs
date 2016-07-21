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
using CommonModels = Common.Models;
using RideShare.SharedInterfaces;
using Org.Xml.Sax;
using System.Threading.Tasks;
using Android.Locations;
using Android.Util;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.LocationServiceDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class LocationServiceDroid : Java.Lang.Object,ILocationService,ILocationListener
    {
        string tag = "LocationServiceDroid";
        LocationManager locationManger;
        Location location;
        
        public LocationServiceDroid()
        {
            locationManger = (LocationManager)Application.Context.GetSystemService(Context.LocationService);
            locationManger.RequestLocationUpdates(LocationManager.GpsProvider, 5000, 1, this);
        }
  
        public  CommonModels.Location GetCurrentLocation()
        {
            if(location != null)
            {
                return new CommonModels.Location() { Latitude = location.Latitude, Longitude = location.Longitude };
            }
            return null;
        }

        public void OnLocationChanged(Location location)
        {
            this.location = location;
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Debug(tag, provider + " disabled by user");
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Debug(tag, provider + " enabled by user");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug(tag, provider + " availability has changed to " + status.ToString());
        }
    }
}