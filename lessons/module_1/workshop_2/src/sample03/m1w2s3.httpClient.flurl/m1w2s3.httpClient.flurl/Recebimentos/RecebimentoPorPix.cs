namespace m1w2s3.httpClient.flurl.Recebimentos;

public class RecebimentoPorPix : Recebimento
{
    public RecebimentoPorPix(int id, decimal valor, EStatus status, DateTime criadoEm, string codigoPix)
        : base(id, valor, status, criadoEm)
    {
        CodigoPix = codigoPix;
    }

    public string CodigoPix { get; } = string.Empty;
}
