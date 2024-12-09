using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Delete
{
    public class DeleteBlogPostCategoryCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}