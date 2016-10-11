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
        IUserVehicleAdditionResult userVehicleAdditionResult;
        public RegisterVehicleDetailsPage(IUserVehicleAdditionResult userVehicleAdditionResult, DriverLocator.Models.Vehicle selectedVehicle)
        {
            this.userVehicleAdditionResult = userVehicleAdditionResult;
            InitializeComponent();
            Content.BindingContext = new RegisterVehicleDetailsViewModel(this, selectedVehicle);
            var vm = Content.BindingContext as RegisterVehicleDetailsViewModel;
            registerButton.Clicked += RegisterButtonClicked;
            vehicleMakePicker.SelectedIndexChanged += vehicleMakePicker_SelectedIndexChanged;
            vehicleModelPicker.SelectedIndexChanged += vehicleModelPicker_SelectedIndexChanged;

            vehicleMakePicker.Items.Add("Vehicle Make");
            if (vm.VehicleDefinitionData != null && vm.VehicleDefinitionData.Count != 0)
            {
                var thisMakes = vm.VehicleDefinitionData.Select(x => x.Make).Distinct();
                foreach (string item in thisMakes)
                {
                    vehicleMakePicker.Items.Add(item);
                }

                if (String.IsNullOrEmpty(vm.VehicleMake))
                    vehicleMakePicker.SelectedIndex = 0;
                else
                    vehicleMakePicker.SelectedIndex = vehicleMakePicker.Items.IndexOf(vehicleMakePicker.Items.Single(vDef => vDef == vm.VehicleMake));
            }
            else
                vehicleMakePicker.SelectedIndex = 0;               
        }

        void vehicleModelPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetVehicleDetails();
        }

        void vehicleMakePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            vehicleModelPicker.Items.Clear();
            vehicleModelPicker.Items.Add("Vehicle Model");
            var vm = Content.BindingContext as RegisterVehicleDetailsViewModel;
            if (vm.VehicleDefinitionData != null && vm.VehicleDefinitionData.Count != 0)
            {
                var thisModels = vm.VehicleDefinitionData.Where(vDef => vDef.Make == vehicleMakePicker.Items[vehicleMakePicker.SelectedIndex]).Select(vDef => vDef.Model);
                                
                foreach (string item in thisModels)
                {
                    vehicleModelPicker.Items.Add(item);
                }

                if (String.IsNullOrEmpty(vm.VehicleModel))
                    vehicleModelPicker.SelectedIndex = 0;
                else
                    vehicleModelPicker.SelectedIndex = vehicleModelPicker.Items.IndexOf(vehicleModelPicker.Items.Single(vDef => vDef == vm.VehicleModel));
            }
            else
                vehicleModelPicker.SelectedIndex = 0;

            SetVehicleDetails();
        }

        private void SetVehicleDetails()
        {
            var vm = Content.BindingContext as RegisterVehicleDetailsViewModel;

            if (vehicleMakePicker.SelectedIndex != 0 && vehicleMakePicker.SelectedIndex != -1)
                vm.VehicleMake = vehicleMakePicker.Items[vehicleMakePicker.SelectedIndex];
            else
                vm.VehicleMake = String.Empty;

            if (vehicleModelPicker.SelectedIndex != 0 && vehicleModelPicker.SelectedIndex != -1)
                vm.VehicleModel = vehicleModelPicker.Items[vehicleModelPicker.SelectedIndex];
            else
                vm.VehicleModel = String.Empty;
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
