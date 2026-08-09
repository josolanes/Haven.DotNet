namespace Haven.DotNet.Models;

/// <summary>
/// SendMessage request model
/// </summary>
public class SendMessageRequest
{
	/// <summary>
	/// The message content
	/// </summary>
	public required string Content { get; set; }
	
	/// <summary>
	/// Override the bot's display name (optional)
	/// </summary>
	public string? Username { get; set; }
	
	/// <summary>
	/// Override the bot's avatar (optional)
	/// </summary>
	public string? AvatarUrl { get; set; }
	
	/// <summary>
	/// When true, only deliver to <see cref="RecipientId" /> and do not store in history (optional)
	/// </summary>
	public string? Ephemeral { get; set; }
	
	/// <summary>
	/// Required when <see cref="Ephemeral" /> is true, the user id to deliver to (optional)
	/// </summary>
	public string? RecipientId { get; set; }
}