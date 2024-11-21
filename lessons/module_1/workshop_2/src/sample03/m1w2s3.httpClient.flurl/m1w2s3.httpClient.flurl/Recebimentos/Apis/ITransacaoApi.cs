using CSharpFunctionalExtensions;

namespace m1w2s3.httpClient.flurl.Recebimentos.Apis;

public interface ITransacaoApi
{
    Task<Result<Transacao>> CapturarPorPixAsync(CapturaTransacaoContext context, CancellationToken cancellationToken);
    Task<Result<Transacao>> CapturarPorCartaoDeCreditoAsync(CapturaTransacaoContext context, CancellationToken cancellationToken);
}
