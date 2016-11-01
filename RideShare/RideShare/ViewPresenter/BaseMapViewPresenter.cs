using DriverLocator.Models;
using GoogleApiClient.Maps;
using GoogleApiClient.Models;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.ViewPresenter
{
    public class BaseMapViewPresenter : IDisposable
    {
        protected IMapPageProcessor mapPageProcessor;
        protected IMapSocketService mapSocketService;
        private DriverLocator.DriverLocatorService driverLocatorService;
        private IAppDataService appDataService;
        bool isNearToDestination;
        bool isNearPopupOpened;
        public bool HasRides { get; set; }

        public BaseMapViewPresenter(IMapPageProcessor mapPageProcessor,IMapSocketService mapSocketService, DriverLocator.DriverLocatorService driverLocatorService)
        {
            this.driverLocatorService = driverLocatorService;
            this.mapPageProcessor = mapPageProcessor;
            this.mapSocketService = mapSocketService;  
            this.appDataService = this.appDataService = DependencyService.Get<IAppDataService>();
            Init();
        }

        private void Init()
        {
            mapPageProcessor.OnMapInfoWindowClicked = OnMapInfoWindowClicked;
            mapPageProcessor.OnNewCoordinatesRecived = OnNewCoordinatesRecived;
            mapPageProcessor.OnInitializationCompleted = OnInitializationCompleted;
            mapSocketService.MapCoordinateChanged += OnNewCoordinateChanged;
            mapPageProcessor.OnNewStatusChanged = OnNewStatusChanged;
            StartDestinationFinding();
        }

        private void OnNewCoordinateChanged(object sender, EventArgs e)
        {
            OnNewCoordinatesRecived();
        }
       
        private void StartDestinationFinding()
        {
            Action destinationFindingWork = new Action(() =>
            {
                
                while (true)
                {
                    var userData = driverLocatorService.GetSelectedUserCoordinate(appDataService.Get("current_user"));

                    if( userData.UserLocation.Destination.Latitude != null && userData.UserLocation.Destination.Longitude != null )
                    {
                        Coordinate userCoordinate = new Coordinate() { Latitude = double.Parse(userData.UserLocation.Location.Latitude), Longitude = double.Parse(userData.UserLocation.Location.Longitude) };
                        Coordinate destinationCoordinate = new Coordinate() { Latitude = double.Parse(userData.UserLocation.Destination.Latitude), Longitude = double.Parse(userData.UserLocation.Destination.Longitude) };

                        var direction = GetDirections(userCoordinate, destinationCoordinate).Routes.SingleOrDefault().Legs.SingleOrDefault().Distance.Value;

                        if (direction < 300)
                        {
                            if (isNearToDestination == false)
                            {
                                isNearToDestination = true;
                                OnNearTheDestination();
                            }
                        }
                        else
                        {
                            isNearToDestination = false;
                        }
                    }
                    Task.Delay(3000).Wait();
                    //Thread.Sleep(3000);
                }
                


            });

            Task.Factory.StartNew(destinationFindingWork);
            //destinationFindingThread = new Thread(destinationFindingWork);
            //destinationFindingThread.Start();
        }

        protected virtual void OnMapInfoWindowClicked(CustomPin customPin) { }

        private void OnPopupCanceled() {

            if(isNearPopupOpened)
            {
                mapPageProcessor.HideDoubleButtonPopupBox();
            }
        }

        private void OnPopupConfirmed() {

            if (isNearPopupOpened)
            {
                var result = driverLocatorService.FinishRide(appDataService.Get("current_user")).Result;
                if(result.IsSuccess)
                {
                    mapPageProcessor.HideDoubleButtonPopupBox();
                }
                else
                {
                    mapPageProcessor.HideDoubleButtonPopupBox();
                    mapPageProcessor.ShowInfoWindowPopupBox(new InfoWindowContent() { Title = "Error", Description = result.Message });
                }
               
            }

        }
        protected virtual void OnNewCoordinatesRecived() { }
        protected virtual void OnInitializationCompleted() {
            //mapPageProcessor.HideBusyIndecator();
        }
        protected virtual List<CustomPin> LoadPinData() { return null; }
        protected virtual RouteData LoadRouteData() { return null; }

        protected virtual GetDirectionsResponse GetDirections(Coordinate sourceCoordinate, Coordinate destinationCoordinate)
        {
            GoogleMapsDirectionsClient googleMapsDirectionsClient = new GoogleMapsDirectionsClient();
            var source = new GoogleApiClient.Models.Coordinate() { Latitude = sourceCoordinate.Latitude, Longitude = sourceCoordinate.Longitude };
            var destination = new GoogleApiClient.Models.Coordinate() { Latitude = destinationCoordinate.Latitude, Longitude = destinationCoordinate.Longitude };
            var directions = googleMapsDirectionsClient.GetDirections(new GoogleApiClient.Models.GetDirectionRequest() { DestinationCoordinate = destination, SourceCoordinate = source });
            return directions;
        }

        protected virtual GetDirectionsResponse GetDirections(Coordinate sourceCoordinate, Coordinate destinationCoordinate, List<Coordinate> wayPoints)
        {
            GoogleMapsDirectionsClient googleMapsDirectionsClient = new GoogleMapsDirectionsClient();
            var source = new GoogleApiClient.Models.Coordinate() { Latitude = sourceCoordinate.Latitude, Longitude = sourceCoordinate.Longitude };
            var destination = new GoogleApiClient.Models.Coordinate() { Latitude = destinationCoordinate.Latitude, Longitude = destinationCoordinate.Longitude };

            List<GoogleApiClient.Models.Coordinate> convertedWaypoints = wayPoints.Select((coordinate) => { return new GoogleApiClient.Models.Coordinate() { Latitude = coordinate.Latitude, Longitude = coordinate.Longitude }; }).ToList();
            var directions = googleMapsDirectionsClient.GetDirections(new GoogleApiClient.Models.GetDirectionRequest() { DestinationCoordinate = destination, SourceCoordinate = source,WayPoints = convertedWaypoints });
            return directions;
        }

        protected virtual void RefreshPins(bool mooveToLocation)
        {
            
            mapPageProcessor.RefreshPins(mooveToLocation, LoadPinData);
        }

        protected virtual void RefreshRoute(bool mooveToLocation)
        {
            mapPageProcessor.RefreshRoute(mooveToLocation, LoadRouteData);
        }

        protected virtual void InitDestination()
        {
            if (!System.String.IsNullOrEmpty(App.CurrentLoggedUser.Destination.Name))
            {
                var destination = new LocationSearchResult() { Latitude = double.Parse(App.CurrentLoggedUser.Destination.Latitude), Longitude = double.Parse(App.CurrentLoggedUser.Destination.Longitude), LocationName = App.CurrentLoggedUser.Destination.Name };
                mapPageProcessor.SetDestination(destination);
            }
        }

        protected virtual void OnNewStatusChanged() { }

        protected virtual void OnNearTheDestination() {
            isNearPopupOpened = true;

            if(HasRides)
            {
                mapPageProcessor.ShowDoubleButtonPopup("Your are near to the destination. Do you want to end this ride?", "Yes", "No", this.OnPopupConfirmed, this.OnPopupCanceled);
            }
            
        }

        public void Dispose()
        {
            //destinationFindingThread.Dispose();
        }
    }
}
