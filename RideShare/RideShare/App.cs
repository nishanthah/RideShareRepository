using DriverLocator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
//using RideShare.Data;

namespace RideShare
{
    public class NotificationInfo
    {
        public string RequestId { get; set; }
        public string LocationName { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

    public class App : Application
    {
        //public static UserManager User_Manager { get; private set; }
        public static UserCoordinate CurrentLoggedUser { get; set; }

        public App()
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
             //MainPage = new RideShare.MainPage();
            // MainPage = new RideShare.EditProfilePage();


            MainPage = new NavigationPage(new LogInPage());
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
