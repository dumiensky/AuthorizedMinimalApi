namespace AuthorizedMinimalApi;

public interface IKeycloakTokenProxy
{
	Task<KeycloakTokenResponse> LoginAsync(string username, string password);
}