namespace m1w2s3.httpClient.flurl.Recebimentos.Apis;

public record Transacao(string Id, string Status, decimal Valor, ETipoTransacao TipoTransacao, string CartaoTruncado, string CodigoPix);
