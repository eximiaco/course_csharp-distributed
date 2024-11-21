namespace m1w2s7.azureServiceBus.ServiceBus;

public static class DisposableExtensions
{
    public static IDisposable AsDisposable<T>(this T instance, Action<T> disposeAction) => new Disposable(() => disposeAction(instance));

    class Disposable : IDisposable
    {
        readonly Action _action;
        public Disposable(Action action) => _action = action ?? throw new ArgumentNullException(nameof(action));
        public void Dispose() => _action();
    }
}
