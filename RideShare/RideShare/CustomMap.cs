using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace RideShare
{
    public class CustomPin
    {
        public Pin Pin { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string MobileNo { get; set; }
        public string Image { get; set; }
        public string UserName { get; set; }
        public UserType UserType { get; set; }
    }

    public class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }

        public List<Position> RouteCoordinates { get; set; }

        public Action<CustomPin> OnInfoWindowClicked;

        public CustomMap()
        {
            RouteCoordinates = new List<Position>();
            CustomPins = new List<CustomPin>();
        }
    }
}
