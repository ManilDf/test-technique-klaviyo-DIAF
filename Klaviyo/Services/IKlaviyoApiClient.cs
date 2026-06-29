namespace KlaviyoTest.Klaviyo.Services;

public interface IKlaviyoApiClient
{
    /// <summary>
    /// Crée ou met à jour un profil dans Klaviyo.
    /// POST /api/profiles/ — les headers revision et Content-Type sont déjà gérés.
    /// Le header Authorization (Klaviyo-API-Key) est ajouté avec l'accessToken fourni.
    /// </summary>
    Task<HttpResponseMessage> CreateOrUpdateProfileAsync(object payload, string accessToken);
}
