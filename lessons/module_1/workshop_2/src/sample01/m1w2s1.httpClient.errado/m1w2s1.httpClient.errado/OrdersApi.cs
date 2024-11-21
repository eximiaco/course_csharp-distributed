using Newtonsoft.Json;

namespace m1w2s1.httpClient.errado;

public class OrdersApi
{
    private readonly string _url = "orders_api_uri";

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(_url, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(json)!;
    }
}

public record OrderDto(int Id, decimal Amount);
