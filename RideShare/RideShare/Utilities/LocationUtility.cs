using DriverLocator.Models;
using Newtonsoft.Json;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CommonModels = Common.Models;

namespace RideShare.Utilities
{

    public static class LocationUtilityExtentions
    {
        public static string ToJson(this List<CommonModels.Location> locations)
        {
            return JsonConvert.SerializeObject(locations);
        }

        public static string ToEncodedPolyLine(this List<CommonModels.Location> coordinates)
        {
            int plat = 0;
            int plng = 0;
            System.Text.StringBuilder encodedCoordinates = new System.Text.StringBuilder();
            if (coordinates != null)
            {
                foreach (CommonModels.Location coordinate in coordinates)
                {
                    if (coordinate != null)
                    {
                        // Round to 5 decimal places and drop the decimal
                        int late5 = (int)(coordinate.Latitude * 1e5);
                        int lnge5 = (int)(coordinate.Longitude * 1e5);
                        // Encode the differences between the coordinates
                        encodedCoordinates.Append(EncodeSignedNumber(late5 - plat));
                        encodedCoordinates.Append(EncodeSignedNumber(lnge5 - plng));
                        // Store the current coordinates
                        plat = late5;
                        plng = lnge5;
                    }

                }
            }
            return encodedCoordinates.ToString();
        }

        /// <summary>
        /// Encode a signed number in the encode format.
        /// </summary>
        /// <param name="num">The signed number</param>
        /// <returns>The encoded string</returns>
        private static string EncodeSignedNumber(int num)
        {
            int sgn_num = num << 1; //shift the binary value
            if (num < 0) //if negative invert
            {
                sgn_num = ~(sgn_num);
            }
            return (EncodeNumber(sgn_num));
        }

        /// <summary>
        /// Encode an unsigned number in the encode format.
        /// </summary>
        /// <param name="num">The unsigned number</param>
        /// <returns>The encoded string</returns>
        private static string EncodeNumber(int num)
        {
            System.Text.StringBuilder encodeString = new System.Text.StringBuilder();
            while (num >= 0x20)
            {
                encodeString.Append((char)((0x20 | (num & 0x1f)) + 63));
                num >>= 5;
            }
            encodeString.Append((char)(num + 63));
            // All backslashes needs to be replaced with double backslashes
            // before being used in a Javascript string.
            return encodeString.ToString().Replace(@"\", @"\\");
        }

    }
    public class LocationUtility
    {
        ILocationService locService;
        IAppDataService appDataService;
        private string TAG = typeof(LocationUtility).Name;


        public LocationUtility(ILocationService locationService)
        {
            RoutePoints = new List<CommonModels.Location>();
            appDataService = DependencyService.Get<IAppDataService>();
            this.locService = locationService;
        }

        public static List<CommonModels.Location> RoutePoints { get; set; }

        public void UpdateCurrentLocation()
        {
            try
            {
                var currentUser = appDataService.Get("current_user");

                var location = locService.GetCurrentLocation();

                if (!System.String.IsNullOrEmpty(currentUser) && location != null)
                {
                    DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);

                    UpdateUserLocationRequest request = new UpdateUserLocationRequest();
                    request.Latitude = location.Latitude;
                    request.Longitude = location.Longitude;

                    bool canUpdateLocation = false;



                    var isUserLocationUpdated = App.CurrentLoggedUser != null
                                                    && App.CurrentLoggedUser.Location.Latitude != null
                                                    && App.CurrentLoggedUser.Location.Longitude != null
                                                    && double.Parse(App.CurrentLoggedUser.Location.Latitude) != location.Latitude
                                                    && double.Parse(App.CurrentLoggedUser.Location.Longitude) != location.Longitude;

                    var isLocationNull = App.CurrentLoggedUser != null
                                          && App.CurrentLoggedUser.Location.Latitude == null
                                          && App.CurrentLoggedUser.Location.Longitude == null;

                    if (isLocationNull || isUserLocationUpdated)
                    {
                        canUpdateLocation = true;
                    }

                    if (canUpdateLocation)
                    {

                        var result = driverLocatorService.UpdateUserLocation(currentUser, request);
                        if (result.IsSuccess)
                        {


                            App.CurrentLoggedUser.Location.Latitude = location.Latitude.ToString();
                            App.CurrentLoggedUser.Location.Longitude = location.Longitude.ToString();
                            //Log.Debug(TAG, System.String.Format("Updated App User Location : User = {2}, Lat = {0}, Lng = {1}", location.Latitude, location.Longitude, currentUser));

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
            catch (System.Exception ex)
            {
                //Log.Debug(TAG, System.String.Format("Skipped Update App User Location : User = {2}, Lat = {0}, Lng = {1}", location.Latitude, location.Longitude, currentUser));
            }

        }

        public void AddHistoryLocation()
        {
            try
            {
                DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                var currentUser = appDataService.Get("current_user");
                var currentUserLocation = driverLocatorService.GetSelectedUserCoordinate(currentUser).UserLocation;

                if (currentUser != null && currentUserLocation != null)
                {
                    if (currentUserLocation.User.UserType == CommonModels.UserType.Rider)
                    {
                        var recentRequest = currentUserLocation.User.RecentRequest;
                        if (recentRequest != null)
                        {
                            var currentRequestStatus = driverLocatorService.GetRideHistoryByFilter("_id", recentRequest).RideHistories.SingleOrDefault().RequestStatus;

                            // Add to local list
                            if (currentRequestStatus == RequestStatus.RiderMet)
                            {
                                var currentLocation = locService.GetCurrentLocation();
                                if (currentLocation != null)
                                {
                                    AddToCurrentPoints(locService.GetCurrentLocation());
                                }

                            }

                            // Send to server
                            else if (currentRequestStatus == RequestStatus.RideCompleted)
                            {
                                UpdatePolylineRequest request = new UpdatePolylineRequest();
                                var polyLine = GetCurrentPoints().ToEncodedPolyLine();
                                if (!String.IsNullOrEmpty(polyLine))
                                {
                                    request.PolyLine = GetCurrentPoints().GroupBy(x => x).Select(x => x.First()).ToList().ToEncodedPolyLine();
                                    driverLocatorService.UpdatePolyline(recentRequest, request);
                                    appDataService.Save("user_rideing_points", null);
                                }

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public List<CommonModels.Location> GetCurrentPoints()
        {

            var ridingPoints = appDataService.Get("user_rideing_points");
            if (ridingPoints != null)
            {
                return JsonConvert.DeserializeObject<List<CommonModels.Location>>(ridingPoints);
            }
            return null;
        }

        public void AddToCurrentPoints(CommonModels.Location location)
        {
            var currentPoints = GetCurrentPoints();
            if (currentPoints != null)
            {
                currentPoints.Add(location);

            }
            else
            {
                currentPoints = new List<CommonModels.Location>();
                currentPoints.Add(location);
            }

            appDataService.Save("user_rideing_points", JsonConvert.SerializeObject(currentPoints));
        }


    }
}
