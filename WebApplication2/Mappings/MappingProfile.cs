using AutoMapper;
using WebApplication2.Dtos;
using WebApplication2.Models;

namespace WebApplication2.Mappings
{
    public class MappingProfile : Profile
    {
      public MappingProfile() {
            CreateMap<NewPassengerDto,Passanger>();
            CreateMap<NewDriverDto, Driver>();
            CreateMap<RequestRideDto, Rides>();
        }
    }
}
