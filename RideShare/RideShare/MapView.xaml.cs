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
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        CustomMap map;
        IMapSocketService mapSocketService;

        public Action<LocationSearchResult> OnLocationSelected { get; set; }
        public Action<CustomPin> OnMapInfoWindowClicked { get; set; }
        public Action OnSendNotificationPopupConfirmed { get; set; }
        public Action OnSendNotificationPopupCanceled { get; set; }
        public Action OnNewCoordinatesRecived { get; set; }
        public Action OnNewStatusChanged { get; set; }
        public RouteData RouteResult { get; set; }
        public List<CustomPin> MapPins { get; set; }
        public CustomPin SelectedPin { get; set; }
        public LocationSearchResult SelectedDestination { get; private set; }
        BaseMapViewPresenter precenter;

        public MapView()
        {
            Init();
            precenter = new PresenterLocator(this, driverLocatorService).GetPrecenter(null);
        }

        public MapView(NotificationInfo notificationInfoData)
        {
                Init();
                precenter = new PresenterLocator(this, driverLocatorService).GetPrecenter(notificationInfoData);
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
           
                    
            notificationPopup.BackgroundColor = new Color(0, 0, 0, 0.5);
            destinationSelector.GestureRecognizers.Add(new TapGestureRecognizer(OnTapDestinationSelector));

            
            cancelPopupButton.Clicked += CancelPopupButton_Clicked;
            sendRequestButon.Clicked += SendRequestButon_Clicked;
            btnToolBarChangeStatus.Text = "Set PikedUp";
            cancelinfoWindowPopupButton.Clicked += CancelinfoWindowPopupButton_Clicked;
            this.OnLocationSelected = OnLocationSelecteResult;
            this.OnMapInfoWindowClicked = OnInfoWindowClicked;
            mapSocketService = DependencyService.Get<IMapSocketService>();


        }

        private void BtnToolBarChangeStatus_Clicked(object sender, EventArgs e)
        {
            OnNewStatusChanged();
        }

        private void CancelinfoWindowPopupButton_Clicked(object sender, EventArgs e)
        {
            HideInfoWindowPopupBox();
        }

        private void SendRequestButon_Clicked(object sender, EventArgs e)
        {
            OnSendNotificationPopupConfirmed();
        }

        private void InitPopup()
        {
            sendingPopupForeground.BackgroundColor = new Color(0, 0, 0, 0.5);
            infoWindowPopup.BackgroundColor = new Color(0, 0, 0, 0.5);
        }

        #region MapViewRelatedFunctions
        private void InitMap()
        {
            map = new CustomMap
            {
                IsShowingUser = false,
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
                Id = mapPin.UserName
            };

            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    map.Pins.Add(pin.Pin);
            //});
            MapPins.Add(pin);

        }

        public void RefreshPins(bool canMoveToLocation,Func<List<CustomPin>> loadPinsFunction)
        {
            Task<List<CustomPin>>.Factory.StartNew(loadPinsFunction).ContinueWith((task) => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        map.Pins.Clear();
                        var result = task.Result;
                        SetPins(MapPins);
                        map.CustomPins = result;

                        if (canMoveToLocation)
                        {
                            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(double.Parse(App.CurrentLoggedUser.Location.Latitude), double.Parse(App.CurrentLoggedUser.Location.Longitude)), Xamarin.Forms.Maps.Distance.FromKilometers(2)));
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                   
                });
            });
        }
 
        private void SetPins(List<CustomPin> customPins)
        {
           foreach(var pin in customPins)
            {
                map.Pins.Add(pin.Pin);
            }
        }

        public void RefreshRoute(bool canMoveToLocation,Func<RouteData> getDataFunction)
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
                        if (canMoveToLocation)
                        {
                            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(double.Parse(App.CurrentLoggedUser.Location.Latitude), double.Parse(App.CurrentLoggedUser.Location.Longitude)), Xamarin.Forms.Maps.Distance.FromKilometers(2)));
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
            });

        }

        public void ShowDoubleButtonPopup(string title,string buttonConfirmText,string buttonCancelText)
        {
            //notificationMessage.Text = title;
            //sendingPopupBack.IsVisible = true;
            sendRequestButon.Text = buttonConfirmText;
            cancelPopupButton.Text = buttonCancelText;
            sendNotificationLable.Text = title;
            sendingPopupForeground.IsVisible = true;
        }

        public void HideDoubleButtonPopupBox()
        {
            sendNotificationLable.Text = String.Empty;
            //sendingPopupBack.IsVisible = false;
            sendingPopupForeground.IsVisible = false;
        }

        private void CancelPopupButton_Clicked(object sender, EventArgs e)
        {
            OnSendNotificationPopupCanceled();
        }

        void OnTapDestinationSelector(View sender, object e)
        {
            //App.Current.MainPage = new NavigationPage(new LocationSearch());
            Navigation.PushAsync(new LocationSearch(this));
        }

        void OnLocationSelecteResult(LocationSearchResult result)
        {
            SelectedDestination = result;
            driverLocatorService.UpdateUserDestination(App.CurrentLoggedUser.User.UserName, new UpdateUserDestinationRequest() { DestinationName = result.LocationName, Latitude = result.Latitude, Longitude = result.Longitude });
            SetDestination(result);
        }

        public void SetDestination(LocationSearchResult destination)
        {
            destinationText.Text = String.Format("{0}", destination.LocationName);
            SelectedDestination = destination;
        }
        public void ShowInfoWindowPopupBox(InfoWindowContent infoWindowContent)
        {
            infoWindowTitle.Text = infoWindowContent.Title;
            infoWindowDescription.Text = infoWindowContent.Description;
            infoWindowPopup.IsVisible = true;            
        }

        public void HideInfoWindowPopupBox()
        {
            infoWindowTitle.Text = String.Empty;
            infoWindowDescription.Text = String.Empty;
            infoWindowPopup.IsVisible = false;
        }

        public void NavigateToRiderView()
        {
            App.Current.MainPage = new NavigationPage(new MapView());
        }

        public void LoadCurrentStatus(string status)
        {
            if(status!=null)
            { 
                
                //toolBar1.IsVisible = true;
                btnToolBarChangeStatus.Text = status;
            }
            else
            {
                //toolBar1.IsVisible = false;
            }
        }

        #endregion MapViewRelatedFunctions



    }
}
