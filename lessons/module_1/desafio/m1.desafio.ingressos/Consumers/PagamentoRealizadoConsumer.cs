using Silverback.Messaging.Publishing;

namespace m1.desafio.ingressos.Consumers;

public class PagamentoRealizadoConsumer(IPublisher publisher)
{
    public async Task Consume(PagamentoRealizadoEvent pagamentoRealizado)
    {
        // Emite o ingresso após o pagamento ser realizado

        await publisher.PublishAsync(new IngressoEmitidoEvent(IngressoId: 123)).ConfigureAwait(false);
    }
}

public record PagamentoRealizadoEvent(int IngressoId, int PagamentoId);
public record IngressoEmitidoEvent(int IngressoId);