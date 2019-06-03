using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Models
{
    public class BookingWithInfo
    {
        public Guid Bookingid { get; set; }
        public Users user { get; set; }
        public Ads ad { get; set; }
        public int? bookedPrice { get; set; }
        public DateTime? arrivalDate { get; set; }
        public DateTime? departureDate { get; set; }
        public DateTime? createdAt { get; set; }
    }
}
