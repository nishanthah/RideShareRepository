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
using Android.Locations;
using Org.Xml.Sax;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.LocationServiceDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class LocationServiceDroid : ILocationService,ILocationListener
    {
        LocationManager locationManger;
        string locationProvider;
        public LocationServiceDroid()
        {
            locationManger = (LocationManager)MainApp.Context.GetSystemService(Context.LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = locationManger.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }
        }

        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public  CommonModels.Location GetCurrentLocation()
        {
            throw new NotImplementedException();
        }

        public void OnLocationChanged(Location location)
        {
            throw new NotImplementedException();
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }
    }
}