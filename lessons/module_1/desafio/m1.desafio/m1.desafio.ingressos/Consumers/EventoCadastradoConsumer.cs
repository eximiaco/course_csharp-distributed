namespace m1.desafio.ingressos.Consumers;

public class EventoCadastradoConsumer
{
    public async Task Consume(EventoCadastradoEvent eventoCadastrado)
    {
        // Disponibiliza os ingressos do evento para venda
    }
}

public record EventoCadastradoEvent(int Id);
