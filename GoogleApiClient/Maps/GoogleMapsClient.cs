using Common;
using GoogleApiClient.Helpers;
using GoogleApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApiClient.Maps
{
    public class GoogleMapsDirectionsClient
    {

       
        public string ApiKey
        {
            get 
            {
                return "AIzaSyC4gJh0TBYqXX3H0PXXGdMJIQok4TivVZo";
            }
           
        }

        public GoogleMapsDirectionsClient()
        {

        }

        public GetDirectionsResponse GetDirections(GetDirectionRequest request)
        {
            GoogleDirectionsApiUrlBuilder urlBuilder = new GoogleDirectionsApiUrlBuilder();
            urlBuilder.ApiKey = this.ApiKey;
            urlBuilder.AddWaypoints<Coordinate>(request.WayPoints);
            urlBuilder.SourceCoordinates = request.SourceCoordinate;
            urlBuilder.DestinationCoordinates = request.DestinationCoordinate;

            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Url = urlBuilder.GetGeneratedUrl();
            requestHandler.Method = "GET";
            return requestHandler.SendRequest<GetDirectionsResponse>();
        }
    }
}
