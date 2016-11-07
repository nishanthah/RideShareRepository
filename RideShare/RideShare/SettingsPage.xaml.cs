using RideShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RideShare.Common;

namespace RideShare
{
    public partial class SettingsPage : ContentPage, IAccountDeleteResult, ICommonPageProcessor
    {
        public SettingsPage()
        {
            InitializeComponent();
            Content.BindingContext = new SettingsViewModel(this, this);
        }
            

        public void ShowDoubleButtonPopup(string title, string message, Action successAction, Action failedAction)
        {
            DisplayAlert(title, message, "Yes", "No").ContinueWith((task) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (task.Result)
                    {
                        successAction();
                    }
                    else
                    {
                        failedAction();
                    }

                });


            });
        }

        public void MoveToLoginPage()
        {
            App.Current.MainPage = new NavigationPage(new LogInPage());
        }

        public void MoveToMainPage()
        {
            App.Current.MainPage = new MainPage();
        }

        public void InvokeInMainThread(Action action)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                action();
            });

        }
    }
}
