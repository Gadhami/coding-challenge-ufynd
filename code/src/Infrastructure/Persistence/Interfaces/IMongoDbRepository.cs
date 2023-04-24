using MongoDB.Driver;
using Hotels.Domain.Interfaces;

namespace Hotels.Infrastructure.Persistence.Interfaces;

public interface IMongoDbRepository<T> : IRepository<T> where T : class, IEntity
{
    Task<IEnumerable<T>> GetAllWithFilterAsync(FilterDefinition<T> filter);
}