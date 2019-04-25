using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace ProjectDB
{
    class FileHandler
    {
        string conString = "Data Source=DESKTOP-UUVV8KJ\\SQLEXPRESS;Initial Catalog = AirportDB; Integrated Security = True";

        SqlConnection conn = null;
        SqlDataReader read = null;
        SqlCommand cmd = null;

        public List<Flight> Read()
        {
            List<Flight> allFlight = new List<Flight>();

            try
            {                
                conn = new SqlConnection(conString);
                conn.Open();

                string select = "SELECT * FROM Flight";

                cmd = new SqlCommand(select, conn);

                read = cmd.ExecuteReader();

                while (read.Read())
                {
                    allFlight.Add(new Flight(int.Parse(read[0].ToString()), read[1].ToString(), read[2].ToString(), read[3].ToString(), read[4].ToString(), read[5].ToString(), read[6].ToString(), int.Parse(read[7].ToString())));
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("List<Flight> Read()" + e.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return allFlight;
        }

        public void Add(Flight newFlight)
        {
            try
            {               
                conn = new SqlConnection(conString);
                conn.Open();

                string fromAirport = newFlight.FromAirport;
                string toAirport = newFlight.ToAirport;
                string depTime = newFlight.DeparTime;
                string arrTime = newFlight.ArriveTime;
                string service = newFlight.Service;
                string aircraft = newFlight.AircraftType;
                int routeNo = newFlight.RouteNo;

                string insert = @"INSERT INTO Flight (FlightNo, FromAirport, ToAirport, DepartureTime, ArriveTime, Service, AircraftType, RouteNo)
                                VALUES ('"+ fromAirport + "', '" + toAirport + "', '" + depTime + "', '" + arrTime + "', '" + service + "', '" + aircraft + "', '" + routeNo + "',)";

                cmd = new SqlCommand(insert, conn);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Flight inserted!" + e.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void Clear(int ID)
        {
            int result = 0;

            try
            {
                conn = new SqlConnection(conString);
                conn.Open();

                string delete = @"DELETE FROM Flight
                                WHERE FlightNo = '"+ ID +"'";

                cmd = new SqlCommand(delete, conn);

                result = cmd.ExecuteNonQuery();

                if (result == 1)
                {
                    System.Windows.Forms.MessageBox.Show("Flight deleted");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Flight does not delete");
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Flight deleted!" + e.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
