using Hotels.WebAPI.Entities.DTO;

namespace Hotels.WebAPI.Entities;

public class HotelDataWrapper
{
    public HotelDTO Hotel { get; set; }
    public ICollection<HotelRateDTO> HotelRates { get; set; }
}