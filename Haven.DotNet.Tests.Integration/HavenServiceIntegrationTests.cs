using System.Text;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Haven.DotNet.Tests.Integration;

public class HavenServiceIntegrationTests
{
	private HttpClient _client = null!;
	private HavenDotNetWebApplicationFactory<Program> _factory = null!;
	private WireMockServer _server = null!;

	[SetUp]
	public void SetUp()
	{
		_server = WireMockServer.Start();

		Environment.SetEnvironmentVariable("Haven__BaseUrl", _server.Urls[0]);
		Environment.SetEnvironmentVariable("Haven__Token", "1234");

		Assert.DoesNotThrow(() =>
			_factory = new HavenDotNetWebApplicationFactory<Program>(),
			"Unable to complete IServiceCollection setup or unable to reach Haven API to reset commands");

		_client = _factory.CreateClient();
	}

	[TearDown]
	public void TearDown()
	{
		_client.Dispose();
		_factory.Dispose();
		_server.Stop();
		_server.Dispose();

		Environment.SetEnvironmentVariable("Haven__BaseUrl", null);
		Environment.SetEnvironmentVariable("Haven__Token", null);
	}

	[Test]
	public async Task Callback_SendsRequestToHavenApi()
	{
		_server
			.Given(
				Request.Create()
					.WithPath("/api/webhooks/1234")
					.UsingPost()
			)
			.RespondWith(
				Response.Create()
					.WithStatusCode(200)
					.WithHeader("Content-Type", "application/json")
					.WithBody("""{ "ok": true }""")
			);

		var callbackJson = """
		                   {
		                     "command": "slash",
		                     "args": "test"
		                   }
		                   """;

		using var content = new StringContent(callbackJson, Encoding.UTF8, "application/json");

		var response = await _client.PostAsync("/callback", content);

		var responseBody = await response.Content.ReadAsStringAsync();

		TestContext.WriteLine($"App response: {(int)response.StatusCode} {response.StatusCode}");
		TestContext.WriteLine(responseBody);

		Assert.That(response.IsSuccessStatusCode, Is.True);
	}
}