using Hotels.Domain.Entities;

namespace Hotels.Domain.Interfaces;

public interface IUnitOfWork  // : IDisposable
{
    IRepository<Hotel>     Hotels { get; }
    IRepository<HotelRate> Rates  { get; }

    IRepository<T> Tables<T>() where T : class;
}