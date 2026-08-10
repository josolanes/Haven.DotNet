using Haven.DotNet.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Haven.DotNet.Extensions;

/// <summary>
/// IServiceCollection extension methods to help set up Haven.DotNet
/// </summary>
public static class ServiceExtensions
{
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Add Haven.DotNet support to an application
		/// </summary>
		/// <param name="baseUrl">The base URL for your Haven instance. NOTE: Private/internal URLs are not supported by Haven, see <seealso href="https://github.com/ancsemi/Haven/blob/0fdaf322b702ea9784e4e05adfc7e1ae8a118611/src/socketHandlers/index.js#L958">SSRF callback safety check for details</seealso></param>
		/// <param name="token">The token for your Haven bot. This can be found in Settings -> Admin -> Bots -> Manage Bots -> Click Bot -> [End of Webhook URL, after the /api/webhooks/]</param>
		/// <param name="webhookSecret">Optional webhook secret for HMAC signature verification. This should match the bot's webhook secret in Haven</param>
		/// <typeparam name="T">The class implementing the <see cref="IHavenDotNetHandler"/> interface</typeparam>
		/// <returns>An <see cref="IServiceCollection"/> with Haven.DotNet support added</returns>
		public IServiceCollection AddHavenDotNet<T>(string baseUrl, string token, string? webhookSecret = null) where T : class, IHavenDotNetHandler
		{
			services.AddHttpClient<IHavenService>(nameof(Haven.DotNet), client =>
			{
				client.BaseAddress = new Uri(baseUrl);
				client.DefaultRequestHeaders.Add("Accept", "application/json");
			});
			
			services.AddTransient<IHavenService>(provider => new HavenService(provider.GetRequiredService<IHttpClientFactory>(), token, webhookSecret));
			services.AddTransient<IHavenDotNetHandler, T>();
			
			return services;
		}
	}
}