using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Enums;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.GenerateRandom
{
    public class GenerateRandomBlogPostCommandHandler : IRequestHandler<GenerateRandomBlogPostCommand, BlogPostResponse>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IOpenAiService _openAiService;
        private readonly string currentClassName = "";

        public GenerateRandomBlogPostCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper, IOpenAiService openAiService, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            _openAiService = openAiService ?? throw new ArgumentNullException(nameof(openAiService));
            currentClassName = GetType().Name;
            _configuration = configuration;
        }

        public async Task<BlogPostResponse> Handle(GenerateRandomBlogPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _openAiService.GenerateBlogPostAsync(BlogPostType.LatestNews,cancellationToken);

                var blogPost = new Domain.Entities.BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = result.Title,
                    Content = result.HtmlContent,
                    UserId = Guid.Parse("0b3020cf-c562-4ac6-82ee-342780ebfbee"),
                    BlogPostCategoryId = result.BlogPostCategoryId,
                    BackLinkKeywords = result.BackLinkKeywords,
                    URL = result.URL,
                    ProductID = result.ProductId,
                    Image = result.Image,
                    Published = true
                };

                await _context.BlogPosts.AddAsync(blogPost, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                string tags = await _openAiService.GenerateTagsAsync(blogPost.Content, blogPost.Id, cancellationToken);
                blogPost.Tags = tags;
                _context.BlogPosts.Update(blogPost);
                await _context.SaveChangesAsync(cancellationToken);

                await ProcessKeywordsAndLinkToPost(blogPost.BackLinkKeywords, blogPost.Id, cancellationToken);
                await AddBacklinksToContent(blogPost.Content, blogPost.Id, cancellationToken);
                await UpdateTagPostCounts(tags, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        public async Task ProcessKeywordsAndLinkToPost(string keywordJson, Guid blogPostId, CancellationToken cancellationToken)
        {
            try
            {
                var keywordList = JsonConvert.DeserializeObject<List<KeywordScore>>(keywordJson);
                foreach (var keywordScore in keywordList)
                {
                    var seoKeyword = await _context.SEOKeywords
                                                   .FirstOrDefaultAsync(k => k.Keyword == keywordScore.Keyword, cancellationToken);
                    if (seoKeyword == null)
                    {
                        seoKeyword = new Domain.Entities.SEOKeyword { Id = Guid.NewGuid(), Keyword = keywordScore.Keyword, CategoryId = Guid.Parse("37159d28-d1b6-4e94-ba8b-11b9dbf06267") };
                        await _context.SEOKeywords.AddAsync(seoKeyword, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    var blogPostKeywordLink = new BlogPostSEOKeyword
                    {
                        BlogPostID = blogPostId,
                        SEOKeywordID = seoKeyword.Id,
                        Score = keywordScore.Score
                    };
                    await _context.BlogPostSEOKeywords.AddAsync(blogPostKeywordLink, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw;
            }            
        }

        public async Task AddBacklinksToContent(string content, Guid blogPostId, CancellationToken cancellationToken)
        {
            try
            {
                var keywordsWithUrls = await _context.BlogPostSEOKeywords
                    .Where(bpsk => bpsk.BlogPostID != blogPostId)
                    .Select(bpsk => new {
                        bpsk.SEOKeyword.Keyword,
                        bpsk.Score,
                        BlogPostUrl = bpsk.BlogPost.URL
                    })
                    .GroupBy(k => k.Keyword)
                    .Select(g => g.OrderByDescending(k => k.Score).FirstOrDefault())
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var headerMatches = new Dictionary<string, string>();
                var headerRegex = new Regex(@"<h[1-6].*?>.*?</h[1-6]>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                int counter = 0;
                content = headerRegex.Replace(content, match =>
                {
                    var key = $"@@HEADER{counter++}@@";
                    headerMatches[key] = match.Value;
                    return key;
                });

                var linkedKeywords = new HashSet<string>();

                foreach (var kw in keywordsWithUrls)
                {
                    if (!linkedKeywords.Contains(kw.Keyword.ToLower()))
                    {
                        var escapedKeyword = Regex.Escape(kw.Keyword);
                        var keywordRegex = new Regex($@"\b{escapedKeyword}\b", RegexOptions.IgnoreCase);
                        var replacement = $"<a class=\"backlink-cvzejg3k6w\" href=\"http://localhost:4200/#/post/{kw.BlogPostUrl}\">{kw.Keyword}</a>";
                        content = keywordRegex.Replace(content, replacement, 1);
                        linkedKeywords.Add(kw.Keyword.ToLower());
                    }
                }

                foreach (var match in headerMatches)
                {
                    content = content.Replace(match.Key, match.Value);
                }

                var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                if (blogPost != null)
                {
                    blogPost.Content = content;
                    _context.BlogPosts.Update(blogPost);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating blog post content: {ex.Message}");
                throw;
            }
        }


        private async Task UpdateTagPostCounts(string tagString, CancellationToken cancellationToken)
        {
            var tagNames = tagString.Split(',')
                           .Select(tag => tag.Trim())
                           .Where(tag => !string.IsNullOrEmpty(tag))
                           .Distinct();

            foreach (var tagName in tagNames)
            {
                var tag = await _context.Tag.FirstOrDefaultAsync(t => t.Name == tagName, cancellationToken);

                if (tag != null)
                {
                    tag.PostCount += 1;
                }
                else
                {
                    tag = new Domain.Entities.Tag { Name = tagName, PostCount = 1 };
                    await _context.Tag.AddAsync(tag, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
