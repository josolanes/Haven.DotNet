namespace Haven.DotNet.Models;

/// <summary>
/// The RegisterCommand request model
/// </summary>
public class RegisterCommandRequest
{
	/// <summary>
	/// The command
	/// </summary>
	public required string Command { get; set; }
	
	/// <summary>
	/// The <see cref="Command" />'s description
	/// </summary>
	public required string Description { get; set; }
	
	/// <summary>
	/// The <see cref="Command" />'s subcommands
	/// </summary>
	public List<Subcommand>? Subcommands { get; set; }
}

/// <summary>
/// Subcommands under the <see cref="RegisterCommandRequest.Command" />
/// </summary>
public class Subcommand
{
	/// <summary>
	/// <see cref="Subcommand" />'s name
	/// </summary>
	public required string Name { get; set; }
	
	/// <summary>
	/// <see cref="Subcommand" />'s description
	/// </summary>
	public required string Description { get; set; }
}