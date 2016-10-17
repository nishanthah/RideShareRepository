using DriverLocator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverLocator.Models
{
    public class UserFavouritePlacesResponse : ResponseBase
    {
        [JsonProperty("userFavPlaces")]
        public ObservableCollection<FavouritePlace> UserFavouritePlaces { get; set; }
    }
}
