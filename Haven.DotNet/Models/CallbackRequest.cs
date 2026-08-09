namespace Haven.DotNet.Models;

/// <summary>
/// The callback request model to be received at the callback url
/// </summary>
public class CallbackRequest
{
	/// <summary>
	/// The slash command issued
	/// </summary>
	public required string Command { get; set; }

	/// <summary>
	/// The arguments for the command, ex: <c>start game</c> (in this case start is the subcommand and game is an argument passed to the subcommand)
	/// </summary>
	public required string Args { get; set; } = "";
}