using RideShare.Common;
using RideShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare
{
    public partial class RegistrationComplete : ContentPage, ILoginPageProcessor
    {
        public RegistrationComplete()
        {
            InitializeComponent();
            Content.BindingContext = new RegistrationCompleteViewModel(this);
        }

        public void MoveToMainPage()
        {
            App.Current.MainPage = new MainPage();
        }

        public void MoveToCreateUserPage()
        {
            App.Current.MainPage = new RegisterPage();
        }

        public void MoveToSignUpPage()
        {
            App.Current.MainPage = new LogInPage();
        }

        public void InvokeInMainThread(Action action)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                action();
            });
        }

        public void MoveToRegistrationCompletePage()
        {
            App.Current.MainPage = new RegistrationComplete();
        }

        public void MoveToForgotPasswordPage()
        {
            App.Current.MainPage = new ForgotPasswordStepOne();
        }
    }
}
