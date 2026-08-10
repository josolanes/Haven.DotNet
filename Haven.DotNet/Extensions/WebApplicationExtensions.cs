using System.Reflection;
using System.Text.Json;
using Haven.DotNet.Attributes;
using Haven.DotNet.Handlers;
using Haven.DotNet.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Haven.DotNet.Extensions;

/// <summary>
/// WebApplication extension methods to help set up Haven.DotNet
/// </summary>
public static class WebApplicationExtensions
{
	private static IHavenService? _havenService;
	
	extension(WebApplication app)
	{
		/// <summary>
		/// Add necessary Haven.DotNet callback support to the application
		/// </summary>
		/// <returns>A <see cref="WebApplication"/> with Haven.DotNet callback support added</returns>
		/// <exception cref="InvalidOperationException">Thrown if no handler implementation is found for <see cref="IHavenDotNetHandler"/></exception>
		public WebApplication UseHavenDotNetCallback()
		{
			app.MapPost("/callback", async (HttpContext httpContext, ILogger<IHavenService> logger) =>
			{
				var handler = app.Services.GetService(typeof(IHavenDotNetHandler)) as IHavenDotNetHandler;

				InitializeHavenService(app.Services);

				var shouldHandle = true;
				var body = string.Empty;

				// Perform HMAC signature verification
				if (httpContext.Request.Headers.ContainsKey("X-Haven-Signature"))
				{
					using var reader = new StreamReader(httpContext.Request.Body);
					body = await reader.ReadToEndAsync();
					
					var expectedSignature = _havenService!.GetHmacSignature(body);

					if (expectedSignature != httpContext.Request.Headers["X-Haven-Signature"])
					{
						logger.LogWarning("HMAC Signature Mismatch on callback request");
						shouldHandle = false;
					}
					else
					{
						shouldHandle = true;
					}
				}

				if (shouldHandle)
				{
					var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
					var callbackRequest = JsonSerializer.Deserialize<CallbackRequest>(body, jsonOptions);

					if (callbackRequest == null)
					{
						throw new InvalidOperationException("Unable to deserialize callback request");
					}
					
					var message = handler == null
						? throw new InvalidOperationException($"No handler implementation found for {nameof(IHavenDotNetHandler)}")
						: await handler.Handle(callbackRequest.Command, callbackRequest.Args);

					await _havenService!.SendMessageAsync(message);
				}
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

	private static void InitializeHavenService(IServiceProvider serviceProvider)
	{
		_havenService ??= serviceProvider.GetService(typeof(IHavenService)) as IHavenService ??
		                  throw new InvalidOperationException($"No service implementation found for {nameof(IHavenService)}");
	}
}