using M3.Desafio.SeedWork;

namespace M3.Desafio.SeedWork.EfCore;

public class UnitOfWork : IUnitOfWork
{
    private readonly IEFDbContextAccessor<OtelDbContext> _efDbContextAccessor;

    public UnitOfWork(IEFDbContextAccessor<OtelDbContext> efDbContextAccessor)
    {
        _efDbContextAccessor = efDbContextAccessor;
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        await _efDbContextAccessor.Get().SaveChangesAsync(cancellationToken);
    }
}
