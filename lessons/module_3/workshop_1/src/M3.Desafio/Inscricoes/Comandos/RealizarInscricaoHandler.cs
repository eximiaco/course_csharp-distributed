using CSharpFunctionalExtensions;
using M3.Desafio.Inscricoes.Telemetria;
using M3.Desafio.SeedWork;

namespace M3.Desafio.Inscricoes.Comandos;

public class RealizarInscricaoHandler(
    InscricoesRepositorio inscricoesRepositorio,
    IUnitOfWork unitOfWork,
    RealizarInscricaoTelemetry realizarInscricaoTelemetry) : IService<RealizarInscricaoHandler>
{
    public async Task<Result> Executar(RealizarInscricaoComando comando, CancellationToken cancellationToken)
    {
        realizarInscricaoTelemetry.NovaInscricaoRecebida(comando);

        if (!await inscricoesRepositorio.AlunoExiste(comando.Aluno))
        {
            realizarInscricaoTelemetry.AlunoNaoLocalizado(comando);
            return Result.Failure("Aluno não existe.");
        }

        realizarInscricaoTelemetry.AlunoLocalizado();

        if (!await inscricoesRepositorio.ResponsavelExiste(comando.Responsavel))
        {
            realizarInscricaoTelemetry.ResponsavelNaoLocalizado(comando);
            return Result.Failure("Responsável não existe.");
        }

        realizarInscricaoTelemetry.ResponsavelLocalizado();

        var turma = await inscricoesRepositorio.RecuperarTurma(comando.Turma, cancellationToken);
        if (turma.HasNoValue)
        {
            realizarInscricaoTelemetry.TurmaNaoLocalizada(comando);
            return Result.Failure("Turma não existe.");
        }

        var inscricao = Inscricao.Criar(comando.Aluno, comando.Responsavel, turma.Value.Id);
        if (inscricao.IsFailure)
        {
            realizarInscricaoTelemetry.NaoFoiPossivelCriarInscricao(comando, inscricao.Error);
            return inscricao;
        }

        await inscricoesRepositorio.Adicionar(inscricao.Value, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        realizarInscricaoTelemetry.InscricaoRelizada(inscricao.Value);
        return Result.Success();
    }
}
