using Newtonsoft.Json;

namespace Hotels.WebAPI.Entities.DTO;

public class HotelDTO
{
    [JsonProperty("hotelID")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("classification")]
    public int Classification { get; set; }

    [JsonProperty("reviewscore")]
    public decimal ReviewScore { get; set; }

    [JsonProperty("hotelRates")]
    public ICollection<HotelRateDTO> Rates { get; set; }
}