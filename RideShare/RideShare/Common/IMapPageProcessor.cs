using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace RideShare.Common
{
    public class InfoWindowContent
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public interface IMapPageProcessor
    {
        Action<CustomPin> OnMapInfoWindowClicked { get; set; }
        Action OnNewCoordinatesRecived { get; set; }
        Action OnNewStatusChanged { get; set; }
        RouteData RouteResult { get; set; }
        List<CustomPin> MapPins { get; set; }
        CustomPin SelectedPin { get; set; }
        LocationSearchResult SelectedDestination { get; }

        void AddPin(MapPin mapPin);
        void RefreshPins(bool canMoveToLocation, Func<List<CustomPin>> loadPinsFunction);
        void RefreshRoute(bool canMoveToLocation, Func<RouteData> loadRouteFunction);
        void ShowDoubleButtonPopup(string title, string buttonConfirmText, string buttonCancelText,Action confirmAction,Action cancelAction);
        void HideDoubleButtonPopupBox();
        void ShowInfoWindowPopupBox(InfoWindowContent infoWindowContent);
        void HideInfoWindowPopupBox();
        void NavigateToRiderView();
        void SetDestination(LocationSearchResult destination);
        void LoadCurrentStatus(string status);

    }
}
