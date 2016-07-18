using GoogleApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApiClient.Helpers
{
    
    public class GoogleDirectionsApiUrlBuilder
    {

        public const string BASE_URL_WITH_WAYPOINTS = "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&waypoints={2}&key={3}";
        public const string BASE_URL_WITHOUT_WAYPOINTS = "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&key={2}";
        private Coordinate sourceCoordinates=new Coordinate();

        public string ApiKey { get; set; }
        public Coordinate SourceCoordinates
        {
            get { return sourceCoordinates; }
            set { sourceCoordinates= value; }
        }

        private Coordinate destinationCoordinates = new Coordinate();

        public Coordinate DestinationCoordinates
        {
            get { return destinationCoordinates; }
            set { destinationCoordinates = value; }
        }

        private string source;

        public string Source
        {
            get 
            { 
                if(!String.IsNullOrEmpty(source))
                {
                    return source;
                }
                else
                {
                    return sourceCoordinates.Latitude + "," + sourceCoordinates.Longitude;
                }
            }
            set { source = value; }
        }

        private string destination;

        public string Destination
        {
            get
            {
                if (!String.IsNullOrEmpty(destination))
                {
                    return destination;
                }
                else
                {
                    return destinationCoordinates.Latitude + "," + destinationCoordinates.Longitude;
                }
            }
            set { destination = value; }
        }

        private List<string> coordinatedWaypoints = new List<string>();


        public List<string> CoordinatedWaypoints
        {
            get { return coordinatedWaypoints; }
        }

        public void AddWaypoints<T>(IList<T> listOfCoordinates) where T : class
        {
            if (listOfCoordinates != null)
            {
                if (typeof(T) == typeof(Coordinate))
                {
                    IList<Coordinate> list = (IList<Coordinate>)listOfCoordinates;
                    foreach (var value in list)
                    {

                        coordinatedWaypoints.Add(value.Latitude + "," + value.Longitude);
                    }

                }
                else
                {
                    foreach (var value in listOfCoordinates)
                    {
                        coordinatedWaypoints.Add(value.ToString());
                    }
                }
            }
            
        } 

        public string GetGeneratedUrl()
        {
            if(coordinatedWaypoints != null || coordinatedWaypoints.Count > 0)
            {
                var waypoints = String.Join("|", CoordinatedWaypoints);
                return String.Format(BASE_URL_WITH_WAYPOINTS, Source, Destination, waypoints, ApiKey);
            }
            else
            {
                return String.Format(BASE_URL_WITH_WAYPOINTS, Source, Destination,ApiKey);
            }
            
        }
    }
}
