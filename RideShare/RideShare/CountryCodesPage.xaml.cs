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
    public interface ICountryCodeSelectionResult
    {
        Action<string> OnCountryCodeSelection { get; set; }
    }

    public partial class CountryCodesPage : ContentPage, ISignUpPageProcessor
    {
        ICountryCodeSelectionResult countryCodeSelectionResult;
        public CountryCodesPage(ICountryCodeSelectionResult countryCodeAdditionResult, string countryName)
        {
            this.countryCodeSelectionResult = countryCodeAdditionResult;
            InitializeComponent();
            Content.BindingContext = new CountryCodeViewModel(this);
            var vm = Content.BindingContext as CountryCodeViewModel;
            countryCodeListView.ItemSelected += countryCodeListView_ItemSelected;
            countryCodeListView.ItemsSource = vm.CountryCodes;            
            if (!String.IsNullOrEmpty(countryName))
                countryCodeListView.SelectedItem = vm.CountryCodes.FirstOrDefault(x => x.Key == countryName);           
        }

        void countryCodeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>)countryCodeListView.SelectedItem;
            if (!selectedItem.Equals(default(KeyValuePair<string, string>)))
                this.countryCodeSelectionResult.OnCountryCodeSelection(selectedItem.Key);
            else
                this.countryCodeSelectionResult.OnCountryCodeSelection(String.Empty);
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
