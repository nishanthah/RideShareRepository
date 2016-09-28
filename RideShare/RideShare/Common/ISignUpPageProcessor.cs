using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.Common
{
    public interface ISignUpPageProcessor
    {
        void MoveToLoginPage();
        void MoveToMainPage();
        void MoveToNextPage();
        void MoveToPreviousPage();
    }
}
