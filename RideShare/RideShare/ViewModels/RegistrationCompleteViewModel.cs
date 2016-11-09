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
    public class RegistrationCompleteViewModel : ViewModelBase
    {
        string registrationCode;
        ILoginPageProcessor nextPageProcessor;
        public ICommand NextCommand { protected set; get; }

        public RegistrationCompleteViewModel(ILoginPageProcessor loginProcessor)
        {
            this.nextPageProcessor = loginProcessor;
            this.NextCommand = new RelayCommand(VerifyRegistrationCode);
        }

        public string RegistrationCode
        {
            get
            {
                return registrationCode;
            }

            set
            {
                registrationCode = value;
                OnPropertyChanged("RegistrationCode");
            }
        }

        private void VerifyRegistrationCode()
        {
            if (this.registrationCode == App.CurrentLoggedUser.User.RegistrationCode)
            {
                Session.AuthenticationService.UpdateRegistrationCode(App.CurrentLoggedUser.User.UserName);
                nextPageProcessor.MoveToMainPage();
            }
        }

    }
}
