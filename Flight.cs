using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDB
{
    class Flight
    {
        private int flightNo;
        private string fromAirport;
        private string toAirport;
        private string deparTime;
        private string arriveTime;
        private string service;
        private string aircraftType;
        private int routeNo;

        public Flight(int flightNo, string fromAirport, string toAirport, string deparTime, string arriveTime, string service, string aircraftType, int routeNo)
        {
            this.flightNo = flightNo;
            this.fromAirport = fromAirport;
            this.toAirport = toAirport;
            this.deparTime = deparTime;
            this.arriveTime = arriveTime;
            this.service = service;
            this.aircraftType = aircraftType;
            this.routeNo = routeNo;
        }

        public int FlightNo { get => flightNo; set => flightNo = value; }
        public string FromAirport { get => fromAirport; set => fromAirport = value; }
        public string ToAirport { get => toAirport; set => toAirport = value; }
        public string DeparTime { get => deparTime; set => deparTime = value; }
        public string ArriveTime { get => arriveTime; set => arriveTime = value; }
        public string Service { get => service; set => service = value; }
        public string AircraftType { get => aircraftType; set => aircraftType = value; }
        public int RouteNo { get => routeNo; set => routeNo = value; }
    }
}
