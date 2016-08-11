using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RideShare.UWP
{
    public sealed partial class XamarinMapOverlay : UserControl
    {
       
        CustomPin customPin;
        Action<CustomPin> onInfoWindowClicked;
        public XamarinMapOverlay(CustomPin pin,Action<CustomPin> onInfoWindowClicked)
        {
            this.InitializeComponent();
            customPin = pin;
            this.onInfoWindowClicked = onInfoWindowClicked;
            SetupData();
        }

        void SetupData()
        {
            //Label.Text = customPin.Pin.Label;
            //Address.Text = customPin.Pin.Address;
        }

        private async void OnInfoButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            onInfoWindowClicked(customPin);
        }

    }
}
