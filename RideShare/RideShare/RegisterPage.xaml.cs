using ImageCircle.Forms.Plugin.Abstractions;
using MediaPicker.Forms.Plugin.Abstractions;
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
    public partial class RegisterPage : ContentPage,ISignUpPageProcessor
    {
        //bool isNewItem;

        IMediaPicker mediaPicker;
        ImageSource imageSource;
        CircleImage profilePhoto;
        String status;

        public RegisterPage()
        {
            InitializeComponent();
            Content.BindingContext = new SignUpViewModel(this);
            //isNewItem = isNew;

            profilePhoto = new CircleImage()
            {
                //BorderColor = Color.Yellow,
                //FillColor = Color.Yellow,
                BorderThickness = 9,
                HeightRequest = 150,
                WidthRequest = 150,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                Source = "add_picture.png"
            };

            var addPictureButton = new Button()
            {
                Text = "Select Picture",
                Command = new Command(async () => { await SelectPicture(); })
            };

            profileImageStackLayout.Children.Add(profilePhoto);
            profileImageStackLayout.Children.Add(addPictureButton);
        }

        private async Task SelectPicture()
        {

            mediaPicker = DependencyService.Get<IMediaPicker>();

            imageSource = null;

            try
            {
                var mediaFile = await mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
                {
                    DefaultCamera = CameraDevice.Front,
                    MaxPixelDimension = 400
                });
                imageSource = ImageSource.FromStream(() => mediaFile.Source);
                
                profilePhoto.Source = imageSource;
            }
            catch (System.Exception ex)
            {
                this.status = ex.Message;
            }
        }

        public void MoveToLoginPage()
        {
            //App.Current.MainPage = new Login();
            App.Current.MainPage = new NavigationPage(new LogInPage());
            
        }

        public void MoveToMainPage()
        {
            App.Current.MainPage = new MainPage();
        }

        //void OnMainPage(object sender, EventArgs e)
        //{

        //   //Navigation.PushAsync(new MainPage());
        //    Application.Current.MainPage = new MainPage();

        //    //NavigationPage.
        //    //api.rideshare.com
        //}

        //async void OnSaveActivated(object sender, EventArgs e)
        //{
        //    var user = (User)BindingContext;
        //    await App.User_Manager.SaveTaskAsync(user, isNewItem);
        //    await Navigation.PopAsync();
        //}
    }
}
