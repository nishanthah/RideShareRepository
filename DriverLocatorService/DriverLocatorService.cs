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
        private const string SERVER = "http://192.168.1.2:8079";
        //private const string SERVER = "http://192.168.42.166:8079";
        private const string GET_USERS_URL = SERVER+"/api/usercoordinates";
        private const string SAVE_USERS_URL = SERVER+"/api/saveuserdata";
        private const string UPDATE_USER_COORDINATE_URL = SERVER + "/api/updatecoordinates";
        private const string SELECTED_USER_COORDINATE_URL = SERVER + "/api/selectedusercoordinate";
        //private const string WEB_SOCKET_URL = "http://172.28.40.120:8079/";

        IAuthenticationService authenticationService;
        //private Socket socketConnection;

        //public event EventHandler<string> OnMapUpdatedNotificationReceived;

        public DriverLocatorService(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;           
        }

        public UserCoordinatesResponse ViewUserCoordinates()
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "GET";
            requestHandler.Url = GET_USERS_URL;
            var userCoordinateResponse=requestHandler.SendRequest<UserCoordinatesResponse>();
            return userCoordinateResponse;
        }

        public SaveUserDataResponse SaveUserData(UserCoordinate userCoordinate)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.AccessToken = authenticationService.AuthenticationToken;
            requestHandler.Method = "POST";
            requestHandler.Url = SAVE_USERS_URL;
            var result=requestHandler.SendRequest<UserCoordinate, SaveUserDataResponse>(userCoordinate);
            return result;
        }


		public SelectedUserCoordinateResponse GetSelectedUserCoordinate()
		{
			HttpRequestHandler requestHandler = new HttpRequestHandler();
			requestHandler.AccessToken = authenticationService.AuthenticationToken;
			requestHandler.Method = "GET";
			requestHandler.Url = SELECTED_USER_COORDINATE_URL;
			var result=requestHandler.SendRequest<SelectedUserCoordinateResponse>();
			return result;
		}

    }
}
