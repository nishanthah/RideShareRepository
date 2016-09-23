using Common.Models;
using DriverLocator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels = Common.Models;
using Xamarin.Forms;

namespace RideShare
{
    public class NotificationData
    {
        public string NotificationId { get; set; }
        public string NotificationText { get; set; }
    }
    public partial class NotificationView : ContentPage
    {
        DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);

        public NotificationView()
        {
            InitializeComponent();
            ShowBusyIndecator();
            Task.Factory.StartNew<List<NotificationData>>(() => {

                return GetNotifications();

            }).ContinueWith((task) => {

                Device.BeginInvokeOnMainThread(() =>
                {
                    listView.ItemsSource = task.Result;
                    HideBusyIndecator();
                });
            });
            
        }

        private List<NotificationData> GetNotifications()
        {
            List<NotificationData> notifications = new List<NotificationData>();

            if (App.CurrentLoggedUser.User.UserType == UserType.Rider)
            {
                var notificationList = driverLocatorService.GetRideHistoryByFilter("userName", App.CurrentLoggedUser.User.UserName).RideHistories.Where(x=>x.RequestStatus != RequestStatus.RideCompleted);
                
                foreach(var notification in notificationList)
                {
                    notifications.Add(new NotificationData() {
                        NotificationId = notification.Id,
                        NotificationText = String.Format("[{0}] {1}",notification.RequestStatus.ToString().ToUpper(),notification.DiverUserName)
                    });
                }
                
            }
            else
            {
                var notificationList = driverLocatorService.GetRideHistoryByFilter("driverUserName", App.CurrentLoggedUser.User.UserName).RideHistories.Where(x => x.RequestStatus != RequestStatus.RideCompleted && x.RequestStatus != RequestStatus.DriverRejected);

                foreach (var notification in notificationList)
                {

                    notifications.Add(new NotificationData() { NotificationId = notification.Id, NotificationText = String.Format("[{0}] {1}", notification.RequestStatus.ToString().ToUpper(), notification.UserName) });
                }
            }
            return notifications;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = e.SelectedItem as NotificationData;
            App.Current.MainPage = new MainPage(new NotificationInfo() { RequestId = selected.NotificationId });
        }

        public void ShowBusyIndecator()
        {
            busyIndicator.IsEnabled = true;
            busyIndicator.IsRunning = true;
            busyIndicator.IsVisible = true;
        }

        public void HideBusyIndecator()
        {
            busyIndicator.IsEnabled = false;
            busyIndicator.IsRunning = false;
            busyIndicator.IsVisible = false;
        }

    }
}
