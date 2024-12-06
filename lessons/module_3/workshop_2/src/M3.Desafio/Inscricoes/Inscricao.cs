using CSharpFunctionalExtensions;
using M3.Desafio.Inscricoes.Eventos;

namespace M3.Desafio.Inscricoes;

public sealed class Inscricao : SeedWork.Entity<Guid>
{
    private Inscricao() { }

    private Inscricao(Guid id, string aluno, string responsavel, int turma, bool Ativa) : base(id)
    {
        Aluno = aluno;
        Responsavel = responsavel;
        Turma = turma;
    }

    public string Aluno { get; } = string.Empty;
    public string Responsavel { get; } = string.Empty;
    public int Turma { get; }
    public bool Ativa { get; private set; }

    public static Result<Inscricao> Criar(string aluno, string responsavel, int turma)
    {
        var inscricao = new Inscricao(Guid.NewGuid(), aluno, responsavel, turma, true);
        inscricao.AddDomainEvent(new InscricaoRealizadaEvento(inscricao.Id, inscricao.Responsavel));
        return inscricao;
    }

    public Result Cancelar()
    {
        if (!Ativa)
            return Result.Failure("Inscrição já cancelada.");

        Ativa = false;
        AddDomainEvent(new InscricaoCanceladaEvento(Id));
        return Result.Success();
    }
}
