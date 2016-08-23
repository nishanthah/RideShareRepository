using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DriverLocator.Models
{
    public class UpdatePolylineRequest
    {
        [JsonProperty("polyLine")]
        public string PolyLine { get; set; }


   
    }
}
