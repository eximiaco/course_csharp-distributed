using CSharpFunctionalExtensions;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Polly;

namespace m1w2s3.httpClient.flurl.Transacoes.Apis;

public class EbanxApi(IOptions<EbanxSettings> options) : ITransacaoApi
{
    private readonly EbanxSettings _settings = options.Value;

    public async Task<Result<TransacaoDto>> CapturarAsync(Transacao transacao, CancellationToken cancellationToken)
    {
        var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            return await _settings.UriCaptura
                .WithHeader("ApiKey", _settings.ApiKey)
                .PostJsonAsync(CapturaTransacaoEbanxRequest.Criar(transacao), HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (response.Outcome == OutcomeType.Failure)
        {
            var exception = response.FinalException as FlurlHttpException;
            return Result.Failure<TransacaoDto>(await exception!.GetResponseStringAsync().ConfigureAwait(false));
        }

        var result = await response.Result.GetJsonAsync<CapturaTransacaoEbanxResponse>().ConfigureAwait(false);
        return new TransacaoDto(result.Info.Result.Id, result.Info.Result.Status, result.Info.Result.Amount);
    }
}

public record CapturaTransacaoEbanxRequest
{
    // Apenas para demonstração. Aqui teria o contrato do gateway
    public static CapturaTransacaoEbanxRequest Criar(Transacao transacao)
        => null!;
}

public record CapturaTransacaoEbanxResponse(CapturaTransacaoEbanxInfoDto Info);
public record CapturaTransacaoEbanxInfoDto(CapturaTransacaoEbanxResultDto Result);
public record CapturaTransacaoEbanxResultDto(string Status, decimal Amount, string Id);
