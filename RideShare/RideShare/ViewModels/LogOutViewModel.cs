using DriverLocatorFormsPortable.Common;
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
            Session.AuthenticationService = null;
            Session.CurrentUserName = null;

            var loginPage = new LogInPage();
            loginPage.Title = "Login Page";
            Application.Current.MainPage = loginPage;
        }

    }
}
