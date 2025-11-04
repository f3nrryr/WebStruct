using Microsoft.AspNetCore.Authorization;

namespace WebStruct.JWT
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;

        public JwtValidationMiddleware(RequestDelegate next, IJwtService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Пропускаем публичные эндпоинты
            if (IsPublicEndpoint(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // Пытаемся получить токен из заголовка
            var token = GetTokenFromHeader(context.Request);

            if (!string.IsNullOrEmpty(token))
            {
                var principal = _jwtService.ValidateToken(token);

                if (principal != null)
                {
                    context.User = principal;
                }
                else
                {
                    // Если токен невалидный - возвращаем 401
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }
            }
            else if (RequiresAuthorization(context))
            {
                // Если токена нет, но эндпоинт требует авторизации
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization required");
                return;
            }

            await _next(context);
        }

        private bool IsPublicEndpoint(PathString path)
        {
            var publicPaths = new[]
            {
            "/api/auth/login",
            "/api/auth/register",
            "/swagger",
            "/favicon.ico"
        };

            return publicPaths.Any(p => path.StartsWithSegments(p));
        }

        private bool RequiresAuthorization(HttpContext context)
        {
            // Проверяем атрибуты Authorize на эндпоинте
            var endpoint = context.GetEndpoint();
            return endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;
        }

        private string GetTokenFromHeader(HttpRequest request)
        {
            if (request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var headerValue = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(headerValue) && headerValue.StartsWith("Bearer "))
                {
                    return headerValue.Substring(7);
                }
            }
            return null;
        }
    }
}
