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

namespace RideShare.ViewPresenter
{
    public class BaseMapViewPresenter
    {
        protected IMapPageProcessor mapPageProcessor;
        protected IMapSocketService mapSocketService;
        
        
        public BaseMapViewPresenter(IMapPageProcessor mapPageProcessor,IMapSocketService mapSocketService)
        {
            this.mapPageProcessor = mapPageProcessor;
            this.mapSocketService = mapSocketService;           
            Init();
        }

        private void Init()
        {
            mapPageProcessor.OnMapInfoWindowClicked = OnMapInfoWindowClicked;
            mapPageProcessor.OnSendNotificationPopupCanceled = OnPopupCanceled;
            mapPageProcessor.OnSendNotificationPopupConfirmed = OnPopupConfirmed;
            mapPageProcessor.OnNewCoordinatesRecived = OnNewCoordinatesRecived;
            mapSocketService.MapCoordinateChanged += OnNewCoordinateChanged;
            mapPageProcessor.OnNewStatusChanged = OnNewStatusChanged;
        }

        private void OnNewCoordinateChanged(object sender, EventArgs e)
        {
            OnNewCoordinatesRecived();
        }

        protected virtual void OnMapInfoWindowClicked(CustomPin customPin) { }
        protected virtual void OnPopupCanceled() { }
        protected virtual void OnPopupConfirmed() { }
        protected virtual void OnNewCoordinatesRecived() { }
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
            if (!String.IsNullOrEmpty(App.CurrentLoggedUser.Destination.Name))
            {
                var destination = new LocationSearchResult() { Latitude = double.Parse(App.CurrentLoggedUser.Destination.Latitude), Longitude = double.Parse(App.CurrentLoggedUser.Destination.Longitude), LocationName = App.CurrentLoggedUser.Destination.Name };
                mapPageProcessor.SetDestination(destination);
            }
        }

        protected virtual void OnNewStatusChanged() { }

    }
}
