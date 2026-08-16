using System.Net;
using System.Reflection;
using Haven.DotNet.Extensions;
using Haven.DotNet.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Haven.DotNet.Tests.Extensions;

public class WebApplicationExtensionsTests
{
	[Test]
	public async Task RegisterCommands_RegistersWhenNoneExisted()
	{
		var mockService = new Mock<IHavenService>();
		mockService.Setup(s => s.DeleteCommand(It.IsAny<string>()))
			.Throws(new WebException("Not found"));

		mockService.Setup(s => s.SetCommands(It.IsAny<RegisterCommandRequest>()));
		
		var mockServiceProvider = new Mock<IServiceProvider>();
		mockServiceProvider.Setup(s => s.GetService(typeof(IHavenService)))
			.Returns(mockService.Object);
		
		var mockLogger = new Mock<ILogger<IHavenService>>();

		var webApplicationExtensionsType = typeof(WebApplicationExtensions);
		var registerCommands = webApplicationExtensionsType.GetMethod(
			"RegisterCommands",
			BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
			null,
			[typeof(IServiceProvider), typeof(string), typeof(string), typeof(ILogger<IHavenService>)],
			null);
		
		var registerCommandsWithGeneric = registerCommands!.MakeGenericMethod(typeof(Subcommand));
		
		registerCommandsWithGeneric.Invoke(
			obj: null,
			parameters: [mockServiceProvider.Object, "some command", "some description", mockLogger.Object]);
		
		mockService.Verify(s => s.DeleteCommand(It.IsAny<string>()), Times.Once);
		mockService.Verify(s => s.SetCommands(It.IsAny<RegisterCommandRequest>()), Times.Once);
	}
}