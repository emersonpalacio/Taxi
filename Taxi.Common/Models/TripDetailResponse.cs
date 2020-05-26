using System;
using System.Collections.Generic;
using System.Text;

namespace Taxi.Common.Models
{
    public class TripDetailResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateLocal => Date.ToLocalTime();
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
