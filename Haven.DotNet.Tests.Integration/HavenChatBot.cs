using Haven.DotNet.Attributes;
using Haven.DotNet.Handlers;

namespace Haven.DotNet.Tests.Integration;

public class HavenChatBot : IHavenDotNetHandler
{
	public async Task<string> Handle(string command, string args)
	{
		var subSlashCommand = args.Split(" ")[0];

		return subSlashCommand switch
		{
			"test" => await Test(),
			"another" => await Another(),
			_ => string.Empty
		};
	}

	[SubSlashCommand("test", "test command")]
	private static Task<string> Test()
	{
		return Task.FromResult("This is just a test");
	}
	
	[SubSlashCommand("another", "another test command")]
	private static Task<string> Another()
	{
		return Task.FromResult("This is just another test");
	}
}