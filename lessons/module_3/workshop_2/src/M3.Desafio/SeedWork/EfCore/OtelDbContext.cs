using M3.Desafio.Inscricoes;
using M3.Desafio.SeedWork.EfCore.Mappings;
using Microsoft.EntityFrameworkCore;

namespace M3.Desafio.SeedWork.EfCore;

public class OtelDbContext : DbContext
{
    private readonly IServiceBus _serviceBus;

    public OtelDbContext(DbContextOptions<OtelDbContext> options, IServiceBus serviceBus) : base(options)
    {
        _serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));

        System.Diagnostics.Debug.WriteLine("InscricoesDbContext::ctor ->" + GetHashCode());
    }

    public DbSet<Inscricao> Inscricoes { get; set; }
    public DbSet<Turma> Turmas { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _serviceBus.DispatchDomainEventsAsync(this, cancellationToken).ConfigureAwait(false);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InscricoesConfigurations());
        modelBuilder.ApplyConfiguration(new TurmasConfigurations());
    }
}
