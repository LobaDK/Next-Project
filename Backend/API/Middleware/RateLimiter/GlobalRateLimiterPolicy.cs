using System.Security.Cryptography;
using System.Text;
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
            string hashedPartition = HashPartitionKey(partition);

            _logger.LogWarning("Rate limit exceeded for partition '{Partition}'.", hashedPartition);

            return new ValueTask(context.HttpContext.Response.WriteAsync(msg, token));
        };
    
    
    private static string HashPartitionKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return "empty";

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(hash)[..32];
    }

    private string GetPartitionKey(HttpContext httpContext)
    {
        string key;
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            List<Claim> uniqueNameClaims = [.. httpContext.User.Claims.Where(claim => claim.Type == "unique_name")];

            if (uniqueNameClaims.Count == 1)
            {
                key = uniqueNameClaims[0].Value;
            }
            else
            {
                List<string> claimTypes = [.. httpContext.User.Claims.Select(claim => claim.Type)];
                _logger.LogError("Error extracting unique_name claim for rate limiting (found {Count} matching claims). Falling back to remote IP address. Available claims: {Claims}", uniqueNameClaims.Count, string.Join(", ", claimTypes));
                key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            }
        }
        else
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            key = string.IsNullOrEmpty(ipAddress) ? "anonymous" : ipAddress;
        }

        return key;
    }
}
