using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApiClient.Models
{
    public class GetDirectionRequest
    {
        private Coordinate sourceCoordinate = new Coordinate();

        public Coordinate SourceCoordinate
        {
            get { return sourceCoordinate; }
            set { sourceCoordinate = value; }
        }

        private Coordinate destinationCoordinate = new Coordinate();

        public Coordinate DestinationCoordinate
        {
            get { return destinationCoordinate; }
            set { destinationCoordinate = value; }
        }

        public List<Coordinate> WayPoints { get; set; }


    }
}
