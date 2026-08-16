using Microsoft.AspNetCore.Mvc.Testing;

namespace Haven.DotNet.Tests.Integration;

public class HavenDotNetWebApplicationFactory<TProgram>
	: WebApplicationFactory<TProgram> where TProgram : class
{
}