using Common.Models;
using RideShare.Common;
using RideShare.SharedInterfaces;
using RideShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace RideShare
{
    public partial class LogInPage : ContentPage,ILoginPageProcessor
    {
        IUrbanAirshipNotificationService urbanAirshipNotificationService = DependencyService.Get<IUrbanAirshipNotificationService>();


        public LogInPage()
        {
            InitializeComponent();
            InitUserType();
            Title = "Login Page";
            Content.BindingContext = new LoginViewModel(this, urbanAirshipNotificationService);
            loginAsRiderButton.Clicked += loginAsRiderButton_Clicked;
            loginAsDriverButton.Clicked += loginAsDriverButton_Clicked;
            //driverIcon.GestureRecognizers.Add(new TapGestureRecognizer(OnTapDriver));
            //riderIcon.GestureRecognizers.Add(new TapGestureRecognizer(OnTapRider));
        }

        void loginAsDriverButton_Clicked(object sender, EventArgs e)
        {
            ChangeUserType(UserType.Driver);
        }

        void loginAsRiderButton_Clicked(object sender, EventArgs e)
        {
            ChangeUserType(UserType.Rider);
        }

        void OnMainPage(object sender, EventArgs e)
        {

            Application.Current.MainPage = new MainPage();

        }
        
        public void InitUserType()
        {
            ChangeUserType(UserType.Rider);           
        }

        private void ChangeUserType(UserType userType)
        {
            if(userType== UserType.Driver)
            {
                //driverIcon.Source = "driverLogActive_icon.png";
                //riderIcon.Source = "userLog_icon.png";
                Session.CurrentUserType = UserType.Driver;
            }
            else
            {
                //driverIcon.Source = "driverLog_icon.png";
                //riderIcon.Source = "userLogActive_icon.png";
                Session.CurrentUserType = UserType.Rider;
            }
        }

        void OnTapDriver(View sender, object e)
        {

            ChangeUserType(UserType.Driver);


        }

        void OnTapRider(View sender, object e)
        {

            ChangeUserType(UserType.Rider);

        }

        void GoToRegister(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new RegisterPage());
            App.Current.MainPage = new RegisterPage();
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
           // App.Current.MainPage = new SignUp();
        }

        public void InvokeInMainThread(Action action)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                action();
            });
                
        }

        public void MoveToForgotPasswordPage()
        {
            App.Current.MainPage = new NavigationPage(new ForgotPasswordStepOne());
        }


        public void MoveToRegistrationCompletePage()
        {
            App.Current.MainPage = new MainPage(new RegistrationComplete());
        }
    }
}
