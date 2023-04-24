using System.ComponentModel.DataAnnotations.Schema;
using Hotels.Domain.Interfaces;

namespace Hotels.Domain.Entities;

public class HotelRate : IEntity
{
    [Column("rateID")]
    public string Id   { get; set; } = null!;

    [Column("rateName")]
    public string Name { get; set; } = null!;

    [Column("rateDescription")]
    public string Description { get; set; } = null!;

    public int Adults  { get; set; }
    public int Los     { get; set; }
    public Price Price { get; set; }
    public DateTime TargetDay { get; set; }

    public ICollection<RateTag> rateTags { get; set; }

    public object ShallowCopy()
    {
        return MemberwiseClone();
    }
}