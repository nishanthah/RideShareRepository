
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.SharedInterfaces
{
    public interface ILocationService
    {
        Location GetCurrentLocation();       
    }
}
