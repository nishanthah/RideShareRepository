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
using RideShare.ViewPresenter;

namespace RideShare
{
    public class RouteData
    {
        public MapPin SourcePin { get; set; }
        public MapPin DestinationPin { get; set; }
        public List<Position> RouteCoordinates { get; set; } 
    }


    public class MapPin
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; }
        public string PhoneNo { get; set; }
        public string ImageIcon { get; set; }
        public UserType UserType { get; set; }
        public string UserName { get; set; }
    }


    public partial class MapView : ContentPage,IMapPageProcessor,ILocationSelectionResult
    {
        
        CustomMap map;
        IMapSocketService mapSocketService;

        public Action<LocationSearchResult> OnLocationSelected { get; set; }
        public Action<CustomPin> OnMapInfoWindowClicked { get; set; }
        public Action OnPopupConfirmed { get; set; }
        public Action OnPopupCanceled { get; set; }
        public Action OnNewCoordinatesRecived { get; set; }
        public RouteData RouteResult { get; set; }
        public List<CustomPin> MapPins { get; set; }
        public CustomPin SelectedPin { get; set; }
        public LocationSearchResult SelectedDestination { get; private set; }

        static NotificationInfo notificationInfo;

        public MapView()
        {
            Init();
            new RiderViewPresenter(this);    
        }

        public MapView(NotificationInfo notificationInfoData)
        {
            Init();
            notificationInfo = notificationInfoData;
        }

        async void OnInfoWindowClicked(CustomPin pin)
        {
            SelectedPin = pin;
            OnMapInfoWindowClicked(pin);
        }

        private void Init()
        {
            InitializeComponent();
            InitMap();
            InitPopup();
            MapPins = new List<CustomPin>();
            mapSocketService = DependencyService.Get<IMapSocketService>();
                    
            notificationPopup.BackgroundColor = new Color(0, 0, 0, 0.5);
            destinationSelector.GestureRecognizers.Add(new TapGestureRecognizer(OnTapDestinationSelector));

            mapSocketService.MapCoordinateChanged += mapSocketService_MapCoordinateChanged;
            cancelPopupButton.Clicked += CancelPopupButton_Clicked;
            this.OnLocationSelected = OnLocationSelecteResult;
            this.OnMapInfoWindowClicked = OnInfoWindowClicked;
            

        }

        private void mapSocketService_MapCoordinateChanged(object sender, EventArgs e)
        {
            this.OnNewCoordinatesRecived();
        }

        private void InitPopup()
        {
            sendingPopupForeground.BackgroundColor = new Color(0, 0, 0, 0.5);
            sendRequestButon.Clicked += SendRequestButon_Clicked;
        }

        private void SendRequestButon_Clicked(object sender, EventArgs e)
        {
            //RideHistory rideHistory = new RideHistory();
            //rideHistory.UserName = Session.CurrentUserName;
            //rideHistory.DiverUserName = selectedPin.UserName;
            //rideHistory.SourceName = String.Format("{0}(Lat = {1}, Lng = {2}", String.Empty, selectedPin.Pin.Position.Latitude, selectedPin.Pin.Position.Longitude);
            //rideHistory.SourceLongitude = selectedPin.Pin.Position.Longitude.ToString();
            //rideHistory.SourceLatitude = selectedPin.Pin.Position.Latitude.ToString() ;
            //rideHistory.DestinationName = String.Format("{0}(Lat = {1}, Lng = {2}", String.Empty, App.CurrentLoggedUser.Location.Latitude, App.CurrentLoggedUser.Location.Longitude);
            //rideHistory.DestinationLongitude = App.CurrentLoggedUser.Location.Longitude;
            //rideHistory.DestinationLatitude = App.CurrentLoggedUser.Location.Latitude;
            //var result = driverLocatorService.CreateHistory(rideHistory);
            //if (result.IsSuccess)
            //{
            //    HidePopupBox();
            //    appData.Save("selected_pin_username", rideHistory.DiverUserName);
            //    DisplayAlert("Success", "Successfully sent the notification to driver", "Ok");
            //    //this.ShowNotificationPopup("Successfully sent the notification to driver");
            //    //var notificator = DependencyService.Get<IToastNotificator>();
            //    //notificator.Notify(ToastNotificationType.Success, "Success", "Successfully sent the notification to driver", TimeSpan.FromSeconds(3));
            //}
        }

        //#region BusinessProcessRelatedFunctions

        ////private RouteData GetRouteResult()
        ////{
        ////    try
        ////    {
        ////        LoadDriversAndRiders();
        ////        RouteData routeResult = new RouteData();
        ////        //RouteResult routeResult = new RouteResult();
        ////        routeResult.RideHistoryItem = driverLocatorService.GetRideHistoryByFilter("_id", notificationInfo.RequestId).RideHistories.FirstOrDefault();

        ////        routeResult.RiderLocation = riderResponse.UserLocations.Find(x => x.User.UserName == routeResult.RideHistoryItem.UserName).Location;


        ////        Coordinate destinationCoordinate = new Coordinate() { Latitude = double.Parse(routeResult.RiderLocation.Latitude), Longitude = double.Parse(routeResult.RiderLocation.Longitude) };
        ////        Coordinate sourceCoordinate = new Coordinate() { Latitude = double.Parse(App.CurrentLoggedUser.Location.Latitude), Longitude = double.Parse(App.CurrentLoggedUser.Location.Longitude) };


        ////        routeResult.RouteCoordinates = GetLineCoordinates(sourceCoordinate, destinationCoordinate);


        ////        return routeResult;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }
        ////}

        ////private List<Position> GetLineCoordinates(Coordinate sourceCoordinate, Coordinate destinationCoordinate)
        ////{
        ////    List<Position> routeCoordinates = new List<Position>();
        ////    GoogleMapsDirectionsClient googleMapsDirectionsClient = new GoogleMapsDirectionsClient();
        ////    var source = new GoogleApiClient.Models.Coordinate() { Latitude = sourceCoordinate.Latitude, Longitude = sourceCoordinate.Longitude };
        ////    var destination = new GoogleApiClient.Models.Coordinate() { Latitude = destinationCoordinate.Latitude, Longitude = destinationCoordinate.Longitude };

        ////    //var source = new GoogleApiClient.Models.Coordinate() { Latitude = 7.087310, Longitude = 80.014366 };
        ////    //var destination = new GoogleApiClient.Models.Coordinate() { Latitude = 7.209709, Longitude = 79.842796 };
        ////    var directions = googleMapsDirectionsClient.GetDirections(new GoogleApiClient.Models.GetDirectionRequest() { DestinationCoordinate = destination, SourceCoordinate = source });
        ////    //map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(source.Latitude, source.Longitude), Distance.FromKilometers(10)));
        ////    foreach (var route in directions.Routes)
        ////    {
        ////        foreach (var coordinates in route.OverViewPolyLine.DecodedOverViewPolyLine)
        ////        {
        ////            routeCoordinates.Add(new Position(coordinates.Latitude, coordinates.Longitude));
        ////        }
        ////    }

        ////    return routeCoordinates;
        ////}

        //#endregion

        #region MapViewRelatedFunctions
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

        public void AddPin(MapPin mapPin)
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
                Id = Guid.NewGuid()
            };

            Device.BeginInvokeOnMainThread(() =>
            {
                map.Pins.Add(pin.Pin);
            });
            MapPins.Add(pin);

        }

        public void RefreshPins(bool canMoveToLocation,Action loadPinsAction)
        {
            Task.Factory.StartNew(() => {
                loadPinsAction();
            }).ContinueWith((task) => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    map.Pins.Clear();
                    map.CustomPins.Clear();
                    map.CustomPins = MapPins;

                    if (canMoveToLocation)
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(double.Parse(App.CurrentLoggedUser.Location.Latitude), double.Parse(App.CurrentLoggedUser.Location.Longitude)), Xamarin.Forms.Maps.Distance.FromKilometers(2)));
                    }
                });
            });
        }
 
        public void ShowRoute(Func<RouteData> getDataFunction)
        {
            Task<RouteData>.Factory.StartNew(getDataFunction).ContinueWith((task) => {

                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        map.Pins.Clear();
                        map.CustomPins = new List<CustomPin>();
                        map.RouteCoordinates = new List<Position>();
                        var data = task.Result;
                        var sourcePin = data.SourcePin;
                        var destinationPin = data.DestinationPin;
                        var routeCoordinates = data.RouteCoordinates;
                        AddPin(sourcePin);
                        AddPin(destinationPin);
                        map.CustomPins = MapPins;
                        map.RouteCoordinates = routeCoordinates;
                    }
                    catch (Exception ex)
                    {

                    }
                });
            });

        }

        public void ShowPopupBox(string title)
        {
            notificationMessage.Text = title;
            //sendingPopupBack.IsVisible = true;
            sendingPopupForeground.IsVisible = true;
        }

        public void HidePopupBox()
        {
            //sendingPopupBack.IsVisible = false;
            sendingPopupForeground.IsVisible = false;
        }

        private void CancelPopupButton_Clicked(object sender, EventArgs e)
        {
            HidePopupBox();
        }

        void OnTapDestinationSelector(View sender, object e)
        {
            //App.Current.MainPage = new NavigationPage(new LocationSearch());
            Navigation.PushAsync(new LocationSearch(this));
        }

        void OnLocationSelecteResult(LocationSearchResult result)
        {
            SelectedDestination = result;
            destinationText.Text = String.Format("{0} (Lat={1}, Lng={2})", result.LocationName, result.Latitude, result.Longitude);
        }

        #endregion MapViewRelatedFunctions



    }
}
