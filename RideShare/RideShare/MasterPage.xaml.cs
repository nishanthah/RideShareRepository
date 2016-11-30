using ImageCircle.Forms.Plugin.Abstractions;
using RideShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace RideShare
{
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        CircleImage profilePhoto;

        public MasterPage()
        {
            InitializeComponent();

            Content.BindingContext = new MasterPageViewModel();

            var vm = Content.BindingContext as MasterPageViewModel;
            profilePhoto = new CircleImage()
            {
                HeightRequest = 75,
                WidthRequest = 75,
                VerticalOptions = LayoutOptions.Center,
                Source = vm.ProfileImageSource
            };

            profilePictureStackLayout.Children.Add(profilePhoto);

            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Notifications",
                IconSource = "notification.png",
                TargetType = typeof(NotificationView),
                IsEnabled = String.IsNullOrEmpty(App.CurrentLoggedUser.User.RegistrationCode)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Edit Profile",
                IconSource = "edit.png",
                TargetType = typeof(RegisterPage),
                IsEnabled = String.IsNullOrEmpty(App.CurrentLoggedUser.User.RegistrationCode)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "History",
                IconSource = "history.png",
                TargetType = typeof(HistoryView),
                IsEnabled = String.IsNullOrEmpty(App.CurrentLoggedUser.User.RegistrationCode)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Settings",
                IconSource = "settings.png",
                TargetType = typeof(SettingsPage),
                IsEnabled = String.IsNullOrEmpty(App.CurrentLoggedUser.User.RegistrationCode)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "About",
                IconSource = "about.png",
                TargetType = typeof(AboutPage),
                IsEnabled = true
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = Session.CurrentUserType == global::Common.Models.UserType.Driver ? "Provide Ride" : "Get Ride",
                IconSource = "map.png",
                TargetType = typeof(MapView),
                IsEnabled = String.IsNullOrEmpty(App.CurrentLoggedUser.User.RegistrationCode)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Logout",
                IconSource = "logout.png",
                TargetType = typeof(LogoutPage),
                IsEnabled = true
            });
            
            listView.ItemsSource = masterPageItems;
        }

        
      
    }
}
