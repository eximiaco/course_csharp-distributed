using M3.Desafio.Financeiro.Comandos;
using M3.Desafio.Inscricoes.Eventos;
using M3.Desafio.SeedWork.EfCore;
using Silverback.Messaging.Messages;

namespace M3.Desafio.Financeiro.Consumers;

public class GerarMensalidadesParaNovaInscricaoConsumer(
    IEFDbContextAccessor<OtelDbContext> accessor,
    IEFDbContextFactory<OtelDbContext> factory,
    GerarMensalidadesParaNovaInscricaoHandler gerarMensalidadesParaNovaInscricaoHandler)
{
    public async Task ConsumeMessage(IInboundEnvelope<InscricaoRealizadaEvento> message, CancellationToken cancellationToken)
    {
        await using var dbContext = factory.Create();
        accessor.Register(dbContext);

        GerarMensalidadesParaNovaInscricaoComando comando = new(message.Message!.Id, message.Message.Responsavel);
        await gerarMensalidadesParaNovaInscricaoHandler.Executar(comando, cancellationToken);
    }
}
