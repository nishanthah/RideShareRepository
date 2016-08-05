using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Authentication.Models;
using Authentication.Common;

namespace Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private const string SERVER = "http://vauthapp.herokuapp.com";
        //private const string SERVER = "http://192.168.42.166:8078";
        private const string CREATE_USER_URL = SERVER + "/authapp/useraccount";
        private const string AUTHENCATION_URL = SERVER + "/authapp/accesstoken";
        private const string USER_INFO_URL = SERVER + "/authapp/userinfo";

        public string authenticationToken=String.Empty;

        public bool IsAuthenticated { get; set; }
        public string AuthenticationToken
        {
            get { return authenticationToken; }
            set { authenticationToken = value; }
        }

        public CreateUserResponse CreateUser(User user)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Method = HttpMethod.POST;
            requestHandler.Url = CREATE_USER_URL;
            return requestHandler.SendRequest<User, CreateUserResponse>(user);
        }

        public bool Authenticate(string userName, string password)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Url = AUTHENCATION_URL;
            requestHandler.Method = HttpMethod.POST;
            var user = new User() {UserName=userName,Password=password};
            var authenticationResult=requestHandler.SendRequest<User, AuthenticationResponse>(user);

            if(authenticationResult.IsSuccess)
            {
                authenticationToken = authenticationResult.Token;
                //UpdateClaimDetails(authenticationToken);              
            }
            IsAuthenticated = authenticationResult.IsSuccess;
            return authenticationResult.IsSuccess;
        }

        public UserInfoResponse GetUserInfo(string token)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Url = USER_INFO_URL;
            requestHandler.AccessToken = token;
            requestHandler.Method = HttpMethod.GET;
            var userInfo = requestHandler.SendRequest<UserInfoResponse>();
            return userInfo;
        }

        public UpdateUserResponse UpdateUser(User user)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = CREATE_USER_URL;
            requestHandler.AccessToken = authenticationToken;
            return requestHandler.SendRequest<User, UpdateUserResponse>(user);
        }
    }
}
