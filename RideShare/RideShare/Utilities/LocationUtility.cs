
using Android.OS;
using Android.Util;
using DriverLocator.Models;
using Java.Lang;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.Utilities
{
    public class LocationUtility
    {
        ILocationService locService;
        private string TAG = typeof(LocationUtility).Name;
        public LocationUtility(ILocationService locationService)
        {
            this.locService = locationService;
        }

        public void UpdateCurrentLocation()
        {
            IAppDataService appDataService = DependencyService.Get<IAppDataService>();

            var currentUser = appDataService.Get("current_user");

            var location = locService.GetCurrentLocation();

            if (!System.String.IsNullOrEmpty(currentUser) && location != null)
            {
                DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                
                UpdateUserLocationRequest request = new UpdateUserLocationRequest();
                request.Latitude = location.Latitude;
                request.Longitude = location.Longitude;

                var isUserLocationUpdated = App.CurrentLoggedUser != null
                                                && double.Parse(App.CurrentLoggedUser.Location.Latitude) != location.Latitude
                                                && double.Parse(App.CurrentLoggedUser.Location.Longitude) != location.Longitude;
                if (isUserLocationUpdated)
                {
                    var result = driverLocatorService.UpdateUserLocation(currentUser, request);
                    if (result.IsSuccess)
                    {
                        try
                        {

                            App.CurrentLoggedUser.Location.Latitude = location.Latitude.ToString();
                            App.CurrentLoggedUser.Location.Longitude = location.Longitude.ToString();
                            //Log.Debug(TAG, System.String.Format("Updated App User Location : User = {2}, Lat = {0}, Lng = {1}", location.Latitude, location.Longitude, currentUser));
                        }
                        catch (System.Exception ex)
                        {
                            //Log.Debug(TAG, System.String.Format("Skipped Update App User Location : User = {2}, Lat = {0}, Lng = {1}", location.Latitude, location.Longitude, currentUser));
                        }
                    }
                }
                else
                {
                   // Log.Debug(TAG, System.String.Format("Location not changed and not updated user location : User = {2}, Lat = {0}, Lng = {1}", location.Latitude, location.Longitude, currentUser));
                }

            }
            else
            {
                //Log.Debug(TAG, "User not logged in for update location");
            }
            
        }

        
    }
}
