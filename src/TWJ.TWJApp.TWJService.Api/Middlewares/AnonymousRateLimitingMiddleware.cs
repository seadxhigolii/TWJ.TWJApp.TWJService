using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace TWJ.TWJApp.TWJService.Api.Middlewares
{
    public class AnonymousRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        private const int RateLimit = 60; // Max requests per minute
        private const int TimeWindowInSeconds = 60; // Time window in seconds

        public AnonymousRateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;

            if (allowAnonymous)
            {
                var remoteIp = context.Connection.RemoteIpAddress?.ToString();
                var cacheEntry = Cache.GetOrCreate(remoteIp, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(TimeWindowInSeconds);
                    return new RateLimitInfo { RequestCount = 0 };
                });

                cacheEntry.RequestCount++;

                if (cacheEntry.RequestCount > RateLimit)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }

    public class RateLimitInfo
    {
        public int RequestCount { get; set; }
    }

}
