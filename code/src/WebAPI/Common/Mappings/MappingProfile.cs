using AutoMapper;

using Hotels.Domain.Entities;
using Hotels.WebAPI.Entities.DTO;

namespace Hotels.WebAPI.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Hotel,     HotelDTO>().ReverseMap();
        CreateMap<HotelRate, HotelRateDTO>().ReverseMap();
        CreateMap<Price,     PriceDTO>().ReverseMap();
        CreateMap<RateTag,   RateTagDTO>().ReverseMap();
    }
}