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
using UrbanAirship.Push;
using UrbanAirship;
using Android.Support.V4.Content;
using Android.Util;
using RideShare.SharedInterfaces;
using UrbanAirshipClient;
using Android.Preferences;
using UrbanAirship.RichPush;

namespace RideShare.Droid
{
    [BroadcastReceiver(Exported = false)]
    [IntentFilter(new string[] { "com.urbanairship.push.CHANNEL_UPDATED", "com.urbanairship.push.OPENED", "com.urbanairship.push.DISMISSED", "com.urbanairship.push.RECEIVED" },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    public class UrbanAirshipReceiver : AirshipReceiver
    {

        public const string ACTION_CHANNEL_UPDATED = "channel_updated";
        public const string KEY_ACCEPT_BUTTON = "accept";

        private const string TAG = "UrbanAirshipReceiver";

        private const string KEY_NOTIFICATION_INTENT = "com.virtusa.driverlocatorforms.NOTIFICATION";

        protected override void OnChannelRegistrationSucceeded(Context context, String channelId)
        {
            Log.Info(TAG, "Channel registration updated. Channel Id:" + channelId);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("urban_airship_client_id", channelId);
            // editor.Commit();    // applies changes synchronously on older APIs
            editor.Apply();          
            Intent intent = new Intent(ACTION_CHANNEL_UPDATED);
            LocalBroadcastManager.GetInstance(context).SendBroadcast(intent);

            Intent intent1 = new Intent("com.virtusa.driverlocatorforms.LOADINGCOMPLETED");
            intent1.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
            context.StartActivity(intent1);
            //com.virtusa.driverlocatorforms.LOADINGCOMPLETED
            //OnRegistrationSucceeded(this,new OnRegistrationSucceededEventArgs() {ChannelId = channelId });
        }

        protected override void OnChannelRegistrationFailed(Context context)
        {
            Log.Info(TAG, "Channel registration failed.");
            //OnRegistrationFailed(this, null);
        }

        protected override void OnPushReceived(Context context, PushMessage message, bool notificationPosted)
        {
           
            Log.Info(TAG, "Received push message. Alert: " + message.Alert + ". Notification posted: " + notificationPosted);
            //RichPushMessage message = UAirship.Shared().PushManager.
            //OnPushMessageReceived(this, new OnPushMessageReceivedEventArgs() { Message = message.Alert });
            
        }

        protected override void OnNotificationPosted(Context context, AirshipReceiver.NotificationInfo notificationInfo)
        {
            Log.Info(TAG, "Notification posted. Alert: " + notificationInfo.Message.Alert + ". Notification ID: " + notificationInfo.NotificationId);
            //OnPushNotificationPosted(this, new OnPushNotificationPostedEventArgs() { Message = notificationInfo.Message.Alert, NotificationId = notificationInfo.NotificationId });
        }

        protected override bool OnNotificationOpened(Context context, AirshipReceiver.NotificationInfo notificationInfo)
        {
            Log.Info(TAG, "Notification opened. Alert: " + notificationInfo.Message.Alert + ". Notification ID: " + notificationInfo.NotificationId);

            //OnPushNotificationOpened(this, new OnPushNotificationOpenedEventArgs() { Message = notificationInfo.Message.Alert, NotificationId = notificationInfo.NotificationId, Button= ButtonType.None });
            // Return false here to allow Urban Airship to auto launch the launcher
            // activity for foreground notification action buttons
            return false;
        }

        protected override bool OnNotificationOpened(Context context, AirshipReceiver.NotificationInfo notificationInfo, AirshipReceiver.ActionButtonInfo actionButtonInfo)
        {
            var messageBundle = notificationInfo.Message.PushBundle;

            if (actionButtonInfo.ButtonId == KEY_ACCEPT_BUTTON)
            {
                Intent intent = new Intent(KEY_NOTIFICATION_INTENT);
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
                intent.PutExtra(MainActivity.KEY_REQUEST_ID_EXTRA, messageBundle.GetString(MainActivity.KEY_REQUEST_ID_EXTRA));
                intent.PutExtra(MainActivity.KEY_SOURCE_NAME_EXTRA, messageBundle.GetString(MainActivity.KEY_SOURCE_NAME_EXTRA));
                intent.PutExtra(MainActivity.KEY_SOURCE_LATITUDE_EXTRA, messageBundle.GetString(MainActivity.KEY_SOURCE_LATITUDE_EXTRA));
                intent.PutExtra(MainActivity.KEY_SOURCE_LONGITUDE_EXTRA, messageBundle.GetString(MainActivity.KEY_SOURCE_LONGITUDE_EXTRA));
                intent.PutExtra(MainActivity.KEY_DESTINATION_NAME_EXTRA, messageBundle.GetString(MainActivity.KEY_DESTINATION_NAME_EXTRA));
                intent.PutExtra(MainActivity.KEY_DESTINATION_LATITUDE_EXTRA, messageBundle.GetString(MainActivity.KEY_DESTINATION_LATITUDE_EXTRA));
                intent.PutExtra(MainActivity.KEY_DESTINATION_LONGITUDE_EXTRA, messageBundle.GetString(MainActivity.KEY_DESTINATION_LONGITUDE_EXTRA));
                context.StartActivity(intent);
            }
            else
            {

            }
            return false;
        }

        protected override void OnNotificationDismissed(Context context, AirshipReceiver.NotificationInfo notificationInfo)
        {
            Log.Info(TAG, "Notification dismissed. Alert: " + notificationInfo.Message.Alert + ". Notification ID: " + notificationInfo.NotificationId);
            //OnPushNotificationDismissed(this, new OnPushNotificationDismissedEventArgs() { Message = notificationInfo.Message.Alert, NotificationId = notificationInfo.NotificationId });
        }
    }
}