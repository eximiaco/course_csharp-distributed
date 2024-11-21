namespace m1w2s3.httpClient.flurl.Transacoes.Apis;

public class TraducaoStatusServiceFactory(IServiceProvider serviceProvider)
{
    public ITraducaoStatus Criar(EGateway gateway)
    {
        return gateway switch
        {
            EGateway.Ebanx => serviceProvider.GetRequiredService<TraducaoStatusEbanxService>(),
            EGateway.PagarMe => serviceProvider.GetRequiredService<TraducaoStatusPagarMeService>(),
            _ => throw new ArgumentOutOfRangeException(nameof(gateway), "Gateway não configurado.")
        };
    }
}
