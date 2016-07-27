using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace RideShare
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();

            masterPage.ListView.ItemSelected += OnItemSelected;
            masterPage.ListView.ItemSelected += OnItemSelected2;

            if (Device.OS == TargetPlatform.Windows)
            {
                Master.Icon = "swap.png";
            }
            //Detail = new NavigationPage(new MapView());
        }

     
        public MainPage(NotificationInfo notificationInfo) : this()
        {
            RenderMianPageWithNotificationInfo(notificationInfo);
        }

        void RenderMianPageWithNotificationInfo(NotificationInfo notificationInfo)
        {
            Detail = new NavigationPage(new MapView(notificationInfo));
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;

            if (item != null && item.Title != "Logout")
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                masterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }      
        }

        async void OnItemSelected2(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;

            if (item != null && item.Title == "Logout")
            {
                // Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                masterPage.ListView.SelectedItem = null;
                IsPresented = false;
                var detailPage = new LogoutPage();
                await Navigation.PushModalAsync(detailPage);
            }
        }
    }
}
