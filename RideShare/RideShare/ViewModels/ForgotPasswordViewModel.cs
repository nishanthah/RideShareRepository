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
    public class ForgotPasswordViewModel : ViewModelBase
    {
        string userName;
        IForgotPasswordPageProcessor fpwdPageProcessor;
        public ICommand NextCommand { protected set; get; }

        public ForgotPasswordViewModel(IForgotPasswordPageProcessor fpwdProcessor)
        {
            this.fpwdPageProcessor = fpwdProcessor;
            this.NextCommand = new RelayCommand(VerifyEmailAddressExists);
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

        private void VerifyEmailAddressExists()
        {
            ResponseBase userInfo = null;

            if (App.DeviceType == App.DeviceTypes.Android)
            {
                if (App.DeviceVersion >= 23)
                    userInfo = Session.AuthenticationService.GetUserInfoByUserNameAndSendEmail(this.userName);
                else
                    userInfo = Session.AuthenticationService.SendEmailWithCode(this.userName, "FPWD");
                
                bool userExists = userInfo.IsSuccess && (userInfo.Message == "User exists");
            }

            fpwdPageProcessor.MoveToLoginPage();

        }
    }
}
