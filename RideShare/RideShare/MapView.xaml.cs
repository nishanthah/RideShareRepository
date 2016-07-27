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
using GoogleApiClient.Models;
using Android.OS;
using Android.Util;

namespace RideShare
{
    public class RouteResult
    {
        private RideHistory rideHistoryItem;

        public RideHistory RideHistoryItem
        {
            get { return rideHistoryItem; }
            set { rideHistoryItem = value; }
        }

        private DriverLocator.Models.Location riderLocation;

        public DriverLocator.Models.Location RiderLocation
        {
            get { return riderLocation; }
            set { riderLocation = value; }
        }

        private List<Position> routeCoordinates;

        public List<Position> RouteCoordinates
        {
            get { return routeCoordinates; }
            set { routeCoordinates = value; }
        }

        public RouteResult()
        {
            routeCoordinates = new List<Position>();
            rideHistoryItem = new RideHistory();
            RiderLocation = new DriverLocator.Models.Location();
        }
    }

    public partial class MapView : ContentPage,IMapPageProcessor,ILocationSelectionResult
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        CustomMap map;
        CustomPin selectedPin;
        IMapSocketService mapSocketService;
        SendRequestViewModel sendRequestViewModel;
        IBaseUrl baseResource;
        List<CustomPin> customPins;
        List<Position> routeCorrdinates;

        public Action<LocationSearchResult> OnLocationSelected { get; set; }
        static NotificationInfo notificationInfo;

        UserLocationResponse driversResponse;
        UserLocationResponse riderResponse;
        IAppDataService appData;
        bool ShowTestSimulator = false;
        bool isDriverNotificationAccepted = false;
        public MapView()
        {
            Init();
           
            LoadUserData(true);
            //RenderLine();
        }

        public MapView(NotificationInfo notificationInfoData)
        {
            isDriverNotificationAccepted = true;
            Init();
            notificationInfo = notificationInfoData;
            LoadDriversAndRiders();
            ShowRoute();

            //Java.Lang.Thread t = new Java.Lang.Thread(() => {
            //    while (true)
            //    {
            //        ShowRoute();
            //        Java.Lang.Thread.Sleep(5000);
            //    }

            //});
            ////t.Start();
            //ShowNotificationInMap(notificationInfo);
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
            appData = DependencyService.Get<IAppDataService>();

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
                appData.Save("selected_pin_username", rideHistory.DiverUserName);
                DisplayAlert("Success", "Successfully sent the notification to driver", "Ok");
                //this.ShowNotificationPopup("Successfully sent the notification to driver");
                //var notificator = DependencyService.Get<IToastNotificator>();
                //notificator.Notify(ToastNotificationType.Success, "Success", "Successfully sent the notification to driver", TimeSpan.FromSeconds(3));
            }
        }

        private void IsolateSourceAndDestination()
        {

        }

        private void CancelPopupButton_Clicked(object sender, EventArgs e)
        {
            HidePopupBox();
        }

        void mapSocketService_MapCoordinateChanged(object sender, EventArgs e)
        {
            if(isDriverNotificationAccepted)
            {
                ShowRoute();
            }
            else
            {
                LoadUserData(false);
            }
            //Java.Lang.Thread t = new Java.Lang.Thread(() => {              
                   
            //});
            //t.Start();
            //Device.BeginInvokeOnMainThread(() =>
            //{
           
           // });
            
        }

        void OnLocationSelecteResult(LocationSearchResult result)
        {
            destinationText.Text =String.Format("{0} (Lat={1}, Lng={2})",result.LocationName,result.Latitude,result.Longitude);
        }

        private async void LoadDriversAndRiders()
        {
            driversResponse = await driverLocatorService.GetDrivers();
            riderResponse = await driverLocatorService.GetRiders();
        }

        private void LoadUserData(bool canMoveToLocation)
        {
            Task.Factory.StartNew(() =>{
                LoadDriversAndRiders();

            }).ContinueWith((task)=> {
                Device.BeginInvokeOnMainThread(() =>
                {
                    map.Pins.Clear();
                    map.CustomPins.Clear();
                    customPins = new List<CustomPin>();

                    var rider = App.CurrentLoggedUser;
                    var currentRiders = new List<UserLocation>();
                    currentRiders.Add(rider);
                    ShowOnMap(driversResponse.UserLocations, canMoveToLocation);
                    ShowOnMap(currentRiders, canMoveToLocation);
                    map.CustomPins = customPins;

                    if (canMoveToLocation)
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(double.Parse(App.CurrentLoggedUser.Location.Latitude), double.Parse(App.CurrentLoggedUser.Location.Longitude)), Xamarin.Forms.Maps.Distance.FromKilometers(2)));
                    }
                });
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


        private void ShowRoute()
        {

            Task<RouteResult>.Factory.StartNew(GetRouteResult).ContinueWith((task) => {
                
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        map.Pins.Clear();
                        map.CustomPins = new List<CustomPin>();
                        map.RouteCoordinates = new List<Position>();
                        customPins = new List<CustomPin>();

                        var data = task.Result;
                        var riderLocation = data.RiderLocation;
                        var rideHistoryItem = data.RideHistoryItem;
                        var routeCoordinates = data.RouteCoordinates;

                        AddPin(riderLocation.Longitude,
                            riderLocation.Latitude,
                            false,
                            String.Format("Lng={0},Lat={1}",
                            riderLocation.Longitude,
                            riderLocation.Latitude),
                            UserType.Rider,
                            String.Empty,
                            "userLogActive_icon.png",
                            rideHistoryItem.UserName);

                        AddPin(App.CurrentLoggedUser.Location.Longitude, App.CurrentLoggedUser.Location.Latitude, false, String.Format("Lng={0},Lat={1}", App.CurrentLoggedUser.Location.Longitude, App.CurrentLoggedUser.Location.Latitude), UserType.Driver, String.Empty, "driverLogActive_icon.png", App.CurrentLoggedUser.User.UserName);
                        map.CustomPins = customPins;
                        map.RouteCoordinates = routeCoordinates;
                    }
                    catch(Exception ex)
                    {

                    }
                });
            });
            
        }

        private RouteResult GetRouteResult()
        {
            try
            {
                LoadDriversAndRiders();
                RouteResult routeResult = new RouteResult();
                //RouteResult routeResult = new RouteResult();
                routeResult.RideHistoryItem = driverLocatorService.GetRideHistoryByFilter("_id", notificationInfo.RequestId).RideHistories.FirstOrDefault();

                routeResult.RiderLocation = riderResponse.UserLocations.Find(x => x.User.UserName == routeResult.RideHistoryItem.UserName).Location;


                Coordinate destinationCoordinate = new Coordinate() { Latitude = double.Parse(routeResult.RiderLocation.Latitude), Longitude = double.Parse(routeResult.RiderLocation.Longitude) };
                Coordinate sourceCoordinate = new Coordinate() { Latitude = double.Parse(App.CurrentLoggedUser.Location.Latitude), Longitude = double.Parse(App.CurrentLoggedUser.Location.Longitude) };


                routeResult.RouteCoordinates = GetLineCoordinates(sourceCoordinate, destinationCoordinate);


                return routeResult;
            }
           catch(Exception ex)
            {
                throw ex;
            }
        }
        //private void ShowRoute()
        //{
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        map.Pins.Clear();
        //        map.CustomPins = new List<CustomPin>();
        //        map.RouteCoordinates = new List<Position>();
        //        customPins = new List<CustomPin>();

        //        var driverCurrentLocation = driversResponse.UserLocations.Find(x => x.User.UserName == appData.Get("selectedPin.UserName")).Location;


        //        RenderPin(notificationInfo.Source.Longitude.ToString(), notificationInfo.Source.Latitude.ToString(), true, notificationInfo.Source.LocationName, UserType.Driver, String.Empty, "userLogActive_icon.png", String.Empty);
        //        RenderPin(notificationInfo.Destination.Longitude.ToString(), notificationInfo.Destination.Latitude.ToString(), true, notificationInfo.Destination.LocationName, UserType.Rider, String.Empty, "driverLogActive_icon.png", String.Empty);

        //        map.CustomPins = customPins;
        //        RenderLine(notificationInfo.Source, notificationInfo.Destination);
        //        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(double.Parse(App.CurrentLoggedUser.Location.Latitude), double.Parse(App.CurrentLoggedUser.Location.Longitude)), Distance.FromKilometers(2)));


        //    });
        //}

        private void ShowOnMap(List<UserLocation> userCoordinates,bool canMoveToLocation)
        {
            if(userCoordinates!=null)
            {
                foreach (var item in userCoordinates)
                {
                    //if(item.User.UserName != App.CurrentLoggedUser.User.UserName)
                    //{
                        AddPin(item.Location.Longitude, item.Location.Latitude, canMoveToLocation, item.User.FirstName + " " + item.User.LastName + " | Position : " + item.Location.Longitude + " , " + item.Location.Latitude, item.User.UserType, item.User.MobileNo, "userLogActive_icon.png", item.User.UserName);
                    //}
                    
                }
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

        private void AddPin(string longitudeCoordinate, string latitudeCoordinate,bool canMoveToLoacation, string lable,UserType userType,string mobileNo,string image,string userName)
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

            Device.BeginInvokeOnMainThread(() =>
            {
                map.Pins.Add(pin.Pin);
            });
            customPins.Add(pin);
   
        }


        private List<Position> GetLineCoordinates(Coordinate sourceCoordinate, Coordinate destinationCoordinate)
        {
            List<Position> routeCoordinates = new List<Position>();
            GoogleMapsDirectionsClient googleMapsDirectionsClient = new GoogleMapsDirectionsClient();
            var source = new GoogleApiClient.Models.Coordinate() { Latitude = sourceCoordinate.Latitude, Longitude = sourceCoordinate.Longitude };
            var destination = new GoogleApiClient.Models.Coordinate() { Latitude = destinationCoordinate.Latitude, Longitude = destinationCoordinate.Longitude };

            //var source = new GoogleApiClient.Models.Coordinate() { Latitude = 7.087310, Longitude = 80.014366 };
            //var destination = new GoogleApiClient.Models.Coordinate() { Latitude = 7.209709, Longitude = 79.842796 };
            var directions = googleMapsDirectionsClient.GetDirections(new GoogleApiClient.Models.GetDirectionRequest() { DestinationCoordinate = destination, SourceCoordinate = source });
            //map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(source.Latitude, source.Longitude), Distance.FromKilometers(10)));
            foreach (var route in directions.Routes)
            {
                foreach (var coordinates in route.OverViewPolyLine.DecodedOverViewPolyLine)
                {
                    routeCoordinates.Add(new Position(coordinates.Latitude, coordinates.Longitude));
                }
            }

            return routeCoordinates;
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
