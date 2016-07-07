using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using UrbanAirshipClient.Common;
using UrbanAirshipClient.Models;

namespace UrbanAirshipClient
{
    public enum DeviceTypes
    {
        Android,
        IOS
    }

    public class UrbanAirshipApiClient : INotificationClient
    {
        private const string REGISTER_USER_URL = "https://go.urbanairship.com/api/named_users/associate";
        private const string URBAN_AIRSHIP_API_ACCEPT_HEDERS = "application/vnd.urbanairship+json;version=3;";
        public string AppKey { get; set; }
        public string AppMasterSecret { get; set; }

        public UrbanAirshipApiClient(string appKey,string appMasterSecret)
        {
            this.AppKey = appKey;
            this.AppMasterSecret = appMasterSecret;
        }

        public bool RegisterUser(RegisterUserRequest regUserRequest)
        {
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            requestHandler.BasicAuthorization.UserName = AppKey;
            requestHandler.BasicAuthorization.Password = AppMasterSecret;
            requestHandler.AcceptHeders = URBAN_AIRSHIP_API_ACCEPT_HEDERS;
            requestHandler.Url = REGISTER_USER_URL;
            requestHandler.Method = "POST";
            return requestHandler.SendRequest<RegisterUserRequest,RegisterUserResponse>(regUserRequest).Success;
        }
    }
}
