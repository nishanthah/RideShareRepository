using DriverLocator.Models;
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

        public App(bool isLoading)
        {
            //remove dead code
            //// The root page of your application
            //MainPage = new ContentPage/
            //{
            //    Content = new StackLayout
            //    {
            //        VerticalOptions = LayoutOptions.Center,
            //        Children = {
            //            new Label {
            //               // XAlign = TextAlignment.Center,
            //                Text = "Welcome to Xamarin Forms!"
            //            }
            //        }
            //    }
            //};
            //inPage = new RideShare.MainPage();
            // MainPage = new RideShare.EditProfilePage();
            if (isLoading)
            {
                MainPage = new SplashScreen();
            }
            else
            {
                
                MainPage = new NavigationPage(new LogInPage());
            }

            //MainPage = new NavigationPage(new LogInPage());
            // NavigationPage.SetHasBackButton(MainPage, false);
            // NavigationPage.SetHasBackButton(MainPage, false);
            //  NavigationPage.SetHasBackButton(MasterPage, false);

            //User_Manager = new UserManager(new RideShareService());
        }

        public App(NotificationInfo notificationInfo)
        {
            // The root page of your application
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
