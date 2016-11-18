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
using RideShare.Droid.Services;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.HistoryUpdatorDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class HistoryUpdatorDroid : IHistoryUpdator
    {
        public void StopHistoryUpdatorService()
        {
            MainApp.Context.StopService(new Intent(MainApp.Context, typeof(HistoryRouteUpdatorService)));
        }

        public void StartHistoryUpdatorService()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                MainApp.Context.StartService(new Intent(MainApp.Context, typeof(HistoryRouteUpdatorService)));

            }));
            t.IsBackground = false;
            t.Start();
        }
    }
}