using CSharpFunctionalExtensions;
using M3.Desafio.SeedWork;
using Serilog;

namespace M3.Desafio.Inscricoes.Comandos;

public class RealizarInscricaoHandler(
    InscricoesRepositorio inscricoesRepositorio,
    IUnitOfWork unitOfWork,
    ILogger logger) : IService<RealizarInscricaoHandler>
{
    public async Task<Result> Executar(RealizarInscricaoComando comando, CancellationToken cancellationToken)
    {
        logger.Information($"Realizando a inscrição do aluno {comando.Aluno}.");

        if (!await inscricoesRepositorio.AlunoExiste(comando.Aluno))
        {
            logger.Error($"Aluno {comando.Aluno} não existe.");
            return Result.Failure("Aluno não existe.");
        }

        if (!await inscricoesRepositorio.ResponsavelExiste(comando.Responsavel))
        {
            logger.Error($"Responsável {comando.Responsavel} não existe.");
            return Result.Failure("Responsável não existe.");
        }

        var turma = await inscricoesRepositorio.RecuperarTurma(comando.Turma, cancellationToken);
        if (turma.HasNoValue)
        {
            logger.Error($"Turma {comando.Turma} não existe.");
            return Result.Failure("Turma não existe.");
        }

        var inscricao = Inscricao.Criar(comando.Aluno, comando.Responsavel, turma.Value.Id);
        if (inscricao.IsFailure)
        {
            logger.Error($"Falha criando a inscrição. Erro: {inscricao.Error}");
            return inscricao;
        }

        await inscricoesRepositorio.Adicionar(inscricao.Value, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        logger.Information($"Inscrição {inscricao.Value.Id} realizada com sucesso.");
        return Result.Success();
    }
}
