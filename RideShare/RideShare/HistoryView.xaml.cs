using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleApiClient.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
namespace RideShare
{
    public partial class HistoryView : ContentPage
    {
        CustomMap map;
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        public HistoryView()
        {
            InitializeComponent();
            LoadHistorydata();
        }

        public HistoryView(NotificationInfo notificationInfo)
        {
            InitializeComponent();
            //LoadHistorydata(notificationInfo);
        }

        private void LoadHistorydata()
        {
            var userCoordinates = driverLocatorService.ViewUserCoordinates();
            var Histories = driverLocatorService.GetRideHistoryByFilter("UserName","user1x" );//App.CurrentLoggedUser.User.UserName

            foreach (var History in Histories.RideHistories)
            {
                foreach (var coordinates in History.DecodedOverviewPolyLine)
                {                   
                    map.RouteCoordinates.Add(new Position(coordinates.Latitude, coordinates.Longitude));
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(coordinates.Latitude, coordinates.Longitude), Distance.FromMiles(50)));
                }
            }
            InitMap();
        }

        private void InitMap()
        {
            map = new CustomMap
            {
                IsShowingUser = true,
                HeightRequest = 30,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            mapContainer.Children.Clear();
            mapContainer.Children.Add(map);           
        }
    }
}
