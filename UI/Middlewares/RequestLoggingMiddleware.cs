using System.Diagnostics;
using System.Security.Claims;

namespace UI.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context){
            var request = context.Request;
            var response = context.Response;
            var startTime = DateTime.UtcNow;
            var requestId = Guid.NewGuid().ToString();
            var requestPath = request.Path;
            var requestMethod = request.Method;
            var requestHeaders = request.Headers;
            var requestBody = await request.ReadAsStringAsync();
            var requestQuery = request.Query;
        }








    }
}    