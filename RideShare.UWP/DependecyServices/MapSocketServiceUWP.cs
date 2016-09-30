using Quobject.SocketIoClientDotNet.Client;
using RideShare.SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



[assembly: Xamarin.Forms.Dependency(typeof(RideShare.UWP.DependecyServices.MapSocketServiceUWP))]
namespace RideShare.UWP.DependecyServices
{
    public class MapSocketServiceUWP : IMapSocketService
    {
        public event EventHandler MapCoordinateChanged;

        public MapSocketServiceUWP()
        {
            var socket = IO.Socket("http://172.26.204.15:8079");
            socket.On("coordinate_changed", MapUpdated);
        }

        private void MapUpdated(object obj)
        {
            MapCoordinateChanged(obj, null);
        }
    }


}
