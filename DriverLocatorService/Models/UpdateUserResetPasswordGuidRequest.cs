using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UpdateUserResetPasswordGuidRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("guid")]
        public string Guid { get; set; }
    }
}
