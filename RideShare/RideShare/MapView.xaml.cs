using DriverLocator.Models;
using RideShare;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace RideShare
{
    public partial class MapView : ContentPage
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);

        public MapView()
        {
            InitializeComponent();
            var mapSocketService = DependencyService.Get<IMapSocketService>();
            mapSocketService.MapCoordinateChanged += mapSocketService_MapCoordinateChanged;
            LoadUserData();
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
        private void ShowOnMap(List<UserCoordinate> userCoordinates)
        {
            var map = new Map
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            foreach (var item in userCoordinates)
            {
                double latitude = 0;
                double longitude = 0;

                double.TryParse(item.Latitude, out latitude);
                double.TryParse(item.Longitude, out longitude);


                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromMiles(30)));

                var position = new Position(latitude, longitude);

                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = position,
                    Label = item.FirstName + " " + item.LastName + " | Position : " + latitude + " , " + longitude
                };

                map.Pins.Add(pin);
            }
            mapContainer.Children.Clear();
            mapContainer.Children.Add(map);
        }

        //async void OnLogoutButtonClicked(object sender, EventArgs e)
        //{
        //    App.IsUserLoggedIn = false;
        //    Navigation.InsertPageBefore(new Login(), this);
        //    await Navigation.PopAsync();
        //}



    }
}
