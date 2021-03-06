﻿using DriverLocator.Models;
using GoogleApiClient.Helpers;
using GoogleApiClient.Models;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace RideShare.ViewPresenter
{

    public class DriverRequestedRidesPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService;
        IAppDataService appDataService;
        RideHistory selectedRide;

        public DriverRequestedRidesPresenter(IMapPageProcessor mapPageProcessor, IMapSocketService mapSocketService, DriverLocator.DriverLocatorService driverLocatorService) : base(mapPageProcessor, mapSocketService, driverLocatorService)
        {
            this.appDataService = DependencyService.Get<IAppDataService>();
            this.driverLocatorService = driverLocatorService;
            base.InitDestination();
            RefreshPins(true);
            base.OnInitializationCompleted();
        }
      
        protected override List<CustomPin> LoadPinData()
        {
            List<CustomPin> coordinates = new List<CustomPin>();
            var currentUser = appDataService.Get("current_user");
            var rideHistories = driverLocatorService.GetRideHistoryByFilter("driverUserName", currentUser)
                                    .RideHistories
                                    .Where(x=>x.RequestStatus == RequestStatus.Requested 
                                          || x.RequestStatus == RequestStatus.DriverAccepted
                                          || x.RequestStatus == RequestStatus.RiderMet)
                                    .ToList();
            if(rideHistories.Count > 0)
            {
                this.HasRides = true;
            }
            var driver = driverLocatorService.GetSelectedUserCoordinate(currentUser).UserLocation;

            Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude), Longitude = double.Parse(driver.Location.Longitude) };

            MapPin driverPin = new MapPin();

            driverPin.ImageIcon = "userLogActive_icon.png";
            driverPin.Latitude = double.Parse(driver.Location.Latitude);
            driverPin.Longitude = double.Parse(driver.Location.Longitude);
            driverPin.PhoneNo = !String.IsNullOrEmpty(driver.User.MobileNo) ? driver.User.MobileNo : "Not Set";

            driverPin.Title = String.Format("{0} {1}",
                                        driver.User.FirstName,
                                        driver.User.LastName);

            driverPin.UserName = driver.User.UserName;
            driverPin.UserType = driver.User.UserType;
            coordinates.Add(GetFromatted(driverPin));

            foreach (var rideHistory in rideHistories)
            {
                var rider = driverLocatorService.GetSelectedUserCoordinate(rideHistory.UserName).UserLocation;

                if(rider.User.IsLoggedIn)
                {
                    Coordinate destinationCoordinate = new Coordinate() { Latitude = double.Parse(rider.Location.Latitude), Longitude = double.Parse(rider.Location.Longitude) };
                    var directionsResult = GetDirections(driverCoordinate, destinationCoordinate);
                    IList<Leg> legs = null;
                    if(directionsResult.Routes != null && directionsResult.Routes.Count > 0)
                    {
                       legs = directionsResult.Routes.First().Legs;
                    }
                    

                    MapPin riderPin = new MapPin();

                    riderPin.ImageIcon = "userLogActive_icon.png";
                    riderPin.Latitude = double.Parse(rider.Location.Latitude);
                    riderPin.Longitude = double.Parse(rider.Location.Longitude);
                    riderPin.PhoneNo = !String.IsNullOrEmpty(rider.User.MobileNo) ? rider.User.MobileNo : "Not Set";

                    riderPin.Title = String.Format("[{4}]{0} {1} | ({2} {3})",
                                                rider.User.FirstName,
                                                rider.User.LastName,
                                                legs != null ? legs.Sum(x => x.Distance.Value).ToKilometers() : String.Empty,
                                                legs != null ? legs.Sum(x => x.Duration.Value).ToTimeString() : String.Empty,
                                                rideHistory.RequestStatus.ToString().ToUpper());
                    riderPin.Data = rideHistory;
                    riderPin.UserName = rider.User.UserName;
                    riderPin.UserType = rider.User.UserType;
                    coordinates.Add(GetFromatted(riderPin));
                }
            }
            return coordinates;
        }

        private List<Position> GetRouteCoordinates(Route route)
        {
            List<Position> routeCoordinates = new List<Position>();

            foreach (var coordinates in route.OverViewPolyLine.DecodedOverViewPolyLine)
            {
                routeCoordinates.Add(new Position(coordinates.Latitude, coordinates.Longitude));
            }

            return routeCoordinates;
        }

        protected override void OnMapInfoWindowClicked(CustomPin customPin)
        {
            
            if(customPin.Data != null)
            {
                this.selectedRide = (RideHistory)customPin.Data;

                if (selectedRide.RequestStatus == RequestStatus.Requested)
                {  
                    mapPageProcessor.ShowDoubleButtonPopup("Confirm the Accept or Reject?", "Accept", "Reject", this.UpdateToAccept, this.UpdateToReject);
                }

                else if (selectedRide.RequestStatus == RequestStatus.DriverAccepted)
                {

                    mapPageProcessor.ShowDoubleButtonPopup("Are you want to set this rider as pickedup?", "Yes", "No",this.UpdateToRiderMet,this.DismissPopup);
                }

                else if (selectedRide.RequestStatus == RequestStatus.RiderMet)
                {
                    mapPageProcessor.ShowDoubleButtonPopup("Are you want to end this ride?", "Yes", "No",this.UpdateToRideCompleted,this.DismissPopup);
                }
            }

        }

        
        private void UpdateToAccept()
        {
            UpdateStatus(RequestStatus.DriverAccepted);
        }

        private void UpdateToReject()
        {
            UpdateStatus(RequestStatus.DriverRejected);
        }

        private void UpdateToRiderMet()
        {
            UpdateStatus(RequestStatus.RiderMet);
        }

        private void UpdateToRideCompleted()
        {
            UpdateStatus(RequestStatus.RideCompleted);
            
        }

        private void DismissPopup()
        {
            mapPageProcessor.HideDoubleButtonPopupBox();
        }

        private void UpdateStatus(RequestStatus status)
        {
            var isSuccess = driverLocatorService.UpdateRideHistoryStatus(new UpdateRideHistoryRequest() { Id = selectedRide.Id, Status = status }).IsSuccess;

            if (isSuccess)
            {
                RefreshPins(false);
                mapPageProcessor.HideDoubleButtonPopupBox();

            }
        }

        protected override void OnNewCoordinatesRecived()
        {
                RefreshPins(false);
        }
     
        protected override void OnNewStatusChanged()
        {
            
        }

        private CustomPin GetFromatted(MapPin mapPin)
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
                Id = mapPin.UserName,
                Data = mapPin.Data
            };
            return pin;
        }

    }
}
