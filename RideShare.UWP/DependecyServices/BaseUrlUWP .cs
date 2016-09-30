using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.UWP.DependecyServices.BaseUrlUWP))]
namespace RideShare.UWP.DependecyServices
{
    public class BaseUrlUWP : IBaseUrl
    {
        public string Get()
        {
            
            return "ms-appx://Rideshare/Assets/";
        }
    }
}
