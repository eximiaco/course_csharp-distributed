namespace m1w2s3.httpClient.flurl.Recebimentos.Apis;

public record EbanxSettings
{
    public string UriCapturaPix { get; set; } = string.Empty;
    public string UriCapturaCartaoDeCredito { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
