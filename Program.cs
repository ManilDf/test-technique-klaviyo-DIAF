using KlaviyoTest.Infrastructure;
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
// Décommentez ces lignes après avoir créé vos classes.

// builder.Services.AddScoped<IKlaviyoMemberMapper, KlaviyoMemberMapper>();
// builder.Services.AddScoped<IKlaviyoMemberSyncService, KlaviyoMemberSyncService>();

// ───────────────────────────────────────────────────────────────────────────

var app = builder.Build();

app.MapGet("/", () => "Klaviyo Test API — OK");

app.Run();
