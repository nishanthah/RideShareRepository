using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.SharedInterfaces
{
    public interface IAppDataService
    {
        void Save(string key, string value);
        string Get(string key);
    }
}
