namespace Starter.API.Middlewares;

internal static class MiddlewareExtensions
{
    internal static IApplicationBuilder UseLogContextTraceLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<LogContextTraceLoggingMiddleware>();
        //app.UseMiddleware<TenantMiddleware>();

        return app;
    }
}
