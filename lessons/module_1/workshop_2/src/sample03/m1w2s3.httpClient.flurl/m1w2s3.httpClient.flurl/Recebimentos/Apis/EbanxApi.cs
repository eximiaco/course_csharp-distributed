using CSharpFunctionalExtensions;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Polly;

namespace m1w2s3.httpClient.flurl.Recebimentos.Apis;

public class EbanxApi(IOptions<EbanxSettings> options) : ITransacaoApi
{
    private readonly EbanxSettings _settings = options.Value;

    public async Task<Result<Transacao>> CapturarPorCartaoDeCreditoAsync(CapturaTransacaoContext context, CancellationToken cancellationToken)
    {
        var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            return await _settings.UriCapturaCartaoDeCredito
                .WithHeader("ApiKey", _settings.ApiKey)
                .PostJsonAsync(CapturaTransacaoEbanxRequest.Criar(context), HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (response.Outcome == OutcomeType.Failure)
        {
            var exception = response.FinalException as FlurlHttpException;
            return Result.Failure<Transacao>(await exception!.GetResponseStringAsync().ConfigureAwait(false));
        }

        var result = await response.Result.GetJsonAsync<CapturaTransacaoEbanxResponse>().ConfigureAwait(false);
        return new Transacao(result.Info.Result.Id, result.Info.Result.Status, result.Info.Result.Amount, ETipoTransacao.CartaoDeCredito, "", "");
    }

    public async Task<Result<Transacao>> CapturarPorPixAsync(CapturaTransacaoContext context, CancellationToken cancellationToken)
    {
        var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            return await _settings.UriCapturaPix
                .WithHeader("ApiKey", _settings.ApiKey)
                .PostJsonAsync(CapturaTransacaoEbanxRequest.Criar(context), HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (response.Outcome == OutcomeType.Failure)
        {
            var exception = response.FinalException as FlurlHttpException;
            return Result.Failure<Transacao>(await exception!.GetResponseStringAsync().ConfigureAwait(false));
        }

        var result = await response.Result.GetJsonAsync<CapturaTransacaoEbanxResponse>().ConfigureAwait(false);
        return new Transacao(result.Info.Result.Id, result.Info.Result.Status, result.Info.Result.Amount, ETipoTransacao.Pix, "", "");
    }
}

public record CapturaTransacaoEbanxRequest
{
    // Apenas para demonstração. Aqui teria o contrato do gateway
    public static CapturaTransacaoEbanxRequest Criar(CapturaTransacaoContext context)
        => null!;
}

public record CapturaTransacaoEbanxResponse(CapturaTransacaoEbanxInfoDto Info);
public record CapturaTransacaoEbanxInfoDto(CapturaTransacaoEbanxResultDto Result);
public record CapturaTransacaoEbanxResultDto(string Status, decimal Amount, string Id);
