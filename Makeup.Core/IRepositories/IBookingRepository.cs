using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Core.Entities;

namespace Travel.Core.IRepositories
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        public Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
    }
}
