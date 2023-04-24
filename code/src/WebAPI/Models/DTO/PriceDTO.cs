using Newtonsoft.Json;

namespace Hotels.WebAPI.Entities.DTO;

public class PriceDTO
{
    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("numericFloat")]
    public decimal NumericFloat { get; set; }

    [JsonProperty("numericInteger")]
    public int NumericInteger { get; set; }
}