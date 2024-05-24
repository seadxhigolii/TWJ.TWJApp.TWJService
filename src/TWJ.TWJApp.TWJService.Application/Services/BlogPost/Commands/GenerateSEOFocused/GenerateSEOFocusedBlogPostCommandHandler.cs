using CsvHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.OpenAI;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Import;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.GenerateRandom
{
    public class GenerateSEOFocusedBlogPostCommandHandler : IRequestHandler<GenerateSEOFocuedBlogPostCommand, BlogPostResponse>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IOpenAiService _openAiService;
        private readonly string currentClassName = "";

        public GenerateSEOFocusedBlogPostCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper, IOpenAiService openAiService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            _openAiService = openAiService ?? throw new ArgumentNullException(nameof(openAiService));
            currentClassName = GetType().Name;
        }

        public async Task<BlogPostResponse> Handle(GenerateSEOFocuedBlogPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //var result = await _openAiService.GenerateSEOFocusedBlogPostAsync(cancellationToken);
                var result = new BlogPostResponse();

                var blogPost = new Domain.Entities.BlogPost
                {
                    Title = result.Title,
                    UserId = Guid.Parse("0b3020cf-c562-4ac6-82ee-342780ebfbee"),
                    Content = result.HtmlContent,
                    BlogPostCategoryId = result.BlogPostCategoryId,
                    BackLinkKeywords = result.BackLinkKeywords,
                    URL = result.URL,
                    ProductID = result.ProductId
                };

                await _context.BlogPosts.AddAsync(blogPost);
                await _context.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}
