namespace KlaviyoTest.Infrastructure;

public interface ITenantConnectionResolver
{
    /// <summary>
    /// Retourne la chaîne de connexion SQL Server pour le tenant donné.
    /// Lève InvalidOperationException si le connectionId est introuvable dans la configuration.
    /// </summary>
    string GetConnectionString(int connectionId);
}
