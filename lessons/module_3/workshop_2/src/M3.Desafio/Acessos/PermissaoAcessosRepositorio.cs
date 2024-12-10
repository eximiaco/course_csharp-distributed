using M3.Desafio.SeedWork;
using M3.Desafio.SeedWork.EfCore;

namespace M3.Desafio.Acessos;

public class PermissaoAcessosRepositorio(IEFDbContextAccessor<OtelDbContext> dbContext) : IService<PermissaoAcessosRepositorio>
{
    public async Task Adicionar(PermissaoAcesso permissaoAcesso, CancellationToken cancellationToken)
    {
        await dbContext.Get().PermissaoAcessos.AddAsync(permissaoAcesso, cancellationToken);
    }
}
