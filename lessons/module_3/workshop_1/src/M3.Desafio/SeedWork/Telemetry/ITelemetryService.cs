namespace M3.Desafio.SeedWork.Telemetry;

public interface ITelemetryService : IDisposable
{
    ITelemetryService AddTag(string tag, object? value);
    ITelemetryService AddEvent(string message);
    ITelemetryService AddLogInformationAndEvent(string messageTemplate, object? propertyValues);
    ITelemetryService AddWarningEvent(string messageTemplate, object? propertyValues);
    ITelemetryService AddException(string error, Exception exception);
    void SetSucess(string messageTemplate, object? propertyValues);
    void SetError(string messageTemplate, object? propertyValues);
    void AddWarning(string messageTemplate, params object?[]? propertyValues);
    void AddInformation(string messageTemplate, params object?[]? propertyValues);
}
