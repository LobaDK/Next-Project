using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Middleware.RateLimiter;

public class GlobalRateLimiterPolicy(ILogger<GlobalRateLimiterPolicy> logger) : IRateLimiterPolicy<string>
{
    private readonly ILogger<GlobalRateLimiterPolicy> _logger = logger;
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
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

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected =>
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

            string partition = GetPartitionKey(context.HttpContext);

            _logger.LogWarning("Rate limit exceeded for partition '{Partition}'.", partition);

            context.HttpContext.Response.WriteAsync(msg, token);
            return new ValueTask();
        };
    
    private string GetPartitionKey(HttpContext httpContext)
    {
        string key;
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                key = httpContext.User.Claims.Single(claim => claim.Type == "unique_name").Value;
            }
            catch (InvalidOperationException ex)
            {
                List<string> claimTypes = [.. httpContext.User.Claims.Select(claim => claim.Type)];
                _logger.LogError(ex, "Error extracting unique_name claim for rate limiting. Falling back to remote IP address. Available claims: {Claims}", string.Join(", ", claimTypes));
                key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            }
        }
        else
        {
            key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            key = string.IsNullOrEmpty(ipAddress) ? "anonymous" : ipAddress;
        }

        return key;
    }
}
