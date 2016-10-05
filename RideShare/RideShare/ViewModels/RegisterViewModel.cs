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
    public class SignUpViewModel : ViewModelBase
    {
        string firstName;
        string lastName;
        string userName;
        string password;
        string email;
        byte[] _profilePhoto;
        string profilePictureEncoded = string.Empty;
        ObservableCollection<DriverLocator.Models.Vehicle> vehicles; 


        ISignUpPageProcessor signUpPageProcessor;
        public ICommand TapCommand { protected set; get; }

        public SignUpViewModel(ISignUpPageProcessor signUpPageProcessor)
        {
            this.signUpPageProcessor = signUpPageProcessor;
            this.TapCommand = new RelayCommand(OnTapped);
            vehicles = new ObservableCollection<DriverLocator.Models.Vehicle>();
            if (Session.AuthenticationService.IsAuthenticated)
            {
                var currentUserDetails = App.CurrentLoggedUser.User;
                this.FirstName = currentUserDetails.FirstName;
                this.LastName = currentUserDetails.LastName;
                this.UserName = currentUserDetails.UserName;
                this.Email = currentUserDetails.EMail;
                if (!String.IsNullOrEmpty(currentUserDetails.profileImageEncoded))
                {
                    this.ProfilePhoto = Convert.FromBase64String(currentUserDetails.profileImageEncoded);
                }
                vehicles = App.CurrentLoggedUser.Vehicles;
                this.SignUpCommand = new RelayCommand(Update);                
            }
            else
            {
                Session.AuthenticationService = new AuthenticationService();
                this.SignUpCommand = new RelayCommand(SignUp);

                if (App.CurrentUserVehicles != null)
                    App.CurrentUserVehicles.Clear();
            }

        }

        public ICommand SignUpCommand { protected set; get; }

        

        public bool isAuthenticated
        {
            get { return Session.AuthenticationService.IsAuthenticated; }
        }

        public byte[] ProfilePhoto
        {
            get
            {
                return _profilePhoto;
            }

            set
            {
                _profilePhoto = value;
                OnPropertyChanged("ProfilePhoto");
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        public ObservableCollection<DriverLocator.Models.Vehicle> Vehicles
        {
            get
            {
                return vehicles;
            }

            set
            {
                vehicles = value;
                OnPropertyChanged("Vehicles");
            }
        }

        private void SignUp()
        {
            var user = new User()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                UserName = this.UserName,
                EMail = this.Email,
                Password = this.Password,
                profileImageEncoded = GetProfilePictureEncoded()
            };
            

            var signUpSucceeded = AreDetailsValid(user);

            if (signUpSucceeded)
            {

                var result = Session.AuthenticationService.CreateUser(user);
                if (Session.AuthenticationService.Authenticate(user.UserName, user.Password))
                {
                    UpdateUserInLocal();

                    if (App.CurrentUserVehicles != null)
                        UpdateVehiclesInLocal();
                        
                    
                    DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                    var userCorrdinateResult = driverLocatorService.GetSelectedUserCoordinate(this.userName);

                    if (userCorrdinateResult.IsSuccess)
                    {
                        App.CurrentLoggedUser = userCorrdinateResult.UserLocation;                                               
                    }                    

                    this.signUpPageProcessor.MoveToLoginPage();
                }
                else
                    this.signUpPageProcessor.MoveToLoginPage();
            }
            else
            {
                ErrorMessage = "Sign up failed";
            }
        }

        private void Update()
        {
            var user = new User()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                UserName = this.UserName,
                EMail = this.Email,
                Password = this.Password,
                profileImageEncoded = GetProfilePictureEncoded()
            };

            var Isvalid = AreDetailsValid(user, true);

            if (Isvalid)
            {
                var result = Session.AuthenticationService.UpdateUser(user);
                UpdateUserInLocal();

                if (App.CurrentUserVehicles != null)
                    UpdateVehiclesInLocal();
                    

                DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                var userCorrdinateResult = driverLocatorService.GetSelectedUserCoordinate(this.userName);

                if (userCorrdinateResult.IsSuccess)
                {
                    App.CurrentLoggedUser = userCorrdinateResult.UserLocation;
                }                

                this.signUpPageProcessor.MoveToMainPage();
            }
            else
            {
                ErrorMessage = "Update failed";
            }
        }

        public string GetProfilePictureEncoded()
        {
            if (this.ProfilePhoto != null && this.ProfilePhoto.Length != 0)
            {
                profilePictureEncoded = Convert.ToBase64String(this.ProfilePhoto);
            }
            return profilePictureEncoded;
        }


        bool AreDetailsValid(User user, bool isUpdate = false)
        {
            if(!isUpdate)
                return (!string.IsNullOrWhiteSpace(user.UserName) &&
                !string.IsNullOrWhiteSpace(user.Password) &&
                !string.IsNullOrWhiteSpace(user.UserName) &&
                !string.IsNullOrWhiteSpace(user.EMail));
            else
                return (!string.IsNullOrWhiteSpace(user.UserName) &&
               !string.IsNullOrWhiteSpace(user.UserName) &&
               !string.IsNullOrWhiteSpace(user.EMail));
        }

        public void UpdateUserInLocal()
        {
            var result = Session.AuthenticationService.GetUserInfo(Session.AuthenticationService.AuthenticationToken);
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
            DriverLocator.Models.User dlUser = new DriverLocator.Models.User();
            dlUser.UserName = result.UserName;
            dlUser.FirstName = result.FirstName;
            dlUser.LastName = result.LastName;
            dlUser.EMail = result.EMail;
            dlUser.profileImageEncoded = result.profileImageEncoded;
            var response = driverLocatorService.SaveUserData(dlUser);
        }

        public void UpdateVehiclesInLocal()
        {
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);

            if (App.CurrentUserVehicles != null)
            {
                foreach (DriverLocator.Models.Vehicle v in App.CurrentUserVehicles)
                {
                    DriverLocator.Models.UpdateVehicleDetailsRequest dlVehicleRequest = new DriverLocator.Models.UpdateVehicleDetailsRequest();
                    dlVehicleRequest.UserName = App.CurrentLoggedUser.User.UserName;
                    dlVehicleRequest.VehicleMake = v.VehicleMake;
                    dlVehicleRequest.VehicleModel = v.VehicleModel;
                    dlVehicleRequest.VehicleColor = v.VehicleColor;
                    dlVehicleRequest.VehicleMaxPassengerCount = v.VehicleMaxPassengerCount;
                    dlVehicleRequest.VehicleNumberPlate = v.VehicleNumberPlate;
                    dlVehicleRequest.PreviousVehicleNumberPlate = v.PreviousVehicleNumberPlate;
                    var response = driverLocatorService.UpdateVehicleDetails(dlVehicleRequest);
                }
            }            
        }


        private void OnTapped ()
        {
            this.signUpPageProcessor.MoveToNextPage();
        }
    }
}
