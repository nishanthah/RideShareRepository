using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum LocationServiceStatus
    {        
        OutOfService = 0,        
        TemporarilyUnavailable = 1,
        Available = 2
    }
    public interface ILocationServiceStatusCallback
    {
        void OnLocationStatusChange(LocationServiceStatus status);
    }
}
