using M3.Desafio.Inscricoes.Comandos;
using M3.Desafio.SeedWork;
using M3.Desafio.SeedWork.Telemetry;
using Serilog;

namespace M3.Desafio.Inscricoes.Telemetria;

public class RealizarInscricaoTelemetry(
    TelemetryFactory telemetryFactory,
    InscricoesOtelMetrics otelMetrics,
    OtelVariables otelVariables,
    ILogger logger) : IService<RealizarInscricaoTelemetry>, IDisposable
{
    private readonly ITelemetryService _telemetryService = telemetryFactory.Create($"{nameof(RealizarInscricaoHandler.Executar)}");
    private bool _disposed;

    public void NovaInscricaoRecebida(RealizarInscricaoComando comando)
    {
        _telemetryService
            .AddTag(otelVariables.AlunoId, comando.Aluno)
            .AddTag(otelVariables.ResponsavelId, comando.Responsavel)
            .AddTag(otelVariables.TurmaId, comando.Turma)
            .AddEvent("Nova inscrição recebida.");

        logger.Information("Nova inscrição recebida. | Aluno: {aluno}. Responsavel: {responsavel}. Turma: {turma}");
    }

    public void AlunoNaoLocalizado(RealizarInscricaoComando comando)
    {
        _telemetryService.SetError("Aluno não foi localizado", new { });
        otelMetrics.InscricaoNaoRealizada(comando.Turma);
        logger.Error("Aluno não localizado. | Aluno: {aluno}.", comando.Aluno);
        Dispose(true);
    }

    public void AlunoLocalizado()
    {
        _telemetryService.AddEvent("Aluno localizado");
    }

    public void ResponsavelNaoLocalizado(RealizarInscricaoComando comando)
    {
        _telemetryService.SetError("Responsável não foi localizado", new { });
        otelMetrics.InscricaoNaoRealizada(comando.Turma);
        logger.Error("Responsável não localizado. | Responsavel: {responsavel}.", comando.Responsavel);
        Dispose(true);
    }

    public void ResponsavelLocalizado()
    {
        _telemetryService.AddEvent("Responsável localizado");
    }

    public void TurmaNaoLocalizada(RealizarInscricaoComando comando)
    {
        _telemetryService.SetError("Turma não foi localizada", new { });
        otelMetrics.InscricaoNaoRealizada(comando.Turma);
        logger.Error("Turma não localizada. | Turma: {turma}.", comando.Turma);
        Dispose(true);
    }

    public void TurmaLocalizada(Turma turma)
    {
        _telemetryService
            .AddTag(otelVariables.QuantidadeVagas, turma.Vagas)
            .AddEvent("Turma localizada");
    }

    public void NaoFoiPossivelCriarInscricao(RealizarInscricaoComando comando, string error)
    {
        _telemetryService.SetError("Falha ao criar inscricao [{error}]", new { error });
        otelMetrics.InscricaoNaoRealizada(comando.Turma);
        Dispose(true);
    }

    public void InscricaoRelizada(Inscricao inscricao)
    {
        _telemetryService
            .AddTag(otelVariables.InscricaoId, inscricao.Id)
            .SetSucess("Inscrição realizada", new { });
        otelMetrics.InscricaoRealizada(inscricao.Turma);
        Dispose(true);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            _telemetryService!.Dispose();
        }
        _disposed = true;
    }
}
