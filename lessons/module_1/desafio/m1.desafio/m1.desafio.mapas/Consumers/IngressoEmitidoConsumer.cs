namespace m1.desafio.mapas.Consumers;

public class IngressoEmitidoConsumer
{
    public async Task Consume(IngressoEmitidoEvent pagamentoRealizado)
    {
        // Atualiza a lotação do mapa do evento após ingresso ter sido emitido
    }
}

public record IngressoEmitidoEvent(int IngressoId);