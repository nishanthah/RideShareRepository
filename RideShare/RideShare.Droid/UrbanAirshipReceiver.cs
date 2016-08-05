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

        private const string KEY_NOTIFICATION_ACCEPTEDINTENT = "com.virtusa.driverlocatorforms.NOTIFICATIONACCEPTED";
        private const string KEY_NOTIFICATION_REJECTEDINTENT = "com.virtusa.driverlocatorforms.NOTIFICATIONREJECTED";
        private const string KEY_NOTIFICATION_OPENEDINTENT = "com.virtusa.driverlocatorforms.NOTIFICATIONOPENED";

        protected override void OnChannelRegistrationSucceeded(Context context, String channelId)
        {
            Log.Info(TAG, "Channel registration updated. Channel Id:" + channelId);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("urban_airship_client_id", channelId);
            editor.Apply();          
            Intent intent = new Intent(ACTION_CHANNEL_UPDATED);
            LocalBroadcastManager.GetInstance(context).SendBroadcast(intent);

            Intent intent1 = new Intent("com.virtusa.driverlocatorforms.LOADINGCOMPLETED");
            intent1.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
            context.StartActivity(intent1);
        }

        protected override void OnChannelRegistrationFailed(Context context)
        {
            Log.Info(TAG, "Channel registration failed.");
        }

        protected override void OnPushReceived(Context context, PushMessage message, bool notificationPosted)
        {
           
            Log.Info(TAG, "Received push message. Alert: " + message.Alert + ". Notification posted: " + notificationPosted);

            
        }

        protected override void OnNotificationPosted(Context context, AirshipReceiver.NotificationInfo notificationInfo)
        {
            Log.Info(TAG, "Notification posted. Alert: " + notificationInfo.Message.Alert + ". Notification ID: " + notificationInfo.NotificationId);
        }

        protected override bool OnNotificationOpened(Context context, AirshipReceiver.NotificationInfo notificationInfo)
        {
            Log.Info(TAG, "Notification opened. Alert: " + notificationInfo.Message.Alert + ". Notification ID: " + notificationInfo.NotificationId);

            var messageBundle = notificationInfo.Message.PushBundle;
            Intent intent = new Intent(KEY_NOTIFICATION_OPENEDINTENT);
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
            intent.PutExtra(MainActivity.KEY_REQUEST_ID_EXTRA, messageBundle.GetString(MainActivity.KEY_REQUEST_ID_EXTRA));
            context.StartActivity(intent);

            return false;
        }

        protected override bool OnNotificationOpened(Context context, AirshipReceiver.NotificationInfo notificationInfo, AirshipReceiver.ActionButtonInfo actionButtonInfo)
        {
            var messageBundle = notificationInfo.Message.PushBundle;

            if (actionButtonInfo.ButtonId == KEY_ACCEPT_BUTTON)
            {
                Intent intent = new Intent(KEY_NOTIFICATION_ACCEPTEDINTENT);
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
                intent.PutExtra(MainActivity.KEY_REQUEST_ID_EXTRA, messageBundle.GetString(MainActivity.KEY_REQUEST_ID_EXTRA));
                context.StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(KEY_NOTIFICATION_REJECTEDINTENT);
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
                intent.PutExtra(MainActivity.KEY_REQUEST_ID_EXTRA, messageBundle.GetString(MainActivity.KEY_REQUEST_ID_EXTRA));
                context.StartActivity(intent);
            }
            return false;
        }

        protected override void OnNotificationDismissed(Context context, AirshipReceiver.NotificationInfo notificationInfo)
        {
            Log.Info(TAG, "Notification dismissed. Alert: " + notificationInfo.Message.Alert + ". Notification ID: " + notificationInfo.NotificationId);
        }
    }
}