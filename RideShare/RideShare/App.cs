using Authentication;
using DriverLocator.Models;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CommonModels = Common.Models;
//using RideShare.Data;

namespace RideShare
{
    public class Coordinate
    {
        public string LocationName { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

    public enum NotificationStatus
    {
        Accepted,
        Rejected,
        Opened
    }

    public class NotificationInfo
    {
        public string RequestId { get; set; }
        public NotificationStatus NotificationStatus { get; set; }
    }

    public class App : Application
    {
        //public static UserManager User_Manager { get; private set; }
        public static UserLocation CurrentLoggedUser { get; set; }
        public static ObservableCollection<Vehicle> CurrentUserVehicles { get; set; }
        IAppDataService appDataService = DependencyService.Get<IAppDataService>();
        

        public App(bool isLoading)
        { 
            if (isLoading)
            {
                MainPage = new SplashScreen();
            }
            else
            {
                MainPage = new SplashScreen();
                Session.AuthenticationService = new AuthenticationService();
                Task.Factory.StartNew<bool>(() => {

                    var accessToken = appDataService.Get("access_token");
                    if (accessToken != null)
                    {
                        var userInfo = Session.AuthenticationService.GetUserInfo(accessToken);
                        if (userInfo.IsSuccess)
                        {
#if WithNotification
                        
#else
                            IUrbanAirshipNotificationService urbanAirshipNotificationService = DependencyService.Get<IUrbanAirshipNotificationService>();
                            urbanAirshipNotificationService.InitializeNamedUser(userInfo.UserName);
#endif

                            Session.CurrentUserName = userInfo.UserName;
                            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                            var userCorrdinateResult = driverLocatorService.GetSelectedUserCoordinate(userInfo.UserName);
                            App.CurrentLoggedUser = userCorrdinateResult.UserLocation;
                            appDataService.Save("current_user", App.CurrentLoggedUser.User.UserName);
                            return true;
                        }
                    }

                    return false;

                }).ContinueWith((task) => {

                    Device.BeginInvokeOnMainThread(() =>
                    {

                        if (task.Result)
                        {
                            MainPage = new MainPage();
                        }
                        else
                        {
                            MainPage = new NavigationPage(new LogInPage());
                        }

                    });
                       
                    
                });
                
            }
        }

        public App(NotificationInfo notificationInfo)
        {
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
            // The root page of your application
            appDataService.Save("currentRequestId", notificationInfo.RequestId);

            Task.Factory.StartNew(async() => {
                await driverLocatorService.UpdateNotificationStatus(notificationInfo.RequestId, CommonModels.NotificationStatus.Delivered);
            });
            
            MainPage = new MainPage(notificationInfo);


        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
