using M3.Desafio.SeedWork;

namespace M3.Desafio.Acessos;

public sealed class PermissaoAcesso : Entity<Guid>
{
    public PermissaoAcesso(Guid id, Guid inscricaoId) : base(id)
    {
        InscricaoId = inscricaoId;
    }

    public Guid InscricaoId { get; }
}
