using System.Diagnostics.Metrics;

namespace M3.Desafio.Inscricoes.Telemetria;

public class InscricoesOtelMetrics
{
    public InscricoesOtelMetrics(string name = "inscricoes.metrics")
    {
        Name = name;
        var meter = new Meter(Name);
        InscricoesCount = meter.CreateCounter<int>("inscricoes.metrics.total", "inscricao");
        InscricoesErroCount = meter.CreateCounter<int>("inscricoes.metrics.error", "inscricao");
    }

    public string Name { get; }

    private Counter<int> InscricoesCount { get; }
    private Counter<int> InscricoesErroCount { get; }

    public void InscricaoNaoRealizada(int turma)
    {
        InscricoesErroCount.Add(1,
            new KeyValuePair<string, object?>("turma", turma));
    }

    public void InscricaoRealizada(int turmaId)
    {
        InscricoesCount.Add(1,
            new KeyValuePair<string, object?>("turma", turmaId));
    }
}
