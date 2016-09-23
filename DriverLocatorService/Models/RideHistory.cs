using Common.Models;
using GoogleApiClient.Helpers;
using GoogleApiClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace DriverLocator.Models
{

    public enum RequestStatus
    {
        Requested = 1,
        DriverAccepted = 2,
        DriverRejected = 3,
        RiderMet = 4,
        RideCompleted = 5
    }
    /* Sample Request Body
      {     
        "userName": "user1x", 
        "driverUserName": "user2x",
        "requestStatus":1,
        "sourseName": "Colombo",
        "destinationName":"Kandy",
        "sourceLongitude": "12.34",
        "sourceLatitude": "22.5",
        "destinationLongitude": "23",
        "destinationLatitude": "55"
        }
     */
    public class RideHistory
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("driverUserName")]
        public string DiverUserName { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("notificationStatus")]
        public NotificationStatus NotificationStatus { get; set; }

        [JsonProperty("sourseName")]
        public string SourceName { get; set; }

        [JsonProperty("destinationName")]
        public string DestinationName { get; set; }

        [JsonProperty("sourceLongitude")]
        public string SourceLongitude { get; set; }

        [JsonProperty("sourceLatitude")]
        public string SourceLatitude { get; set; }

        [JsonProperty("destinationLongitude")]
        public string DestinationLongitude { get; set; }

        [JsonProperty("destinationLatitude")]
        public string DestinationLatitude { get; set; }

        [JsonProperty("polyline")]
        public string PolyLine { get; set; }

        [JsonProperty("requestAcceptTime")]
        public DateTime RequestAcceptTime { get; set; }

        [JsonProperty("requestRejectTime")]
        public DateTime RequestRejectTime { get; set; }

        [JsonProperty("riderMeetTime")]
        public DateTime RiderMeetTime { get; set; }

        [JsonProperty("riderEndTime")]
        public DateTime RiderEndTime { get; set; }

        public IList<Coordinate> DecodedOverviewPolyLine{ get { return PolyLineDecoder.Decode(this.PolyLine); } }
    }
}
