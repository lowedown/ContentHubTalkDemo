using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace ContentHubFacade.Middleware
{
	public class RequestLogger
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<RequestLogger> _logger;

		public RequestLogger(RequestDelegate next, ILogger<RequestLogger> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			_logger.LogInformation($"{context.Request.Method} {context.Request.GetEncodedPathAndQuery()}");
			await _next(context);
		}
	}
}
