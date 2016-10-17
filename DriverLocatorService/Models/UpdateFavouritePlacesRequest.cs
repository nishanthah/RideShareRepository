using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UpdateFavouritePlacesRequest
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("userGivenplaceName")]
        public string UserGivenplaceName { get; set; }

        [JsonProperty("placeName")]
        public string PlaceName { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("previousUserGivenPlaceName")]
        public string PreviousUserGivenPlaceName { get; set; }

        [JsonProperty("placeID")]
        public string PlaceID { get; set; }

        [JsonProperty("placeReference")]
        public string PlaceReference { get; set; }
    }
}
