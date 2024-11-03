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
    public class DestinationRepository : GenericRepository<Destination>, IDestinationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DestinationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Destination>> GetWithHotelsAsync()
        {
            var dest = await dbContext.Destinations
                .Include(d => d.Hotels)
                .ToListAsync();
            return dest;
        }
    }

}
