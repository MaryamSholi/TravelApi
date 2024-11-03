using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Core.Entities;
using Travel.Core.IRepositories;
using Travel.Infrastructure.Data;

namespace Travel.Infrastructure.Repositories
{
    public class BookingRepository :GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext dbContext;

        public BookingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            var booking = await dbContext.Bookings
            .Include(b => b.LocalUser)
            .Include(b => b.Flight)
            .Include(b => b.Hotel)
            .Where(b => b.UserId == userId)
            .ToListAsync();

            return booking;
        }

    }
}
