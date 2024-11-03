using Travel.Core.Entities.DTO;
using Travel.Core.Entities;
using AutoMapper;
namespace Travel.Api.Mapping_Profiles
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<Destination, DestinationDTO>().ReverseMap();
            CreateMap<Destination, DestinationFormDTO>().ReverseMap();
            CreateMap<Hotel, HotelDTO>().ReverseMap();
            CreateMap<Hotel, HotelDesDTO>().ReverseMap();
            CreateMap<Hotel, HotelFormDTO>().ReverseMap();
            CreateMap<Flight, FlightDTO>().ReverseMap(); 
            CreateMap<LocalUser, LocalUserDTO>().ReverseMap();
            CreateMap<Booking, BookingDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.LocalUser))
                .ForMember(dest => dest.Flight, opt => opt.MapFrom(src => src.Flight))
                .ForMember(dest => dest.Hotel, opt => opt.MapFrom(src => src.Hotel))
                .ReverseMap();
            CreateMap<Booking, BookingFormDTO>().ReverseMap();

        }
    }
}
