using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UpdateUserDestinationRequest
    {
        [JsonProperty("destinationName")]
        public string DestinationName { get; set; }

        [JsonProperty("destinationLongitude")]
        public double Longitude { get; set; }

        [JsonProperty("destinationLatitude")]
        public double Latitude { get; set; }
    }
}
