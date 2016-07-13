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
using Android.Preferences;
using UrbanAirshipClient;
using UrbanAirship;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.UrbanAirshipServiceDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class UrbanAirshipServiceDroid : IUrbanAirshipNotificationService
    {
        public void InitializeNamedUser(string userName)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            string channelId = prefs.GetString("urban_airship_client_id", null);

            UrbanAirshipApiClient urbanAirshipClient = new UrbanAirshipApiClient(Session.AppKey, Session.AppMasterSecret);
            //urbanAirshipClient.RegisterUser(new UrbanAirshipClient.Models.RegisterUserRequest() { ChannelId = channelId, DeviceType = "android", NamedUserId = userName });
            UAirship.Shared().PushManager.NamedUser.Id = userName;
            //UAirship.Shared().PushManager.UpdateRegistration();
        }
    }
}