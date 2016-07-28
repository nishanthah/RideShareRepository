using Authentication;
using Authentication.Models;
using Common.Models;
using DriverLocatorFormsPortable.Common;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RideShare.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        string userName;
        string password;
        string errorMessage;

        ILoginPageProcessor loginProcessor;
        IUrbanAirshipNotificationService urbanAirshipNotificationService;

        public ICommand LoginCommand { protected set; get; }

        public ICommand SignUpCommand { protected set; get; }

        public LoginViewModel(ILoginPageProcessor loginProcessor, IUrbanAirshipNotificationService urbanAirshipNotificationService)
        {
            this.loginProcessor = loginProcessor;
            this.urbanAirshipNotificationService = urbanAirshipNotificationService;
            Session.AuthenticationService = new AuthenticationService();
            this.LoginCommand = new RelayCommand(Login);
            this.SignUpCommand = new RelayCommand(SignUp);
        }

        bool AreCredentialsCorrect(User user)
        {
            if (Session.AuthenticationService.Authenticate(user.UserName, user.Password))
            {
                return true;
            }
            return false;
        }

        private void Login()
        {
            var user = new User
            {
                UserName = this.UserName,
                Password = this.Password
            };

            var isValid = AreCredentialsCorrect(user);

            if (isValid)
            {

                DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                var userCorrdinateResult = driverLocatorService.GetSelectedUserCoordinate(this.userName);
                Session.CurrentUserName = this.UserName;
                urbanAirshipNotificationService.InitializeNamedUser(this.UserName);

                if (userCorrdinateResult.IsSuccess)
                {
                    App.CurrentLoggedUser = userCorrdinateResult.UserLocation;
                    driverLocatorService.UpdateUserType(App.CurrentLoggedUser.User.UserName,new DriverLocator.Models.UpdateUserTypeRequest() { UserType = Session.CurrentUserType });
                    IAppDataService appDataService = DependencyService.Get<IAppDataService>();
                    appDataService.Save("current_user", App.CurrentLoggedUser.User.UserName);
                    loginProcessor.MoveToMainPage();
                }
                else
                {
                    loginProcessor.MoveToCreateUserPage();
                }
            }
            else
            {
                this.ErrorMessage = "Login failed";
                this.Password = string.Empty;
            }
        }

        private void SignUp()
        {            
            loginProcessor.MoveToCreateUserPage();
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
    }
}
