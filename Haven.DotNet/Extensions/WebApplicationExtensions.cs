using System.Reflection;
using Haven.DotNet.Attributes;
using Haven.DotNet.Handlers;
using Haven.DotNet.Models;
using Microsoft.AspNetCore.Builder;

namespace Haven.DotNet.Extensions;

/// <summary>
/// WebApplication extension methods to help set up Haven.DotNet
/// </summary>
public static class WebApplicationExtensions
{
	extension(WebApplication app)
	{
		/// <summary>
		/// Add necessary Haven.DotNet callback support to the application
		/// </summary>
		/// <returns>A <see cref="WebApplication"/> with Haven.DotNet callback support added</returns>
		/// <exception cref="InvalidOperationException">Thrown if no handler implementation is found for <see cref="IHavenDotNetHandler"/></exception>
		public WebApplication UseHavenDotNetCallback()
		{
			app.MapPost("/callback", async (CallbackRequest request) =>
			{
				var handler = app.Services.GetService(typeof(IHavenDotNetHandler)) as IHavenDotNetHandler;

				return handler == null
					? throw new InvalidOperationException($"No handler implementation found for {nameof(IHavenDotNetHandler)}")
					: await handler.Handle(request.Command, request.Args);
			});

			return app;
		}
		
		/// <summary>
		/// Register a slash command with Haven
		/// </summary>
		/// <param name="slashCommandName">The slash command name to register</param>
		/// <param name="slashCommandDescription">The slash command description</param>
		/// <typeparam name="T">The class implementing the <see cref="IHavenDotNetHandler"/> interface</typeparam>
		public async Task RegisterHavenSlashCommand<T>(string slashCommandName, string slashCommandDescription)
		{
			await RegisterCommands<T>(app.Services, slashCommandName, slashCommandDescription);
		}
	}
	
	private static async Task RegisterCommands<T>(IServiceProvider services, string slashCommandName, string slashCommandDescription)
	{
		var chatService = services.GetService(typeof(IHavenService)) as IHavenService;
		var newCommands = GetCommandsFromAttributes<T>();

		if (chatService == null)
		{
			throw new InvalidOperationException($"No service implementation found for {nameof(IHavenService)}");
		}

		await chatService.DeleteCommand(slashCommandName);

		var slashCommand = new RegisterCommandRequest()
		{
			Command = slashCommandName,
			Description = slashCommandDescription,
			Subcommands = []
		};
		
		foreach (var command in newCommands)
		{
			slashCommand.Subcommands.Add(command);
		}
		
		await chatService.SetCommands(slashCommand);
	}

	private static List<Subcommand> GetCommandsFromAttributes<T>()
	{
		var slashCommands = new List<Subcommand>();
		Type type = typeof(T);
		MethodInfo[] methods = type.GetMethods();
		foreach (var method in methods)
		{
			if (Attribute.GetCustomAttribute(method, typeof(SubSlashCommandAttribute)) is SubSlashCommandAttribute attribute)
			{
				slashCommands.Add(new Subcommand()
				{
					Name = attribute.Name,
					Description = attribute.Description
				});
			}
		}
		
		return slashCommands;
	}
}