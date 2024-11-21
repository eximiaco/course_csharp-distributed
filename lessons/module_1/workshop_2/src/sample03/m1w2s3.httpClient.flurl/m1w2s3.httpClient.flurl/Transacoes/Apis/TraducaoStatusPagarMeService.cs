namespace m1w2s3.httpClient.flurl.Transacoes.Apis;

public class TraducaoStatusPagarMeService : ITraducaoStatus
{
    public EStatus Traduzir(string status)
    {
        return status switch
        {
            "1" => EStatus.Confirmed,
            "2" => EStatus.Pending,
            "3" => EStatus.Failure,
            "4" => EStatus.Canceled,
            "9" => EStatus.Denied,
            _ => EStatus.Indefinido
        };
    }
}
