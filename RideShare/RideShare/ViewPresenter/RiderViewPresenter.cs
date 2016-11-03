using DriverLocator.Models;
using GoogleApiClient.Helpers;
using GoogleApiClient.Models;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace RideShare.ViewPresenter
{
    public class RiderViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService;

        public RiderViewPresenter(IMapPageProcessor mapPageProcessor,IMapSocketService mapSocketService, DriverLocator.DriverLocatorService driverLocatorService) :base(mapPageProcessor,mapSocketService,driverLocatorService)
        {
            this.driverLocatorService = driverLocatorService;
            base.InitDestination();
            RefreshPins(true);
            base.OnInitializationCompleted();
        }

        bool isRideRequested = false;
        string rideHistoryId = String.Empty;

        protected override List<CustomPin> LoadPinData()
        {
           if(isRideRequested)
            {
                return LoadOnlyRideData();
            }
           else
            {
                return LoadAllData();
            }
        }

        private List<CustomPin> LoadAllData()
        {
            List<CustomPin> mapPins= new List<CustomPin>();
            var availableDrivers = driverLocatorService.GetDrivers().Result.UserLocations.Where(x=>x.User.IsLoggedIn == true);

            foreach (var driver in availableDrivers)
            {
                if(driver.Location.Latitude != null && driver.Location.Longitude != null)
                {
                    MapPin pin = new MapPin();
                    pin.ImageIcon = "userLogActive_icon.png";
                    pin.Latitude = double.Parse(driver.Location.Latitude);
                    pin.Longitude = double.Parse(driver.Location.Longitude);
                    pin.PhoneNo = !String.IsNullOrEmpty(driver.User.MobileNo) ? driver.User.MobileNo : "Not Set";
                    pin.Title = String.Format("{0} {1} | Destination={2}",
                                    driver.User.FirstName,
                                    driver.User.LastName,
                                    driver.Destination.Name != null ? driver.Destination.Name : "Not Set");

                    pin.UserName = driver.User.UserName;
                    pin.UserType = driver.User.UserType;
                    mapPins.Add(GetFromatted(pin));
                }
                
            }

            if (App.CurrentLoggedUser.Location.Latitude != null && App.CurrentLoggedUser.Location.Longitude != null)
            {
                var riderPin = new MapPin();
                riderPin.ImageIcon = "userLogActive_icon.png";
                riderPin.Latitude = double.Parse(App.CurrentLoggedUser.Location.Latitude);
                riderPin.Longitude = double.Parse(App.CurrentLoggedUser.Location.Longitude);
                riderPin.PhoneNo = !String.IsNullOrEmpty(App.CurrentLoggedUser.User.MobileNo) ? App.CurrentLoggedUser.User.MobileNo : "Not Set";
                riderPin.Title = App.CurrentLoggedUser.User.FirstName + " " + App.CurrentLoggedUser.User.LastName;
                riderPin.UserName = App.CurrentLoggedUser.User.UserName;
                riderPin.UserType = App.CurrentLoggedUser.User.UserType;
                mapPins.Add(GetFromatted(riderPin));
            }
            return mapPins;
        }

        private List<CustomPin> LoadOnlyRideData()
        {
            List<CustomPin> mapPins = new List<CustomPin>();
            var notification = driverLocatorService.GetRideHistoryByFilter("_id", rideHistoryId).RideHistories.FirstOrDefault();
            var driver = driverLocatorService.GetSelectedUserCoordinate(notification.DiverUserName).UserLocation;
            var rider = driverLocatorService.GetSelectedUserCoordinate(notification.UserName).UserLocation;

            Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude), Longitude = double.Parse(driver.Location.Longitude) };
            Coordinate riderCoordinate = new Coordinate() { Latitude = double.Parse(rider.Location.Latitude), Longitude = double.Parse(rider.Location.Longitude) };

            IList<Leg> legs = null;

            var directions = GetDirections(driverCoordinate, riderCoordinate);

            if (directions.Routes != null && directions.Routes.Count > 0)
            {
                legs = directions.Routes.First().Legs;
            }

            // Create driver pin
            MapPin driverPin = new MapPin();
            driverPin.ImageIcon = "userLogActive_icon.png";
            driverPin.Latitude = double.Parse(driver.Location.Latitude);
            driverPin.Longitude = double.Parse(driver.Location.Longitude);
            driverPin.PhoneNo = !String.IsNullOrEmpty(driver.User.MobileNo) ? driver.User.MobileNo : "Not Set";

            driverPin.Title = String.Format("{0} {1} | Destination={2} | ({3} {4})",
                                    driver.User.FirstName,
                                    driver.User.LastName,
                                    driver.Destination.Name != null ? driver.Destination.Name : "Not Set",
                                    legs != null ? legs.Sum(x => x.Distance.Value).ToKilometers() : String.Empty,
                                    legs != null ? legs.Sum(x => x.Duration.Value).ToTimeString() : String.Empty);

            driverPin.UserName = driver.User.UserName;
            driverPin.UserType = driver.User.UserType;

            if(driver.User.IsLoggedIn)
            {
                mapPins.Add(GetFromatted(driverPin));
            }
            

            // Create rider pin
            var riderPin = new MapPin();
            riderPin.ImageIcon = "userLogActive_icon.png";
            riderPin.Latitude = double.Parse(rider.Location.Latitude);
            riderPin.Longitude = double.Parse(rider.Location.Longitude);
            riderPin.PhoneNo = !String.IsNullOrEmpty(rider.User.MobileNo) ? rider.User.MobileNo : "Not Set";

            riderPin.Title = String.Format("{0} {1}",
                                    rider.User.FirstName,
                                    rider.User.LastName);

            riderPin.UserName = rider.User.UserName;
            riderPin.UserType = rider.User.UserType;
            mapPins.Add(GetFromatted(riderPin));

            return mapPins;
        }

        protected override void OnMapInfoWindowClicked(CustomPin customPin)
        {
            mapPageProcessor.ShowDoubleButtonPopup("Are you sure you want to send the pickup request to this driver?","Send Request","Cancel",this.SendRideRequest,this.DismissPopup);
        }

        private void DismissPopup()
        {
            mapPageProcessor.HideDoubleButtonPopupBox();
        }

        private void SendRideRequest()
        {
            RideHistory rideHistory = new RideHistory();
            rideHistory.UserName = Session.CurrentUserName;
            rideHistory.DiverUserName = mapPageProcessor.SelectedPin.UserName;
            rideHistory.SourceName = String.Format("{0}(Lat = {1}, Lng = {2}", String.Empty, mapPageProcessor.SelectedPin.Pin.Position.Latitude, mapPageProcessor.SelectedPin.Pin.Position.Longitude);
            rideHistory.SourceLongitude = mapPageProcessor.SelectedPin.Pin.Position.Longitude.ToString();
            rideHistory.SourceLatitude = mapPageProcessor.SelectedPin.Pin.Position.Latitude.ToString();

            //rideHistory.DestinationName = String.Format("{0}(Lat = {1}, Lng = {2}", String.Empty, App.CurrentLoggedUser.Location.Latitude, App.CurrentLoggedUser.Location.Longitude);
            //rideHistory.DestinationLongitude = App.CurrentLoggedUser.Location.Longitude;
            //rideHistory.DestinationLatitude = App.CurrentLoggedUser.Location.Latitude;

            if(mapPageProcessor.SelectedDestination!=null)
            {
                rideHistory.DestinationName = String.Format("{0}(Lat = {1}, Lng = {2}", mapPageProcessor.SelectedDestination.LocationName, mapPageProcessor.SelectedDestination.Latitude, mapPageProcessor.SelectedDestination.Longitude);
                rideHistory.DestinationLongitude = mapPageProcessor.SelectedDestination.Longitude.ToString();
                rideHistory.DestinationLatitude = mapPageProcessor.SelectedDestination.Latitude.ToString();
            }
            else
            {
                mapPageProcessor.ShowInfoWindowPopupBox(new InfoWindowContent() { Title = "Information", Description = "Please set the destination" });
                return;
            }

            var result = driverLocatorService.CreateHistory(rideHistory);
            if (result.IsSuccess)
            {
                mapPageProcessor.HideDoubleButtonPopupBox();
                isRideRequested = true;
                rideHistoryId = result.RequestId;
                RefreshPins(true);
            }
        }

        private CustomPin GetFromatted(MapPin mapPin)
        {
            var position = new Position(mapPin.Latitude, mapPin.Longitude);

            var pin = new CustomPin
            {
                Pin = new Pin
                {
                    Type = PinType.Place,
                    Position = position,
                    Label = mapPin.Title,
                },
                Title = mapPin.Title,
                UserType = mapPin.UserType,
                MobileNo = "Mobile No:" + mapPin.PhoneNo,
                Image = "profile_images/" + mapPin.ImageIcon,
                UserName = mapPin.UserName,
                Id = mapPin.UserName
            };
            return pin;
        }

        protected override void OnNewCoordinatesRecived()
        {
            RefreshPins(false);
        }

    }
}
