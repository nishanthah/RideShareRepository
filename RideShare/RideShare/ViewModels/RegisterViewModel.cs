using Authentication;
using Authentication.Models;
using DriverLocatorFormsPortable.Common;
using RideShare.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RideShare.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        string firstName = String.Empty;
        string lastName = String.Empty;
        string userName = String.Empty;
        string password = String.Empty;
        string email = String.Empty;
        string gender;
        byte[] _profilePhoto;
        string profilePictureEncoded = string.Empty;
        bool isButtonEnabled;
        ObservableCollection<DriverLocator.Models.Vehicle> vehicles;
        ObservableCollection<DriverLocator.Models.FavouritePlace> favPlaces;
        string passwordErrorMessage = "The password must be 8-15 characters long and must include atleast one capital letter and a special character";
        string requiredFieldErrorMessage = "Required";
        string emailAddressErrorMessage = "Invalid email";
        string errorMessage;


        ISignUpPageProcessor signUpPageProcessor;
        public ICommand TapCommand { protected set; get; }
        public ICommand TapCommandLogin { protected set; get; }
        public ICommand TapFavsCommand { protected set; get; }

        public SignUpViewModel(ISignUpPageProcessor signUpPageProcessor)
        {
            this.signUpPageProcessor = signUpPageProcessor;
            this.TapCommand = new RelayCommand(OnTapped);
            this.TapCommandLogin = new RelayCommand(OnTappedLogin);
            this.TapFavsCommand = new RelayCommand(OnTappedFavs);
            vehicles = new ObservableCollection<DriverLocator.Models.Vehicle>();
            favPlaces = new ObservableCollection<DriverLocator.Models.FavouritePlace>();
            if (Session.AuthenticationService != null && Session.AuthenticationService.IsAuthenticated)
            {
                var currentUserDetails = App.CurrentLoggedUser.User;
                this.FirstName = currentUserDetails.FirstName;
                this.LastName = currentUserDetails.LastName;
                this.UserName = currentUserDetails.UserName;
                this.Email = currentUserDetails.EMail;
                this.gender = currentUserDetails.Gender;
                this.password = "********";
                if (!String.IsNullOrEmpty(currentUserDetails.profileImageEncoded))
                {
                    this.ProfilePhoto = Convert.FromBase64String(currentUserDetails.profileImageEncoded);
                }
                vehicles = App.CurrentUserVehicles = App.CurrentLoggedUser.Vehicles;
                favPlaces = App.CurrentUserFavouritePlaces = App.CurrentLoggedUser.FavouritePlaces;
                this.SignUpCommand = new RelayCommand(Update);
            }
            else
            {
                this.SignUpCommand = new RelayCommand(SignUp);

                if (App.CurrentUserVehicles != null)
                    App.CurrentUserVehicles.Clear();

                if (App.CurrentUserFavouritePlaces != null)
                    App.CurrentUserFavouritePlaces.Clear();
            }

        }

        public ICommand SignUpCommand { protected set; get; }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        public bool isAuthenticated
        {
            get { return Session.AuthenticationService.IsAuthenticated; }
        }

        public bool IsButtonEnabled
        {
            get
            {
                return isButtonEnabled;
            }

            set
            {
                isButtonEnabled = value;
                OnPropertyChanged("IsButtonEnabled");
            }
        }

        public string PasswordErrorMessage
        {
            get { return passwordErrorMessage; }
        }

        public string RequiredFieldErrorMessage
        {
            get { return requiredFieldErrorMessage; }
        }

        public string EmailAddressErrorMessage
        {
            get { return emailAddressErrorMessage; }
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
                CheckFormValiditiy();
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
                CheckFormValiditiy();
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
                userName = value.Replace(" ", String.Empty);
                OnPropertyChanged("UserName");
                CheckFormValiditiy();
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
                password = value.Replace(" ", String.Empty);
                OnPropertyChanged("Password");
                CheckFormValiditiy();
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
                CheckFormValiditiy();
            }
        }

        public string Gender
        {
            get
            {
                return gender;
            }

            set
            {
                gender = value;
                OnPropertyChanged("Gender");
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

        public ObservableCollection<DriverLocator.Models.FavouritePlace> FavPlaces
        {
            get
            {
                return favPlaces;
            }

            set
            {
                favPlaces = value;
                OnPropertyChanged("FavPlaces");
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
                profileImageEncoded = GetProfilePictureEncoded(),
                Gender = this.gender,
                ResetPasswordGuid = null
            };


            var signUpSucceeded = AreDetailsValid(user);

            if (signUpSucceeded)
            {

                var result = Session.AuthenticationService.CreateUser(user);
                if (Session.AuthenticationService.Authenticate(user.UserName, user.Password))
                {
                    UpdateUserInLocal();


                    DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                    var userCorrdinateResult = driverLocatorService.GetSelectedUserCoordinate(this.userName);

                    if (userCorrdinateResult.IsSuccess)
                    {
                        App.CurrentLoggedUser = userCorrdinateResult.UserLocation;
                    }

                    if (App.CurrentUserVehicles != null)
                        UpdateVehiclesInLocal();

                    if (App.CurrentUserFavouritePlaces != null)
                        UpdateFavPlacesInLocal();

                    this.signUpPageProcessor.MoveToLoginPage();
                }
                else
                    this.signUpPageProcessor.MoveToLoginPage();
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
                Password = this.password == "********" ? null : this.Password,
                profileImageEncoded = GetProfilePictureEncoded(),
                Gender = this.gender,
                ResetPasswordGuid = null
            };

            var Isvalid = AreDetailsValid(user, true);

            if (Isvalid)
            {
                var result = Session.AuthenticationService.UpdateUser(user);
                UpdateUserInLocal();

                DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                var userCorrdinateResult = driverLocatorService.GetSelectedUserCoordinate(this.userName);

                if (userCorrdinateResult.IsSuccess)
                {
                    App.CurrentLoggedUser = userCorrdinateResult.UserLocation;
                }

                if (App.CurrentUserVehicles != null)
                    UpdateVehiclesInLocal();

                if (App.CurrentUserFavouritePlaces != null)
                    UpdateFavPlacesInLocal();

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
            bool returnValue = true;
            if (!isUpdate)
            {
                DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                var response = driverLocatorService.GetSelectedUserCoordinate(user.UserName);
                if (!response.IsSuccess && response.UserLocation == null)
                {
                    this.ErrorMessage = "Username already exists";
                    returnValue = false;
                }
                else
                {
                    this.ErrorMessage = String.Empty;
                    returnValue = true;
                }

                if (string.IsNullOrWhiteSpace(user.UserName) ||
                string.IsNullOrWhiteSpace(user.Password) ||
                string.IsNullOrWhiteSpace(user.UserName) ||
                string.IsNullOrWhiteSpace(user.EMail))
                {
                    this.ErrorMessage = "Register failed. One or more fields is/are empty";
                    returnValue = false;
                }

                return returnValue;
            }

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
            dlUser.Gender = result.Gender;
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

        public void UpdateFavPlacesInLocal()
        {
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);

            if (App.CurrentUserFavouritePlaces != null)
            {
                foreach (DriverLocator.Models.FavouritePlace fp in App.CurrentUserFavouritePlaces)
                {
                    DriverLocator.Models.UpdateFavouritePlacesRequest dlFavPlacesRequest = new DriverLocator.Models.UpdateFavouritePlacesRequest();
                    dlFavPlacesRequest.UserName = App.CurrentLoggedUser.User.UserName;
                    dlFavPlacesRequest.Latitude = fp.Latitude;
                    dlFavPlacesRequest.Longitude = fp.Longitude;
                    dlFavPlacesRequest.PlaceName = fp.PlaceName;
                    dlFavPlacesRequest.PreviousUserGivenPlaceName = fp.PreviousUserGivenPlaceName;
                    dlFavPlacesRequest.UserGivenplaceName = fp.UserGivenplaceName;
                    dlFavPlacesRequest.PlaceID = fp.PlaceID;
                    dlFavPlacesRequest.PlaceReference = fp.PlaceReference;
                    var response = driverLocatorService.UpdateUserFavouritePlaces(dlFavPlacesRequest);
                }
            }
        }


        private void OnTapped()
        {
            this.signUpPageProcessor.MoveToPage("RegisterVehicleDetailsPage");
        }

        private void OnTappedLogin()
        {
            this.signUpPageProcessor.MoveToLoginPage();
        }

        private void OnTappedFavs()
        {
            this.signUpPageProcessor.MoveToPage("RegisterFavouritePlacesPage");
        }

        private void CheckFormValiditiy()
        {
            string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";            

            IsButtonEnabled = !String.IsNullOrEmpty(this.email) &&
                !String.IsNullOrEmpty(this.firstName) && !String.IsNullOrEmpty(this.lastName) && !String.IsNullOrEmpty(this.userName) && !String.IsNullOrEmpty(this.password)
                && (Regex.IsMatch(this.email, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250.00))) && 
                    (Regex.IsMatch(this.password, passwordRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250.00)));



        }
    }
}
