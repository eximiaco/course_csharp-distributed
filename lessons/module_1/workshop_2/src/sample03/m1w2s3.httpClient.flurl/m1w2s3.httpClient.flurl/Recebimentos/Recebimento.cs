using m1w2s3.httpClient.flurl.Recebimentos.Apis;

namespace m1w2s3.httpClient.flurl.Recebimentos;

public abstract class Recebimento
{
    protected Recebimento(int id, decimal valor, EStatus status, DateTime criadoEm)
    {
        Id = id;
        Valor = valor;
        Status = status;
        CriadoEm = criadoEm;
    }

    public int Id { get; }
    public decimal Valor { get; }
    public EStatus Status { get; private set; }
    public DateTime CriadoEm { get; }

    public static Recebimento Criar(Transacao transacao)
    {
        if (transacao.TipoTransacao == ETipoTransacao.Pix)
            return new RecebimentoPorPix(0, transacao.Valor, EStatus.Pending, DateTime.Now, transacao.CodigoPix);
        return new RecebimentoPorCartao(0, transacao.Valor, EStatus.Pending, DateTime.Now, transacao.CartaoTruncado);
    }
}
