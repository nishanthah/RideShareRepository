using Authentication;
using Authentication.Models;
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
        public static ObservableCollection<FavouritePlace> CurrentUserFavouritePlaces { get; set; }
        IAppDataService appDataService = DependencyService.Get<IAppDataService>();
        public enum DeviceTypes { iOS, Android, Windows };
        public static DeviceTypes DeviceType;
        public static int DeviceVersion;
        public static string DeviceUniqueID;
        public static string ApplicationVersion;


        public App(bool isLoading, string guid)
        {
            if (isLoading)
            {
                MainPage = new SplashScreen();
            }            
            else
            {
                UserInfoResponse userInfo = null;
                MainPage = new SplashScreen();
                Task.Factory.StartNew<bool>(() =>
                {
                    if (String.IsNullOrEmpty(guid))
                    {
                        var accessToken = appDataService.Get("access_token");
                        if (accessToken != null)
                            userInfo = Session.AuthenticationService.GetUserInfo(accessToken);
                        else
                            return false;
                    }
                    else
                    {
                        userInfo = Session.AuthenticationService.GetUserInfoByGUID(guid);
                        if (userInfo.IsSuccess)
                            appDataService.Save("access_token", Session.AuthenticationService.AuthenticationToken);
                    }

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
                        if (userCorrdinateResult.IsSuccess)
                        {
                            App.CurrentLoggedUser = userCorrdinateResult.UserLocation;
                            App.CurrentLoggedUser.User.RegistrationCode = userInfo.RegistrationCode;
                            appDataService.Save("current_user", App.CurrentLoggedUser.User.UserName);
                            appDataService.Save("is_user_logged_in", "true");
                            return true;
                        }
                    }                    

                    return false;

                }).ContinueWith((task) =>
                {

                    Device.BeginInvokeOnMainThread(() =>
                    {

                        if (task.Result)
                        {
                            if (!String.IsNullOrEmpty(userInfo.RegistrationCode))
                                MainPage = new MainPage(new RegistrationComplete());
                            else
                            {
                                if (!String.IsNullOrEmpty(guid))
                                    MainPage = new MainPage(new RegisterPage());
                                else
                                    MainPage = new MainPage();
                            }
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

        public static void LogoutUser()
        {
            if (Session.AuthenticationService != null && App.CurrentLoggedUser != null && App.CurrentLoggedUser.User != null)
            {
                DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                driverLocatorService.UpdateUserLoginStatus(App.CurrentLoggedUser.User.UserName, false);
                driverLocatorService.SendLogoutNotificationSelf(App.CurrentLoggedUser.User.UserName);
                driverLocatorService.SendLogoutNotificationConnections(App.CurrentLoggedUser.User.UserName, Session.CurrentUserType.ToString());                
            }
        }
    }
}
