using Haven.DotNet.Extensions;
using Haven.DotNet.Tests.Integration;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var havenBaseUrl = builder.Configuration["Haven:BaseUrl"]
	?? throw new InvalidOperationException("Missing required configuration value: Haven:BaseUrl");

var havenToken = builder.Configuration["Haven:Token"]
	?? throw new InvalidOperationException("Missing required configuration value: Haven:Token");

builder.Services.AddHavenDotNet<HavenChatBot>(
	havenBaseUrl,
	havenToken);

var app = builder.Build();

app.UseHavenDotNetCallback();

await app.RegisterHavenSlashCommand<HavenChatBot>(
	"slash",
	"main slash command");

app.Run();

#pragma warning disable ASP0027
#pragma warning disable ASP0027
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program;
#pragma warning restore ASP0027
#pragma warning restore ASP0027
