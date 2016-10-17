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
    public class RegisterFavouritePlacesViewModel : ViewModelBase
    {
        string userGivenplaceName;
        string placeName;
        string longitude;
        string latitude;
        string previousUserGivenPlaceName;
        string placeID;
        string placeReference;
        ObservableCollection<DriverLocator.Models.FavouritePlace> favouritePlaces;

        ISignUpPageProcessor signUpVehiclePageProcessor;
        public ICommand RegisterFavPlace { protected set; get; }

        public RegisterFavouritePlacesViewModel(ISignUpPageProcessor signUpVehiclePageProcessor, DriverLocator.Models.FavouritePlace selectedFavPlace)
        {
            this.signUpVehiclePageProcessor = signUpVehiclePageProcessor;

            if (selectedFavPlace != null)
            {
                if (selectedFavPlace.UserGivenplaceName == null)
                    previousUserGivenPlaceName = this.userGivenplaceName = String.Empty;
                else
                    previousUserGivenPlaceName = this.userGivenplaceName = selectedFavPlace.UserGivenplaceName;

                if (selectedFavPlace.Latitude == null)
                    this.latitude = String.Empty;
                else
                    this.latitude = selectedFavPlace.Latitude;

                if (selectedFavPlace.Longitude == null)
                    this.longitude = String.Empty;
                else
                    this.longitude = selectedFavPlace.Longitude;

                if (selectedFavPlace.PlaceName == null)
                    this.placeName = String.Empty;
                else
                    this.placeName = selectedFavPlace.PlaceName;

                if (selectedFavPlace.PlaceID == null)
                    this.placeID = String.Empty;
                else
                    this.placeID = selectedFavPlace.PlaceID;

                if (selectedFavPlace.PlaceReference == null)
                    this.placeReference = String.Empty;
                else
                    this.placeReference = selectedFavPlace.PlaceReference;
            }

            this.RegisterFavPlace = new RelayCommand(UpdateFavouritePlaces);
        }

        public string UserGivenplaceName
        {
            get
            {
                return userGivenplaceName;
            }

            set
            {
                userGivenplaceName = value;
                OnPropertyChanged("UserGivenplaceName");
            }
        }

        public string PlaceName
        {
            get
            {
                return placeName;
            }

            set
            {
                placeName = value;
                OnPropertyChanged("PlaceName");
            }
        }

        public string Longitude
        {
            get
            {
                return longitude;
            }

            set
            {
                longitude = value;
                OnPropertyChanged("Longitude");
            }
        }

        public string Latitude
        {
            get
            {
                return latitude;
            }

            set
            {
                latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        public string PlaceID
        {
            get
            {
                return placeID;
            }

            set
            {
                placeID = value;
                OnPropertyChanged("PlaceID");
            }
        }

        public string PlaceReference
        {
            get
            {
                return placeReference;
            }

            set
            {
                placeReference = value;
                OnPropertyChanged("PlaceReference");
            }
        }

        private void UpdateFavouritePlaces()
        {
            UpdateFavPlacesInLocal();

            this.signUpVehiclePageProcessor.MoveToPreviousPage();
        }

        public void UpdateFavPlacesInLocal()
        {
            bool exists = false;

            if (App.CurrentUserFavouritePlaces == null || App.CurrentUserFavouritePlaces.Count == 0)
                App.CurrentUserFavouritePlaces = new ObservableCollection<DriverLocator.Models.FavouritePlace>();

            DriverLocator.Models.FavouritePlace newFavPlace = new DriverLocator.Models.FavouritePlace()
            {
                Latitude = this.latitude,
                Longitude = this.longitude,
                PlaceName = this.placeName,
                PreviousUserGivenPlaceName = this.previousUserGivenPlaceName,
                UserGivenplaceName = this.userGivenplaceName,
                PlaceID = this.placeID,
                PlaceReference = this.placeReference
            };

            foreach (DriverLocator.Models.FavouritePlace place in App.CurrentUserFavouritePlaces)
            {
                if (place.UserGivenplaceName == this.userGivenplaceName)
                {
                    App.CurrentUserFavouritePlaces.Remove(place);
                    App.CurrentUserFavouritePlaces.Add(newFavPlace);
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                newFavPlace.PreviousUserGivenPlaceName = this.userGivenplaceName;
                if (!App.CurrentUserFavouritePlaces.Contains(newFavPlace))
                    App.CurrentUserFavouritePlaces.Add(newFavPlace);
            }
        } 

    }
}
