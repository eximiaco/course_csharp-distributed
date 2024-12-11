using System.Diagnostics;

namespace M3.Desafio.SeedWork.Telemetry;

public sealed class OtelTracingService
{
    public ActivitySource ActivitySource { get; }

    public OtelTracingService(TelemetrySettings? settings) => ActivitySource = new(settings!.ServiceName);
}
