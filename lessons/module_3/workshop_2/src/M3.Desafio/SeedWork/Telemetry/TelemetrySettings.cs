namespace M3.Desafio.SeedWork.Telemetry;

public record TelemetrySettings(string ServiceName, string ServiceVersion, TelemetryExporter Exporter);
public record TelemetryExporter(string Type, string Endpoint);
