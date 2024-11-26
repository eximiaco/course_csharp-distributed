namespace m1.desafio.notificacoes.Consumers;

public class PagamentoRealizadoConsumer
{
    public async Task Consume(PagamentoRealizadoEvent pagamentoRealizado)
    {
        // Notificação a pessoa da compra
    }
}

public record PagamentoRealizadoEvent(int IngressoId, int PagamentoId);
