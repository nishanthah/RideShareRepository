using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace RideShare.ViewModels
{
    public class CustomMapViewModel : ViewModelBase
    {
        private bool isShowingUser;

        public bool IsShowingUser
        {
            get { return isShowingUser; }
            set 
            {
                if (isShowingUser == value)
                    return;

                isShowingUser = value;
                OnPropertyChanged("IsShowingUser");
            }
        }
        //private CustomMap customMap;

        //public CustomMap CustomMap
        //{
        //    get { return customMap; }
        //    set { customMap = value;
        //    OnPropertyChanged("CustomMap");
        //    }
        //}
        string driverName;
        public string DriverName
        {
            get
            {
                return driverName;
            }
            set
            {
                driverName = value;
                OnPropertyChanged("DriverName");
            }
        }

        string riderMeetTime;
        public string RiderMeetTime
        {
            get
            {
                return riderMeetTime;
            }
            set
            {
                riderMeetTime = value;
                OnPropertyChanged("RiderMeetTime");
            }
        }
        private double heightRequest;

        public double HeightRequest
        {
            get { return heightRequest; }
            set 
            {
                if (heightRequest == value)
                    return;
                heightRequest = value;
                OnPropertyChanged("HeightRequest");
            }
        }

        private double widthRequest;

        public double WidthRequest
        {
            get { return widthRequest; }
            set 
            {
                if (widthRequest == value)
                    return;
                widthRequest = value;
                OnPropertyChanged("WidthRequest");
            }
        }

        private List<Position> routeCoordinates;

        public List<Position> RouteCoordinates
        {
            get { return routeCoordinates; }
            set
            {
                if (routeCoordinates == value)
                    return;
                routeCoordinates = value;
                OnPropertyChanged("RouteCoordinates");
            }
        }

        private double baseLatitude;

        public double BaseLatitude
        {
            get { return baseLatitude; }
            set
            {
                if (baseLatitude == value)
                    return;
                baseLatitude = value;
                OnPropertyChanged("BaseLatitude");
            }
        }

        private double baseLongitude;

        public double BaseLongitude
        {
            get { return baseLongitude; }
            set
            {
                if (baseLongitude == value)
                    return;
                baseLongitude = value;
                OnPropertyChanged("BaseLongitude");
            }
        }
    }
}
