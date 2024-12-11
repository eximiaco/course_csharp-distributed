using CSharpFunctionalExtensions;

namespace M3.Desafio.Financeiro;

public sealed class Mensalidade : SeedWork.Entity<Guid>
{
    public Mensalidade(Guid id, Guid inscricaoId, string responsavelFinanceiro, decimal valor) : base(id)
    {
        InscricaoId = inscricaoId;
        ResponsavelFinanceiro = responsavelFinanceiro;
        Valor = valor;
    }

    public Guid InscricaoId { get; }
    public string ResponsavelFinanceiro { get; }
    public decimal Valor { get; }

    public static Result<Mensalidade> Criar(Guid inscricaoId, string resposnavel, decimal valor)
    {
        return new Mensalidade(Guid.NewGuid(), inscricaoId, resposnavel, valor);
    }
}
