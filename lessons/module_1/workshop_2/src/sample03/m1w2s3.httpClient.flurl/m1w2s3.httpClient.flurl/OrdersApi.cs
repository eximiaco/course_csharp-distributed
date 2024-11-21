using CSharpFunctionalExtensions;
using Flurl.Http;
using Polly;

namespace m1w2s3.httpClient.flurl;

public class OrdersApi
{
    private readonly string _url = "orders_api_uri";

    public async Task<Result<IEnumerable<OrderDto>>> GetOrdersWithBearerTokenAsync(string token, CancellationToken cancellationToken)
    {
		try
		{
            var response = await _url
                .WithOAuthBearerToken(token)
                .SetQueryParam("Status", "Open")
                .SetQueryParam("Date", DateTime.Now)
                .GetAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                var error = await response.GetJsonAsync<OrderErrorDto>();
                return Result.Failure<IEnumerable<OrderDto>>(error.Error);
            }

            var orders = await response.GetJsonAsync<IEnumerable<OrderDto>>().WaitAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(orders);
        }
		catch (FlurlParsingException ex)  // Se o response que está sendo desserializado não condizer com o tipo informado, uma exception desse tipo será lançada
        {
            return Result.Failure<IEnumerable<OrderDto>>("Falha recuperando os pedidos.");
        }
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetOrdersWithApiKeyAsync(string apiKey, CancellationToken cancellationToken)
    {
        try
        {
            Dictionary<string, object> queryParams = new()
            {
                { "page", 2 },
                { "sort", "asc" },
                { "filter", "active" }
            };

            var response = await _url
                .WithHeader("x-api-key", apiKey)
                .SetQueryParams(queryParams)
                .GetAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                var error = await response.GetJsonAsync<OrderErrorDto>();
                return Result.Failure<IEnumerable<OrderDto>>(error.Error);
            }

            var orders = await response.GetJsonAsync<IEnumerable<OrderDto>>().WaitAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(orders);
        }
        catch (FlurlParsingException ex)  // Se o response que está sendo desserializado não condizer com o tipo informado, uma exception desse tipo será lançada
        {
            return Result.Failure<IEnumerable<OrderDto>>("Falha recuperando os pedidos.");
        }
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetOrdersWithClientCredentialsAsync(string clientId, string clientSecret, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _url
                .WithBasicAuth(clientId, clientSecret)
                .GetAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                var error = await response.GetJsonAsync<OrderErrorDto>();
                return Result.Failure<IEnumerable<OrderDto>>(error.Error);
            }

            var orders = await response.GetJsonAsync<IEnumerable<OrderDto>>().WaitAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(orders);
        }
        catch (FlurlParsingException ex)  // Se o response que está sendo desserializado não condizer com o tipo informado, uma exception desse tipo será lançada
        {
            return Result.Failure<IEnumerable<OrderDto>>("Falha recuperando os pedidos.");
        }
    }

    public async Task<Result<int>> CreateOrderAsync(CreateOrderRequest request, string token, CancellationToken cancellationToken)
    {
        var response = await HttpRetryPolicy.AsyncRetryPolicy.ExecuteAndCaptureAsync(async () =>
        {
            return await _url
                .WithOAuthBearerToken(token)
                .PostJsonAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (response.Outcome == OutcomeType.Failure)
        {
            var exception = response.FinalException as FlurlHttpException;

            // Retornamos todo o response (seja JSON, XML ou HTML) como string. Poderia também ter desserializado num objeto
            return Result.Failure<int>(await exception!.GetResponseStringAsync().ConfigureAwait(false));
        }

        var orderCreated = await response.Result.GetJsonAsync<CreateOrderResponse>().ConfigureAwait(false);
        return orderCreated.Id;
    }
}

public record OrderDto(int Id, decimal Amount);
public record OrderErrorDto(string Error);
public record CreateOrderRequest(decimal Amount, DateTime CreatedAt, string ClientName);
public record CreateOrderResponse(int Id);