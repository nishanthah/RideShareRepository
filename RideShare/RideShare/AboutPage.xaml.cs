using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace RideShare
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            Title = "About Page";
            Content = new StackLayout
            {
                Children = {
                    new Label {
                        Text = "About Page data goes here",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    }
                }
            };
        }
    }
}
