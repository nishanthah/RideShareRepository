using ImageCircle.Forms.Plugin.Abstractions;
using MediaPicker.Forms.Plugin.Abstractions;
using RideShare.Common;
using RideShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare
{
    public interface IUserVehicleAdditionResult
    {
        Action<ObservableCollection<DriverLocator.Models.Vehicle>> OnUserVehicleAdded { get; set; }
    }

    public partial class RegisterVehicleDetailsPage : ContentPage, ISignUpPageProcessor
    {      
        //String status;
        IUserVehicleAdditionResult userVehicleAdditionResult;
        public RegisterVehicleDetailsPage(IUserVehicleAdditionResult userVehicleAdditionResult, DriverLocator.Models.Vehicle selectedVehicle)
        {
            this.userVehicleAdditionResult = userVehicleAdditionResult;
            InitializeComponent();
            Content.BindingContext = new RegisterVehicleDetailsViewModel(this, selectedVehicle);
            //isNewItem = isNew;
            var vm = Content.BindingContext as RegisterVehicleDetailsViewModel;
            registerButton.Clicked += RegisterButtonClicked;
        }

        private void RegisterButtonClicked(object sender, EventArgs e)
        {
            this.userVehicleAdditionResult.OnUserVehicleAdded(App.CurrentUserVehicles);
        }

        public void MoveToLoginPage()
        {
            App.Current.MainPage = new NavigationPage(new LogInPage());
        }

        public void MoveToMainPage()
        {
            App.Current.MainPage = new MainPage();
        }

        public void MoveToNextPage()
        {
            App.Current.MainPage = new NavigationPage(new LogInPage());
        }

        public void MoveToPreviousPage()
        {
            Navigation.PopModalAsync();            
        }
    }
}
