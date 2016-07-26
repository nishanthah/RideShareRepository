using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Common;
using Common;
using DriverLocator.Models;

namespace DriverLocator
{
    public class DriverLocatorService
    {
        //private const string SERVER = "http://172.28.40.120:8079";
        private const string SERVER = "http://ridesharemain.herokuapp.com";
        private const string GET_USERS_URL = SERVER+"/api/users";
        private const string SAVE_USERS_URL = SERVER+"/api/users";
        private const string UPDATE_USER_COORDINATE_URL = SERVER + "/api/updatecoordinates";
        private const string SELECTED_USER_COORDINATE_URL = SERVER + "/api/users/{0}";
        private const string CREATE_RIDEHISTORY_URL = SERVER + "/api/ridehistory";
        private const string RIDEHISTORY_BY_FILTER_URL = SERVER + "/api/ridehistory/{0}/?filter={1}";
        private const string RIDEHISTORY_UPDATE_STATUS_URL = SERVER + "/ridehistory/status/{0}";
        private const string UPDATE_USER_TYPE_URL = SERVER + "/api/users/type";
        private const string UPDATE_USER_LOCATION_URL = SERVER + "/api/users/{0}/location";
        private const string GET_DRIVERS_URL = SERVER + "/api/drivers";
        private const string GET_RIDERS_URL = SERVER + "/api/riders";
        //private const string WEB_SOCKET_URL = "http://172.28.40.120:8079/";

        IAuthenticationService authenticationService;
        //private Socket socketConnection;

        //public event EventHandler<string> OnMapUpdatedNotificationReceived;

        public DriverLocatorService(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;           
        }

        public UserLocationResponse ViewUserCoordinates()
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "GET";
            requestHandler.Url = GET_USERS_URL;
            var userCoordinateResponse=requestHandler.SendRequest<UserLocationResponse>();
            return userCoordinateResponse;
        }

        public SaveUserDataResponse SaveUserData(User userCoordinate)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "POST";
            requestHandler.Url = SAVE_USERS_URL;
            var result=requestHandler.SendRequest<User, SaveUserDataResponse>(userCoordinate);
            return result;
        }

        public UpdateUserLocationResponse UpdateUserLocation(string userName,UpdateUserLocationRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "PUT";
            requestHandler.Url = String.Format(UPDATE_USER_LOCATION_URL, userName);
            var result = requestHandler.SendRequest<UpdateUserLocationRequest, UpdateUserLocationResponse>(request);
            return result;
        }

        public SelectedUserCoordinateResponse GetSelectedUserCoordinate(string userName)
		{
			HttpRequestHandler requestHandler = new HttpRequestHandler();
			requestHandler.AccessToken = authenticationService.AuthenticationToken;
			requestHandler.Method = "GET";
			requestHandler.Url = String.Format(SELECTED_USER_COORDINATE_URL,userName);
			var result=requestHandler.SendRequest<SelectedUserCoordinateResponse>();
			return result;
		}

        public CreateRideHistoryResponse CreateHistory(RideHistory rideHistory)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "POST";
            requestHandler.Url = CREATE_RIDEHISTORY_URL;
            var result = requestHandler.SendRequest<RideHistory, CreateRideHistoryResponse>(rideHistory);
            return result;
        }

        public FilteredRideHistoryResponse GetRideHistoryByFilter(string filterFieldName, string value)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "GET";
            requestHandler.Url = String.Format(RIDEHISTORY_BY_FILTER_URL, value, filterFieldName);
            var result = requestHandler.SendRequest<FilteredRideHistoryResponse>();
            return result;
        }

        public UpdateRideHistoryStatusResponse UpdateRideHistoryStatus(UpdateRideHistoryRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "PUT";
            requestHandler.Url = String.Format(RIDEHISTORY_UPDATE_STATUS_URL, request.Id);
            var result = requestHandler.SendRequest<UpdateRideHistoryRequest, UpdateRideHistoryStatusResponse>(request);
            return result;
        }

        public UpdateUserTypeResponse UpdateUserType(UpdateUserTypeRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "PUT";
            requestHandler.Url = UPDATE_USER_TYPE_URL;
            var result = requestHandler.SendRequest<UpdateUserTypeRequest, UpdateUserTypeResponse>(request);
            return result;
        }

        public UserLocationResponse GetDrivers()
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "GET";
            requestHandler.Url = GET_DRIVERS_URL;
            var result = requestHandler.SendRequest<UserLocationResponse>();
            return result;
        }

        public UserLocationResponse GetRiders()
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "GET";
            requestHandler.Url = GET_RIDERS_URL;
            var result = requestHandler.SendRequest<UserLocationResponse>();
            return result;
        }
    }
}
