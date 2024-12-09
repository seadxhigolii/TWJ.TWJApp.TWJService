using TWJ.TWJApp.TWJService.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace TWJ.TWJApp.TWJService.Common.Helpers
{
    public static class AuthHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContext)
        {
            _httpContextAccessor = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        /// <summary>
        /// Returns Id of current user.
        /// </summary>
        public static Guid Id => _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == UserClaimEnum.UserId).Select(x => Guid.Parse(x.Value)).FirstOrDefault();

        /// <summary>
        /// Returns authenticated user token.
        /// </summary>
        public static string Token => GetRequiredToken();

        private static string GetRequiredToken()
        {
            HttpRequest request = _httpContextAccessor.HttpContext.Request;

            if (request.Path.StartsWithSegments(HubEnum.BaseUrl))
                return $"Bearer {request.Query["access_token"]}";
            else
                return request.Headers["Authorization"];
        }
    }
}
