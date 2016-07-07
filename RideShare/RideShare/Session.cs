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

        public static string CurrentUserName { get; set; }
        public static string AppKey { get { return "IsFOZ2ABTlKQOEvJJkEJ5Q"; } }
        public static string AppMasterSecret { get { return "c7aYTrIvTZyVPomI04FQRA"; } }
    }
}
