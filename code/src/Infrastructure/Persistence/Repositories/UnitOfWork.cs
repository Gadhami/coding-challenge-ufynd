using Hotels.Domain.Entities;
using Hotels.Domain.Interfaces;

namespace Hotels.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IRepository<Hotel> hotelsRepo, IRepository<HotelRate> hotelRatesRepo)
    {
        Hotels = hotelsRepo;
        Rates  = hotelRatesRepo;
    }

    public IRepository<Hotel>     Hotels { get; }
    public IRepository<HotelRate> Rates  { get; }

    public IRepository<T> Tables<T>() where T : class
    {
        return typeof(T) switch
        {
            var _ when typeof(T) == typeof(Hotel) => (IRepository<T>)Hotels,
            var _ when typeof(T) == typeof(HotelRate) => (IRepository<T>)Rates,

            _ => throw new ArgumentException("You must provide a valid table model object (Hotel, Rate, ...)")
        };
    }

    #region Dispose Code

    //public void Dispose()
    //{
    //    Dispose();
    //    GC.SuppressFinalize(this);
    //}

    #endregion Dispose Code
}