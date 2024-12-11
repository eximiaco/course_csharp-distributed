using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using M3.Desafio.Inscricoes;

namespace M3.Desafio.SeedWork.EfCore.Mappings;
public class TurmasConfigurations : IEntityTypeConfiguration<Turma>
{
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        builder.ToTable("Turmas");
        builder.HasKey(p => p.Id);
        builder.Property(c => c.Vagas);
    }
}