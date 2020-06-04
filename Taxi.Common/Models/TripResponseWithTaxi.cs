using System;
using System.Collections.Generic;
using System.Text;

namespace Taxi.Common.Models
{
    public class TripResponseWithTaxi:TripResponse
    {
        public TaxiResponse Taxi { get; set; }
    }
}
