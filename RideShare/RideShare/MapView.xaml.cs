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
using Common;

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
        public object Data { get; set; }
    }


    public partial class MapView : ContentPage, IMapPageProcessor, ILocationSelectionResult, ILocationServiceStatusCallback
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        CustomMap map;
        //Label l = new Label() {HorizontalOptions = LayoutOptions.Center};
        IMapSocketService mapSocketService;
        ILocationServiceHelper locationServiceHelper;
        INavigationService navigationService;

        bool waitingForLocationServiceEnable = false;
        public Action<LocationSearchResult> OnLocationSelected { get; set; }
        public Action<CustomPin> OnMapInfoWindowClicked { get; set; }
        public Action OnNewCoordinatesRecived { get; set; }
        public Action OnNewStatusChanged { get; set; }
        public Action OnNearTheDestination { get; set; }
        public Action OnInitializationCompleted { get; set; }
        public RouteData RouteResult { get; set; }
        public List<CustomPin> MapPins { get; set; }
        public CustomPin SelectedPin { get; set; }
        public LocationSearchResult SelectedDestination { get; private set; }
        private Action popupConfirmAction { get; set; }
        private Action popupCancelAction { get; set; }

        BaseMapViewPresenter precenter;

        public MapView()
        {
            Init();           
            InitMapData();
        }


        private void InitMapData(NotificationInfo notificationInfoData = null)
        {
            ShowBusyIndecator();
            if (locationServiceHelper.IsGPSAvailable)
            {
                Task.Factory.StartNew<BaseMapViewPresenter>(() =>
                {

                    PresenterLocator presenterLocator = new PresenterLocator(this, driverLocatorService);
                    return presenterLocator.GetPrecenter(notificationInfoData);

                }).ContinueWith((task) =>
                {

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        precenter = task.Result;
                    });
                });
            }
            else
            {
                DisplayAlert("GPS Location Not Available", "Please turn on your device location service", "Yes", "No").ContinueWith((task) =>
                {
                    if (task.Result)
                    {
                        locationServiceHelper.ShowLocationSettings();
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        waitingForLocationServiceEnable = true;
                    });


                });

            }
        }

        public MapView(NotificationInfo notificationInfoData)
        {
            Init();
            InitMapData(notificationInfoData);

        }

        async void OnInfoWindowClicked(CustomPin pin)
        {
            SelectedPin = pin;
            OnMapInfoWindowClicked(pin);
        }

        private void Init()
        {
            InitializeComponent();
            //ShowBusyIndecator();
            InitMap();
            InitPopup();
            MapPins = new List<CustomPin>();


            notificationPopup.BackgroundColor = new Color(0, 0, 0, 0.5);
            
            destinationSelector.GestureRecognizers.Add(new TapGestureRecognizer(OnTapDestinationSelector));


            cancelPopupButton.Clicked += CancelPopupButton_Clicked;
            sendRequestButon.Clicked += SendRequestButon_Clicked;
            openNavigationButton.GestureRecognizers.Add(new TapGestureRecognizer(OpenNavigationButton_Clicked));
            btnToolBarChangeStatus.Text = "Set PikedUp";
            cancelinfoWindowPopupButton.Clicked += CancelinfoWindowPopupButton_Clicked;
            this.OnLocationSelected = OnLocationSelecteResult;
            this.OnMapInfoWindowClicked = OnInfoWindowClicked;
            mapSocketService = DependencyService.Get<IMapSocketService>();
            locationServiceHelper = DependencyService.Get<ILocationServiceHelper>();
            navigationService = DependencyService.Get<INavigationService>();
            Session.LocationServiceStatusCallback = this;

        }

        private void OpenNavigationButton_Clicked(View sender, object e)
        {
            if(SelectedPin!=null)
            {
                navigationService.ShowNavigationApp(SelectedPin.Pin.Position.Longitude, SelectedPin.Pin.Position.Latitude);
            }
            else
            {
                ShowInfoWindowPopupBox(new InfoWindowContent() { Title = "Please Select a pin", Description = "Please select a pin to start navigation" });
            }
            
        }

        public void ShowBusyIndecator()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                busyIndicator.IsEnabled = true;
                busyIndicator.IsRunning = true;
                busyIndicator.IsVisible = true;
            });

        }

        public void HideBusyIndecator()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                busyIndicator.IsEnabled = false;
                busyIndicator.IsRunning = false;
                busyIndicator.IsVisible = false;
            });

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
            popupConfirmAction();
        }

        private void InitPopup()
        {
            sendingPopupForeground.BackgroundColor = new Color(0, 0, 0, 0.5);
            infoWindowPopup.BackgroundColor = new Color(0, 0, 0, 0.5);
        }

        #region MapViewRelatedFunctions
        private void InitMap()
        {
            //TODO: When the longitude or latitude is negative map starts at Rome
            double _latlitulde, _longitude;
            double.TryParse(App.CurrentLoggedUser.Location.Latitude, out _latlitulde);
            double.TryParse(App.CurrentLoggedUser.Location.Longitude, out _longitude);
            
            map = new CustomMap
            {
                IsShowingUser = false,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BaseLatitude = _latlitulde,
                BaseLongitude = _longitude
            };

            mapContainer.Children.Clear();
            mapContainer.Children.Add(map);
            map.OnInfoWindowClicked = OnInfoWindowClicked;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnMapTapped; ;
            map.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void OnMapTapped(object sender, EventArgs e)
        {
            HideDoubleButtonPopupBox();
            HideInfoWindowPopupBox();
        }

        public void AddPin(MapPin mapPin)
        {
            Device.BeginInvokeOnMainThread(() =>
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
            });

        }

        public void RefreshPins(bool canMoveToLocation, Func<List<CustomPin>> loadPinsFunction)
        {
            Task<List<CustomPin>>.Factory.StartNew(loadPinsFunction).ContinueWith((task) =>
            {
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
                        HideBusyIndecator();
                    }
                    catch (Exception ex)
                    {

                    }

                });
            });
        }

        private void SetPins(List<CustomPin> customPins)
        {
            foreach (var pin in customPins)
            {
                map.Pins.Add(pin.Pin);
            }
        }

        public void RefreshRoute(bool canMoveToLocation, Func<RouteData> getDataFunction)
        {
            Task<RouteData>.Factory.StartNew(getDataFunction).ContinueWith((task) =>
            {

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
                        HideBusyIndecator();
                    }
                    catch (Exception ex)
                    {

                    }
                });
            });

        }

        public void ShowDoubleButtonPopup(string title, string buttonConfirmText, string buttonCancelText, Action confirmAction, Action cancelAction)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                sendRequestButon.Text = buttonConfirmText;
                cancelPopupButton.Text = buttonCancelText;
                sendNotificationLable.Text = title;
                sendingPopupForeground.IsVisible = true;
                popupConfirmAction = confirmAction;
                popupCancelAction = cancelAction;
            });


        }

        public void HideDoubleButtonPopupBox()
        {
            sendNotificationLable.Text = String.Empty;
            //sendingPopupBack.IsVisible = false;
            sendingPopupForeground.IsVisible = false;
        }

        private void CancelPopupButton_Clicked(object sender, EventArgs e)
        {
            popupCancelAction();
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
            Device.BeginInvokeOnMainThread(() =>
            {
                destinationText.Text = String.Format("{0}", destination.LocationName);
                SelectedDestination = destination;
            });

        }
        public void ShowInfoWindowPopupBox(InfoWindowContent infoWindowContent,Action okAction = null)
        {
            
            Device.BeginInvokeOnMainThread(() =>
            {
                infoWindowTitle.Text = infoWindowContent.Title;
                infoWindowDescription.Text = infoWindowContent.Description;
                infoWindowPopup.IsVisible = true;
                if(okAction != null)
                {
                    okAction();
                }
            });
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
            Device.BeginInvokeOnMainThread(() =>
            {

                if (status != null)
                {
                    btnToolBarChangeStatus.Text = status;
                }
            });

        }

        public void OnLocationStatusChange(LocationServiceStatus status)
        {
            InitMapData();
        }


        #endregion MapViewRelatedFunctions



    }
}
