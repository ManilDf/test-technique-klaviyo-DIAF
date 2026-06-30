using KlaviyoTest.Klaviyo.Mappers;
using Microsoft.Extensions.Logging;

namespace KlaviyoTest.Klaviyo.Services;

public class KlaviyoMemberSyncService : IKlaviyoMemberSyncService
{
    private readonly IKlaviyoMemberMapper _mapper;
    private readonly IKlaviyoApiClient _apiClient;
    private readonly ILogger<KlaviyoMemberSyncService> _logger;

    public KlaviyoMemberSyncService(
        IKlaviyoMemberMapper mapper,
        IKlaviyoApiClient apiClient,
        ILogger<KlaviyoMemberSyncService> logger)
    {
        _mapper = mapper;
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task SyncMemberAsync(int connectionId, int memberId, string accessToken)
    {
        var member = await _mapper.GetMemberAsync(connectionId, memberId);

        var response = await _apiClient.CreateOrUpdateProfileAsync(member, accessToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Klaviyo API error {(int)response.StatusCode}: {body}");
        }

        _logger.LogInformation("Member {MemberId} synced to Klaviyo.", memberId);
    }
}
