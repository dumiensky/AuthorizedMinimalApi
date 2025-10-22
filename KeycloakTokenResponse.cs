using System.Text.Json.Serialization;

namespace AuthorizedMinimalApi;

public class KeycloakTokenResponse
{
	[JsonPropertyName("access_token")]
	public required string AccessToken { get; set; }

	[JsonPropertyName("expires_in")]
	public int ExpiresIn { get; set; }

	[JsonPropertyName("refresh_token")]
	public required string RefreshToken { get; set; }
}