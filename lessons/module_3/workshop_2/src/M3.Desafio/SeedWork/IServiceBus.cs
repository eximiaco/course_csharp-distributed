namespace M3.Desafio.SeedWork;

public interface IServiceBus : IService<IServiceBus>
{
    Task PublishAsync(object message, CancellationToken cancellationToken);
}
