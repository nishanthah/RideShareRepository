using DriverLocator.Models;
using RideShare.ViewModels;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using RideShare.Common;
using GoogleApiClient.Maps;
using Common.Models;

namespace RideShare
{
    public partial class MapView : ContentPage,IMapPageProcessor
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        CustomMap map;
        IMapSocketService mapSocketService;
        SendRequestViewModel sendRequestViewModel;

        public MapView()
        {
            Init();
            LoadUserData();
            //RenderLine();
        }

        public MapView(NotificationInfo notificationInfo)
        {
            Init();
            ShowNotificationInMap(notificationInfo);
        }

        private void Init()
        {
            InitializeComponent();
            mapSocketService = DependencyService.Get<IMapSocketService>();
            sendRequestViewModel = new SendRequestViewModel(this, driverLocatorService);
            requestArea.BindingContext = sendRequestViewModel;
            InitMap();
            mapSocketService.MapCoordinateChanged += mapSocketService_MapCoordinateChanged;
        }
        void mapSocketService_MapCoordinateChanged(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                LoadUserData();
            });
        }

        private void LoadUserData()
        {
            //var userCoordinates = driverLocatorService.ViewUserCoordinates();
            //ShowOnLabels(userCoordinates.UserCoordinates);

            var drivers = driverLocatorService.GetDrivers();
            var rider = App.CurrentLoggedUser;
            var currentRiders = new List<UserLocation>();
            currentRiders.Add(rider);
            ShowOnMap(drivers.UserLocations);
            ShowOnMap(currentRiders);
        }

        //private List<UserLocation> TestSeed()
        //{
        //    var userLocations = new List<UserLocation>();
            
        //}
        private void ShowOnLabels(List<UserLocation> userCoordinates)
        {
            messageLabel.Text = "";
            foreach (var userCorrdinate in userCoordinates)
            {
                messageLabel.Text += userCorrdinate.User.UserName + "|" + userCorrdinate.User.EMail + "|" + userCorrdinate.User.FirstName + "|" + userCorrdinate.Location.Longitude + "|" + userCorrdinate.Location.Latitude + Environment.NewLine;
            }
        }

        private void ShowNotificationInMap(NotificationInfo notificationInfo)
        {
            map.Pins.Clear();
            map.CustomPins.Clear();
            RenderPin(notificationInfo.Source.Longitude.ToString(), notificationInfo.Source.Latitude.ToString(), notificationInfo.Source.LocationName,UserType.Driver);
            RenderPin(notificationInfo.Destination.Longitude.ToString(), notificationInfo.Destination.Latitude.ToString(), notificationInfo.Destination.LocationName,UserType.Rider);
            RenderLine(notificationInfo.Source, notificationInfo.Destination);
        }

        private void ShowOnMap(List<UserLocation> userCoordinates)
        {

            foreach (var item in userCoordinates)
            {
                RenderPin(item.Location.Longitude, item.Location.Latitude, item.User.FirstName + " " + item.User.LastName + " | Position : " + item.Location.Longitude + " , " + item.Location.Latitude,item.User.UserType);
            }

        }

        private void InitMap()
        {
            map = new CustomMap
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            mapContainer.Children.Clear();
            mapContainer.Children.Add(map);
        }

        private void RenderPin(string longitudeCoordinate, string latitudeCoordinate, string lable,UserType userType)
        {
            double latitude = 0;
            double longitude = 0;

            double.TryParse(latitudeCoordinate, out latitude);
            double.TryParse(longitudeCoordinate, out longitude);

            var position = new Position(latitude, longitude);

            var pin = new CustomPin
            {
                Pin = new Pin
                {
                    Type = PinType.Place,
                    Position = position,
                    Label = lable
                },
                Title = lable,
                UserType = userType
            };

            map.CustomPins.Add(pin);
            
            map.Pins.Add(pin.Pin);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromMiles(60)));
        }

        private void RenderLine(Coordinate sourceCoordinate, Coordinate destinationCoordinate)
        {            
            GoogleMapsDirectionsClient googleMapsDirectionsClient = new GoogleMapsDirectionsClient();
            var source = new GoogleApiClient.Models.Coordinate() { Latitude = sourceCoordinate.Latitude, Longitude = sourceCoordinate.Longitude };
            var destination = new GoogleApiClient.Models.Coordinate() { Latitude = destinationCoordinate.Latitude, Longitude = destinationCoordinate.Longitude };

            //var source = new GoogleApiClient.Models.Coordinate() { Latitude = 7.087310, Longitude = 80.014366 };
            //var destination = new GoogleApiClient.Models.Coordinate() { Latitude = 7.209709, Longitude = 79.842796 };
            var directions  = googleMapsDirectionsClient.GetDirections(new GoogleApiClient.Models.GetDirectionRequest() { DestinationCoordinate = destination, SourceCoordinate = source });

            foreach(var route in directions.Routes)
            {
                foreach(var coordinates in route.OverViewPolyLine.DecodedOverViewPolyLine)
                {
                    map.RouteCoordinates.Add(new Position(coordinates.Latitude, coordinates.Longitude));
                }
            }
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(source.Latitude, source.Longitude), Distance.FromMiles(1.0)));
        }
    }
}
