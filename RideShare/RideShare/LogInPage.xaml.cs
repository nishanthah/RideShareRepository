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
            Title = "Login Page";
            Content.BindingContext = new LoginViewModel(this, urbanAirshipNotificationService);
            driverIcon.GestureRecognizers.Add(new TapGestureRecognizer(OnTapDriver));
            riderIcon.GestureRecognizers.Add(new TapGestureRecognizer(OnTapRider));
            //var layout = new StackLayout();
            //var button = new Button
            //{
            //    Text = "StackLayout",
            //    VerticalOptions = LayoutOptions.Start,
            //    HorizontalOptions = LayoutOptions.FillAndExpand
            //};
            //var yellowBox = new BoxView { Color = Color.Yellow, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            //var greenBox = new BoxView { Color = Color.Green, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            //var blueBox = new BoxView
            //{
            //    Color = Color.Blue,
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    HeightRequest = 75
            //};

            //layout.Children.Add(button);
            //layout.Children.Add(yellowBox);
            //layout.Children.Add(greenBox);
            //layout.Children.Add(blueBox);
            //layout.Spacing = 10;
            //Content = layout;

            //InitializeComponent();
            //Title = "Edit Profile Page";
            //Content = new StackLayout
            //{
            //    Children = {
            //        new Label {
            //            Text = "Edit Profile Page data goes here",
            //            HorizontalOptions = LayoutOptions.Center,
            //            VerticalOptions = LayoutOptions.CenterAndExpand
            //        }
            //    }
            //};
        }

        void OnMainPage(object sender, EventArgs e)
        {
            
           //Navigation.PushAsync(new MainPage());
            Application.Current.MainPage = new MainPage();
            
            //NavigationPage.
            //api.rideshare.com
        }

        void OnTapDriver(View sender, object e)
        {
            
                driverIcon.Source = "driverLogActive_icon.png";
                riderIcon.Source = "userLog_icon.png";
           
        }


        void OnTapRider(View sender, object e)
        {

            driverIcon.Source = "driverLog_icon.png";
            riderIcon.Source = "userLogActive_icon.png";

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

    }
}
