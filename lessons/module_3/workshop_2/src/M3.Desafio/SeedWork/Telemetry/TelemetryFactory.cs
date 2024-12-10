using Serilog;

namespace M3.Desafio.SeedWork.Telemetry;

public class TelemetryFactory(
    OtelTracingService otelTracingService,
    ILogger logger)
{
    public ITelemetryService Create(string spanName)
    {
        return new TelemetryService(
            otelTracingService.ActivitySource.StartActivity(spanName),
            logger);
    }
}
