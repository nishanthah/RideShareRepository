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
using Plugin.Toasts;
using System.Collections.ObjectModel;

namespace RideShare
{
    public partial class MapView : ContentPage,IMapPageProcessor,ILocationSelectionResult
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        CustomMap map;
        CustomPin selectedPin;
        IMapSocketService mapSocketService;
        SendRequestViewModel sendRequestViewModel;
        IBaseUrl baseResource;
        List<CustomPin> customPins;
        public Action<LocationSearchResult> OnLocationSelected { get; set; }


        bool ShowTestSimulator = false;
        public MapView()
        {
            Init();
           
            LoadUserData(true);
            //RenderLine();
        }

        public MapView(NotificationInfo notificationInfo)
        {
            Init();
            ShowNotificationInMap(notificationInfo);
        }

        async void OnInfoWindowClicked(CustomPin pin)
        {
            //var answer = await DisplayAlert("Select Driver?", "Do you want to select this driver?", "Yes", "No");
            //if(answer)
            //{
            //messageLabel.Text = pin.Id.ToString();
            //}
            selectedPin = pin;
            ShowPopupBox();
        }

        private void Init()
        {
            InitializeComponent();

            InitPopup();
            notificationPopup.BackgroundColor = new Color(0, 0, 0, 0.5);
           
            mapSocketService = DependencyService.Get<IMapSocketService>();
            baseResource = DependencyService.Get<IBaseUrl>();
            
            destinationSelector.GestureRecognizers.Add(new TapGestureRecognizer(OnTapDestinationSelector));

            //if (ShowTestSimulator)
            //{
            //    simulatorView.IsVisible = true;
            //}
            //else
            //{
            //    simulatorView.IsVisible = false;
            //}
            //sendRequestViewModel = new SendRequestViewModel(this, driverLocatorService);
            //requestArea.BindingContext = sendRequestViewModel;
            InitMap();
            mapSocketService.MapCoordinateChanged += mapSocketService_MapCoordinateChanged;
            cancelPopupButton.Clicked += CancelPopupButton_Clicked;
            this.OnLocationSelected = OnLocationSelecteResult;

        }

        private void InitPopup()
        {
            sendingPopupForeground.BackgroundColor = new Color(0, 0, 0, 0.5);
            sendRequestButon.Clicked += SendRequestButon_Clicked;
        }

        private void SendRequestButon_Clicked(object sender, EventArgs e)
        {
            RideHistory rideHistory = new RideHistory();
            rideHistory.UserName = Session.CurrentUserName;
            rideHistory.DiverUserName = selectedPin.UserName;
            rideHistory.SourceName = String.Format("{0}(Lat = {1}, Lng = {2}", String.Empty, selectedPin.Pin.Position.Latitude, selectedPin.Pin.Position.Longitude);
            rideHistory.SourceLongitude = selectedPin.Pin.Position.Longitude.ToString();
            rideHistory.SourceLatitude = selectedPin.Pin.Position.Latitude.ToString() ;
            rideHistory.DestinationName = String.Format("{0}(Lat = {1}, Lng = {2}", String.Empty, App.CurrentLoggedUser.Location.Latitude, App.CurrentLoggedUser.Location.Longitude);
            rideHistory.DestinationLongitude = App.CurrentLoggedUser.Location.Longitude;
            rideHistory.DestinationLatitude = App.CurrentLoggedUser.Location.Latitude;
            var result = driverLocatorService.CreateHistory(rideHistory);
            if (result.IsSuccess)
            {
                HidePopupBox();
                DisplayAlert("Success", "Successfully sent the notification to driver", "Ok");
                //this.ShowNotificationPopup("Successfully sent the notification to driver");
                //var notificator = DependencyService.Get<IToastNotificator>();
                //notificator.Notify(ToastNotificationType.Success, "Success", "Successfully sent the notification to driver", TimeSpan.FromSeconds(3));
            }
        }

        private void CancelPopupButton_Clicked(object sender, EventArgs e)
        {
            HidePopupBox();
        }

        void mapSocketService_MapCoordinateChanged(object sender, EventArgs e)
        {
            //Device.BeginInvokeOnMainThread(() =>
            //{
                LoadUserData(false);
           // });
            
        }

        void OnLocationSelecteResult(LocationSearchResult result)
        {
            destinationText.Text =String.Format("{0} (Lat={1}, Lng={2})",result.LocationName,result.Latitude,result.Longitude);
        }

        private void LoadUserData(bool canMoveToLocation)
        {

            //var userCoordinates = driverLocatorService.ViewUserCoordinates();
            //ShowOnLabels(userCoordinates.UserCoordinates);
            Device.BeginInvokeOnMainThread(() =>
            {
                map.Pins.Clear();
                map.CustomPins.Clear();
                customPins= new List<CustomPin>() ;
                var drivers = driverLocatorService.GetDrivers();
                var rider = App.CurrentLoggedUser;
                var currentRiders = new List<UserLocation>();
                currentRiders.Add(rider);
                ShowOnMap(drivers.UserLocations, canMoveToLocation);
                ShowOnMap(currentRiders, canMoveToLocation);
                map.CustomPins = customPins;

                if (canMoveToLocation)
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(double.Parse(App.CurrentLoggedUser.Location.Latitude), double.Parse(App.CurrentLoggedUser.Location.Longitude)), Distance.FromKilometers(2)));
                }

            });
           
        }

        //private List<UserLocation> TestSeed()
        //{
        //    var userLocations = new List<UserLocation>();
            
        //}
        private void ShowOnLabels(List<UserLocation> userCoordinates)
        {
            //messageLabel.Text = "";
            //foreach (var userCorrdinate in userCoordinates)
            //{
            //    messageLabel.Text += userCorrdinate.User.UserName + "|" + userCorrdinate.User.EMail + "|" + userCorrdinate.User.FirstName + "|" + userCorrdinate.Location.Longitude + "|" + userCorrdinate.Location.Latitude + Environment.NewLine;
            //}
        }

        private void ShowNotificationInMap(NotificationInfo notificationInfo)
        {
            map.Pins.Clear();
            map.CustomPins = new List<CustomPin>();
            RenderPin(notificationInfo.Source.Longitude.ToString(), notificationInfo.Source.Latitude.ToString(),true, notificationInfo.Source.LocationName,UserType.Driver, String.Empty, "userLogActive_icon.png",String.Empty);
            RenderPin(notificationInfo.Destination.Longitude.ToString(), notificationInfo.Destination.Latitude.ToString(),true, notificationInfo.Destination.LocationName,UserType.Rider,String.Empty, "driverLogActive_icon.png",String.Empty);
            RenderLine(notificationInfo.Source, notificationInfo.Destination);
        }

        private void ShowOnMap(List<UserLocation> userCoordinates,bool canMoveToLocation)
        {

            foreach (var item in userCoordinates)
            {
                RenderPin(item.Location.Longitude, item.Location.Latitude, canMoveToLocation, item.User.FirstName + " " + item.User.LastName + " | Position : " + item.Location.Longitude + " , " + item.Location.Latitude,item.User.UserType,item.User.MobileNo, "userLogActive_icon.png",item.User.UserName);
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
            map.OnInfoWindowClicked = OnInfoWindowClicked;
        }

        private void RenderPin(string longitudeCoordinate, string latitudeCoordinate,bool canMoveToLoacation, string lable,UserType userType,string mobileNo,string image,string userName)
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
                UserName = userName,
                Id = Guid.NewGuid()
            };

            //map.CustomPins.Add(pin);
            map.Pins.Add(pin.Pin);
            customPins.Add(pin);
   
        }

        private void RenderLine(Coordinate sourceCoordinate, Coordinate destinationCoordinate)
        {            
            GoogleMapsDirectionsClient googleMapsDirectionsClient = new GoogleMapsDirectionsClient();
            var source = new GoogleApiClient.Models.Coordinate() { Latitude = sourceCoordinate.Latitude, Longitude = sourceCoordinate.Longitude };
            var destination = new GoogleApiClient.Models.Coordinate() { Latitude = destinationCoordinate.Latitude, Longitude = destinationCoordinate.Longitude };

            //var source = new GoogleApiClient.Models.Coordinate() { Latitude = 7.087310, Longitude = 80.014366 };
            //var destination = new GoogleApiClient.Models.Coordinate() { Latitude = 7.209709, Longitude = 79.842796 };
            var directions  = googleMapsDirectionsClient.GetDirections(new GoogleApiClient.Models.GetDirectionRequest() { DestinationCoordinate = destination, SourceCoordinate = source });
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(source.Latitude, source.Longitude), Distance.FromKilometers(10)));
            foreach (var route in directions.Routes)
            {
                foreach(var coordinates in route.OverViewPolyLine.DecodedOverViewPolyLine)
                {
                    map.RouteCoordinates.Add(new Position(coordinates.Latitude, coordinates.Longitude));
                }
            }
            
        }

        private void ShowPopupBox()
        {
            //sendingPopupBack.IsVisible = true;
            sendingPopupForeground.IsVisible = true;
        }

        private void HidePopupBox()
        {
            //sendingPopupBack.IsVisible = false;
            sendingPopupForeground.IsVisible = false;
        }

    }
}
