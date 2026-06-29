namespace KlaviyoTest.Infrastructure;

public class TenantConnectionResolver : ITenantConnectionResolver
{
    private readonly IConfiguration _config;

    public TenantConnectionResolver(IConfiguration config)
    {
        _config = config;
    }

    public string GetConnectionString(int connectionId)
    {
        return _config.GetConnectionString($"Tenant_{connectionId}")
            ?? throw new InvalidOperationException(
                $"Chaîne de connexion introuvable pour le tenant {connectionId}. " +
                $"Vérifiez la clé 'Tenant_{connectionId}' dans ConnectionStrings.");
    }
}
