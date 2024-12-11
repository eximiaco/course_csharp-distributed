using M3.Desafio.Acessos.Comandos;
using M3.Desafio.Inscricoes.Eventos;
using M3.Desafio.SeedWork.EfCore;
using Silverback.Messaging.Messages;

namespace M3.Desafio.Acessos.Consumers;

public class DarPermissaoAcessoParaNovaInscricaoConsumer(
    IEFDbContextAccessor<OtelDbContext> accessor,
    IEFDbContextFactory<OtelDbContext> factory,
    DarPermissaoAcessoParaNovaInscricaoHandler darPermissaoAcessoParaNovaInscricaoHandler)
{
    public async Task Executar(IInboundEnvelope<InscricaoRealizadaEvento> message, CancellationToken cancellationToken)
    {
        await using var dbContext = factory.Create();
        accessor.Register(dbContext);

        DarPermissaoAcessoParaNovaInscricaoCommand comando = new(message.Message!.Id);
        await darPermissaoAcessoParaNovaInscricaoHandler.Executar(comando, cancellationToken);
    }
}
