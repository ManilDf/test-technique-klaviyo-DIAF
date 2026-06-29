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

    public Task SyncMemberAsync(int connectionId, int memberId, string accessToken)
        => throw new NotImplementedException();
}
