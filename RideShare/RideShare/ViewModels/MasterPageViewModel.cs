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

        public string UserName
        {
            get
            {
                return _UserName;
            }

            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        public MasterPageViewModel()
        {
            var currentUserDetails = App.CurrentLoggedUser.User;
            byte[] ImageData = Convert.FromBase64String(currentUserDetails.profileImageEncoded);
            ProfileImageSource = ImageSource.FromStream(() => new MemoryStream(ImageData));
            UserName = App.CurrentLoggedUser.User.UserName;
        }
    }
}
