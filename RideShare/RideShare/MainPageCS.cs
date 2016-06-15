using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TryXamarinForms
{
    public class MainPageCS : MasterDetailPage
    {
        MasterPageCS masterPage;

        public MainPageCS()
        {
            masterPage = new MasterPageCS();
            Master = masterPage;
            Detail = new NavigationPage(new ContactsPageCS());

            masterPage.ListView.ItemSelected += OnItemSelected;

            if (Device.OS == TargetPlatform.Windows)
            {
                Master.Icon = "swap.png";
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                masterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}
