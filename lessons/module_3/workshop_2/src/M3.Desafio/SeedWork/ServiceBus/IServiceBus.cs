namespace M3.Desafio.SeedWork.ServiceBus;

public interface IServiceBus : IService<IServiceBus>
{
    Task PublishAsync(object message);
}
