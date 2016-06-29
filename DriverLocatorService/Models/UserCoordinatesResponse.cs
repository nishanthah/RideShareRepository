using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UserCoordinatesResponse : ResponseBase
    {
        [JsonProperty("userCoordinates")]
        public List<UserCoordinate> UserCoordinates { get; set; }
    }
}
