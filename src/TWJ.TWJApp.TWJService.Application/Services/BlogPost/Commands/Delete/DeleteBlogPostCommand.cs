
using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Delete
{
    public class DeleteBlogPostCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}