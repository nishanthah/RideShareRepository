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
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string MobileNo { get; set; }
        public string Image { get; set; }
        public string UserName { get; set; }
        public UserType UserType { get; set; }
    }

    public class CustomMap : Map
    {
        public static readonly BindableProperty RouteCoordinatesProperty = BindableProperty.Create(nameof(RouteCoordinates), typeof(List<Position>), typeof(CustomMap), new List<Position>(), BindingMode.TwoWay);
        public static readonly BindableProperty CustomPinsProperty = BindableProperty.Create(nameof(CustomPins), typeof(List<CustomPin>), typeof(CustomMap), new List<CustomPin>(), BindingMode.TwoWay);

        

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
