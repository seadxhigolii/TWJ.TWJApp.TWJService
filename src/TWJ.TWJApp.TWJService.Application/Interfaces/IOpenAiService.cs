using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Domain.Enums;

namespace TWJ.TWJApp.TWJService.Application.Interfaces
{
    public interface IOpenAiService
    {
        Task<string> GenerateTagsAsync(string content, Guid blogPostId, CancellationToken cancellationToken);
        Task<BlogPostResponse> GenerateBlogPostAsync(BlogPostType postType, CancellationToken cancellationToken);

    }
}
