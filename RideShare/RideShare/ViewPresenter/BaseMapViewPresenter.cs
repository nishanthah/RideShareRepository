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
            mapPageProcessor.OnPopupCanceled = OnPopupCanceled;
            mapPageProcessor.OnPopupConfirmed = OnPopupConfirmed;
            mapPageProcessor.OnNewCoordinatesRecived = OnNewCoordinatesRecived;
            mapSocketService.MapCoordinateChanged += OnNewCoordinateChanged;
        }

        private void OnNewCoordinateChanged(object sender, EventArgs e)
        {
            OnNewCoordinatesRecived();
        }

        protected virtual void OnMapInfoWindowClicked(CustomPin customPin) { }
        protected virtual void OnPopupCanceled() { }
        protected virtual void OnPopupConfirmed() { }
        protected virtual void OnNewCoordinatesRecived() { }
        protected virtual void LoadPinData() { }

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
        
    }
}
