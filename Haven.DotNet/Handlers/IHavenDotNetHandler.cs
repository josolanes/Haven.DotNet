namespace Haven.DotNet.Handlers;

/// <summary>
/// The Haven command handler to process commands appropriately
/// </summary>
public interface IHavenDotNetHandler
{
	/// <summary>
	/// How to handle a command and its arguments
	/// </summary>
	/// <param name="command">The slash command</param>
	/// <param name="args">Any text following the slash command, ex: <c>start game</c> where <c>start</c> may be a subcommand</param>
	/// <returns>The response to be displayed in the chat</returns>
	Task<string> Handle(string command, string args);
}