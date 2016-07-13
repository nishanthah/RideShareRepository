using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UpdateUserTypeRequest
    {
        [JsonProperty("userType")]
        public UserType UserType { get; set; }
    }
}
