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

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.BaseUrlDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class BaseUrlDroid : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}