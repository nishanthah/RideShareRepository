using System;
using Newtonsoft.Json;

namespace DriverLocator.Models
{
	public class SelectedUserCoordinateResponse : ResponseBase
	{
        [JsonProperty("userCoordinate")]
        public UserCoordinate UserCoordinate { get; set; }

		
	}
}

