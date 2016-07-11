using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UpdateRideHistoryRequest
    {
        public string Id { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus Status { get; set; }
    }
}
