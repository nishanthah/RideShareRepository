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
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace RideShare.Droid
{
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {

        GoogleMap map;
        List<Position> routeCoordinates;
        List<CustomPin> customPins;
        bool isDrawn;

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(Android.Graphics.Color.Blue);

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
                //map.InfoWindowClick -= OnInfoWindowClick;
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                routeCoordinates = formsMap.RouteCoordinates;
                customPins = formsMap.CustomPins;
                ((Android.Gms.Maps.MapView)Control).GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("VisibleRegion") && !isDrawn)
            {
                map.Clear();

                foreach (var pin in customPins)
                {
                    var marker = new MarkerOptions();
                    marker.SetPosition(new LatLng(pin.Pin.Position.Latitude, pin.Pin.Position.Longitude));
                    marker.SetTitle(pin.UserType.ToString());
                    marker.SetSnippet(pin.Pin.Address);

                    if(pin.UserType == global::Common.Models.UserType.Driver)
                    {
                        marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.car));
                    }

                    else if (pin.UserType == global::Common.Models.UserType.Rider)
                    {
                        marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.person));
                    }

                    map.AddMarker(marker);
                }
                isDrawn = true;
            }
        }

    }

}