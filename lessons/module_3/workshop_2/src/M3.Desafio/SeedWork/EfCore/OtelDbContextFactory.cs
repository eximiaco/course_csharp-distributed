using M3.Desafio.SeedWork.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace M3.Desafio.SeedWork.EfCore;

public sealed class OtelDbContextFactory : IEFDbContextFactory<OtelDbContext>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceBus _serviceBus;

    public OtelDbContextFactory(IConfiguration configuration, IServiceBus serviceBus)
    {
        _configuration = configuration;
        _serviceBus = serviceBus;
    }

    public OtelDbContext Create()
    {
        var options = new DbContextOptionsBuilder<OtelDbContext>()
             .EnableDetailedErrors()
             .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), options => options.EnableRetryOnFailure())
             .Options;
        return new OtelDbContext(options, _serviceBus);
    }
}
