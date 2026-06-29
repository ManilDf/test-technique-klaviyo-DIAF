using KlaviyoTest.Klaviyo.Models;

namespace KlaviyoTest.Klaviyo.Mappers;

public interface IKlaviyoMemberMapper
{
    Task<KlaviyoMember> GetMemberAsync(int connectionId, int memberId);
}
