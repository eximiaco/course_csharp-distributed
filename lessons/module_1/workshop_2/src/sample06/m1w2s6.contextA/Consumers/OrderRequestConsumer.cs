using m1w2s6;
using m1w2s6.contextA;
using Silverback.Messaging.Publishing;

namespace M1W2S6.ContextA.Consumers
{
    public class OrderRequestConsumer(ILogger<OrderRequestConsumer> logger, IPublisher publisher)
    {
        public async Task Consume(OrderRequest order)
        {
            logger.LogInformation("Processando pedido: {OrderId}", order.OrderId);
            
            // Simula processamento
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            // Publica evento de pedido criado
            await publisher.PublishAsync(new OrderCreatedEvent 
            { 
                OrderId = order.OrderId 
            });
            
            logger.LogInformation("Pedido processado e evento publicado: {OrderId}", order.OrderId);
        }
    }
} 