using DriverLocatorFormsPortable.Common;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RideShare.ViewModels
{
    public interface IAccountDeleteResult
    {
        void ShowDoubleButtonPopup(string title, string message, Action successAction, Action failedAction);
    }

    public class SettingsViewModel : ViewModelBase
    {
        ICommonPageProcessor pageProcessor;        
        public ICommand TapDeleteMyAccountCommand { protected set; get; }
        IAccountDeleteResult accountDeletedResult;

        public SettingsViewModel(ICommonPageProcessor commonPageProcessor, IAccountDeleteResult accountDeleteResult)
        {
            this.pageProcessor = commonPageProcessor;
            this.TapDeleteMyAccountCommand = new RelayCommand(DeletAccount);
            this.accountDeletedResult = accountDeleteResult;            
        }

        private void DeletAccount()
        {           
            accountDeletedResult.ShowDoubleButtonPopup("Delete Account", "Are you sure you want to delete this account?", OnPopupSuccess, OnPopupCanceled);
        }

        private void OnPopupSuccess()
        {
            DriverLocator.DriverLocatorService driverLocatorService = new DriverLocator.DriverLocatorService(Session.AuthenticationService);
            driverLocatorService.DeleteUser(App.CurrentLoggedUser.User.UserName);
            IAppDataService appDataService = DependencyService.Get<IAppDataService>();
            appDataService.Save("access_token", null);
            appDataService.Save("current_user", null);
            Session.AuthenticationService.DeleteUser(new Authentication.Models.User()
            {
                UserName = App.CurrentLoggedUser.User.UserName
            });
            Session.ClearAuthenticationInstance();
            Session.CurrentUserName = null;

            //var loginPage = new LogInPage();
            //loginPage.Title = "Login Page";

            //App.Current.MainPage = loginPage;
            //this.pageProcessor.MoveToLoginPage();

            
                //Session.CurrentUserName = this.UserName;
                //App.CurrentLoggedUser.User.UserType = Session.CurrentUserType;

                this.pageProcessor.MoveToLoginPage();
           
        }

        private void OnPopupCanceled()
        {
            //this.pageProcessor.MoveToMainPage();
        }        
    }
}
