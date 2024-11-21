namespace m1w2s3.httpClient.flurl.Transacoes.Apis;

public class GatewayApiFactory(IServiceProvider serviceProvider)
{
    public ITransacaoApi CriarApiTransacao(EGateway gateway)
    {
        return gateway switch
        {
            EGateway.Ebanx => serviceProvider.GetRequiredService<EbanxApi>(),
            EGateway.PagarMe => serviceProvider.GetRequiredService<PagarMeApi>(),
            _ => throw new ArgumentOutOfRangeException(nameof(gateway), "Gateway não configurado."),
        };
    }
}
