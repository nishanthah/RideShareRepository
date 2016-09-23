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
    public class DriverViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService;

        public DriverViewPresenter(IMapPageProcessor mapPageProcessor, IMapSocketService mapSocketService, DriverLocator.DriverLocatorService driverLocatorService) :base(mapPageProcessor,mapSocketService,driverLocatorService)
        {
            this.driverLocatorService = driverLocatorService;
            base.InitDestination();
            RefreshPins(true);
            base.OnInitializationCompleted();
        }

        bool isRideRequested = false;
        string rideHistoryId = String.Empty;

        protected override List<CustomPin> LoadPinData()
        {

            List<CustomPin> mapPins = new List<CustomPin>();
            var riders = driverLocatorService.GetRiders().Result;

            
            foreach (var rider in riders.UserLocations)
            {
                if(rider.Location.Latitude != null && rider.Location.Longitude != null)
                {
                    MapPin pin = new MapPin();
                    pin.ImageIcon = "userLogActive_icon.png";
                    pin.Latitude = double.Parse(rider.Location.Latitude);
                    pin.Longitude = double.Parse(rider.Location.Longitude);
                    pin.PhoneNo = rider.User.MobileNo;
                    pin.Title = rider.User.FirstName + " " + rider.User.LastName;
                    pin.UserName = rider.User.UserName;
                    pin.UserType = rider.User.UserType;
                    mapPins.Add(GetFromatted(pin));
                }
                
            }

            if (App.CurrentLoggedUser.Location.Latitude != null && App.CurrentLoggedUser.Location.Longitude != null)
            {
                var driverPin = new MapPin();
                driverPin.ImageIcon = "userLogActive_icon.png";
                driverPin.Latitude = double.Parse(App.CurrentLoggedUser.Location.Latitude);
                driverPin.Longitude = double.Parse(App.CurrentLoggedUser.Location.Longitude);
                driverPin.PhoneNo = App.CurrentLoggedUser.User.MobileNo;
                driverPin.Title = App.CurrentLoggedUser.User.FirstName + " " + App.CurrentLoggedUser.User.LastName + " | Position : " + App.CurrentLoggedUser.Location.Longitude + " , " + App.CurrentLoggedUser.Location.Latitude;
                driverPin.UserName = App.CurrentLoggedUser.User.UserName;
                driverPin.UserType = App.CurrentLoggedUser.User.UserType;
                mapPins.Add(GetFromatted(driverPin));
            }
                

            return mapPins;
        }

        protected override void OnMapInfoWindowClicked(CustomPin customPin)
        {
            //mapPageProcessor.ShowDoubleButtonPopup("Are you sure you want to send the pickup request to this driver?","Send Request","Cancel");
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
                Id = mapPin.UserName
            };
            return pin;
        }

        protected override void OnNewCoordinatesRecived()
        {
            RefreshPins(false);
        }
    }
}
