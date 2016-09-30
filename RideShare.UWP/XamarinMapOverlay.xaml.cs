using System;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RideShare.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class XamarinMapOverlay : UserControl
    {
        CustomPin customPin;

        public XamarinMapOverlay(CustomPin pin)
        {
            this.InitializeComponent();
            customPin = pin;
            SetupData();
        }

        void SetupData()
        {
            Label.Text = customPin.Pin.Label;
            Address.Text = customPin.Pin.Address;
        }

        private async void OnInfoButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(customPin.Url));
        }
    }
}



