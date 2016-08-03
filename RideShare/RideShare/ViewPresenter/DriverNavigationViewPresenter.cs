using DriverLocator.Models;
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
    public class DriverNavigationViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        RideHistory rideHistory;

        public DriverNavigationViewPresenter(IMapPageProcessor mapPageProcessor,IMapSocketService mapSocketService, RideHistory rideHistory):base(mapPageProcessor,mapSocketService)
        {
            this.rideHistory = rideHistory;
            //RefreshRoute(true);
        }

        protected override RouteData LoadRouteData()
        {
            RouteData routeData = new RouteData();
            
            var driver = driverLocatorService.GetSelectedUserCoordinate(rideHistory.DiverUserName).UserLocation;
            var rider = driverLocatorService.GetSelectedUserCoordinate(rideHistory.UserName).UserLocation;

            Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude), Longitude = double.Parse(driver.Location.Longitude) };
            Coordinate riderCoordinate = new Coordinate() { Latitude = double.Parse(rider.Location.Latitude), Longitude = double.Parse(rider.Location.Longitude) };

            var directions = GetDirections(driverCoordinate, riderCoordinate);
            var route = directions.Routes.SingleOrDefault();
            var leg = directions.Routes.SingleOrDefault().Legs.SingleOrDefault();

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
                                    leg.Distance.Text,
                                    leg.Duration.Text);

            driverPin.UserName = driver.User.UserName;
            driverPin.UserType = driver.User.UserType;

            routeData.SourcePin = driverPin;          
            // Create rider pin
            var riderPin = new MapPin();
            riderPin.ImageIcon = "userLogActive_icon.png";
            riderPin.Latitude = double.Parse(rider.Location.Latitude);
            riderPin.Longitude = double.Parse(rider.Location.Longitude);
            riderPin.PhoneNo = rider.User.MobileNo;

            driverPin.Title = String.Format("{0} {1} | Lat={2},Lng={3} | ({4} {5})",
                                    rider.User.FirstName,
                                    rider.User.LastName,
                                    rider.Location.Latitude,
                                    rider.Location.Longitude,
                                    leg.Distance.Text,
                                    leg.Duration.Text);

            riderPin.UserName = rider.User.UserName;
            riderPin.UserType = rider.User.UserType;

            routeData.DestinationPin = riderPin;
            routeData.RouteCoordinates = GetRouteCoordinates(GetDirections(driverCoordinate, riderCoordinate).Routes.SingleOrDefault());
            return routeData;
        }

        private List<Position> GetRouteCoordinates(Route route)
        {
            List<Position> routeCoordinates = new List<Position>();

            foreach (var coordinates in route.OverViewPolyLine.DecodedOverViewPolyLine)
            {
                    routeCoordinates.Add(new Position(coordinates.Latitude, coordinates.Longitude));
            }
        
            return routeCoordinates;
        }

        protected override void OnMapInfoWindowClicked(CustomPin customPin)
        {
           
        }

        protected override void OnPopupCanceled()
        {

        }

        protected override void OnPopupConfirmed()
        {
            
        }

        protected override void OnNewCoordinatesRecived()
        {
            //RefreshRoute(false);
        }

    }
}
