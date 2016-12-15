using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RideShare.SharedInterfaces;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.Droid.DependecyServices.FileReaderServiceDroid))]
namespace RideShare.Droid.DependecyServices
{
    public class FileReaderServiceDroid : IFileReader
    {
        public Dictionary<string, string> GetCountryCodesWithNames()
        {
            Dictionary<string, string> countryCodeList = new Dictionary<string, string>();
            string countryCodes = String.Empty;
            Android.Content.Res.AssetManager assets = MainApp.Context.Assets;
            
            using (StreamReader r = new StreamReader(assets.Open("CountryCodes.json")))
            {
                countryCodes = r.ReadToEnd();
                CountryCodesResult items = JsonConvert.DeserializeObject<CountryCodesResult>(countryCodes);

                foreach (CountryCode cntry in items.Countries)
                {
                    countryCodeList.Add(cntry.CountryName, cntry.Code.Replace(" ", String.Empty));
                }
            }

            return countryCodeList;
        }

        public Dictionary<string, string> GetCountryNamesWithFlagName()
        {
            Dictionary<string, string> countryCodeList = new Dictionary<string, string>();
            string countryCodes = String.Empty;
            Android.Content.Res.AssetManager assets = MainApp.Context.Assets;

            using (StreamReader r = new StreamReader(assets.Open("CountryCodes.json")))
            {
                countryCodes = r.ReadToEnd();
                CountryCodesResult items = JsonConvert.DeserializeObject<CountryCodesResult>(countryCodes);

                foreach (CountryCode cntry in items.Countries)
                {
                    countryCodeList.Add(cntry.CountryName, cntry.CountryName.Replace(" ", "_"));
                }
            }

            return countryCodeList;
        }

        public class CountryCodesResult
        {
            [JsonProperty("countries")]
            public List<CountryCode> Countries { get; set; }
        }

        public class CountryCode
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("name")]
            public string CountryName { get; set; }

            public string CountryFlagImageName { get; set; }
        }
    }
}