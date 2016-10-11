using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UserVehicleDefinitionDataResponse : ResponseBase
    {
        [JsonProperty("userVehicleDefData")]
        public ObservableCollection<VehicleDefinitionData> UserVehicleDefinitionData { get; set; }
    }
}
