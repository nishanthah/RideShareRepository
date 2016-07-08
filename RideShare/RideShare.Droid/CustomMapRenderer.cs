using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

using Android.Widget;
using Xamarin.Forms;
using RideShare.Droid;

using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Maps.Android;
using Android.Gms.Maps;
using Xamarin.Forms.Maps;
using Android.Gms.Maps.Model;
using RideShare;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace RideShare.Droid
{
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {

        GoogleMap map;
        List<Position> routeCoordinates;

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(0x66FF0000);

            foreach (var position in routeCoordinates)
            {
                polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
            }

            map.AddPolyline(polylineOptions);
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                routeCoordinates = formsMap.RouteCoordinates;
                
                ((Android.Gms.Maps.MapView)Control).GetMapAsync(this);
            }
        }

    }
}