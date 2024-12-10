using Silverback.Messaging.Publishing;

namespace M3.Desafio.SeedWork.ServiceBus.Silverback;

public class SilverbackServiceBus(IPublisher publisher) : IServiceBus
{
    public Task PublishAsync(object message)
    {
        return publisher.PublishAsync(message);
    }
}
