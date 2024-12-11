namespace M3.Desafio.SeedWork.EfCore;

public sealed class OtelDbContextAccessor : IEFDbContextAccessor<OtelDbContext>
{
    private OtelDbContext _contexto = null!;

    public OtelDbContext Get()
    {
        return _contexto ?? throw new InvalidOperationException("Contexto deve ser registrado!");
    }

    public void Register(OtelDbContext context)
    {
        _contexto = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task ClearAsync()
    {
        await DisposeAsync();
    }

    public void Dispose()
    {
        DisposeCore();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_contexto is null)
            return;

        await _contexto.DisposeAsync().ConfigureAwait(false);
        _contexto = null!;
    }

    private void DisposeCore()
    {
        if (_contexto is null)
            return;

        _contexto.Dispose();
        _contexto = null!;
    }
}
