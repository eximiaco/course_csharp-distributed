namespace m1w2s3.httpClient.flurl.Transacoes.Apis;

public class TraducaoStatusEbanxService : ITraducaoStatus
{
    public EStatus Traduzir(string status)
    {
        return status switch
        {
            "OK" => EStatus.Confirmed,
            "PENDING" => EStatus.Pending,
            "NOK" => EStatus.Denied,
            _ => EStatus.Indefinido
        };
    }
}
