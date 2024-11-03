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
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            DestinationRepository = new DestinationRepository(dbContext);
            HotelRepository = new HotelRepository(dbContext);
            FlightRepository = new FlightRepository(dbContext);
            BookingRepository = new BookingRepository(dbContext);
        }
        public IDestinationRepository DestinationRepository { get; set; }
        public IHotelRepository HotelRepository { get; set; }
        public IFlightRepository FlightRepository { get; set; }
        public IBookingRepository BookingRepository { get; set; }

        public async Task<int> Save() => await dbContext.SaveChangesAsync();

    }
}
