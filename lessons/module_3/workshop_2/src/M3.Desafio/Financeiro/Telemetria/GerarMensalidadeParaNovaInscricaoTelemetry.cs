using M3.Desafio.Financeiro.Comandos;
using M3.Desafio.SeedWork;
using M3.Desafio.SeedWork.Telemetry;
using Serilog;

namespace M3.Desafio.Financeiro.Telemetria;

public class GerarMensalidadeParaNovaInscricaoTelemetry(
    TelemetryFactory telemetryFactory,
    OtelVariables otelVariables,
    ILogger logger) : IService<GerarMensalidadeParaNovaInscricaoTelemetry>, IDisposable
{
    private readonly ITelemetryService _telemetryService = telemetryFactory.Create($"{nameof(GerarMensalidadesParaNovaInscricaoHandler.Executar)}");
    private bool _disposed;

    public void NovaInscricaoParaGeracaoMensalidadeRecebida(GerarMensalidadesParaNovaInscricaoComando comando)
    {
        _telemetryService
           .AddTag(otelVariables.ResponsavelId, comando.Responsavel)
           .AddTag(otelVariables.InscricaoId, comando.InscricaoId)
           .AddEvent("Nova inscrição recebida.");

        logger.Information("Nova inscrição recebida. | Inscricao: {inscricao}. Responsavel: {responsavel}.", comando.InscricaoId, comando.Responsavel);
    }

    public void MensalidadesGeradasComSucesso(IEnumerable<Mensalidade> mensalidades)
    {
        _telemetryService
            .AddTag(otelVariables.MensalidadeIds, mensalidades.Select(m => m.Id))
        .SetSucess("Inscrição realizada para as mensalidades [{mensalidades}]", new { mensalidades = mensalidades.Select(m => m.Id) });
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
