using CSharpFunctionalExtensions;
using M3.Desafio.SeedWork;

namespace M3.Desafio.Inscricoes.Comandos;

public class RealizarInscricaoHandler(InscricoesRepositorio inscricoesRepositorio, IUnitOfWork unitOfWork) : IService<RealizarInscricaoHandler>
{
    public async Task<Result> Executar(RealizarInscricaoComando comando, CancellationToken cancellationToken)
    {
        if (!await inscricoesRepositorio.AlunoExiste(comando.Aluno))
            return Result.Failure("Aluno não existe.");

        if (!await inscricoesRepositorio.ResponsavelExiste(comando.Responsavel))
            return Result.Failure("Responsável não existe.");

        var turma = await inscricoesRepositorio.RecuperarTurma(comando.Turma, cancellationToken);
        if (turma.HasNoValue)
            return Result.Failure("Turma não existe.");

        var inscricao = Inscricao.Criar(comando.Aluno, comando.Responsavel, turma.Value.Id);
        if (inscricao.IsFailure)
            return inscricao;

        await inscricoesRepositorio.Adicionar(inscricao.Value, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return Result.Success();
    }
}
