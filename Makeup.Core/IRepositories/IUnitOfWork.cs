using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.IRepositories
{
    public interface IUnitOfWork<T> where T : class
    {
        public IDestinationRepository DestinationRepository { get; set; }
        public IHotelRepository HotelRepository { get; set; }
        public IFlightRepository FlightRepository { get; set; }
        public IBookingRepository BookingRepository { get; set; }
        public Task<int> Save();
    }
}
