using M3.Desafio.Acessos;
using M3.Desafio.Financeiro;
using M3.Desafio.Inscricoes;
using M3.Desafio.SeedWork.EfCore.Mappings;
using M3.Desafio.SeedWork.ServiceBus;
using Microsoft.EntityFrameworkCore;

namespace M3.Desafio.SeedWork.EfCore;

public class OtelDbContext : DbContext
{
    private readonly IServiceBus _serviceBus;

    public OtelDbContext(DbContextOptions<OtelDbContext> options, IServiceBus serviceBus) : base(options)
    {
        _serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
    }

    public DbSet<Inscricao> Inscricoes { get; set; }
    public DbSet<Turma> Turmas { get; set; }
    public DbSet<Mensalidade> Mensalidades { get; set; }
    public DbSet<PermissaoAcesso> PermissaoAcessos { get; set; }

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
        modelBuilder.ApplyConfiguration(new MensalidadeConfiguration());
        modelBuilder.ApplyConfiguration(new PermissaoAcessoConfiguration());
    }
}
