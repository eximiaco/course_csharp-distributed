using M3.Desafio.Acessos.Comandos;
using M3.Desafio.SeedWork;
using M3.Desafio.SeedWork.Telemetry;
using Serilog;

namespace M3.Desafio.Acessos.Telemetria;

public class DarPermissaoAcessoParaNovaInscricaoTelemetry(
    TelemetryFactory telemetryFactory,
    OtelVariables otelVariables,
    ILogger logger) : IService<DarPermissaoAcessoParaNovaInscricaoTelemetry>, IDisposable
{
    private readonly ITelemetryService _telemetryService = telemetryFactory.Create($"{nameof(DarPermissaoAcessoParaNovaInscricaoHandler.Executar)}");
    private bool _disposed;

    public void DarPermissaoAcessoParaNovaInscricao(DarPermissaoAcessoParaNovaInscricaoCommand comando)
    {
        _telemetryService
           .AddTag(otelVariables.InscricaoId, comando.InscricaoId);

        logger.Information("Nova inscrição recebida para concedimento de acesso. | Inscricao: {inscricao}.", comando.InscricaoId);
    }

    public void PermissaoConcedidaComSucesso(Guid permissaoId)
    {
        _telemetryService
            .AddTag(otelVariables.PermissaoId, permissaoId)
        .SetSucess("Permissão [{permissao}] concedida com sucesso", new { permissao = permissaoId });
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
