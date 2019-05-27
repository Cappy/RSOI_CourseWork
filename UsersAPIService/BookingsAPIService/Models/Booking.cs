using System;
using System.Collections.Generic;

namespace BookingsAPIService.Models
{
    public partial class Booking
    {
        public Guid Bookingid { get; set; }
        public Guid Adid { get; set; }
        public Guid Userid { get; set; }
        public int? BookedPrice { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
