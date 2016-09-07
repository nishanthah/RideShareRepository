using DriverLocator.Models;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
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
        IAppDataService appDataService = DependencyService.Get<IAppDataService>();
        public App(bool isLoading)
        {
            
            if (isLoading)
            {
                MainPage = new SplashScreen();
            }
            else
            {
                
                MainPage = new NavigationPage(new LogInPage());
            }
        }

        public App(NotificationInfo notificationInfo)
        {
            // The root page of your application
            appDataService.Save("currentRequestId", notificationInfo.RequestId);
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
