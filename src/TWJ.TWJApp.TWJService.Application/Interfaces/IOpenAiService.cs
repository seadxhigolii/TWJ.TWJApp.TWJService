using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Generate;
using TWJ.TWJApp.TWJService.Domain.Enums;

namespace TWJ.TWJApp.TWJService.Application.Interfaces
{
    public interface IOpenAiService
    {
        //Task<string> GenerateBlogPostAsync(GenerateBlogPostCommand command, CancellationToken cancellationToken);

        Task<string> GenerateTagsAsync(string content, Guid blogPostId, CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateBlogPostAsync(BlogPostType postType, CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateSEOFocusedBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateMythBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateWeightLossBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateNutritionalAdviceBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateExerciseGuidesBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateRecipesAndMealPlansBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateNumberedBlogPostTitleAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateBrightsideBlogPostTitleAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateMindsetAndMotivationBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateLatestNewsBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateHealthyLifestyleBlogPostAsync(CancellationToken cancellationToken);
        Task<BlogPostResponse> GeneratePersonalStoryBlogPostAsync(CancellationToken cancellationToken);

    }
}
