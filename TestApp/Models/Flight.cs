using Microsoft.AspNetCore.Mvc;

namespace TestApp.Models
{
    public class Flight
    {
        public string FlightId { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public string Success { get; set; }
    }
}
