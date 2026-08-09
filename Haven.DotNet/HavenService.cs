using System.Net.Http.Json;
using Haven.DotNet.Models;

namespace Haven.DotNet;

/// <inheritdoc />
public class HavenService(
	IHttpClientFactory httpClientFactory,
	string token) : IHavenService
{
	/// <inheritdoc />
	public async Task SendMessageAsync(string message)
	{
		var httpClient = httpClientFactory.CreateClient(nameof(Haven.DotNet));
		await httpClient.PostAsync($"/api/webhooks/{token}",
			JsonContent.Create(new SendMessageRequest()
			{
				Content = message
			}));
	}

	/// <inheritdoc />
	public async Task<List<string>> GetSubcommands()
	{
		var httpClient = httpClientFactory.CreateClient(nameof(Haven.DotNet));
		
		var response = await httpClient.GetAsync($"/api/webhooks/{token}/commands");
		var commands = await response.Content.ReadFromJsonAsync<GetCommandsResponse>();
		return commands?.Commands?.FirstOrDefault()?.Subcommands?.Select(c => c.Name)?.ToList() ?? [];
	}
	
	/// <inheritdoc />
	public async Task SetCommands(RegisterCommandRequest commands)
	{
		var httpClient = httpClientFactory.CreateClient(nameof(Haven.DotNet));
		await httpClient.PostAsync($"/api/webhooks/{token}/commands",
			JsonContent.Create(commands));
	}

	/// <inheritdoc />
	public async Task DeleteCommand(string commandName)
	{
		var httpClient = httpClientFactory.CreateClient(nameof(Haven.DotNet));
		await httpClient.DeleteAsync($"/api/webhooks/{token}/commands/{commandName}");
	}
}