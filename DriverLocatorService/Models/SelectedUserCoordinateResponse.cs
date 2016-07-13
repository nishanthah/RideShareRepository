using System;
using Newtonsoft.Json;

namespace DriverLocator.Models
{
	public class SelectedUserCoordinateResponse : ResponseBase
	{
        [JsonProperty("userData")]
        public UserLocation UserLocation { get; set; }

		
	}
}

