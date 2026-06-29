namespace KlaviyoTest.Klaviyo.Services;

public interface IKlaviyoMemberSyncService
{
    Task SyncMemberAsync(int connectionId, int memberId, string accessToken);
}
