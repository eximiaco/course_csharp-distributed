using m1.desafio.ingressos.Integrations;

namespace m1.desafio.ingressos.Domain;

public class VenderIngressoCommandHandler(PagamentosApi pagamentosApi)
{
    public async Task VenderAsync(VenderIngressoCommand comando, CancellationToken cancellationToken)
    {
        // Realiza a venda do ingresso

        // Faz a chamada para a API de pagamento de forma síncrona
        await pagamentosApi.PagarAsync(new RealizarPagamentoRequest(), cancellationToken).ConfigureAwait(false);
    }
}
