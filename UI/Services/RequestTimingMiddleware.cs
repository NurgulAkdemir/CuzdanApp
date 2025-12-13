using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UI.Services
{
	public class RequestTimingMiddleware
	{
		private readonly RequestDelegate _next;
		public RequestTimingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, IPerformanceMetricsService metricsService)
		{
			var sw = Stopwatch.StartNew();
			try
			{
				await _next(context);
			}
			finally
			{
				sw.Stop();
				metricsService.RecordRequest(sw.ElapsedMilliseconds);
			}
		}
	}
}


