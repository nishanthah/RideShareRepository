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
    class GooglePlacesClient
    {
        private string ApiKey
        {
            get 
            {
                return "AIzaSyCcgcdVESAmhvKP74MbgH2SDyuS63qqNtI";
            }
           
        }

        public GooglePlacesClient()
        {

        }

        public GetPlacesResponse GetPlaces(String textSearch)
        {
            const string strAutoCompleteGoogleApi = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key={0}&input={1}";
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Url = String.Format(strAutoCompleteGoogleApi, ApiKey, textSearch);
            requestHandler.Method = "GET";
            return requestHandler.SendRequest<GetPlacesResponse>();          
        }

        public GetPlaceCoordinates GetCoordinates(String referenceNo)
        {
            const string strPlaceGoogleApi = "https://maps.googleapis.com/maps/api/place/details/json?reference={0}&key={1}";
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Url = String.Format(strPlaceGoogleApi,referenceNo, ApiKey);
            requestHandler.Method = "GET";
            return requestHandler.SendRequest<GetPlaceCoordinates>();
        }
    }    
}
