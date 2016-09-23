using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.ViewModels
{
    public class MasterPageViewModel : ViewModelBase
    {
        ImageSource _profileImageSource;
        string _UserName;

        public ImageSource ProfileImageSource
        {
            get
            {
                return _profileImageSource;
            }

            set
            {
                _profileImageSource = value;
                OnPropertyChanged("ProfileImageSource");
            }
        }

        public string UserDetails
        {
            get
            {
                return _UserName;
            }

            set
            {
                _UserName = value;
                OnPropertyChanged("UserDetails");
            }
        }
        public string _userType;
        public string UserType
        {
            get
            {
                return _userType;
            }

            set
            {
                _userType = value;
                OnPropertyChanged("UserType");
            }
        }

        public MasterPageViewModel()
        {
            var currentUserDetails = App.CurrentLoggedUser.User;
            if(!String.IsNullOrEmpty(currentUserDetails.profileImageEncoded))
            {
                byte[] ImageData = Convert.FromBase64String(currentUserDetails.profileImageEncoded);
                ProfileImageSource = ImageSource.FromStream(() => new MemoryStream(ImageData));
            }
            else
                ProfileImageSource = "add_picture.png";
            UserDetails = String.Format("{0} [{1}]",App.CurrentLoggedUser.User.UserName, App.CurrentLoggedUser.User.UserType.ToString());
        }
    }
}
