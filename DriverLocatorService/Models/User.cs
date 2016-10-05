using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class Location
    {
        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }
    }

    public class User
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string EMail { get; set; }

        [JsonProperty("mobileNo")]
        public string MobileNo { get; set; }

        [JsonProperty("userType")]
        public UserType UserType { get; set; }

        [JsonProperty("profileImage")]
        public string profileImageEncoded { get; set; }

        [JsonProperty("resentRequest")]
        public string RecentRequest { get; set; }
    }

    public class UserLocation
    {

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("destination")]
        public Destination Destination { get; set; }

        [JsonProperty("vehicles")]
        public ObservableCollection<Vehicle> Vehicles { get; set; }

    }

    public class Destination : Location
    {
        [JsonProperty("name")]
        public string  Name { get; set; }

    }

    public class Vehicle
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("vehicleMake")]
        public string VehicleMake { get; set; }

        [JsonProperty("vehicleModel")]
        public string VehicleModel { get; set; }

        [JsonProperty("vehicleColor")]
        public string VehicleColor { get; set; }

        [JsonProperty("vehicleMaxPassengerCount")]
        public int VehicleMaxPassengerCount { get; set; }

        [JsonProperty("vehicleNumberPlate")]
        public string VehicleNumberPlate { get; set; }

        public string VehicleDisplayName { get; set; }
        
        [JsonProperty("previousVehicleNumberPlate")]
        public string PreviousVehicleNumberPlate { get; set; }
 
    }

    public class VehicleDefinitionData
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

    }
}
