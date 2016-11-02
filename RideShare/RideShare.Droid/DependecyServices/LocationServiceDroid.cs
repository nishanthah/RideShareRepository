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
using Common;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;
using Java.Lang;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.LocationServiceDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class LocationServiceDroid : Java.Lang.Object, ILocationService,GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener,IDisposable,IResultCallback
    {
        string tag = "LocationServiceDroid";
        //LocationManager locationManger;
        GoogleApiClient apiClient;
        LocationRequest locRequest;
        Location location;

        public bool IsGPSAvailable
        {
            get
            {
                LocationManager lm = (LocationManager)MainApp.Context.GetSystemService(Context.LocationService);
                return lm.IsProviderEnabled(LocationManager.GpsProvider);
            }

        }

        public LocationServiceDroid()
        {
           
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

        public void OnConnected(Bundle connectionHint)
        {
            location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);

            locRequest = new LocationRequest();
            locRequest.SetPriority(100);
            locRequest.SetFastestInterval(500);
            locRequest.SetInterval(1000);

            LocationServices.FusedLocationApi.RequestLocationUpdates(apiClient, locRequest, this);
        }

        public void OnConnectionSuspended(int cause)
        {
            //throw new NotImplementedException();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Session.IsGPSEnabled = false;
        }

        public new void Dispose()
        {
            base.Dispose();
            if (apiClient.IsConnected)
            {
                LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, this);
                apiClient.Disconnect();
            }
        }

        public void StartLocationService()
        {
            apiClient = new GoogleApiClient.Builder(MainApp.Context, this, this).AddApi(LocationServices.API).Build();

            //locRequest = new LocationRequest();
            //locRequest.SetPriority(100);
            //locRequest.SetFastestInterval(500);
            //locRequest.SetInterval(1000);

            //LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder()
            //    .AddLocationRequest(locRequest);

            //builder.SetNeedBle(false);
            Session.IsGPSEnabled= IsGPSAvailable;
            apiClient.Connect();
            //PendingResult result = LocationServices.SettingsApi.CheckLocationSettings(apiClient, builder.Build());
            //result.SetResultCallback(this);
        }

        public void OnResult(Java.Lang.Object result)
        {
            Statuses status = ((LocationSettingsResult)result).Status;
            LocationSettingsStates  locationSettingStatus= ((LocationSettingsResult)result).LocationSettingsStates;
            //switch (status.StatusCode)
            //{
            //    case LocationSettingsStatusCodes.Success:
            //     break;
            //    case LocationSettingsStatusCodes.ResolutionRequired:
            //        // Location settings are not satisfied. But could be fixed by showing the user
            //        // a dialog.
            //        //try
            //        //{
            //        //    // Show the dialog by calling startResolutionForResult(),
            //        //    // and check the result in onActivityResult().
            //        //    //status.StartResolutionForResult(
            //        //    //    OuterClass.this,
            //        //    //    RequesCode);
            //        //}
            //        //catch (SendIntentException e)
            //        //{
            //        //    // Ignore the error.
            //        //}
            //        Session.IsGPSEnabled = false;
            //        break;
            //    case LocationSettingsStatusCodes.SettingsChangeUnavailable:
            //        Session.IsGPSEnabled = false;
            //     break;
            //}
        }
    }

    

    
}