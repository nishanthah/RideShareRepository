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

namespace RideShare.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Xamarin.FormsMaps.Init("IjEnMQF6phKHvfFCM4xi~v44o8YfbJrlwghx_takbCw~Ar9W5w48T74aHZf5T_RTthpLsA72s916agdG9duZSCYSYv8tSFs6qiqlkd6jBqhP");
            LoadApplication(new RideShare.App(false));
        }
    }
}
