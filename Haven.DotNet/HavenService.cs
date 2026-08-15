using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Haven.DotNet.Models;

namespace Haven.DotNet;

/// <inheritdoc />
public class HavenService(
	HttpClient httpClient,
	string token,
	string? webhookSecret = null) : IHavenService
{
	/// <inheritdoc />
	public async Task SendMessage(string message)
	{
		await httpClient.PostAsync($"/api/webhooks/{token}",
			JsonContent.Create(new SendMessageRequest
			{
				Content = message
			}));
	}

	/// <inheritdoc />
	public async Task<List<string>> GetSubcommands()
	{
		var response = await httpClient.GetAsync($"/api/webhooks/{token}/commands");
		var commands = await response.Content.ReadFromJsonAsync<GetCommandsResponse>();
		return commands?.Commands.FirstOrDefault()?.Subcommands?.Select(c => c.Name).ToList() ?? [];
	}
	
	/// <inheritdoc />
	public async Task SetCommands(RegisterCommandRequest commands)
	{
		await httpClient.PostAsync($"/api/webhooks/{token}/commands",
			JsonContent.Create(commands));
	}

	/// <inheritdoc />
	public async Task DeleteCommand(string commandName)
	{
		await httpClient.DeleteAsync($"/api/webhooks/{token}/commands/{commandName}");
	}
	
	/// <inheritdoc />
	public string GetHmacSignature(string payload)
	{
		if (string.IsNullOrEmpty(webhookSecret))
		{
			return string.Empty;
		}
		
		using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
		
		var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
		return Convert.ToHexString(computedHash).ToLowerInvariant();
	}
}