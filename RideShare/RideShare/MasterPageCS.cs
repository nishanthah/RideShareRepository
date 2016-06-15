using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TryXamarinForms
{
    public class MasterPageCS : ContentPage
    {
        public ListView ListView { get { return listView; } }

        ListView listView;

        public MasterPageCS()
        {
            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Edit Profile",
                IconSource = "contacts.png",
                TargetType = typeof(EditProfilePage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "History",
                IconSource = "contacts.png",
                TargetType = typeof(HistoryPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Settings",
                IconSource = "todo.png",
                TargetType = typeof(SettingsPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "About",
                IconSource = "reminders.png",
                TargetType = typeof(AboutPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Logout",
                IconSource = "reminders.png",
                TargetType = typeof(LogoutPage)
            });

            listView = new ListView
            {
                ItemsSource = masterPageItems,
                ItemTemplate = new DataTemplate(() => {
                    var imageCell = new ImageCell();
                    imageCell.SetBinding(TextCell.TextProperty, "Title");
                    imageCell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");
                    return imageCell;
                }),
                VerticalOptions = LayoutOptions.FillAndExpand,
                SeparatorVisibility = SeparatorVisibility.None
            };

            Padding = new Thickness(0, 40, 0, 0);
            Icon = "hamburger.png";
            Title = "Personal Organiser";
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    listView
                }
            };
        }
    }
}
