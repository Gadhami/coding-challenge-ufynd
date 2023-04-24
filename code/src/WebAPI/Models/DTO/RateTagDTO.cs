using Newtonsoft.Json;

namespace Hotels.WebAPI.Entities.DTO;

public class RateTagDTO
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("shape")]
    public bool Shape { get; set; }
}