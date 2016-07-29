using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class CreateRideHistoryResponse : ResponseBase
    {
        [JsonProperty("id")]
        public string RequestId { get; set; }
    }
}
