using Authentication;
using Authentication.Models;
using Common.Models;
using DriverLocatorFormsPortable.Common;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using GoogleApiClient.Maps;
using System.Collections.ObjectModel;

namespace RideShare.ViewModels
{
    public class RouteResult
    {
        private List<Position> routeCoordinates;

        public List<Position> RouteCoordinates
        {
            get { return routeCoordinates; }
            set { routeCoordinates = value; }
        }

        private DateTime riderMeetTime;

        public DateTime RiderMeetTime
        {
            get { return riderMeetTime; }
            set { riderMeetTime = value; }
        }

        public RouteResult()
        {
            routeCoordinates = new List<Position>();
        }
    }
    public class HistoryViewModel : ViewModelBase
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        ObservableCollection<CustomMapViewModel> maplist = new ObservableCollection<CustomMapViewModel>();
        DateTime meettime = DateTime.Now;


        public ObservableCollection<CustomMapViewModel> MapList
        {
            get
            {
                return maplist;
            }
            set
            {
                maplist = value;
                OnPropertyChanged("MapList");
            }
        }


        public HistoryViewModel()
        {
            IsBusy = true;
            Task<List<RouteResult>>.Factory.StartNew(GetRouteResult).ContinueWith((task) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var routes = task.Result;

                    foreach(var route in routes)
                    {
                        if (route.RouteCoordinates.Count > 0)
                        {
                            InitMap(route.RouteCoordinates, route.RiderMeetTime);

                        }
                    }
                    
                    IsBusy = false;
                });
            });
        }


        double l1 = 0;
        double l2 = 0;
        private List<RouteResult> GetRouteResult()
        {
            List<RouteResult> routeResult = new List<RouteResult>();
            try
            {
                var Histories = driverLocatorService.GetRideHistoryByFilter("userName", App.CurrentLoggedUser.User.UserName);
                foreach (var ridehistory in Histories.RideHistories)
                {
                    if (ridehistory.DecodedOverviewPolyLine != null)
                    {

                        List<Position> positions = new List<Position>();
                        foreach (var position in ridehistory.DecodedOverviewPolyLine)
                        {
                            positions.Add(new Position(position.Latitude, position.Longitude));
                        }
                        RouteResult route = new RouteResult();
                        route.RouteCoordinates = positions;
                        routeResult.Add(route);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return routeResult;
        }

        private List<Position> GetLineCoordinates(Coordinate sourceCoordinate, Coordinate destinationCoordinate, List<GoogleApiClient.Models.Coordinate> wayPoints)
        {
            List<Position> routeCoordinates = new List<Position>();
            GoogleMapsDirectionsClient googleMapsDirectionsClient = new GoogleMapsDirectionsClient();

            var source = new GoogleApiClient.Models.Coordinate() { Latitude = sourceCoordinate.Latitude, Longitude = sourceCoordinate.Longitude };
            var destination = new GoogleApiClient.Models.Coordinate() { Latitude = destinationCoordinate.Latitude, Longitude = destinationCoordinate.Longitude };

            var directions = googleMapsDirectionsClient.GetDirections(new GoogleApiClient.Models.GetDirectionRequest() { DestinationCoordinate = destination, SourceCoordinate = source, WayPoints = wayPoints });

            foreach (var route in directions.Routes)
            {
                foreach (var coordinates in route.OverViewPolyLine.DecodedOverViewPolyLine)
                {
                    routeCoordinates.Add(new Position(coordinates.Latitude, coordinates.Longitude));
                }
            }

            return routeCoordinates;
        }


        private void InitMap(List<Position> RouteCoordinates, DateTime RiderMeetTime)
        {
            string _time = String.Format("{0:dd-MM-yyyy}", RiderMeetTime) + "/" + RiderMeetTime.ToString("h:mm tt");
            var middleCoordinate = RouteCoordinates[RouteCoordinates.Count / 2];
            CustomMapViewModel map = new CustomMapViewModel
            {

                IsShowingUser = true,
                HeightRequest = 300,
                WidthRequest = 600,
                BaseLatitude = middleCoordinate.Latitude,
                BaseLongitude = middleCoordinate.Longitude,
                DriverName = "Driver Name : " + App.CurrentLoggedUser.User.UserName,
                RiderMeetTime = _time,
                
            };

            map.RouteCoordinates = RouteCoordinates;
            MapList.Add(map);
            
        }
    }
}
