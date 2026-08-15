using System.Net;
using Moq;
using Moq.Protected;

namespace Haven.DotNet.Tests;

internal class MockHttpClient
{
	public HttpClient Object => new(_handler.Object)
	{
		BaseAddress = new Uri("http://localhost")
	};
	public HttpRequestMessage? Request { get; private set; }
	
	public string? RequestContent { get; private set; }

	private readonly Mock<HttpMessageHandler> _handler = new(MockBehavior.Loose);
	
	public MockHttpClient Setup(HttpMethod method, string uri, HttpResponseMessage response)
	{
		_handler
			.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.Is<HttpRequestMessage>(x => x.Method == method && x.RequestUri!.OriginalString == $"http://localhost{uri}"),
				ItExpr.IsAny<CancellationToken>())
			.Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) =>
			{
				Request = request;
				RequestContent = request.Content?.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
			})
			.ReturnsAsync(response);

		return this;
	}

	public void Verify(HttpMethod method, string uri, Times times)
	{
		_handler
			.Protected()
			.Verify(
				"SendAsync",
				times,
				ItExpr.Is<HttpRequestMessage>(x => x.Method == method && x.RequestUri!.OriginalString == $"http://localhost{uri}"),
				ItExpr.IsAny<CancellationToken>());
	}

	public void Verify(HttpMethod method, string uri, HttpContent content, Times times)
	{
		using var request = new HttpRequestMessage
		{
			Method = method,
			RequestUri = new Uri($"http://localhost{uri}"),
			Content = content,
			Version = HttpVersion.Version11
		};

		_handler
			.Protected()
			.Verify(
				"SendAsync",
				times,
				ItExpr.Is<HttpRequestMessage>(x => x.ToString() == request.ToString()),
				ItExpr.IsAny<CancellationToken>());
	}
}