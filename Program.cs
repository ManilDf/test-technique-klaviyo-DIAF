using KlaviyoTest.Infrastructure;
using KlaviyoTest.Klaviyo.Mappers;
using KlaviyoTest.Klaviyo.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure ─────────────────────────────────────────────────────────
// Ne pas modifier ce bloc.

builder.Services.AddSingleton<ITenantConnectionResolver, TenantConnectionResolver>();

builder.Services.AddHttpClient(nameof(IKlaviyoApiClient), client =>
{
    client.BaseAddress = new Uri("https://a.klaviyo.com/");
    client.DefaultRequestHeaders.Add("revision", "2024-02-15");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddScoped<IKlaviyoApiClient, KlaviyoApiClient>();

// ── Vos services ───────────────────────────────────────────────────────────

builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IKlaviyoMemberMapper, KlaviyoMemberMapper>();
builder.Services.AddScoped<IKlaviyoMemberSyncService, KlaviyoMemberSyncService>();

// ───────────────────────────────────────────────────────────────────────────

var app = builder.Build();

app.MapGet("/", () => "Klaviyo Test API — OK");

app.MapPost("/sync", async (SyncRequest req, IKlaviyoMemberSyncService syncService) =>
{
    try
    {
        await syncService.SyncMemberAsync(req.ConnectionId, req.MemberId, req.AccessToken);
        return Results.Ok(new { message = $"Member {req.MemberId} synced successfully." });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();

record SyncRequest(int ConnectionId, int MemberId, string AccessToken);
