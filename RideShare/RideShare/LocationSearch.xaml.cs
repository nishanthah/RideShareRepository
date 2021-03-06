﻿using GoogleApiClient.Maps;
using GoogleApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace RideShare
{
    public interface ILocationSelectionResult
    {
        Action<LocationSearchResult> OnLocationSelected { get; set; }
    }

    public class LocationSearchResult
    {
        public string LocationName { get; set; }
        public string LocationId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string LocationRefernce { get; set; }
        public string LocationDetail { get; set; }
    }


    public partial class LocationSearch : ContentPage
    {
        ILocationSelectionResult locationSelectionResult;
        LocationSearchResult selectedLocation;
        
        public LocationSearch(ILocationSelectionResult locationSelectionResult)
        {
            InitializeComponent();
            this.locationSelectionResult = locationSelectionResult;
            selectButton.Clicked += SelectButtonClicked;

            if(App.CurrentLoggedUser.FavouritePlaces != null || App.CurrentLoggedUser.FavouritePlaces.Count > 0)
            {
                var allFavouritePlaces = App.CurrentLoggedUser.FavouritePlaces.Select(x => new LocationSearchResult() { Latitude = double.Parse(x.Latitude), LocationDetail = x.PlaceName, LocationId = x.PlaceID, LocationName = x.UserGivenplaceName, LocationRefernce = x.PlaceReference, Longitude = double.Parse(x.Longitude) });
                listView.ItemsSource = allFavouritePlaces;
            }
            
            
        }

        private void SelectButtonClicked(object sender, EventArgs e)
        {
            GooglePlacesClient googlePlacesClient = new GooglePlacesClient();
            var locationDetails = googlePlacesClient.GetCoordinates(selectedLocation.LocationRefernce);

            selectedLocation.Longitude = locationDetails.CoordinateResult.Geometry.Location.Longitude;
            selectedLocation.Latitude = locationDetails.CoordinateResult.Geometry.Location.Latitude;
            locationSelectionResult.OnLocationSelected(selectedLocation);
           
            //App.Current.MainPage = new NavigationPage(new MapView());
            Navigation.PopAsync();
        }

        private void BindListData(string query)
        {
            GooglePlacesClient googlePlacesClient = new GooglePlacesClient();

            if(App.CurrentLoggedUser.FavouritePlaces != null && App.CurrentLoggedUser.FavouritePlaces.Count() > 0)
            {
                var allFavouritePlaces = App.CurrentLoggedUser.FavouritePlaces.Select(x => new LocationSearchResult() { Latitude = double.Parse(x.Latitude), LocationDetail = x.PlaceName, LocationId = x.PlaceID, LocationName = x.UserGivenplaceName, LocationRefernce = x.PlaceReference, Longitude = double.Parse(x.Longitude) });
                var favLocations = allFavouritePlaces.Where(x => x.LocationName.ToLower().Contains(query.ToLower()));
                var data = googlePlacesClient.GetPlaces(query);
                listView.ItemsSource = favLocations.Union(MapSearchData(data.Predictions)).ToList();
            }
            else
            {
                var data = googlePlacesClient.GetPlaces(query);
                listView.ItemsSource = MapSearchData(data.Predictions).ToList();
            }
            
            
            
        }

        private void OnValueChanged(object sender, TextChangedEventArgs e)
        { 
            
            BindListData(SearchEntry.Text);
        }

        private IList<LocationSearchResult> MapSearchData(IList<Prediction> predictions)
        {
            List<LocationSearchResult> locs = new List<RideShare.LocationSearchResult>();
            
            foreach (var prediction in predictions)
            {
                locs.Add(new LocationSearchResult() { Latitude = 1, Longitude = 11,LocationRefernce = prediction.Refernce, LocationId = prediction.Placeid, LocationName = prediction.Description });
            }
            return locs;
        }
        
        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedLocation = e.SelectedItem as LocationSearchResult;                        
        }

        
    }
}
