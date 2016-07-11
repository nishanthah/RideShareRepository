using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApiClient.Models
{
    public class Coordinate
    {
        public Coordinate()
        {

        }
        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
