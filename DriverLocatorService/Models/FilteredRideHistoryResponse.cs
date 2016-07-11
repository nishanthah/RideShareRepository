using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class FilteredRideHistoryResponse : ResponseBase
    {
        [JsonProperty("data")]
        public IList<RideHistory> RideHistories { get; set; }
    }
}
