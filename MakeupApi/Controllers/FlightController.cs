using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Travel.Core.Entities;
using Travel.Core.Entities.DTO;
using Travel.Core.IRepositories;
using Travel.Infrastructure.Repositories;

namespace Travel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FlightController : ControllerBase
    {
        private readonly IUnitOfWork<Flight> unitOfWork;
        private readonly IMapper mapper;
        public ApiResponse response;

        public FlightController(IUnitOfWork<Flight> unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            response = new ApiResponse();
        }
       
        [HttpGet]
        [ResponseCache(CacheProfileName = ("defaultCache"))]
        public async Task<ActionResult<ApiResponse>> GetAllFlights([FromQuery] string? destination = null, int PageSize = 2, int PageNumber = 1)
        {
            Expression<Func<Flight, bool>> filter = null;
            if (!string.IsNullOrEmpty(destination))
            {
                filter = x => x.Destination.Equals(destination);
            }
            var flights = await unitOfWork.FlightRepository.GetAll(page_size: PageSize, page_number: PageNumber,  Filter: filter);
            var check = flights.Any();
            if(check)
            {
                response.StatusCode = 200;
                response.IsSuccess = check;
                var flightMapped = mapper.Map<IEnumerable<FlightDTO>>(flights);
                response.Result = flightMapped;
                return response;
            }
            else
            {
                response.Message = "No flight Founds";
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

                var flight = await unitOfWork.FlightRepository.GetById(id);
                if (flight == null)
                {
                    var x = flight.ToString();
                    return NotFound(new ApiResponse(404, "Flight Not Found"));
                }

                return Ok(new ApiResponse(200, result: flight));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string> { "Internal Server Error", ex.Message }, StatusCodes.Status500InternalServerError));
            }

        }
       
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> CreateFlight([FromBody] FlightDTO flightDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var flight = mapper.Map<Flight>(flightDTO);
            await unitOfWork.FlightRepository.Create(flight);
            await unitOfWork.Save();
            return CreatedAtAction(nameof(GetAllFlights), new { id = flight.Id }, flightDTO);
        }
      
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> UpdateFlight(int id, [FromBody] FlightDTO flightDTO)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiValidationResponse(new List<string> { "Invalid ID", "Try Positive Integer" }, 400));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFlight = await unitOfWork.FlightRepository.GetById(id);

            if (existingFlight == null)
            {
                return NotFound(new ApiResponse(404, $"Flight with ID {id} not found."));
            }

            existingFlight.Airline = flightDTO.Airline;
            existingFlight.Origin = flightDTO.Origin;
            existingFlight.Destination = flightDTO.Destination;
            existingFlight.ArrivalTime = flightDTO.ArrivalTime;
            existingFlight.ArrivalTime = flightDTO.ArrivalTime;
            existingFlight.Price = flightDTO.Price;

            unitOfWork.FlightRepository.Update(existingFlight);
            await unitOfWork.Save();

            return Ok(new ApiResponse(200, $"Flight updated successfully."));
        }
       
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeleteFlight(int id)
        {
            unitOfWork.FlightRepository.Delete(id);
            await unitOfWork.Save();
            return Ok();
        }
       
        [HttpGet("{origin}")]
        public async Task<ActionResult<ApiResponse>> GetFlightByOrigin(string origin)
        {
            var flights = await unitOfWork.FlightRepository.GetFlightsByOriginAsync(origin);
            var mappedFlights = mapper.Map<IEnumerable<Flight>, IEnumerable<FlightDTO>>(flights);
            return Ok(mappedFlights);
        }

    }
}
