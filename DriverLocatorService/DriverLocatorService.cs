using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Common;
using Common;
using DriverLocator.Models;
using Common.Models;

namespace DriverLocator
{

    public class DriverLocatorService
    {
#if Local
                private const string SERVER = "http://172.26.204.15:8079";
#else
        private const string SERVER = "http://ridesharemain.herokuapp.com";
        #endif

        //private const string SERVER = "http://172.28.40.49:8079";
        private const string GET_USERS_URL = SERVER+"/api/users";
        private const string SAVE_USERS_URL = SERVER+"/api/users";
        private const string UPDATE_USER_COORDINATE_URL = SERVER + "/api/updatecoordinates";
        private const string SELECTED_USER_COORDINATE_URL = SERVER + "/api/users/{0}";
        private const string CREATE_RIDEHISTORY_URL = SERVER + "/api/ridehistory";
        private const string UPDATE_POLYLINE_URL = SERVER + "/api/ridehistory/{0}/poly_line";
        private const string RIDEHISTORY_BY_FILTER_URL = SERVER + "/api/ridehistory/{0}/?filter={1}";
        private const string RIDEHISTORY_UPDATE_STATUS_URL = SERVER + "/api/ridehistory/status/{0}";
        private const string UPDATE_USER_TYPE_URL = SERVER + "/api/users/{0}/type";
        private const string UPDATE_USER_LOCATION_URL = SERVER + "/api/users/{0}/location";
        private const string UPDATE_USER_DESTINATION_URL = SERVER + "/api/users/{0}/destination";
        private const string GET_DRIVERS_URL = SERVER + "/api/drivers";
        private const string GET_RIDERS_URL = SERVER + "/api/riders";
        private const string UPDATE_USER_VEHICLE_URL = SERVER + "/api/vehicles";
        private const string GET_USER_VEHICLE_URL = SERVER + "/api/users/{0}/vehicles";
        private const string FINISH_RIDE_URL = SERVER + "/api/ridehistory/driver/{0}/ride/end";
        private const string UPDATE_NOTIFICATION_STATUS_URL = SERVER + "/api/ridehistory/notification_status/{0}";
        private const string UPDATE_USER_LOGIN_STATUS_URL = SERVER + "/api/users/{0}/login_status";
		private const string GET_USER_VEHICLE_DEF_DATA_URL = SERVER + "/api/vehicledefinitiondata";
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
            requestHandler.Method = HttpMethod.GET;
            requestHandler.Url = GET_USERS_URL;
            var userCoordinateResponse=requestHandler.SendRequest<UserLocationResponse>();
            return userCoordinateResponse;
        }

        public UserVehiclesResponse GetUserVehiclesByUser(string userName)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.GET;
            requestHandler.Url = String.Format(GET_USER_VEHICLE_URL, userName);
            var userVehiclesResponse = requestHandler.SendRequest<UserVehiclesResponse>();
            return userVehiclesResponse;
        }

        public SaveUserDataResponse SaveUserData(User userCoordinate)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.POST;
            requestHandler.Url = SAVE_USERS_URL;
            var result=requestHandler.SendRequest<User, SaveUserDataResponse>(userCoordinate);
            return result;
        }

        public UpdateUserLoginStatusResponse UpdateUserLoginStatus(string userName,bool isLoggedIn)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = String.Format(UPDATE_USER_LOGIN_STATUS_URL, userName);
            var userLoginStatusRequest = new UpdateUserLoginStatusRequest() { LoginStatus = isLoggedIn };
            var result = requestHandler.SendRequest<UpdateUserLoginStatusRequest, UpdateUserLoginStatusResponse>(userLoginStatusRequest);
            return result;
        }

        public UpdateUserLocationResponse UpdateUserLocation(string userName, UpdateUserLocationRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = String.Format(UPDATE_USER_LOCATION_URL, userName);
            var result = requestHandler.SendRequest<UpdateUserLocationRequest, UpdateUserLocationResponse>(request);
            return result;
        }

        public UpdateVehicleDetailsResponse UpdateVehicleDetails(UpdateVehicleDetailsRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.POST;
            requestHandler.Url = UPDATE_USER_VEHICLE_URL;
            var result = requestHandler.SendRequest<UpdateVehicleDetailsRequest, UpdateVehicleDetailsResponse>(request);
            return result;
        }

        public UpdateUserDestinationResponse UpdateUserDestination(string userName, UpdateUserDestinationRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = String.Format(UPDATE_USER_DESTINATION_URL, userName);
            var result = requestHandler.SendRequest<UpdateUserDestinationRequest, UpdateUserDestinationResponse>(request);
            return result;
        }

        public SelectedUserCoordinateResponse GetSelectedUserCoordinate(string userName)
		{
			HttpRequestHandler requestHandler = new HttpRequestHandler();
			requestHandler.AccessToken = authenticationService.AuthenticationToken;
			requestHandler.Method = HttpMethod.GET;
			requestHandler.Url = String.Format(SELECTED_USER_COORDINATE_URL,userName);
			var result=requestHandler.SendRequest<SelectedUserCoordinateResponse>();
			return result;
		}

        public CreateRideHistoryResponse CreateHistory(RideHistory rideHistory)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.POST;
            requestHandler.Url = CREATE_RIDEHISTORY_URL;
            var result = requestHandler.SendRequest<RideHistory, CreateRideHistoryResponse>(rideHistory);
            return result;
        }

        public UpdatePolylineResponse UpdatePolyline(string id, UpdatePolylineRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = String.Format(UPDATE_POLYLINE_URL, id);
            var result = requestHandler.SendRequest<UpdatePolylineRequest, UpdatePolylineResponse>(request);
            return result;
        }

        public FilteredRideHistoryResponse GetRideHistoryByFilter(string filterFieldName, string value)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.GET;
            requestHandler.Url = String.Format(RIDEHISTORY_BY_FILTER_URL, value, filterFieldName);
            var result = requestHandler.SendRequest<FilteredRideHistoryResponse>();
            return result;
        }

        public UpdateRideHistoryStatusResponse UpdateRideHistoryStatus(UpdateRideHistoryRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = String.Format(RIDEHISTORY_UPDATE_STATUS_URL, request.Id);
            var result = requestHandler.SendRequest<UpdateRideHistoryRequest, UpdateRideHistoryStatusResponse>(request);
            return result;
        }

        public UpdateUserTypeResponse UpdateUserType(string userName,UpdateUserTypeRequest request)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url =String.Format(UPDATE_USER_TYPE_URL,userName);
            var result = requestHandler.SendRequest<UpdateUserTypeRequest, UpdateUserTypeResponse>(request);
            return result;
        }

        public async Task<UserLocationResponse> GetDrivers()
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.GET;
            requestHandler.Url = GET_DRIVERS_URL;
            var result = await requestHandler.SendRequestAsync<UserLocationResponse>();
            return result;
        }

        public async Task<UserLocationResponse> GetRiders()
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.GET;
            requestHandler.Url = GET_RIDERS_URL;
            var result = await requestHandler.SendRequestAsync<UserLocationResponse>();
            return result;
        }

        public async Task<FinishRideResponse> FinishRide(string driverUserName)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = String.Format(FINISH_RIDE_URL,driverUserName);
            var result = await requestHandler.SendRequestAsync<FinishRideResponse>();
            return result;
        }

        public async Task<UpdateNotificationStatusResponse> UpdateNotificationStatus(string notificationId,NotificationStatus notificationStatus)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = String.Format(UPDATE_NOTIFICATION_STATUS_URL, notificationId);
            var request = new UpdateNotificationStatusRequest() { NotificationStatus = notificationStatus};
            var result = await requestHandler.SendRequestAsync<UpdateNotificationStatusRequest, UpdateNotificationStatusResponse>(request);
            return result;
        }
		
		public UserVehicleDefinitionDataResponse GetUserVehicleDefinitionData()
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = HttpMethod.GET;
            requestHandler.Url = GET_USER_VEHICLE_DEF_DATA_URL;
            var userVehicleDefinitionDataResponse = requestHandler.SendRequest<UserVehicleDefinitionDataResponse>();
            return userVehicleDefinitionDataResponse;
        }
    }
}
