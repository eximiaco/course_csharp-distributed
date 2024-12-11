using Microsoft.EntityFrameworkCore;

namespace M3.Desafio.SeedWork.EfCore;

public interface IEFDbContextFactory<T> where T : DbContext
{
    T Create();
}
