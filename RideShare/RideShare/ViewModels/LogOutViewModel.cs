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
            IAppDataService appDataService = DependencyService.Get<IAppDataService>();
            appDataService.Save("access_token", null);
            Session.AuthenticationService = null;
            Session.CurrentUserName = null;

            var loginPage = new LogInPage();
            loginPage.Title = "Login Page";
            
            Application.Current.MainPage = loginPage;
        }

    }
}
