﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Models;

namespace Authentication.Common
{
    public interface IAuthenticationService
    {
        bool Authenticate(string userName, string password);
        UserInfoResponse GetUserInfo(string token);
        CreateUserResponse CreateUser(User user);
        UpdateUserResponse UpdateUser(User user);
        string AuthenticationToken { get; set; }

        bool IsAuthenticated { get; }
    }
}
