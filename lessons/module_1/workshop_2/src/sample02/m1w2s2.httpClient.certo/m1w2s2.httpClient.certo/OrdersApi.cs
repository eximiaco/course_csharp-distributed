using CSharpFunctionalExtensions;
using System.Text.Json;

namespace m1w2s2.httpClient.certo;

public class OrdersApi(HttpClient httpClient)
{
    private readonly string _url = "orders_api_uri";

    public async Task<Result<IEnumerable<OrderDto>>> GetOrdersAsync(CancellationToken cancellationToken)
    {
		try
		{
            var response = await httpClient
                .GetAsync(_url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response
                    .Content.ReadFromJsonAsync<OrderErrorDto>(cancellationToken)
                    .ConfigureAwait(false);
                return Result.Failure<IEnumerable<OrderDto>>(error!.Error);
            }

            var orders = await response
                .Content
                .ReadFromJsonAsync<IEnumerable<OrderDto>>(cancellationToken)
                .ConfigureAwait(false);
            return Result.Success(orders!);
        }
        // Se o response que está sendo desserializado não condizer com o tipo informado,
        // uma exception desse tipo será lançada
        catch (JsonException ex)
		{
            return Result.Failure<IEnumerable<OrderDto>>("Falha recuperando os pedidos.");
        }
    }
}

public record OrderDto(int Id, decimal Amount);
public record OrderErrorDto(string Error);