using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace RideShare
{
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();
        }

        async void OnCancelButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

        async void OnLogOutButtonClicked(object sender, EventArgs args)
        {
            Application.Current.MainPage = new LogInPage();
        }
        
    }
}
