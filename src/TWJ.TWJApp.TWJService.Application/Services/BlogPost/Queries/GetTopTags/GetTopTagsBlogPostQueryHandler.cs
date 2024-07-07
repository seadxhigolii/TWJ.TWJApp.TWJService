using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using System.Globalization;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetTopTags
{
    public class GetTopTagsBlogPostQueryHandler : IRequestHandler<GetTopTagsBlogPostQuery, CombinedBlogPostModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetTopTagsBlogPostQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<CombinedBlogPostModel> Handle(GetTopTagsBlogPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var latestPosts = await GetLatestBlogPosts(cancellationToken);
                var topTagsWithPosts = await GetTopTagsWithPosts(cancellationToken);

                return new CombinedBlogPostModel
                {
                    LatestPosts = latestPosts,
                    TopTagsWithPosts = topTagsWithPosts
                };
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }

        private async Task<IList<BlogPostDto>> GetLatestBlogPosts(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var threshold = now.Subtract(TimeSpan.FromDays(5));

            var data = await (from post in _context.BlogPosts
                              join user in _context.User on post.UserId equals user.Id
                              where post.Published && post.CreatedAt >= threshold
                              orderby post.CreatedAt descending
                              select new BlogPostDto
                              {
                                  Id = post.Id,
                                  Title = post.Title,
                                  AuthorName = user.FirstName + " " + user.LastName,
                                  CreatedAt = post.CreatedAt,
                                  URL = post.URL,
                                  ImageURL = post.Image,
                                  UserID = post.UserId
                              })
                              .AsNoTracking()
                              .Take(4)
                              .ToListAsync(cancellationToken);

            return data;
        }

        private async Task<IList<GetTopTagsBlogPostModel>> GetTopTagsWithPosts(CancellationToken cancellationToken)
        {
            var topTags = await _context.Tag
                                        .OrderByDescending(t => t.PostCount)
                                        .Take(4)
                                        .ToListAsync(cancellationToken);

            var result = new List<GetTopTagsBlogPostModel>();

            foreach (var tag in topTags)
            {
                var blogPostIds = await _context.BlogPostTags
                                                .Where(bpt => bpt.TagID == tag.Id && tag.PostCount > 0)
                                                .Select(bpt => bpt.BlogPostID)
                                                .ToListAsync(cancellationToken);

                var blogPosts = await (from post in _context.BlogPosts
                                       join user in _context.User on post.UserId equals user.Id
                                       where blogPostIds.Contains(post.Id) && post.Published
                                       orderby post.CreatedAt descending
                                       select new BlogPostDto
                                       {
                                           Id = post.Id,
                                           Title = post.Title,
                                           AuthorName = user.FirstName + " " + user.LastName,
                                           CreatedAt = post.CreatedAt,
                                           URL = post.URL,
                                           ImageURL = post.Image,
                                           UserID = post.UserId
                                       })
                                       .AsNoTracking()
                                       .Take(4)
                                       .ToListAsync(cancellationToken);

                result.Add(new GetTopTagsBlogPostModel
                {
                    TagName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tag.Name),
                    TagID = tag.Id, 
                    BlogPosts = blogPosts
                });
            }

            return result;
        }

    }
}
