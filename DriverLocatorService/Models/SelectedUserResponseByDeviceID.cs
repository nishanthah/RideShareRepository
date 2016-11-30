using DriverLocator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class SelectedUserResponseByDeviceID : ResponseBase
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }
    }
}
