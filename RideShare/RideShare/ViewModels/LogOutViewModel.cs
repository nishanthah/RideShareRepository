using DriverLocatorFormsPortable.Common;
using RideShare.SharedInterfaces;
using System.Windows.Input;
using Xamarin.Forms;

namespace RideShare.ViewModels
{
    public class LogOutViewModel : ViewModelBase
    {
        public ICommand SignOutCommand { protected set; get; }

        public LogOutViewModel()
        {
            this.SignOutCommand = new RelayCommand(signOut);
        }


        private void signOut()
        {
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
            driverLocatorService.UpdateUserLoginStatus(App.CurrentLoggedUser.User.UserName, false);
            IAppDataService appDataService = DependencyService.Get<IAppDataService>();
            appDataService.Save("access_token", null);
            Session.ClearAuthenticationInstance();
            Session.CurrentUserName = null;

            var loginPage = new LogInPage();
            loginPage.Title = "Login Page";
            
            Application.Current.MainPage = loginPage;
        }

    }
}
