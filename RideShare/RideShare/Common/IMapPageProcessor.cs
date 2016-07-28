using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace RideShare.Common
{
    public interface IMapPageProcessor
    {
        Action<CustomPin> OnMapInfoWindowClicked { get; set; }
        Action OnPopupConfirmed { get; set; }
        Action OnPopupCanceled { get; set; }
        Action OnNewCoordinatesRecived { get; set; }
        RouteData RouteResult { get; set; }
        List<CustomPin> MapPins { get; set; }
        CustomPin SelectedPin { get; set; }
        LocationSearchResult SelectedDestination { get; }

        void AddPin(MapPin mapPin);
        void RefreshPins(bool canMoveToLocation, Action loadPinsAction);
        void ShowRoute(Func<RouteData> getDataFunction);
        void ShowPopupBox(string title);
        void HidePopupBox();

    }
}
