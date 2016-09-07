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
               
                var historyInfo = driverLocatorService.GetRideHistoryByFilter("_id", userData.UserLocation.User.RecentRequest).RideHistories.FirstOrDefault();

                if (userData.UserLocation.User.UserType == UserType.Rider)
                {
                    if (historyInfo.RequestStatus == RequestStatus.RideCompleted)
                    {
                        precenter = new RiderViewPresenter(mapPageProcessor, mapSocketService);
                    }
                    else
                    {
                        precenter = new RiderNavigationViewPresenter(mapPageProcessor, mapSocketService, historyInfo);
                    }
                    
                }
                else if (userData.UserLocation.User.UserType == UserType.Driver)
                {
                    if (notificationInfo != null)
                    {
                        precenter = new DriverNavigationViewPresenter(mapPageProcessor, mapSocketService, notificationInfo, historyInfo);
                    }
                    else
                    {
                        if (historyInfo.RequestStatus == RequestStatus.RideCompleted)
                        {
                            precenter = new DriverViewPresenter(mapPageProcessor, mapSocketService);
                        }
                        else
                        {
                            precenter = new DriverNavigationViewPresenter(mapPageProcessor, mapSocketService, historyInfo);
                        }
                            
                    }
                }

            }
            else
            {
                if (userData.UserLocation.User.UserType == UserType.Rider)
                {
                    precenter = new RiderViewPresenter(mapPageProcessor, mapSocketService);
                }

                else if (userData.UserLocation.User.UserType == UserType.Driver)
                {
                    precenter = new DriverViewPresenter(mapPageProcessor, mapSocketService);
                }

            }

            return precenter;
        }
    }
}
