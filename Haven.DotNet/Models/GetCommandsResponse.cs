namespace Haven.DotNet.Models;

/// <summary>
/// The response model from the GetCommands request
/// </summary>
public class GetCommandsResponse
{
	/// <summary>
	/// A list of <see cref="RegisterCommandRequest" /> objects
	/// </summary>
	public required List<RegisterCommandRequest> Commands { get; init; } = [];
}