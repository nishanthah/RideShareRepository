using Authentication.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare
{
    public static class Session
    {
        public static IAuthenticationService AuthenticationService { get; set; }
    }
}
