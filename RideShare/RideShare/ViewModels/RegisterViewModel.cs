using Authentication;
using Authentication.Models;
using DriverLocatorFormsPortable.Common;
using RideShare.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RideShare.ViewModels
{
    public class SignUpViewModel : ViewModelBase
        {
            string firstName;
            string lastName;
            string userName;
            string password;
            string email;
            ISignUpPageProcessor signUpPageProcessor;

            public SignUpViewModel(ISignUpPageProcessor signUpPageProcessor)
            {
            Session.AuthenticationService = new AuthenticationService();
            this.signUpPageProcessor = signUpPageProcessor;
                this.SignUpCommand = new RelayCommand(SignUp);
            }

            public ICommand SignUpCommand { protected set; get; }

            public string FirstName
            {
                get
                {
                    return firstName;
                }

                set
                {
                    firstName = value;
                    OnPropertyChanged("FirstName");
                }
            }

            public string LastName
            {
                get
                {
                    return lastName;
                }

                set
                {
                    lastName = value;
                    OnPropertyChanged("LastName");
                }
            }

            public string UserName
            {
                get
                {
                    return userName;
                }

                set
                {
                    userName = value;
                    OnPropertyChanged("UserName");
                }
            }

            public string Password
            {
                get
                {
                    return password;
                }

                set
                {
                    password = value;
                    OnPropertyChanged("Password");
                }
            }

            public string Email
            {
                get
                {
                    return email;
                }

                set
                {
                    email = value;
                    OnPropertyChanged("Email");
                }
            }

            private void SignUp()
            {
                var user = new User()
                {
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    UserName = this.UserName,
                    EMail = this.Email,
                    Password = this.Password
                };


                var signUpSucceeded = AreDetailsValid(user);

                if (signUpSucceeded)
                {

                    var result = Session.AuthenticationService.CreateUser(user);
                    this.signUpPageProcessor.MoveToLoginPage();
                }
                else
                {
                    ErrorMessage = "Sign up failed";
                }
            }

            bool AreDetailsValid(User user)
            {
                return (!string.IsNullOrWhiteSpace(user.UserName) &&
                    !string.IsNullOrWhiteSpace(user.Password) &&
                    !string.IsNullOrWhiteSpace(user.UserName) &&
                    !string.IsNullOrWhiteSpace(user.EMail));
            }

        }
}
