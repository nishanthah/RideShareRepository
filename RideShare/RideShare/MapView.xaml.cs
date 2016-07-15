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
    public partial class MapView : ContentPage,IMapPageProcessor,ILocationSelectionResult
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        CustomMap map;
        IMapSocketService mapSocketService;
        SendRequestViewModel sendRequestViewModel;
        IBaseUrl baseResource;
        public Action<LocationSearchResult> OnLocationSelected { get; set; }


        bool ShowTestSimulator = false;
        public MapView()
        {
            Init();
            map.OnInfoWindowClicked = OnInfoWindowClicked;
            LoadUserData();
            //RenderLine();
        }

        public MapView(NotificationInfo notificationInfo)
        {
            Init();
            ShowNotificationInMap(notificationInfo);
        }

        async void OnInfoWindowClicked(CustomPin pin)
        {
            var answer = await DisplayAlert("Select Driver?", "Do you want to select this driver?", "Yes", "No");
            if(answer)
            {
                messageLabel.Text = pin.Id.ToString();
            }
        }

        private void Init()
        {
            InitializeComponent();
            mapSocketService = DependencyService.Get<IMapSocketService>();
            baseResource = DependencyService.Get<IBaseUrl>();
            this.OnLocationSelected = OnLocationSelecteResult;
            destinationSelector.GestureRecognizers.Add(new TapGestureRecognizer(OnTapDestinationSelector));

            if (ShowTestSimulator)
            {
                simulatorView.IsVisible = true;
            }
            else
            {
                simulatorView.IsVisible = false;
            }
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

        void OnLocationSelecteResult(LocationSearchResult result)
        {
            destinationSelector.Text =String.Format("Your Destination : {0} (Lat={1}, Lng={2})",result.LocationName,result.Latitude,result.Longitude);
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
            RenderPin(notificationInfo.Source.Longitude.ToString(), notificationInfo.Source.Latitude.ToString(), notificationInfo.Source.LocationName,UserType.Driver, "", "userLogActive_icon.png");
            RenderPin(notificationInfo.Destination.Longitude.ToString(), notificationInfo.Destination.Latitude.ToString(), notificationInfo.Destination.LocationName,UserType.Rider,"", "driverLogActive_icon.png");
            RenderLine(notificationInfo.Source, notificationInfo.Destination);
        }

        private void ShowOnMap(List<UserLocation> userCoordinates)
        {

            foreach (var item in userCoordinates)
            {
                RenderPin(item.Location.Longitude, item.Location.Latitude, item.User.FirstName + " " + item.User.LastName + " | Position : " + item.Location.Longitude + " , " + item.Location.Latitude,item.User.UserType,item.User.MobileNo, "userLogActive_icon.png");
            }

        }

        void OnTapDestinationSelector(View sender, object e)
        {
            //App.Current.MainPage = new NavigationPage(new LocationSearch());
            Navigation.PushAsync(new LocationSearch(this));
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

        private void RenderPin(string longitudeCoordinate, string latitudeCoordinate, string lable,UserType userType,string mobileNo,string image)
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
                    Label = lable,
                },
                Title = lable,
                UserType = userType,
                MobileNo = "Mobile No:" + mobileNo,
                Image = "profile_images/" + image,
                Id = Guid.NewGuid()
            };

            map.CustomPins.Add(pin);
            
            map.Pins.Add(pin.Pin);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromKilometers(3)));
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
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(source.Latitude, source.Longitude), Distance.FromMiles(50)));
        }
    }
}
