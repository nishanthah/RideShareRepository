using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleApiClient.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Collections.ObjectModel;
namespace RideShare
{  
    public partial class HistoryView : ContentPage
    {   
        public HistoryView()
        {
            InitializeComponent();
            Content.BindingContext = new RideShare.ViewModels.HistoryViewModel(); 
         
        }
    }
}
