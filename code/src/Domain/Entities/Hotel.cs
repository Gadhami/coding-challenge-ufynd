using System.ComponentModel.DataAnnotations.Schema;
using Hotels.Domain.Interfaces;

namespace Hotels.Domain.Entities;

public class Hotel : IEntity
{
    [Column("hotelID")]
    public string Id   { get; set; } = null!;

    public string Name { get; set; } = null!;
    public int Classification  { get; set; }
    public decimal ReviewScore { get; set; }

    public ICollection<HotelRate> Rates { get; set; }
}