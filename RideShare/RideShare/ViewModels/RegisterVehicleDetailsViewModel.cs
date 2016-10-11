using Authentication;
using Authentication.Models;
using DriverLocatorFormsPortable.Common;
using RideShare.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RideShare.ViewModels
{
    public class RegisterVehicleDetailsViewModel : ViewModelBase
    {
        string vehicleMake;
        string vehicleModel;
        string vehicleColor;
        int vehicleMaxPassengerCount;
        string vehicleNumberPlate;
        string previousVehicleNumberPlate;
        ObservableCollection<DriverLocator.Models.VehicleDefinitionData> vehicleDefinitionData;

        ISignUpPageProcessor signUpVehiclePageProcessor;
        public ICommand SignUpCommand { protected set; get; }
        

        public RegisterVehicleDetailsViewModel(ISignUpPageProcessor signUpVehiclePageProcessor, DriverLocator.Models.Vehicle selectedVehicle)
        {
            this.signUpVehiclePageProcessor = signUpVehiclePageProcessor;

            if (selectedVehicle != null)
            {
                if (selectedVehicle.VehicleMake == null)
                    this.vehicleMake = String.Empty;
                else
                    this.vehicleMake = selectedVehicle.VehicleMake;
                
                if (selectedVehicle.VehicleModel == null)
                    this.vehicleModel = String.Empty;
                else
                    this.vehicleModel = selectedVehicle.VehicleModel;

                if (selectedVehicle.VehicleColor == null)
                    this.vehicleColor = String.Empty;
                else
                    this.vehicleColor = selectedVehicle.VehicleColor;

                this.vehicleMaxPassengerCount = selectedVehicle.VehicleMaxPassengerCount;

                if (selectedVehicle.VehicleNumberPlate == null)
                    previousVehicleNumberPlate = this.vehicleNumberPlate = String.Empty;
                else
                    previousVehicleNumberPlate = this.vehicleNumberPlate = selectedVehicle.VehicleNumberPlate;

                vehicleDefinitionData = GetVehicleDefinitionData();                
            }

            this.SignUpCommand = new RelayCommand(UpdateVehicleDetails);
        }

        public bool isAuthenticated
        {
            get { return Session.AuthenticationService.IsAuthenticated; }
        }

        public string VehicleMake
        {
            get
            {
                return vehicleMake;
            }

            set
            {
                vehicleMake = value;
                OnPropertyChanged("VehicleMake");
            }
        }

        public string VehicleModel
        {
            get
            {
                return vehicleModel;
            }

            set
            {
                vehicleModel = value;
                OnPropertyChanged("VehicleModel");
            }
        }

        public string VehicleColor
        {
            get
            {
                return vehicleColor;
            }

            set
            {
                vehicleColor = value;
                OnPropertyChanged("VehicleColor");
            }
        }

        public string VehicleMaxPassengerCount
        {
            get
            {
                if (vehicleMaxPassengerCount == 0)
                    return String.Empty;
                else
                    return vehicleMaxPassengerCount.ToString();
            }

            set
            {
                int outputValue;

                if (!String.IsNullOrEmpty(value))
                {
                    if(Int32.TryParse(value, out outputValue))
                        vehicleMaxPassengerCount = Convert.ToInt32(value);
                    else
                        vehicleMaxPassengerCount = 0;
                }
                else
                    vehicleMaxPassengerCount = 0;
                OnPropertyChanged("VehicleMaxPassengerCount");
            }
        }

        public string VehicleNumberPlate
        {
            get
            {
                return vehicleNumberPlate;
            }

            set
            {
                vehicleNumberPlate = value;
                OnPropertyChanged("VehicleNumberPlate");
            }
        }        

        public ObservableCollection<DriverLocator.Models.VehicleDefinitionData> VehicleDefinitionData
        {
            get
            {
                return vehicleDefinitionData;
            }

            set
            {
                vehicleDefinitionData = value;
                OnPropertyChanged("VehicleDefinitionData");
            }
        }

        private void UpdateVehicleDetails()
        {
            UpdateVehicleInLocal();

            this.signUpVehiclePageProcessor.MoveToPreviousPage();
        }

        public void UpdateVehicleInLocal()
        {
            bool exists = false;

            if (App.CurrentUserVehicles == null || App.CurrentUserVehicles.Count == 0)
                App.CurrentUserVehicles = new ObservableCollection<DriverLocator.Models.Vehicle>();

            DriverLocator.Models.Vehicle newUserVehicle = new DriverLocator.Models.Vehicle() 
            { 
                VehicleMake = this.vehicleMake, VehicleModel = this.vehicleModel, VehicleColor = this.vehicleColor, VehicleMaxPassengerCount = this.vehicleMaxPassengerCount,
                VehicleNumberPlate = this.vehicleNumberPlate,
                PreviousVehicleNumberPlate = this.previousVehicleNumberPlate,
                VehicleDisplayName = String.Format("{0} {1}", this.vehicleModel, this.vehicleNumberPlate) 
            };

            foreach (DriverLocator.Models.Vehicle vehicle in App.CurrentUserVehicles)
            {
                if (vehicle.VehicleNumberPlate == this.previousVehicleNumberPlate)
                {
                    App.CurrentUserVehicles.Remove(vehicle);
                    App.CurrentUserVehicles.Add(newUserVehicle);
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                if (!Session.AuthenticationService.IsAuthenticated)
                    newUserVehicle.PreviousVehicleNumberPlate = this.vehicleNumberPlate;
                if (!App.CurrentUserVehicles.Contains(newUserVehicle))
                    App.CurrentUserVehicles.Add(newUserVehicle);
            }     
        }        

        private ObservableCollection<DriverLocator.Models.VehicleDefinitionData> GetVehicleDefinitionData()
        {
            ObservableCollection<DriverLocator.Models.VehicleDefinitionData> returnList = new ObservableCollection<DriverLocator.Models.VehicleDefinitionData>();
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
            var userVehicleDefinitionDataResult = driverLocatorService.GetUserVehicleDefinitionData();
            returnList = userVehicleDefinitionDataResult.UserVehicleDefinitionData;
            return returnList;
        }
    }
}
