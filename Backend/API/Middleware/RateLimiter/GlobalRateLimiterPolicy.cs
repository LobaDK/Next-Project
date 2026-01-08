using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Middleware.RateLimiter;

public class GlobalRateLimiterPolicy : IRateLimiterPolicy<string>
{
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        string key;
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            key = httpContext.User.Claims.Select(claim => claim).Single(claim => claim.Type == "unique_name").Value;
        }
        else
        {
            key = httpContext.Request.Headers.Host.ToString();
        }
        
        return RateLimitPartition.GetFixedWindowLimiter(key,
        partition => new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 50,
            QueueLimit = 10,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            Window = TimeSpan.FromMinutes(1)
        });
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } =
        (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            
            string msg;
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                msg = $"Rate limit exceeded. Please try again in {retryAfter.TotalSeconds:N0} seconds.";
            }
            else
            {
                msg = "Rate limit exceeded. Please try again later.";
            }

            context.HttpContext.Response.WriteAsync(msg, token);
            return new ValueTask();
        };
}
