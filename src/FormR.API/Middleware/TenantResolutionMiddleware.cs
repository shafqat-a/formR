using System.Security.Claims;

namespace FormR.API.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract tenant ID from JWT claims
        var tenantIdClaim = context.User?.FindFirst("tenant_id")?.Value;

        if (!string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            context.Items["TenantId"] = tenantId;
            _logger.LogDebug("Resolved tenant ID: {TenantId}", tenantId);
        }
        else
        {
            _logger.LogWarning("Could not resolve tenant ID from JWT claims");
        }

        await _next(context);
    }
}

public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}
