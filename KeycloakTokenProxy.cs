using System.Text.Json;

namespace AuthorizedMinimalApi;

public class KeycloakTokenProxy(HttpClient httpClient, IConfiguration configuration, ILogger<KeycloakTokenProxy> logger) : IKeycloakTokenProxy
{
	public async Task<KeycloakTokenResponse> LoginAsync(string username, string password)
	{
		var tokenEndpoint = $"{configuration["Keycloak:BaseUrl"]}/realms/{configuration["Keycloak:Realm"]}/protocol/openid-connect/token";

		var requestBody = new Dictionary<string, string>
		{
			{ "grant_type", "password" },
			{ "client_id", configuration["Keycloak:ClientId"]! },
			{ "client_secret", configuration["Keycloak:ClientSecret"]! }, 
			{ "username", username },
			{ "password", password }
		};

		var content = new FormUrlEncodedContent(requestBody);

		var response = await httpClient.PostAsync(tokenEndpoint, content);

		if (response.IsSuccessStatusCode)
		{
			var responseContent = await response.Content.ReadAsStringAsync();
			var tokenResult = JsonSerializer.Deserialize<KeycloakTokenResponse>(responseContent);

			return tokenResult!;
		}
		
		logger.LogWarning("Authentication failed, response: {@Response}", response);

		throw new("failed");
	}
}