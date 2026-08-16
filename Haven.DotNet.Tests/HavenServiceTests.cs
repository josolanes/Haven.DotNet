using System.Net;
using System.Net.Http.Json;
using Haven.DotNet.Models;
using Moq;
using NUnit.Framework;

namespace Haven.DotNet.Tests;

public class HavenServiceTests
{
	private const string token = "some token";
	
	[TestCase("some message")]
	public async Task SendMessage_Success(string message)
	{
		var httpClient = CreateMockHttp(HttpMethod.Post, $"/api/webhooks/{token}");
		
		var havenService = CreateHavenService(httpClient, token);
		await havenService.SendMessage(message);
		
		httpClient.Verify(HttpMethod.Post, $"/api/webhooks/{token}", Times.Once());
		Assert.That(httpClient.Request!.Content, Is.InstanceOf<JsonContent>());
		Assert.That(((JsonContent)httpClient.Request!.Content!).Value, Is.InstanceOf<SendMessageRequest>());
		Assert.That(((SendMessageRequest)((JsonContent)httpClient.Request!.Content!).Value!).Content, Is.EqualTo(message));
	}
	
	[Test]
	public async Task GetSubcommands_Success()
	{
		var response = new GetCommandsResponse
		{
			Commands =
			[
				new RegisterCommandRequest
				{
					Command = "1st",
					Description = "1st description",
					Subcommands =
					[
						new Subcommand
						{
							Name = "1st subcommand",
							Description = "1st subcommand description"
						},
						new Subcommand
						{
							Name = "2nd subcommand",
							Description = "2nd subcommand description"
						}
					]
				}
			]
		};
		
		var httpClient = CreateMockHttp(
			HttpMethod.Get,
			new HttpResponseMessage
			{
				Content = JsonContent.Create(response)
			},
			$"/api/webhooks/{token}/commands");
		
		var havenService = CreateHavenService(httpClient, token);
		var actualResponse = await havenService.GetSubcommands();
		
		httpClient.Verify(HttpMethod.Get, $"/api/webhooks/{token}/commands", Times.Once());
		Assert.That(actualResponse, Is.EqualTo(
			response.Commands.FirstOrDefault()!.Subcommands!.Select(c => c.Name)));
	}
	
	public async Task SetCommands_Success()
	{
		var httpClient = CreateMockHttp(HttpMethod.Post, $"/api/webhooks/{token}/commands");

		var requestContent = new RegisterCommandRequest
		{
			Command = "1st",
			Description = "1st description",
			Subcommands =
			[
				new Subcommand
				{
					Name = "1st subcommand",
					Description = "1st subcommand description"
				},
				new Subcommand
				{
					Name = "2nd subcommand",
					Description = "2nd subcommand description"
				}
			]
		};
		
		var havenService = CreateHavenService(httpClient, token);
		await havenService.SetCommands(requestContent);
		
		httpClient.Verify(HttpMethod.Post, $"/api/webhooks/{token}/commands", Times.Once());
		Assert.That(httpClient.Request!.Content, Is.InstanceOf<JsonContent>());
		Assert.That(((JsonContent)httpClient.Request!.Content!).Value, Is.InstanceOf<RegisterCommandRequest>());
		Assert.That(((RegisterCommandRequest)((JsonContent)httpClient.Request!.Content!).Value!), Is.EqualTo(requestContent));
	}
	
	[TestCase("command one")]
	[TestCase("command two")]
	public async Task DeleteCommand_Success(string commandName)
	{
		var httpClient = CreateMockHttp(HttpMethod.Delete, $"/api/webhooks/{token}/commands/{commandName}");
		
		var havenService = CreateHavenService(httpClient, token);
		await havenService.DeleteCommand(commandName);
		
		httpClient.Verify(HttpMethod.Delete, $"/api/webhooks/{token}/commands/{commandName}", Times.Once());
	}
	
	[TestCase("test payload", "some secret","ce5774cf3f063d3e9000e5bd3e8cd8d359029abbeb733dc3fb4901edc9690003")]
	[TestCase("a different payload with more text", "b688d8002c20561c660159400f4d57d4bcb2f6626fd3d9bcf268973ef940d43d","a4ec4bd3e6c80f037ee025ed74eaa7753de83cf1598dc2d4cbf331d142dd32ee")]
	public void GetHmacSignature_ProperlyCalculatesSignature(string payload, string secret, string expected)
	{
		var httpClient = CreateMockHttp(HttpMethod.Get, new HttpResponseMessage(), string.Empty);
		
		var havenService = CreateHavenService(httpClient, secret);
		
		var signature = havenService.GetHmacSignature(payload);
		
		Assert.That(signature, Is.EqualTo(expected));
	}

	private static MockHttpClient CreateMockHttp(
		HttpMethod method,
		string path)
	{
		var mockHttp = new MockHttpClient();
		mockHttp.Setup(method, path, new HttpResponseMessage { StatusCode = HttpStatusCode.OK});

		return mockHttp;
	}
	
	private static MockHttpClient CreateMockHttp(
		HttpMethod method,
		HttpResponseMessage response,
		string path )
	{
		var mockHttp = new MockHttpClient();
		mockHttp.Setup(method, path, response);

		return mockHttp;
	}
	
	private static HavenService CreateHavenService(MockHttpClient client, string? webhookSecret)
	{
		return new HavenService(client.Object, token, webhookSecret);
	}
}