using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetById
{
    public class GetBlogPostCategoryByIdQuery : IRequest<GetBlogPostCategoryByIdModel>
    {
        public Guid Id { get; set; }
    }
}