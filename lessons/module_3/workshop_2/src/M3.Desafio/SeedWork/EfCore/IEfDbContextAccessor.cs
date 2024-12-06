using Microsoft.EntityFrameworkCore;

namespace M3.Desafio.SeedWork.EfCore;

public interface IEFDbContextAccessor<T> : IDisposable, IAsyncDisposable where T : DbContext
{
    void Register(T context);
    T Get();
    Task ClearAsync();
}
