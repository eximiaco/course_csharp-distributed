using M3.Desafio.SeedWork.EfCore;

namespace M3.Desafio.Inscricoes.API.Infrastructure;

public class UnitOfWorkMiddleware(
    IEFDbContextAccessor<OtelDbContext> accessor,
    IEFDbContextFactory<OtelDbContext> factory) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await using var dbContext = factory.Create();
        accessor.Register(dbContext);
        await next(context);
    }
}
