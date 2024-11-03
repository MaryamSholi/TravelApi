using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.Entities.DTO
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public LocalUserDTO User { get; set; }
        public FlightDTO Flight { get; set; }
        public HotelDTO Hotel { get; set; }
    }
}
