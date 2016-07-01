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

        public MasterPage()
        {
            InitializeComponent();

            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Edit Profile",
                IconSource = "edit.png",
                TargetType = typeof(EditProfilePage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "History",
                IconSource = "history.png",
                TargetType = typeof(HistoryPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Settings",
                IconSource = "settings.png",
                TargetType = typeof(SettingsPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "About",
                IconSource = "about.png",
                TargetType = typeof(AboutPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Logout",
                IconSource = "logout.png",
                TargetType = typeof(LogoutPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Map View",
                IconSource = "logout.png",
                TargetType = typeof(MapView2)
            });

            listView.ItemsSource = masterPageItems;
        }
    }
}
