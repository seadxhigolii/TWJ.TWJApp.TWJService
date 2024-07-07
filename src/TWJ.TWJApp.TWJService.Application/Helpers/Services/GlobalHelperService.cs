using System;
using System.Linq;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Domain.Entities;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Drawing;

namespace TWJ.TWJApp.TWJService.Application.Helpers.Services
{
    public class GlobalHelperService : IGlobalHelperService
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GlobalHelperService(ITWJAppDbContext context, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> GetUserFromCache()
        {
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var userId = GetUserIdFromToken(token);

            var user = await _context.User.FindAsync(userId);

            if (user != null)
            {
                return user;
            }

            return null;
        }


        private Guid GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                throw new ArgumentException("Invalid token");
            }

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new ArgumentException("User ID claim not found or invalid in the token");
            }

            return userId;
        }



        public async Task<bool> Log(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            Log log = new Log()
            {
                Class = className,
                Method = methodName,
                Message = ex.Message,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return true;
        }

        public string TitleToUrlSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            var slug = title.ToLowerInvariant();
            slug = slug.Trim();
            slug = Regex.Replace(slug, @"[^a-z0-9\s]", "");
            slug = Regex.Replace(slug, @"\s+", "-");

            var sameURLBlogPost = _context.BlogPosts.Any(x => x.URL == slug);

            if (sameURLBlogPost)
            {
                int suffixNumber = 1;
                string baseSlug = slug;
                string newSlug = $"{baseSlug}-no-{suffixNumber}";

                var match = Regex.Match(slug, @"-no-(\d+)$");
                if (match.Success)
                {
                    suffixNumber = int.Parse(match.Groups[1].Value) + 1;
                    baseSlug = slug.Substring(0, slug.LastIndexOf("-no-"));
                    newSlug = $"{baseSlug}-no-{suffixNumber}";
                }

                while (_context.BlogPosts.Any(x => x.URL == newSlug))
                {
                    suffixNumber++;
                    newSlug = $"{baseSlug}-no-{suffixNumber}";
                }

                slug = newSlug;
            }

            return slug;
        }


        public string RemoveTextWithinSquareBrackets(string input)
        {
            string pattern = @"\[.*?\]";
            return Regex.Replace(input, pattern, "");
        }
        public int CalculateFontSize(string text, RectangleF rect)
        {
            float area = rect.Width * rect.Height;

            int baseFontSize = (int)Math.Sqrt(area / text.Length);

            int fontSize = (int)(baseFontSize * 0.6); // Adjustable

            return Math.Max(12, Math.Min(fontSize, 72));
        }
        public void DrawTextWithSpacing(Graphics graphics, string text, Font font, Brush brush, RectangleF rect, float letterSpacing, float spaceSpacing, StringFormat format)
        {
            SizeF textSize = graphics.MeasureString(text, font, int.MaxValue, format);
            float totalWidth = textSize.Width + (text.Length - 1) * letterSpacing;

            float startX = rect.X + (rect.Width - totalWidth) / 2;
            float y = rect.Y + (rect.Height - textSize.Height) / 2;

            for (int i = 0; i < text.Length; i++)
            {
                string character = text[i].ToString();
                graphics.DrawString(character, font, brush, startX, y, format);
                SizeF charSize = graphics.MeasureString(character, font, int.MaxValue, format);

                if (character == " ")
                {
                    startX += charSize.Width + spaceSpacing;
                }
                else
                {
                    startX += charSize.Width + letterSpacing;
                }
            }
        }
    }
}
