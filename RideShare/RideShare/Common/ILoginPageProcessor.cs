using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.Common
{
    public interface ILoginPageProcessor
    {
        void MoveToMainPage();
        void MoveToCreateUserPage();
        void MoveToSignUpPage();
        void InvokeInMainThread(Action action);
        void MoveToForgotPasswordPage();
        void MoveToRegistrationCompletePage();
    }
}
