using DriverLocatorFormsPortable.Common;
using RideShare.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RideShare.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        string version;

        public AboutViewModel()
        {
            this.Version = "Application Version - " + App.ApplicationVersion;
        }

        public string Version
        {
            get
            {
                return version;
            }

            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }
    }
}
