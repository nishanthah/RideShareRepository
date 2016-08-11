using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using RideShare;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Xamarin.Forms.Platform.UWP;
using RideShare.UWP;
using Xamarin.Forms.Maps.UWP;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace RideShare.UWP
{
    public class CustomMapRenderer : MapRenderer
    {

        MapControl nativeMap;
        List<CustomPin> customPins;
        XamarinMapOverlay mapOverlay;
        bool xamarinOverlayShown = false;
        List<Position> routeCoordinates;
        Action<CustomPin> onInfoWindowClicked;
        bool isDrawn;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                nativeMap.MapElementClick -= OnMapElementClick;
                nativeMap.Children.Clear();
                mapOverlay = null;
                nativeMap = null;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                nativeMap = Control as MapControl;

                routeCoordinates = formsMap.RouteCoordinates;
                customPins = formsMap.CustomPins;
                onInfoWindowClicked = formsMap.OnInfoWindowClicked;

                nativeMap.Children.Clear();
                nativeMap.MapElementClick += OnMapElementClick;
            }
        }

        private void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var mapIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            if (mapIcon != null)
            {
                if (!xamarinOverlayShown)
                {
                    var customPin = GetCustomPin(mapIcon);
                    if (customPin == null)
                    {
                        throw new Exception("Custom pin not found");
                    }

                    if (mapOverlay == null)
                    {
                        mapOverlay = new XamarinMapOverlay(customPin, onInfoWindowClicked);
                    }

                    var snPosition = new BasicGeoposition { Latitude = customPin.Pin.Position.Latitude, Longitude = customPin.Pin.Position.Longitude };
                    var snPoint = new Geopoint(snPosition);

                    nativeMap.Children.Add(mapOverlay);
                    MapControl.SetLocation(mapOverlay, snPoint);
                    MapControl.SetNormalizedAnchorPoint(mapOverlay, new Windows.Foundation.Point(0.5, 1.0));
                    xamarinOverlayShown = true;


                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
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
            nativeMap = Control as MapControl;

            var coordinates = new List<BasicGeoposition>();
            foreach (var position in ((CustomMap)Element).RouteCoordinates)
            {
                coordinates.Add(new BasicGeoposition() { Latitude = position.Latitude, Longitude = position.Longitude });
            }

            var polyline = new MapPolyline();
            polyline.StrokeColor = Windows.UI.Color.FromArgb(128, 255, 0, 0);
            polyline.StrokeThickness = 5;
            polyline.Path = new Geopath(coordinates);
            nativeMap.MapElements.Add(polyline);

        }

        private void ChangePinRender()
        {
            customPins = ((CustomMap)Element).CustomPins;
            foreach (var pin in customPins)
            {
                var snPosition = new BasicGeoposition { Latitude = pin.Pin.Position.Latitude, Longitude = pin.Pin.Position.Longitude };
                var snPoint = new Geopoint(snPosition);

                var mapIcon = new MapIcon();

                if (pin.UserType == global::Common.Models.UserType.Driver)
                {
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Icons/pin.png"));
                }

                else if (pin.UserType == global::Common.Models.UserType.Rider)
                {
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Icons/pin.png"));
                }
                
                mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                mapIcon.Location = snPoint;
                mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);
                nativeMap.MapElements.Add(mapIcon);
            }
            
        }

        private CustomPin GetCustomPin(MapIcon marker)
        {
            return customPins.ToList().Find(x => x.Id.ToString() == marker.Title.ToString());
        }

      
    }

}