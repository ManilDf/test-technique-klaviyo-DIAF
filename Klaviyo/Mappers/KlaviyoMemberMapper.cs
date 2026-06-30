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

    public async Task<KlaviyoMember> GetMemberAsync(int connectionId, int memberId)
    {
        var connectionString = _connectionResolver.GetConnectionString(connectionId);
        await using var connection = _connectionFactory.CreateConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                m.FirstName, m.LastName, m.PhoneNumber, m.BirthDate,
                m.City, m.Country, m.LoyaltyPoints, m.EnrolledAt,
                me.EmailAddress
            FROM Member m
            LEFT JOIN MemberEmail me ON me.MemberEmailId = m.MainEmailAddressId
            WHERE m.MemberId = @MemberId";

        var param = command.CreateParameter();
        param.ParameterName = "@MemberId";
        param.Value = memberId;
        command.Parameters.Add(param);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            throw new KeyNotFoundException($"Member {memberId} not found.");

        var phoneOrdinal    = reader.GetOrdinal("PhoneNumber");
        var birthOrdinal    = reader.GetOrdinal("BirthDate");
        var cityOrdinal     = reader.GetOrdinal("City");
        var countryOrdinal  = reader.GetOrdinal("Country");
        var emailOrdinal    = reader.GetOrdinal("EmailAddress");
        var enrolledOrdinal = reader.GetOrdinal("EnrolledAt");

        var city    = reader.IsDBNull(cityOrdinal)    ? null : reader.GetString(cityOrdinal);
        var country = reader.IsDBNull(countryOrdinal) ? null : reader.GetString(countryOrdinal);

        var birthDate = reader.IsDBNull(birthOrdinal)
            ? null
            : reader.GetDateTime(birthOrdinal).ToString("yyyy-MM-dd");

        var enrolledAt = DateTime.SpecifyKind(reader.GetDateTime(enrolledOrdinal), DateTimeKind.Utc)
            .ToString("o");

        return new KlaviyoMember
        {
            Data = new KlaviyoMemberData
            {
                Attributes = new KlaviyoMemberAttributes
                {
                    Email       = reader.IsDBNull(emailOrdinal) ? null : reader.GetString(emailOrdinal),
                    FirstName   = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName    = reader.GetString(reader.GetOrdinal("LastName")),
                    PhoneNumber = reader.IsDBNull(phoneOrdinal) ? null : reader.GetString(phoneOrdinal),
                    Location    = city == null && country == null
                        ? null
                        : new KlaviyoLocation { City = city, Country = country },
                    Properties  = new KlaviyoProperties
                    {
                        LoyaltyPoints = reader.GetInt32(reader.GetOrdinal("LoyaltyPoints")),
                        EnrolledAt    = enrolledAt,
                        Birthday      = birthDate
                    }
                }
            }
        };
    }
}
