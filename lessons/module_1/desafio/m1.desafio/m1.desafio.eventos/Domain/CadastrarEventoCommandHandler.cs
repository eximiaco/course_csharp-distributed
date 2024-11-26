using Silverback.Messaging.Publishing;

namespace m1.desafio.eventos.Domain;

public class CadastrarEventoCommandHandler(IPublisher publisher)
{
    public async Task Cadastrar(CadastrarEventoCommand comando, CancellationToken cancellationToken)
    {
        // Faz o cadastro do evento

        // Publica evento de indicando que um evento foi cadastrado
        // Os contextos interessados podem se inscrever e receber esse evento
        await publisher.PublishAsync(new EventoCadastradoEvent(Id: 1)).ConfigureAwait(false);
    }
}

public record EventoCadastradoEvent(int Id);
