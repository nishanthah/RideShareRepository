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
using UrbanAirship;
using RideShare.Droid.Services;
using System.Threading;

namespace RideShare.Droid
{
    [Application]
    public class MainApp : Application
    {
        public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
                UAirship.TakeOff(this, (UAirship airship) =>
                {
                    UAirship.Shared().PushManager.UserNotificationsEnabled = true;
                });
            }
            catch (Exception ex)
            {                
            }
            
            

            //Thread t = new Thread(new ThreadStart(() => {
            //    StartService(new Intent(this, typeof(LocationUpdatorService)));
            //    StartService(new Intent(this, typeof(HistoryRouteUpdatorService)));
                
            //}));
            //t.IsBackground = false;
            //t.Start();
           
        }        
    }
}