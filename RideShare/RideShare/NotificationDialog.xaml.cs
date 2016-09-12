using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RideShare
{
    public partial class NotificationDialog : ContentPage
    {
        public NotificationDialog(string description,List<Button> buttons)
        {
            InitializeComponent();
            this.txtDescription.Text = description;
            foreach(var button in buttons)
            {
                buttonContainer.Children.Add(button);
            }
        }

    }
}
