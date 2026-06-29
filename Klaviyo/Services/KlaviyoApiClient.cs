using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace KlaviyoTest.Klaviyo.Services;

public class KlaviyoApiClient : IKlaviyoApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<KlaviyoApiClient> _logger;

    public KlaviyoApiClient(IHttpClientFactory factory, ILogger<KlaviyoApiClient> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> CreateOrUpdateProfileAsync(object payload, string accessToken)
    {
        var client = _factory.CreateClient(nameof(IKlaviyoApiClient));

        var request = new HttpRequestMessage(HttpMethod.Post, "api/profiles/");
        request.Headers.Authorization = new AuthenticationHeaderValue("Klaviyo-API-Key", accessToken);
        request.Content = JsonContent.Create(payload);

        _logger.LogInformation("[KlaviyoApiClient] POST /api/profiles/");

        return await client.SendAsync(request);
    }
}
