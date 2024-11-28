using Microsoft.Extensions.Primitives;

namespace Starter.API.Middlewares;

internal sealed class TenantMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context)
    {

        context.Request.Headers.TryGetValue("tenant", out StringValues tenantFromHeader);
        if (!string.IsNullOrWhiteSpace(tenantFromHeader))
        {
            //await currentTenant.SetTenant(tenantFromHeader!);
        }

        await _next(context);

    }
}
