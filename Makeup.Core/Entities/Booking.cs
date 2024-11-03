using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.Entities
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey(nameof(LocalUser))]
        public string UserId { get; set; }
        [ForeignKey(nameof(Flight))]
        public int FlightId { get; set; }
        [ForeignKey(nameof(Hotel))]
        public int HotelId { get; set; } 
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public virtual LocalUser? LocalUser { get; set; }
        public virtual Flight? Flight { get; set; }
        public virtual Hotel? Hotel { get; set; }



    }
}
