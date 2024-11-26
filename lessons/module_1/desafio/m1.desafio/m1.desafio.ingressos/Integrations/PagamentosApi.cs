using CSharpFunctionalExtensions;
using Flurl.Http;
using Polly;

namespace m1.desafio.ingressos.Integrations;

public class PagamentosApi(IConfiguration configuration)
{
    private readonly string _uri = configuration.GetValue<string>("PagamentoApiUri")!;

    public async Task<Result> PagarAsync(RealizarPagamentoRequest request, CancellationToken cancellationToken)
    {
        var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            return await _uri
                .PostJsonAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (response.Outcome == OutcomeType.Failure)
        {
            var exception = response.FinalException as FlurlHttpException;
            return Result.Failure(await exception!.GetResponseStringAsync().ConfigureAwait(false));
        }

        var result = await response.Result.GetJsonAsync<RealizarPagamentoResponse>().ConfigureAwait(false);
        return Result.Success();
    }
}

public record RealizarPagamentoRequest;
public record RealizarPagamentoResponse(int PagamentoId);
