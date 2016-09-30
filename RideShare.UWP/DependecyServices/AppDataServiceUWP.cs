using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(RideShare.UWP.DependecyServices.AppDataServiceUWP))]
namespace RideShare.UWP.DependecyServices
{
    public class AppDataServiceUWP : IAppDataService
    {
       
        public void Save(string key, string value)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[key] = value;

        }

        public string Get(string key)
        {
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Object value = localSettings.Values[key];
                return value.ToString();
               
            }
            catch(Exception ex)
            {
                return null;
            }

        }
    }
}
