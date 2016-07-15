using GoogleApiClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApiClient.Models
{    
    public class Prediction
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("place_id")]
        public string Placeid { get; set; }

    }
    public class GetPlacesResponse
    {
        [JsonProperty("predictions")]
        public IList<Prediction> Predictions { get; set; }
    }
}
