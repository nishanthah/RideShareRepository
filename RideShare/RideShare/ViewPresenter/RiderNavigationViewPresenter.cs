using DriverLocator.Models;
using GoogleApiClient.Helpers;
using GoogleApiClient.Maps;
using GoogleApiClient.Models;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace RideShare.ViewPresenter
{
    public class RiderNavigationViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService;
        RideHistory rideHistory;
        IAppDataService appDataService;

        public RiderNavigationViewPresenter(IMapPageProcessor mapPageProcessor,IMapSocketService mapSocketService, RideHistory rideHistory, DriverLocator.DriverLocatorService driverLocatorService) :base(mapPageProcessor,mapSocketService,driverLocatorService)
        {
            this.driverLocatorService = driverLocatorService;           
            this.rideHistory = rideHistory;
            this.appDataService = DependencyService.Get<IAppDataService>();
            base.InitDestination();
            RefreshPins(true);
            if (rideHistory.RequestStatus == RequestStatus.DriverAccepted)
            {
                var infoWindowText = String.Format("{0} accepted your request to {1}", rideHistory.DiverUserName, rideHistory.DestinationName);

                if (appDataService.Get(rideHistory.Id + rideHistory.RequestStatus.ToString()) != "closed")
                {
                    mapPageProcessor.ShowInfoWindowPopupBox(new InfoWindowContent() { Description = infoWindowText, Title = "Ride Request Accepted" }, () =>
                    {
                        appDataService.Save(rideHistory.Id + rideHistory.RequestStatus.ToString(), "closed");
                    });
                }
                
            }
            else if (rideHistory.RequestStatus == RequestStatus.DriverRejected)
            {
                var infoWindowText = String.Format("{0} rejected your request to {1}", rideHistory.DiverUserName, rideHistory.DestinationName);
                if (appDataService.Get(rideHistory.Id + rideHistory.RequestStatus.ToString()) != "closed")
                {
                    mapPageProcessor.ShowInfoWindowPopupBox(new InfoWindowContent() { Description = infoWindowText, Title = "Ride Request Rejected" }, () =>
                    {
                        appDataService.Save(rideHistory.Id + rideHistory.RequestStatus.ToString(), "closed");
                    });
                }

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

            IList<Leg> legs = null;

            
            var directions = GetDirections(driverCoordinate, riderCoordinate);

            if(directions.Routes != null && directions.Routes.Count > 0)
            {
                legs = directions.Routes.First().Legs;
            }
            // Create driver pin
            MapPin driverPin = new MapPin();
            driverPin.ImageIcon = "userLogActive_icon.png";
            driverPin.Latitude = double.Parse(driver.Location.Latitude);
            driverPin.Longitude = double.Parse(driver.Location.Longitude);
            driverPin.PhoneNo = !String.IsNullOrEmpty(driver.User.MobileNo) ? driver.User.MobileNo : "Not Set"; 

            driverPin.Title =String.Format("{0} {1} | Destination={2} | ({3} {4})", 
                                    driver.User.FirstName,
                                    driver.User.LastName,
                                    rider.Destination.Name != null ? rider.Destination.Name : "Not Set",
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
            riderPin.PhoneNo = !String.IsNullOrEmpty(rider.User.MobileNo) ? rider.User.MobileNo : "Not Set"; ;

            riderPin.Title = String.Format("{0} {1} | ({2} {3})",
                                    rider.User.FirstName,
                                    rider.User.LastName,
                                    legs != null ? legs.Sum(x => x.Distance.Value).ToKilometers() : String.Empty,
                                    legs != null ? legs.Sum(x => x.Duration.Value).ToTimeString() : String.Empty);

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
