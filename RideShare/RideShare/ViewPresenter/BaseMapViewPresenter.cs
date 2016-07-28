using RideShare.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.ViewPresenter
{
    public class BaseMapViewPresenter
    {
        protected IMapPageProcessor mapPageProcessor;
        public BaseMapViewPresenter(IMapPageProcessor mapPageProcessor)
        {
            this.mapPageProcessor = mapPageProcessor;
        }

        private void Init()
        {
            mapPageProcessor.OnMapInfoWindowClicked = OnMapInfoWindowClicked;
            mapPageProcessor.OnPopupCanceled = OnPopupCanceled;
            mapPageProcessor.OnPopupConfirmed = OnPopupConfirmed;
            mapPageProcessor.OnNewCoordinatesRecived = OnNewCoordinatesRecived;
        }

        protected virtual void OnMapInfoWindowClicked(CustomPin customPin) { }
        protected virtual void OnPopupCanceled() { }
        protected virtual void OnPopupConfirmed() { }
        protected virtual void OnNewCoordinatesRecived() { }
        protected virtual void LoadPinData() { }
        protected virtual void RefreshPins()
        {
            mapPageProcessor.RefreshPins(true, LoadPinData);
        }
    }
}
