using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContentHubFacade.Middleware
{
	public class ApiKeyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ApiKeyMiddleware> _logger;
		private const string APIKEYNAME = "apikey";

		public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, IConfiguration appSettings)
		{
			if (!appSettings.GetValue<bool>("ApiKey.Enabled"))
			{
				await _next(context);
				return;
			}

			if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
			{
				context.Response.StatusCode = 401;
				await context.Response.WriteAsync("Api Key was not provided.");
				_logger.LogWarning("Request blocked: Missing Api Key");
				return;
			}

			var apiKey = Environment.GetEnvironmentVariable(APIKEYNAME) ?? appSettings.GetValue<string>(APIKEYNAME);

			if (!apiKey.Equals(extractedApiKey))
			{
				context.Response.StatusCode = 401;
				await context.Response.WriteAsync("Invalid Api Key provided.");
				_logger.LogWarning("Request blocked: Invalid Api Key");
				return;
			}

			await _next(context);
		}
	}
}
