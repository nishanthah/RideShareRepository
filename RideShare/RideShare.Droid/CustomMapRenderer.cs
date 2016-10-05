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
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Media;
using Java.IO;
using Android.Content.Res;
using System.Collections.ObjectModel;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace RideShare.Droid
{
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback, GoogleMap.IInfoWindowAdapter
    {

        GoogleMap map;
        List<Position> routeCoordinates;
        List<CustomPin> customPins;
        Action<CustomPin> onInfoWindowClicked;
        bool isDrawn;
        Marker selectedMarker = null;
        bool isRendered = false;

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            map.InfoWindowClick += OnInfoWindowClick;
            map.MarkerClick += Map_MarkerClick;
            map.MapClick += Map_MapClick;
            map.SetInfoWindowAdapter(this);
            RenderPolyLine();
        }

        private void Map_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            selectedMarker = null;
        }


        private void Map_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            selectedMarker = e.Marker;
            selectedMarker.ShowInfoWindow();
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);


            if (e.OldElement != null)
            {
                map.InfoWindowClick -= OnInfoWindowClick;
                // Unsubscribe
            }

            if (e.NewElement != null)
            {

                var formsMap = (CustomMap)e.NewElement;
                if (formsMap.IsStatic)
                {
                    if(!isRendered)
                    {
                        routeCoordinates = formsMap.RouteCoordinates;
                        customPins = formsMap.CustomPins;
                        onInfoWindowClicked = formsMap.OnInfoWindowClicked;
                        ((Android.Gms.Maps.MapView)Control).GetMapAsync(this);
                        isRendered = true;
                    }
                   
                }
                else
                {
                    routeCoordinates = formsMap.RouteCoordinates;
                    customPins = formsMap.CustomPins;
                    onInfoWindowClicked = formsMap.OnInfoWindowClicked;
                    ((Android.Gms.Maps.MapView)Control).GetMapAsync(this);
                }
               


            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            base.OnElementPropertyChanged(sender, e);

            map = ((Android.Gms.Maps.MapView)Control).Map;


            if (e.PropertyName.Equals("VisibleRegion"))
            {

                if (!isDrawn)
                {
                    ChangePinRender();
                    isDrawn = true;
                }
            }

            else if (e.PropertyName.Equals("CustomPins"))
            {
                ChangePinRender();
            }
            RenderPolyLine();
        }

        private void RenderPolyLine()
        {
            routeCoordinates = ((CustomMap)Element).RouteCoordinates;
            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(Android.Graphics.Color.Blue);

            if (routeCoordinates != null)
            {
                foreach (var position in routeCoordinates)
                {
                    polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
                }

                map.AddPolyline(polylineOptions);
            }

        }

        private void ChangePinRender()
        {
            map.Clear();
            customPins = ((CustomMap)Element).CustomPins;
            foreach (var pin in customPins)
            {
                var marker = new MarkerOptions();
                marker.SetPosition(new LatLng(pin.Pin.Position.Latitude, pin.Pin.Position.Longitude));
                marker.SetTitle(pin.Id.ToString());
                marker.SetSnippet(pin.Pin.Address);

                if (pin.UserType == global::Common.Models.UserType.Driver)
                {
                    marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.car));
                }

                else if (pin.UserType == global::Common.Models.UserType.Rider)
                {
                    marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.person));
                }

                var addedMarker = map.AddMarker(marker);

                if (selectedMarker != null && selectedMarker.Title == addedMarker.Title)
                {
                    addedMarker.ShowInfoWindow();
                }

            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (changed)
            {
                isDrawn = false;
            }
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            onInfoWindowClicked(GetCustomPin(e.Marker));
        }

        private CustomPin GetCustomPin(Marker marker)
        {
            return customPins.ToList().Find(x => x.Id.ToString() == marker.Title.ToString());
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;
                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);

                var infoImage = view.FindViewById<ImageView>(Resource.Id.markerInfoImage);
                var infoTitle = view.FindViewById<TextView>(Resource.Id.markerInfoTitle);
                var infoSummary = view.FindViewById<TextView>(Resource.Id.markerInfoSummary);



                System.IO.Stream ims = Context.Assets.Open(customPin.Image);

                // load image as Drawable
                Drawable d = Drawable.CreateFromStream(ims, null);

                // set image to ImageView
                infoImage.SetImageDrawable(d);


                //File file = new File(customPin.Image);
                //var image  = Android.Net.Uri.FromFile(file);
                //var resource=ResourceManager.GetDrawableByName("driverLogActive_icon.png");
                //infoImage.SetImageResource(resource);
                //infoImag = customPin.Title;

                infoTitle.Text = customPin.Title;

                infoSummary.Text = customPin.MobileNo;

                return view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }
    }

}