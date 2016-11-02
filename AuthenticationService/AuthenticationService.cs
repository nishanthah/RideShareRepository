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
#if Local
                private const string SERVER = "http://172.26.204.146:8078";
#else
        private const string SERVER = "http://vauthapp.herokuapp.com";
        #endif

        private const string CREATE_USER_URL = SERVER + "/authapp/useraccount";
        private const string AUTHENCATION_URL = SERVER + "/authapp/accesstoken";
        private const string USER_INFO_URL = SERVER + "/authapp/userinfo";
        private const string USER_INFO_BY_GUID = SERVER + "/authapp/userinfobyguid/{0}";
        private const string USER_INFO_BY_USERNAME_GUID_SEND_EMAIL_URL = SERVER + "/authapp/userinfosendemailwithguid/{0}";
        private const string USER_INFO_BY_USERNAME_CODE_SEND_EMAIL_URL = SERVER + "/authapp/userinfosendemailwithcode/{0}/{1}";

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
            if(userInfo.IsSuccess)
            {
                IsAuthenticated = true;
                this.authenticationToken = token;                
            }
            else
            {
                IsAuthenticated = false;                
            }
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

        public UserInfoResponse GetUserInfoByGUID(string guid)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Url = String.Format(USER_INFO_BY_GUID, guid);
            requestHandler.Method = HttpMethod.GET;
            requestHandler.AccessToken = authenticationToken;
            var authenticationInfo = requestHandler.SendRequest<AuthenticationResponse>();
            UserInfoResponse userInfo = null;
            if (authenticationInfo.IsSuccess)
            {
                IsAuthenticated = true;
                authenticationToken = authenticationInfo.Token;
                userInfo = GetUserInfo(authenticationToken);
            }
            else
            {
                IsAuthenticated = false;
            }
            return userInfo;
        }

        public ResponseBase GetUserInfoByUserNameAndSendEmail(string userName)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Url = String.Format(USER_INFO_BY_USERNAME_GUID_SEND_EMAIL_URL, userName);            
            requestHandler.Method = HttpMethod.GET;
            requestHandler.AccessToken = authenticationToken;
            var userInfo = requestHandler.SendRequest<ResponseBase>();
            return userInfo;
        }

        public ResponseBase UpdateUserPassWord(string userName)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.Method = HttpMethod.PUT;
            requestHandler.Url = String.Format(USER_INFO_BY_USERNAME_CODE_SEND_EMAIL_URL, userName, (userName.GetHashCode() % 100000).ToString());
            requestHandler.AccessToken = authenticationToken;
            return requestHandler.SendRequest<ResponseBase>();
        }        
    }
}
