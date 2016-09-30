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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RideShare.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage // : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Xamarin.FormsMaps.Init("Ak9VaN3Wh00PbaDOVzRdsI3P_xEBSwa0sUFw8qeaQHbawMZ0B1kjjmJal0j8kP2i");
            this.LoadApplication(new RideShare.App(false));
        }
    }
}
