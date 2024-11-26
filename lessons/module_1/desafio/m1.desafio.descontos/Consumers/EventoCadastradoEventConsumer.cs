namespace m1.desafio.descontos.Consumers;

public class EventoCadastradoEventConsumer
{
    public async Task Consume(EventoCadastradoEvent eventoCadastrado)
    {
        // Libera o cadastro dos descontos para o evento em questão
    }
}

public record EventoCadastradoEvent(int Id);
