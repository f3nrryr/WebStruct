namespace WebStruct.RateLimit
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitService _rateLimitService;

        public RateLimitMiddleware(RequestDelegate next, IRateLimitService rateLimitService)
        {
            _next = next;
            _rateLimitService = rateLimitService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Пропускаем публичные эндпоинты
            if (IsPublicEndpoint(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var clientId = GetClientId(context);
            var endpoint = context.Request.Path;

            if (!_rateLimitService.IsAllowed(clientId, endpoint))
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            await _next(context);
        }

        private string GetClientId(HttpContext context)
        {
            // Используем IP адрес как идентификатор клиента
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private bool IsPublicEndpoint(PathString path)
        {
            var publicPaths = new[]
            {
            "/swagger",
            "/favicon.ico"
        };

            return publicPaths.Any(p => path.StartsWithSegments(p));
        }
    }
}
