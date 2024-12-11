using M3.Desafio.Acessos.Telemetria;
using M3.Desafio.SeedWork;

namespace M3.Desafio.Acessos.Comandos;

public class DarPermissaoAcessoParaNovaInscricaoHandler
    (PermissaoAcessosRepositorio permissaoAcessosRepositorio,
    IUnitOfWork unitOfWork,
    DarPermissaoAcessoParaNovaInscricaoTelemetry darPermissaoAcessoParaNovaInscricaoTelemetry) : IService<DarPermissaoAcessoParaNovaInscricaoCommand>
{
    public async Task Executar(DarPermissaoAcessoParaNovaInscricaoCommand comando, CancellationToken cancellationToken)
    {
        darPermissaoAcessoParaNovaInscricaoTelemetry.DarPermissaoAcessoParaNovaInscricao(comando);

        PermissaoAcesso permissaoAcesso = new(Guid.NewGuid(), comando.InscricaoId);
        await permissaoAcessosRepositorio.Adicionar(permissaoAcesso, cancellationToken).ConfigureAwait(false);
        await unitOfWork.Commit(cancellationToken).ConfigureAwait(false);

        darPermissaoAcessoParaNovaInscricaoTelemetry.PermissaoConcedidaComSucesso(permissaoAcesso.Id);
    }
}
