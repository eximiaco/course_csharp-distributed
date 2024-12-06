namespace M3.Desafio.SeedWork.EfCore;

public class MemoryServiceBus : IServiceBus
{
    public Task PublishAsync(object message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
