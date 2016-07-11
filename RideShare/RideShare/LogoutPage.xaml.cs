using RideShare.ViewModels;
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
            Content.BindingContext = new LogOutViewModel();
        }

        //TODO: Need to findout how to move this to viewModel
        async void OnCancelButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

    }
}
