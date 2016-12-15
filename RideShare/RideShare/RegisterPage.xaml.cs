using com.google.i18n.phonenumbers;
using ImageCircle.Forms.Plugin.Abstractions;
using MediaPicker.Forms.Plugin.Abstractions;
using RideShare.Common;
using RideShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare
{
    public partial class RegisterPage : ContentPage, ISignUpPageProcessor, IUserVehicleAdditionResult, IUserFavouritePlaceAdditionResult, IAgreementResult, ICountryCodeSelectionResult
    {
        //bool isNewItem;

        IMediaPicker mediaPicker;
        ImageSource imageSource;
        CircleImage profilePhoto;
        byte[] profileImage;
        String status;
        public Action<ObservableCollection<DriverLocator.Models.Vehicle>> OnUserVehicleAdded { get; set; }
        public Action<ObservableCollection<DriverLocator.Models.FavouritePlace>> OnUserFavouritePlaceAdded { get; set; }
        public Action<string> OnCountryCodeSelection { get; set; }
        ObservableCollection<DriverLocator.Models.Vehicle> sortedVehicleList;
        ObservableCollection<DriverLocator.Models.FavouritePlace> sortedFavPlaceList;

        public RegisterPage()
        {
            InitializeComponent();
            Content.BindingContext = new SignUpViewModel(this);
            var vm = Content.BindingContext as SignUpViewModel;
            genderPicker.SelectedIndexChanged += genderPicker_SelectedIndexChanged;
            mobileNumberEntry.Unfocused += mobileNumberEntry_Unfocused;
            mobileNumberEntry.TextChanged += mobileNumberEntry_TextChanged;
            countryCodeEntry.Unfocused += countryCodeEntry_Unfocused;
            countryCodeEntry.TextChanged += countryCodeEntry_TextChanged;
            profilePhoto = new CircleImage()
            {
                //BorderColor = Color.Yellow,
                //FillColor = Color.Yellow,
                BorderThickness = 9,
                HeightRequest = 150,
                WidthRequest = 150,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center
                //Source = "add_picture.png"
            };

            var addPictureButton = new Button()
            {
                Text = "Select Picture",
                Command = new Command(async () => { await SelectPicture(); })
            };

            genderPicker.Items.Add("Gender");
            genderPicker.Items.Add("Male");
            genderPicker.Items.Add("Female");

            if (vm.isAuthenticated)
            {
                if(vm.ProfilePhoto != null && vm.ProfilePhoto.Length != 0)
                {
                    profileImage = vm.ProfilePhoto;
                    imageSource = GetImageSourceFromByteArray(profileImage);
                    profilePhoto.Source = imageSource;
                }
                else
                {
                    profilePhoto.Source = "add_picture.png";
                }

                if (vm.Vehicles != null)
                {
                    foreach (DriverLocator.Models.Vehicle vehicle in vm.Vehicles)
                    {
                        vehicle.VehicleDisplayName = String.Format("{0} {1}", vehicle.VehicleModel, vehicle.VehicleNumberPlate);
                        vehicle.PreviousVehicleNumberPlate = vehicle.VehicleNumberPlate;
                    }

                    if (vm.Vehicles.Count != 0)
                        sortedVehicleList = new ObservableCollection<DriverLocator.Models.Vehicle>(from i in vm.Vehicles orderby i.VehicleNumberPlate select i);
                    else
                        sortedVehicleList = vm.Vehicles;                    
                }

                if (vm.FavPlaces != null)
                {
                    foreach (DriverLocator.Models.FavouritePlace favPlace in vm.FavPlaces)
                    {
                        favPlace.PreviousUserGivenPlaceName = favPlace.UserGivenplaceName;
                    }

                    if (vm.FavPlaces.Count != 0)
                        sortedFavPlaceList = new ObservableCollection<DriverLocator.Models.FavouritePlace>(from i in vm.FavPlaces orderby i.UserGivenplaceName select i);
                    else
                        sortedFavPlaceList = vm.FavPlaces;                    
                }

                if (vm.Gender == "Female")
                    genderPicker.SelectedIndex = 2;
                else if (vm.Gender == "Male")
                    genderPicker.SelectedIndex = 1;
                else
                    genderPicker.SelectedIndex = 0;                                    
            }
            else
            {
                if (profileImage != null && profileImage.Length > 0)
                    vm.ProfilePhoto = profileImage;

                if (profilePhoto.Source == null)
                    profilePhoto.Source = "add_picture.png";                
            }

            profileImageStackLayout.Children.Add(profilePhoto);
            profileImageStackLayout.Children.Add(addPictureButton);
            OnUserVehicleAdded = OnUserVehicleAddedResult;
            OnUserFavouritePlaceAdded = OnUserFavouritePlaceAddedResult;
            OnCountryCodeSelection = OnCountryCodeSelectionResult;
            RefreshFavouritePlaces();
            RefreshUserVehicles();          
            
        }

        void countryCodeEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = Content.BindingContext as SignUpViewModel;
            vm.IsMobileNumberErrorMessageEnabled = MobileNumberValidator();
        }

        void countryCodeEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var vm = Content.BindingContext as SignUpViewModel;
            vm.IsMobileNumberErrorMessageEnabled = MobileNumberValidator();
        }

        void mobileNumberEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = Content.BindingContext as SignUpViewModel;
            if (vm.IsMobileNumberErrorMessageEnabled)
                vm.IsMobileNumberErrorMessageEnabled = MobileNumberValidator();
        }
        

        void mobileNumberEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var vm = Content.BindingContext as SignUpViewModel;
            vm.IsMobileNumberErrorMessageEnabled = MobileNumberValidator();
        }        

        public bool MobileNumberValidator()
        {
            char[] separators = { '+' };

            if (String.IsNullOrEmpty(countryCodeEntry.Text))
                return true;

            if (!String.IsNullOrEmpty(countryCodeEntry.Text))
            {
                if (!String.IsNullOrEmpty(mobileNumberEntry.Text))
                {
                    try
                    {
                        RideShare.SharedInterfaces.IFileReader fileReader = DependencyService.Get<RideShare.SharedInterfaces.IFileReader>();
                        Dictionary<string, string> countryCodes = fileReader.GetCountryCodesWithNames();

                        string thisCountryCode = countryCodes.FirstOrDefault(i => i.Key == countryCodeEntry.Text).Value; ;
                        PhoneNumberUtil phoneUtil = PhoneNumberUtil.getInstance();
                        int countryCodeWithoutPlus = Convert.ToInt16(thisCountryCode.Split(separators)[1].Replace(" ", String.Empty));
                        com.google.i18n.phonenumbers.Phonenumber.PhoneNumber mobileNumber =
                            new com.google.i18n.phonenumbers.Phonenumber.PhoneNumber().setCountryCode(countryCodeWithoutPlus).setNationalNumber(Convert.ToInt64(mobileNumberEntry.Text));
                        return !phoneUtil.isValidNumber(mobileNumber);
                    }
                    catch (Exception)
                    {
                        return true;

                    }

                }
                else
                    return false;
            }
            else
                return true;
        }

        void genderPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var vm = Content.BindingContext as SignUpViewModel;
            vm.Gender = genderPicker.Items[genderPicker.SelectedIndex];            
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
                profileImage = GetByteArray(mediaFile.Source);

                var vm = Content.BindingContext as SignUpViewModel;
                vm.ProfilePhoto = profileImage;
            }
            catch (System.Exception ex)
            {
                this.status = ex.Message;
            }
        }

        public byte[] GetByteArray(Stream input)
        {
            byte[] imageArray;
            using (MemoryStream mStream = new MemoryStream())
            {
                input.CopyTo(mStream);
                imageArray = mStream.ToArray();
            }

            return imageArray;
        }

        public ImageSource GetImageSourceFromByteArray(byte[] input)
        {
            return ImageSource.FromStream(() => new MemoryStream(input));
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


        public void MoveToPage(string page)
        {
            NavigationPage nextPage = new NavigationPage();
            if(page == "RegisterVehicleDetailsPage")
                nextPage = new NavigationPage(new RegisterVehicleDetailsPage(this, new DriverLocator.Models.Vehicle()));
            else if (page == "RegisterFavouritePlacesPage")
                nextPage = new NavigationPage(new RegisterFavouritePlacesPage(this, new DriverLocator.Models.FavouritePlace()));
            else if (page == "CountryCodesPage")
                nextPage = new NavigationPage(new CountryCodesPage(this, this.countryCodeEntry.Text));

            Navigation.PushModalAsync(nextPage);            
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            NavigationPage nextPage = new NavigationPage(new RegisterVehicleDetailsPage(this, e.SelectedItem as DriverLocator.Models.Vehicle));
            Navigation.PushModalAsync(nextPage); 
        }

        private void OnfavItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            NavigationPage nextPage = new NavigationPage(new RegisterFavouritePlacesPage(this, e.SelectedItem as DriverLocator.Models.FavouritePlace));
            Navigation.PushModalAsync(nextPage);
        }


        public void MoveToPreviousPage()
        {
            Navigation.PopModalAsync();
        }

        void OnUserVehicleAddedResult(ObservableCollection<DriverLocator.Models.Vehicle> results)
        {
            if (results != null && results.Count != 0)
                sortedVehicleList = new ObservableCollection<DriverLocator.Models.Vehicle>(from i in results orderby i.VehicleNumberPlate select i);
            else
                sortedVehicleList = results;
            RefreshUserVehicles();


        }

        void OnUserFavouritePlaceAddedResult(ObservableCollection<DriverLocator.Models.FavouritePlace> results)
        {
            if (results != null && results.Count != 0)
                sortedFavPlaceList = new ObservableCollection<DriverLocator.Models.FavouritePlace>(from i in results orderby i.UserGivenplaceName select i);
            else
                sortedFavPlaceList = results;
            RefreshFavouritePlaces();
            
        } 
        
        private void RefreshUserVehicles()
        {
            if (sortedVehicleList != null)
            {
                vehicleListView.ItemsSource = sortedVehicleList;
                vehicleListView.HeightRequest = sortedVehicleList.Count * vehicleListView.RowHeight;
            }
            else
                vehicleListView.HeightRequest = 0;
        }
        private void RefreshFavouritePlaces()
        {
            if (sortedFavPlaceList != null)
            {
                favPlacesListView.ItemsSource = sortedFavPlaceList;
                favPlacesListView.HeightRequest = sortedFavPlaceList.Count * favPlacesListView.RowHeight;
            }
            else
                favPlacesListView.HeightRequest = 0;
        }

        public void ShowDoubleButtonPopup(string title, string message, Action successAction, Action failedAction)
        {
            DisplayAlert(title, message, "I Agree", "I do not Agree").ContinueWith((task) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (task.Result)
                    {
                        successAction();
                    }
                    else
                    {
                        failedAction();
                    }
                });


            });            
        }

        void OnCountryCodeSelectionResult(string selectedName)
        {
            var vm = Content.BindingContext as SignUpViewModel;
            vm.CountryName = selectedName;
        }
    }
}
