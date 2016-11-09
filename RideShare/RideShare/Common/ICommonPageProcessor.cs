using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.Common
{
    public interface ICommonPageProcessor
    {
        void MoveToLoginPage();
        void MoveToMainPage();
        //void MoveToPage(string page);
        //void MoveToPreviousPage();
        //void MoveToNextPage();
        void InvokeInMainThread(Action action);
    }
}
