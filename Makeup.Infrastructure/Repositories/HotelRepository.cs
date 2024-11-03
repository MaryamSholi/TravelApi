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
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        private readonly ApplicationDbContext dbContext;

        public HotelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Hotel>> GetHotelsByDestinationIdAsync(int destinationId)
        {
            var hotel = await dbContext.Hotels.Where(h => h.DestinationId == destinationId).ToListAsync();
            return hotel;
        }
    }
}
