namespace Haven.DotNet.Attributes;

/// <summary>
/// Attribute to define a subcommand for a slash command
/// </summary>
public class SubSlashCommandAttribute : Attribute
{
	/// <summary>
	/// Constructor used to set publicly visible properties
	/// </summary>
	/// <param name="name"></param>
	/// <param name="description"></param>
	public SubSlashCommandAttribute(string name, string description)
	{
		Name = name;
		Description = description;
	}
	
	/// <summary>
	/// The name of the subcommand
	/// </summary>
	public string Name { get; init; }
	
	/// <summary>
	/// The description of the subcommand
	/// </summary>
	public string Description { get; init; }
}