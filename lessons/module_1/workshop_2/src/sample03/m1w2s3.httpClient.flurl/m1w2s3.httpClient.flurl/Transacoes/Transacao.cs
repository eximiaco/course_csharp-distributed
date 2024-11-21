namespace m1w2s3.httpClient.flurl.Transacoes;

public class Transacao
{
    public Transacao(EGateway gateway, decimal amount, EStatus status, DateTime createdAt, Payer payer)
    {
        Gateway = gateway;
        Amount = amount;
        Status = status;
        CreatedAt = createdAt;
        Payer = payer;
    }

    public EGateway Gateway { get; }
    public decimal Amount { get; }
    public EStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public Payer Payer { get; } = null!;

    public void Capturar(EStatus status)
    {
        Status = status;
    }
}
