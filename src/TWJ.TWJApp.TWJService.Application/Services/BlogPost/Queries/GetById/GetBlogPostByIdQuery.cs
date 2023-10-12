
using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetById
{
    public class GetBlogPostByIdQuery : IRequest<GetBlogPostByIdModel>
    {
        public Guid Id { get; set; }
    }
}