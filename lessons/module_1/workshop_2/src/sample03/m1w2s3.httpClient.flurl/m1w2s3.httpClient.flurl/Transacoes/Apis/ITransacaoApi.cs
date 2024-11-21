using CSharpFunctionalExtensions;

namespace m1w2s3.httpClient.flurl.Transacoes.Apis;

public interface ITransacaoApi
{
    Task<Result<TransacaoDto>> CapturarAsync(Transacao transacao, CancellationToken cancellationToken);
}
