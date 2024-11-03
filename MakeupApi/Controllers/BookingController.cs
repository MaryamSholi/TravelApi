using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Travel.Core.Entities.DTO;
using Travel.Core.Entities;
using Travel.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using Travel.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Travel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookingController : ControllerBase
    {
        private readonly IUnitOfWork<Booking> unitOfWork;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;
        public ApiResponse response;

        public BookingController(IUnitOfWork<Booking> unitOfWork, IMapper mapper, ApplicationDbContext dbContext)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.dbContext = dbContext;
            response = new ApiResponse();
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = ("defaultCache"))]
        public async Task<ActionResult<ApiResponse>> GetAllBooking( int PageSize = 2, int PageNumber = 1)
        {

            var bookings = await unitOfWork.BookingRepository.GetAll(page_size: PageSize, page_number: PageNumber, includeProperty: "LocalUser, Flight, Hotel", Filter: null) ;
            var check = bookings.Any();
            if (check)
            {
                response.StatusCode = 200;
                response.IsSuccess = check;
                var bookingsMapped = mapper.Map<IEnumerable<BookingDTO>>(bookings);
                response.Result = bookingsMapped;
                return response;
            }
            else
            {
                response.Message = "No Booking Founds";
                response.StatusCode = 200;
                response.IsSuccess = false;
                return response;
            }
        }
       
        [HttpGet("getById")]
        public async Task<ActionResult> GetById([FromQuery] int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiValidationResponse(new List<string> { "Invalid ID", "Try Positive Integer " }, 400));
                }
                var booking =
                 await dbContext.Bookings       
                 .Where(b => b.Id == id)     
                 .Include(b => b.LocalUser).Include(b => b.Flight).Include(b => b.Hotel)      
                 .FirstOrDefaultAsync();
                if (booking == null)
                {
                    var x = booking.ToString();
                    return NotFound(new ApiResponse(404, "booking Not Found"));
                }

                return Ok(new ApiResponse(200, result: booking));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string> { "Internal Server Error", ex.Message }, StatusCodes.Status500InternalServerError));
            }

        }
      
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> CreateBooking([FromBody] BookingFormDTO bookingDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var booking = mapper.Map<Booking>(bookingDTO);
            await unitOfWork.BookingRepository.Create(booking);
            await unitOfWork.Save();
            return CreatedAtAction(nameof(GetAllBooking), new { id = booking.Id }, bookingDTO);
        }
      
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            var booking = await dbContext.Bookings      
                .Where(b => b.Id == id)     
                .Include(b => b.LocalUser)    
                .Include(b => b.Flight)     
                .Include(b => b.Hotel)     
                .FirstOrDefaultAsync();

            dbContext.Bookings.Remove(booking);
            await unitOfWork.Save();
            return Ok();
        }
       
        [HttpGet("{UserId}")]
        public async Task<ActionResult<ApiResponse>> GetBookingByUserId(string UserId)
        {
            var booking = await unitOfWork.BookingRepository.GetBookingsByUserIdAsync(UserId);
            var mappedBooking = mapper.Map<IEnumerable<Booking>, IEnumerable<BookingDTO>>(booking);
            return Ok(mappedBooking);
        }
    }
}
