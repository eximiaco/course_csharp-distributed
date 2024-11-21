using m1w2s4.Contracts;
using MassTransit;

namespace m1w2s5.contextA.Consumers
{
    public class OrderRequestConsumer : IConsumer<OrderRequest>
    {
        private readonly ILogger<OrderRequestConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderRequestConsumer(ILogger<OrderRequestConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderRequest> context)
        {
            _logger.LogInformation("Processando pedido: {OrderId}", context.Message.OrderId);
            
            // Simula processamento
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            // Publica evento de pedido criado
            await _publishEndpoint.Publish(new OrderCreatedEvent 
            { 
                OrderId = context.Message.OrderId 
            });
            
            _logger.LogInformation("Pedido processado e evento publicado: {OrderId}", context.Message.OrderId);
        }
    }
} 