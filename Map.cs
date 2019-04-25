using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Configuration;
using System.Data.SqlClient;

namespace ProjectDB
{
    public partial class Map : Form
    {
        public Map()
        {
            InitializeComponent();
        }

        string conString = "Data Source=DESKTOP-UUVV8KJ\\SQLEXPRESS;Initial Catalog = AirportDB; Integrated Security = True";

        FileHandler fileHandler = new FileHandler();
        BindingSource bind = new BindingSource();
        List<Flight> flight = new List<Flight>();

        GMarkerGoogle marker;
        GMapOverlay mapOverlay;
        DataTable dt;
        GMapOverlay Poligono = new GMapOverlay("Polygon");
        GMapOverlay Routa = new GMapOverlay("Route");
        GDirections Routsdir;
        List<PointLatLng> points = new List<PointLatLng>();
        List<Tuple<double, double>> datry = new List<Tuple<double, double>>();

        int RowSelected = 0;
        double inilong = -86.9080556;
        double inilat = 40.4258333;

        private void Map_Load(object sender, EventArgs e)
        {
            flight = fileHandler.Read();
            bind.DataSource = flight;
            dataGridView1.DataSource = bind;

            dt = new DataTable();
            dt.Columns.Add(new DataColumn("FlightNo", typeof(string)));
            dt.Columns.Add(new DataColumn("FromAirport", typeof(string)));
            dt.Columns.Add(new DataColumn("ToAirport", typeof(string)));
            dt.Columns.Add(new DataColumn("DepartureTime", typeof(string)));
            dt.Columns.Add(new DataColumn("ArriveTime", typeof(string)));
            dt.Columns.Add(new DataColumn("Service", typeof(string)));
            dt.Columns.Add(new DataColumn("AircraftType", typeof(string)));
            dt.Columns.Add(new DataColumn("RouteNo", typeof(string)));

            dataGridView1.DataSource = dt;

            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(inilat, inilong);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 13;
            gMapControl1.AutoScroll = true;

            mapOverlay = new GMapOverlay("marker");
            marker = new GMarkerGoogle(new PointLatLng(inilat, inilong), GMarkerGoogleType.green);
            mapOverlay.Markers.Add(marker);
            marker.ToolTipMode = MarkerTooltipMode.Always;
            marker.ToolTipText = $"Latitude:{inilat}, longitude: {inilong}";
            gMapControl1.Overlays.Add(mapOverlay);
        }

        private void Registry(object sender, DataGridViewCellMouseEventArgs e)
        {
            RowSelected = e.RowIndex;
            marker.Position = new PointLatLng(datry[RowSelected].Item1, datry[RowSelected].Item2);
            marker.ToolTipText = $" Latitude:{marker.Position.Lat}, Longitude:{marker.Position.Lng}";
            gMapControl1.Position = marker.Position;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.RemoveAt(RowSelected);
            points.RemoveAt(RowSelected);
            datry.RemoveAt(RowSelected);


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            GeoCoderStatusCode status;
            Placemark pos = (Placemark)GMapProviders.GoogleMap.GetPlacemark(new PointLatLng(marker.Position.Lat, marker.Position.Lng), out status);
            datry.Add(Tuple.Create(marker.Position.Lat, marker.Position.Lng));
            points.Add(new PointLatLng(datry[datry.Count - 1].Item1
                , datry[datry.Count - 1].Item2));
        }

        private void btnDirection_Click(object sender, EventArgs e)
        {
            if (gMapControl1.Overlays.Contains(Routa))
            {
                Routa.Routes.Clear();
                gMapControl1.Overlays.Remove(Routa);
                txtDistance.Text = string.Empty;
            }
            else
            {
                var routefromap = GMapProviders.GoogleMap.GetDirections(out Routsdir, points[0], points, points[points.Count - 1], false, false, false, false, true);
                GMapRoute tablemaproute = new GMapRoute(Routsdir.Route, "Route");
                Routa.Routes.Add(tablemaproute);
                gMapControl1.Overlays.Add(Routa);
                gMapControl1.Zoom = gMapControl1.Zoom + 1;
                gMapControl1.Zoom = gMapControl1.Zoom - 1;
                txtDistance.Text = tablemaproute.Distance.ToString();
            }
        }

        private void btnPolygon_Click(object sender, EventArgs e)
        {
            if (gMapControl1.Overlays.Contains(Poligono))
            {
                Poligono.Polygons.Clear();
                gMapControl1.Overlays.Remove(Poligono);
            }
            else
            {
                GMapPolygon polygonpoints = new GMapPolygon(points, "Polygon");
                Poligono.Polygons.Add(polygonpoints);
                gMapControl1.Overlays.Add(Poligono);
                gMapControl1.Zoom = gMapControl1.Zoom + 1;
                gMapControl1.Zoom = gMapControl1.Zoom - 1;
            }
        }

        private void btnSatellite_Click(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GMapProviders.GoogleSatelliteMap;
        }

        private void btnOriginal_Click(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
        }

        private void btnRelief_Click(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GMapProviders.GoogleTerrainMap;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GeoCoderStatusCode status;
            string address = cboSearch.Text;
            PointLatLng? point = GMapProviders.GoogleMap.GetPoint(address, out status);
            if (status == GeoCoderStatusCode.OK & point != null)
            {
                double nlat = point.Value.Lat;
                double nlng = point.Value.Lng;

                marker.Position = new PointLatLng(nlat, nlng);
                gMapControl1.Position = new PointLatLng(nlat, nlng);
            }
            else
            {
                string message = "Your address may have an error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, string.Empty, buttons);

            }
        }

        private void cboSearch_SelectedIndexChanged(object sender, EventArgs e)
        {

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand())
            {
                connection.Open();
                command.Parameters.AddWithValue("@FlightNo", cboSearch.Text);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    double lat = Convert.ToDouble(reader["Latitude"]);
                    double lon = Convert.ToDouble(reader["Longitude"]);
       

                    marker.Position = new PointLatLng(lat, lon);
                    gMapControl1.Position = new PointLatLng(lat, lon);
                }
            }
        }

        public void populate()
        {

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = fileHandler.Read();
            dataGridView1.DataMember = "flight";
        }

        private void txtDistance_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
