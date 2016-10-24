using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RideShare.SharedInterfaces;
using Uri = Android.Net.Uri;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.NavigationServiceDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class NavigationServiceDroid : INavigationService
    {
        public void ShowNavigationApp(double longitude, double latitude)
        {
            Uri gmmIntentUri = Uri.Parse(String.Format("google.navigation:q={0},{1}",latitude,longitude));
            Intent mapIntent = new Intent(Intent.ActionView, gmmIntentUri);
            mapIntent.SetFlags(ActivityFlags.NewTask);
            mapIntent.SetPackage("com.google.android.apps.maps");
            MainApp.Context.StartActivity(mapIntent);
        }
    }
}