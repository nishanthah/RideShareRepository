using DriverLocator.Models;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.ViewPresenter
{
    public class RiderViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);

        public RiderViewPresenter(IMapPageProcessor mapPageProcessor,IMapSocketService mapSocketService):base(mapPageProcessor,mapSocketService)
        {
            RefreshPins(true);
        }

        bool isRideRequested = false;
        string rideHistoryId = String.Empty;

        protected override void LoadPinData()
        {
           if(isRideRequested)
            {
                LoadOnlyRideData();
            }
           else
            {
                LoadAllData();
            }
        }

        private void LoadAllData()
        {
            mapPageProcessor.MapPins = new List<CustomPin>();
            var drivers = driverLocatorService.GetDrivers().Result;
            foreach (var driver in drivers.UserLocations)
            {
                MapPin pin = new MapPin();
                pin.ImageIcon = "userLogActive_icon.png";
                pin.Latitude = double.Parse(driver.Location.Latitude);
                pin.Longitude = double.Parse(driver.Location.Longitude);
                pin.PhoneNo = driver.User.MobileNo;
                pin.Title = driver.User.FirstName + " " + driver.User.LastName + " | Position : " + driver.Location.Longitude + " , " + driver.Location.Latitude;
                pin.UserName = driver.User.UserName;
                pin.UserType = driver.User.UserType;
                mapPageProcessor.AddPin(pin);
            }

            var riderPin = new MapPin();
            riderPin.ImageIcon = "userLogActive_icon.png";
            riderPin.Latitude = double.Parse(App.CurrentLoggedUser.Location.Latitude);
            riderPin.Longitude = double.Parse(App.CurrentLoggedUser.Location.Longitude);
            riderPin.PhoneNo = App.CurrentLoggedUser.User.MobileNo;
            riderPin.Title = App.CurrentLoggedUser.User.FirstName + " " + App.CurrentLoggedUser.User.LastName + " | Position : " + App.CurrentLoggedUser.Location.Longitude + " , " + App.CurrentLoggedUser.Location.Latitude;
            riderPin.UserName = App.CurrentLoggedUser.User.UserName;
            riderPin.UserType = App.CurrentLoggedUser.User.UserType;
            mapPageProcessor.AddPin(riderPin);
        }

        private void LoadOnlyRideData()
        {
            mapPageProcessor.MapPins = new List<CustomPin>();
            var notification = driverLocatorService.GetRideHistoryByFilter("_id", rideHistoryId).RideHistories.FirstOrDefault();
            var driver = driverLocatorService.GetSelectedUserCoordinate(notification.DiverUserName).UserLocation;
            var rider = driverLocatorService.GetSelectedUserCoordinate(notification.UserName).UserLocation;

            Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude), Longitude = double.Parse(driver.Location.Longitude) };
            Coordinate riderCoordinate = new Coordinate() { Latitude = double.Parse(rider.Location.Latitude), Longitude = double.Parse(rider.Location.Longitude) };

            var directions = GetDirections(driverCoordinate, riderCoordinate).Routes.SingleOrDefault().Legs.SingleOrDefault();

            // Create driver pin
            MapPin driverPin = new MapPin();
            driverPin.ImageIcon = "userLogActive_icon.png";
            driverPin.Latitude = double.Parse(driver.Location.Latitude);
            driverPin.Longitude = double.Parse(driver.Location.Longitude);
            driverPin.PhoneNo = driver.User.MobileNo;

            driverPin.Title = String.Format("{0} {1} | Lat={2},Lng={3} | ({4} {5})",
                                    driver.User.FirstName,
                                    driver.User.LastName,
                                    driver.Location.Latitude,
                                    driver.Location.Longitude,
                                    directions.Distance.Text,
                                    directions.Duration.Text);

            driverPin.UserName = driver.User.UserName;
            driverPin.UserType = driver.User.UserType;
            mapPageProcessor.AddPin(driverPin);

            // Create rider pin
            var riderPin = new MapPin();
            riderPin.ImageIcon = "userLogActive_icon.png";
            riderPin.Latitude = double.Parse(rider.Location.Latitude);
            riderPin.Longitude = double.Parse(rider.Location.Longitude);
            riderPin.PhoneNo = rider.User.MobileNo;

            riderPin.Title = String.Format("{0} {1} | Lat={2},Lng={3} | ({4} {5})",
                                    rider.User.FirstName,
                                    rider.User.LastName,
                                    rider.Location.Latitude,
                                    rider.Location.Longitude,
                                    directions.Distance.Text,
                                    directions.Duration.Text);

            riderPin.UserName = rider.User.UserName;
            riderPin.UserType = rider.User.UserType;

            mapPageProcessor.AddPin(riderPin);
        }

        protected override void OnMapInfoWindowClicked(CustomPin customPin)
        {
            mapPageProcessor.ShowPopupBox("Are you sure you want to send the pickup request to this driver?");
        }

        protected override void OnPopupCanceled()
        {
            mapPageProcessor.HidePopupBox();
        }

        protected override void OnPopupConfirmed()
        {
            RideHistory rideHistory = new RideHistory();
            rideHistory.UserName = Session.CurrentUserName;
            rideHistory.DiverUserName = mapPageProcessor.SelectedPin.UserName;
            rideHistory.SourceName = String.Format("{0}(Lat = {1}, Lng = {2}", String.Empty, mapPageProcessor.SelectedPin.Pin.Position.Latitude, mapPageProcessor.SelectedPin.Pin.Position.Longitude);
            rideHistory.SourceLongitude = mapPageProcessor.SelectedPin.Pin.Position.Longitude.ToString();
            rideHistory.SourceLatitude = mapPageProcessor.SelectedPin.Pin.Position.Latitude.ToString();
            rideHistory.DestinationName = String.Format("{0}(Lat = {1}, Lng = {2}", String.Empty, App.CurrentLoggedUser.Location.Latitude, App.CurrentLoggedUser.Location.Longitude);
            rideHistory.DestinationLongitude = App.CurrentLoggedUser.Location.Longitude;
            rideHistory.DestinationLatitude = App.CurrentLoggedUser.Location.Latitude;
            var result = driverLocatorService.CreateHistory(rideHistory);
            if (result.IsSuccess)
            {
                mapPageProcessor.HidePopupBox();
                isRideRequested = true;
                rideHistoryId = result.RequestId;
                RefreshPins(true);
            }
        }

        protected override void OnNewCoordinatesRecived()
        {
            RefreshPins(false);
        }

    }
}
