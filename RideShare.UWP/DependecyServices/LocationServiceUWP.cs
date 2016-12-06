using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonModels = Common.Models;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace RideShare.UWP.DependecyServices
{
    public class LocationServiceUWP : ILocationService
    {
        public CommonModels.Location GetCurrentLocation()
        {
            var accessStatus =  Geolocator.RequestAccessAsync();
            switch (accessStatus.GetResults())
            {
                case GeolocationAccessStatus.Allowed:

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };

                    // Subscribe to StatusChanged event to get updates of location status changes
                   // geolocator.StatusChanged += OnStatusChanged;

                    try
                    {
                        // Carry out the operation
                        Geoposition pos =  geolocator.GetGeopositionAsync().GetResults();
                        CommonModels.Location location = new CommonModels.Location();
                        location.Latitude = pos.Coordinate.Latitude;
                        location.Longitude = pos.Coordinate.Longitude;
                        return location;
                    }
                    catch (Exception ex)
                    {
                        // handle the exception (notify user, etc.)
                      //  return null;
                    }
                    break;

                case GeolocationAccessStatus.Denied:
                    // handle access denied case here
                 //   return null;
                    break;

                case GeolocationAccessStatus.Unspecified:
                    // handle unspecified error case here
                  //  return null;
                    break;
            }
            return null;

        }

        CommonModels.Location ILocationService.GetCurrentLocation()
        {
            throw new NotImplementedException();
        }

        void ILocationService.InitLocationService()
        {
            throw new NotImplementedException();
        }

        void ILocationService.StartLocationService()
        {
            throw new NotImplementedException();
        }

        void ILocationService.StopLocationService()
        {
            throw new NotImplementedException();
        }
    }
}
