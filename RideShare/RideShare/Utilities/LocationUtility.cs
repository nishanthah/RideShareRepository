
using DriverLocator.Models;
using Java.Lang;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.Utilities
{
    public class LocationUtility
    {
       
        public static Location GetCurrentLocation()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 5;
            var position = locator.GetPositionAsync(timeoutMilliseconds: 10000);
            return  new Location() { Latitude = position.Result.Latitude.ToString(), Longitude = position.Result.Longitude.ToString() };            
        }

        public static void UpdateCurrentLocation()
        {
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 5;
            var position = locator.GetPositionAsync(timeoutMilliseconds: 10000);

            UpdateUserLocationRequest request = new UpdateUserLocationRequest();
            request.Latitude = position.Result.Latitude;
            request.Longitude = position.Result.Longitude;
            var result = driverLocatorService.UpdateUserLocation(App.CurrentLoggedUser.User.UserName, request);
            if(result.IsSuccess)
            {                 
                App.CurrentLoggedUser.Location.Latitude = position.Result.Latitude.ToString();
                App.CurrentLoggedUser.Location.Longitude = position.Result.Longitude.ToString();
            }
            
        }

        
    }
}
