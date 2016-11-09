using GoogleApiClient.Maps;
using GoogleApiClient.Models;
using RideShare.Common;
using RideShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare
{
    public interface IUserFavouritePlaceAdditionResult
    {
        Action<ObservableCollection<DriverLocator.Models.FavouritePlace>> OnUserFavouritePlaceAdded { get; set; }
    }

    public partial class RegisterFavouritePlacesPage : ContentPage, ISignUpPageProcessor
    {
        LocationSearchResult selectedLocation;
        IUserFavouritePlaceAdditionResult userFavPlaceAdditionResult;
        bool isFirstLoad;

        public RegisterFavouritePlacesPage(IUserFavouritePlaceAdditionResult userfavPlaceAdditionResult, DriverLocator.Models.FavouritePlace selectedfavPlace)
        {
            isFirstLoad = true;
            this.userFavPlaceAdditionResult = userfavPlaceAdditionResult;
            InitializeComponent();
            Content.BindingContext = new RegisterFavouritePlacesViewModel(this, selectedfavPlace);
            var vm = Content.BindingContext as RegisterFavouritePlacesViewModel;
            registerButton.Clicked += RegisterButtonClicked;
            
        }

        private void BindListData(string query)
        {
            GooglePlacesClient googlePlacesClient = new GooglePlacesClient();
            var data = googlePlacesClient.GetPlaces(query);
            favPlacesListView.ItemsSource = MapSearchData(data.Predictions);
        }

        private void OnValueChanged(object sender, TextChangedEventArgs e)
        {
            if (!isFirstLoad)
                BindListData(favPlaceSearchEntry.Text);
            else
                isFirstLoad = false;
        }

        private IList<LocationSearchResult> MapSearchData(IList<Prediction> predictions)
        {
            List<LocationSearchResult> locs = new List<RideShare.LocationSearchResult>();
            if (predictions != null)
            {
                foreach (var prediction in predictions)
                {
                    locs.Add(new LocationSearchResult() { Latitude = 1, Longitude = 11, LocationRefernce = prediction.Refernce, LocationId = prediction.Placeid, LocationName = prediction.Description });
                }
            }
            return locs;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedLocation = e.SelectedItem as LocationSearchResult;
            favPlaceSearchEntry.Text = selectedLocation.LocationName;
            favPlacesListView.ItemsSource = null;
            GooglePlacesClient googlePlacesClient = new GooglePlacesClient();
            var locationDetails = googlePlacesClient.GetCoordinates(selectedLocation.LocationRefernce);
            
            var vm = Content.BindingContext as RegisterFavouritePlacesViewModel;
            //vm.UserGivenplaceName = favPlaceNameEntry.Text;
            //vm.PlaceName = selectedLocation.LocationName;
            vm.Longitude = locationDetails.CoordinateResult.Geometry.Location.Longitude.ToString();
            vm.Latitude = locationDetails.CoordinateResult.Geometry.Location.Latitude.ToString();
            vm.PlaceID = selectedLocation.LocationId;
            vm.PlaceReference = selectedLocation.LocationRefernce;
        }

        private void RegisterButtonClicked(object sender, EventArgs e)
        {
            ObservableCollection<DriverLocator.Models.FavouritePlace> newfavPlaces = new ObservableCollection<DriverLocator.Models.FavouritePlace>();
            if (App.CurrentLoggedUser != null && App.CurrentLoggedUser.FavouritePlaces != null)
            {
                newfavPlaces = App.CurrentLoggedUser.FavouritePlaces;
                foreach (DriverLocator.Models.FavouritePlace fp in App.CurrentUserFavouritePlaces)
                {
                    if (!App.CurrentLoggedUser.FavouritePlaces.Contains(fp))
                        newfavPlaces.Add(fp);
                }
            }
            else
            {
                foreach (DriverLocator.Models.FavouritePlace fp in App.CurrentUserFavouritePlaces)
                {
                    newfavPlaces.Add(fp);
                }
            }
            this.userFavPlaceAdditionResult.OnUserFavouritePlaceAdded(newfavPlaces);
        }

        public void MoveToLoginPage()
        {
            App.Current.MainPage = new NavigationPage(new LogInPage());
        }

        public void MoveToMainPage()
        {
            App.Current.MainPage = new MainPage();
        }

        public void MoveToPage(string page)
        {
            App.Current.MainPage = new NavigationPage(new LogInPage());
        }

        public void MoveToPreviousPage()
        {
            Navigation.PopModalAsync();
        }
    }
}
