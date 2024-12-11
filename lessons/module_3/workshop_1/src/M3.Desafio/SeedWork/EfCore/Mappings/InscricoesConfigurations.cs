using M3.Desafio.Inscricoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M3.Desafio.SeedWork.EfCore.Mappings;

public class InscricoesConfigurations : IEntityTypeConfiguration<Inscricao>
{
    public void Configure(EntityTypeBuilder<Inscricao> builder)
    {
        builder.ToTable("Matriculas");
        builder.HasKey(p => p.Id);
        builder.Property(c => c.Aluno).HasColumnType("varchar(300)").IsRequired(true);
        builder.Property(c => c.Responsavel).HasColumnType("varchar(300)").IsRequired(true);
        builder.Property(c => c.Turma).IsRequired();
        builder.Ignore(c => c.Ativa);
    }
}
