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
            RenderLine();
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
            var userCoordinates = driverLocatorService.ViewUserCoordinates();
            ShowOnLabels(userCoordinates.UserCoordinates);
            ShowOnMap(userCoordinates.UserCoordinates);
        }

        private void ShowOnLabels(List<UserCoordinate> userCoordinates)
        {
            messageLabel.Text = "";
            foreach (var userCorrdinate in userCoordinates)
            {
                messageLabel.Text += userCorrdinate.UserName + "|" + userCorrdinate.EMail + "|" + userCorrdinate.FirstName + "|" + userCorrdinate.Longitude + "|" + userCorrdinate.Latitude + Environment.NewLine;
            }
        }

        private void ShowNotificationInMap(NotificationInfo notificationInfo)
        {
            map.Pins.Clear();
            RenderPin(notificationInfo.Longitude, notificationInfo.Latitude, notificationInfo.LocationName);
        }

        private void ShowOnMap(List<UserCoordinate> userCoordinates)
        {

            foreach (var item in userCoordinates)
            {
                RenderPin(item.Longitude, item.Latitude, item.FirstName + " " + item.LastName + " | Position : " + item.Longitude + " , " + item.Latitude);
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

        private void RenderPin(string longitudeCoordinate, string latitudeCoordinate, string lable)
        {
            double latitude = 0;
            double longitude = 0;

            double.TryParse(latitudeCoordinate, out latitude);
            double.TryParse(longitudeCoordinate, out longitude);


            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromMiles(60)));

            var position = new Position(latitude, longitude);

            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = lable
            };

            map.Pins.Add(pin);
        }

        private void RenderLine()
        {
            map.RouteCoordinates.Add(new Position(7.087310, 80.014366));
            map.RouteCoordinates.Add(new Position(7.209709, 79.842796));

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(7.087310, 80.014366), Distance.FromMiles(1.0)));
        }
    }
}
