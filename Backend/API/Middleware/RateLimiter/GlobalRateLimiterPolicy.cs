using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Middleware.RateLimiter;

public class GlobalRateLimiterPolicy() : IRateLimiterPolicy<string>
{
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        ILogger<GlobalRateLimiterPolicy> logger = httpContext.RequestServices.GetRequiredService<ILogger<GlobalRateLimiterPolicy>>();
        
        string key = GetPartitionKey(httpContext);
        
        return RateLimitPartition.GetFixedWindowLimiter(key,
        partition => new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 100,
            QueueLimit = 10,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            Window = TimeSpan.FromMinutes(1)
        });
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } =
        (context, token) =>
        {
            ILogger<GlobalRateLimiterPolicy> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<GlobalRateLimiterPolicy>>();
            
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

            string partition = GetPartitionKey(context.HttpContext);

            logger.LogWarning("Rate limit exceeded for partition '{Partition}'.", partition);

            context.HttpContext.Response.WriteAsync(msg, token);
            return new ValueTask();
        };
    
    private static string GetPartitionKey(HttpContext httpContext)
    {
        ILogger<GlobalRateLimiterPolicy> logger = httpContext.RequestServices.GetRequiredService<ILogger<GlobalRateLimiterPolicy>>();

        string key;
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                key = httpContext.User.Claims.Select(claim => claim).Single(claim => claim.Type == "unique_name").Value;
            }
            catch (Exception ex)
            {
                List<string> claimTypes = [.. httpContext.User.Claims.Select(claim => claim.Type)];
                logger.LogError(ex, "Error extracting unique_name claim for rate limiting. Falling back to Host header. Available claims: {Claims}", string.Join(", ", claimTypes));
                key = httpContext.Request.Headers.Host.ToString();
            }
        }
        else
        {
            key = httpContext.Request.Headers.Host.ToString();
        }

        return key;
    }
}
