using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApiClient.Helpers
{
    public static class UnitConverter
    {
        public static string ToKilometers(this double value)
        {
            return (value / 1000) + "km";
        }

        public static string ToTimeString(this double value)
        {
            TimeSpan time = TimeSpan.FromSeconds(value);
            string str = time.ToString(@"hh\:mm\:ss");
            return str;
        }
    }
}
