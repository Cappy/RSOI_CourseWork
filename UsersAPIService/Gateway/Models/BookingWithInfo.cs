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
    }
}
