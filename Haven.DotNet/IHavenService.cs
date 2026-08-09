using Haven.DotNet.Models;

namespace Haven.DotNet;

/// <summary>
/// A service to allow interaction with the Haven chat API
/// Docs: https://github.com/ancsemi/Haven/blob/main/GUIDE.md#-bot--webhook-developer-guide
/// </summary>
public interface IHavenService
{
	/// <summary>
	/// Send a message to the chat
	/// </summary>
	/// <param name="message">The message to send</param>
	/// <returns></returns>
	Task SendMessageAsync(string message);

	/// <summary>
	/// Gets the subcommands registered to the bot
	/// </summary>
	/// <returns>A list of subcommands registered to the bot</returns>
	Task<List<string>> GetSubcommands();

	/// <summary>
	/// Sets the command and subcommands for the bot
	/// </summary>
	/// <param name="commands">A <see cref="RegisterCommandRequest" /> containing command and subcommand names and descriptions</param>
	/// <returns></returns>
	Task SetCommands(RegisterCommandRequest commands);
	
	/// <summary>
	/// Deletes a base command from the bot
	/// </summary>
	/// <param name="commandName">The base command name to delete</param>
	/// <returns></returns>
	Task DeleteCommand(string commandName);
}