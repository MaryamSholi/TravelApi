using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Travel.Core.Entities.DTO;
using Travel.Core.Entities;
using Travel.Infrastructure.Repositories;
using Travel.Core.IRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Travel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DestinationController : ControllerBase
    {
        private readonly IUnitOfWork<Destination> unitOfWork;
        private readonly IMapper mapper;
        public ApiResponse response;

        public DestinationController(IUnitOfWork<Destination> unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            response = new ApiResponse();
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = ("defaultCache"))]
        public async Task<ActionResult<ApiResponse>> GetAllDestinations([FromQuery] string? cityName = null, int PageSize = 2, int PageNumber = 1)
        {
            Expression<Func<Destination, bool>> filter = null;
            if (!string.IsNullOrEmpty(cityName))
            {
                filter = x => x.Name.Equals(cityName);
            }
            var destinations = await unitOfWork.DestinationRepository.GetAll(page_size: PageSize, page_number: PageNumber, Filter: filter);
            var check = destinations.Any();
            if (check)
            {
                response.StatusCode = 200;
                response.IsSuccess = check;
                var destinationMapped = mapper.Map<IEnumerable<DestinationFormDTO>>(destinations);
                response.Result = destinationMapped;
                return response;
            }
            else
            {
                response.Message = "No destinations Founds";
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

                var destination = await unitOfWork.DestinationRepository.GetById(id);
                if (destination == null)
                {
                    var x = destination.ToString();
                    return NotFound(new ApiResponse(404, "destination Not Found"));
                }

                return Ok(new ApiResponse(200, result: destination));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string> { "Internal Server Error", ex.Message }, StatusCodes.Status500InternalServerError));
            }

        }
       
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> CreateDestination([FromBody] DestinationFormDTO destinationFormDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var destination = mapper.Map<Destination>(destinationFormDTO);
            await unitOfWork.DestinationRepository.Create(destination);
            await unitOfWork.Save();
            return CreatedAtAction(nameof(GetAllDestinations), new { id = destination.Id }, destinationFormDTO);
        }
      
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> UpdateDestination(int id, [FromBody] DestinationFormDTO destinationFormDTO)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiValidationResponse(new List<string> { "Invalid ID", "Try Positive Integer" }, 400));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDestination = await unitOfWork.DestinationRepository.GetById(id);

            if (existingDestination == null)
            {
                return NotFound(new ApiResponse(404, $"Destination with ID {id} not found."));
            }

            existingDestination.Name = destinationFormDTO.Name;
            existingDestination.Country = destinationFormDTO.Country;
            existingDestination.Description = destinationFormDTO.Description;
            existingDestination.Price = destinationFormDTO.Price;


            unitOfWork.DestinationRepository.Update(existingDestination);
            await unitOfWork.Save();

            return Ok(new ApiResponse(200, $"Destination with updated successfully."));
        }
      
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeleteDestinaton(int id)
        {
            unitOfWork.DestinationRepository.Delete(id);
            await unitOfWork.Save();
            return Ok();
        }
       
        [HttpGet("GetWithHotels")]
        public async Task<ActionResult<ApiResponse>> GetAllDestinationsWithHotels()
        {
            try
            {
                var destinations = await unitOfWork.DestinationRepository.GetWithHotelsAsync();

                if (destinations == null || !destinations.Any())
                {
                    return NotFound(new ApiResponse(404, "No destinations with hotels found."));
                }
                var destinationDTOs = mapper.Map<IEnumerable<DestinationDTO>>(destinations);

                var response = new ApiResponse(200, result: destinationDTOs);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiValidationResponse(new List<string> { "Internal Server Error", ex.Message }, 500));
            }
        }
    }
}
