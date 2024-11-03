using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Travel.Core.Entities.DTO;
using Travel.Core.Entities;
using Travel.Core.IRepositories;
using Travel.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Travel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork<Hotel> unitOfWork;
        private readonly IMapper mapper;
        public ApiResponse response;

        public HotelController(IUnitOfWork<Hotel> unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            response = new ApiResponse();
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = ("defaultCache"))]
        public async Task<ActionResult<ApiResponse>> GetAllHotel([FromQuery] string? name = null, int PageSize = 2, int PageNumber = 1)
        {
            Expression<Func<Hotel, bool>> filter = null;
            if (!string.IsNullOrEmpty(name))
            {
                filter = x => x.Name.Equals(name);
            }
            var hotels = await unitOfWork.HotelRepository.GetAll(page_size: PageSize, page_number: PageNumber, includeProperty: "Destination", Filter: filter);
            var check = hotels.Any();
            if (check)
            {
                response.StatusCode = 200;
                response.IsSuccess = check;
                var hotelMapped = mapper.Map<IEnumerable<HotelDTO>>(hotels);
                response.Result = hotelMapped;
                return response;
            }
            else
            {
                response.Message = "No Hotels Founds";
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

                var hotel = await unitOfWork.HotelRepository.GetById(id);
                if (hotel == null)
                {
                    var x = hotel.ToString();
                    return NotFound(new ApiResponse(404, "hotel Not Found"));
                }

                return Ok(new ApiResponse(200, result: hotel));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string> { "Internal Server Error", ex.Message }, StatusCodes.Status500InternalServerError));
            }

        }
       
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> CreateHotel([FromBody] HotelFormDTO hotelDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var hotel = mapper.Map<Hotel>(hotelDTO);
            await unitOfWork.HotelRepository.Create(hotel);
            await unitOfWork.Save();
            return CreatedAtAction(nameof(GetAllHotel), new { id = hotel.Id }, hotelDTO);
        }
       
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> UpdateHotel(int id, [FromBody] HotelFormDTO hotelDTO)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiValidationResponse(new List<string> { "Invalid ID", "Try Positive Integer" }, 400));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingHotel = await unitOfWork.HotelRepository.GetById(id);

            if (existingHotel == null)
            {
                return NotFound(new ApiResponse(404, $"Hotel with ID {id} not found."));
            }

            existingHotel.Name = hotelDTO.Name;
            existingHotel.DestinationId = hotelDTO.DestinationId;
            existingHotel.Address = hotelDTO.Address;
            existingHotel.PricePerNight = hotelDTO.PricePerNight;


            unitOfWork.HotelRepository.Update(existingHotel);
            await unitOfWork.Save();

            return Ok(new ApiResponse(200, $"Hotel with updated successfully."));
        }
       
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeleteHotel(int id)
        {
            unitOfWork.HotelRepository.Delete(id);
            await unitOfWork.Save();
            return Ok();
        }
      
        [HttpGet("{DesId}")]
        public async Task<ActionResult<ApiResponse>> GetHotelByDestinationId(int DesId)
        {
            var hotel = await unitOfWork.HotelRepository.GetHotelsByDestinationIdAsync(DesId);
            var mappedHotel = mapper.Map<IEnumerable<Hotel>, IEnumerable<HotelDTO>>(hotel);
            return Ok(mappedHotel);
        }
    }
    



}
