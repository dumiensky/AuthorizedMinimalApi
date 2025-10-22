// made from https://medium.com/@faulycoelho/net-web-api-with-keycloak-11e0286240b9

using AuthorizedMinimalApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization()
	.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateIssuer = true,
			ValidIssuer = $"{builder.Configuration["Keycloak:BaseUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}",

			ValidateAudience = true,
			ValidAudience = "account",

			ValidateIssuerSigningKey = true,
			ValidateLifetime = false,

			IssuerSigningKeyResolver = (_, _, _, parameters) =>
			{
				var client = new HttpClient();
				var keyUri = $"{parameters.ValidIssuer}/protocol/openid-connect/certs";
				var response = client.GetAsync(keyUri).Result;
				var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);

				return keys.GetSigningKeys();
			}
		};

		options.RequireHttpsMetadata = false; // Only in develop environment
		options.SaveToken = true;
	});

builder.Services.AddHttpClient();
builder.Services.AddTransient<IKeycloakTokenProxy, KeycloakTokenProxy>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World from AuthorizedMinimalApi!");

app.MapGet("/admin", [Authorize(Roles = "admin")]() => "You are an admin");
app.MapGet("/general", [Authorize(Roles = "general")]() => "You have general access");
app.MapGet("/authorized", [Authorize]() => "You are authorized, but your role was not checked");

app.MapPost("/login",
	async (LoginRequest req, IKeycloakTokenProxy proxy) =>
	{
		try
		{
			var response = await proxy.LoginAsync(req.Username, req.Password);
			return Results.Ok(response);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return Results.Unauthorized();
		}
	});

app.Run();