using M3.Desafio.Acessos;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace M3.Desafio.SeedWork.EfCore.Mappings;

public sealed class PermissaoAcessoConfiguration : IEntityTypeConfiguration<PermissaoAcesso>
{
    public void Configure(EntityTypeBuilder<PermissaoAcesso> builder)
    {
        builder.ToTable("Permissoes");
        builder.HasKey(p => p.Id);
        builder.HasKey(p => p.InscricaoId);
    }
}