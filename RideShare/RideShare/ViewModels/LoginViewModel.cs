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
        bool isUserLoggedIn = false;
        ILoginPageProcessor loginProcessor;
        IUrbanAirshipNotificationService urbanAirshipNotificationService;
        IAppDataService appDataService = DependencyService.Get<IAppDataService>();

        public ICommand LoginCommand { protected set; get; }

        public ICommand SignUpCommand { protected set; get; }

        public ICommand TapForgotPwdOneCommand { protected set; get; }

        public LoginViewModel(ILoginPageProcessor loginProcessor, IUrbanAirshipNotificationService urbanAirshipNotificationService)
        {
            this.loginProcessor = loginProcessor;
            this.urbanAirshipNotificationService = urbanAirshipNotificationService;
            this.LoginCommand = new RelayCommand(Login);
            this.SignUpCommand = new RelayCommand(SignUp);
            this.TapForgotPwdOneCommand = new RelayCommand(OnForgotPwdNext);
            //InitRecentAuthntication();
        }


        //void InitRecentAuthntication()
        //{
        //    var accessToken = appDataService.Get("access_token");
        //    if (accessToken != null)
        //    {
        //       var userInfo= Session.AuthenticationService.GetUserInfo(accessToken);
        //       if(userInfo.IsSuccess)
        //       {
        //            UserName = userInfo.UserName;
        //            Password = "********";
        //            isUserLoggedIn = true;
        //       }
        //       else
        //       {
        //            UserName = String.Empty;
        //            Password = String.Empty;
        //            isUserLoggedIn = false;
        //        }
        //    }
        //}

        bool AreCredentialsCorrect(User user)
        {
            if (Session.AuthenticationService.Authenticate(user.UserName, user.Password))
            {
                appDataService.Save("access_token", Session.AuthenticationService.AuthenticationToken);
                return true;
            }

            return false;
        }

        private void Login()
        {
            IsBusy = true;

            Task.Factory.StartNew(() =>
            {

                var user = new User
                {
                    UserName = this.UserName,
                    Password = this.Password
                };

                var isValid = AreCredentialsCorrect(user);

                if (isValid)
                {
                    UpdateUserInLocal();
                    DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
                    var userCorrdinateResult = driverLocatorService.GetSelectedUserCoordinate(this.userName);


#if WithNotification
                        
#else
                    urbanAirshipNotificationService.InitializeNamedUser(this.UserName);
#endif

                    if (userCorrdinateResult.IsSuccess)
                    {
                        
                            App.CurrentLoggedUser = userCorrdinateResult.UserLocation;
                            driverLocatorService.UpdateUserType(App.CurrentLoggedUser.User.UserName, new DriverLocator.Models.UpdateUserTypeRequest() { UserType = Session.CurrentUserType });
                        driverLocatorService.UpdateUserLoginStatus(App.CurrentLoggedUser.User.UserName, true);
                        appDataService.Save("current_user", App.CurrentLoggedUser.User.UserName);

                        loginProcessor.InvokeInMainThread(() =>
                        {
                            Session.CurrentUserName = this.UserName;
                            App.CurrentLoggedUser.User.UserType = Session.CurrentUserType;
                            
                            loginProcessor.MoveToMainPage();                            
                        });

                    }
                }
                else
                {
                    loginProcessor.InvokeInMainThread(() =>
                    {
                        this.ErrorMessage = "Login failed";
                        this.Password = string.Empty;
                        IsBusy = false;
                    });

                }


            });
            
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

        public void UpdateUserInLocal()
        {
            var result = Session.AuthenticationService.GetUserInfo(Session.AuthenticationService.AuthenticationToken);
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
            DriverLocator.Models.User dlUser = new DriverLocator.Models.User();
            dlUser.UserName = this.UserName;
            dlUser.FirstName = result.FirstName;
            dlUser.LastName = result.LastName;
            dlUser.EMail = result.EMail;
            dlUser.profileImageEncoded = result.profileImageEncoded;
            dlUser.Gender = result.Gender;
            var response = driverLocatorService.SaveUserData(dlUser);
        }

        private void OnForgotPwdNext()
        {
            this.loginProcessor.MoveToForgotPasswordPage();
        }
    }
}
