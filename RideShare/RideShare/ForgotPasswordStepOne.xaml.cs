﻿using RideShare.Common;
using RideShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare
{
    public partial class ForgotPasswordStepOne : ContentPage, IForgotPasswordPageProcessor
    {
        public ForgotPasswordStepOne()
        {
            InitializeComponent();
            Content.BindingContext = new ForgotPasswordViewModel(this);
        }

        public void MoveToLoginPage()
        {
            App.Current.MainPage = new NavigationPage(new LogInPage());
        }

        public void MoveToNextPage()
        {
            App.Current.MainPage = new MainPage(new ForgotPasswordStepOne());
        }

        public void MoveToPreviousPage()
        {
            throw new NotImplementedException();
        }
    }
}
