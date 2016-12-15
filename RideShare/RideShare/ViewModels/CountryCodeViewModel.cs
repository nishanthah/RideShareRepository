using DriverLocatorFormsPortable.Common;
using RideShare.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RideShare.ViewModels
{
    public class CountryCodeViewModel : ViewModelBase
    {
        Dictionary<string, string> countryCodes = new Dictionary<string, string>();
        //string currentUserCountryCode;

        ISignUpPageProcessor countryCodePageProcessor;
        public ICommand SelectCountryCode { protected set; get; }

        public CountryCodeViewModel(ISignUpPageProcessor passedCountryCodePageProcessor)
        {
            this.countryCodePageProcessor = passedCountryCodePageProcessor;

            RideShare.SharedInterfaces.IFileReader fileReader = DependencyService.Get<RideShare.SharedInterfaces.IFileReader>();
            countryCodes = fileReader.GetCountryNamesWithFlagName();

            this.SelectCountryCode = new RelayCommand(UpdateSelectedCountryCode);
        }

        public Dictionary<string, string> CountryCodes
        {
            get
            {
                return countryCodes;
            }

            set
            {
                countryCodes = value;
                OnPropertyChanged("CountryCodes");
            }
        }

        public bool isAuthenticated
        {
            get { return Session.AuthenticationService != null ? Session.AuthenticationService.IsAuthenticated : false; }
        }

        private void UpdateSelectedCountryCode()
        {
            this.countryCodePageProcessor.MoveToPreviousPage();
        }
    }
}
