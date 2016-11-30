using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.Utilities
{
    public class CountryCode
    {
        [JsonProperty("code")]
        string Code;

        [JsonProperty("name")]
        string CountryName;
    }

    public class CountryCodeUtility
    {
        public List<string> GetCountryCodes()
        {
            List<string> countryCodes = new List<string>();
            string json = DependencyService.Get<RideShare.SharedInterfaces.IBaseUrl>().Get() + "CountryCodes.json";
            var codes = Newtonsoft.Json.JsonConvert.DeserializeObject<CountryCode>(json);
            return countryCodes;
        }
    }
}
