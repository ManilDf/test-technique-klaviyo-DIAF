using KlaviyoTest.Infrastructure;
using KlaviyoTest.Klaviyo.Models;

namespace KlaviyoTest.Klaviyo.Mappers;

public class KlaviyoMemberMapper : IKlaviyoMemberMapper
{
    private readonly ITenantConnectionResolver _connectionResolver;
    private readonly IDbConnectionFactory _connectionFactory;

    public KlaviyoMemberMapper(
        ITenantConnectionResolver connectionResolver,
        IDbConnectionFactory connectionFactory)
    {
        _connectionResolver = connectionResolver;
        _connectionFactory = connectionFactory;
    }

    public Task<KlaviyoMember> GetMemberAsync(int connectionId, int memberId)
        => throw new NotImplementedException();
}
