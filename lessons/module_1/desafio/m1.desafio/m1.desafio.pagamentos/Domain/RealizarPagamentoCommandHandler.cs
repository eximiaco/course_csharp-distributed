using Silverback.Messaging.Publishing;

namespace m1.desafio.pagamentos.Domain;

public class RealizarPagamentoCommandHandler(IPublisher publisher)
{
    public async Task PagarAsync(RealizarPagamentoCommand comando, CancellationToken cancellationToken)
    {
        // Realiza o pagamento

        await publisher.PublishAsync(new PagamentoRealizadoEvent(comando.IngressoId, PagamentoId: 1)).ConfigureAwait(false);
    }
}

public record PagamentoRealizadoEvent(int IngressoId, int PagamentoId);
