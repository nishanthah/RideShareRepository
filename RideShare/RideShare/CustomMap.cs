using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace RideShare
{
    public class CustomPin
    {
        public Pin Pin { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string MobileNo { get; set; }
        public string Image { get; set; }
        public string UserName { get; set; }
        public UserType UserType { get; set; }
    }

    public class CustomMap : Map
    {
        public static readonly BindableProperty RouteCoordinatesProperty = BindableProperty.Create("RouteCoordinates", typeof(List<Position>), typeof(CustomMap), new List<Position>(), BindingMode.TwoWay);
        public static readonly BindableProperty CustomPinsProperty = BindableProperty.Create("CustomPins", typeof(List<CustomPin>), typeof(CustomMap), new List<CustomPin>(), BindingMode.TwoWay);

        public static readonly BindableProperty BaseLatitudeProperty = BindableProperty.Create("BaseLatitude", typeof(double), typeof(CustomMap), 0.0D, propertyChanged: OnLatitudeOrLongitudeChanged);
        public static readonly BindableProperty BaseLongitudeProperty = BindableProperty.Create("BaseLongitude", typeof(double), typeof(CustomMap), 0.0D, propertyChanged: OnLatitudeOrLongitudeChanged);
        //public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create("IsShowingUser", typeof(bool), typeof(CustomMap), false, BindingMode.TwoWay);

        public double BaseLatitude
        {
            get { return (double)GetValue(BaseLatitudeProperty); }
            set { SetValue(BaseLatitudeProperty, value); }
        }

        public double BaseLongitude
        {
            get { return (double)GetValue(BaseLongitudeProperty); }
            set { SetValue(BaseLongitudeProperty, value); }
        }

        private bool hasScrollEnabled;
 
        public bool HasScrollEnabled
        {
           get { return hasScrollEnabled; }
           set { hasScrollEnabled = value; }
        }
        private bool hasZoomEnabled;
        public bool HasZoomEnabled
         {
           get { return hasZoomEnabled; }
           set { hasZoomEnabled = value; }
         }
 
        private bool isShowingUser;
        public bool IsShowingUser
        {
            get { return isShowingUser; }
            set { isShowingUser = value; }
        }

        //public bool IsShowingUser
        //{
        //    get { return (bool)GetValue(IsShowingUserProperty); }
        //    set { SetValue(IsShowingUserProperty, value); }
        //}

        //public static readonly BindableProperty RCoordinatesProperty = BindableProperty.Create("RCoordinates", typeof(List<Position>), typeof(CustomMap), null, propertyChanged: OnRCoordinatesChanged);

        static void OnLatitudeOrLongitudeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as CustomMap;

            if (map != null)
            {
                MoveMapToRegion(map);
            }
        }

        static void MoveMapToRegion(CustomMap map)
        {
            if (map.BaseLatitude > 0 && map.BaseLongitude > 0)
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(map.BaseLatitude, map.BaseLongitude), Distance.FromKilometers(10)));
        }


        public List<CustomPin> CustomPins
        {
            get { return (List<CustomPin>)GetValue(CustomPinsProperty); }
            set { SetValue(CustomPinsProperty, value); }
        }

        public List<Position> RouteCoordinates
        {
            get { return (List<Position>)GetValue(RouteCoordinatesProperty); }
            set { SetValue(RouteCoordinatesProperty, value); }
        }

        public Action<CustomPin> OnInfoWindowClicked;

        public CustomMap()
        {
            RouteCoordinates = new List<Position>();
            CustomPins = new List<CustomPin>();
        }
    }
}
