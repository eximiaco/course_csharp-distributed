using CSharpFunctionalExtensions;
using m1w2s3.httpClient.flurl.Recebimentos.Apis;

namespace m1w2s3.httpClient.flurl.Recebimentos;

public class CapturarTransacaoService(EbanxApi ebanxApi)
{
    public async Task<Result<Transacao>> CapturarAsync(CapturaTransacaoContext context, CancellationToken cancellationToken)
    {
        if (context.TipoTransacao == ETipoTransacao.CartaoDeCredito)
            return await ebanxApi.CapturarPorCartaoDeCreditoAsync(context, cancellationToken).ConfigureAwait(false);
        return await ebanxApi.CapturarPorPixAsync(context, cancellationToken).ConfigureAwait(false);
    }
}
