using M3.Desafio.Financeiro;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace M3.Desafio.SeedWork.EfCore.Mappings;

public sealed class MensalidadeConfiguration : IEntityTypeConfiguration<Mensalidade>
{
    public void Configure(EntityTypeBuilder<Mensalidade> builder)
    {
        builder.ToTable("Mensalidades");
        builder.HasKey(p => p.Id);
        builder.Property(c => c.InscricaoId).IsRequired(true);
        builder.Property(c => c.ResponsavelFinanceiro);
        builder.Property(c => c.Valor).IsRequired(true);
    }
}
