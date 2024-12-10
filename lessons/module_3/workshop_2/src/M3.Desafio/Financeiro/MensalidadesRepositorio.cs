using M3.Desafio.SeedWork.EfCore;
using M3.Desafio.SeedWork;

namespace M3.Desafio.Financeiro;

public sealed class MensalidadesRepositorio : IService<MensalidadesRepositorio>
{
    private readonly IEFDbContextAccessor<OtelDbContext> _dbContext;

    public MensalidadesRepositorio(IEFDbContextAccessor<OtelDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Adicionar(IEnumerable<Mensalidade> mensalidades, CancellationToken cancellationToken)
    {
        await _dbContext.Get().Mensalidades.AddRangeAsync(mensalidades, cancellationToken);
    }
}
