using DriverLocator.Models;
using GoogleApiClient.Maps;
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
    public class RiderNavigationViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService;
        RideHistory rideHistory;
        public RiderNavigationViewPresenter(IMapPageProcessor mapPageProcessor,IMapSocketService mapSocketService, RideHistory rideHistory, DriverLocator.DriverLocatorService driverLocatorService) :base(mapPageProcessor,mapSocketService,driverLocatorService)
        {
            this.driverLocatorService = driverLocatorService;           
            this.rideHistory = rideHistory;
            base.InitDestination();
            RefreshPins(true);
            if (rideHistory.RequestStatus == RequestStatus.DriverAccepted)
            {
                var infoWindowText = String.Format("{0} accepted your request to {1}", rideHistory.DiverUserName, rideHistory.DestinationName);
                mapPageProcessor.ShowInfoWindowPopupBox(new InfoWindowContent() { Description = infoWindowText, Title = "Ride Request Accepted" });
            }
            else if (rideHistory.RequestStatus == RequestStatus.DriverRejected)
            {
                var infoWindowText = String.Format("{0} rejected your request to {1}", rideHistory.DiverUserName, rideHistory.DestinationName);
                mapPageProcessor.ShowInfoWindowPopupBox(new InfoWindowContent() { Description = infoWindowText, Title = "Ride Request Rejected" });
            }
            base.OnInitializationCompleted();
        }

        protected override List<CustomPin> LoadPinData()
        {
            List<CustomPin> mapPins = new List<CustomPin>();
            var driver = driverLocatorService.GetSelectedUserCoordinate(rideHistory.DiverUserName).UserLocation;
            var rider = driverLocatorService.GetSelectedUserCoordinate(rideHistory.UserName).UserLocation;

            Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude),Longitude = double.Parse(driver.Location.Longitude) };
            Coordinate riderCoordinate = new Coordinate() { Latitude = double.Parse(rider.Location.Latitude), Longitude = double.Parse(rider.Location.Longitude) };

            var directions = GetDirections(driverCoordinate, riderCoordinate).Routes.First().Legs.First();

            // Create driver pin
            MapPin driverPin = new MapPin();
            driverPin.ImageIcon = "userLogActive_icon.png";
            driverPin.Latitude = double.Parse(driver.Location.Latitude);
            driverPin.Longitude = double.Parse(driver.Location.Longitude);
            driverPin.PhoneNo = driver.User.MobileNo;

            driverPin.Title =String.Format("{0} {1} | Destination={2} | ({3} {4})", 
                                    driver.User.FirstName,
                                    driver.User.LastName,
                                    rider.Destination.Name != null ? rider.Destination.Name : "Not Set",
                                    directions.Distance.Text,
                                    directions.Duration.Text);

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

            mapPins.Add(GetFromatted(riderPin));
            return mapPins;
        }
   
        protected override void OnMapInfoWindowClicked(CustomPin customPin)
        {

        }
        
        protected override void OnNewCoordinatesRecived()
        {
            RefreshPins(false);
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

    }
}
