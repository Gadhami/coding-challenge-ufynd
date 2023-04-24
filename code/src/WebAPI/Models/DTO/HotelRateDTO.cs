using Newtonsoft.Json;

namespace Hotels.WebAPI.Entities.DTO;

public class HotelRateDTO
{
    [JsonProperty("rateID")]
    public string Id { get; set; }

    [JsonProperty("rateName")]
    public string Name { get; set; }

    [JsonProperty("rateDescription")]
    public string Description { get; set; }

    [JsonProperty("adults")]
    public int Adults { get; set; }

    [JsonProperty("los")]
    public int Los { get; set; }

    [JsonProperty("price")]
    public PriceDTO Price { get; set; }

    [JsonProperty("targetDay")]
    public DateTime TargetDay { get; set; }

    [JsonProperty("rateTags")]
    public ICollection<RateTagDTO> RateTags { get; set; }
}