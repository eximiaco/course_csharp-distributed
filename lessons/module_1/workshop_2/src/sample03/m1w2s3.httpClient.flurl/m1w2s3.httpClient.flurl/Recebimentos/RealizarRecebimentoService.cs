using CSharpFunctionalExtensions;
using m1w2s3.httpClient.flurl.Recebimentos.Apis;

namespace m1w2s3.httpClient.flurl.Recebimentos;

public class RealizarRecebimentoService(CapturarTransacaoService capturarTransacaoService)
{
    public async Task<Result<Recebimento>> CapturarAsync(CapturaTransacaoContext capturaTransacaoContext, CancellationToken cancellationToken)
    {
        // Camada Sintática: não importa se a transação foi por cartão de crédito ou pix, o objeto retornado é o mesmo
        var transacaoCapturada = await capturarTransacaoService.CapturarAsync(capturaTransacaoContext, cancellationToken).ConfigureAwait(false);
        if (transacaoCapturada.IsFailure)
            return Result.Failure<Recebimento>(transacaoCapturada.Error);

        // Camada Semântica: transforma a transação num recebimento
        return Recebimento.Criar(transacaoCapturada.Value);
    }
}
