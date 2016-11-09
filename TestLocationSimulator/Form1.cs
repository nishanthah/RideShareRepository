using Authentication;
using Authentication.Common;
using DriverLocator;
using DriverLocator.Models;
using GoogleApiClient.Maps;
using GoogleApiClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestLocationSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            authenticationService = new AuthenticationService();
            driverLoacatorService = new DriverLocatorService(authenticationService);
        }

        IAuthenticationService authenticationService;
        DriverLocatorService driverLoacatorService;

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        Thread thread;
        private void button1_Click(object sender, EventArgs e)
        {
            Coordinate source = new Coordinate() { Longitude = double.Parse(textBox1.Text), Latitude = double.Parse(textBox2.Text) };
            Coordinate destination = new Coordinate() { Longitude = double.Parse(textBox4.Text), Latitude = double.Parse(textBox3.Text) };
            var directions = GetDirections(source, destination);
            textBox5.Text = directions.Routes.FirstOrDefault().OverViewPolyLine.Ponits;
            thread = new Thread(new ThreadStart(() => {
                foreach (var coordinates in directions.Routes.FirstOrDefault().OverViewPolyLine.DecodedOverViewPolyLine)
                {
                    UpdateUserLocationRequest req = new UpdateUserLocationRequest();
                    req.Latitude = coordinates.Latitude;
                    req.Longitude = coordinates.Longitude;
                    driverLoacatorService.UpdateUserLocation(textBox8.Text, req);
                    Thread.Sleep(2000);
                }
            }));
            thread.Start();
            
        }

        private GetDirectionsResponse GetDirections(Coordinate sourceCoordinate, Coordinate destinationCoordinate)
        {
            GoogleMapsDirectionsClient googleMapsDirectionsClient = new GoogleMapsDirectionsClient();
            var source = new GoogleApiClient.Models.Coordinate() { Latitude = sourceCoordinate.Latitude, Longitude = sourceCoordinate.Longitude };
            var destination = new GoogleApiClient.Models.Coordinate() { Latitude = destinationCoordinate.Latitude, Longitude = destinationCoordinate.Longitude };
            var directions = googleMapsDirectionsClient.GetDirections(new GoogleApiClient.Models.GetDirectionRequest() { DestinationCoordinate = destination, SourceCoordinate = source });
            return directions;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(authenticationService.Authenticate(textBox6.Text,textBox7.Text).IsSuccess)
            {
                label7.Text = "Success";
            }
            else
            {
                label7.Text = "Failed";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            thread.Abort();
        }
    }
}
