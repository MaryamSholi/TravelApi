using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Core.Entities;

namespace Travel.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<LocalUser>
    {
        public DbSet<LocalUser> Users { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //combosit primary key
            modelBuilder.Entity<Booking>().HasKey(x => new { x.Id, x.UserId, x.FlightId, x.HotelId });

           /* modelBuilder.Entity<LocalUser>().HasData(
            new LocalUser { Username = "john_doe", Email = "john@example.com", Password = "password123", Role = "User" },
            new LocalUser { Username = "jane_admin", Email = "jane@example.com", Password = "adminpass", Role = "Admin" }
        );

            modelBuilder.Entity<Destination>().HasData(
                new Destination { Id = 1, Name = "Paris", Country = "France", Description = "City of Lights", Price = 1000m },
                new Destination { Id = 2, Name = "New York", Country = "USA", Description = "The Big Apple", Price = 1200m }
            );

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { Id = 1, Name = "Parisian Hotel", DestinationId = 1, Address = "123 Paris St", PricePerNight = 150m },
                new Hotel { Id = 2, Name = "NYC Grand", DestinationId = 2, Address = "456 NYC Ave", PricePerNight = 200m }
            );

            modelBuilder.Entity<Flight>().HasData(
                new Flight { Id = 1, Airline = "Air France", Origin = "London", Destination = "Paris", TakeOffTime = DateTime.Now.AddHours(2), ArrivalTime = DateTime.Now.AddHours(5), Price = 300m },
                new Flight { Id = 2, Airline = "Delta Airlines", Origin = "London", Destination = "New York", TakeOffTime = DateTime.Now.AddHours(3), ArrivalTime = DateTime.Now.AddHours(9), Price = 600m }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking { Id = 1, UserId = "1", FlightId = 1, HotelId = 1, BookingDate = DateTime.Now, Status = "Confirmed" },
                new Booking { Id = 2, UserId = "2", FlightId = 2, HotelId = 2, BookingDate = DateTime.Now, Status = "Pending" }
            );*/


            base.OnModelCreating(modelBuilder);

        }
    }
}
