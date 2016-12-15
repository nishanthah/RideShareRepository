using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.SharedInterfaces
{
    public interface IFileReader
    {
        Dictionary<string, string> GetCountryCodesWithNames();
        Dictionary<string, string> GetCountryNamesWithFlagName();
    }
}
