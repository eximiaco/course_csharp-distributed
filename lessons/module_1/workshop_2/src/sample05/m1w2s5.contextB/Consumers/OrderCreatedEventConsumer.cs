using m1w2s4.Contracts;
using MassTransit;

namespace M1W2S4.ContextB.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventConsumer> _logger;

        public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            _logger.LogInformation(
                "ContextB - Evento de pedido criado recebido: {OrderId} em {CreatedAt}", 
                context.Message.OrderId, 
                context.Message.CreatedAt);

            // Simula algum processamento
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            _logger.LogInformation(
                "ContextB - Processamento do pedido {OrderId} finalizado", 
                context.Message.OrderId);
        }
    }
} 