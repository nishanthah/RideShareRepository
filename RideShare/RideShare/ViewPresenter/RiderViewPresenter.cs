using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.ViewPresenter
{
    public class RiderViewPresenter : BaseMapViewPresenter
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);

        //IAppDataService appData;

        public RiderViewPresenter(IMapPageProcessor mapPageProcessor):base(mapPageProcessor)
        {
            RefreshPins();
        }

        protected override void LoadPinData()
        {
            mapPageProcessor.MapPins = new List<CustomPin>();
            var drivers = driverLocatorService.GetDrivers().Result;
            foreach(var driver in drivers.UserLocations)
            {
                MapPin pin = new MapPin();
                pin.ImageIcon = "userLogActive_icon.png";
                pin.Latitude = double.Parse(driver.Location.Latitude);
                pin.Longitude = double.Parse(driver.Location.Longitude);
                pin.PhoneNo = driver.User.MobileNo;
                pin.Title = driver.User.FirstName + " " + driver.User.LastName + " | Position : " + driver.Location.Longitude + " , " + driver.Location.Latitude;
                pin.UserName = driver.User.UserName;
                pin.UserType = driver.User.UserType;
                mapPageProcessor.AddPin(pin);
            }
            
            var riderPin= new MapPin();
            riderPin.ImageIcon = "userLogActive_icon.png";
            riderPin.Latitude = double.Parse(App.CurrentLoggedUser.Location.Latitude);
            riderPin.Longitude = double.Parse(App.CurrentLoggedUser.Location.Longitude);
            riderPin.PhoneNo = App.CurrentLoggedUser.User.MobileNo;
            riderPin.Title = App.CurrentLoggedUser.User.FirstName + " " + App.CurrentLoggedUser.User.LastName + " | Position : " + App.CurrentLoggedUser.Location.Longitude + " , " + App.CurrentLoggedUser.Location.Latitude;
            riderPin.UserName = App.CurrentLoggedUser.User.UserName;
            riderPin.UserType = App.CurrentLoggedUser.User.UserType;
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
            RefreshPins();
        }



    }
}
