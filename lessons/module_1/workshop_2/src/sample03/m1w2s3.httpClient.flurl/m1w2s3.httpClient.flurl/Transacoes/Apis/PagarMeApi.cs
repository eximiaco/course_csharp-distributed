using CSharpFunctionalExtensions;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Polly;

namespace m1w2s3.httpClient.flurl.Transacoes.Apis;

public class PagarMeApi(IOptions<PagarMeSettings> pagarMeSettings) : ITransacaoApi
{
    private readonly PagarMeSettings _pagarMeSettings = pagarMeSettings.Value;

    public async Task<Result<TransacaoDto>> CapturarAsync(Transacao transacao, CancellationToken cancellationToken)
    {
        var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            return await _pagarMeSettings.UriCaptura
                .WithHeader("ApiKey", _pagarMeSettings.ApiKey)
                .PostJsonAsync(CapturaTransacaoPagarMeRequest.Criar(transacao), HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (response.Outcome == OutcomeType.Failure)
        {
            var exception = response.FinalException as FlurlHttpException;
            return Result.Failure<TransacaoDto>(await exception!.GetResponseStringAsync().ConfigureAwait(false));
        }

        var result = await response.Result.GetJsonAsync<CapturaTransacaoPagarMeResponse>().ConfigureAwait(false);
        return new TransacaoDto(result.Info.Id, result.Result.Status, result.Info.Amount);
    }
}

public record CapturaTransacaoPagarMeRequest
{
    // Apenas para demonstração. Aqui teria o contrato do gateway
    public static CapturaTransacaoPagarMeRequest Criar(Transacao transacao)
        => null!;
}

public record CapturaTransacaoPagarMeResponse(CapturaTransacaoPagarMeInfoDto Info, CapturaTransacaoPagarMeResultDto Result);
public record CapturaTransacaoPagarMeInfoDto(string Id, decimal Amount);
public record CapturaTransacaoPagarMeResultDto(string Status);