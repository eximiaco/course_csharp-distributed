namespace M3.Desafio.Common;

public interface IServiceBus
{
    Task PublishAsync(object message, CancellationToken cancellationToken);
}
