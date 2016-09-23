using Common.Models;
using DriverLocator.Models;
using RideShare.Common;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.ViewPresenter
{
    public enum MapViewPreSenterType
    {
        RiderNavigationViewPresenter,
        RiderViewPresenter,
        DriverNavigationViewPresenter,
        DriverRequestedRidesPresenter,
        DriverViewPresenter
    }

    public class PresenterLocator
    {
        IMapPageProcessor mapPageProcessor;
        IMapSocketService mapSocketService;
        DriverLocator.DriverLocatorService driverLocatorService;
        IAppDataService appDataService;

        public PresenterLocator(IMapPageProcessor mapPageProcessor, DriverLocator.DriverLocatorService driverLocatorService)
        {
            this.driverLocatorService = driverLocatorService;
            this.mapPageProcessor = mapPageProcessor;
            this.mapSocketService = DependencyService.Get<IMapSocketService>();
            this.appDataService = DependencyService.Get<IAppDataService>();
        }

        public BaseMapViewPresenter GetPrecenter(NotificationInfo notificationInfo)
        {
            BaseMapViewPresenter precenter = null;
            var userData = driverLocatorService.GetSelectedUserCoordinate(appDataService.Get("current_user"));
            
            if(!String.IsNullOrEmpty(userData.UserLocation.User.RecentRequest))
            {
                
                if (userData.UserLocation.User.UserType == UserType.Rider)
                {
                    var historyInfo = driverLocatorService.GetRideHistoryByFilter("_id", userData.UserLocation.User.RecentRequest).RideHistories.FirstOrDefault();
                    if (historyInfo.RequestStatus == RequestStatus.Requested || historyInfo.RequestStatus == RequestStatus.DriverAccepted )
                    {
                        precenter = new RiderNavigationViewPresenter(mapPageProcessor, mapSocketService, historyInfo, driverLocatorService);

                    }
                    else
                    {
                        precenter = new RiderViewPresenter(mapPageProcessor, mapSocketService, driverLocatorService);
                    }
                    
                }
                else if (userData.UserLocation.User.UserType == UserType.Driver)
                {
                    
                    if (notificationInfo != null)
                    {
                        var historyInfo = driverLocatorService.GetRideHistoryByFilter("_id", notificationInfo.RequestId).RideHistories.FirstOrDefault();
                        precenter = new DriverNavigationViewPresenter(mapPageProcessor, mapSocketService, historyInfo, driverLocatorService);
                    }
                    else
                    {
                        var historyInfo = driverLocatorService.GetRideHistoryByFilter("driverUserName", userData.UserLocation.User.UserName).RideHistories;

                        var currectRidesCount = historyInfo.Where(x => x.RequestStatus == RequestStatus.DriverAccepted
                                                                    ||
                                                                    x.RequestStatus == RequestStatus.Requested 
                                                                    ||
                                                                    x.RequestStatus == RequestStatus.RiderMet).Count();

                        if (currectRidesCount > 0)
                        {
                            precenter = new DriverRequestedRidesPresenter(mapPageProcessor, mapSocketService, driverLocatorService);
                        }
                        else
                        {
                            precenter = new DriverViewPresenter(mapPageProcessor, mapSocketService, driverLocatorService);                            
                        }
                            
                    }
                }
               
            }
            else
            {
                if (userData.UserLocation.User.UserType == UserType.Rider)
                {
                    precenter = new RiderViewPresenter(mapPageProcessor, mapSocketService, driverLocatorService);
                }

                else if (userData.UserLocation.User.UserType == UserType.Driver)
                {
                    precenter = new DriverViewPresenter(mapPageProcessor, mapSocketService, driverLocatorService);
                }

            }

            return precenter;
        }

        //public BaseMapViewPresenter GetPrecenter(MapViewPreSenterType )
        //{

        //}
    }
}
