using KlaviyoTest.Infrastructure;
using KlaviyoTest.Klaviyo.Mappers;
using Microsoft.Data.Sqlite;
using Moq;
using Xunit;

namespace KlaviyoTest.Tests.Mappers;

public class KlaviyoMemberMapperTests : IDisposable
{
    private readonly string _connStr = $"Data Source=KlaviyoTest_{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keeper;
    private readonly Mock<ITenantConnectionResolver> _resolverMock = new();
    private readonly Mock<IDbConnectionFactory> _factoryMock = new();
    private readonly KlaviyoMemberMapper _sut;

    public KlaviyoMemberMapperTests()
    {
        _keeper = new SqliteConnection(_connStr);
        _keeper.Open();
        SetupDatabase();

        _resolverMock
            .Setup(r => r.GetConnectionString(It.IsAny<int>()))
            .Returns(_connStr);

        _factoryMock
            .Setup(f => f.CreateConnection(_connStr))
            .Returns(() => new SqliteConnection(_connStr));

        _sut = new KlaviyoMemberMapper(_resolverMock.Object, _factoryMock.Object);
    }

    private void SetupDatabase()
    {
        using var cmd = _keeper.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE Member (
                MemberId INTEGER PRIMARY KEY,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                PhoneNumber TEXT,
                BirthDate TEXT,
                City TEXT,
                Country TEXT,
                LoyaltyPoints INTEGER NOT NULL DEFAULT 0,
                MainEmailAddressId INTEGER,
                EnrolledAt TEXT NOT NULL
            );
            CREATE TABLE MemberEmail (
                MemberEmailId INTEGER PRIMARY KEY,
                MemberId INTEGER NOT NULL,
                EmailAddress TEXT NOT NULL
            );
            INSERT INTO MemberEmail VALUES (1, 1, 'diaf.manil@gmail.com');
            INSERT INTO Member VALUES (1, 'Manil', 'Diaf', '+33753490896', '2002-10-21', 'Lille', 'France', 450, 1, '2024-01-15T10:30:00Z');
            INSERT INTO Member VALUES (2, 'Jean', 'Dupont', NULL, NULL, NULL, NULL, 0, NULL, '2023-06-01T08:00:00Z');
        ";
        cmd.ExecuteNonQuery();
    }

    public void Dispose() => _keeper.Dispose();

    [Fact]
    public async Task GetMemberAsync_ReturnsMappedMember_WhenMemberExists()
    {
        var result = await _sut.GetMemberAsync(1, 1);

        Assert.Equal("diaf.manil@gmail.com", result.Data.Attributes.Email);
        Assert.Equal("Manil", result.Data.Attributes.FirstName);
        Assert.Equal("Diaf", result.Data.Attributes.LastName);
        Assert.Equal("+33753490896", result.Data.Attributes.PhoneNumber);
        Assert.Equal("2002-10-21", result.Data.Attributes.Properties.Birthday);
        Assert.Equal("Lille", result.Data.Attributes.Location?.City);
        Assert.Equal("France", result.Data.Attributes.Location?.Country);
        Assert.Equal(450, result.Data.Attributes.Properties?.LoyaltyPoints);
    }

    [Fact]
    public async Task GetMemberAsync_ThrowsKeyNotFoundException_WhenMemberNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetMemberAsync(1, 999));
    }

    [Fact]
    public async Task GetMemberAsync_HandlesNullableFields_WhenOptionalFieldsAreNull()
    {
        var result = await _sut.GetMemberAsync(1, 2);

        Assert.Null(result.Data.Attributes.PhoneNumber);
        Assert.Null(result.Data.Attributes.Properties.Birthday);
        Assert.Null(result.Data.Attributes.Location);
    }
}
