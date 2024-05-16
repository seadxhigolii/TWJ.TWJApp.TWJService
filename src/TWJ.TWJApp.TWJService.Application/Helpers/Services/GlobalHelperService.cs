using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Domain.Entities;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace TWJ.TWJApp.TWJService.Application.Helpers.Services
{
    public class GlobalHelperService : IGlobalHelperService
    {
        private readonly ITWJAppDbContext _context;

        public GlobalHelperService(ITWJAppDbContext context)
        {
            _context = context;
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
    }
}
