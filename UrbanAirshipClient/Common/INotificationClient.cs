using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanAirshipClient.Models;

namespace UrbanAirshipClient.Common
{
    public interface INotificationClient
    {
        bool RegisterUser(RegisterUserRequest regUserRequest);
    }
}
