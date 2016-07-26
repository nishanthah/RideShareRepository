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
using Android.Preferences;
using RideShare.SharedInterfaces;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.AppDataServiceDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class AppDataServiceDroid : IAppDataService
    {
        ISharedPreferences prefs;
        public AppDataServiceDroid()
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
        }

        public void Save(string key, string value)
        {           
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(key, value);
            editor.Apply();           
        }

        public string Get(string key)
        {
            return prefs.GetString(key, null);
        }
    }
}