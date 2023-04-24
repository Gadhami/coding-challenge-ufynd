using AutoMapper;

using Hotels.Domain.Entities;

namespace Hotels.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Hotel,     HotelDTO>().ReverseMap();
            CreateMap<HotelRate, HotelRateDTO>().ReverseMap();
        }
    }
}