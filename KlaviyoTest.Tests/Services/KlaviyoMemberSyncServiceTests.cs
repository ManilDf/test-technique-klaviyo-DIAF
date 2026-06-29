using KlaviyoTest.Klaviyo.Mappers;
using KlaviyoTest.Klaviyo.Models;
using KlaviyoTest.Klaviyo.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

namespace KlaviyoTest.Tests.Services;

public class KlaviyoMemberSyncServiceTests
{
    private readonly Mock<IKlaviyoMemberMapper> _mapperMock = new();
    private readonly Mock<IKlaviyoApiClient> _apiClientMock = new();
    private readonly Mock<ILogger<KlaviyoMemberSyncService>> _loggerMock = new();
    private readonly KlaviyoMemberSyncService _sut;

    public KlaviyoMemberSyncServiceTests()
    {
        _sut = new KlaviyoMemberSyncService(_mapperMock.Object, _apiClientMock.Object, _loggerMock.Object);
    }

    private static KlaviyoMember SampleMember() => new()
    {
        Data = new KlaviyoMemberData
        {
            Attributes = new KlaviyoMemberAttributes
            {
                Email = "jean.dupont@email.com",
                FirstName = "Jean",
                LastName = "Dupont",
                Properties = new KlaviyoProperties
                {
                    LoyaltyPoints = 450,
                    EnrolledAt = "2024-01-15T10:30:00Z"
                }
            }
        }
    };

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    public async Task SyncMemberAsync_DoesNotThrow_WhenApiReturnsSuccess(HttpStatusCode statusCode)
    {
        _mapperMock.Setup(m => m.GetMemberAsync(1, 42)).ReturnsAsync(SampleMember());
        _apiClientMock
            .Setup(a => a.CreateOrUpdateProfileAsync(It.IsAny<object>(), "token"))
            .ReturnsAsync(new HttpResponseMessage(statusCode));

        await _sut.SyncMemberAsync(1, 42, "token");
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task SyncMemberAsync_ThrowsInvalidOperationException_WhenApiReturnsError(HttpStatusCode statusCode)
    {
        _mapperMock.Setup(m => m.GetMemberAsync(1, 42)).ReturnsAsync(SampleMember());
        _apiClientMock
            .Setup(a => a.CreateOrUpdateProfileAsync(It.IsAny<object>(), "token"))
            .ReturnsAsync(new HttpResponseMessage(statusCode) { Content = new StringContent("API error") });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.SyncMemberAsync(1, 42, "token"));
    }

    [Fact]
    public async Task SyncMemberAsync_PropagatesKeyNotFoundException_WhenMemberNotFound()
    {
        _mapperMock
            .Setup(m => m.GetMemberAsync(1, 999))
            .ThrowsAsync(new KeyNotFoundException("Member 999 not found."));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SyncMemberAsync(1, 999, "token"));
    }

    [Fact]
    public async Task SyncMemberAsync_CallsApiWithCorrectToken()
    {
        const string token = "pk_my_secret_token";
        _mapperMock.Setup(m => m.GetMemberAsync(1, 42)).ReturnsAsync(SampleMember());
        _apiClientMock
            .Setup(a => a.CreateOrUpdateProfileAsync(It.IsAny<object>(), token))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        await _sut.SyncMemberAsync(1, 42, token);

        _apiClientMock.Verify(a => a.CreateOrUpdateProfileAsync(It.IsAny<object>(), token), Times.Once);
    }
}
