using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.SharedInterfaces
{
    public interface IUrbanAirshipNotificationService
    {
        //event EventHandler OnRegistrationSucceeded;
        //event EventHandler OnRegistrationFailed;
        //event EventHandler OnPushMessageReceived;
        //event EventHandler OnPushNotificationPosted;
        //event EventHandler OnPushNotificationOpened;
        //event EventHandler OnPushNotificationDismissed;

        void InitializeNamedUser(string userName);
    }

    public enum ButtonType
    {
        Confirm,
        Reject,
        None
    }
    public class OnRegistrationSucceededEventArgs : EventArgs
    {
        public string ChannelId { get; set; }
    }

    public class OnPushMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class OnPushNotificationPostedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int NotificationId { get; set; }
    }

    public class OnPushNotificationOpenedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int NotificationId { get; set; }
        public ButtonType Button  { get; set; }
    }

    public class OnPushNotificationDismissedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int NotificationId { get; set; }
    }
}
