# Haven.DotNet
[Haven](https://github.com/ancsemi/Haven) bot library for .NET developers.

[Haven bot developer guide](https://github.com/ancsemi/Haven/blob/main/GUIDE.md#-bot--webhook-developer-guide) used for reference.
Haven.DotNet was created to enable using Haven bots in .NET 10 and is written per the Haven documentation.

### Library Usage Steps
1. Install `Haven.DotNet` via NuGet
2. Implement the `IHavenDotNetHandler` class and `Handle` method
3. If you want to use subcommands also, add each subcommand as a separate method in your `IHavenDotNetHandler` implementation class and have your `Handle` method choose which method to call based on received `args`
    * Each subcommand method must have an attribute with the subcommand name and description. Example:
   ```csharp
   [SubSlashCommand("list", "lists the things")]
   public async Task<string> List()
   {
       // Do work here where your return is the message to show in the Haven channel
   }
   ```
4. Add Haven.DotNet support to your application. Example:
   ```csharp
   var builder = WebApplication.CreateBuilder(args);
   
   builder.Services.AddHavenDotNet<HavenChatbot>(baseUrl, token);
   ```
5. Add Haven callback url support. Example:
   ```csharp
   var host = builder.Build();
   
   host.UseHavenDotNetCallback();
   ```
6. Register your slash command. Example:
   ```csharp
   await host.RegisterHavenSlashCommand<HavenChatbot>("containr", "Control containers");
   ```
7. Start your application

### Example Application Where Haven.DotNet Is Used
For an example application using Haven.DotNet, see [ContainrBot](https://github.com/josolanes/ContainrBot): an extensible container management chat bot with flexible chat service and container orchestration support.

### Haven Bot Setup
1. Create a bot in Haven
    * Right click a channel -> Webhooks -> [Bot Name] -> Create -> Copy the URL
    * If you're the Haven server admin, you can also retrieve the token later in Settings -> Admin -> Bots -> Manage Bots -> Click Bot -> [End of Webhook URL, after the /api/webhooks/]
2. Add the callback url to your bot
    * Settings -> Admin -> Bots -> Manage Bots -> Click Bot -> Callback URL
    * NOTE: Private/internal URLs are not supported by Haven, see [SSRF callback safety check for details](https://github.com/ancsemi/Haven/blob/0fdaf322b702ea9784e4e05adfc7e1ae8a118611/src/socketHandlers/index.js#L958)
        * If you use a local IP or standard local address, you will see errors in the Haven logs like `Bot command /containr: callback URL blocked by SSRF guard` and the callback url will not be reached