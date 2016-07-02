using Authentication;
using Authentication.Models;
using DriverLocatorFormsPortable.Common;
using RideShare.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RideShare.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        string userName;
        string password;
        string errorMessage;

        ILoginPageProcessor loginProcessor;

        public ICommand LoginCommand { protected set; get; }

        public ICommand SignUpCommand { protected set; get; }

        public LoginViewModel(ILoginPageProcessor loginProcessor)
        {
            this.loginProcessor = loginProcessor;
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
                var userCorrdinateResult = driverLocatorService.GetSelectedUserCoordinate();

                if (userCorrdinateResult.IsSuccess)
                {
                    App.CurrentLoggedUser = userCorrdinateResult.UserCoordinate;
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
            loginProcessor.MoveToSignUpPage();
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
