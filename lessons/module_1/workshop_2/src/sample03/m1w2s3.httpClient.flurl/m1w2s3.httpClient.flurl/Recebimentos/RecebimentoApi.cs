using CSharpFunctionalExtensions;
using m1w2s3.httpClient.flurl.Recebimentos.Apis;

namespace m1w2s3.httpClient.flurl.Recebimentos;

public class RecebimentoApi(ITransacaoApi transacaoApi)
{
    public async Task<Result<Transacao>> CapturarAsync(CapturaTransacaoContext context, CancellationToken cancellationToken)
    {
        if (context.TipoTransacao == ETipoTransacao.CartaoDeCredito)
            return await transacaoApi.CapturarPorCartaoDeCreditoAsync(context, cancellationToken).ConfigureAwait(false);
        return await transacaoApi.CapturarPorPixAsync(context, cancellationToken).ConfigureAwait(false);
    }
}
