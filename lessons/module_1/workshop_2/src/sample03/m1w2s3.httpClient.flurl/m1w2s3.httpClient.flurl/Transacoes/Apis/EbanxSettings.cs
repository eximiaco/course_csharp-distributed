namespace m1w2s3.httpClient.flurl.Transacoes.Apis;

public record EbanxSettings
{
    public string UriCaptura { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
