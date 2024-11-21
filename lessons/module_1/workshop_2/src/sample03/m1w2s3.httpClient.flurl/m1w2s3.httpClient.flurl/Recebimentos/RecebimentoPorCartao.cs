namespace m1w2s3.httpClient.flurl.Recebimentos;

public class RecebimentoPorCartao : Recebimento
{
    public RecebimentoPorCartao(int id, decimal valor, EStatus status, DateTime criadoEm, string cartaoTruncado)
        : base(id, valor, status, criadoEm)
    {
        CartaoTruncado = cartaoTruncado;
    }

    public string CartaoTruncado { get; } = string.Empty;
}
