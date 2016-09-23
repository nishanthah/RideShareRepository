using DriverLocator.Models;
using GoogleApiClient.Models;
using GoogleApiClient.Helpers;
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
    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class DriverNavigationViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService;
        RideHistory rideHistory;
        NotificationInfo notificationInfo;
        RequestStatus currentStatus;

        public DriverNavigationViewPresenter(IMapPageProcessor mapPageProcessor, IMapSocketService mapSocketService, NotificationInfo notificationInfo, RideHistory rideHistory, DriverLocator.DriverLocatorService driverLocatorService) : base(mapPageProcessor, mapSocketService, driverLocatorService)
        {
            this.driverLocatorService = driverLocatorService;
            this.notificationInfo = notificationInfo;
            this.rideHistory = rideHistory;
            base.InitDestination();
            currentStatus = rideHistory.RequestStatus;
            RefreshRoute(true);

            if (rideHistory.RequestStatus == RequestStatus.Requested)
            {
               
                mapPageProcessor.ShowDoubleButtonPopup("Are you want to accept this ride?", "Yes", "No",this.UpdateToDriverAccept,this.UpdateToDiverReject);
            }

           
        }


        public DriverNavigationViewPresenter(IMapPageProcessor mapPageProcessor, IMapSocketService mapSocketService, RideHistory rideHistory, DriverLocator.DriverLocatorService driverLocatorService) : base(mapPageProcessor, mapSocketService,driverLocatorService)
        {
            this.driverLocatorService = driverLocatorService;
            this.rideHistory = rideHistory;
            base.InitDestination();
            currentStatus = rideHistory.RequestStatus;
            

            if (rideHistory.RequestStatus == RequestStatus.Requested)
            {
                Task.Factory.StartNew<string>(() =>
                {
                    var driver = driverLocatorService.GetSelectedUserCoordinate(rideHistory.DiverUserName).UserLocation;
                    var rider = driverLocatorService.GetSelectedUserCoordinate(rideHistory.UserName).UserLocation;

                    Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude), Longitude = double.Parse(driver.Location.Longitude) };
                    Coordinate riderCoordinate = new Coordinate() { Latitude = double.Parse(rider.Location.Latitude), Longitude = double.Parse(rider.Location.Longitude) };

                    Coordinate driverDestinationCoordinate = null;
                    if (driver.Destination.Latitude != null)
                    {
                        driverDestinationCoordinate = new Coordinate() { Latitude = double.Parse(driver.Destination.Latitude), Longitude = double.Parse(driver.Destination.Longitude) };
                    }
                    List<Coordinate> riderWaypoint = new List<Coordinate>() { riderCoordinate };

                    var directions = GetDirections(driverCoordinate, driverDestinationCoordinate, riderWaypoint);
                    var route = directions.Routes.SingleOrDefault();
                    var leg = directions.Routes.SingleOrDefault().Legs.SingleOrDefault();

                    var message = String.Format("Distance to destination:{0}\nTime to Destination:{1}\nAre you want to accept this ride?", leg.Distance.Text, leg.Duration.Text);
                    return message;

                }).ContinueWith((task) =>
                {
                    RefreshRoute(true);
                    mapPageProcessor.ShowDoubleButtonPopup(task.Result, "Yes", "No", this.UpdateToDriverAccept, this.UpdateToDiverReject);
                    base.OnInitializationCompleted();
                });
            }

            else if (rideHistory.RequestStatus == RequestStatus.DriverAccepted)
            {
                RefreshRoute(true);
                mapPageProcessor.ShowDoubleButtonPopup("Are you want to set this as pickedup?", "Yes", "No", this.UpdateToDriverPickedUp, () => { mapPageProcessor.HideDoubleButtonPopupBox(); });
                base.OnInitializationCompleted();
            }

            else if (rideHistory.RequestStatus == RequestStatus.RiderMet)
            {
                RefreshRoute(true);
                mapPageProcessor.ShowDoubleButtonPopup("Are you sure you want to end this ride?", "Yes", "No", this.UpdateToDriverRideEnded, () => { mapPageProcessor.HideDoubleButtonPopupBox(); });
                base.OnInitializationCompleted();
            }
            

        }

        
        protected override RouteData LoadRouteData()
        {
            RouteData routeData = new RouteData();

            var driver = driverLocatorService.GetSelectedUserCoordinate(rideHistory.DiverUserName).UserLocation;
            var rider = driverLocatorService.GetSelectedUserCoordinate(rideHistory.UserName).UserLocation;

            Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude), Longitude = double.Parse(driver.Location.Longitude) };
            Coordinate riderCoordinate = new Coordinate() { Latitude = double.Parse(rider.Location.Latitude), Longitude = double.Parse(rider.Location.Longitude) };

            Coordinate driverDestinationCoordinate = null;

            if (driver.Destination.Latitude != null)
            {
                driverDestinationCoordinate = new Coordinate() { Latitude = double.Parse(driver.Destination.Latitude), Longitude = double.Parse(driver.Destination.Longitude) };
            }

            List<Coordinate> riderWaypoint = new List<Coordinate>() { riderCoordinate };
            var directions = GetDirections(driverCoordinate, driverDestinationCoordinate, riderWaypoint);
            var route = directions.Routes.SingleOrDefault();
            var legs = directions.Routes[0].Legs;

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
                                    legs.Sum(x=>x.Distance.Value).ToKilometers(),
                                    legs.Sum(x => x.Duration.Value).ToTimeString());

            driverPin.UserName = driver.User.UserName;
            driverPin.UserType = driver.User.UserType;

            routeData.SourcePin = driverPin;
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
                                    legs.Sum(x => x.Distance.Value).ToKilometers(),
                                    legs.Sum(x => x.Duration.Value).ToTimeString());

            riderPin.UserName = rider.User.UserName;
            riderPin.UserType = rider.User.UserType;

            routeData.DestinationPin = riderPin;
            routeData.RouteCoordinates = GetRouteCoordinates(directions.Routes.SingleOrDefault());
            return routeData;
        }

        protected override List<CustomPin> LoadPinData()
        {
            List<CustomPin> coordinates = new List<CustomPin>();
            var driver = driverLocatorService.GetSelectedUserCoordinate(rideHistory.DiverUserName).UserLocation;
            Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude), Longitude = double.Parse(driver.Location.Longitude) };
            Coordinate destinationCoordinate = new Coordinate() { Latitude = double.Parse(rideHistory.DestinationLatitude), Longitude = double.Parse(rideHistory.DestinationLongitude) };

            var leg = GetDirections(driverCoordinate, destinationCoordinate).Routes.SingleOrDefault().Legs.SingleOrDefault();

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
            coordinates.Add(GetFromatted(driverPin));
            return coordinates;
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
  
        private void NavigateToRiderView()
        {
            mapPageProcessor.NavigateToRiderView();
        }

        private void UpdateToDriverPickedUp()
        {
            var isSuccess = driverLocatorService.UpdateRideHistoryStatus(new UpdateRideHistoryRequest() { Id = rideHistory.Id, Status = RequestStatus.RiderMet }).IsSuccess;

            if (isSuccess)
            {
                mapPageProcessor.HideDoubleButtonPopupBox();
            }
        }

        private void UpdateToDriverRideEnded()
        {
            var isSuccess = driverLocatorService.UpdateRideHistoryStatus(new UpdateRideHistoryRequest() { Id = rideHistory.Id, Status = RequestStatus.RideCompleted }).IsSuccess;

            if (isSuccess)
            {
                mapPageProcessor.HideDoubleButtonPopupBox();
            }
        }

        private void UpdateToDriverAccept()
        {
            var isSuccess = driverLocatorService.UpdateRideHistoryStatus(new UpdateRideHistoryRequest() { Id = rideHistory.Id, Status = RequestStatus.DriverAccepted }).IsSuccess;

            if (isSuccess)
            {
                mapPageProcessor.HideDoubleButtonPopupBox();
                mapPageProcessor.LoadCurrentStatus("Set As PickedUp");
            }
        }

        private void UpdateToDiverReject()
        {
            var isSuccess = driverLocatorService.UpdateRideHistoryStatus(new UpdateRideHistoryRequest() { Id = rideHistory.Id, Status = RequestStatus.DriverRejected }).IsSuccess;

            if (isSuccess)
            {
                mapPageProcessor.NavigateToRiderView();
            }
        }

        protected override void OnNewCoordinatesRecived()
        {
            if (currentStatus == RequestStatus.RiderMet)
            {
                RefreshPins(false);
            }
            else 
            {
                RefreshRoute(false);
            }
        }

        /// <summary>
        /// Encodes the list of coordinates to a Google Maps encoded coordinate string.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns>Encoded coordinate string</returns>
        public static string EncodeCoordinates(List<Coordinate> coordinates)
        {
            int plat = 0;
            int plng = 0;
            System.Text.StringBuilder encodedCoordinates = new System.Text.StringBuilder();
            foreach (Coordinate coordinate in coordinates)
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

        List<Coordinate> coordinates = new List<Coordinate>();
        protected override void OnNewStatusChanged()
        {            
            IAppDataService appDataService = DependencyService.Get<IAppDataService>();

            var userData = driverLocatorService.GetSelectedUserCoordinate(appDataService.Get("current_user"));

            var historyInfo = driverLocatorService.GetRideHistoryByFilter("_id", userData.UserLocation.User.RecentRequest).RideHistories.FirstOrDefault();

            if (historyInfo.RequestStatus == RequestStatus.DriverAccepted)
            {
                var isSuccess = driverLocatorService.UpdateRideHistoryStatus(new UpdateRideHistoryRequest() { Id = historyInfo.Id, Status = RequestStatus.RiderMet }).IsSuccess;
                currentStatus = RequestStatus.RiderMet;
                mapPageProcessor.LoadCurrentStatus("Set As Ride Completed");

                //Update Polyline
                //Task t = Task.Run(() =>
                //{
                //    var currentUser = appDataService.Get("current_user");
                //    ILocationService locService = DependencyService.Get<ILocationService>();
                //    var location = locService.GetCurrentLocation();
                //    if (!System.String.IsNullOrEmpty(currentUser) && location != null)
                //    {
                //        Coordinate _Coordinate = new Coordinate()
                //        {
                //            Latitude = location.Latitude,
                //            Longitude = location.Longitude
                //        };
                //        coordinates.Add(_Coordinate);
                //    }
                //});          

            }

            if (historyInfo.RequestStatus == RequestStatus.RiderMet)
            {
                var isSuccess = driverLocatorService.UpdateRideHistoryStatus(new UpdateRideHistoryRequest() { Id = historyInfo.Id, Status = RequestStatus.RideCompleted }).IsSuccess;
                currentStatus = RequestStatus.RideCompleted;
                mapPageProcessor.NavigateToRiderView();

                string history = EncodeCoordinates(coordinates);
                driverLocatorService.UpdatePolyline(appDataService.Get("appDataService"), new DriverLocator.Models.UpdatePolylineRequest() { PolyLine = history });
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


    }
}
