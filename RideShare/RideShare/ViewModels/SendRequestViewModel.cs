using DriverLocatorFormsPortable.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DriverLocator;
using DriverLocator.Models;
using RideShare.Common;

namespace RideShare.ViewModels
{
    public class SendRequestViewModel : ViewModelBase
    {
        private string driverUserName;

        private string source;

        private string destination;

        public string DriverUserName
        {
            get
            {
                return driverUserName;
            }
            set
            {
                driverUserName = value;
                OnPropertyChanged("DriverUserName");
            }
        }

        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                OnPropertyChanged("Source");
            }
        }

        public string Destination
        {
            get
            {
                return destination;
            }
            set
            {
                destination = value;
                OnPropertyChanged("Destination");
            }
        }

        public string SourceName
        {
            get
            {
                return Source.Split(',')[0];
            }
        }

        public string SourceLongitude
        {
            get
            {
                return Source.Split(',')[1];
            }
        }

        public string SourceLatitude
        {
            get
            {
                return Source.Split(',')[2];
            }
        }

        public string DestinationName
        {
            get
            {
                return Destination.Split(',')[0];
            }
        }

        public string DestinationLongitude
        {
            get
            {
                return Destination.Split(',')[1];
            }
        }

        public string DestinationLatitude
        {
            get
            {
                return Destination.Split(',')[2];
            }
        }

        IMapPageProcessor mapPageProcessor;
        DriverLocatorService driverLoacatorService;

        public ICommand SendRequestCommand { protected set; get; }

        public SendRequestViewModel(IMapPageProcessor mapPageProcessor, DriverLocatorService driverLocatorService)
        {
            this.mapPageProcessor = mapPageProcessor;
            this.driverLoacatorService = driverLocatorService;
            this.SendRequestCommand = new RelayCommand(SendRequest);
        }

        public void SendRequest()
        {
            RideHistory rideHistory = new RideHistory();
            rideHistory.UserName = Session.CurrentUserName;
            rideHistory.DiverUserName = DriverUserName;
            rideHistory.SourceName = SourceName;
            rideHistory.SourceLongitude = SourceLongitude;
            rideHistory.SourceLatitude = SourceLatitude;
            rideHistory.DestinationName = DestinationName;
            rideHistory.DestinationLongitude = DestinationLongitude;
            rideHistory.DestinationLatitude = DestinationLatitude;
            var result = driverLoacatorService.CreateHistory(rideHistory);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
            }
        }

    }
}
