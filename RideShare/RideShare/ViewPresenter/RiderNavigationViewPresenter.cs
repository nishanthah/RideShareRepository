﻿using GoogleApiClient.Maps;
using GoogleApiClient.Models;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace RideShare.ViewPresenter
{
    public class RiderNavigationViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
        string rideHistoryId;
        public RiderNavigationViewPresenter(IMapPageProcessor mapPageProcessor,IMapSocketService mapSocketService,string rideHistoryId) :base(mapPageProcessor,mapSocketService)
        {
            this.rideHistoryId = rideHistoryId;
            RefreshPins(true);
        }

        protected override void LoadPinData()
        {
            mapPageProcessor.MapPins = new List<CustomPin>();
            var notification = driverLocatorService.GetRideHistoryByFilter("_id", rideHistoryId).RideHistories.FirstOrDefault();
            var driver = driverLocatorService.GetSelectedUserCoordinate(notification.DiverUserName).UserLocation;
            var rider = driverLocatorService.GetSelectedUserCoordinate(notification.UserName).UserLocation;

            Coordinate driverCoordinate = new Coordinate() { Latitude = double.Parse(driver.Location.Latitude),Longitude = double.Parse(driver.Location.Longitude) };
            Coordinate riderCoordinate = new Coordinate() { Latitude = double.Parse(rider.Location.Latitude), Longitude = double.Parse(rider.Location.Longitude) };

            var directions = GetDirections(driverCoordinate, riderCoordinate).Routes.SingleOrDefault().Legs.SingleOrDefault();

            // Create driver pin
            MapPin driverPin = new MapPin();
            driverPin.ImageIcon = "userLogActive_icon.png";
            driverPin.Latitude = double.Parse(driver.Location.Latitude);
            driverPin.Longitude = double.Parse(driver.Location.Longitude);
            driverPin.PhoneNo = driver.User.MobileNo;

            driverPin.Title =String.Format("{0} {1} | Lat={2},Lng={3} | ({4} {5})", 
                                    driver.User.FirstName,
                                    driver.User.LastName, 
                                    driver.Location.Latitude,
                                    driver.Location.Longitude,
                                    directions.Distance.Text,
                                    directions.Duration.Text);

            driverPin.UserName = driver.User.UserName;
            driverPin.UserType = driver.User.UserType;
            mapPageProcessor.AddPin(driverPin);

            // Create rider pin
            var riderPin = new MapPin();
            riderPin.ImageIcon = "userLogActive_icon.png";
            riderPin.Latitude = double.Parse(rider.Location.Latitude);
            riderPin.Longitude = double.Parse(rider.Location.Longitude);
            riderPin.PhoneNo = rider.User.MobileNo;

            riderPin.Title = String.Format("{0} {1} | Lat={2},Lng={3} | ({4} {5})",
                                    rider.User.FirstName,
                                    rider.User.LastName,
                                    rider.Location.Latitude,
                                    rider.Location.Longitude,
                                    directions.Distance.Text,
                                    directions.Duration.Text);

            riderPin.UserName = rider.User.UserName;
            riderPin.UserType = rider.User.UserType;

            mapPageProcessor.AddPin(riderPin);
        }
   
        protected override void OnMapInfoWindowClicked(CustomPin customPin)
        {

        }

        protected override void OnPopupCanceled()
        {

        }

        protected override void OnPopupConfirmed()
        {

        }

        protected override void OnNewCoordinatesRecived()
        {
            RefreshPins(false);
        }

    }
}
