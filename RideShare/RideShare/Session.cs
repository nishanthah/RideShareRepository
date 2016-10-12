using Authentication.Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare
{
    public static class Session
    {
        public static object lockObj = new object();

        private static IAuthenticationService authenticationService;

        public static IAuthenticationService AuthenticationService
        {
            get
            {
                if (authenticationService == null)
                {
                    lock (lockObj)
                    {
                        if (authenticationService == null)
                            authenticationService = new Authentication.AuthenticationService();
                    }
                }

                return authenticationService;

            }           
        }

        public static void ClearAuthenticationInstance()
        {
            if(authenticationService != null)
            {
                lock(lockObj)
                {
                    if(authenticationService != null)
                    {
                        authenticationService = null;
                    }
                }
            }
            
        }

        //public static IAuthenticationService AuthenticationService { get; set; }

        public static string CurrentUserName { get; set; }
        public static UserType CurrentUserType { get; set; }
        public static string AppKey { get { return "7RaPmFE0QP-L_pDKsTyLCA"; } }
        //public static string AppMasterSecret { get { return "bXBa8jeLTIObXeOEuNzFBQ"; } }
        public static string AppMasterSecret { get { return "JEf6KdsJRY-G7a6QOg19Jg"; } }
    }
}
