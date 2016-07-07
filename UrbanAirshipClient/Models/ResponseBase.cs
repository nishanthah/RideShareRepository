using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanAirshipClient.Models
{
    public class ResponseBase
    {
        [JsonProperty("ok")]
        public bool Success { get; set; }
    }
}
