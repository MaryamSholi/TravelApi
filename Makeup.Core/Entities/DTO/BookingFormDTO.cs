using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.Entities.DTO
{
    public class BookingFormDTO
    {
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } 
        public string UserId { get; set; } 
        public int FlightId { get; set; }
        public int HotelId { get; set; }
    }
}
