using System;
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

        string AuthenticationToken { get; }

        bool IsAuthenticated { get; }
    }
}
