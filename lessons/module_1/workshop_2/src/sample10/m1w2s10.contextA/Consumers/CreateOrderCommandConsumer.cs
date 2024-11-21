using Silverback.Messaging.Publishing;

namespace m1w2s10.contextA.Consumers
{
    public class CreateOrderCommandConsumer(ILogger<CreateOrderCommandConsumer> logger, IPublisher publisher)
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