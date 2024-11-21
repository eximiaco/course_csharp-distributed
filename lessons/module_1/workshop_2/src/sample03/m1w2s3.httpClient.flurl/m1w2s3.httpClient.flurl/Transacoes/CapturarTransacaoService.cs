using CSharpFunctionalExtensions;
using m1w2s3.httpClient.flurl.Transacoes.Apis;

namespace m1w2s3.httpClient.flurl.Transacoes;

public class CapturarTransacaoService(GatewayApiFactory gatewayApiFactory, TraducaoStatusServiceFactory traducaoStatusServiceFactory)
{
    public async Task<Result> CapturarAsync(Transacao transacao, EGateway gateway, CancellationToken cancellationToken)
    {
        var gatewayService = gatewayApiFactory.CriarApiTransacao(gateway);
        var transacaoCapturada = await gatewayService.CapturarAsync(transacao, cancellationToken).ConfigureAwait(false);
        if (transacaoCapturada.IsFailure)
            return transacaoCapturada;

        var traducaoStatusService = traducaoStatusServiceFactory.Criar(gateway);
        var status = traducaoStatusService.Traduzir(transacaoCapturada.Value.Status);
        transacao.Capturar(status);
        return Result.Success();
    }
}
