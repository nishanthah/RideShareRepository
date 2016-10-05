using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UpdateVehicleDetailsRequest
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("vehicleMake")]
        public string VehicleMake { get; set; }

        [JsonProperty("vehicleModel")]
        public string VehicleModel { get; set; }

        [JsonProperty("vehicleColor")]
        public string VehicleColor { get; set; }

        [JsonProperty("vehicleMaxPassengerCount")]
        public int VehicleMaxPassengerCount { get; set; }

        [JsonProperty("vehicleNumberPlate")]
        public string VehicleNumberPlate { get; set; }

        [JsonProperty("previousVehicleNumberPlate")]
        public string PreviousVehicleNumberPlate { get; set; }
 
    }
}
