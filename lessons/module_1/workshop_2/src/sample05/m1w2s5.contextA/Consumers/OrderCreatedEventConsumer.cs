using m1w2s4.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace m1w2s5.contextA.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventConsumer> _logger;

        public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            _logger.LogInformation("Contexto Pedidos, recebeu evento de pedido criado: {OrderId} em {CreatedAt}", 
                context.Message.OrderId, 
                context.Message.CreatedAt);
                
            return Task.CompletedTask;
        }
    }
}  