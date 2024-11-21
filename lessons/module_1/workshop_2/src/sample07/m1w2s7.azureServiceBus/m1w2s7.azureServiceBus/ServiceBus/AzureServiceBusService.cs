using Azure.Messaging.ServiceBus;
using CSharpFunctionalExtensions;
using System.Collections.Concurrent;
using System.Text;

namespace m1w2s7.azureServiceBus.ServiceBus;

public sealed class AzureServiceBusService(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("AzureServiceBus")!;
    private readonly ConcurrentStack<IDisposable> _disposables = new ConcurrentStack<IDisposable>();

    private ServiceBusClientOptions _options => new ServiceBusClientOptions()
    {
        RetryOptions = new ServiceBusRetryOptions()
        {
            Mode = ServiceBusRetryMode.Exponential,
            MaxRetries = 10,
            Delay = TimeSpan.FromMilliseconds(800),
            MaxDelay = TimeSpan.FromMinutes(3),
            TryTimeout = TimeSpan.FromMinutes(3)
        }
    };

    public async Task SendAsync<TMessage>(TMessage message, string queueOrTopic = "", CancellationToken cancellationToken = default)
    {
        queueOrTopic = string.IsNullOrWhiteSpace(queueOrTopic) ? message.GetType().Name : queueOrTopic;

        await using var client = new ServiceBusClient(_connectionString, _options);
        var sender = client.CreateSender(queueOrTopic);

        _disposables.Push(sender.AsDisposable(s => AsyncHelpers.RunSync(async () => await s.CloseAsync().ConfigureAwait(false))));
        var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message.ToJson()));
        await sender.SendMessageAsync(serviceBusMessage, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result> SendAsync<TMessage>(string queueOrTopic, IEnumerable<TMessage> messages, CancellationToken cancellationToken = default)
    {
        await using var client = new ServiceBusClient(_connectionString, _options);
        var sender = client.CreateSender(queueOrTopic);

        using var batch = await sender.CreateMessageBatchAsync(cancellationToken);
        foreach (var message in messages)
        {
            var messageToAdd = new ServiceBusMessage(Encoding.UTF8.GetBytes(message.ToJson()));
            if (!batch.TryAddMessage(messageToAdd))
                return Result.Failure("O batch de mensagens é muito grande para ser enviado.");
        }

        _disposables.Push(sender.AsDisposable(s => AsyncHelpers.RunSync(async () => await s.CloseAsync().ConfigureAwait(false))));
        await sender.SendMessagesAsync(batch, cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }

    public void Dispose()
    {
        while (_disposables.TryPop(out var disposable))
            disposable.Dispose();
    }
}
