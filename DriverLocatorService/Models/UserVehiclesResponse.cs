using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UserVehiclesResponse : ResponseBase
    {
        [JsonProperty("userVehicles")]
        public ObservableCollection<Vehicle> UserVehicles { get; set; }
    }
}
