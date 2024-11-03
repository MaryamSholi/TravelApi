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
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly ApplicationDbContext dbContext;

        public FlightRepository(ApplicationDbContext dbContext ) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Flight>> GetFlightsByOriginAsync(string origin)
        {
            var flight = await dbContext.Flights.Where(f => f.Origin == origin).ToListAsync();
            return flight;
        }
    }
}
