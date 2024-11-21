using Flurl.Http.Testing;
using m1w2s3.httpClient.flurl;

namespace Client.Flurl.Tests;

public class OrdersApiTests
{
    [Test]
    public async Task GetOrdersWithBearerTokenAsync_SuccessfulResponse_ReturnsOrders()
    {
        // Arrange
        var api = new OrdersApi();
        var token = "valid_token";
        var expectedOrders = new List<OrderDto>
        {
            new OrderDto(1, 100.0m),
            new OrderDto(2, 200.0m)
        };

        using var httpTest = new HttpTest();
        httpTest.RespondWithJson(expectedOrders);

        // Act
        var result = await api.GetOrdersWithBearerTokenAsync(token, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.That(result.Value, Is.EqualTo(expectedOrders));
        httpTest.ShouldHaveCalled("orders_api_uri")
                .WithOAuthBearerToken(token)
                .WithVerb(HttpMethod.Get)
                .WithQueryParam("Status", "Open")
                .WithQueryParam("Date")
                .Times(1);
    }

    [Test]
    public async Task GetOrdersWithApiKeyAsync_SuccessfulResponse_ReturnsOrders()
    {
        // Arrange
        var api = new OrdersApi();
        var apiKey = "valid_api_key";
        var expectedOrders = new List<OrderDto>
        {
            new OrderDto(1, 100.0m),
            new OrderDto(2, 200.0m)
        };

        using var httpTest = new HttpTest();
        httpTest.RespondWithJson(expectedOrders);

        // Act
        var result = await api.GetOrdersWithApiKeyAsync(apiKey, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.That(result.Value, Is.EqualTo(expectedOrders));
        httpTest.ShouldHaveCalled("orders_api_uri")
                .WithHeader("x-api-key", apiKey)
                .WithVerb(HttpMethod.Get)
                .WithQueryParam("page", 2)
                .WithQueryParam("sort", "asc")
                .WithQueryParam("filter", "active")
                .Times(1);
    }

    [Test]
    public async Task GetOrdersWithClientCredentialsAsync_SuccessfulResponse_ReturnsOrders()
    {
        // Arrange
        var api = new OrdersApi();
        var clientId = "client_id";
        var clientSecret = "client_secret";
        var expectedOrders = new List<OrderDto>
        {
            new OrderDto(1, 100.0m),
            new OrderDto(2, 200.0m)
        };

        using var httpTest = new HttpTest();
        httpTest.RespondWithJson(expectedOrders);

        // Act
        var result = await api.GetOrdersWithClientCredentialsAsync(clientId, clientSecret, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.That(result.Value, Is.EqualTo(expectedOrders));
        httpTest.ShouldHaveCalled("orders_api_uri")
                .WithBasicAuth(clientId, clientSecret)
                .WithVerb(HttpMethod.Get)
                .Times(1);
    }

    [Test]
    public async Task CreateOrderAsync_SuccessfulResponse_ReturnsOrderId()
    {
        // Arrange
        var api = new OrdersApi();
        var token = "valid_token";
        var request = new CreateOrderRequest(150.0m, DateTime.UtcNow, "Cliente X");
        var expectedResponse = new CreateOrderResponse(1);

        using var httpTest = new HttpTest();
        httpTest.RespondWithJson(expectedResponse);

        // Act
        var result = await api.CreateOrderAsync(request, token, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.That(result.Value, Is.EqualTo(expectedResponse.Id));
        httpTest.ShouldHaveCalled("orders_api_uri")
                .WithOAuthBearerToken(token)
                .WithVerb(HttpMethod.Post)
                .WithRequestJson(request)
                .Times(1);
    }

}