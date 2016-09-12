using GoogleApiClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApiClient.Models
{
    public class Route
    {
        [JsonProperty("overview_polyline")]
        public OverviewPolyLine OverViewPolyLine { get; set; }
       
        [JsonProperty("legs")]
        public IList<Leg> Legs { get; set; }
    }

    public class Leg
    {
        [JsonProperty("distance")]
        public Distance Distance { get; set; }

        [JsonProperty("duration")]
        public Distance Duration { get; set; }

        [JsonProperty("start_address")]
        public string StartAddress { get; set; }

        [JsonProperty("end_address")]
        public string EndAddress { get; set; }

        [JsonProperty("start_location")]
        public Location StartLocation { get; set; }

        [JsonProperty("end_location")]
        public Location EndLocation { get; set; }
    }

    public class Distance
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }

    public class Duration
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Location
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
    public class OverviewPolyLine
    {
        [JsonProperty("points")]
        public string Ponits { get; set; }

        public IList<Coordinate> DecodedOverViewPolyLine { get { return PolyLineDecoder.Decode(this.Ponits); } }


    }
    public class GetDirectionsResponse
    {
        [JsonProperty("routes")]
        public IList<Route> Routes { get; set; }
    }
}
